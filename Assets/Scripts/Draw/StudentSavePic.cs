using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
namespace jxzt
{
    /// <summary>
    /// 学生答案截图保存
    /// 将学生绘图内容截图保存为PNG文件
    /// 用于学生端提交作业时保存答案
    /// </summary>
    public class StudentSavePic : MonoBehaviour
    {
        [Tooltip("截图存储路径")]
        public string path;
        public bool capture;


        public RawImage rawImage;
        Texture2D mtexture;

        //保存图像信息，时间戳+像素RGB
        private struct ImageData
        {
            public long timestamp;
            public byte[] data;
        }
        // Start is called before the first frame update
        void Start()
        {
            path = Application.streamingAssetsPath + "/XueShengDaAnSave/";
        }

        // Update is called once per frame
        void Update()
        {
        }
        //按钮回调
        public void InputSave(string saveName = "")//RawImage rawImg
        {


            if (!capture)
            {
                capture = true;
                StartCoroutine(CaptureScreenshot2(saveName));
                Debug.Log("Successd!");


            }
        }

        private void ClosePop()
        {
            transform.GetChild(1).gameObject.SetActive(false);
        }

        public IEnumerator CaptureAndSave02(string saveName)
        {
            if (capture)
            {
                //等待屏幕渲染结束后获取屏幕像素信息
                yield return new WaitForEndOfFrame();
                mtexture = TextureToTexture2D(rawImage.texture); //![在这里插入图片描述](https://img-blog.csdnimg.cn/20210319150638808.gif)

                mtexture.Apply();
                //将图片信息编码为字节信息 
                ImageData pack;
                pack.data = mtexture.EncodeToPNG();
                //保存图片到 你的路径
                pack.timestamp = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                FileStream file = File.Create(path + saveName + ".png");//
                file.Write(pack.data, 0, pack.data.Length);
                file.Close();
                capture = false;
            }
        }

        /// <summary>
        /// 运行模式下Texture转换成Texture2D
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        private Texture2D TextureToTexture2D(Texture texture)
        {
            Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
            Graphics.Blit(texture, renderTexture);

            RenderTexture.active = renderTexture;
            texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture2D.Apply();

            RenderTexture.active = currentRT;
            RenderTexture.ReleaseTemporary(renderTexture);

            return texture2D;
        }
        /// <summary>
        /// Captures the screenshot2.
        /// </summary>
        /// <returns>The screenshot2.</returns>
        /// <param name="rect">Rect.截图的区域，左下角为o点</param>
        public IEnumerator CaptureScreenshot2(string saveName)//Rect rect,
        {
            ScreenCapture.CaptureScreenshot(Application.streamingAssetsPath + "/XueShengDaAnSave/" + saveName + ".png");
            yield return new WaitForSeconds(3);
            GameObject.Find("PaintBoard").transform.GetComponent<StudentMainManager>().ClearData();
            transform.GetChild(1).gameObject.SetActive(true);
            Invoke("ClosePop", 2);

        }
    }

}
