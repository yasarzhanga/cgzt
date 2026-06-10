using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;
namespace jxzt
{
    /// <summary>
    /// 答案检查/判分管理器
    /// 核心判分逻辑：对比学生答案与标准答案，判断图线位置、线型、尺寸等是否正确
    /// 支持多种错误类型检测：线型错误、位置偏移、长度异常、剖面线错误、尺寸标注错误等
    /// </summary>
    public class AnswerCheck : MonoBehaviour
    {
        public StudentLayerManager studentlayer_manager;
        public List<LayerManager> standardlayer_manager;
        public string str_ErrorPoints;
        public static float errorPosOffSet_x = 306;//571
        public static float errorPosOffSet_y = 84;//126
        string biaoshiError;
        public int markCount = 0;
        public float defaultErrorLimit = 0.27f;
        public float defaultRightLimit = 0.35f;
        public Dictionary<string, List<ErrorLimit>> errorLimitDic;


        // 调试用：记录当前一次判分的标准/学生包围盒
        public bool debugDrawMarkBounds = true;

        private Rect _debugStandardRect;
        private Rect _debugStudentRect;

        // ===== 新增：UI 调试框引用 =====
        [Header("调试包围盒（挂在 Canvas/UI/CameraCompare 下的 RectTransform）")]
        public RectTransform standardBoxRect;   // 红框
        public RectTransform studentBoxRect;    // 黄框

        // ========== 新增：分批处理进度回调 ==========
        /// <summary>
        /// 判分进度回调（Action: 当前索引, 总数, 当前图层信息）
        /// </summary>
        public Action<int, int, LayerManager> OnCheckProgress;
        /// <summary>
        /// 单条判分完成回调（Action: 图层管理器, 是否正确, 错误信息）
        /// </summary>
        public Action<LayerManager, bool, string> OnSingleCheckComplete;
        /// <summary>
        /// 全部判分完成回调（Action: 正确数, 总数）
        /// </summary>
        public Action<int, int> OnAllCheckComplete;
        /// <summary>
        /// 判分开始时间（用于计算耗时）
        /// </summary>
        private float _checkStartTime = 0f;
        /// <summary>
        /// 当前判分的正确数量
        /// </summary>
        private int _correctCount = 0;

        private LayerPixelCache _studentPixelCache;
        private List<PolylineGroup> _polylineGroups;
        private Dictionary<int, PolylineGroup> _polylineGroupByLayerNum;
        private Dictionary<int, Dictionary<int, PolylineSegmentResult>> _polylineResultCache;
        private const bool UseDistanceFieldNearestSearch = true;
        private const int MaxDistanceFieldArea = 6000000;
        private const int DistanceFieldCostMultiplier = 4;
        private const int StudentPixelTileSize = 128;
        private const float PatternGeometryLimit = 0.65f;
        private const float PatternMinLengthRatio = 0.78f;
        private const float PatternMaxLengthRatio = 1.18f;
        private const float PatternMaxExtraSpanRatio = 0.18f;


        private void Start() {
            errorLimitDic = LoadErrorLimitConfig();
        }

        /// <summary>
        /// 判分入口方法
        /// 遍历所有标准答案图层，统计标识数量，为后续逐条判分做准备
        /// </summary>
        public void Check()
        {
            bool isMark = false;//是否是标识层

            if (studentlayer_manager != null)
            {
                if (_studentPixelCache != null && !_studentPixelCache.IsValidFor(studentlayer_manager))
                {
                    _studentPixelCache = null;
                }

                markCount = 0;
                for (int i = 0; i < standardlayer_manager.Count; i++)
                {
                    if (standardlayer_manager[i].lineshape == lineshape.标识 && standardlayer_manager[i].lineType == linetype.unknown)
                    {
                        markCount++;
                    }
                }
                str_ErrorPoints = "";
                Debug.Log("先看判分数量" + standardlayer_manager.Count);

                // 记录判分开始时间
                _checkStartTime = Time.realtimeSinceStartup;
                _correctCount = 0;
                BuildPolylineGroups();


            }
        }

        public bool CheckEveryLineWithProgress(int markCount, LayerManager stand_answer, int currentIndex, int totalCount)
        {
            // 调用原有判分逻辑
            CheckEveryLine(markCount, stand_answer);

            // 触发进度回调
            OnCheckProgress?.Invoke(currentIndex, totalCount, stand_answer);

            // 判断是否正确并触发完成回调
            bool isCorrect = stand_answer.layerError == "正确";
            if (isCorrect)
            {
                _correctCount++;
            }
            OnSingleCheckComplete?.Invoke(stand_answer, isCorrect, stand_answer.layerError);

            // 如果是最后一条，触发完成回调
            if (currentIndex == totalCount - 1)
            {
                float totalTime = Time.realtimeSinceStartup - _checkStartTime;
                Debug.Log($"[判分完成] 总计: {totalCount}, 正确: {_correctCount}, 耗时: {totalTime:F2}秒");
                OnAllCheckComplete?.Invoke(_correctCount, totalCount);
            }

            return isCorrect;
        }

        /// <summary>
        /// 获取预估剩余判分时间（新增）
        /// 根据已完成数量和耗时估算剩余时间
        /// </summary>
        /// <param name="completedCount">已完成数量</param>
        /// <param name="totalCount">总数量</param>
        /// <returns>预估剩余秒数</returns>
        public float GetEstimatedRemainingTime(int completedCount, int totalCount)
        {
            if (completedCount <= 0 || totalCount <= 0)
                return 0f;

            float elapsed = Time.realtimeSinceStartup - _checkStartTime;
            float avgTimePerItem = elapsed / completedCount;
            int remainingCount = totalCount - completedCount;
            return avgTimePerItem * remainingCount;
        }

        /// <summary>
        /// 逐条判分：对比单条标准答案与学生答案
        /// 调用 Compare 方法进行几何匹配，根据返回结果设置错误类型（正确/线型错误/位置偏移等）
        /// </summary>
        /// <param name="markCount">标识数量</param>
        /// <param name="stand_answer">标准答案图层管理器</param>
        public void CheckEveryLine(int markCount, LayerManager stand_answer)
        {
            string errorReson;
            if (TryGetPolylineSegmentResult(stand_answer, out PolylineSegmentResult polylineResult))
            {
                stand_answer.displayError_position = polylineResult.ErrorPosition;
                stand_answer.isRight = polylineResult.Error == ErrorReson.正确;
                if (studentlayer_manager != null)
                {
                    studentlayer_manager.lineType = polylineResult.StudentLineType;
                }
                errorReson = polylineResult.Error.ToString();
            }
            else
            {
                errorReson = Compare(studentlayer_manager, stand_answer, markCount);//0-1
            }
            // 若返回空或无法解析，设置为默认错误（图线不在或偏离正确位置）避免 Enum.Parse 异常
            if (string.IsNullOrEmpty(errorReson))
            {
                errorReson = ErrorReson.图线不在或偏离正确位置.ToString();
            }
            if (!Enum.TryParse<ErrorReson>(errorReson, out var errorEnum))
            {
                Debug.LogWarning("[AnswerCheck] 无法解析错误枚举: " + errorReson + ", 自动置为正确");
                errorEnum = ErrorReson.正确; // 解析失败时按正确处理，避免影响后续逻辑
            }

            if (ScoringPerf.VerboseLayerLogs)
            {
                Debug.Log("判分：" + errorEnum);
            }
            if (errorEnum != ErrorReson.正确)
            {
                EnsureStandardLayerPixels(stand_answer);
                stand_answer.UpdateTex();
            }
            switch (errorEnum)
            {
                case ErrorReson.正确:
                    str_ErrorPoints += "a";
                    stand_answer.layerError = ErrorReson.正确.ToString();
                    break;
                case ErrorReson.线型使用错误:
                    stand_answer.layerError = ErrorReson.线型使用错误.ToString() + "，应该画" + stand_answer.lineType + "画成了" + studentlayer_manager.lineType;
                    str_ErrorPoints += "b";
                    break;
                case ErrorReson.图线不在或偏离正确位置:
                    if (stand_answer.lineshape == lineshape.直线)
                        stand_answer.layerError = stand_answer.lineType.ToString() + "不在或偏离正确位置";
                    else
                        stand_answer.layerError = stand_answer.lineType.ToString() + stand_answer.lineshape.ToString() + "不在或偏离正确位置";
                    str_ErrorPoints += "c";
                    break;
                case ErrorReson.图线过长:
                    if (stand_answer.lineshape == lineshape.直线)
                        stand_answer.layerError = stand_answer.lineType.ToString() + "过长";
                    else
                        stand_answer.layerError = stand_answer.lineType.ToString() + stand_answer.lineshape.ToString() + "过长";
                    str_ErrorPoints += "d"; // 原代码有两个 case ?????? 分支，第二个追加 e，这里保留第一次出现的 d
                    break;
                case ErrorReson.图线过短:
                    if (stand_answer.lineshape == lineshape.直线)
                        stand_answer.layerError = stand_answer.lineType.ToString() + "过短";
                    else
                        stand_answer.layerError = stand_answer.lineType.ToString() + stand_answer.lineshape.ToString() + "过短";
                    str_ErrorPoints += "e";
                    break;
                case ErrorReson.剖面线方向绘制错误:
                    stand_answer.layerError = ErrorReson.剖面线方向绘制错误.ToString();
                    str_ErrorPoints += "f";
                    break;
                case ErrorReson.剖面线不是45度线:
                    stand_answer.layerError = ErrorReson.剖面线不是45度线.ToString();
                    str_ErrorPoints += "g";
                    break;
                case ErrorReson.剖面线间距大小不一:
                    stand_answer.layerError = ErrorReson.剖面线间距大小不一.ToString();
                    str_ErrorPoints += "h";
                    break;
                case ErrorReson.在标注位置未发现对应尺寸:
                    stand_answer.layerError = ErrorReson.在标注位置未发现对应尺寸.ToString();
                    str_ErrorPoints += "i";
                    break;
                case ErrorReson.尺寸绘制不标准格式错误:
                    stand_answer.layerError = ErrorReson.尺寸绘制不标准格式错误.ToString();
                    str_ErrorPoints += "j";
                    break;
                case ErrorReson.尺寸符号错误:
                    stand_answer.layerError = ErrorReson.尺寸符号错误.ToString();
                    str_ErrorPoints += "k";
                    break;
                case ErrorReson.尺寸数值错误:
                    stand_answer.layerError = ErrorReson.尺寸数值错误.ToString();
                    str_ErrorPoints += "l";
                    break;
                case ErrorReson.尺寸标注数值位置不对:
                    stand_answer.layerError = ErrorReson.尺寸标注数值位置不对.ToString();
                    str_ErrorPoints += "m";
                    break;
                case ErrorReson.标注了多余尺寸:
                    stand_answer.layerError = ErrorReson.标注了多余尺寸.ToString();
                    str_ErrorPoints += "n";
                    break;
                case ErrorReson.公差符号错用:
                    stand_answer.layerError = ErrorReson.公差符号错用.ToString();
                    str_ErrorPoints += "o";
                    break;
                case ErrorReson.未标注公差:
                    stand_answer.layerError = ErrorReson.未标注公差.ToString();
                    str_ErrorPoints += "p";
                    break;
                case ErrorReson.公差数字错误:
                    stand_answer.layerError = ErrorReson.公差数字错误.ToString();
                    str_ErrorPoints += "q";
                    break;
                case ErrorReson.公差格式错误:
                    stand_answer.layerError = ErrorReson.公差格式错误.ToString();
                    str_ErrorPoints += "r";
                    break;
                case ErrorReson.基准要素标识位置错误:
                    stand_answer.layerError = ErrorReson.基准要素标识位置错误.ToString();
                    str_ErrorPoints += "s";
                    break;
                case ErrorReson.基准要素标识未标识:
                    stand_answer.layerError = ErrorReson.基准要素标识未标识.ToString();
                    str_ErrorPoints += "t";
                    break;
                case ErrorReson.基准要素符号错误:
                    stand_answer.layerError = ErrorReson.基准要素符号错误.ToString();
                    str_ErrorPoints += "u";
                    break;
                case ErrorReson.剖面图标识未注写:
                    stand_answer.layerError = ErrorReson.剖面图标识未注写.ToString();
                    str_ErrorPoints += "v";
                    break;
                case ErrorReson.未填写技术要求:
                    stand_answer.layerError = ErrorReson.未填写技术要求.ToString();
                    str_ErrorPoints += "w";
                    break;
                case ErrorReson.回答错误:
                    stand_answer.layerError = ErrorReson.回答错误.ToString();
                    str_ErrorPoints += "x";
                    break;
                case ErrorReson.二维码或姓名学号:
                    stand_answer.layerError = ErrorReson.二维码或姓名学号.ToString();
                    break;
            }
            if (ScoringPerf.VerboseLayerLogs)
            {
                Debug.Log("判分：" + stand_answer.layerError);
                Debug.Log("错误点记录：" + str_ErrorPoints);
            }
            if (errorEnum != ErrorReson.正确)
            {
                stand_answer.score = 0;
            }
            else
            {
            }
        }

        private void EnsureStandardLayerPixels(LayerManager layer)
        {
            if (layer == null || layer.data == null || layer.Image_colors == null || layer.LayerSize == null)
            {
                return;
            }

            int width = layer.LayerSize.width;
            int height = layer.LayerSize.height;
            Color32[] colors = layer.Image_colors;
            Array.Clear(colors, 0, colors.Length);
            Color32 displayColor = new Color32(0, 85, 255, 255);
            foreach (var point in layer.data)
            {
                if (point == null || point.x < 0 || point.x >= width || point.y < 0 || point.y >= height)
                {
                    continue;
                }

                int index = point.x + point.y * width;
                if (index >= 0 && index < colors.Length)
                {
                    colors[index] = displayColor;
                }
            }
        }

        /// <summary>
        /// 对比学生答案与标准答案
        /// 根据图层类型（标识/文字/几何图线）分别进行匹配检测
        /// 返回 ErrorReson 枚举值表示判分结果
        /// </summary>
        /// <param name="layer">学生答案图层管理器</param>
        /// <param name="standardlayer">标准答案图层管理器</param>
        /// <param name="markCount">标识数量</param>
        /// <returns>错误类型枚举字符串</returns>
        public string Compare(LayerManager layer, LayerManager standardlayer, int markCount)
        {
            if (standardlayer == null)
            {
                return ErrorReson.图线不在或偏离正确位置.ToString();
            }

            if (standardlayer.lineshape == lineshape.标识 && standardlayer.lineType == linetype.unknown)
            {
                Debug.Log("标识判分" + standardlayer.layerNum);
                if (!HasValidMarkBox(standardlayer.markData))
                {
                    standardlayer.isRight = false;
                    return ErrorReson.在标注位置未发现对应尺寸.ToString();
                }

                ErrorReson markError_str = IsMarkPositionOk(layer?.markData, standardlayer.markData, (int)standardlayer.biaoshi, standardlayer.biaoshis);
                if (markError_str == ErrorReson.正确)
                {
                    standardlayer.isRight = true;
                    Vector2 standardrightPos = GetMidpoint(new Vector2(standardlayer.markData[0][0].x, standardlayer.markData[0][0].y), new Vector2(standardlayer.markData[0][2].x, standardlayer.markData[0][2].y));
                    standardlayer.displayError_position = standardrightPos;
                    Debug.Log("标识判分" + ErrorReson.正确.ToString());
                    return ErrorReson.正确.ToString();
                }
                else
                {
                    Vector2 standarderrorPos = GetMidpoint(new Vector2(standardlayer.markData[0][0].x, standardlayer.markData[0][0].y), new Vector2(standardlayer.markData[0][2].x, standardlayer.markData[0][2].y));
                    standardlayer.displayError_position = standarderrorPos;
                    standardlayer.isRight = false;

                    if (markError_str == ErrorReson.尺寸绘制不标准格式错误)
                    {
                        biaoshiError = ErrorReson.尺寸绘制不标准格式错误.ToString();
                        Debug.Log("标识判分" + biaoshiError);
                        return biaoshiError;
                    }

                    biaoshiError = "";
                    switch (standardlayer.biaoshi)
                    {
                        case 标识.尺寸:
                            biaoshiError = ErrorReson.在标注位置未发现对应尺寸.ToString();
                            break;
                        case 标识.粗糙度:
                            biaoshiError = ErrorReson.基准要素标识未标识.ToString();
                            break;
                        case 标识.弧度:
                            biaoshiError = ErrorReson.在标注位置未发现对应尺寸.ToString();
                            break;
                        case 标识.直径:
                            biaoshiError = ErrorReson.在标注位置未发现对应尺寸.ToString();
                            break;
                        case 标识.半径:
                            biaoshiError = ErrorReson.在标注位置未发现对应尺寸.ToString();
                            break;
                        case 标识.unknown:
                            biaoshiError = ErrorReson.在标注位置未发现对应尺寸.ToString();
                            break;
                    }
                    Debug.Log("标识判分" + biaoshiError);
                    return biaoshiError;
                }


            }
            else if (standardlayer.lineshape == lineshape.文字识别 && standardlayer.lineType == linetype.unknown)
            {
                if (standardlayer.ocr != 文字识别.姓名.ToString() && standardlayer.ocr != 文字识别.学号.ToString())
                {
                    List<int> nums = GameObject.Find("Script").transform.GetComponent<OCR>().IsOCRPositionOk(layer.OCRData, standardlayer.OCRData[0]);

                    if (nums.Count > 0)
                    {
                        string ocrstrMessage = "";
                        for (int i = 0; i < nums.Count; i++)
                        {
                            ocrstrMessage += GameObject.Find("Script").transform.GetComponent<OCR>().generalOcr.words_result[nums[i]].words;
                            Debug.Log("文字识别标准答案的信息" + standardlayer.OCRString);
                            Debug.Log("文字识别筛选到的信息" + ocrstrMessage);
                            if (standardlayer.OCRString.Contains("*#*#")) {
                                if (!string.IsNullOrEmpty(ocrstrMessage)) {
                                    standardlayer.isRight = true;
                                    return ErrorReson.正确.ToString();
                                }
                            }

                            //多答案可选判断
                            if (standardlayer.OCRString.Contains("#"))
                            {
                                string[] parts = standardlayer.OCRString.Split('#');
                                for (int p = 0; p < parts.Length; p++)
                                {

                                    if (AreAllCharactersInString(parts[p], ocrstrMessage))
                                    {
                                        Debug.Log("文字识别获取到的信息" + ocrstrMessage);
                                        standardlayer.isRight = true;
                                        return ErrorReson.正确.ToString();
                                    }
                                }
                            }
                            else
                            {
                                if (AreAllCharactersInString(standardlayer.OCRString, ocrstrMessage))
                                {
                                    Debug.Log("文字识别获取到的信息" + ocrstrMessage);
                                    standardlayer.isRight = true;
                                    return ErrorReson.正确.ToString();
                                }
                            }


                        }
                        Vector2 standarderrorPos = GetMidpoint(new Vector2(standardlayer.OCRData[0][0].x, standardlayer.OCRData[0][0].y), new Vector2(standardlayer.OCRData[0][2].x, standardlayer.OCRData[0][2].y));
                        standardlayer.displayError_position = standarderrorPos;
                        standardlayer.isRight = false;
                        biaoshiError = ErrorReson.回答错误.ToString();
                        if (standardlayer.OCRString.Contains("*#*#")) {
                            biaoshiError = ErrorReson.在标注位置未发现对应尺寸.ToString();
                        }
                        return biaoshiError;
                    }
                    else
                    {
                        Debug.Log("文字识别位置未获取信息");
                        Vector2 standarderrorPos = GetMidpoint(new Vector2(standardlayer.OCRData[0][0].x, standardlayer.OCRData[0][0].y), new Vector2(standardlayer.OCRData[0][2].x, standardlayer.OCRData[0][2].y));
                        standardlayer.displayError_position = standarderrorPos;
                        standardlayer.isRight = false;
                        biaoshiError = ErrorReson.回答错误.ToString();
                        if (standardlayer.OCRString.Contains("*#*#")) {
                            biaoshiError = ErrorReson.在标注位置未发现对应尺寸.ToString();
                        }
                        return biaoshiError;
                    }


                }
                else
                {
                    Debug.Log("姓名或学号不判分");
                    biaoshiError = ErrorReson.二维码或姓名学号.ToString();
                    return biaoshiError;
                }

            }           
            else if (standardlayer.lineType != linetype.unknown)           
            {
                LineMatchResult lineMatch = EvaluateLineMatch(layer, standardlayer);
                layer.lineType = lineMatch.StudentLineType;
                standardlayer.displayError_position = lineMatch.ErrorPosition;

                string lineMatchDetail = $"recall={lineMatch.Recall:F3};precision={lineMatch.Precision:F3};missing={lineMatch.MissingRatio:F3};extra={lineMatch.ExtraRatio:F3};p95={lineMatch.P95Distance:F1};bestOffset={lineMatch.BestOffset};studentLineType={lineMatch.StudentLineType};standardLineType={standardlayer.lineType};patterned={lineMatch.IsPatternedMode};geometry={lineMatch.GeometryPrecision:F3};macro={lineMatch.MacroCoverage:F3};macroLimit={lineMatch.MacroCoverageLimit:F3};span={lineMatch.PatternLengthRatio:F3};endpointMiss={lineMatch.PatternEndpointMiss:F1};endpointLimit={lineMatch.PatternEndpointLimit:F1};extraSpan={lineMatch.PatternExtraSpanRatio:F3};lengthRatio={lineMatch.ProjectedLengthRatio:F3}";
                ScoringPerf.LayerMatchMetric(standardlayer.layerNum, lineMatchDetail);
                if (ScoringPerf.VerboseLayerLogs)
                {
                    Debug.Log($"[AnswerCheck] 双向线条匹配 layerNum={standardlayer.layerNum}, {lineMatchDetail}");
                }

                if (!lineMatch.HasStandardPixels || !lineMatch.HasStudentPixels)
                {
                    standardlayer.isRight = false;
                    return ErrorReson.图线不在或偏离正确位置.ToString();
                }

                if (lineMatch.IsPatternedMode)
                {
                    ErrorReson patternedError = GetPatternedLineError(lineMatch, standardlayer.lineType);
                    if (patternedError == ErrorReson.图线过短)
                    {
                        standardlayer.displayError_position = lineMatch.MissingErrorPosition;
                    }
                    else if (patternedError == ErrorReson.图线过长)
                    {
                        standardlayer.displayError_position = lineMatch.ExtraErrorPosition;
                    }
                    else if (patternedError != ErrorReson.正确)
                    {
                        standardlayer.displayError_position = lineMatch.ErrorPosition;
                    }

                    standardlayer.isRight = patternedError == ErrorReson.正确;
                    return patternedError.ToString();
                }

                if (lineMatch.Recall < lineMatch.ErrorLimit && lineMatch.Precision < lineMatch.PrecisionLimit)
                {
                    standardlayer.isRight = false;
                    return ErrorReson.图线不在或偏离正确位置.ToString();
                }

                if (lineMatch.StudentLineType == linetype.unknown)
                {
                    standardlayer.isRight = false;
                    return ErrorReson.图线不在或偏离正确位置.ToString();
                }

                if (standardlayer.lineType != lineMatch.StudentLineType)
                {
                    standardlayer.isRight = false;
                    return ErrorReson.线型使用错误.ToString();
                }

                if (lineMatch.IsTooShort)
                {
                    if (IsCurvedLineShape(standardlayer.lineshape) && IsLineMatchRightEnough(lineMatch, standardlayer))
                    {
                        standardlayer.isRight = true;
                        return ErrorReson.正确.ToString();
                    }

                    standardlayer.displayError_position = lineMatch.MissingErrorPosition;
                    standardlayer.isRight = false;
                    return ErrorReson.图线过短.ToString();
                }

                if (lineMatch.IsTooLong)
                {
                    standardlayer.displayError_position = lineMatch.ExtraErrorPosition;
                    standardlayer.isRight = false;
                    return ErrorReson.图线过长.ToString();
                }

                if (IsLineMatchRightEnough(lineMatch, standardlayer))
                {
                    standardlayer.isRight = true;
                    return ErrorReson.正确.ToString();
                }

                standardlayer.isRight = false;
                if (IsCurvedLineShape(standardlayer.lineshape)
                    && lineMatch.MissingRatio > lineMatch.ExtraRatio
                    && lineMatch.MissingRatio < GetCurvedTooShortMissingLimit())
                {
                    return ErrorReson.图线不在或偏离正确位置.ToString();
                }

                return lineMatch.MissingRatio > lineMatch.ExtraRatio
                    ? ErrorReson.图线过短.ToString()
                    : ErrorReson.图线不在或偏离正确位置.ToString();

#if false
                ColorsWithIndex student = new ColorsWithIndex(layer.LayerSize.width, layer.LayerSize.height, layer.Image_colors);
                int n = 0;
                for (int i = 0; i < layer.Image_colors.Length; i++)
                {
                    if (layer.Image_colors[i].a != 0)
                    {
                        n++;
                    }
                }
                Debug.Log("managerimagecolors.counts4" + n);
                //将点绘制到imagecolor数组里
                if (standardlayer.data != null)
                {
                    standardlayer.LoadData(standardlayer.data);
                }

                ColorsWithIndex standard = new ColorsWithIndex(standardlayer.LayerSize.width, standardlayer.LayerSize.height, standardlayer.Image_colors);

                List<PositionInt> studenteffectpositions = student.GetPositionInts();
                List<PositionInt> standardeffectpositions = standard.GetPositionInts();

                float pengzhanglv = 1;

                int oldlength = standardeffectpositions.Count;

                for (int i = 0; i < 4; i++)
                {
                    standardeffectpositions = Tools.dilation(standardeffectpositions);
                }

                int nowlength = standardeffectpositions.Count;

                //膨胀率
                pengzhanglv = (float)nowlength / oldlength;

                List<PositionInt> standardframeSelectPos = Algorithm.GetOBB(standardeffectpositions);


                if (studenteffectpositions.Count != 0)
                {
                    float t = 0;
                    Vector2Int bestRect = new Vector2Int();
                    for (int i = -20; i <= 20; i += 2)//10
                    {
                        for (int j = -20; j <= 20; j += 2)//10
                        {
                            int checkedpointnum = 0;
                            foreach (var item in standardeffectpositions)
                            {
                                PositionInt checkpoint = item + i * PositionInt.up + j * PositionInt.right;
                                if (student[checkpoint.x, checkpoint.y].a > 0)
                                {
                                    checkedpointnum += 1;
                                }
                            }
                            float tmp = ((float)checkedpointnum / (float)standardeffectpositions.Count);//* pengzhanglv

                            if (t < tmp)
                            {

                                t = tmp;
                                bestRect.x = j; bestRect.y = i;
                            }
                        }
                    }
                    t *= pengzhanglv;


                    for (int i = 0; i < standardeffectpositions.Count; i++)
                    {
                        standardeffectpositions[i] += new PositionInt(bestRect.x, bestRect.y);
                    }

                    ColorsWithIndex wholeEffectStudentArea = new ColorsWithIndex(student.width, student.height);

                    foreach (var item in standardeffectpositions)
                    {
                        wholeEffectStudentArea[item.x, item.y] = student[item.x, item.y];
                    }

                    layer.lineType = LayerManager.CheckLineType(Algorithm.GetSlices(wholeEffectStudentArea));
                    List<PositionInt> studentframeSelectPos = Algorithm.GetOBB(wholeEffectStudentArea.GetPositionInts());
                    Vector2 studentframeerrorPos = Vector2.zero;
                    if (studentframeSelectPos.Count > 0)
                    {
                        studentframeerrorPos = GetMidpoint(new Vector2(studentframeSelectPos[0].x, studentframeSelectPos[0].y), new Vector2(studentframeSelectPos[2].x, studentframeSelectPos[2].y));
                    }


                    Debug.Log("bestRect:" + bestRect.ToString());
                    Debug.Log("compare" + t);
                    Debug.Log("studentlayer.lineType" + layer.lineType + "标准答案线段类型" + standardlayer.lineType + "标准答案序号" + standardlayer.layerNum);
                    if (layer.lineType== linetype.unknown)
                    {
                        if (standardlayer.lineshape == lineshape.二维码) {
                            Debug.Log("return ErrorReson.二维码或姓名学号.ToString() " + standardlayer.lineshape);
                            return ErrorReson.二维码或姓名学号.ToString();
                        }
                        else { 
                            Debug.Log("[AnswerCheck] 线型识别失败，判定为图线不在或偏离正确位置");
                            if (standardframeSelectPos.Count == 4) {
                                Vector2 standardframeerrorPos = GetMidpoint(new Vector2(standardframeSelectPos[0].x, standardframeSelectPos[0].y), new Vector2(standardframeSelectPos[2].x, standardframeSelectPos[2].y));
                                standardlayer.displayError_position = standardframeerrorPos;
                            }

                            standardlayer.isRight = false;
                            return ErrorReson.图线不在或偏离正确位置.ToString();

                        }
                    }

                    var errorLimit = defaultErrorLimit;
                    var rightLimit = defaultRightLimit;

                    //相似度判分
                    foreach (var i in errorLimitDic) {
                        if (i.Key == TeacherMainManager.instance.CurrentTitleIdForUpload) { 
                            foreach (var j in i.Value) {
                                if(j.index == standardlayer.layerNum) {
                                    errorLimit = j.errorLimit;
                                    rightLimit = j.rightLimit;
                                    Debug.Log($"使用配置表中的误差阈值: errorLimit={errorLimit}, rightLimit={rightLimit} for layerNum={standardlayer.layerNum}");
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    
                    if (t < errorLimit)//0.32f，0.27f
                    {
                        if (standardframeSelectPos.Count == 4)
                        {
                            Vector2 standardframeerrorPos = GetMidpoint(new Vector2(standardframeSelectPos[0].x, standardframeSelectPos[0].y), new Vector2(standardframeSelectPos[2].x, standardframeSelectPos[2].y));
                            standardlayer.displayError_position = standardframeerrorPos;
                        }

                        standardlayer.isRight = false;
                        return ErrorReson.图线不在或偏离正确位置.ToString();
                    }
                    else
                    {
                        if (standardlayer.lineType == layer.lineType)//&& standardlayer.lineshape == layer.lineshape
                        {
                            if (errorLimit < t && t < rightLimit)//0.32f < t && t < 0.5f#0.27f < t && t < 0.35f
                            {
                                List<PositionInt> lackSelectPos = CombineLists(standardframeSelectPos, studentframeSelectPos);
                                Vector2 lackframeerrorPos = Vector2.zero;
                                if (lackSelectPos.Count == 4)
                                {
                                    lackframeerrorPos = GetMidpoint(new Vector2(lackSelectPos[0].x, lackSelectPos[0].y), new Vector2(lackSelectPos[2].x, lackSelectPos[2].y));
                                }
                                else
                                {
                                    lackframeerrorPos = studentframeerrorPos;
                                }
                                standardlayer.displayError_position = lackframeerrorPos;
                                standardlayer.isRight = false;
                                return ErrorReson.图线过短.ToString();
                            }

                            if (t >= rightLimit)//t > 0.5f,0.35f
                            {
                                standardlayer.isRight = true;
                                return ErrorReson.正确.ToString();
                            }
                        }
                        else
                        {
                            standardlayer.displayError_position = studentframeerrorPos; 
                            standardlayer.isRight = false;
                            return ErrorReson.线型使用错误.ToString();
                        }
                    }

                    return "";


                }
                else
                {
                    Vector2 standardframeerrorPos = Vector2.zero;
                    if (standardframeSelectPos.Count == 4)
                    {
                        standardframeerrorPos = GetMidpoint(new Vector2(standardframeSelectPos[0].x, standardframeSelectPos[0].y), new Vector2(standardframeSelectPos[2].x, standardframeSelectPos[2].y));
                    }
                    standardlayer.displayError_position = standardframeerrorPos; 
                    standardlayer.isRight = false;
                    return ErrorReson.图线不在或偏离正确位置.ToString();
                }
#endif
            }
            else //if (standardlayer.lineshape == lineshape.二维码 || standardlayer.lineshape == lineshape.判分区域)
            {
                Debug.Log("return ErrorReson.二维码或姓名学号.ToString() " + standardlayer.lineshape );
                return ErrorReson.二维码或姓名学号.ToString();
            }
        }


        public bool DisplayErrorflog()
        {
            int n = 0;
            bool isAllRight = true;
            var parent = TeacherMainManager.instance != null ? TeacherMainManager.instance.transform : null;
            Dictionary<int, Transform> layerTransformByNum = null;
            if (parent != null)
            {
                layerTransformByNum = new Dictionary<int, Transform>();
                for (int ci = 0; ci < parent.childCount; ci++)
                {
                    var tlm = parent.GetChild(ci).GetComponent<TeacherLayerManager>();
                    if (tlm != null)
                    {
                        layerTransformByNum[tlm.layerNum] = parent.GetChild(ci);
                    }
                }

                foreach (var layerTf in layerTransformByNum.Values)
                {
                    if (layerTf != null && layerTf.childCount > 2)
                    {
                        layerTf.GetChild(2).gameObject.SetActive(false);
                    }
                }
            }

            foreach (var stand_answer in standardlayer_manager)
            {

                if (stand_answer.lineshape != lineshape.二维码 && stand_answer.lineshape != lineshape.判分区域 && stand_answer.ocr != 文字识别.姓名.ToString() && stand_answer.ocr != 文字识别.学号.ToString())
                {
                    n++;
                    if (!stand_answer.isRight)
                    {
                        isAllRight = false;
                    }

                    if (parent == null)
                    {
                        Debug.LogWarning("[DisplayErrorflog] TeacherMainManager.transform 为空");
                        continue;
                    }
                    // 查找与 layerNum 对应的标准层 Transform（不要直接用索引）
                    if (layerTransformByNum == null || !layerTransformByNum.TryGetValue(stand_answer.layerNum, out Transform layerTf))
                    {
                        Debug.LogWarning($"[DisplayErrorflog] 未找到匹配的标准层 Transform: layerNum={stand_answer.layerNum}");
                        continue;
                    }
                    int childCnt = layerTf.childCount;

                    if (stand_answer.isRight)
                    {
                        // 需要子节点0与1
                        if (childCnt >= 2)
                        {
                            stand_answer.displayError_position = new Vector2(stand_answer.displayError_position.x * 0.3f + errorPosOffSet_x, stand_answer.displayError_position.y * 0.3f + errorPosOffSet_y);
                            layerTf.GetChild(0).position = stand_answer.displayError_position;
                            layerTf.GetChild(0).gameObject.SetActive(false);
                            layerTf.GetChild(1).gameObject.SetActive(false);
                        }
                        else
                        {
                            Debug.LogWarning($"[DisplayErrorflog] 子节点不足(需要>=2) layerNum={stand_answer.layerNum}, childCnt={childCnt}");
                        }
                    }
                    else
                    {
                        if (stand_answer.lineshape == lineshape.文字识别)
                        {
                            // 仅针对自定义 OCR 给出提示圈
                            if ((文字识别)System.Enum.Parse(typeof(文字识别), stand_answer.ocr) == 文字识别.自定义)
                            {
                                if (childCnt >= 2)
                                {
                                    layerTf.GetChild(0).gameObject.SetActive(false);
                                    layerTf.GetChild(1).gameObject.SetActive(true);
                                    layerTf.GetChild(1).position = new Vector2(stand_answer.xuhaoPosX / 2, stand_answer.xuhaoPosY / 2);
                                    if (layerTf.GetChild(1).childCount > 1)
                                    {
                                        layerTf.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = n.ToString();
                                        layerTf.GetChild(1).GetChild(1).gameObject.SetActive(true);
                                        layerTf.GetChild(1).GetChild(1).GetComponent<TMP_Text>().text = stand_answer.OCRString;
                                    }
                                }
                                else
                                {
                                    Debug.LogWarning($"[DisplayErrorflog] 文字识别子节点不足(需要>=2) layerNum={stand_answer.layerNum}, childCnt={childCnt}");
                                }
                            }
                        }
                        else
                        {
                            if (childCnt >= 2)
                            {
                                layerTf.GetChild(0).gameObject.SetActive(true);// 显示错号/叉号
                                // 设置错号透明度为50%
                                var errorRawImage = layerTf.GetChild(0).GetComponent<UnityEngine.UI.RawImage>();
                                if (errorRawImage != null)
                                {
                                    Color c = errorRawImage.color;
                                    c.a = 0.1f;
                                    errorRawImage.color = c;
                                }
                                layerTf.GetChild(1).gameObject.SetActive(true);// 显示错误序号
                                layerTf.GetChild(1).position = new Vector2(stand_answer.xuhaoPosX / 2, stand_answer.xuhaoPosY / 2);
                                layerTf.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = n.ToString();// 标准层序号

                                // 设置错号位置为错误坐标
                                stand_answer.displayError_position = new Vector2(stand_answer.displayError_position.x * 0.3f + errorPosOffSet_x, stand_answer.displayError_position.y * 0.3f + errorPosOffSet_y);
                                layerTf.GetChild(0).position = stand_answer.displayError_position;
                            }
                            else
                            {
                                Debug.LogWarning($"[DisplayErrorflog] 普通层子节点不足(需要>=2) layerNum={stand_answer.layerNum}, childCnt={childCnt}");
                            }
                        }
                    }
                }
            }
            if (isAllRight && n > 0)
            {
                var firstValidLayer = standardlayer_manager.FirstOrDefault(l =>
                    l.lineshape != lineshape.二维码 &&
                    l.lineshape != lineshape.判分区域 &&
                    l.ocr != 文字识别.姓名.ToString() &&
                    l.ocr != 文字识别.学号.ToString()
                );

                if (firstValidLayer != null &&
                    layerTransformByNum != null &&
                    layerTransformByNum.TryGetValue(firstValidLayer.layerNum, out Transform okTf) &&
                    okTf != null &&
                    okTf.childCount > 2)
                {
                    okTf.GetChild(2).gameObject.SetActive(true);
                    Debug.Log($"[DisplayErrorflog] 整题全对，显示对号 layerNum={firstValidLayer.layerNum}");
                }
            }

            return isAllRight;
        }

        public Vector2 GetMidpoint(Vector2 a, Vector2 b)
        {
            return new Vector2((a.x + b.x) / 2, (a.y + b.y) / 2);
        }

        private struct PixelBounds
        {
            public int minX;
            public int maxX;
            public int minY;
            public int maxY;
        }

        private struct LineLimits
        {
            public float errorLimit;
            public float rightLimit;
            public float precisionLimit;
        }

        private struct LineMatchResult
        {
            public bool HasStandardPixels;
            public bool HasStudentPixels;
            public bool IsPatternedMode;
            public float Recall;
            public float Precision;
            public float MissingRatio;
            public float ExtraRatio;
            public float P95Distance;
            public float Tolerance;
            public float ErrorLimit;
            public float RightLimit;
            public float PrecisionLimit;
            public float GeometryPrecision;
            public float MacroCoverage;
            public float MacroCoverageLimit;
            public float PatternLengthRatio;
            public float PatternEndpointMiss;
            public float PatternEndpointLimit;
            public float PatternExtraSpanRatio;
            public float ProjectedLengthRatio;
            public Vector2Int BestOffset;
            public linetype StudentLineType;
            public Vector2 ErrorPosition;
            public Vector2 MissingErrorPosition;
            public Vector2 ExtraErrorPosition;
            public bool IsTooShort;
            public bool IsTooLong;
        }

        private struct ProjectionStats
        {
            public float Min;
            public float Max;
            public float Length;
        }

        private sealed class PolylineGroup
        {
            public int GroupId;
            public linetype LineType;
            public List<PolylineSegmentGuide> Segments = new List<PolylineSegmentGuide>();
            public List<PositionInt> StandardPixels = new List<PositionInt>();
            public float Tolerance;
        }

        private sealed class PolylineSegmentGuide
        {
            public LayerManager Layer;
            public List<PositionInt> Pixels;
            public Vector2 P0;
            public Vector2 P1;
            public Vector2 Axis;
            public float Length;
            public float Tolerance;
        }

        private sealed class PolylineSegmentResult
        {
            public ErrorReson Error;
            public Vector2 ErrorPosition;
            public linetype StudentLineType;
            public float Coverage;
            public float LengthRatio;
            public float ExtraRatio;
        }

        private sealed class LayerPixelCache
        {
            public LayerManager Layer;
            public Color32[] Source;
            public int Width;
            public int Height;
            public List<PositionInt> Pixels;
            public HashSet<long> PointSet;
            public int TileSize;
            public int TileColumns;
            public int TileRows;
            public List<PositionInt>[] TileBuckets;

            public bool IsValidFor(LayerManager layer)
            {
                return layer != null
                       && Layer == layer
                       && Source == layer.Image_colors
                       && layer.LayerSize != null
                       && Width == layer.LayerSize.width
                       && Height == layer.LayerSize.height;
            }

            public bool Contains(int x, int y)
            {
                return x >= 0 && x < Width && y >= 0 && y < Height && PointSet.Contains(PointKey(x, y));
            }

            public bool HasOpaquePixel(int x, int y)
            {
                return x >= 0
                       && x < Width
                       && y >= 0
                       && y < Height
                       && Source != null
                       && Source[x + y * Width].a > 0;
            }

            public List<PositionInt> GetPixelsInsideBounds(PixelBounds bounds)
            {
                if (Pixels == null || Pixels.Count == 0)
                {
                    return new List<PositionInt>();
                }

                if (TileBuckets == null || TileBuckets.Length == 0 || TileSize <= 0 || TileColumns <= 0 || TileRows <= 0)
                {
                    List<PositionInt> fallback = new List<PositionInt>();
                    foreach (var point in Pixels)
                    {
                        if (IsInsideBounds(point, bounds))
                        {
                            fallback.Add(point);
                        }
                    }
                    return fallback;
                }

                int minX = Mathf.Clamp(bounds.minX, 0, Width - 1);
                int maxX = Mathf.Clamp(bounds.maxX, 0, Width - 1);
                int minY = Mathf.Clamp(bounds.minY, 0, Height - 1);
                int maxY = Mathf.Clamp(bounds.maxY, 0, Height - 1);
                if (minX > maxX || minY > maxY)
                {
                    return new List<PositionInt>();
                }

                int minTileX = minX / TileSize;
                int maxTileX = maxX / TileSize;
                int minTileY = minY / TileSize;
                int maxTileY = maxY / TileSize;
                int estimatedCapacity = 0;
                for (int tileY = minTileY; tileY <= maxTileY; tileY++)
                {
                    int row = tileY * TileColumns;
                    for (int tileX = minTileX; tileX <= maxTileX; tileX++)
                    {
                        List<PositionInt> bucket = TileBuckets[row + tileX];
                        if (bucket != null)
                        {
                            estimatedCapacity += bucket.Count;
                        }
                    }
                }

                List<PositionInt> result = new List<PositionInt>(estimatedCapacity);
                for (int tileY = minTileY; tileY <= maxTileY; tileY++)
                {
                    int row = tileY * TileColumns;
                    for (int tileX = minTileX; tileX <= maxTileX; tileX++)
                    {
                        List<PositionInt> bucket = TileBuckets[row + tileX];
                        if (bucket == null)
                        {
                            continue;
                        }

                        foreach (var point in bucket)
                        {
                            if (IsInsideBounds(point, bounds))
                            {
                                result.Add(point);
                            }
                        }
                    }
                }
                return result;
            }
        }

        private sealed class DistanceField
        {
            private const int Infinity = 1 << 28;

            private readonly PixelBounds bounds;
            private readonly int width;
            private readonly int height;
            private readonly int[] distancesSq;

            private DistanceField(PixelBounds bounds, int width, int height, int[] distancesSq)
            {
                this.bounds = bounds;
                this.width = width;
                this.height = height;
                this.distancesSq = distancesSq;
            }

            public static DistanceField Build(List<PositionInt> targetPixels, PixelBounds bounds)
            {
                int width = GetBoundsWidth(bounds);
                int height = GetBoundsHeight(bounds);
                if (width <= 0 || height <= 0)
                {
                    return new DistanceField(bounds, 0, 0, new int[0]);
                }

                int[] grid = new int[width * height];
                for (int i = 0; i < grid.Length; i++)
                {
                    grid[i] = Infinity;
                }

                if (targetPixels != null)
                {
                    foreach (var point in targetPixels)
                    {
                        int x = point.x - bounds.minX;
                        int y = point.y - bounds.minY;
                        if (x >= 0 && x < width && y >= 0 && y < height)
                        {
                            grid[x + y * width] = 0;
                        }
                    }
                }

                int[] temp = new int[grid.Length];
                int[] distances = grid;
                int maxSide = Mathf.Max(width, height);
                int[] sourceLine = new int[maxSide];
                int[] resultLine = new int[maxSide];
                int[] locations = new int[maxSide];
                double[] boundaries = new double[maxSide + 1];

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        sourceLine[y] = grid[x + y * width];
                    }

                    DistanceTransform1D(sourceLine, height, resultLine, locations, boundaries);

                    for (int y = 0; y < height; y++)
                    {
                        temp[x + y * width] = resultLine[y];
                    }
                }

                for (int y = 0; y < height; y++)
                {
                    int row = y * width;
                    for (int x = 0; x < width; x++)
                    {
                        sourceLine[x] = temp[row + x];
                    }

                    DistanceTransform1D(sourceLine, width, resultLine, locations, boundaries);

                    for (int x = 0; x < width; x++)
                    {
                        distances[row + x] = resultLine[x];
                    }
                }

                return new DistanceField(bounds, width, height, distances);
            }

            public bool TryGetDistanceSq(PositionInt point, int maxDistanceSq, out int distanceSq)
            {
                distanceSq = maxDistanceSq + 1;
                if (width <= 0 || height <= 0 || distancesSq == null || distancesSq.Length == 0)
                {
                    return false;
                }

                int x = point.x - bounds.minX;
                int y = point.y - bounds.minY;
                if (x < 0 || x >= width || y < 0 || y >= height)
                {
                    return false;
                }

                distanceSq = distancesSq[x + y * width];
                return distanceSq <= maxDistanceSq;
            }

            private static void DistanceTransform1D(int[] values, int count, int[] output, int[] locations, double[] boundaries)
            {
                int segment = 0;

                locations[0] = 0;
                boundaries[0] = double.NegativeInfinity;
                boundaries[1] = double.PositiveInfinity;

                for (int q = 1; q < count; q++)
                {
                    double intersection;
                    do
                    {
                        int r = locations[segment];
                        intersection = (((double)values[q] + (double)q * q) - ((double)values[r] + (double)r * r)) / (2.0 * (q - r));
                        if (intersection <= boundaries[segment])
                        {
                            segment--;
                        }
                        else
                        {
                            break;
                        }
                    }
                    while (segment >= 0);

                    if (segment < 0)
                    {
                        segment = 0;
                        locations[0] = q;
                        boundaries[0] = double.NegativeInfinity;
                        boundaries[1] = double.PositiveInfinity;
                    }
                    else
                    {
                        segment++;
                        locations[segment] = q;
                        boundaries[segment] = intersection;
                        boundaries[segment + 1] = double.PositiveInfinity;
                    }
                }

                segment = 0;
                for (int q = 0; q < count; q++)
                {
                    while (boundaries[segment + 1] < q)
                    {
                        segment++;
                    }

                    int r = locations[segment];
                    int diff = q - r;
                    long distance = (long)diff * diff + values[r];
                    output[q] = distance >= Infinity ? Infinity : (int)distance;
                }
            }
        }

        private void BuildPolylineGroups()
        {
            _polylineGroups = new List<PolylineGroup>();
            _polylineGroupByLayerNum = new Dictionary<int, PolylineGroup>();
            _polylineResultCache = new Dictionary<int, Dictionary<int, PolylineSegmentResult>>();

            if (standardlayer_manager == null || standardlayer_manager.Count < 2)
            {
                return;
            }

            List<PolylineSegmentGuide> guides = new List<PolylineSegmentGuide>();
            foreach (var layer in standardlayer_manager)
            {
                if (!CanUsePolylineLayer(layer))
                {
                    continue;
                }

                List<PositionInt> pixels = GetStandardPixels(layer);
                if (pixels.Count < 2)
                {
                    continue;
                }

                if (TryCreatePolylineSegmentGuide(layer, pixels, out PolylineSegmentGuide guide))
                {
                    guides.Add(guide);
                }
            }

            if (guides.Count < 2)
            {
                return;
            }

            List<int>[] adjacency = new List<int>[guides.Count];
            for (int i = 0; i < adjacency.Length; i++)
            {
                adjacency[i] = new List<int>();
            }

            for (int i = 0; i < guides.Count; i++)
            {
                for (int j = i + 1; j < guides.Count; j++)
                {
                    if (ArePolylineSegmentsConnected(guides[i], guides[j]))
                    {
                        adjacency[i].Add(j);
                        adjacency[j].Add(i);
                    }
                }
            }

            bool[] visited = new bool[guides.Count];
            int groupId = 1;
            for (int i = 0; i < guides.Count; i++)
            {
                if (visited[i] || adjacency[i].Count == 0)
                {
                    continue;
                }

                List<int> component = new List<int>();
                Queue<int> queue = new Queue<int>();
                queue.Enqueue(i);
                visited[i] = true;
                while (queue.Count > 0)
                {
                    int index = queue.Dequeue();
                    component.Add(index);
                    foreach (int next in adjacency[index])
                    {
                        if (!visited[next])
                        {
                            visited[next] = true;
                            queue.Enqueue(next);
                        }
                    }
                }

                if (component.Count < 2 || !IsSimplePolylineComponent(component, guides))
                {
                    continue;
                }

                PolylineGroup group = new PolylineGroup
                {
                    GroupId = groupId++,
                    LineType = guides[component[0]].Layer.lineType
                };

                HashSet<long> seenPixels = new HashSet<long>();
                float maxTolerance = 0f;
                foreach (int index in component)
                {
                    PolylineSegmentGuide guide = guides[index];
                    group.Segments.Add(guide);
                    maxTolerance = Mathf.Max(maxTolerance, guide.Tolerance);
                    foreach (var point in guide.Pixels)
                    {
                        if (seenPixels.Add(PointKey(point.x, point.y)))
                        {
                            group.StandardPixels.Add(point);
                        }
                    }
                }

                group.Tolerance = Mathf.Max(8f, maxTolerance);
                _polylineGroups.Add(group);
                foreach (var segment in group.Segments)
                {
                    _polylineGroupByLayerNum[segment.Layer.layerNum] = group;
                }

                if (ScoringPerf.VerboseLayerLogs)
                {
                    string layerNums = string.Join(",", group.Segments.Select(s => s.Layer.layerNum));
                    Debug.Log($"[AnswerCheck] 识别折线组 group={group.GroupId}, layers={layerNums}, lineType={group.LineType}");
                }
            }
        }

        private bool CanUsePolylineLayer(LayerManager layer)
        {
            return layer != null
                   && layer.lineshape == lineshape.直线
                   && layer.lineType != linetype.unknown;
        }

        private bool TryCreatePolylineSegmentGuide(LayerManager layer, List<PositionInt> pixels, out PolylineSegmentGuide guide)
        {
            guide = null;
            if (layer == null || pixels == null || pixels.Count < 2)
            {
                return false;
            }

            Vector2 axis = GetPrincipalAxis(pixels);
            ProjectionStats stats = GetProjectionStats(pixels, axis);
            if (stats.Length < 8f)
            {
                return false;
            }

            Vector2 center = GetAveragePoint(pixels);
            float centerProjection = Vector2.Dot(center, axis);
            guide = new PolylineSegmentGuide
            {
                Layer = layer,
                Pixels = pixels,
                Axis = axis,
                P0 = center + axis * (stats.Min - centerProjection),
                P1 = center + axis * (stats.Max - centerProjection),
                Length = stats.Length,
                Tolerance = GetLineTolerance(pixels)
            };
            return true;
        }

        private bool ArePolylineSegmentsConnected(PolylineSegmentGuide a, PolylineSegmentGuide b)
        {
            if (a == null || b == null || a.Layer.lineType != b.Layer.lineType)
            {
                return false;
            }

            float joinTolerance = Mathf.Max(10f, Mathf.Min(a.Tolerance, b.Tolerance) * 1.5f);
            float joinToleranceSq = joinTolerance * joinTolerance;
            return (a.P0 - b.P0).sqrMagnitude <= joinToleranceSq
                   || (a.P0 - b.P1).sqrMagnitude <= joinToleranceSq
                   || (a.P1 - b.P0).sqrMagnitude <= joinToleranceSq
                   || (a.P1 - b.P1).sqrMagnitude <= joinToleranceSq;
        }

        private bool IsSimplePolylineComponent(List<int> component, List<PolylineSegmentGuide> guides)
        {
            if (component == null || guides == null || component.Count < 2 || component.Count > 12)
            {
                return false;
            }

            linetype lineType = guides[component[0]].Layer.lineType;
            foreach (int index in component)
            {
                if (guides[index].Layer.lineType != lineType)
                {
                    return false;
                }
            }

            foreach (int endpointOwner in component)
            {
                PolylineSegmentGuide ownerGuide = guides[endpointOwner];
                if (CountSegmentsNearEndpoint(ownerGuide.P0, component, guides, ownerGuide.Tolerance) > 2
                    || CountSegmentsNearEndpoint(ownerGuide.P1, component, guides, ownerGuide.Tolerance) > 2)
                {
                    return false;
                }
            }

            return true;
        }

        private int CountSegmentsNearEndpoint(Vector2 endpoint, List<int> component, List<PolylineSegmentGuide> guides, float tolerance)
        {
            float limit = Mathf.Max(10f, tolerance * 1.5f);
            float limitSq = limit * limit;
            int count = 0;
            foreach (int index in component)
            {
                PolylineSegmentGuide guide = guides[index];
                if ((guide.P0 - endpoint).sqrMagnitude <= limitSq || (guide.P1 - endpoint).sqrMagnitude <= limitSq)
                {
                    count++;
                }
            }
            return count;
        }

        private bool TryGetPolylineSegmentResult(LayerManager layer, out PolylineSegmentResult result)
        {
            result = null;
            if (layer == null || studentlayer_manager == null)
            {
                return false;
            }

            if (_polylineGroupByLayerNum == null || _polylineResultCache == null)
            {
                BuildPolylineGroups();
            }

            if (_polylineGroupByLayerNum == null || !_polylineGroupByLayerNum.TryGetValue(layer.layerNum, out PolylineGroup group))
            {
                return false;
            }

            if (!_polylineResultCache.TryGetValue(group.GroupId, out Dictionary<int, PolylineSegmentResult> groupResult))
            {
                groupResult = EvaluatePolylineGroup(group);
                _polylineResultCache[group.GroupId] = groupResult;
            }

            return groupResult != null && groupResult.TryGetValue(layer.layerNum, out result);
        }

        private Dictionary<int, PolylineSegmentResult> EvaluatePolylineGroup(PolylineGroup group)
        {
            Dictionary<int, PolylineSegmentResult> results = CreateDefaultPolylineResults(group, ErrorReson.图线不在或偏离正确位置);
            if (group == null || group.Segments == null || group.Segments.Count == 0 || group.StandardPixels == null || group.StandardPixels.Count == 0)
            {
                return results;
            }

            LayerPixelCache studentCache = GetStudentPixelCache(studentlayer_manager);
            if (studentCache.Pixels == null || studentCache.Pixels.Count == 0)
            {
                return results;
            }

            LayerManager firstLayer = group.Segments[0].Layer;
            List<PositionInt> searchStandardPixels = DilateRepeated(group.StandardPixels, 2, firstLayer.LayerSize.width, firstLayer.LayerSize.height);
            Vector2Int bestOffset = FindBestOffset(searchStandardPixels, studentCache, 24, 2);

            int segmentCount = group.Segments.Count;
            Vector2 bestOffsetVector = new Vector2(bestOffset.x, bestOffset.y);
            Vector2[] p0 = new Vector2[segmentCount];
            Vector2[] p1 = new Vector2[segmentCount];
            Vector2[] axis = new Vector2[segmentCount];
            float[] length = new float[segmentCount];
            List<PositionInt>[] assignedPixels = new List<PositionInt>[segmentCount];
            List<PositionInt>[] extensionPixels = new List<PositionInt>[segmentCount];
            List<PositionInt>[] shiftedSegmentPixels = new List<PositionInt>[segmentCount];

            List<PositionInt> shiftedGroupPixels = new List<PositionInt>(group.StandardPixels.Count);
            for (int i = 0; i < segmentCount; i++)
            {
                PolylineSegmentGuide segment = group.Segments[i];
                p0[i] = segment.P0 + bestOffsetVector;
                p1[i] = segment.P1 + bestOffsetVector;
                axis[i] = segment.Axis;
                length[i] = Mathf.Max(1f, segment.Length);
                assignedPixels[i] = new List<PositionInt>();
                extensionPixels[i] = new List<PositionInt>();
                shiftedSegmentPixels[i] = ShiftPositions(segment.Pixels, bestOffset);
                shiftedGroupPixels.AddRange(shiftedSegmentPixels[i]);
            }

            int candidateExpand = Mathf.CeilToInt(group.Tolerance * 8f);
            PixelBounds candidateBounds = ExpandBounds(GetBounds(shiftedGroupPixels), candidateExpand);
            List<PositionInt> candidatePixels = studentCache.GetPixelsInsideBounds(candidateBounds);
            if (candidatePixels.Count == 0)
            {
                return results;
            }

            float finiteDistanceLimit = group.Tolerance * 2.5f;
            float finiteDistanceLimitSq = finiteDistanceLimit * finiteDistanceLimit;
            float extensionBandLimit = group.Tolerance * 2.2f;
            float extensionBandLimitSq = extensionBandLimit * extensionBandLimit;
            List<PositionInt> inGroupPixels = new List<PositionInt>();
            List<PositionInt> extraOutsideGroupPixels = new List<PositionInt>();

            foreach (var point in candidatePixels)
            {
                Vector2 p = new Vector2(point.x, point.y);
                int bestSegment = -1;
                float bestDistanceSq = float.MaxValue;
                for (int i = 0; i < segmentCount; i++)
                {
                    float distanceSq = DistancePointToSegmentSq(p, p0[i], p1[i], out _);
                    if (distanceSq < bestDistanceSq)
                    {
                        bestDistanceSq = distanceSq;
                        bestSegment = i;
                    }
                }

                if (bestSegment >= 0 && bestDistanceSq <= finiteDistanceLimitSq)
                {
                    assignedPixels[bestSegment].Add(point);
                    inGroupPixels.Add(point);
                    continue;
                }

                int extensionSegment = -1;
                float bestExtensionDistanceSq = float.MaxValue;
                for (int i = 0; i < segmentCount; i++)
                {
                    GetPointSegmentProjection(p, p0[i], axis[i], out float projection, out float perpendicularDistanceSq);
                    float extensionSlack = Mathf.Max(group.Tolerance * 6f, length[i] * 0.22f);
                    bool outsideSegment = projection < 0f || projection > length[i];
                    float overrun = projection < 0f ? -projection : projection - length[i];
                    if (outsideSegment
                        && overrun <= extensionSlack
                        && perpendicularDistanceSq <= extensionBandLimitSq
                        && perpendicularDistanceSq < bestExtensionDistanceSq)
                    {
                        bestExtensionDistanceSq = perpendicularDistanceSq;
                        extensionSegment = i;
                    }
                }

                if (extensionSegment >= 0)
                {
                    extensionPixels[extensionSegment].Add(point);
                }
                else
                {
                    extraOutsideGroupPixels.Add(point);
                }
            }

            LineLimits limits = GetLineLimits(group.Segments[0].Layer);
            float groupPrecision = candidatePixels.Count == 0 ? 0f : inGroupPixels.Count / (float)candidatePixels.Count;
            float groupExtraRatio = 1f - groupPrecision;
            Vector2 groupExtraPosition = extraOutsideGroupPixels.Count > 0 ? GetAveragePoint(extraOutsideGroupPixels) : Vector2.zero;

            Dictionary<int, PolylineSegmentResult> segmentResults = new Dictionary<int, PolylineSegmentResult>();
            for (int i = 0; i < segmentCount; i++)
            {
                PolylineSegmentGuide segment = group.Segments[i];
                float coverage = GetSegmentCoverage(assignedPixels[i], p0[i], axis[i], length[i], group.Tolerance, out Vector2 missingPosition);
                float extraRatio = GetSegmentExtraRatio(assignedPixels[i], extensionPixels[i], p0[i], axis[i], length[i], group.Tolerance, out Vector2 extraPosition, out float lengthRatio);
                linetype studentType = assignedPixels[i].Count > 0
                    ? RecognizeLineTypeByProjection(assignedPixels[i], axis[i], segment.Layer.lineType == linetype.实线, length[i])
                    : linetype.unknown;

                ErrorReson error = ErrorReson.正确;
                Vector2 errorPosition = GetAveragePoint(shiftedSegmentPixels[i]);
                float rightCoverage = GetPolylineSegmentRightCoverage(length[i]);
                if (assignedPixels[i].Count == 0 && extensionPixels[i].Count == 0)
                {
                    error = ErrorReson.图线不在或偏离正确位置;
                }
                else if (coverage < 0.20f)
                {
                    error = ErrorReson.图线不在或偏离正确位置;
                    errorPosition = missingPosition;
                }
                else if (studentType != linetype.unknown && studentType != segment.Layer.lineType)
                {
                    error = ErrorReson.线型使用错误;
                    errorPosition = assignedPixels[i].Count > 0 ? GetAveragePoint(assignedPixels[i]) : errorPosition;
                }
                else if (extraRatio > 0.12f && lengthRatio > 1.12f)
                {
                    error = ErrorReson.图线过长;
                    errorPosition = extraPosition;
                }
                else if (coverage < rightCoverage)
                {
                    error = ErrorReson.图线过短;
                    errorPosition = missingPosition;
                }
                else if (groupExtraRatio > 0.55f && groupPrecision < limits.precisionLimit)
                {
                    error = ErrorReson.图线不在或偏离正确位置;
                    errorPosition = groupExtraPosition != Vector2.zero ? groupExtraPosition : errorPosition;
                }

                segmentResults[segment.Layer.layerNum] = new PolylineSegmentResult
                {
                    Error = error,
                    ErrorPosition = errorPosition,
                    StudentLineType = studentType == linetype.unknown ? group.LineType : studentType,
                    Coverage = coverage,
                    LengthRatio = lengthRatio,
                    ExtraRatio = extraRatio
                };
            }

            if (ScoringPerf.VerboseLayerLogs)
            {
                string detail = string.Join("; ", segmentResults.Select(kv => $"#{kv.Key}:{kv.Value.Error},cov={kv.Value.Coverage:F2},span={kv.Value.LengthRatio:F2},extra={kv.Value.ExtraRatio:F2}"));
                Debug.Log($"[AnswerCheck] 折线组判分 group={group.GroupId}, precision={groupPrecision:F3}, extra={groupExtraRatio:F3}, offset={bestOffset}, {detail}");
            }

            return segmentResults;
        }

        private Dictionary<int, PolylineSegmentResult> CreateDefaultPolylineResults(PolylineGroup group, ErrorReson error)
        {
            Dictionary<int, PolylineSegmentResult> results = new Dictionary<int, PolylineSegmentResult>();
            if (group == null || group.Segments == null)
            {
                return results;
            }

            foreach (var segment in group.Segments)
            {
                Vector2 errorPosition = segment.Pixels != null && segment.Pixels.Count > 0 ? GetAveragePoint(segment.Pixels) : Vector2.zero;
                results[segment.Layer.layerNum] = new PolylineSegmentResult
                {
                    Error = error,
                    ErrorPosition = errorPosition,
                    StudentLineType = segment.Layer.lineType,
                    Coverage = 0f,
                    LengthRatio = 0f,
                    ExtraRatio = 0f
                };
            }

            return results;
        }

        private float GetSegmentCoverage(List<PositionInt> pixels, Vector2 p0, Vector2 axis, float length, float tolerance, out Vector2 missingPosition)
        {
            missingPosition = p0 + axis * (length * 0.5f);
            if (pixels == null || pixels.Count == 0 || length <= 0f)
            {
                return 0f;
            }

            float binSize = Mathf.Max(tolerance * 2f, 16f);
            int binCount = Mathf.Clamp(Mathf.CeilToInt(length / binSize), 4, 12);
            bool[] covered = new bool[binCount];
            float perpendicularLimitSq = tolerance * tolerance * 3.24f;
            foreach (var point in pixels)
            {
                Vector2 p = new Vector2(point.x, point.y);
                GetPointSegmentProjection(p, p0, axis, out float projection, out float perpendicularDistanceSq);
                if (projection < 0f || projection > length || perpendicularDistanceSq > perpendicularLimitSq)
                {
                    continue;
                }

                int bin = Mathf.Clamp(Mathf.FloorToInt((projection / length) * binCount), 0, binCount - 1);
                covered[bin] = true;
            }

            int coveredCount = 0;
            int firstMissing = -1;
            for (int i = 0; i < covered.Length; i++)
            {
                if (covered[i])
                {
                    coveredCount++;
                }
                else if (firstMissing < 0)
                {
                    firstMissing = i;
                }
            }

            if (firstMissing >= 0)
            {
                float missingProjection = ((firstMissing + 0.5f) / binCount) * length;
                missingPosition = p0 + axis * missingProjection;
            }

            return coveredCount / (float)binCount;
        }

        private float GetSegmentExtraRatio(List<PositionInt> assignedPixels, List<PositionInt> extensionPixels, Vector2 p0, Vector2 axis, float length, float tolerance, out Vector2 extraPosition, out float lengthRatio)
        {
            extraPosition = p0 + axis * (length * 0.5f);
            lengthRatio = 0f;
            List<PositionInt> pixels = new List<PositionInt>();
            if (assignedPixels != null) pixels.AddRange(assignedPixels);
            if (extensionPixels != null) pixels.AddRange(extensionPixels);
            if (pixels.Count == 0 || length <= 0f)
            {
                return 0f;
            }

            float minProjection = float.MaxValue;
            float maxProjection = float.MinValue;
            List<PositionInt> extraPixels = new List<PositionInt>();
            float overrunLimit = Mathf.Max(10f, length * 0.12f);
            foreach (var point in pixels)
            {
                Vector2 p = new Vector2(point.x, point.y);
                float projection = Vector2.Dot(p - p0, axis);
                if (projection < minProjection) minProjection = projection;
                if (projection > maxProjection) maxProjection = projection;
                if (projection < -overrunLimit || projection > length + overrunLimit)
                {
                    extraPixels.Add(point);
                }
            }

            float span = Mathf.Max(0f, maxProjection - minProjection);
            float extraLength = Mathf.Max(0f, -minProjection) + Mathf.Max(0f, maxProjection - length);
            lengthRatio = span / length;
            if (extraPixels.Count > 0)
            {
                extraPosition = GetAveragePoint(extraPixels);
            }
            return extraLength / length;
        }

        private float GetPolylineSegmentRightCoverage(float length)
        {
            if (length < 80f)
            {
                return 0.60f;
            }

            if (length < 160f)
            {
                return 0.68f;
            }

            return 0.72f;
        }

        private float DistancePointToSegmentSq(Vector2 point, Vector2 a, Vector2 b, out float t)
        {
            Vector2 ab = b - a;
            float lengthSq = ab.sqrMagnitude;
            if (lengthSq <= 0.0001f)
            {
                t = 0f;
                return (point - a).sqrMagnitude;
            }

            t = Mathf.Clamp01(Vector2.Dot(point - a, ab) / lengthSq);
            Vector2 closest = a + ab * t;
            return (point - closest).sqrMagnitude;
        }

        private void GetPointSegmentProjection(Vector2 point, Vector2 p0, Vector2 axis, out float projection, out float perpendicularDistanceSq)
        {
            Vector2 delta = point - p0;
            projection = Vector2.Dot(delta, axis);
            Vector2 projected = p0 + axis * projection;
            perpendicularDistanceSq = (point - projected).sqrMagnitude;
        }

        private LineMatchResult EvaluateLineMatch(LayerManager studentLayer, LayerManager standardLayer)
        {
            LayerPixelCache studentCache = GetStudentPixelCache(studentLayer);
            List<PositionInt> studentPixels = studentCache.Pixels;
            List<PositionInt> standardPixels = GetStandardPixels(standardLayer);
            LineLimits limits = GetLineLimits(standardLayer);

            LineMatchResult result = new LineMatchResult
            {
                HasStandardPixels = standardPixels.Count > 0,
                HasStudentPixels = studentPixels.Count > 0,
                ErrorLimit = limits.errorLimit,
                RightLimit = limits.rightLimit,
                PrecisionLimit = limits.precisionLimit,
                StudentLineType = linetype.unknown,
                ErrorPosition = standardPixels.Count > 0 ? GetAveragePoint(standardPixels) : Vector2.zero,
                MissingErrorPosition = standardPixels.Count > 0 ? GetAveragePoint(standardPixels) : Vector2.zero,
                ExtraErrorPosition = studentPixels.Count > 0 ? GetAveragePoint(studentPixels) : Vector2.zero
            };

            if (!result.HasStandardPixels || !result.HasStudentPixels)
            {
                return result;
            }

            result.Tolerance = GetLineTolerance(standardPixels);
            if (IsCurvedLineShape(standardLayer.lineshape))
            {
                result.Tolerance *= 1.45f;
                result.RightLimit = Mathf.Min(result.RightLimit, 0.50f);
                result.PrecisionLimit = Mathf.Min(result.PrecisionLimit, 0.35f);
            }
            List<PositionInt> searchStandardPixels = DilateRepeated(standardPixels, 2, standardLayer.LayerSize.width, standardLayer.LayerSize.height);
            using (ScoringPerf.ScopeDetailed(ScoringPerf.TitleKey($"AnswerCheck.OffsetSearch#{standardLayer.layerNum}", ScoringPerf.CurrentTitleId), $"standardSearchPixels={searchStandardPixels.Count}"))
            {
                result.BestOffset = FindBestOffset(searchStandardPixels, studentCache, 24, 2);
            }

            List<PositionInt> shiftedStandardPixels = ShiftPositions(standardPixels, result.BestOffset);
            Vector2 mainAxis = GetPrincipalAxis(shiftedStandardPixels);
            float standardProjectedLength = Mathf.Max(1f, GetProjectedLength(shiftedStandardPixels, mainAxis));
            bool usePatternedLineMatch = CanUsePatternedLineMatch(standardLayer);
            float candidateLengthExpand = usePatternedLineMatch ? 0.22f : 0.08f;
            int candidateExpand = Mathf.CeilToInt(result.Tolerance * 4f + standardProjectedLength * candidateLengthExpand);
            PixelBounds candidateBounds = ExpandBounds(GetBounds(shiftedStandardPixels), candidateExpand);
            List<PositionInt> studentCandidatePixels;
            using (ScoringPerf.ScopeDetailed(ScoringPerf.TitleKey($"AnswerCheck.CandidateBoundsQuery#{standardLayer.layerNum}", ScoringPerf.CurrentTitleId), $"studentPixels={studentPixels.Count};bounds={DescribeBounds(candidateBounds)};tile={StudentPixelTileSize}"))
            {
                studentCandidatePixels = studentCache.GetPixelsInsideBounds(candidateBounds);
            }
            using (ScoringPerf.ScopeDetailed(ScoringPerf.TitleKey($"AnswerCheck.CandidateFilter#{standardLayer.layerNum}", ScoringPerf.CurrentTitleId), $"candidates={studentCandidatePixels.Count};standard={shiftedStandardPixels.Count}"))
            {
                if (usePatternedLineMatch)
                {
                    studentCandidatePixels = FilterStraightLineBand(studentCandidatePixels, shiftedStandardPixels, mainAxis, result.Tolerance);
                    studentCandidatePixels = FilterNeighborStandardLinePixels(studentCandidatePixels, standardLayer, result.BestOffset, shiftedStandardPixels, mainAxis, result.Tolerance);
                }
                else
                {
                    studentCandidatePixels = FilterStudentCandidateComponents(studentCandidatePixels, shiftedStandardPixels, result.Tolerance, standardLayer.layerNum);
                    if (standardLayer.lineshape == lineshape.直线)
                    {
                        studentCandidatePixels = FilterStraightLineBand(studentCandidatePixels, shiftedStandardPixels, mainAxis, result.Tolerance);
                        studentCandidatePixels = FilterNeighborStandardLinePixels(studentCandidatePixels, standardLayer, result.BestOffset, shiftedStandardPixels, mainAxis, result.Tolerance);
                    }
                }
            }

            if (studentCandidatePixels.Count == 0)
            {
                result.HasStudentPixels = false;
                return result;
            }

            if (usePatternedLineMatch)
            {
                return EvaluatePatternedLineMatch(result, shiftedStandardPixels, studentCandidatePixels, mainAxis, standardProjectedLength);
            }

            int maxSearchDistance = Mathf.CeilToInt(result.Tolerance * 3f);
            int maxSearchDistanceSq = maxSearchDistance * maxSearchDistance;
            float toleranceSq = result.Tolerance * result.Tolerance;

            int matchedStandard = 0;
            List<float> standardDistances = new List<float>(shiftedStandardPixels.Count);
            List<PositionInt> missingStandardPixels = new List<PositionInt>();
            PixelBounds standardQueryBounds = ExpandBounds(GetBounds(shiftedStandardPixels), maxSearchDistance);
            bool useDistanceFieldForStandard = ShouldUseDistanceField(shiftedStandardPixels.Count, standardQueryBounds, maxSearchDistance);
            if (useDistanceFieldForStandard)
            {
                DistanceField studentDistanceField = null;
                using (ScoringPerf.ScopeDetailed(ScoringPerf.TitleKey($"AnswerCheck.DistanceFieldStudent#{standardLayer.layerNum}", ScoringPerf.CurrentTitleId), $"targets={studentCandidatePixels.Count};query={shiftedStandardPixels.Count};roi={DescribeBounds(standardQueryBounds)};maxDistance={maxSearchDistance}"))
                {
                    studentDistanceField = DistanceField.Build(studentCandidatePixels, standardQueryBounds);
                }

                using (ScoringPerf.ScopeDetailed(ScoringPerf.TitleKey($"AnswerCheck.DistanceStandard#{standardLayer.layerNum}", ScoringPerf.CurrentTitleId), $"standard={shiftedStandardPixels.Count};studentCandidates={studentCandidatePixels.Count};maxDistance={maxSearchDistance};mode=distanceField;roi={DescribeBounds(standardQueryBounds)}"))
                {
                    foreach (var point in shiftedStandardPixels)
                    {
                        if (studentDistanceField.TryGetDistanceSq(point, maxSearchDistanceSq, out int distanceSq))
                        {
                            standardDistances.Add(Mathf.Sqrt(distanceSq));
                            if (distanceSq <= toleranceSq)
                            {
                                matchedStandard++;
                            }
                            else
                            {
                                missingStandardPixels.Add(point);
                            }
                        }
                        else
                        {
                            standardDistances.Add(maxSearchDistance + 1f);
                            missingStandardPixels.Add(point);
                        }
                    }
                }
            }
            else
            {
                HashSet<long> studentSet = BuildPointSet(studentCandidatePixels);
                using (ScoringPerf.ScopeDetailed(ScoringPerf.TitleKey($"AnswerCheck.DistanceStandard#{standardLayer.layerNum}", ScoringPerf.CurrentTitleId), $"standard={shiftedStandardPixels.Count};studentSet={studentSet.Count};maxDistance={maxSearchDistance};mode=radiusScan"))
                {
                    foreach (var point in shiftedStandardPixels)
                    {
                        if (TryFindNearestDistance(studentSet, point, maxSearchDistance, out float distance))
                        {
                            standardDistances.Add(distance);
                            if (distance <= result.Tolerance)
                            {
                                matchedStandard++;
                            }
                            else
                            {
                                missingStandardPixels.Add(point);
                            }
                        }
                        else
                        {
                            standardDistances.Add(maxSearchDistance + 1f);
                            missingStandardPixels.Add(point);
                        }
                    }
                }
            }

            int matchedStudent = 0;
            List<PositionInt> extraStudentPixels = new List<PositionInt>();
            PixelBounds studentQueryBounds = ExpandBounds(GetBounds(studentCandidatePixels), maxSearchDistance);
            bool useDistanceFieldForStudent = ShouldUseDistanceField(studentCandidatePixels.Count, studentQueryBounds, maxSearchDistance);
            if (useDistanceFieldForStudent)
            {
                DistanceField standardDistanceField = null;
                using (ScoringPerf.ScopeDetailed(ScoringPerf.TitleKey($"AnswerCheck.DistanceFieldStandard#{standardLayer.layerNum}", ScoringPerf.CurrentTitleId), $"targets={shiftedStandardPixels.Count};query={studentCandidatePixels.Count};roi={DescribeBounds(studentQueryBounds)};maxDistance={maxSearchDistance}"))
                {
                    standardDistanceField = DistanceField.Build(shiftedStandardPixels, studentQueryBounds);
                }

                using (ScoringPerf.ScopeDetailed(ScoringPerf.TitleKey($"AnswerCheck.DistanceStudent#{standardLayer.layerNum}", ScoringPerf.CurrentTitleId), $"studentCandidates={studentCandidatePixels.Count};standard={shiftedStandardPixels.Count};maxDistance={maxSearchDistance};mode=distanceField;roi={DescribeBounds(studentQueryBounds)}"))
                {
                    foreach (var point in studentCandidatePixels)
                    {
                        if (standardDistanceField.TryGetDistanceSq(point, maxSearchDistanceSq, out int distanceSq) && distanceSq <= toleranceSq)
                        {
                            matchedStudent++;
                        }
                        else
                        {
                            extraStudentPixels.Add(point);
                        }
                    }
                }
            }
            else
            {
                HashSet<long> standardSet = BuildPointSet(shiftedStandardPixels);
                using (ScoringPerf.ScopeDetailed(ScoringPerf.TitleKey($"AnswerCheck.DistanceStudent#{standardLayer.layerNum}", ScoringPerf.CurrentTitleId), $"studentCandidates={studentCandidatePixels.Count};standardSet={standardSet.Count};maxDistance={maxSearchDistance};mode=radiusScan"))
                {
                    foreach (var point in studentCandidatePixels)
                    {
                        if (TryFindNearestDistance(standardSet, point, maxSearchDistance, out float distance) && distance <= result.Tolerance)
                        {
                            matchedStudent++;
                        }
                        else
                        {
                            extraStudentPixels.Add(point);
                        }
                    }
                }
            }

            result.Recall = matchedStandard / (float)shiftedStandardPixels.Count;
            result.Precision = matchedStudent / (float)studentCandidatePixels.Count;
            result.MissingRatio = 1f - result.Recall;
            result.ExtraRatio = 1f - result.Precision;
            result.P95Distance = Percentile(standardDistances, 0.95f);
            result.StudentLineType = RecognizeLineTypeByProjection(studentCandidatePixels, mainAxis, standardLayer.lineType == linetype.实线, standardProjectedLength);

            float studentProjectedLength = GetProjectedLength(studentCandidatePixels, mainAxis);
            float lengthRatio = studentProjectedLength / standardProjectedLength;
            result.ProjectedLengthRatio = lengthRatio;

            result.MissingErrorPosition = missingStandardPixels.Count > 0 ? GetAveragePoint(missingStandardPixels) : GetAveragePoint(shiftedStandardPixels);
            result.ExtraErrorPosition = extraStudentPixels.Count > 0 ? GetAveragePoint(extraStudentPixels) : GetAveragePoint(studentCandidatePixels);
            result.ErrorPosition = result.Recall < result.ErrorLimit
                ? GetAveragePoint(shiftedStandardPixels)
                : (result.ExtraRatio > result.MissingRatio ? result.ExtraErrorPosition : result.MissingErrorPosition);

            bool isCurvedLineShape = IsCurvedLineShape(standardLayer.lineshape);
            float shortMissingLimit = isCurvedLineShape
                ? GetCurvedTooShortMissingLimit()
                : Mathf.Max(0.32f, 1f - result.RightLimit + 0.08f);
            float shortLengthRatioLimit = isCurvedLineShape ? 0.62f : 0.78f;
            float shortExtraRatioLimit = isCurvedLineShape ? 0.72f : 0.65f;
            result.IsTooShort = result.Recall >= result.ErrorLimit
                                && (result.MissingRatio >= shortMissingLimit || lengthRatio < shortLengthRatioLimit)
                                && result.ExtraRatio < shortExtraRatioLimit;

            result.IsTooLong = result.Recall >= result.RightLimit
                               && (result.ExtraRatio > 0.42f || lengthRatio > 1.18f)
                               && result.Precision < 0.86f;

            return result;
        }

        private bool IsLineMatchRightEnough(LineMatchResult lineMatch, LayerManager standardLayer)
        {
            if (!IsCurvedLineShape(standardLayer.lineshape))
            {
                return lineMatch.Recall >= lineMatch.RightLimit
                       && lineMatch.Precision >= lineMatch.PrecisionLimit
                       && lineMatch.P95Distance <= lineMatch.Tolerance * 1.5f;
            }

            bool strictCurveMatch = lineMatch.Recall >= lineMatch.RightLimit
                                    && lineMatch.Precision >= lineMatch.PrecisionLimit
                                    && lineMatch.P95Distance <= lineMatch.Tolerance * 2.0f;
            bool lenientCurveMatch = lineMatch.Recall >= Mathf.Max(lineMatch.ErrorLimit, 0.32f)
                                     && lineMatch.Precision >= lineMatch.PrecisionLimit
                                     && lineMatch.P95Distance <= lineMatch.Tolerance * 2.6f
                                     && lineMatch.ProjectedLengthRatio >= 0.62f
                                     && lineMatch.ExtraRatio < 0.72f;
            return strictCurveMatch || lenientCurveMatch;
        }

        private bool IsCurvedLineShape(lineshape shape)
        {
            return shape == lineshape.圆弧 || shape == lineshape.圆 || shape == lineshape.椭圆;
        }

        private float GetCurvedTooShortMissingLimit()
        {
            return 0.58f;
        }

        private bool CanUsePatternedLineMatch(LayerManager standardLayer)
        {
            return standardLayer != null
                   && standardLayer.lineshape == lineshape.直线
                   && IsPatternedLineType(standardLayer.lineType);
        }

        private bool IsPatternedLineType(linetype type)
        {
            return type == linetype.虚线 || type == linetype.点画线 || type == linetype.双点画线;
        }

        private bool IsPatternedLineTypeCompatible(linetype expected, linetype actual)
        {
            return IsPatternedLineType(expected) && expected == actual;
        }

        private ErrorReson GetPatternedLineError(LineMatchResult lineMatch, linetype standardLineType)
        {
            if (lineMatch.StudentLineType == linetype.unknown || lineMatch.GeometryPrecision < PatternGeometryLimit)
            {
                return ErrorReson.图线不在或偏离正确位置;
            }

            if (!IsPatternedLineTypeCompatible(standardLineType, lineMatch.StudentLineType))
            {
                return ErrorReson.线型使用错误;
            }

            if (lineMatch.IsTooShort)
            {
                return ErrorReson.图线过短;
            }

            if (lineMatch.IsTooLong)
            {
                return ErrorReson.图线过长;
            }

            if (lineMatch.MacroCoverage >= lineMatch.MacroCoverageLimit
                && lineMatch.PatternLengthRatio >= PatternMinLengthRatio
                && lineMatch.PatternLengthRatio <= PatternMaxLengthRatio
                && lineMatch.PatternEndpointMiss <= lineMatch.PatternEndpointLimit
                && lineMatch.PatternExtraSpanRatio <= PatternMaxExtraSpanRatio)
            {
                return ErrorReson.正确;
            }

            return lineMatch.MissingRatio > lineMatch.ExtraRatio
                ? ErrorReson.图线过短
                : ErrorReson.图线不在或偏离正确位置;
        }

        private LineMatchResult EvaluatePatternedLineMatch(LineMatchResult result, List<PositionInt> shiftedStandardPixels, List<PositionInt> studentCandidatePixels, Vector2 axis, float standardProjectedLength)
        {
            ProjectionStats standardStats = GetProjectionStats(shiftedStandardPixels, axis);
            ProjectionStats studentStats = GetProjectionStats(studentCandidatePixels, axis);
            float standardLength = Mathf.Max(1f, Mathf.Max(standardStats.Length, standardProjectedLength));
            float endpointLimit = GetPatternEndpointLimit(standardLength, result.Tolerance);
            float bandWidth = result.Tolerance * 2.2f;
            Vector2 center = GetAveragePoint(shiftedStandardPixels);
            Vector2 normal = new Vector2(-axis.y, axis.x).normalized;
            int macroBinCount = GetPatternMacroBinCount(standardLength, result.Tolerance);
            bool[] macroBins = new bool[macroBinCount];
            float macroBinWidth = standardLength / macroBinCount;

            int inGuideCount = 0;
            List<float> guideDistances = new List<float>(studentCandidatePixels.Count);
            List<PositionInt> extraStudentPixels = new List<PositionInt>();

            foreach (var point in studentCandidatePixels)
            {
                Vector2 p = new Vector2(point.x, point.y);
                float projection = Vector2.Dot(p, axis);
                float perpendicularDistance = Mathf.Abs(Vector2.Dot(p - center, normal));
                bool insideBand = perpendicularDistance <= bandWidth;
                bool insideGuideSpan = projection >= standardStats.Min - endpointLimit && projection <= standardStats.Max + endpointLimit;

                guideDistances.Add(insideBand ? perpendicularDistance : bandWidth + 1f);

                if (insideBand && insideGuideSpan)
                {
                    inGuideCount++;
                    if (projection >= standardStats.Min && projection <= standardStats.Max)
                    {
                        int bin = Mathf.Clamp(Mathf.FloorToInt((projection - standardStats.Min) / macroBinWidth), 0, macroBinCount - 1);
                        macroBins[bin] = true;
                    }
                }
                else
                {
                    extraStudentPixels.Add(point);
                }
            }

            int coveredBins = 0;
            foreach (bool covered in macroBins)
            {
                if (covered)
                {
                    coveredBins++;
                }
            }

            float missingStart = Mathf.Max(0f, studentStats.Min - standardStats.Min);
            float missingEnd = Mathf.Max(0f, standardStats.Max - studentStats.Max);
            float extraSpan = Mathf.Max(0f, standardStats.Min - studentStats.Min) + Mathf.Max(0f, studentStats.Max - standardStats.Max);

            result.IsPatternedMode = true;
            result.StudentLineType = RecognizeLineTypeByProjection(studentCandidatePixels, axis);
            result.GeometryPrecision = inGuideCount / (float)studentCandidatePixels.Count;
            result.MacroCoverage = coveredBins / (float)macroBinCount;
            result.MacroCoverageLimit = GetPatternMacroCoverageLimit(standardLength);
            result.PatternLengthRatio = studentStats.Length / standardLength;
            result.PatternEndpointMiss = Mathf.Max(missingStart, missingEnd);
            result.PatternEndpointLimit = endpointLimit;
            result.PatternExtraSpanRatio = extraSpan / standardLength;
            result.Recall = result.MacroCoverage;
            result.Precision = result.GeometryPrecision;
            result.MissingRatio = 1f - result.MacroCoverage;
            result.ExtraRatio = Mathf.Max(1f - result.GeometryPrecision, result.PatternExtraSpanRatio);
            result.P95Distance = Percentile(guideDistances, 0.95f);
            result.MissingErrorPosition = missingStart >= missingEnd
                ? GetProjectionEdgePoint(shiftedStandardPixels, axis, standardStats.Min)
                : GetProjectionEdgePoint(shiftedStandardPixels, axis, standardStats.Max);
            result.ExtraErrorPosition = extraStudentPixels.Count > 0 ? GetAveragePoint(extraStudentPixels) : GetAveragePoint(studentCandidatePixels);
            result.IsTooShort = result.GeometryPrecision >= PatternGeometryLimit
                                && (result.MacroCoverage < result.MacroCoverageLimit
                                    || result.PatternLengthRatio < PatternMinLengthRatio
                                    || result.PatternEndpointMiss > endpointLimit)
                                && result.PatternExtraSpanRatio <= PatternMaxExtraSpanRatio * 1.5f;
            result.IsTooLong = result.GeometryPrecision >= PatternGeometryLimit
                               && (result.PatternLengthRatio > PatternMaxLengthRatio
                                   || result.PatternExtraSpanRatio > PatternMaxExtraSpanRatio
                                   || result.ExtraRatio > 0.45f);
            result.ErrorPosition = result.GeometryPrecision < PatternGeometryLimit
                ? result.ExtraErrorPosition
                : (result.IsTooLong ? result.ExtraErrorPosition : result.MissingErrorPosition);

            return result;
        }

        private ProjectionStats GetProjectionStats(List<PositionInt> pixels, Vector2 axis)
        {
            if (pixels == null || pixels.Count == 0)
            {
                return new ProjectionStats();
            }

            float min = Vector2.Dot(new Vector2(pixels[0].x, pixels[0].y), axis);
            float max = min;
            foreach (var point in pixels)
            {
                float projection = Vector2.Dot(new Vector2(point.x, point.y), axis);
                if (projection < min) min = projection;
                if (projection > max) max = projection;
            }

            return new ProjectionStats
            {
                Min = min,
                Max = max,
                Length = Mathf.Max(0f, max - min)
            };
        }

        private float GetPatternEndpointLimit(float standardLength, float tolerance)
        {
            return Mathf.Clamp(Mathf.Max(tolerance * 2f, standardLength * 0.08f), 12f, 36f);
        }

        private int GetPatternMacroBinCount(float standardLength, float tolerance)
        {
            float binSize = Mathf.Max(tolerance * 2.5f, 22f);
            return Mathf.Clamp(Mathf.CeilToInt(standardLength / binSize), 4, 16);
        }

        private float GetPatternMacroCoverageLimit(float standardLength)
        {
            if (standardLength < 80f)
            {
                return 0.60f;
            }

            if (standardLength < 160f)
            {
                return 0.68f;
            }

            return 0.72f;
        }

        private Vector2 GetProjectionEdgePoint(List<PositionInt> pixels, Vector2 axis, float targetProjection)
        {
            if (pixels == null || pixels.Count == 0)
            {
                return Vector2.zero;
            }

            PositionInt best = pixels[0];
            float bestDistance = Mathf.Abs(Vector2.Dot(new Vector2(best.x, best.y), axis) - targetProjection);
            foreach (var point in pixels)
            {
                float distance = Mathf.Abs(Vector2.Dot(new Vector2(point.x, point.y), axis) - targetProjection);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    best = point;
                }
            }

            return new Vector2(best.x, best.y);
        }

        private LayerPixelCache GetStudentPixelCache(LayerManager layer)
        {
            if (_studentPixelCache != null && _studentPixelCache.IsValidFor(layer))
            {
                return _studentPixelCache;
            }

            using (ScoringPerf.Scope("AnswerCheck.StudentPixelCache", $"size={layer.LayerSize.width}x{layer.LayerSize.height}"))
            {
                _studentPixelCache = BuildLayerPixelCache(layer);
            }
            return _studentPixelCache;
        }

        private LayerPixelCache BuildLayerPixelCache(LayerManager layer)
        {
            List<PositionInt> pixels = ExtractOpaquePixels(layer.Image_colors, layer.LayerSize.width, layer.LayerSize.height);
            List<PositionInt>[] tileBuckets = BuildPixelTileBuckets(pixels, layer.LayerSize.width, layer.LayerSize.height, StudentPixelTileSize, out int tileColumns, out int tileRows);
            return new LayerPixelCache
            {
                Layer = layer,
                Source = layer.Image_colors,
                Width = layer.LayerSize.width,
                Height = layer.LayerSize.height,
                Pixels = pixels,
                PointSet = BuildPointSet(pixels),
                TileSize = StudentPixelTileSize,
                TileColumns = tileColumns,
                TileRows = tileRows,
                TileBuckets = tileBuckets
            };
        }

        private List<PositionInt> GetStandardPixels(LayerManager layer)
        {
            if (layer.data != null && layer.data.Length > 0)
            {
                List<PositionInt> pixels = new List<PositionInt>(Math.Min(layer.data.Length, 4096));
                HashSet<long> seen = new HashSet<long>();
                int width = layer.LayerSize.width;
                int height = layer.LayerSize.height;
                foreach (var point in layer.data)
                {
                    if (point == null || point.x < 0 || point.x >= width || point.y < 0 || point.y >= height)
                    {
                        continue;
                    }

                    if (seen.Add(PointKey(point.x, point.y)))
                    {
                        pixels.Add(point);
                    }
                }
                if (ScoringPerf.VerboseLayerLogs && pixels.Count != layer.data.Length)
                {
                    Debug.Log($"[AnswerCheck] 标准答案点集去重 layerNum={layer.layerNum}, raw={layer.data.Length}, unique={pixels.Count}");
                }
                return pixels;
            }

            return ExtractOpaquePixels(layer.Image_colors, layer.LayerSize.width, layer.LayerSize.height);
        }

        private List<PositionInt> ExtractOpaquePixels(Color32[] colors, int width, int height)
        {
            List<PositionInt> pixels = new List<PositionInt>();
            if (colors == null || width <= 0 || height <= 0)
            {
                return pixels;
            }

            int max = Mathf.Min(colors.Length, width * height);
            for (int y = 0; y < height; y++)
            {
                int row = y * width;
                if (row >= max)
                {
                    break;
                }

                int rowEnd = Mathf.Min(row + width, max);
                for (int index = row; index < rowEnd; index++)
                {
                    if (colors[index].a > 0)
                    {
                        pixels.Add(new PositionInt(index - row, y));
                    }
                }
            }
            return pixels;
        }

        private List<PositionInt>[] BuildPixelTileBuckets(List<PositionInt> pixels, int width, int height, int tileSize, out int tileColumns, out int tileRows)
        {
            tileColumns = 0;
            tileRows = 0;
            if (pixels == null || pixels.Count == 0 || width <= 0 || height <= 0 || tileSize <= 0)
            {
                return Array.Empty<List<PositionInt>>();
            }

            tileColumns = (width + tileSize - 1) / tileSize;
            tileRows = (height + tileSize - 1) / tileSize;
            List<PositionInt>[] buckets = new List<PositionInt>[tileColumns * tileRows];
            foreach (var point in pixels)
            {
                if (point == null || point.x < 0 || point.x >= width || point.y < 0 || point.y >= height)
                {
                    continue;
                }

                int tileX = point.x / tileSize;
                int tileY = point.y / tileSize;
                int index = tileX + tileY * tileColumns;
                List<PositionInt> bucket = buckets[index];
                if (bucket == null)
                {
                    bucket = new List<PositionInt>();
                    buckets[index] = bucket;
                }
                bucket.Add(point);
            }
            return buckets;
        }

        private List<PositionInt> GetPixelsInsideBounds(List<PositionInt> pixels, PixelBounds bounds)
        {
            List<PositionInt> result = new List<PositionInt>();
            foreach (var point in pixels)
            {
                if (IsInsideBounds(point, bounds))
                {
                    result.Add(point);
                }
            }
            return result;
        }

        private LineLimits GetLineLimits(LayerManager standardLayer)
        {
            float errorLimit = defaultErrorLimit;
            float rightLimit = defaultRightLimit;

            if (errorLimitDic == null)
            {
                errorLimitDic = LoadErrorLimitConfig();
            }

            string titleId = TeacherMainManager.instance != null ? TeacherMainManager.instance.CurrentTitleIdForUpload : string.Empty;
            if (!string.IsNullOrEmpty(titleId) && errorLimitDic != null && errorLimitDic.TryGetValue(titleId, out var limits))
            {
                foreach (var limit in limits)
                {
                    if (limit.index == standardLayer.layerNum)
                    {
                        errorLimit = limit.errorLimit;
                        rightLimit = limit.rightLimit;
                        if (ScoringPerf.VerboseLayerLogs)
                        {
                            Debug.Log($"使用配置表中的误差阈值: errorLimit={errorLimit}, rightLimit={rightLimit} for layerNum={standardLayer.layerNum}");
                        }
                        break;
                    }
                }
            }

            return new LineLimits
            {
                errorLimit = Mathf.Clamp(errorLimit, 0.18f, 0.45f),
                rightLimit = Mathf.Clamp(rightLimit + 0.25f, 0.55f, 0.78f),
                precisionLimit = 0.45f
            };
        }

        private List<PositionInt> DilateRepeated(List<PositionInt> src, int count, int width, int height)
        {
            HashSet<long> set = new HashSet<long>(src.Count * (count * 4 + 1));
            foreach (var point in src)
            {
                for (int dx = -count; dx <= count; dx++)
                {
                    int remainingY = count - Mathf.Abs(dx);
                    for (int dy = -remainingY; dy <= remainingY; dy++)
                    {
                        int x = point.x + dx;
                        int y = point.y + dy;
                        if (x >= 0 && x < width && y >= 0 && y < height)
                        {
                            set.Add(PointKey(x, y));
                        }
                    }
                }
            }

            List<PositionInt> result = new List<PositionInt>(set.Count);
            foreach (long key in set)
            {
                result.Add(new PositionInt((int)(key >> 32), (int)(key & 0xffffffff)));
            }
            return result;
        }

        private Vector2Int FindBestOffset(List<PositionInt> standardPixels, LayerPixelCache student, int maxOffset, int step)
        {
            int standardCount = standardPixels == null ? 0 : standardPixels.Count;
            if (standardCount == 0 || student == null || student.Source == null)
            {
                return Vector2Int.zero;
            }

            Color32[] studentSource = student.Source;
            int studentWidth = student.Width;
            int studentHeight = student.Height;
            int sourceLength = studentSource.Length;
            int bestOverlap = -1;
            Vector2Int bestOffset = Vector2Int.zero;
            for (int y = -maxOffset; y <= maxOffset; y += step)
            {
                for (int x = -maxOffset; x <= maxOffset; x += step)
                {
                    int overlap = 0;
                    for (int i = 0; i < standardCount; i++)
                    {
                        PositionInt point = standardPixels[i];
                        int sx = point.x + x;
                        int sy = point.y + y;
                        if ((uint)sx < (uint)studentWidth && (uint)sy < (uint)studentHeight)
                        {
                            int index = sx + sy * studentWidth;
                            if (index < sourceLength && studentSource[index].a > 0)
                            {
                                overlap++;
                            }
                        }

                        int remaining = standardCount - i - 1;
                        if (overlap + remaining <= bestOverlap)
                        {
                            break;
                        }
                    }

                    if (overlap > bestOverlap)
                    {
                        bestOverlap = overlap;
                        bestOffset = new Vector2Int(x, y);
                        if (bestOverlap >= standardCount * 0.995f)
                        {
                            return bestOffset;
                        }
                    }
                }
            }
            return bestOffset;
        }

        private List<PositionInt> ShiftPositions(List<PositionInt> src, Vector2Int offset)
        {
            List<PositionInt> result = new List<PositionInt>(src.Count);
            foreach (var point in src)
            {
                result.Add(new PositionInt(point.x + offset.x, point.y + offset.y));
            }
            return result;
        }

        private HashSet<long> BuildPointSet(List<PositionInt> points)
        {
            HashSet<long> set = new HashSet<long>(points.Count);
            foreach (var point in points)
            {
                set.Add(PointKey(point.x, point.y));
            }
            return set;
        }

        private static long PointKey(int x, int y)
        {
            return ((long)x << 32) ^ (uint)y;
        }

        private bool TryFindNearestDistance(HashSet<long> targetSet, PositionInt point, int maxDistance, out float distance)
        {
            int bestSq = int.MaxValue;
            int maxSq = maxDistance * maxDistance;
            for (int dx = -maxDistance; dx <= maxDistance; dx++)
            {
                for (int dy = -maxDistance; dy <= maxDistance; dy++)
                {
                    int sq = dx * dx + dy * dy;
                    if (sq > maxSq || sq >= bestSq)
                    {
                        continue;
                    }

                    if (targetSet.Contains(PointKey(point.x + dx, point.y + dy)))
                    {
                        bestSq = sq;
                    }
                }
            }

            if (bestSq == int.MaxValue)
            {
                distance = maxDistance + 1f;
                return false;
            }

            distance = Mathf.Sqrt(bestSq);
            return true;
        }

        private bool ShouldUseDistanceField(int queryCount, PixelBounds fieldBounds, int maxDistance)
        {
            if (!UseDistanceFieldNearestSearch || queryCount <= 0 || maxDistance <= 0)
            {
                return false;
            }

            long fieldArea = GetBoundsArea(fieldBounds);
            if (fieldArea <= 0 || fieldArea > MaxDistanceFieldArea)
            {
                return false;
            }

            long diameter = maxDistance * 2L + 1L;
            long radiusScanCost = queryCount * diameter * diameter;
            long distanceFieldCost = fieldArea * DistanceFieldCostMultiplier;
            return distanceFieldCost <= radiusScanCost;
        }

        private static int GetBoundsWidth(PixelBounds bounds)
        {
            return bounds.maxX >= bounds.minX ? bounds.maxX - bounds.minX + 1 : 0;
        }

        private static int GetBoundsHeight(PixelBounds bounds)
        {
            return bounds.maxY >= bounds.minY ? bounds.maxY - bounds.minY + 1 : 0;
        }

        private static long GetBoundsArea(PixelBounds bounds)
        {
            return (long)GetBoundsWidth(bounds) * GetBoundsHeight(bounds);
        }

        private static string DescribeBounds(PixelBounds bounds)
        {
            return $"{GetBoundsWidth(bounds)}x{GetBoundsHeight(bounds)}";
        }

        private float GetLineTolerance(List<PositionInt> standardPixels)
        {
            PixelBounds bounds = GetBounds(standardPixels);
            float diagonal = Vector2.Distance(new Vector2(bounds.minX, bounds.minY), new Vector2(bounds.maxX, bounds.maxY));
            return Mathf.Clamp(diagonal * 0.006f + 6f, 8f, 18f);
        }

        private PixelBounds GetBounds(List<PositionInt> pixels)
        {
            PixelBounds bounds = new PixelBounds
            {
                minX = pixels[0].x,
                maxX = pixels[0].x,
                minY = pixels[0].y,
                maxY = pixels[0].y
            };

            foreach (var point in pixels)
            {
                if (point.x < bounds.minX) bounds.minX = point.x;
                if (point.x > bounds.maxX) bounds.maxX = point.x;
                if (point.y < bounds.minY) bounds.minY = point.y;
                if (point.y > bounds.maxY) bounds.maxY = point.y;
            }

            return bounds;
        }

        private PixelBounds ExpandBounds(PixelBounds bounds, int amount)
        {
            bounds.minX -= amount;
            bounds.maxX += amount;
            bounds.minY -= amount;
            bounds.maxY += amount;
            return bounds;
        }

        private static bool IsInsideBounds(PositionInt point, PixelBounds bounds)
        {
            return point.x >= bounds.minX && point.x <= bounds.maxX && point.y >= bounds.minY && point.y <= bounds.maxY;
        }

        private List<PositionInt> FilterStudentCandidateComponents(List<PositionInt> candidatePixels, List<PositionInt> shiftedStandardPixels, float tolerance, int layerNum)
        {
            if (candidatePixels == null || candidatePixels.Count == 0)
            {
                return new List<PositionInt>();
            }

            if (shiftedStandardPixels == null || shiftedStandardPixels.Count == 0)
            {
                return candidatePixels;
            }

            int maxDistance = Mathf.CeilToInt(tolerance * 3f);
            int maxDistanceSq = maxDistance * maxDistance;
            DistanceField standardDistanceField = null;
            HashSet<long> standardSet = null;
            PixelBounds filterBounds = ExpandBounds(GetBounds(candidatePixels), maxDistance);
            bool useDistanceField = UseDistanceFieldNearestSearch && GetBoundsArea(filterBounds) <= MaxDistanceFieldArea;
            if (useDistanceField)
            {
                using (ScoringPerf.ScopeDetailed(ScoringPerf.TitleKey($"AnswerCheck.CandidateFilterField#{layerNum}", ScoringPerf.CurrentTitleId), $"targets={shiftedStandardPixels.Count};candidates={candidatePixels.Count};roi={DescribeBounds(filterBounds)};maxDistance={maxDistance}"))
                {
                    standardDistanceField = DistanceField.Build(shiftedStandardPixels, filterBounds);
                }
            }
            else
            {
                standardSet = BuildPointSet(shiftedStandardPixels);
            }

            List<List<PositionInt>> components;
            using (ScoringPerf.ScopeDetailed(ScoringPerf.TitleKey($"AnswerCheck.SplitComponents#{layerNum}", ScoringPerf.CurrentTitleId), $"candidates={candidatePixels.Count};mode={(useDistanceField ? "distanceField" : "radiusScan")}"))
            {
                components = SplitConnectedComponents(candidatePixels);
            }

            List<PositionInt> filtered = new List<PositionInt>(candidatePixels.Count);

            using (ScoringPerf.ScopeDetailed(ScoringPerf.TitleKey($"AnswerCheck.ComponentNearQuery#{layerNum}", ScoringPerf.CurrentTitleId), $"components={components.Count};candidates={candidatePixels.Count};maxDistance={maxDistance};mode={(useDistanceField ? "distanceField" : "radiusScan")}"))
            {
                foreach (var component in components)
                {
                    int sampleStep = Mathf.Max(1, component.Count / 300);
                    int nearCount = 0;
                    int sampled = 0;

                    for (int i = 0; i < component.Count; i += sampleStep)
                    {
                        sampled++;
                        bool nearStandard = useDistanceField
                            ? standardDistanceField.TryGetDistanceSq(component[i], maxDistanceSq, out _)
                            : TryFindNearestDistance(standardSet, component[i], maxDistance, out float distance) && distance <= maxDistance;
                        if (nearStandard)
                        {
                            nearCount++;
                        }
                    }

                    float nearRatio = sampled == 0 ? 0f : nearCount / (float)sampled;
                    if (nearCount >= 2 || nearRatio >= 0.08f)
                    {
                        filtered.AddRange(component);
                    }
                }
            }

            return filtered.Count > 0 ? filtered : candidatePixels;
        }

        private List<PositionInt> FilterStraightLineBand(List<PositionInt> candidatePixels, List<PositionInt> shiftedStandardPixels, Vector2 axis, float tolerance)
        {
            if (candidatePixels == null || candidatePixels.Count == 0 || shiftedStandardPixels == null || shiftedStandardPixels.Count == 0)
            {
                return candidatePixels ?? new List<PositionInt>();
            }

            Vector2 center = GetAveragePoint(shiftedStandardPixels);
            Vector2 normal = new Vector2(-axis.y, axis.x).normalized;
            float minProjection = Vector2.Dot(new Vector2(shiftedStandardPixels[0].x, shiftedStandardPixels[0].y), axis);
            float maxProjection = minProjection;
            foreach (var point in shiftedStandardPixels)
            {
                float projection = Vector2.Dot(new Vector2(point.x, point.y), axis);
                if (projection < minProjection) minProjection = projection;
                if (projection > maxProjection) maxProjection = projection;
            }

            float projectedLength = Mathf.Max(1f, maxProjection - minProjection);
            float projectionSlack = Mathf.Max(tolerance * 6f, projectedLength * 0.25f);
            float bandWidth = tolerance * 2.2f;
            List<PositionInt> filtered = new List<PositionInt>();
            foreach (var point in candidatePixels)
            {
                Vector2 p = new Vector2(point.x, point.y);
                float projection = Vector2.Dot(p, axis);
                float perpendicularDistance = Mathf.Abs(Vector2.Dot(p - center, normal));
                if (perpendicularDistance <= bandWidth && projection >= minProjection - projectionSlack && projection <= maxProjection + projectionSlack)
                {
                    filtered.Add(point);
                }
            }

            return filtered.Count > 0 ? filtered : candidatePixels;
        }

        private List<PositionInt> FilterNeighborStandardLinePixels(List<PositionInt> candidatePixels, LayerManager currentLayer, Vector2Int currentOffset, List<PositionInt> shiftedCurrentPixels, Vector2 axis, float tolerance)
        {
            if (candidatePixels == null || candidatePixels.Count == 0
                || currentLayer == null
                || currentLayer.lineshape != lineshape.直线
                || shiftedCurrentPixels == null
                || shiftedCurrentPixels.Count == 0
                || standardlayer_manager == null
                || standardlayer_manager.Count <= 1)
            {
                return candidatePixels ?? new List<PositionInt>();
            }

            ProjectionStats currentStats = GetProjectionStats(shiftedCurrentPixels, axis);
            if (currentStats.Length <= 0f)
            {
                return candidatePixels;
            }

            Vector2 currentCenter = GetAveragePoint(shiftedCurrentPixels);
            Vector2 normal = new Vector2(-axis.y, axis.x).normalized;
            float neighborSearchSlack = Mathf.Clamp(Mathf.Max(tolerance * 1.5f, currentStats.Length * 0.04f), 8f, 24f);
            float endpointKeepSlack = Mathf.Clamp(tolerance * 0.35f, 2f, 4f);
            bool hasOutsideCurrentSpan = false;
            foreach (var point in candidatePixels)
            {
                float projection = Vector2.Dot(new Vector2(point.x, point.y), axis);
                if (projection < currentStats.Min - endpointKeepSlack || projection > currentStats.Max + endpointKeepSlack)
                {
                    hasOutsideCurrentSpan = true;
                    break;
                }
            }

            if (!hasOutsideCurrentSpan)
            {
                return candidatePixels;
            }

            float collinearLimit = Mathf.Max(tolerance * 2.5f, 12f);
            int nearDistance = Mathf.CeilToInt(Mathf.Max(tolerance * 2.5f, 10f));
            int nearDistanceSq = nearDistance * nearDistance;
            List<PositionInt> neighborPixels = new List<PositionInt>();

            foreach (var otherLayer in standardlayer_manager)
            {
                if (otherLayer == null
                    || ReferenceEquals(otherLayer, currentLayer)
                    || otherLayer.layerNum == currentLayer.layerNum
                    || otherLayer.lineshape != lineshape.直线
                    || otherLayer.lineType == linetype.unknown)
                {
                    continue;
                }

                List<PositionInt> otherPixels = GetStandardPixels(otherLayer);
                if (otherPixels.Count == 0)
                {
                    continue;
                }

                Vector2 otherAxis = GetPrincipalAxis(otherPixels);
                if (Mathf.Abs(Vector2.Dot(axis.normalized, otherAxis.normalized)) < 0.92f)
                {
                    continue;
                }

                List<PositionInt> shiftedOtherPixels = ShiftPositions(otherPixels, currentOffset);
                Vector2 otherCenter = GetAveragePoint(shiftedOtherPixels);
                float centerDistanceFromCurrentLine = Mathf.Abs(Vector2.Dot(otherCenter - currentCenter, normal));
                if (centerDistanceFromCurrentLine > collinearLimit)
                {
                    continue;
                }

                ProjectionStats otherStats = GetProjectionStats(shiftedOtherPixels, axis);
                bool canOverlapCandidateSpan = otherStats.Max >= currentStats.Min - nearDistance - neighborSearchSlack
                                               && otherStats.Min <= currentStats.Max + nearDistance + neighborSearchSlack;
                if (!canOverlapCandidateSpan)
                {
                    continue;
                }

                neighborPixels.AddRange(shiftedOtherPixels);
            }

            if (neighborPixels.Count == 0)
            {
                return candidatePixels;
            }

            PixelBounds neighborQueryBounds = ExpandBounds(GetBounds(candidatePixels), nearDistance);
            bool useDistanceField = UseDistanceFieldNearestSearch && GetBoundsArea(neighborQueryBounds) <= MaxDistanceFieldArea;
            DistanceField neighborDistanceField = null;
            HashSet<long> neighborSet = null;
            if (useDistanceField)
            {
                neighborDistanceField = DistanceField.Build(neighborPixels, neighborQueryBounds);
            }
            else
            {
                neighborSet = BuildPointSet(neighborPixels);
            }

            List<PositionInt> filtered = new List<PositionInt>(candidatePixels.Count);
            foreach (var point in candidatePixels)
            {
                float projection = Vector2.Dot(new Vector2(point.x, point.y), axis);
                bool outsideCurrentSpan = projection < currentStats.Min - endpointKeepSlack || projection > currentStats.Max + endpointKeepSlack;
                if (outsideCurrentSpan)
                {
                    bool nearNeighbor = useDistanceField
                        ? neighborDistanceField.TryGetDistanceSq(point, nearDistanceSq, out _)
                        : TryFindNearestDistance(neighborSet, point, nearDistance, out float distance) && distance <= nearDistance;
                    if (nearNeighbor)
                    {
                        continue;
                    }
                }

                filtered.Add(point);
            }

            return filtered.Count > 0 ? filtered : candidatePixels;
        }

        private List<List<PositionInt>> SplitConnectedComponents(List<PositionInt> pixels)
        {
            Dictionary<long, PositionInt> pointMap = new Dictionary<long, PositionInt>(pixels.Count);
            foreach (var point in pixels)
            {
                long key = PointKey(point.x, point.y);
                if (!pointMap.ContainsKey(key))
                {
                    pointMap.Add(key, point);
                }
            }

            HashSet<long> visited = new HashSet<long>(pointMap.Count);
            List<List<PositionInt>> components = new List<List<PositionInt>>();
            foreach (var entry in pointMap)
            {
                if (visited.Contains(entry.Key))
                {
                    continue;
                }

                List<PositionInt> component = new List<PositionInt>();
                Queue<PositionInt> queue = new Queue<PositionInt>();
                queue.Enqueue(entry.Value);
                visited.Add(entry.Key);

                while (queue.Count > 0)
                {
                    PositionInt current = queue.Dequeue();
                    component.Add(current);
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            if (dx == 0 && dy == 0)
                            {
                                continue;
                            }

                            long neighborKey = PointKey(current.x + dx, current.y + dy);
                            if (visited.Contains(neighborKey) || !pointMap.TryGetValue(neighborKey, out var neighbor))
                            {
                                continue;
                            }

                            visited.Add(neighborKey);
                            queue.Enqueue(neighbor);
                        }
                    }
                }

                components.Add(component);
            }

            return components;
        }

        private Vector2 GetAveragePoint(List<PositionInt> pixels)
        {
            if (pixels == null || pixels.Count == 0)
            {
                return Vector2.zero;
            }

            float x = 0f;
            float y = 0f;
            foreach (var point in pixels)
            {
                x += point.x;
                y += point.y;
            }

            return new Vector2(x / pixels.Count, y / pixels.Count);
        }

        private float Percentile(List<float> values, float percentile)
        {
            if (values == null || values.Count == 0)
            {
                return 0f;
            }

            values.Sort();
            int index = Mathf.Clamp(Mathf.RoundToInt((values.Count - 1) * percentile), 0, values.Count - 1);
            return values[index];
        }

        private Vector2 GetPrincipalAxis(List<PositionInt> pixels)
        {
            Vector2 center = GetAveragePoint(pixels);
            double xx = 0;
            double yy = 0;
            double xy = 0;
            foreach (var point in pixels)
            {
                double dx = point.x - center.x;
                double dy = point.y - center.y;
                xx += dx * dx;
                yy += dy * dy;
                xy += dx * dy;
            }

            double angle = 0.5 * Math.Atan2(2 * xy, xx - yy);
            Vector2 axis = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            return axis.sqrMagnitude > 0.0001f ? axis.normalized : Vector2.right;
        }

        private float GetProjectedLength(List<PositionInt> pixels, Vector2 axis)
        {
            if (pixels == null || pixels.Count == 0)
            {
                return 0f;
            }

            float min = Vector2.Dot(new Vector2(pixels[0].x, pixels[0].y), axis);
            float max = min;
            foreach (var point in pixels)
            {
                float projection = Vector2.Dot(new Vector2(point.x, point.y), axis);
                if (projection < min) min = projection;
                if (projection > max) max = projection;
            }
            return max - min;
        }

        private linetype RecognizeLineTypeByProjection(List<PositionInt> pixels, Vector2 axis, bool preferSolidForShortInterruptedRuns = false, float referenceProjectedLength = 0f)
        {
            if (pixels == null || pixels.Count == 0)
            {
                return linetype.unknown;
            }

            float minProjection = Vector2.Dot(new Vector2(pixels[0].x, pixels[0].y), axis);
            float maxProjection = minProjection;
            foreach (var point in pixels)
            {
                float projection = Vector2.Dot(new Vector2(point.x, point.y), axis);
                if (projection < minProjection) minProjection = projection;
                if (projection > maxProjection) maxProjection = projection;
            }

            float length = maxProjection - minProjection;
            if (length < 12f)
            {
                return linetype.实线;
            }

            const float binSize = 3f;
            int binCount = Mathf.Max(1, Mathf.CeilToInt(length / binSize) + 1);
            bool[] occupied = new bool[binCount];
            foreach (var point in pixels)
            {
                float projection = Vector2.Dot(new Vector2(point.x, point.y), axis);
                int bin = Mathf.Clamp(Mathf.RoundToInt((projection - minProjection) / binSize), 0, binCount - 1);
                occupied[bin] = true;
            }

            FillSmallGaps(occupied, 2);
            BuildRuns(occupied, out List<int> inkRuns, out List<int> gapRuns);

            if (inkRuns.Count <= 1 || gapRuns.Count == 0)
            {
                return linetype.实线;
            }

            int totalInk = inkRuns.Sum();
            int totalGap = gapRuns.Sum();
            float gapRatio = totalGap / (float)(totalInk + totalGap);
            if (gapRatio < 0.08f)
            {
                return linetype.实线;
            }

            float shortRunLengthLimit = referenceProjectedLength > 0f
                ? Mathf.Clamp(referenceProjectedLength * 1.25f, 36f, 96f)
                : 96f;
            if (preferSolidForShortInterruptedRuns
                && inkRuns.Count <= 2
                && length <= shortRunLengthLimit
                && gapRatio <= 0.28f)
            {
                return linetype.实线;
            }

            List<int> sortedInkRuns = inkRuns.OrderBy(v => v).ToList();
            float shortInk = sortedInkRuns[Mathf.Clamp(sortedInkRuns.Count / 4, 0, sortedInkRuns.Count - 1)];
            float longInk = sortedInkRuns[Mathf.Clamp((sortedInkRuns.Count * 3) / 4, 0, sortedInkRuns.Count - 1)];
            bool hasDotLikeRuns = shortInk <= Mathf.Max(2f, longInk * 0.32f);
            bool hasStableLongRuns = longInk >= Mathf.Max(3f, shortInk * 2.6f);
            if (hasDotLikeRuns && hasStableLongRuns)
            {
                int longCount = inkRuns.Count(v => v >= longInk * 0.75f);
                int dotCount = inkRuns.Count(v => v <= Mathf.Max(2f, longInk * 0.32f));
                if (longCount > 0 && dotCount > 0)
                {
                    return dotCount >= longCount * 1.4f ? linetype.双点画线 : linetype.点画线;
                }
            }

            return linetype.虚线;
        }

        private void FillSmallGaps(bool[] occupied, int maxGapLength)
        {
            int i = 0;
            while (i < occupied.Length)
            {
                if (occupied[i])
                {
                    i++;
                    continue;
                }

                int start = i;
                while (i < occupied.Length && !occupied[i])
                {
                    i++;
                }

                int end = i - 1;
                bool hasLeftInk = start > 0 && occupied[start - 1];
                bool hasRightInk = i < occupied.Length && occupied[i];
                if (hasLeftInk && hasRightInk && end - start + 1 <= maxGapLength)
                {
                    for (int j = start; j <= end; j++)
                    {
                        occupied[j] = true;
                    }
                }
            }
        }

        private void BuildRuns(bool[] occupied, out List<int> inkRuns, out List<int> gapRuns)
        {
            inkRuns = new List<int>();
            gapRuns = new List<int>();
            if (occupied == null || occupied.Length == 0)
            {
                return;
            }

            bool current = occupied[0];
            int count = 1;
            for (int i = 1; i < occupied.Length; i++)
            {
                if (occupied[i] == current)
                {
                    count++;
                    continue;
                }

                if (current) inkRuns.Add(count);
                else gapRuns.Add(count);
                current = occupied[i];
                count = 1;
            }

            if (current) inkRuns.Add(count);
            else gapRuns.Add(count);

            if (occupied.Length > 0 && !occupied[0] && gapRuns.Count > 0)
            {
                gapRuns.RemoveAt(0);
            }
            if (occupied.Length > 1 && !occupied[^1] && gapRuns.Count > 0)
            {
                gapRuns.RemoveAt(gapRuns.Count - 1);
            }
        }

        private static bool HasValidMarkBox(List<List<PositionInt>> markData)
        {
            return markData != null
                   && markData.Count > 0
                   && markData[0] != null
                   && markData[0].Count > 2
                   && markData[0][0] != null
                   && markData[0][2] != null;
        }

        ErrorReson IsMarkPositionOk(List<List<PositionInt>> studentMarkData, List<List<PositionInt>> standardMarkData, int biaoshiValue,string biaoshis)
        {
            List<PositionInt> stand_mark = new List<PositionInt>();
            int standardMarkCount = standardMarkData == null ? 0 : standardMarkData.Count;
            int studentMarkCount = studentMarkData == null ? 0 : studentMarkData.Count;
            Debug.Log("standardMarkData.count" + standardMarkCount);
            Debug.Log("biaoshiValue" + biaoshiValue);
            Debug.Log("studentMarkData.count" + studentMarkCount);
            if (biaoshiValue == 0 || standardMarkCount == 0 || studentMarkCount == 0)
            {
                return ErrorReson.在标注位置未发现对应尺寸;
            }
            if (TeacherMainManager.instance == null || TeacherMainManager.instance.transform.childCount == 0)
            {
                return ErrorReson.在标注位置未发现对应尺寸;
            }

            GameObject StudentLayer = TeacherMainManager.instance.transform.GetChild(0).gameObject;
            foreach (var item in standardMarkData)
            {
                if (item == null || item.Count < 4)
                {
                    continue;
                }

                stand_mark = item;

                int standard_width_min = stand_mark[0].x;
                int standard_width_max = stand_mark[0].x;
                int standard_height_min = stand_mark[0].y;
                int standard_height_max = stand_mark[0].y;

                for (int i = 1; i < stand_mark.Count; i++)
                {
                    if (stand_mark[i].x < standard_width_min)
                    {
                        standard_width_min = stand_mark[i].x;
                    }
                    if (stand_mark[i].x > standard_width_max)
                    {
                        standard_width_max = stand_mark[i].x;
                    }
                    if (stand_mark[i].y < standard_height_min)
                    {
                        standard_height_min = stand_mark[i].y;
                    }
                    if (stand_mark[i].y > standard_height_max)
                    {
                        standard_height_max = stand_mark[i].y;
                    }
                }
                if (debugDrawMarkBounds) {
                    _debugStandardRect = new Rect(
                        standard_width_min,
                        standard_height_min,
                        standard_width_max - standard_width_min,
                        standard_height_max - standard_height_min
                    );
                }

                //标识中心点判分
                bool isCorrect = false;
                int incorrectIndex = -1;
                //新增阈值判断参数（建议放在类成员变量位置）
                float areaThreshold = 0.3f; // 面积允许误差范围±20%
                float distanceThreshold = 100f; // 中心点距离误差


                Vector2 standard_Center0_1Pos = GetMidpoint(new Vector2(standard_width_min, standard_height_min), new Vector2(standard_width_max, standard_height_min));
                Vector2 standard_Center1_2Pos = GetMidpoint(new Vector2(standard_width_max, standard_height_min), new Vector2(standard_width_max, standard_height_max));
                Vector2 standard_Center2_3Pos = GetMidpoint(new Vector2(standard_width_max, standard_height_max), new Vector2(standard_width_min, standard_height_max));
                Vector2 standard_Center3_4Pos = GetMidpoint(new Vector2(standard_width_min, standard_height_max), new Vector2(standard_width_min, standard_height_min));

                float standard_width = Vector2.Distance(standard_Center1_2Pos, standard_Center3_4Pos);
                float standard_height = Vector2.Distance(standard_Center0_1Pos, standard_Center2_3Pos);
                Vector2 standard_width_Center1Pos = standard_Center1_2Pos;
                Vector2 standard_width_Center2Pos = standard_Center3_4Pos;
                Vector2 standard_height_Center1Pos = standard_Center0_1Pos;
                Vector2 standard_height_Center2Pos = standard_Center2_3Pos;

                bool isHorizontal = false;
                if (standard_width < standard_height)
                {
                    float oldstandard_width = standard_width;
                    Debug.Log("oldstandard_width" + oldstandard_width);
                    standard_width = standard_height;
                    Debug.Log("standard_width" + standard_width);
                    standard_height = oldstandard_width;
                    Debug.Log("standard_height" + standard_height);
                    standard_width_Center1Pos = standard_Center0_1Pos;
                    standard_width_Center2Pos = standard_Center2_3Pos;
                    standard_height_Center1Pos = standard_Center1_2Pos;
                    standard_height_Center2Pos = standard_Center3_4Pos;

                }
                if (standard_width_Center1Pos.x == standard_width_Center2Pos.x)
                {
                    isHorizontal = false;
                }
                else if (standard_width_Center1Pos.y == standard_width_Center2Pos.y)
                {
                    isHorizontal = true;
                }

                float standardArea = (standard_width) * (standard_height);
                List<List<PositionInt>> studentMarkBySaveAreaData = new List<List<PositionInt>>();
                if (studentMarkData.Count>0)
                {
                    for (int s = 0; s < studentMarkData.Count; s++)
                    {
                        if (studentMarkData[s] == null || studentMarkData[s].Count < 5 || s >= StudentLayer.transform.childCount)
                        {
                            continue;
                        }

                        if (StudentLayer.transform.childCount > 0)
                        {
                            if (StudentLayer.transform.GetChild(s) != null)
                            {
                                if (StudentLayer.transform.GetChild(s).gameObject.activeSelf)
                                {
                                    #region 遍历学生每一个标识的包围盒得到最大宽高并互相比对包围盒点是否在对方内部
                                    int student_width_min = studentMarkData[s][0].x;

                                    int student_width_max = studentMarkData[s][2].x;

                                    int student_height_min = studentMarkData[s][0].y;

                                    int student_height_max = studentMarkData[s][2].y;
                                    if (debugDrawMarkBounds) {
                                        _debugStudentRect = new Rect(
                                            student_width_min,
                                            student_height_min,
                                            student_width_max - student_width_min,
                                            student_height_max - student_height_min
                                        );
                                    }


                                    Vector2 student_CenterPos = GetMidpoint(new Vector2(student_width_min, student_height_min), new Vector2(student_width_max, student_height_max));
                                    if (student_CenterPos.x >= standard_width_min && student_CenterPos.x <= standard_width_max && student_CenterPos.y >= standard_height_min && student_CenterPos.y <= standard_height_max)
                                    {
                                        if (!string.IsNullOrEmpty(biaoshis) && biaoshis.Contains("#"))
                                        {
                                            string[] part = biaoshis.Split('#');
                                            Debug.Log("part0" + part[0]);
                                            Debug.Log("part1" + part[1]);
                                            Debug.Log("studentMarkData[s][4].x" + studentMarkData[s][4].x);
                                            if (part.Length >= 2
                                                && Enum.TryParse<标识>(part[0], out var markPart0)
                                                && Enum.TryParse<标识>(part[1], out var markPart1)
                                                && ((int)markPart0 == studentMarkData[s][4].x || (int)markPart1 == studentMarkData[s][4].x))
                                            {
                                                Debug.Log("标识中心点判分正确" + s);
                                                StudentLayer.transform.GetChild(s).gameObject.SetActive(false);
                                                isCorrect = true;
                                                break; // 找到正确结果，跳出循环
                                            }
                                        }
                                        else if (biaoshiValue == studentMarkData[s][4].x)
                                        {
                                            Debug.Log("标识中心点判分正确" + s);
                                            StudentLayer.transform.GetChild(s).gameObject.SetActive(false);
                                            isCorrect = true;
                                            break; // 找到正确结果，跳出循环
                                        }
                                        else
                                        {
                                            incorrectIndex = s; // 记录错误索引
                                        }

                                    }
                                    #endregion

                                    #region 计算标准包围盒面积


                                    if (true)//
                                    {
                                        Vector2 student_Center0_1Pos = GetMidpoint(new Vector2(studentMarkData[s][0].x, studentMarkData[s][0].y), new Vector2(studentMarkData[s][1].x, studentMarkData[s][1].y));
                                        Vector2 student_Center1_2Pos = GetMidpoint(new Vector2(studentMarkData[s][1].x, studentMarkData[s][1].y), new Vector2(studentMarkData[s][2].x, studentMarkData[s][2].y));
                                        Vector2 student_Center2_3Pos = GetMidpoint(new Vector2(studentMarkData[s][2].x, studentMarkData[s][2].y), new Vector2(studentMarkData[s][3].x, studentMarkData[s][3].y));
                                        Vector2 student_Center3_4Pos = GetMidpoint(new Vector2(studentMarkData[s][3].x, studentMarkData[s][3].y), new Vector2(studentMarkData[s][0].x, studentMarkData[s][0].y));
                                        float student_width = Vector2.Distance(student_Center1_2Pos, student_Center3_4Pos);
                                        float student_height = Vector2.Distance(student_Center0_1Pos, student_Center2_3Pos);

                                        Vector2 student_width_Center1Pos = student_Center1_2Pos;
                                        Vector2 student_width_Center2Pos = student_Center3_4Pos;
                                        Vector2 student_height_Center1Pos = student_Center0_1Pos;
                                        Vector2 student_height_Center2Pos = student_Center2_3Pos;
                                        if (student_width < student_height)
                                        {
                                            float oldstudent_width = student_width;
                                            Debug.Log("oldstudent_width" + oldstudent_width);
                                            student_width = student_height;
                                            Debug.Log("student_width" + student_width);
                                            student_height = oldstudent_width;
                                            Debug.Log("student_height" + student_height);
                                            student_width_Center1Pos = student_Center0_1Pos;
                                            student_width_Center2Pos = student_Center2_3Pos;
                                            student_height_Center1Pos = student_Center1_2Pos;
                                            student_height_Center2Pos = student_Center3_4Pos;
                                        }


                                        if (1 == biaoshiValue)//|| 2 == biaoshiValue || 3 == biaoshiValue
                                        {
                                            if (standard_width > standard_height)//standard_width > standard_height
                                            {
                                                Debug.Log("标识长度比例width" + (student_width / standard_width) + "标识长度width" + standard_width + "学生长度width" + student_width);


                                                Vector2 stu_centerPos = GetMidpoint(student_width_Center1Pos, student_width_Center2Pos);
                                                if (isHorizontal)
                                                {
                                                    Debug.Log("isHorizontal" + isHorizontal);
                                                    if ((stu_centerPos.x >= standard_width_Center1Pos.x && student_CenterPos.x <= standard_width_Center2Pos.x) || stu_centerPos.x >= standard_width_Center2Pos.x && student_CenterPos.x <= standard_width_Center1Pos.x)
                                                    {
                                                        Debug.Log("isHorizontal_x");
                                                        if (((student_width / standard_width) >= 0.9f || (standard_width / student_width) >= 0.9f)) // 新增长度比例判断
                                                        {
                                                            bool isPerpendicular = AreLinesPerpendicular(standard_height_Center1Pos, standard_height_Center2Pos, student_width_Center1Pos, student_width_Center2Pos);
                                                            Debug.Log("标识长度比例width是否平行与标准法线" + isPerpendicular);
                                                            if (isPerpendicular)
                                                            {
                                                                studentMarkBySaveAreaData.Add(studentMarkData[s]);
                                                                Debug.Log("标识长度width添加元素" + s);
                                                            }

                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    Debug.Log("isHorizontal" + isHorizontal);
                                                    if ((stu_centerPos.y >= standard_width_Center1Pos.y && student_CenterPos.y <= standard_width_Center2Pos.y) || stu_centerPos.y >= standard_width_Center2Pos.y && student_CenterPos.y <= standard_width_Center1Pos.y)
                                                    {
                                                        Debug.Log("noHorizontal_y");
                                                        if (((student_width / standard_width) >= 0.9f || (standard_width / student_width) >= 0.9f)) // 新增长度比例判断
                                                        {
                                                            bool isPerpendicular = AreLinesPerpendicular(standard_height_Center1Pos, standard_height_Center2Pos, student_width_Center1Pos, student_width_Center2Pos);
                                                            Debug.Log("标识长度比例height是否平行与标准法线" + isPerpendicular);
                                                            if (isPerpendicular)
                                                            {
                                                                studentMarkBySaveAreaData.Add(studentMarkData[s]);
                                                                Debug.Log("标识长度height添加元素" + s);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                }
                else
                {
                    return ErrorReson.在标注位置未发现对应尺寸;
                }
               


                if (isCorrect)
                {
                    return ErrorReson.正确;
                }
                #region 标识边框点判分                
                //标识边框点判分
                for (int s = 0; s < studentMarkData.Count; s++)
                {
                    if (studentMarkData[s] == null || studentMarkData[s].Count < 5 || s >= StudentLayer.transform.childCount)
                    {
                        continue;
                    }

                    if (StudentLayer.transform.childCount > 0&&StudentLayer.transform.GetChild(s) != null)
                    {
                        if (StudentLayer.transform.GetChild(s).gameObject.activeSelf)//&& biaoshiValue == studentMarkData[s][4].x
                        {
                            #region 遍历学生每一个标识的包围盒得到最大宽高并互相比对包围盒点是否在对方内部
                            int student_width_min = studentMarkData[s][0].x;
                            int student_width_max = studentMarkData[s][2].x;
                            int student_height_min = studentMarkData[s][0].y;
                            int student_height_max = studentMarkData[s][2].y;

                            Debug.Log("标识边框点判分1" + s);
                            for (int i = 1; i < studentMarkData[s].Count; i++)
                            {
                                if (studentMarkData[s][i].x >= standard_width_min && studentMarkData[s][i].x <= standard_width_max && studentMarkData[s][i].y >= standard_height_min && studentMarkData[s][i].y <= standard_height_max)
                                {

                                    if (biaoshiValue == studentMarkData[s][4].x)
                                    {
                                        Debug.Log("标识边框点判分正确" + s);
                                        StudentLayer.transform.GetChild(s).gameObject.SetActive(false);
                                        isCorrect = true;
                                        break; // 找到正确结果，跳出循环
                                    }
                                    else
                                    {
                                        incorrectIndex = s; // 记录错误索引
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }
                if (isCorrect)
                {
                    return ErrorReson.正确;
                }
                #endregion
                #region 相对位置标识判断
                for (int a = 0; a < studentMarkBySaveAreaData.Count; a++)
                {
                    Debug.Log("相对位置标识判断判分");
                    Debug.Log("相对位置标识判断判分正确");
                    isCorrect = true;
                    break; // 找到正确结果，跳出循环

                }
                studentMarkBySaveAreaData.Clear();
                if (isCorrect)
                {
                    return ErrorReson.正确;
                }
                else if (incorrectIndex != -1)
                {
                    Debug.Log("尺寸绘制不标准格式错误" + incorrectIndex + "标准答案" + biaoshiValue + "识别到的答案" + studentMarkData[incorrectIndex][4].x);
                    StudentLayer.transform.GetChild(incorrectIndex).gameObject.SetActive(false);
                    return ErrorReson.尺寸绘制不标准格式错误;
                }
                #endregion
            }
            return ErrorReson.在标注位置未发现对应尺寸;
        }
        #region 判断两个2D线段是否垂直
        public static bool AreLinesPerpendicular(Vector2 p1, Vector2 p2,
                                           Vector2 q1, Vector2 q2,
                                           float epsilon = 1e-6f)
        {
            // 计算两个线段的方向向量
            Vector2 dir1 = p2 - p1;
            Vector2 dir2 = q2 - q1;

            // 排除零向量情况
            if (dir1 == Vector2.zero || dir2 == Vector2.zero)
            {
                Debug.LogWarning("存在零长度线段");
                return false;
            }
            return Mathf.Abs(Vector2.Dot(dir1.normalized, dir2.normalized)) < epsilon;
        }

        /// <summary>
        /// 使用示例
        /// </summary>
        void ExampleUsage()
        {
            Vector2 line1Start = new Vector2(0, 0);
            Vector2 line1End = new Vector2(1, 0);  // 水平向右

            Vector2 line2Start = new Vector2(0, 0);
            Vector2 line2End = new Vector2(0, 1);  // 垂直向上

            bool isPerpendicular = AreLinesPerpendicular(
                line1Start, line1End,
                line2Start, line2End
            );

            Debug.Log($"线段是否垂直: {isPerpendicular}"); // 输出：true
        }
        #endregion
        /// <summary>
        /// 判断两个2D线段是否垂直
        /// </summary>
        /// <param name="p1">线段1起点</param>
        /// <param name="p2">线段1终点</param>
        /// <param name="q1">线段2起点</param>
        /// <param name="q2">线段2终点</param>
        /// <param name="epsilon">精度阈值（默认1e-6）</param>
        /// <returns>是否垂直</returns>

        List<PositionInt> CombineLists(List<PositionInt> list1, List<PositionInt> list2)
        {
            List<PositionInt> combinedList = new List<PositionInt>();
            List<PositionInt> simiList = new List<PositionInt>();
            foreach (PositionInt item in list1)
            {
                foreach (PositionInt item1 in list2)
                {
                    float d = Vector2.Distance(new Vector2(item1.x, item1.y), new Vector2(item.x, item.y));
                    if (d < 5)
                    {
                        simiList.Add(item);
                        simiList.Add(item1);
                    }
                }

            }

            foreach (PositionInt item in list1)
            {
                if (!simiList.Contains(item))
                {
                    combinedList.Add(item);
                }
            }
            foreach (PositionInt item in list2)
            {
                if (!simiList.Contains(item))
                {
                    combinedList.Add(item);
                }
            }
            return combinedList;
        }

        //遍历字符串中的每个字符，并检查这些字符是否都包含在另一个字符串变量
        public bool AreAllCharactersInString(string source, string target)
        {
            return source.All(c => target.Contains(c));
        }

        // 计算两个二维点之间连线的法线
        public static Vector2 GetNormal(Vector2 pointA, Vector2 pointB)
        {
            // 计算从点A到点B的向量
            Vector2 vectorAB = pointB - pointA;
            // 将向量AB逆时针旋转90度得到法线
            Vector2 normal = new Vector2(-vectorAB.y, vectorAB.x);
            // 归一化法线向量
            return normal.normalized;
        }
        // 判断两条线段是否相交
        public static bool DoLinesIntersect(Vector2 line1Start, Vector2 line1End, Vector2 line2Start, Vector2 line2End)
        {
            float denominator = ((line2End.y - line2Start.y) * (line1End.x - line1Start.x)) - ((line2End.x - line2Start.x) * (line1End.y - line1Start.y));
            if (denominator == 0)
            {
                return false; // 两条线平行
            }

            float ua = (((line2End.x - line2Start.x) * (line1Start.y - line2Start.y)) - ((line2End.y - line2Start.y) * (line1Start.x - line2Start.x))) / denominator;
            float ub = (((line1End.x - line1Start.x) * (line1Start.y - line2Start.y)) - ((line1End.y - line1Start.y) * (line1Start.x - line2Start.x))) / denominator;

            return ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1;
        }

        // 判断法线是否与矩形相交
        public bool IsNormalIntersectingRectangle(Vector2 pointA, Vector2 pointB, Vector2[] rectanglePoints)
        {
            // 计算法线
            Vector2 normal = GetNormal(pointA, pointB);
            // 找到法线所在直线上的一个点，这里选择点A
            Vector2 normalLineStart = GetMidpoint(pointA, pointB);
            Vector2 normalLineEnd = normalLineStart + normal * 100f; // 延长法线用于判断相交

            // 检查法线是否与矩形的四条边相交
            for (int i = 0; i < rectanglePoints.Length; i++)
            {
                Vector2 lineStart = rectanglePoints[i];
                Vector2 lineEnd = rectanglePoints[(i + 1) % rectanglePoints.Length];

                if (DoLinesIntersect(normalLineStart, normalLineEnd, lineStart, lineEnd))
                {
                    return true;
                }
            }

            return false;
        }


        // 把以画板像素为单位的矩形映射到 UI 上的 RectTransform（Game 视图可见）
        private void SetDebugBox(RectTransform box, Rect pixelRect) {
            if (box == null) return;

            // 与判错时使用的坐标变换保持一致（画板像素 -> 画面坐标）
            const float scale = 0.3f;
            float xMin = pixelRect.xMin;
            float xMax = pixelRect.xMax;
            float yMin = pixelRect.yMin;
            float yMax = pixelRect.yMax;

            float width = xMax - xMin;
            float height = yMax - yMin;

            // 以 DebugBoundsRoot 为参考，使用左下角锚点
            box.anchorMin = Vector2.zero;
            box.anchorMax = Vector2.zero;

            // anchoredPosition 是矩形中心
            box.anchoredPosition = new Vector2(xMin + width * 0.5f, yMin + height * 0.5f);
            box.sizeDelta = new Vector2(width, height);

            box.gameObject.SetActive(true);
        }

        // 可选：清理调试框
        public void ClearDebugBoxes() {
            if (standardBoxRect != null)
                standardBoxRect.gameObject.SetActive(false);
            if (studentBoxRect != null)
                studentBoxRect.gameObject.SetActive(false);
        }


        #region 读取 ErrorLimit 配置文件

        public struct ErrorLimit {
            public int index;
            public float errorLimit;
            public float rightLimit;
        }

        public Dictionary<string, List<ErrorLimit>> LoadErrorLimitConfig(string relativePath = "Config/ErrorLimit.txt") {
            var result = new Dictionary<string, List<ErrorLimit>>();
            string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, relativePath);

            if (!System.IO.File.Exists(filePath)) {
                Debug.LogWarning("[AnswerCheck] ErrorLimit 配置文件不存在: " + filePath);
                return result;
            }

            string currentKey = null;
            string[] lines = System.IO.File.ReadAllLines(filePath);

            for (int i = 0; i < lines.Length; i++) {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) {
                    continue;
                }

                // 题号行：#7-1-pb
                if (line.StartsWith("#")) {
                    currentKey = line.Substring(1).Trim();
                    if (string.IsNullOrEmpty(currentKey)) {
                        Debug.LogWarning($"[AnswerCheck] 第{i + 1}行题号为空: {line}");
                        continue;
                    }

                    if (!result.ContainsKey(currentKey)) {
                        result[currentKey] = new List<ErrorLimit>();
                    }
                    continue;
                }

                // 必须先有 #题号
                if (string.IsNullOrEmpty(currentKey)) {
                    Debug.LogWarning($"[AnswerCheck] 第{i + 1}行未找到所属题号，已忽略: {line}");
                    continue;
                }

                // 数据行：index-errorLimit-rightLimit
                string[] parts = line.Split('-');
                if (parts.Length != 3) {
                    Debug.LogWarning($"[AnswerCheck] 第{i + 1}行格式错误(应为 index-errorLimit-rightLimit): {line}");
                    continue;
                }

                if (!int.TryParse(parts[0], out int index)) {
                    Debug.LogWarning($"[AnswerCheck] 第{i + 1}行 index 解析失败: {line}");
                    continue;
                }

                if (!TryParseFloat(parts[1], out float errorLimit)) {
                    Debug.LogWarning($"[AnswerCheck] 第{i + 1}行 errorLimit 解析失败: {line}");
                    continue;
                }

                if (!TryParseFloat(parts[2], out float rightLimit)) {
                    Debug.LogWarning($"[AnswerCheck] 第{i + 1}行 rightLimit 解析失败: {line}");
                    continue;
                }

                result[currentKey].Add(new ErrorLimit {
                    index = index,
                    errorLimit = errorLimit,
                    rightLimit = rightLimit
                });
            }

            return result;
        }

        private bool TryParseFloat(string text, out float value) {
            return float.TryParse(text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out value)
                   || float.TryParse(text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CurrentCulture, out value);
        }
        #endregion
    }
}

