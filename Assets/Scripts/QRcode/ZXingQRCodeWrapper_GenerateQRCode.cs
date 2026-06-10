using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.Common;
namespace jxzt
{
    /// <summary>
    /// 二维码生成测试脚本
    /// 使用 ZXing 库生成不同内容的二维码图片
    /// 支持彩色二维码和带图标二维码的生成
    /// </summary>
    public class ZXingQRCodeWrapper_GenerateQRCode : MonoBehaviour
    {

        public RawImage img1;
        public RawImage img2;
        public RawImage img3;
        public Texture2D icon;

        // Use this for initialization
        void Start()
        {
            //注意：这个宽高度大小256不要变(变的话，最好是倍数变化)。不然生成的信息可能不正确
            //256有可能是这个ZXingNet插件指定大小的绘制像素点数值
            img1.texture = GenerateQRImage1("Hello Wrold!", 512, 512);
            img2.texture = GenerateQRImageWithColor("I Love You!", 256, 256, Color.red);
            img3.texture = GenerateQRImageWithColorAndIcon("中间带图片的二维码图片", 256, 256, Color.blue, icon);

        }

        /// <summary>
        /// 生成2维码 方法一
        /// 经测试：只能生成256x256的
        /// </summary>
        /// <param name="content"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        Texture2D GenerateQRImage1(string content, int width, int height)
        {
            // 编码成color32
            EncodingOptions options = null;
            BarcodeWriter writer = new BarcodeWriter();
            options = new EncodingOptions
            {
                Width = width,
                Height = height,
                Margin = 1,
            };
            options.Hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");
            writer.Format = BarcodeFormat.QR_CODE;
            writer.Options = options;
            Color32[] colors = writer.Write(content);

            // 转成texture2d
            Texture2D texture = new Texture2D(width, height);
            texture.SetPixels32(colors);
            texture.Apply();


            return texture;
        }

        /// <summary>
        /// 生成2维码 方法二
        /// 经测试：能生成任意尺寸的正方形
        /// </summary>
        /// <param name="content"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        Texture2D GenerateQRImageWithColor(string content, int width, int height, Color color)
        {
            BitMatrix bitMatrix;
            Texture2D texture = GenerateQRImageWithColor(content, width, height, color, out bitMatrix);

            return texture;
        }

        /// <summary>
        /// 生成2维码 方法二
        /// 经测试：能生成任意尺寸的正方形
        /// </summary>
        /// <param name="content"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        Texture2D GenerateQRImageWithColor(string content, int width, int height, Color color, out BitMatrix bitMatrix)
        {
            // 编码成color32
            MultiFormatWriter writer = new MultiFormatWriter();
            Dictionary<EncodeHintType, object> hints = new Dictionary<EncodeHintType, object>();
            //设置字符串转换格式，确保字符串信息保持正确
            hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");
            // 设置二维码边缘留白宽度（值越大留白宽度大，二维码就减小）
            hints.Add(EncodeHintType.MARGIN, 1);
            hints.Add(EncodeHintType.ERROR_CORRECTION, ZXing.QrCode.Internal.ErrorCorrectionLevel.M);
            //实例化字符串绘制二维码工具
            bitMatrix = writer.encode(content, BarcodeFormat.QR_CODE, width, height, hints);

            // 转成texture2d
            int w = bitMatrix.Width;
            int h = bitMatrix.Height;
            print(string.Format("w={0},h={1}", w, h));
            Texture2D texture = new Texture2D(w, h);
            for (int x = 0; x < h; x++)
            {
                for (int y = 0; y < w; y++)
                {
                    if (bitMatrix[x, y])
                    {
                        texture.SetPixel(y, x, color);
                    }
                    else
                    {
                        texture.SetPixel(y, x, Color.white);
                    }
                }
            }
            texture.Apply();


            return texture;
        }

        /// <summary>
        /// 生成2维码 方法三
        /// 在方法二的基础上，添加小图标
        /// </summary>
        /// <param name="content"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        Texture2D GenerateQRImageWithColorAndIcon(string content, int width, int height, Color color, Texture2D centerIcon)
        {
            BitMatrix bitMatrix;
            Texture2D texture = GenerateQRImageWithColor(content, width, height, color, out bitMatrix);
            int w = bitMatrix.Width;
            int h = bitMatrix.Height;

            // 添加小图
            int halfWidth = texture.width / 2;
            int halfHeight = texture.height / 2;
            int halfWidthOfIcon = centerIcon.width / 2;
            int halfHeightOfIcon = centerIcon.height / 2;
            int centerOffsetX = 0;
            int centerOffsetY = 0;
            for (int x = 0; x < h; x++)
            {
                for (int y = 0; y < w; y++)
                {
                    centerOffsetX = x - halfWidth;
                    centerOffsetY = y - halfHeight;
                    if (Mathf.Abs(centerOffsetX) <= halfWidthOfIcon && Mathf.Abs(centerOffsetY) <= halfHeightOfIcon)
                    {
                        texture.SetPixel(x, y, centerIcon.GetPixel(centerOffsetX + halfWidthOfIcon, centerOffsetY + halfHeightOfIcon));
                    }
                }
            }
            texture.Apply();

            // 存储成文件
            byte[] bytes = texture.EncodeToPNG();
            string path = System.IO.Path.Combine(Application.dataPath, "qr.png");
            System.IO.File.WriteAllBytes(path, bytes);

            return texture;
        }


    }
}
