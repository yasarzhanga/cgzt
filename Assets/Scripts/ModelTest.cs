using jxzt;
using System.IO;
using UnityEngine;
using System.Collections;

using System.Collections.Generic;


using UnityEngine.UIElements;

using UnityEngine.Rendering;

using static UnityEngine.GraphicsBuffer;

using System.Linq;

using System;


public class ModelTest : MonoBehaviour
{
    public Unity.InferenceEngine.ModelAsset modelAsset;

    Unity.InferenceEngine.Model runtimeModel;

    Unity.InferenceEngine.Worker worker;

    Unity.InferenceEngine.BackendType backendType = Unity.InferenceEngine.BackendType.GPUCompute;

    //Image size for the model

    private const int imageWidth = 640;

    private const int imageHeight = 640;


    private int srcImageWidth;

    private int srcImageHeight;

    private RenderTexture targetRT;

    Unity.InferenceEngine.Tensor<float> inputTensor = null;


    public Texture2D texture2D;

    public string imagePath = Application.streamingAssetsPath + "/Images/" + "001.png";


    Texture2D ConvertTextureFormat(Texture2D originalTexture)

    {

        if (originalTexture == null)

        {

            Debug.LogError("Original texture is null.");

            return null;

        }


        // 创建一个新的 Texture2D 对象，尺寸与原始纹理相同，使用 RGBAHalf 格式

        Texture2D convertedTexture = new Texture2D(originalTexture.width, originalTexture.height, TextureFormat.RGBAHalf, false);


        // 获取原始纹理的像素数据

        Color[] originalPixels = originalTexture.GetPixels();

        Color[] convertedPixels = new Color[originalPixels.Length];


        // 进行像素数据的格式转换

        for (int i = 0; i < originalPixels.Length; i++)

        {

            Color originalColor = originalPixels[i];

            // 将 ARGB32 颜色值转换为 RGBAHalf 颜色值

            convertedPixels[i] = new Color(originalColor.r, originalColor.g, originalColor.b, originalColor.a);

        }


        // 将转换后的像素数据设置到新的 Texture2D 中

        convertedTexture.SetPixels(convertedPixels);

        convertedTexture.Apply();


        return convertedTexture;

    }

    Texture2D LoadTextureFromFileStream()

    {

        // 打开文件

        using (FileStream fileStream = new FileStream(Application.streamingAssetsPath + "/Images/" + "001.png", FileMode.Open, FileAccess.Read))

        {

            // 创建字节数组，大小为文件长度

            byte[] buffer = new byte[fileStream.Length];

            // 读取文件内容到字节数组

            fileStream.Read(buffer, 0, (int)fileStream.Length);

            // 创建 Texture2D 对象

            Texture2D texture = new(4360, 3040);

            // 从字节数组加载图像数据

            if (texture.LoadImage(buffer))

            {

                // 在这里可以对获取到的 texture 进行进一步的处理，比如将其赋给一个 RawImage 组件等

                Debug.Log("Texture loaded successfully.");

            }

            else

            {

                Debug.LogError("Failed to load texture from file.");

            }


            return ConvertTextureFormat(texture);

        }

    }

    private void Start()

    {

        runtimeModel = Unity.InferenceEngine.ModelLoader.Load(modelAsset);

        worker = new Unity.InferenceEngine.Worker(runtimeModel, backendType);


        DetectPic(LoadTextureFromFileStream());

    }


    public GameObject ImageBox;


    public Canvas canvas;


    public void DetectPic(Texture2D tex)

    {

        srcImageHeight = tex.height;

        srcImageWidth = tex.width;

        inputTensor?.Dispose();

        inputTensor = Unity.InferenceEngine.TextureConverter.ToTensor(tex, imageWidth, imageWidth, 3);

        worker.Schedule(inputTensor);

        var result = worker.PeekOutput() as Unity.InferenceEngine.Tensor<float>;


        result = result.ReadbackAndClone();

        List<BoundingBox> boxes = new List<BoundingBox>();

        for (int i = 0; i < 8400; i++)
        {


            float[] confs = new float[6]

            {

                result[0, 4, i],result[0, 5, i],result[0, 6, i],result[0, 7, i],result[0, 8, i],result[0, 9, i]

            };

            var maxconfs = Mathf.Max(confs);

            int Class = -1;

            for (int j = 0; j < confs.Length; j++)
            {

                if (confs[j] == maxconfs)

                {


                    Class = j + 1; break;

                }

            }


            if (maxconfs > 0.5)

            {

                boxes.Add(new BoundingBox() { XMin = result[0, 0, i], XMax = result[0, 0, i] + result[0, 2, i], YMin = result[0, 1, i], YMax = result[0, 1, i] + result[0, 3, i], Confidence = maxconfs, Class = Class });

            }


        }

        List<BoundingBox> boxres = NMS.NonMaxSuppression(boxes, 0.75f);

        foreach (BoundingBox box in boxres)

        {

            box.DrawBox(GameObject.Instantiate(ImageBox, canvas.transform));

        }


    }


    class BoundingBox

    {

        public float XMin { get; set; }

        public float YMin { get; set; }

        public float XMax { get; set; }

        public float YMax { get; set; }

        public float Confidence { get; set; }


        public int Class { get; set; }


        public void DrawBox(GameObject box)

        {

            RectTransform rect = box.GetComponent<RectTransform>();

            rect.anchorMin = Vector2.zero;

            rect.anchorMax = Vector2.zero;

            rect.sizeDelta = new Vector2((XMax - XMin) / imageWidth * 1920, (YMax - YMin) / imageHeight * 1080);

            rect.anchoredPosition = new Vector2(XMin / imageWidth * 1920, 1080 - YMin / imageHeight * 1080) - 0.5f * rect.sizeDelta;

        }

    }


    class NMS

    {

        public static float IntersectionOverUnion(BoundingBox box1, BoundingBox box2)

        {

            float x1 = Math.Max(box1.XMin, box2.XMin);

            float y1 = Math.Max(box1.YMin, box2.YMin);

            float x2 = Math.Min(box1.XMax, box2.XMax);

            float y2 = Math.Min(box1.YMax, box2.YMax);


            float intersectionArea = Math.Max(0, x2 - x1 + 1) * Math.Max(0, y2 - y1 + 1);


            float box1Area = (box1.XMax - box1.XMin + 1) * (box1.YMax - box1.YMin + 1);

            float box2Area = (box2.XMax - box2.XMin + 1) * (box2.YMax - box2.YMin + 1);


            float iou = intersectionArea / (box1Area + box2Area - intersectionArea);

            return iou;

        }


        public static List<BoundingBox> NonMaxSuppression(List<BoundingBox> boxes, float threshold)

        {

            List<BoundingBox> pickedBoxes = new List<BoundingBox>();


            // �������ŶȶԱ߽���������

            boxes = boxes.OrderByDescending(box => box.Confidence).ToList();


            while (boxes.Count > 0)

            {

                // ѡ�����������Ŷȵı߽��

                BoundingBox topBox = boxes[0];

                pickedBoxes.Add(topBox);

                boxes.RemoveAt(0);


                // ɾ������ѡ���ص����������ֵ��������

                List<BoundingBox> overlappingBoxes = new List<BoundingBox>();

                foreach (BoundingBox box in boxes)

                {

                    if (IntersectionOverUnion(topBox, box) > threshold)

                    {

                        overlappingBoxes.Add(box);

                    }

                }

                foreach (BoundingBox box in overlappingBoxes)

                {

                    boxes.Remove(box);

                }

            }


            return pickedBoxes;

        }

    }
}
