using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
namespace jxzt
{
    /// <summary>
    /// 加载图片题目
    /// 从URL异步加载题目图片，支持缩放、去黑边、二维码扫描等处理
    /// 用于教师端/学生端加载远程或本地的题目图片
    /// </summary>
    public class LoadImageTitle : MonoBehaviour
    {
        [SerializeField]
        public Sprite imageSprite;
        public Texture2D texture;
        public Texture2D textureChangeScale;
        public Texture2D textureRemovedBlackFrame;
        public Texture2D textureQRScan;
        public Texture imagetexture;
        public bool isCoroutineRunning = false;
        public byte[] saveImgByte;

        public IEnumerator LoadTexture2D(string url)
        {
            isCoroutineRunning = true;
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();

            if (request.isHttpError || request.isNetworkError)
            {
                Debug.Log("图片为空");
            }
            else
            {
                texture = DownloadHandlerTexture.GetContent(request) as Texture2D;
                if (texture)
                {
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    if (transform.childCount != 0)
                    {
                        if (transform.GetChild(0).GetComponent<Image>())
                        {
                            transform.GetChild(0).GetComponent<Image>().sprite = sprite;
                        }
                    }
                    Debug.Log("图片已导入");
                    isCoroutineRunning = false;
                }

            }

        }
        public Texture2D LoadTextureFromFileStream(string filePath)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, (int)fileStream.Length);
                // 使用小尺寸占位，LoadImage 会按图片自适应尺寸
                Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                tex.LoadImage(buffer, false);
                return ConvertTextureFormat(tex);
            }
        }

        public void LoadImage(string filePath, bool isChangeScale = false)
        {
            // 路径保护在外部已添加，此处按正常加载
            using (FileStream files = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                byte[] imgByte = new byte[files.Length];
                saveImgByte = imgByte;
                files.Read(imgByte, 0, imgByte.Length);
            }

            // 先用 2x2 占位，LoadImage 后再变为原图尺寸；使用 RGBA32 降低内存
            texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            texture.LoadImage(saveImgByte, false);

            if (isChangeScale)
            {
                textureChangeScale = ScaleTextureGPU(texture, TeacherMainManager.instance.width, TeacherMainManager.instance.height);
            }

            if (texture)
            {
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                imageSprite = sprite;
                Debug.Log("图片已导入");
            }
        }

        public Texture2D ConvertToRGBAFloat(Texture2D texture)
        {
            Texture2D rgbaTex = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
            rgbaTex.filterMode = texture.filterMode;
            rgbaTex.wrapMode = texture.wrapMode;
            Color[] pixels = texture.GetPixels();
            rgbaTex.SetPixels(pixels);
            rgbaTex.Apply();
            Destroy(texture);
            return rgbaTex;
        }
        public Texture2D ConvertTextureFormat(Texture2D originalTexture)
        {
            if (originalTexture == null)
            {
                Debug.LogError("Original texture is null.");
                return null;
            }
            // 统一转为 RGBA32，避免 Half 浪费内存
            Texture2D convertedTexture = new Texture2D(originalTexture.width, originalTexture.height, TextureFormat.RGBA32, false);
            Color[] originalPixels = originalTexture.GetPixels();
            convertedTexture.SetPixels(originalPixels);
            convertedTexture.Apply();
            return convertedTexture;
        }
        public Texture2D GetTextureRemoveBlackFrame(string filePath, string filePath1)
        {
            // 加载主图
            byte[] imgByte = File.ReadAllBytes(filePath);
            texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            texture.LoadImage(imgByte, false);

            // 加载二维码图
            byte[] imgByte1 = File.ReadAllBytes(filePath1);
            textureQRScan = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            textureQRScan.LoadImage(imgByte1, false);

            // GPU 缩放，避免大数组分配
            textureRemovedBlackFrame = Texture2DScaleTexture(texture, MainManager.instance.width, MainManager.instance.height, true);
            textureQRScan = ScaleTexture(textureQRScan, MainManager.instance.width, MainManager.instance.height);

            return textureRemovedBlackFrame;
        }

        public Texture2D ClipTexture(int clipX, int clipY, int clipWidth, int clipHeight)
        {
            textureRemovedBlackFrame = new Texture2D(clipWidth, clipHeight, TextureFormat.RGBA32, false);
            for (int x = 0; x < clipWidth; x++)
            {
                for (int y = 0; y < clipHeight; y++)
                {
                    textureRemovedBlackFrame.SetPixel(x, y, textureChangeScale.GetPixel(clipX + x, clipY + y));
                }
            }
            textureRemovedBlackFrame.Apply();
            return textureRemovedBlackFrame;
        }

        public Texture2D ClipTexture1(int clipX, int clipY, int clipWidth, int clipHeight)
        {
            Texture2D textureclip = new Texture2D(clipWidth, clipHeight, TextureFormat.RGBA32, false);
            for (int x = 0; x < clipWidth; x++)
            {
                for (int y = 0; y < clipHeight; y++)
                {
                    textureclip.SetPixel(x, y, textureChangeScale.GetPixel(clipX + x, clipY + y));
                }
            }
            textureclip.Apply();
            return textureclip;
        }
        public Texture2D ClipTexture2(Texture2D source)
        {
            int clipWidth = 3645;//3550
            int clipHeight = 2465;//2405
            int clipX = 355;//379
            int clipY = 317;//317

            if (source == null)
            {
                Debug.LogError("Original texture is null. Please assign a texture.");
                return source;
            }

            clipX = Mathf.Clamp(clipX, 0, source.width);
            clipY = Mathf.Clamp(clipY, 0, source.height);
            clipWidth = Mathf.Clamp(clipWidth, 0, source.width - clipX);
            clipHeight = Mathf.Clamp(clipHeight, 0, source.height - clipY);

            Color[] croppedPixels = source.GetPixels(clipX, clipY, clipWidth, clipHeight);
            Texture2D croppedTexture = new Texture2D(clipWidth, clipHeight, TextureFormat.RGBA32, false);
            Debug.Log("ClipTexture2source.format" + source.format);
            croppedTexture.SetPixels(croppedPixels);
            croppedTexture.Apply();
            textureRemovedBlackFrame = croppedTexture;
            return Texture2DScaleTexture(croppedTexture, MainManager.instance.width, MainManager.instance.height);
        }

        public Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
        {
            // 使用 GPU 缩放替代 CPU 大数组，降低内存占用
            textureChangeScale = ScaleTextureGPU(source, targetWidth, targetHeight);
            return textureChangeScale;
        }

        public Texture2D ScaleTextureForQR(Texture2D source, int targetWidth, int targetHeight)
        {
            source.filterMode = FilterMode.Point;
            source.wrapMode = TextureWrapMode.Clamp;
            source.Apply(true);
            textureChangeScale = ScaleTextureGPU(source, targetWidth, targetHeight);
            return textureChangeScale;
        }

        public Texture2D Texture2DScaleTexture(Texture2D originalTexture, int newWidth, int newHeight, bool isConvertTextureFormat = false)
        {
            var scaled = ScaleTextureGPU(originalTexture, newWidth, newHeight);
            if (isConvertTextureFormat)
            {
                return ConvertTextureFormat(scaled);
            }
            return scaled;
        }

        public Texture2D Texture2DScaleTexture1(Texture2D originalTexture, int newWidth, int newHeight, bool isConvertTextureFormat = false)
        {
            // 采用 GPU 路径
            var scaled = ScaleTextureGPU(originalTexture, newWidth, newHeight);
            return isConvertTextureFormat ? ConvertTextureFormat(scaled) : scaled;
        }

        // GPU 缩放：RenderTexture + ReadPixels
        private Texture2D ScaleTextureGPU(Texture source, int width, int height, TextureFormat format = TextureFormat.RGBA32)
        {
            if (source == null) return null;
            var prevFilter = FilterMode.Bilinear;
            if (source is Texture2D t2d)
            {
                prevFilter = t2d.filterMode;
                t2d.filterMode = FilterMode.Bilinear;
            }
            RenderTexture rt = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);
            var prev = RenderTexture.active;
            try
            {
                Graphics.Blit(source, rt);
                RenderTexture.active = rt;
                Texture2D result = new Texture2D(width, height, format, false);
                result.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);
                result.Apply(false, false);
                return result;
            }
            finally
            {
                RenderTexture.active = prev;
                RenderTexture.ReleaseTemporary(rt);
                if (source is Texture2D t2d2)
                {
                    t2d2.filterMode = prevFilter;
                }
            }
        }

        public string ExportPng(Texture2D source)
        {
            byte[] imgByte = source.EncodeToPNG();
            string savePath = Application.streamingAssetsPath + "/XueShengDaAn/" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + ".png";
            using (FileStream fileStream = new FileStream(savePath, FileMode.Create))
            {
                fileStream.Write(imgByte, 0, imgByte.Length);
            }
            return savePath;
        }
    }
}













