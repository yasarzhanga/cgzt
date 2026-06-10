using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace jxzt
{
    /// <summary>
    /// 屏幕截图控制器
    /// 将指定相机视角的内容截图保存为PNG文件
    /// 支持指定截图区域（宽高）和存储路径
    /// </summary>
    public class ScreScreenGrabCtrl : MonoBehaviour
    {
        //保存图像信息，时间戳+像素RGB
        private struct ImageData
        {
            public long timestamp;
            public byte[] data;
        }
        //tooltip在面板中变量注释提示
        [Tooltip("截图宽度px")]
        public int width;
        [Tooltip("截图高度py")]
        public int height;
        [Tooltip("截图存储路径")]
        public string path;
        public Camera cam;
        public bool capture;
        Texture2D mtexture;
        RenderTexture rt;
        Rect ret;
        void Start()
        {
            //如果没有找到camera，那么获得camera组件，该脚本要挂载到你的camera上
            if (cam == null)
            {
                cam = GetComponent<Camera>();
            }
        }
        //捕获屏幕与保存不要放在主线程中，放在协程或者单独开一个线程
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.C))
            {
                if (!capture)
                {
                    cam.enabled = true;
                    capture = true;
                    StartCoroutine(CaptureAndSave03());
                    Debug.Log("REC!");
                }
                else
                {
                    cam.enabled = false;
                    capture = false;
                    StopCoroutine(CaptureAndSave03());
                    Debug.Log("REC STOP!");
                }
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(CaptureByUI());

            }
        }

        private void GetTextureScreenGrab(int codeValue, string path, Texture2D texture2D)
        {
            //在获取到Texture之后（tex）
            // Encode texture into PNG
            byte[] bytes = texture2D.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
        }

        //第一种截图方式，直接用unity自带截图函数，全屏截图，不能改变图像尺寸
        public IEnumerator CaptureAndSave01()
        {
            while (capture)
            {
                ImageData pack;
                pack.timestamp = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                ScreenCapture.CaptureScreenshot(pack.timestamp + ".png");
                yield return new WaitForSeconds(1);
            }
        }
        //第二种截图方式,将屏幕 图像存储到一个texture2D中去，可以设定图片尺寸
        public IEnumerator CaptureAndSave02()
        {
            while (capture)
            {
                //等待屏幕渲染结束后获取屏幕像素信息
                yield return new WaitForEndOfFrame();
                //创建一个texture2D对象
                mtexture = new Texture2D(width, height, TextureFormat.RGB24, false);
                ret.width = width;
                ret.height = height;
                //读取屏幕像素信息存储为纹理数据
                mtexture.ReadPixels(ret, 0, 0);
                mtexture.Apply();
                //将图片信息编码为字节信息 
                ImageData pack;
                pack.data = mtexture.EncodeToPNG();
                //保存图片到 你的路径
                pack.timestamp = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                FileStream file = File.Create(pack.timestamp + ".png");
                file.Write(pack.data, 0, pack.data.Length);
                file.Close();
            }
        }
        //通过camera截取
        public IEnumerator CaptureAndSave03()
        {
            while (capture)
            {
                yield return new WaitForEndOfFrame();
                //创建一个rendertexture
                rt = new RenderTexture(width, height, 0);
                rt = (RenderTexture)Texture2DZhengShiTu;
                //将rt设置为相机的渲染目标
                cam.targetTexture = rt;
                //开始渲染
                cam.Render();
                //激活渲染贴纸读取信息
                RenderTexture.active = rt;
                mtexture = new Texture2D(width, height, TextureFormat.RGB24, false);
                ret.width = width;
                ret.height = height;
                //读取屏幕像素信息存储为纹理数据
                mtexture.ReadPixels(ret, 0, 0);
                mtexture.Apply();
                //释放相机，销毁渲染贴纸
                cam.targetTexture = null;
                RenderTexture.active = null;
                GameObject.Destroy(mtexture);
                //将图片信息编码为字节信息 
                ImageData pack;
                pack.data = mtexture.EncodeToPNG();
                //保存图片到 你的路径
                pack.timestamp = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                FileStream file = File.Create(path + "/" + pack.timestamp + ".png");
                file.Write(pack.data, 0, pack.data.Length);
                file.Close();
                capture = false;
            }
        }
        public RectTransform UIRectZhuTu;
        public RectTransform UIRectZhengShiTu;
        public RectTransform UIRectFuShiTu;
        public Texture Texture2DZhengShiTu;
        public IEnumerator CaptureByUI()
        {
            //等待帧画面渲染结束
            yield return new WaitForEndOfFrame();
            RectTransform UIRect = UIRectZhuTu;
            int width = (int)(UIRect.rect.width);
            int height = (int)(UIRect.rect.height);
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
            //左下角为原点（0, 0）
            float leftBtmX = UIRect.transform.position.x + UIRect.rect.xMin;
            float leftBtmY = UIRect.transform.position.y + UIRect.rect.yMin;
            //从屏幕读取像素, leftBtmX/leftBtnY 是读取的初始位置,width、height是读取像素的宽度和高度
            tex.ReadPixels(new Rect(leftBtmX, leftBtmY, width, height), 0, 0);
            //执行读取操作
            tex.Apply();
            byte[] bytes = tex.EncodeToPNG();
            //保存
            int codeValue = Random.Range(100000, 999999);
            string fileName = Application.dataPath + "/StreamingAssets/" + "zhutu" + codeValue + ".png";
            System.IO.File.WriteAllBytes(fileName, bytes);
            StartCoroutine(CaptureByUIZhengShiTu());
        }
        public IEnumerator CaptureByUIZhengShiTu()
        {
            GameObject.Find("视图").transform.Find("正视图背景").gameObject.SetActive(true);
            GameObject.Find("视图").transform.Find("俯视图背景").gameObject.SetActive(false);
            //等待帧画面渲染结束
            yield return new WaitForEndOfFrame();
            RectTransform UIRect = UIRectZhengShiTu;
            int width = (int)(UIRect.rect.width);
            int height = (int)(UIRect.rect.height);
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
            //左下角为原点（0, 0）
            float leftBtmX = UIRect.transform.position.x + UIRect.rect.xMin;
            float leftBtmY = UIRect.transform.position.y + UIRect.rect.yMin;
            //从屏幕读取像素, leftBtmX/leftBtnY 是读取的初始位置,width、height是读取像素的宽度和高度
            tex.ReadPixels(new Rect(leftBtmX, leftBtmY, width, height), 0, 0);
            //执行读取操作
            tex.Apply();
            byte[] bytes = tex.EncodeToPNG();
            //保存
            int codeValue = Random.Range(100000, 999999);
            string fileName = Application.dataPath + "/StreamingAssets/" + "zhengshitu" + codeValue + ".png";
            System.IO.File.WriteAllBytes(fileName, bytes);
            StartCoroutine(CaptureByUIFuShiTu());
        }
        public IEnumerator CaptureByUIFuShiTu()
        {
            GameObject.Find("视图").transform.Find("正视图背景").gameObject.SetActive(false);
            GameObject.Find("视图").transform.Find("俯视图背景").gameObject.SetActive(true);
            //等待帧画面渲染结束
            yield return new WaitForEndOfFrame();
            RectTransform UIRect = UIRectFuShiTu;
            int width = (int)(UIRect.rect.width);
            int height = (int)(UIRect.rect.height);
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
            //左下角为原点（0, 0）
            float leftBtmX = UIRect.transform.position.x + UIRect.rect.xMin;
            float leftBtmY = UIRect.transform.position.y + UIRect.rect.yMin;
            //从屏幕读取像素, leftBtmX/leftBtnY 是读取的初始位置,width、height是读取像素的宽度和高度
            tex.ReadPixels(new Rect(leftBtmX, leftBtmY, width, height), 0, 0);
            //执行读取操作
            tex.Apply();
            byte[] bytes = tex.EncodeToPNG();
            //保存
            int codeValue = Random.Range(100000, 999999);
            string fileName = Application.dataPath + "/StreamingAssets/" + "fushitu" + codeValue + ".png";
            System.IO.File.WriteAllBytes(fileName, bytes);
        }
    }
}

