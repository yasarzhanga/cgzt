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
        public Texture2D mtexture;

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
        [Header("动态截图区域")]
        [SerializeField] private bool useDynamicCaptureRect = true;
        [SerializeField] private RectTransform captureRectTransform;
        [SerializeField] private Vector4 captureInset;
        private readonly Vector3[] captureWorldCorners = new Vector3[4];

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
                    LoadImageTitle loader = transform.GetComponent<LoadImageTitle>();
                    loader.LoadImage(pngPath);
                    bool usedDynamicClip = TryResolveCaptureClip(loader.texture.width, loader.texture.height, out int clipX, out int clipY, out int clipWidth, out int clipHeight);
                    using (ScoringPerf.Scope("Screenshot.LoadAndClip", $"clip={clipX},{clipY},{clipWidth},{clipHeight};dynamic={usedDynamicClip}"))
                    {
                        mtexture = ClipTexture(loader.texture, clipX, clipY, clipWidth, clipHeight);
                    }
                }
                else
                {
                    bool usedDynamicClip = TryResolveCaptureClip(Screen.width, Screen.height, out int clipX, out int clipY, out int clipWidth, out int clipHeight);
                    using (ScoringPerf.Scope("Screenshot.ReadPixelsClip", $"clip={clipX},{clipY},{clipWidth},{clipHeight};dynamic={usedDynamicClip}"))
                    {
                        mtexture = CaptureScreenClip(clipX, clipY, clipWidth, clipHeight);
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
            if (originalTexture == null) return null;
            ClampClipToSource(originalTexture.width, originalTexture.height, ref clipX, ref clipY, ref clipWidth, ref clipHeight);
            Texture2D clippedTexture = new Texture2D(clipWidth, clipHeight, TextureFormat.RGBA32, false);
            clippedTexture.SetPixels(originalTexture.GetPixels(clipX, clipY, clipWidth, clipHeight));
            clippedTexture.Apply(false, false);
            return clippedTexture;
        }

        private bool TryResolveCaptureClip(int sourceWidth, int sourceHeight, out int clipX, out int clipY, out int clipWidth, out int clipHeight)
        {
            bool usedDynamicRect = false;
            if (useDynamicCaptureRect && TryGetCaptureScreenRect(out Rect screenRect))
            {
                float scaleX = Screen.width > 0 ? sourceWidth / (float)Screen.width : 1f;
                float scaleY = Screen.height > 0 ? sourceHeight / (float)Screen.height : 1f;
                clipX = Mathf.FloorToInt(screenRect.xMin * scaleX);
                clipY = Mathf.FloorToInt(screenRect.yMin * scaleY);
                int right = Mathf.CeilToInt(screenRect.xMax * scaleX);
                int top = Mathf.CeilToInt(screenRect.yMax * scaleY);
                clipWidth = right - clipX;
                clipHeight = top - clipY;
                usedDynamicRect = true;
            }
            else
            {
                clipX = textureoffset_x;
                clipY = textureoffset_y;
                clipWidth = targettexture_w;
                clipHeight = targettexture_h;
            }

            ClampClipToSource(sourceWidth, sourceHeight, ref clipX, ref clipY, ref clipWidth, ref clipHeight);
            return usedDynamicRect;
        }

        private bool TryGetCaptureScreenRect(out Rect screenRect)
        {
            screenRect = Rect.zero;
            RectTransform targetRect = captureRectTransform;
            if (targetRect == null && rawImage != null)
            {
                targetRect = rawImage.rectTransform;
            }
            if (targetRect == null && TeacherMainManager.instance != null)
            {
                targetRect = TeacherMainManager.instance.GetComponent<RectTransform>();
            }
            if (targetRect == null)
            {
                return false;
            }

            targetRect.GetWorldCorners(captureWorldCorners);
            Canvas canvas = targetRect.GetComponentInParent<Canvas>();
            Camera uiCamera = null;
            if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                uiCamera = canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
            }

            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;
            for (int i = 0; i < captureWorldCorners.Length; i++)
            {
                Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(uiCamera, captureWorldCorners[i]);
                minX = Mathf.Min(minX, screenPoint.x);
                minY = Mathf.Min(minY, screenPoint.y);
                maxX = Mathf.Max(maxX, screenPoint.x);
                maxY = Mathf.Max(maxY, screenPoint.y);
            }

            minX += captureInset.x;
            minY += captureInset.y;
            maxX -= captureInset.z;
            maxY -= captureInset.w;
            minX = Mathf.Clamp(minX, 0f, Screen.width);
            minY = Mathf.Clamp(minY, 0f, Screen.height);
            maxX = Mathf.Clamp(maxX, 0f, Screen.width);
            maxY = Mathf.Clamp(maxY, 0f, Screen.height);
            if (maxX <= minX || maxY <= minY)
            {
                return false;
            }

            screenRect = Rect.MinMaxRect(minX, minY, maxX, maxY);
            return true;
        }

        private void ClampClipToSource(int sourceWidth, int sourceHeight, ref int clipX, ref int clipY, ref int clipWidth, ref int clipHeight)
        {
            sourceWidth = Mathf.Max(1, sourceWidth);
            sourceHeight = Mathf.Max(1, sourceHeight);
            clipX = Mathf.Clamp(clipX, 0, sourceWidth - 1);
            clipY = Mathf.Clamp(clipY, 0, sourceHeight - 1);
            clipWidth = Mathf.Clamp(clipWidth, 1, sourceWidth - clipX);
            clipHeight = Mathf.Clamp(clipHeight, 1, sourceHeight - clipY);
        }

        private Texture2D CaptureScreenClip(int clipX, int clipY, int clipWidth, int clipHeight)
        {
            ClampClipToSource(Screen.width, Screen.height, ref clipX, ref clipY, ref clipWidth, ref clipHeight);

            Texture2D clippedTexture = new Texture2D(clipWidth, clipHeight, TextureFormat.RGB24, false);
            clippedTexture.ReadPixels(new Rect(clipX, clipY, clipWidth, clipHeight), 0, 0, false);
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

