using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace jxzt
{
    /// <summary>
    /// 拍照相机控制器
    /// 打开摄像头获取实时画面，拍照后将图像保存到本地目录
    /// 用于拍摄学生作业照片
    /// </summary>
    public class TakePhotoCamera : MonoBehaviour
    {
        public static TakePhotoCamera instance;
        public byte[] imageTytes;
        //摄像头图像类，继承自texture
        public WebCamTexture tex;
        /// <summary>
        /// 图片保存路径
        /// </summary>
        public string picDirPath = "E:\\paizhao";
        public bool isReady;
        private void Awake()
        {
            instance = this;
            isReady = false;
            StartCoroutine(OpenCamera());
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        IEnumerator OpenCamera()
        {
            //等待用户允许访问
            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            //如果用户允许访问，开始获取图像        
            if (Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                // 监控第一次授权，是否获得到设备（因为很可能第一次授权了，但是获得不到设备，这里这样避免）
                // 多次 都没有获得设备，可能就是真没有摄像头，结束获取 camera
                int i = 0;
                while (WebCamTexture.devices.Length <= 0 && 1 < 300)
                {
                    yield return new WaitForEndOfFrame();
                    i++;
                }
                WebCamDevice[] devices = WebCamTexture.devices;//获取可用设备
                if (WebCamTexture.devices.Length <= 0)
                {
                    Debug.LogError("没有摄像头设备，请检查");
                }
                else
                {
                    //先获取设备
                    WebCamDevice[] device = WebCamTexture.devices;
                    string deviceName = device[0].name;
                    //然后获取图像
                    tex = new WebCamTexture(deviceName, MainManager.instance.width, MainManager.instance.height);
                    tex.Play();
                    isReady = true;
                    Debug.Log("拍照已准备");
                    PicProgress.instance.picProgress = 0;
                    UnityEngine.Debug.Log("picProgress" + 0);
                }
            }
        }

        public string TakingPhoto(string pictureType)
        {
            return Save(tex, pictureType);
        }

        public string Save(WebCamTexture t, string pictureType)
        {
            Texture2D t2d = new Texture2D(t.width, t.height, TextureFormat.ARGB32, true);
            //将WebCamTexture 的像素保存到texture2D中
            t2d.SetPixels(t.GetPixels());
            t2d.Apply();
            imageTytes = t2d.EncodeToJPG();
            var picPath = Application.streamingAssetsPath + "/PaiZhao/" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + ".jpg";
            File.WriteAllBytes(picPath, imageTytes);
            return picPath;
        }
        void StopCamera()
        {
            if (Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                //先获取设备
                WebCamDevice[] device = WebCamTexture.devices;
                string deviceName = device[0].name;
                isReady = false;
                tex.Stop();
            }
        }

        private void OnDisable()
        {
            StopCamera();
        }
    }
}
