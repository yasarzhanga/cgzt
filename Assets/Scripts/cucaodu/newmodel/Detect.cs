using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;
using System.Linq;
using System;
using System.Text;

namespace jxzt
{
    /// <summary>
    /// YOLO 目标检测（Sentis版）
    /// 使用 Unity Sentis 运行 YOLO 模型进行图像识别
    /// 用于检测学生作业中的特定区域或标注
    /// </summary>
    public class RunYOLO : MonoBehaviour
    {
        public Unity.InferenceEngine.ModelAsset modelAsset;
        Unity.InferenceEngine.Model runtimeModel;
        Unity.InferenceEngine.Worker worker;
        Unity.InferenceEngine.BackendType backendType = Unity.InferenceEngine.BackendType.GPUCompute;
        //Image size for the model
        private const int imageWidth = 640;
        private const int imageHeight = 640;
        private const int YoloCandidateCount = 8400;
        private const int ClassCount = 6;
        private const float CandidateThreshold = 0.2f;
        private const float NmsThreshold = 0.45f;
        private const int DiagnosticTopCandidateCount = 12;
        private const int DiagnosticMaxBoxLogCount = 80;

        private int srcImageWidth;
        private int srcImageHeight;
        private RenderTexture targetRT;
        Unity.InferenceEngine.Tensor<float> inputTensor = null;


        public Texture2D texture2D;

        public List<List<PositionInt>> markPositionInts;
        public List<List<PositionInt>> markPositionJudgedInts;
        private void Start()
        {
        }
        public void ExecuteML()
        {
            using (ScoringPerf.Scope("YOLO.ModelLoad", $"model={(modelAsset == null ? "null" : modelAsset.name)};backend={backendType}"))
            {
                runtimeModel = Unity.InferenceEngine.ModelLoader.Load(modelAsset);
                worker = new Unity.InferenceEngine.Worker(runtimeModel, backendType);
            }

            // 获取 Texture2D 的 TextureFormat
            TextureFormat format2 = texture2D.format;
            Debug.Log("DetectTextureformat: " + format2);
            try
            {
                using (ScoringPerf.Scope("YOLO.DetectPic", $"texture={texture2D.width}x{texture2D.height};format={format2}"))
                {
                    DetectPic(texture2D);
                }
            }
            finally
            {
                worker.Dispose();
            }
        }
        public void DetectPic(Texture2D tex)
        {
            srcImageHeight = tex.height;
            srcImageWidth = tex.width;
            // 清理上一次的输入张量
            inputTensor?.Dispose();
            using (ScoringPerf.Scope("YOLO.ToTensor", $"source={srcImageWidth}x{srcImageHeight};target={imageWidth}x{imageHeight}"))
            {
                inputTensor = Unity.InferenceEngine.TextureConverter.ToTensor(tex, imageWidth, imageWidth, 3);
            }
            using (ScoringPerf.Scope("YOLO.Schedule", $"backend={backendType}"))
            {
                worker.Schedule(inputTensor);
            }

            // 注意：PeekOutput 返回的是由 worker 管理的张量，不要释放；
            // ReadbackAndClone 返回新的 CPU Tensor，使用后必须 Dispose。
            var output = worker.PeekOutput() as Unity.InferenceEngine.Tensor<float>;
            Unity.InferenceEngine.Tensor<float> result = null;
            try
            {
                using (ScoringPerf.Scope("YOLO.Readback", "output=0"))
                {
                    result = output.ReadbackAndClone();
                }
                using (ScoringPerf.Scope("YOLO.Postprocess", $"candidates={YoloCandidateCount};candidateThreshold={CandidateThreshold};nmsThreshold={NmsThreshold}"))
                {
                    List<BoundingBox> boxes = new List<BoundingBox>();
                    int[] rawClassCounts = new int[ClassCount + 1];
                    float[] rawClassMaxConfidence = new float[ClassCount + 1];
                    for (int i = 0; i < YoloCandidateCount; i++)
                    {


                        float[] confs = new float[ClassCount]
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

                        if (maxconfs > CandidateThreshold)
                        {

                            if (Class > 0 && Class <= ClassCount)
                            {
                                rawClassCounts[Class]++;
                                rawClassMaxConfidence[Class] = Mathf.Max(rawClassMaxConfidence[Class], maxconfs);
                            }
                            boxes.Add(new BoundingBox() { XMin = result[0, 0, i], XMax = result[0, 0, i] + result[0, 2, i], YMin = result[0, 1, i], YMax = result[0, 1, i] + result[0, 3, i], Confidence = maxconfs, Class = Class, RawIndex = i });
                        }

                    }

                    Debug.Log($"YOLO.RawCandidates total={boxes.Count} {FormatClassStats(rawClassCounts, rawClassMaxConfidence)}");
                    LogTopCandidates(boxes);

                    List<BoundingBox> boxres = NMS.NonMaxSuppression(boxes, NmsThreshold);//0.75
                    Debug.Log($"YOLO.NMS total={boxres.Count} {FormatClassStats(CountBoxesByClass(boxres), null)}");
                    
                    float displayWidth = TeacherMainManager.instance.width;
                    float displayHeight = TeacherMainManager.instance.height;
                    float scaleX = displayWidth / imageWidth;
                    float scaleY = displayHeight / imageHeight;
                    Debug.Log($"YOLO.Scale display={displayWidth:F0}x{displayHeight:F0};model={imageWidth}x{imageHeight};scale={scaleX:F4},{scaleY:F4}");
                    List<List<PositionInt>> positionInts = new List<List<PositionInt>>();
                    for (int i = 0; i < boxres.Count; i++)
                    {
                        List<PositionInt> positionInt = new List<PositionInt>();
                        
                        float boxWidth = boxres[i].XMax - boxres[i].XMin;
                        float boxHeight = boxres[i].YMax - boxres[i].YMin;
                        int left = (int)((boxres[i].XMin - boxWidth / 2) * scaleX);
                        int right = (int)((boxres[i].XMax - boxWidth / 2) * scaleX);
                        int top = (int)(displayHeight - (boxres[i].YMin - boxHeight / 2) * scaleY);
                        int bottom = (int)(displayHeight - (boxres[i].YMax - boxHeight / 2) * scaleY);
                        positionInt.Add(new PositionInt(left, top));
                        positionInt.Add(new PositionInt(left, bottom));
                        positionInt.Add(new PositionInt(right, bottom));
                        positionInt.Add(new PositionInt(right, top));
                       
                        positionInt.Add(new PositionInt((int)(boxres[i].Class), 0));//biaoshi
                        if (i < DiagnosticMaxBoxLogCount)
                        {
                            Debug.Log($"YOLO.Box index={i} {FormatBox(boxres[i], left, right, top, bottom)}");
                        }
                        positionInts.Add(positionInt);
                    }
                    if (boxres.Count > DiagnosticMaxBoxLogCount)
                    {
                        Debug.Log($"YOLO.Box omitted={boxres.Count - DiagnosticMaxBoxLogCount};logged={DiagnosticMaxBoxLogCount}");
                    }
                    markPositionInts = positionInts;
                    Debug.Log($"YOLO.MarkData total={markPositionInts.Count} {FormatMarkClassStats(markPositionInts)}");
                }
            }
            finally
            {
                // 释放 ReadbackAndClone 的 CPU Tensor，避免 CPUTensorData 泄漏
                if (result != null)
                {
                    result.Dispose();
                    result = null;
                }
                // 本次推理已完成，可以安全释放输入张量
                if (inputTensor != null)
                {
                    inputTensor.Dispose();
                    inputTensor = null;
                }
            }
        }

        private static int[] CountBoxesByClass(List<BoundingBox> boxes)
        {
            int[] counts = new int[ClassCount + 1];
            for (int i = 0; i < boxes.Count; i++)
            {
                int classId = boxes[i].Class;
                if (classId > 0 && classId <= ClassCount)
                {
                    counts[classId]++;
                }
            }
            return counts;
        }

        private static string FormatClassStats(int[] counts, float[] maxConfidence)
        {
            StringBuilder builder = new StringBuilder("classes=");
            for (int classId = 1; classId <= ClassCount; classId++)
            {
                if (classId > 1)
                {
                    builder.Append(",");
                }
                builder.Append(classId).Append(":").Append(counts[classId]);
                if (maxConfidence != null)
                {
                    builder.Append("@").Append(maxConfidence[classId].ToString("F3"));
                }
            }
            return builder.ToString();
        }

        private static string FormatMarkClassStats(List<List<PositionInt>> markData)
        {
            int[] counts = new int[ClassCount + 1];
            for (int i = 0; i < markData.Count; i++)
            {
                if (markData[i] == null || markData[i].Count < 5)
                {
                    continue;
                }
                int classId = markData[i][4].x;
                if (classId > 0 && classId <= ClassCount)
                {
                    counts[classId]++;
                }
            }
            return FormatClassStats(counts, null);
        }

        private static void LogTopCandidates(List<BoundingBox> boxes)
        {
            int index = 0;
            foreach (BoundingBox box in boxes.OrderByDescending(item => item.Confidence).Take(DiagnosticTopCandidateCount))
            {
                Debug.Log($"YOLO.RawCandidateTop index={index} {FormatRawBox(box)}");
                index++;
            }
        }

        private static string FormatRawBox(BoundingBox box)
        {
            float width = box.XMax - box.XMin;
            float height = box.YMax - box.YMin;
            float left = box.XMin - width / 2;
            float top = box.YMin - height / 2;
            float right = box.XMin + width / 2;
            float bottom = box.YMin + height / 2;
            return $"rawIndex={box.RawIndex};class={box.Class};conf={box.Confidence:F3};center={box.XMin:F1},{box.YMin:F1};size={width:F1}x{height:F1};rect={left:F1},{top:F1},{right:F1},{bottom:F1}";
        }

        private static string FormatBox(BoundingBox box, int left, int right, int top, int bottom)
        {
            float width = box.XMax - box.XMin;
            float height = box.YMax - box.YMin;
            return $"{FormatRawBox(box)};scaledRect={left},{bottom},{right},{top};scaledSize={Mathf.Abs(right - left)}x{Mathf.Abs(top - bottom)}";
        }


        class BoundingBox
        {
            public float XMin { get; set; }
            public float YMin { get; set; }
            public float XMax { get; set; }
            public float YMax { get; set; }
            public float Confidence { get; set; }

            public int Class { get; set; }
            public int RawIndex { get; set; }
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

                // 根据置信度对边界框进行排序
                boxes = boxes.OrderByDescending(box => box.Confidence).ToList();

                while (boxes.Count > 0)
                {
                    // 选择具有最高置信度的边界框
                    BoundingBox topBox = boxes[0];
                    pickedBoxes.Add(topBox);
                    boxes.RemoveAt(0);

                    // 删除与所选框重叠面积大于阈值的其他框
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


        public List<List<PositionInt>> IsJudgmentZonePositionOk(List<List<PositionInt>> positionInts, List<PositionInt> JudgmentZonePositions)
        {
            List<List<PositionInt>> newPositionInts = new List<List<PositionInt>>();
            List<PositionInt> changedPosData = new List<PositionInt>();
            foreach (var item in JudgmentZonePositions)
            {
                changedPosData.Add(new PositionInt((int)((item.x)), (int)((item.y))));
                changedPosData.Add(new PositionInt((int)((item.x)), (int)((item.y))));
                changedPosData.Add(new PositionInt((int)((item.x)), (int)((item.y))));
                changedPosData.Add(new PositionInt((int)((item.x)), (int)((item.y))));
            }


            int standard_width_min = changedPosData[0].x;
            int standard_width_max = changedPosData[0].x;
            int standard_height_min = changedPosData[0].y;
            int standard_height_max = changedPosData[0].y;

            for (int i = 1; i < changedPosData.Count; i++)
            {
                if (changedPosData[i].x < standard_width_min)
                {
                    standard_width_min = changedPosData[i].x;
                }
                if (changedPosData[i].x > standard_width_max)
                {
                    standard_width_max = changedPosData[i].x;
                }
                if (changedPosData[i].y < standard_height_min)
                {
                    standard_height_min = changedPosData[i].y;
                }
                if (changedPosData[i].y > standard_height_max)
                {
                    standard_height_max = changedPosData[i].y;
                }
            }
            for (int s = 0; s < positionInts.Count; s++)
            {
                #region 遍历学生每一个标识的包围盒得到最大宽高并互相比对包围盒点是否在对方内部   
                int pointNums = 0;
                for (int i = 0; i < positionInts[s].Count; i++)
                {
                    if (positionInts[s][i].x >= standard_width_min && positionInts[s][i].x <= standard_width_max && positionInts[s][i].y >= standard_height_min && positionInts[s][i].y <= standard_height_max)
                    {

                        pointNums++;
                    }
                }
                if (pointNums > 1)
                {
                    newPositionInts.Add(positionInts[s]);
                }

                #endregion

            }
            return newPositionInts;
        }
    }
}
