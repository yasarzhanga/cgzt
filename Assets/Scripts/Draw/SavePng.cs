using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace jxzt
{
    /// <summary>
    /// 保存PNG截图
    /// 将画板内容截图保存为PNG文件，支持指定截图区域和存储路径
    /// 用于保存学生答案或教师标准答案
    /// </summary>
    public class SavePng : MonoBehaviour
    {
        [Tooltip("截图存储路径")]
        public string path;
        public bool capture;


        public RawImage rawImage;
      public  Texture2D mtexture;

        //保存图像信息，时间戳+像素RGB
        private struct ImageData
        {
            public long timestamp;
            public byte[] data;
        }
        public Texture2D saveTexture;
        public string savename;
        public int textureoffset_x = 306;
        public int textureoffset_y = 85;
        public int targettexture_w = 1308;
        public int targettexture_h = 910;

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
        public void InputSave(string saveName = "", bool saveToFile = true)//RawImage rawImg
        {


            if (!capture)
            {

                StartCoroutine(CaptureScreenshot2(saveName, saveToFile));
                capture = true;
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
                mtexture = TextureToTexture2D(rawImage.texture); //![在这里插入图片描述]

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
        public IEnumerator CaptureScreenshot2(string saveName, bool saveToFile = true)//Rect rect,
        {
            ScoringPerf.Start("Screenshot.Capture", $"name={saveName};mode={(saveToFile ? "file" : "memory")}");
            string perfResult = "ok";
            TeacherMainManager.instance.slider.transform.parent.gameObject.SetActive(false);
            var parent = TeacherMainManager.instance != null ? TeacherMainManager.instance.transform : null;
            List<int> tempIndexList = new List<int>();
            for (int i = 0; parent != null && i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                if (!child.gameObject.activeSelf) { 
                    child.gameObject.SetActive(true);
                    tempIndexList.Add(i);
                }
            }
            // 等待当前帧结束
            yield return new WaitForEndOfFrame();

            string pngPath = null;
            if (saveToFile)
            {
                string dir = Application.streamingAssetsPath + "/XueShengDaAnSave/";
                if (!Directory.Exists(dir)) {
                    Directory.CreateDirectory(dir);
                }

                pngPath = dir + saveName + ".png";
                ScoringPerf.Event("Screenshot.CaptureRequested", $"path={pngPath}");
                ScreenCapture.CaptureScreenshot(pngPath);
                TeacherMainManager.instance.slider.transform.parent.gameObject.SetActive(true);

                ScoringPerf.Start("Screenshot.WaitReadable", $"path={pngPath}");
                yield return StartCoroutine(WaitUntilFileReadable(pngPath));
                ScoringPerf.End("Screenshot.WaitReadable", $"path={pngPath}");
            }

            try {
                if (saveToFile)
                {
                    using (ScoringPerf.Scope("Screenshot.LoadAndClip", $"clip={textureoffset_x},{textureoffset_y},{targettexture_w},{targettexture_h}"))
                    {
                        transform.GetComponent<LoadImageTitle>().LoadImage(pngPath);
                        mtexture = ClipTexture(transform.GetComponent<LoadImageTitle>().texture, textureoffset_x, textureoffset_y, targettexture_w, targettexture_h);
                    }
                }
                else
                {
                    using (ScoringPerf.Scope("Screenshot.ReadPixelsClip", $"clip={textureoffset_x},{textureoffset_y},{targettexture_w},{targettexture_h}"))
                    {
                        mtexture = CaptureScreenClip(textureoffset_x, textureoffset_y, targettexture_w, targettexture_h);
                    }

                    LoadImageTitle loader = transform.GetComponent<LoadImageTitle>();
                    if (loader != null)
                    {
                        loader.texture = mtexture;
                        loader.saveImgByte = System.Array.Empty<byte>();
                    }
                    TeacherMainManager.instance.slider.transform.parent.gameObject.SetActive(true);
                }

                savename = saveName;
                StartCoroutine(TeacherMainManager.instance.AegisAnimation(7));
                UnityEngine.Debug.Log("picProgress" + 7 + "执行判分截图");
            }
            catch (System.Exception ex) {
                perfResult = ex is IOException ? "ioError" : "error";
                Debug.LogError("[SavePng] 读取截图失败: " + ex.Message + " path=" + pngPath);
                TeacherMainManager.instance.ErrorTipsClear("截图文件尚未释放，请重试", false);
            }
            finally {
                TeacherMainManager.instance.slider.transform.parent.gameObject.SetActive(true);
                capture = false;
                ScoringPerf.End("Screenshot.Capture", $"result={perfResult};name={saveName};mode={(saveToFile ? "file" : "memory")}");
            }

        }
        public void CaptureScreenshot(string saveName)//Rect rect,
        {
            ScreenCapture.CaptureScreenshot(Application.streamingAssetsPath + "/XueShengDaAnSave/" + saveName + ".png");
            Debug.Log("学生答案已截图");
            GameObject.Find("PaintBoard").transform.GetComponent<TeacherMainManager>().ClearData();
            transform.GetChild(1).gameObject.SetActive(true);

        }
        /// <summary>
        /// 裁剪texture尺寸
        /// </summary>
        /// <param name="originalTexture"></param>
        /// <param name="clipX"></param>
        /// <param name="clipY"></param>
        /// <param name="clipWidth"></param>
        /// <param name="clipHeight"></param>
        /// <returns></returns>
        Texture2D ClipTexture(Texture2D originalTexture, int clipX, int clipY, int clipWidth, int clipHeight)
        {
            Texture2D clippedTexture = new Texture2D(clipWidth, clipHeight);
            for (int x = 0; x < clipWidth; x++)
            {
                for (int y = 0; y < clipHeight; y++)
                {
                    clippedTexture.SetPixel(x, y, originalTexture.GetPixel(clipX + x, clipY + y));
                }
            }
            clippedTexture.Apply();
            return clippedTexture;
        }

        private Texture2D CaptureScreenClip(int clipX, int clipY, int clipWidth, int clipHeight)
        {
            int x = Mathf.Clamp(clipX, 0, Mathf.Max(0, Screen.width - 1));
            int y = Mathf.Clamp(clipY, 0, Mathf.Max(0, Screen.height - 1));
            int width = Mathf.Clamp(clipWidth, 1, Mathf.Max(1, Screen.width - x));
            int height = Mathf.Clamp(clipHeight, 1, Mathf.Max(1, Screen.height - y));

            Texture2D clippedTexture = new Texture2D(width, height, TextureFormat.RGB24, false);
            clippedTexture.ReadPixels(new Rect(x, y, width, height), 0, 0, false);
            clippedTexture.Apply(false, false);
            return clippedTexture;
        }

        private IEnumerator WaitUntilFileReadable(string filePath, float timeoutSeconds = 8f, float retryIntervalSeconds = 0.05f) {
            float start = Time.realtimeSinceStartup;

            while (Time.realtimeSinceStartup - start < timeoutSeconds) {
                if (File.Exists(filePath)) {
                    FileStream stream = null;
                    try {
                        stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        if (stream.Length > 0) {
                            yield break; // 文件可读且有内容
                        }
                    }
                    catch (IOException) {
                        // 正在被写入，继续重试
                    }
                    finally {
                        if (stream != null) {
                            stream.Dispose();
                        }
                    }
                }

                yield return new WaitForSecondsRealtime(retryIntervalSeconds);
            }

            Debug.LogError("[SavePng] 等待截图文件可读超时: " + filePath);
        }
    }
}

