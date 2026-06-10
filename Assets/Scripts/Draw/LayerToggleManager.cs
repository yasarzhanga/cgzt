using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace jxzt
{
    /// <summary>
    /// 图层开关管理器
    /// 管理绘图图层的 Toggle 开关列表，控制图层的显示/隐藏
    /// 每个图层对应一个 Toggle 控件，点击可切换图层可见性
    /// </summary>
    public class LayerToggleManager : MonoBehaviour
    {
        public ToggleGroup ToggleGroup;
        public List<Toggle> toggles;
        public GameObject toggle_pre;
        public Transform PaintBoardTrans;
        public static LayerToggleManager instance;
        private void Start()
        {
            ToggleGroup = GetComponent<ToggleGroup>();
            instance = this;
        }

        public void Init(List<LayerManager> layers)
        {
            for (int i = 0; i < layers.Count; i++)
            {
                createToggle(layers[i]);
            }
        }

        public void createToggle(LayerManager layer)
        {
            GameObject tmp = Instantiate(toggle_pre, ToggleGroup.transform);
            tmp.GetComponent<Toggle>().group = ToggleGroup;
            tmp.GetComponent<Toggle>().onValueChanged.AddListener((e) =>
            {
                if (layer.Active != e)
                {
                    if (e)
                    {
                        layer.SetActive();
                    }
                    else
                    {
                        layer.SetDisactive();


                        layer.frameSelectData = Algorithm.GetOBBPixels(layer.LayerSize, layer.Image_colors).ToArray();


                        switch (layer.lineshape)
                        {
                            case lineshape.圆:
                                break;
                            case lineshape.圆弧:
                                break;
                            case lineshape.椭圆:
                                layer.ellipseData = layer.GetEllipseData(layer.LayerSize, layer.Image_colors);
                                break;
                            case lineshape.标识:
                                layer.markData.Clear();
                                layer.markData.Add(layer.GetMarkData(layer));
                                break;
                            default:
                                layer.lineshape = lineshape.直线;
                                break;
                        }

                        if (layer.lineshape != lineshape.标识)
                        {
                            layer.lineType = layer.GetLineType(layer.LayerSize, layer.Image_colors);

                            foreach (Transform item in GameObject.Find("PaintBoard").transform)
                            {
                                if (item.GetComponent<TeacherLayerManager>())
                                {
                                    if (item.GetComponent<TeacherLayerManager>().layerNum == layer.layerNum)
                                    {
                                        layer.xuhaoPosX = item.GetChild(0).position.x;
                                        layer.xuhaoPosY = item.GetChild(0).position.y;
                                    }
                                }

                            }
                        }
                        else
                        {
                            layer.lineType = linetype.unknown;
                        }
                        if (layer.lineshape == lineshape.直线)
                        {
                            ToggleGroup.transform.GetChild(layer.layerNum - 1).transform.GetChild(0).GetChild(2).transform.GetComponent<TMP_InputField>().text = layer.lineType.ToString();
                        }
                        else
                        {
                            ToggleGroup.transform.GetChild(layer.layerNum - 1).transform.GetChild(0).GetChild(2).transform.GetComponent<TMP_InputField>().text = layer.lineType.ToString() + layer.lineshape.ToString();
                        }

                    }
                }

            });
            tmp.GetComponent<Toggle>().isOn = true;
            tmp.transform.GetChild(0).GetChild(0).transform.GetComponent<TMP_Text>().text = layer.layerNum.ToString();
        }
        public void loadToggle(List<LayerManager> standardlayer_manager)
        {
            using (ScoringPerf.Scope("LayerToggle.Load", $"layers={(standardlayer_manager == null ? 0 : standardlayer_manager.Count)}"))
            {
                if (standardlayer_manager == null)
                {
                    return;
                }

                int n = 0;
                for (int i = 0; i < standardlayer_manager.Count; i++)
                {
                    if (standardlayer_manager[i].lineshape != lineshape.二维码 && standardlayer_manager[i].lineshape != lineshape.判分区域 && standardlayer_manager[i].ocr != 文字识别.姓名.ToString() && standardlayer_manager[i].ocr != 文字识别.学号.ToString())
                    {
                        n++;
                        GameObject tmp = Instantiate(toggle_pre, ToggleGroup.transform);
                        Toggle tg = tmp.GetComponent<Toggle>();
                        tg.group = ToggleGroup;
                        int idx = i; // 捕获索引副本
                        tg.onValueChanged.AddListener((e) =>
                        {
                            if (standardlayer_manager == null || idx < 0 || idx >= standardlayer_manager.Count) return;
                            var layerRef = standardlayer_manager[idx];
                            if (layerRef == null) return;
                            if (layerRef.Active != e)
                            {
                                if (e) layerRef.SetActive(); else layerRef.SetDisactive();
                            }
                        });
                        tmp.transform.GetChild(0).GetChild(0).transform.GetComponent<TMP_Text>().text = n.ToString(); //standardlayer_manager[i].layerNum.ToString();
                        tmp.transform.GetChild(0).GetChild(1).transform.GetComponent<TMP_Text>().text = standardlayer_manager[i].layerError;

                        bool initialOn = standardlayer_manager[i].layerError == "正确";
                        tg.SetIsOnWithoutNotify(initialOn);
                        if (initialOn)
                        {
                            standardlayer_manager[i].active = true;
                            tmp.gameObject.SetActive(false);
                        }
                        else
                        {
                            standardlayer_manager[i].SetDisactiveWithoutNotify();
                        }
                    }
                }
            }
        }
        public void loadToggle_ForCorrect(LayerManager layer, char WrongPoint,int num)
        {
            Debug.Log("获取将要修改的学生答案错误点:" + WrongPoint);
            GameObject tmp = Instantiate(toggle_pre, ToggleGroup.transform);
            tmp.GetComponent<Toggle>().group = ToggleGroup;
            tmp.GetComponent<Toggle>().onValueChanged.AddListener((e) =>
            {
                if (layer.Active != e)
                {
                    if (e)
                    {
                        layer.SetActive();
                    }
                    else
                    {
                        layer.SetDisactive();
                    }
                }

            });
            tmp.GetComponent<Toggle>().isOn = true;
            tmp.transform.GetChild(0).GetChild(0).transform.GetComponent<TMP_Text>().text = num.ToString();
            string str_Error = WrongPoint.ToString();
            switch (str_Error)
            {
                case "a":
                    layer.layerError = "正确";
                    break;
                case "b":
                    layer.layerError = "线型使用错误";
                    break;
                case "c":
                    layer.layerError = "图线不在或偏离正确位置";
                    break;
                case "d":
                    layer.layerError = "图线过长";
                    break;
                case "e":
                    layer.layerError = "图线过短";
                    break;
                case "f":
                    layer.layerError = "剖面线方向绘制错误";
                    break;
                case "g":
                    layer.layerError = "剖面线不是45度线";
                    break;
                case "h":
                    layer.layerError = "剖面线间距大小不一";
                    break;
                case "i":
                    layer.layerError = "在标注位置未发现对应尺寸";
                    break;
                case "j":
                    layer.layerError = "尺寸绘制不标准，格式错误";
                    break;
                case "k":
                    layer.layerError = "尺寸符号错误";
                    break;
                case "l":
                    layer.layerError = "尺寸数值错误";
                    break;
                case "m":
                    layer.layerError = "尺寸标注数值位置不对";
                    break;
                case "n":
                    layer.layerError = "标注了多余尺寸";
                    break;
                case "o":
                    layer.layerError = "公差符号错用";
                    break;
                case "p":
                    layer.layerError = "未标注公差";
                    break;
                case "q":
                    layer.layerError = "公差数字错误";
                    break;
                case "r":
                    layer.layerError = "公差格式错误";
                    break;
                case "s":
                    layer.layerError = "基准要素标识位置错误";
                    break;
                case "t":
                    layer.layerError = "基准要素标识未标识";
                    break;
                case "u":
                    layer.layerError = "基准要素符号错误";
                    break;
                case "v":
                    layer.layerError = "剖面图标识未注写";
                    break;
                case "w":
                    layer.layerError = "未填写技术要求";
                    break;
                case "x":
                    layer.layerError = "回答错误";
                    break;
            }
            tmp.transform.GetChild(0).GetChild(1).transform.GetComponent<TMP_Text>().text = layer.layerError;

        }
        public void ToggleDisplaySort(List<LayerManager> listLayer)
        {
            if (listLayer.Count == ToggleGroup.transform.childCount)
            {
                for (int i = 0; i < ToggleGroup.transform.childCount; i++)
                {
                    if (listLayer[i].lineshape != lineshape.二维码 && listLayer[i].lineshape != lineshape.判分区域 && listLayer[i].ocr != 文字识别.姓名.ToString() && listLayer[i].ocr != 文字识别.学号.ToString() && listLayer[i].layerError != "正确")
                    {
                        ToggleGroup.transform.GetChild(i).GetComponent<Toggle>().isOn = false;
                    }
                    else
                    {
                        ToggleGroup.transform.GetChild(i).gameObject.SetActive(false);
                    }

                }
            }

        }

    }
}

