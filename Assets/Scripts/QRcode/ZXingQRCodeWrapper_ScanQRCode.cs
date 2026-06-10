using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.Common;
using static jxzt.Info;

namespace jxzt
{
    /// <summary>
    /// 二维码扫描识别管理器
    /// 使用摄像头实时扫描或从图片中识别二维码内容
    /// 提取 PPT ID，用于关联题目和答案数据
    /// </summary>
    public class ZXingQRCodeWrapper_ScanQRCode : MonoBehaviour
    {
        [Header("摄像机检测界面")]
        public RawImage cameraTexture;//摄像机映射显示区域
        public RawImage QRTexture;//QR映射显示区域
        public Text text;//用来显示扫描信息
        public Button scanningButton;
        public Texture2D scanPicTexture;
        public Texture2D clipTexture;
        public Texture2D clippedTexture;
        public string pptid;
        public int clipX;
        public int clipY;
        public int clipWidth;
        public int clipHeight;
        public List<string> titleIdNumsNoChoose = new List<string>();
        public List<string> qrCodeNumsNoChoose = new List<string>();
        public Dictionary<string, string> qrCodeNumsNoChooseDic = new Dictionary<string, string>();
        public Dictionary<string, string> titleIdNumsNoChooseDic = new Dictionary<string, string>();
        public List<string> qrCodeNumsFromPic = new List<string>();
        public List<string> qrCodeNums = new List<string>();
        public List<string> titleIdNums = new List<string>();
        public GameObject toggle_Choose;
        // 新增：缓存上一次处理后的题目列表
        private List<string> lastProcessedTitleIdNums = new List<string>();

        // ========== 自动超时全选功能 ==========
        // 协程引用，用于停止协程
        private Coroutine _autoConfirmCoroutine;
        // 超时等待时间（秒）
        private const float AUTO_CONFIRM_DELAY = 5f;
        // 倒计时文本引用（在 Tips_Toggle 子物体中查找）
        private UnityEngine.UI.Text _countdownText;
        /// <summary>
        /// Start 初始化函数
        /// </summary>
        private void Start()
        {
        }

        private void Update()
        {
        }

        public void SyncTeacherQRCodeSelection(List<string> selectedTitles)
        {
            List<string> mergedTitles = MergeOcrDetectedTitleIds(selectedTitles);
            qrCodeNums = new List<string>(mergedTitles);

            TeacherMainManager tmm = TeacherMainManager.instance;
            if (tmm == null)
            {
                GameObject paintBoard = GameObject.Find("PaintBoard");
                if (paintBoard != null)
                {
                    tmm = paintBoard.transform.GetComponent<TeacherMainManager>();
                }
            }

            if (tmm == null)
            {
                return;
            }

            tmm.qrCodeNums = new List<string>(mergedTitles);
            tmm.SyncCurrentQRCodeSnapshot(tmm.qrCodeNums);
        }

        private List<string> MergeOcrDetectedTitleIds(List<string> selectedTitles)
        {
            List<string> result = selectedTitles != null ? new List<string>(selectedTitles) : new List<string>();
            if (TeacherMainManager.instance == null || TeacherMainManager.instance.config == null
                || !string.Equals(TeacherMainManager.instance.config.sign, "ht", StringComparison.OrdinalIgnoreCase))
            {
                return result;
            }

            OCR ocr = null;
            GameObject scriptObj = GameObject.Find("Script");
            if (scriptObj != null)
            {
                ocr = scriptObj.transform.GetComponent<OCR>();
            }

            if (ocr == null)
            {
                return result;
            }

            List<string> ocrTitles = ocr.GetDetectedTitleIdsFromOcrWordsForCurrentSign();
            foreach (string titleId in ocrTitles)
            {
                if (!string.IsNullOrEmpty(titleId) && !result.Contains(titleId))
                {
                    result.Add(titleId);
                    Debug.Log("[QRCodeOcrTitleSupplement] add=" + titleId);
                }
            }

            result.Sort(CompareTitleIdOrder);
            return result;
        }

        private int CompareTitleIdOrder(string left, string right)
        {
            if (TryParseTitleOrder(left, out int leftChapter, out int leftNumber)
                && TryParseTitleOrder(right, out int rightChapter, out int rightNumber))
            {
                int chapterCompare = leftChapter.CompareTo(rightChapter);
                return chapterCompare != 0 ? chapterCompare : leftNumber.CompareTo(rightNumber);
            }

            return string.Compare(left, right, StringComparison.Ordinal);
        }

        private bool TryParseTitleOrder(string titleId, out int chapter, out int number)
        {
            chapter = 0;
            number = 0;
            if (string.IsNullOrEmpty(titleId))
            {
                return false;
            }

            string[] parts = titleId.Split('-');
            return parts.Length >= 2
                && int.TryParse(parts[0], out chapter)
                && int.TryParse(parts[1], out number);
        }

        bool IsScanning = false;
        float interval = 0.5f;//扫描识别时间间隔    
        public void ScanningButtonClick()
        {
            pptid = "";
            Result result = ScanQRCode_Every(ScaleTexture(scanPicTexture, MainManager.instance.width, MainManager.instance.height), clipX, clipY, clipWidth, clipHeight);
            //如果获取到二维码信息了，打印出来
            if (result != null)
            {
                Debug.Log("二维码ID" + result.Text);//二维码识别出来的信息http://keming365.com/user/showPPT?appliId=1253759383439933440
                pptid = GetPPTId_Fun(result.Text);
            }
        }

        public void StudentAnswerPageCodenumButtonClick(bool isSameTitle)
        {
            if (TeacherMainManager.instance.titleIdNums.Count > 0)
            {
                List<string> titleIdNumsFromPic = new List<string>();
                titleIdNumsFromPic = TeacherMainManager.instance.titleIdNums;

                if (isSameTitle && titleIdNumsNoChoose.Count > 0 && AreListsEqual(titleIdNumsFromPic, titleIdNumsNoChoose))
                {
                    Debug.Log("The lists contain the same elements in different order.");

                    // 命中相同题目，直接复用上次的处理结果
                    if (lastProcessedTitleIdNums != null && lastProcessedTitleIdNums.Count > 0)
                    {
                        // 使用副本，避免与缓存互相影响
                        titleIdNums = new List<string>(lastProcessedTitleIdNums);
                        // 同步到 TeacherMainManager
                        var tmm = GameObject.Find("PaintBoard").transform.GetComponent<TeacherMainManager>();
                        tmm.studentAnswerPageCodenum = titleIdNums.Count;
                        tmm.titleIdNums = titleIdNums;
                        // 同步当前图片的题号快照，避免后续流程判断错误
                        tmm.SyncCurrentTitleIdSnapshot(titleIdNums);
                    }

                    Alert.Instance.Tips_Toggle.SetActive(false);
                    Alert.Instance.text_tipsToggle.text = "";

                    Debug.Log("批量选择相同题目赋值" + titleIdNums.Count);
                    //开启协程
                    StartCoroutine(TeacherMainManager.instance.AegisAnimation(3));
                }
                else
                {
                    Debug.Log("The lists do not contain the same elements.");
                    Debug.Log(titleIdNumsFromPic[0]);
                    // make a defensive copy instead of assigning the reference
                    titleIdNumsNoChoose = new List<string>(titleIdNumsFromPic ?? new List<string>());

                    if (Alert.Instance)
                    {
                        Alert.Instance.ShowToggle("选择需要判分的题号");
                    }
                    else
                    {
                        return;
                    }

                    // 清理旧的条目与映射，避免使用上次残留数据
                    titleIdNumsNoChooseDic.Clear();
                    Transform contentParent = Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(1);
                    if (contentParent.childCount > 0)
                    {
                        for (int ci = contentParent.childCount - 1; ci >= 0; ci--)
                        {
                            Destroy(contentParent.GetChild(ci).gameObject);
                        }
                    }

                    List<int> TiHaos = new List<int>();
                    string part0 = "";
                    string part1 = "";
                    int isSameTitleStyle = 0;//0是初始，1是-，2是()
                    for (int i = 0; i < titleIdNumsNoChoose.Count; i++)
                    {
                        GameObject togglechoose = Instantiate(toggle_Choose, contentParent);
                        Debug.Log("Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(1).transform.childCount" + contentParent.childCount);

                        string fullId = titleIdNumsNoChoose[i];
                        string[] parts = fullId.Split('-');
                        string[] partsht = fullId.Split('(');
                        // reset per-iteration state
                        isSameTitleStyle = 0;
                        part0 = "";
                        part1 = "";

                        // 是否支持自动批改标志（默认支持）
                        bool canAuto = true;
                        int flag = 0;
                        if (parts.Length > 2 && int.TryParse(parts[2], out flag))
                        {
                            canAuto = (flag == 0);
                        }

                        if (parts.Length > 1)
                        {
                            if (partsht.Length > 1)
                            {
                                string[] partsht1 = partsht[1].Split(')');
                                if (partsht1.Length > 0 && int.TryParse(partsht1[0], out int num))
                                {
                                    TiHaos.Add(num);
                                    isSameTitleStyle = 2;
                                    Debug.Log("题号截取成功" + partsht1[0]);
                                    part0 = parts[0] + "-" + parts[1];
                                    part1 = partsht1[0];
                                }
                                else
                                {
                                    // 回退使用原始 id
                                    isSameTitleStyle = 0;
                                }
                            }
                            else
                            {
                                isSameTitleStyle = 1;
                                if (int.TryParse(parts[1], out int num2))
                                {
                                    TiHaos.Add(num2);
                                    Debug.Log("题号截取成功" + parts[1]);
                                }
                                part0 = parts[0];
                                part1 = parts[1];
                            }
                        }

                        Debug.Log("题号重新排序");
                        string displayKey = fullId; // 默认显示原始 id
                        string valueToStore = fullId; // 默认存储原始 id

                        if (isSameTitleStyle == 2)
                        {
                            displayKey = part0 + "(" + part1 + ")";
                            if (!titleIdNumsNoChooseDic.ContainsKey(displayKey))
                            {
                                if (parts.Length >= 4)
                                {
                                    string part0temp = parts[0] + "-" + parts[1] + "-" + parts[3];
                                    valueToStore = part0temp;
                                }
                                titleIdNumsNoChooseDic[displayKey] = valueToStore;
                            }
                            togglechoose.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = displayKey;
                        }
                        else if (isSameTitleStyle == 1)
                        {
                            displayKey = part0 + "-" + part1;
                            if (!titleIdNumsNoChooseDic.ContainsKey(displayKey))
                            {
                                if (parts.Length >= 4)
                                {
                                    string part0temp = parts[0] + "-" + parts[1] + "-" + parts[3];
                                    valueToStore = part0temp;
                                }
                                titleIdNumsNoChooseDic[displayKey] = valueToStore;
                            }
                            togglechoose.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = displayKey;
                        }
                        else
                        {
                            // 无法解析格式，直接展示原始 id，并建立同名映射以避免 KeyNotFound
                            displayKey = fullId;
                            if (!titleIdNumsNoChooseDic.ContainsKey(displayKey))
                            {
                                titleIdNumsNoChooseDic[displayKey] = valueToStore;
                            }
                            togglechoose.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = displayKey;
                        }

                        // 设置不可批改的 UI 状态
                        if (!canAuto)
                        {
                            togglechoose.transform.GetComponent<Toggle>().interactable = false;
                            togglechoose.transform.GetComponent<Toggle>().isOn = false;
                            togglechoose.transform.GetChild(1).GetChild(0).GetComponent<Text>().color = new Color32(135, 135, 135, 255);
                            togglechoose.transform.GetChild(1).GetComponent<Button>().interactable = true;
                            togglechoose.transform.GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
                            togglechoose.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate ()
                            {
                                TeacherMainManager.instance.NoRemeberTitleDisplayMessage("本题暂不提供自动批改服务！");
                            });
                        }

                        Transform parentTran = contentParent;
                        TiHaos.Sort();
                        Debug.Log("题号排序2TiHaos.Count" + TiHaos.Count);
                    }

                    Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(4).transform.gameObject.SetActive(true);
                    var hidenToggle = Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(4).transform.GetComponent<Toggle>();
                    Debug.Log("hidenToggle.isOn" + hidenToggle.isOn);

                    if (hidenToggle.isOn)
                    {
                        for (int k = 0; k < contentParent.childCount; k++)
                        {
                            var childT = contentParent.GetChild(k).GetComponent<Toggle>();
                            if (childT != null && childT.interactable)
                                childT.isOn = true;
                        }

                        if (titleIdNums.Count > 0)
                        {
                            titleIdNums.Clear();
                        }
                        for (int i = 0; i < contentParent.childCount; i++)
                        {
                            var txt = contentParent.GetChild(i).GetChild(1).GetChild(0).GetComponent<Text>().text;
                            if (contentParent.GetChild(i).GetComponent<Toggle>().isOn)
                            {
                                if (titleIdNumsNoChooseDic.TryGetValue(txt, out var val))
                                {
                                    titleIdNums.Add(val);
                                    Debug.Log("titleIdNums内" + val);
                                }
                                else
                                {
                                    // 回退：直接使用显示文本
                                    titleIdNums.Add(txt);
                                }
                            }
                        }
                        // 缓存此次处理结果（使用副本）
                        lastProcessedTitleIdNums = new List<string>(titleIdNums);

                        Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(4).transform.gameObject.SetActive(false);
                        Debug.Log("Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(1)Nums" + contentParent.childCount);
                        Debug.Log("titleIdNums" + titleIdNums.Count);
                        Alert.Instance.Tips_Toggle.SetActive(false);
                        Alert.Instance.text_tipsToggle.text = "";
                        var tmm = GameObject.Find("PaintBoard").transform.GetComponent<TeacherMainManager>();
                        tmm.studentAnswerPageCodenum = titleIdNums.Count;
                        tmm.titleIdNums = titleIdNums;
                        // 同步当前图片的题号快照
                        tmm.SyncCurrentTitleIdSnapshot(titleIdNums);

                        if (titleIdNums.Count > 0)
                        {
                            //开启协程
                            StartCoroutine(TeacherMainManager.instance.AegisAnimation(3));
                        }
                        else
                        {
                            titleIdNumsNoChoose.Clear();
                            lastProcessedTitleIdNums.Clear();
                            TeacherMainManager.instance.studentAnswerQRnum = 0;
                            TeacherMainManager.instance.studentAnswerPageCodenum = 0;
                            TeacherMainManager.instance._remainingQRCodeCount = 0;
                            TeacherMainManager.instance._remainingPageCodeCount = 0;
                            StartCoroutine(TeacherMainManager.instance.AegisAnimation(8));
                        }
                    }
                    else
                    {
                        if (titleIdNums.Count > 0)
                        {
                            titleIdNums.Clear();
                        }
                        Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(3).transform.gameObject.SetActive(true);
                        Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(3).transform.GetComponent<Toggle>().isOn = false;
                        //全选按钮监听
                        Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(3).transform.GetComponent<Toggle>().onValueChanged.AddListener((bool ison) =>
                        {
                            for (int i = 0; i < contentParent.childCount; i++)
                            {
                                var tgl = contentParent.GetChild(i).GetComponent<Toggle>();
                                if (tgl != null && tgl.interactable)
                                {
                                    tgl.isOn = ison;
                                }
                            }
                        });

                        // 启动自动超时全选
                        StartAutoConfirm();

                        Alert.Instance.button_yes_Toggle.onClick.RemoveAllListeners();
                        Alert.Instance.button_yes_Toggle.onClick.AddListener(delegate ()
                        {
                            // 用户手动点确定，停止超时协程
                            StopAutoConfirm();

                            for (int i = 0; i < contentParent.childCount; i++)
                            {
                                var tgl = contentParent.GetChild(i).GetComponent<Toggle>();
                                if (tgl != null && tgl.isOn)
                                {
                                    var keyTxt = contentParent.GetChild(i).GetChild(1).GetChild(0).GetComponent<Text>().text;
                                    if (titleIdNumsNoChooseDic.TryGetValue(keyTxt, out var realId))
                                    {
                                        titleIdNums.Add(realId);
                                        Debug.Log("titleIdNums内" + realId);
                                    }
                                    else
                                    {
                                        // 回退：直接使用显示文本
                                        titleIdNums.Add(keyTxt);
                                    }
                                }
                            }
                            // 缓存此次处理结果（使用副本）
                            lastProcessedTitleIdNums = new List<string>(titleIdNums);

                            Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(3).transform.gameObject.SetActive(false);
                            Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(4).transform.gameObject.SetActive(true);
                            Debug.Log("Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(1)Nums" + contentParent.childCount);
                            Debug.Log("titleIdNums" + titleIdNums.Count);
                            Alert.Instance.Tips_Toggle.SetActive(false);
                            Alert.Instance.text_tipsToggle.text = "";
                            var tmm = GameObject.Find("PaintBoard").transform.GetComponent<TeacherMainManager>();
                            tmm.studentAnswerPageCodenum = titleIdNums.Count;
                            tmm.titleIdNums = titleIdNums;
                            // 同步当前图片的题号快照
                            tmm.SyncCurrentTitleIdSnapshot(titleIdNums);

                            if (titleIdNums.Count > 0)
                            {
                                //开启协程
                                StartCoroutine(TeacherMainManager.instance.AegisAnimation(3));
                            }
                            else
                            {
                                titleIdNumsNoChoose.Clear();
                                lastProcessedTitleIdNums.Clear();
                                TeacherMainManager.instance.studentAnswerQRnum = 0;
                                TeacherMainManager.instance.studentAnswerPageCodenum = 0;
                                TeacherMainManager.instance._remainingQRCodeCount = 0;
                                TeacherMainManager.instance._remainingPageCodeCount = 0;
                                StartCoroutine(TeacherMainManager.instance.AegisAnimation(8));
                            }
                        });
                    }
                }
            }
            else
            {
                Debug.Log("错误提示" + "未识别到页码,请手动选择题号");
            }
        }
        public void StudentAnswerScanningButtonClick()
        {
            pptid = "";
            Result[] results = ScanQRCodes(scanPicTexture, scanPicTexture.width, scanPicTexture.height);//
            if (results != null)
            {

                List<string> qrCodeNumsFromPic = new List<string>();
                foreach (var item in results)
                {
                    Debug.Log("二维码ID" + item.Text);//二维码识别出来的信息
                    pptid = GetPPTId_Fun(item.Text);
                    qrCodeNumsFromPic.Add(pptid);//把从图片扫出来的二维码添加进来

                }
                if (AreListsEqual(qrCodeNumsFromPic, qrCodeNumsNoChoose))
                {
                    Debug.Log("The lists contain the same elements in different order.");

                    Alert.Instance.Tips_Toggle.SetActive(false);
                    Alert.Instance.text_tipsToggle.text = "";
                    SyncTeacherQRCodeSelection(qrCodeNums);
                    Debug.Log("批量选择相同题目赋值" + qrCodeNums.Count);
                    //开启协程
                    StartCoroutine(TeacherMainManager.instance.AegisAnimation(3));
                }
                else
                {
                    Debug.Log("The lists do not contain the same elements.");
                    qrCodeNums.Clear();
                    qrCodeNumsNoChoose.Clear();
                    qrCodeNumsNoChoose = qrCodeNumsFromPic;


                    int childCount = Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(1).transform.childCount;
                    if (childCount > 0)
                    {
                        for (int i = 0; i < childCount; i++)
                        {
                            Destroy(Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(1).transform.GetChild(i).gameObject);
                        }
                    }
                    StartCoroutine(ProcessAllQRCodes());
                }
            }
            else
            {
                Debug.Log("ZXing 未识别到二维码，使用百度OCR二维码接口兜底识别");
                // ZXing 未识别到二维码，使用百度OCR二维码接口兜底识别
                var tex = scanPicTexture;
                if (tex != null)
                {
                    Debug.Log("ZXing 未识别到二维码，使用百度OCR二维码接口兜底识别1");
                    byte[] pngBytes = tex.EncodeToPNG();
                    var ocr = GameObject.Find("Script").transform.GetComponent<OCR>();
                    ocr.RecognizeQRCodeSync(pngBytes, false, results =>
                    {
                    });
                }
                else
                {
                }

            }


        }

        public IEnumerator ProcessAllQRCodes()
        {
            List<int> TiHaos = new List<int>();
            string part0 = "";
            // pending 应为要请求的二维码数量，而不是 TiHaos.Count（TiHaos 为空）
            int pending = qrCodeNumsNoChoose != null ? qrCodeNumsNoChoose.Count : 0;
            if (pending == 0) yield break;

            qrCodeNumsNoChooseDic.Clear();

            for (int i = 0; i < qrCodeNumsNoChoose.Count; i++)
            {
                string code = qrCodeNumsNoChoose[i];
                WWWForm wwwform = new WWWForm();
                wwwform.AddField("AreaResource", code);
                Debug.Log("AreaResource：" + code);
                WebManager.Instance.GetStringFunc(Config.GetSingleProblemInfo, wwwform, delegate (string s)
                {
                    Debug.Log("返回题号：" + s);
                    if (s.Length == 0)
                    {
                        Debug.Log("返回题号失败,未识别到题目");
                        pending--; // 一定要减
                        return;
                    }
                    ReturnStateTeacher pt = JsonConvert.DeserializeObject<ReturnStateTeacher>(s);
                    switch (pt.code)
                    {
                        case 200:
                            Debug.Log("返回题号成功：" + pt.message);

                            GameObject togglechoose = Instantiate(toggle_Choose, Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(1));
                            togglechoose.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = pt.data[0].title_id;

                            string[] parts = pt.data[0].title_id.Split('-');
                            string[] partsht = pt.data[0].title_id.Split('(');
                            if (parts.Length > 1)
                            {

                                if (partsht.Length > 1)
                                {
                                    string[] partsht1 = partsht[1].Split(')');
                                    TiHaos.Add(int.Parse(partsht1[0]));
                                    Debug.Log("题号截取成功" + partsht1[0]);
                                    part0 = partsht[0];
                                }
                                else
                                {
                                    TiHaos.Add(int.Parse(parts[1]));
                                    Debug.Log("题号截取成功" + parts[1]);
                                    part0 = parts[0];
                                }

                            }
                            Transform parentTran = Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(1).transform;
                            TiHaos.Sort();
                            if (partsht.Length > 1)
                            {
                                string dictKey = part0 + "(" + partsht[1];
                                if (!qrCodeNumsNoChooseDic.ContainsKey(dictKey))
                                    qrCodeNumsNoChooseDic[dictKey] = pt.data[0].title_id;
                                else
                                    Debug.Log("Duplicate qrCode key skipped: " + dictKey);
                            }
                            else
                            {
                                string dictKey = part0 + "-" + parts[1];
                                if (!qrCodeNumsNoChooseDic.ContainsKey(dictKey))
                                    qrCodeNumsNoChooseDic[dictKey] = pt.data[0].title_id;
                                else
                                    Debug.Log("Duplicate qrCode key skipped: " + dictKey);
                            }

                            // 更新父节点中的显示文本（尽管顺序可能要更精细处理）
                            for (int j = 0; j < parentTran.childCount && j < TiHaos.Count; j++)
                            {
                                if (partsht.Length > 1)
                                {
                                    parentTran.GetChild(j).GetChild(1).GetChild(0).GetComponent<Text>().text = part0 + "(" + TiHaos[j].ToString() + ")";
                                }
                                else
                                {
                                    parentTran.GetChild(j).GetChild(1).GetChild(0).GetComponent<Text>().text = part0 + "-" + TiHaos[j].ToString();
                                }
                            }
                            pending--; // 成功也要减
                            break;
                        case 400:
                            Debug.Log("返回题目信息失败：" + pt.message);
                            pending--;
                            break;
                        case 404:
                            Debug.Log("登录失效：" + pt.message);
                            if (Alert.Instance)
                            {
                                Alert.Instance.HideToggle();
                                TeacherMainManager.instance.ErrorTipsClear("登录失效,请返回登陆界面重新登录", true, true);
                            }
                            pending--;
                            break;
                        default:
                            pending--;
                            break;
                    }
                });
            }

            // 等待所有回调完成（注意：如果某些回调永远不调用，会一直等，必要时可加超时）
            yield return new WaitUntil(() => pending == 0);
            Debug.Log("yield return new WaitUntil(() => pending == 0)：");
            Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(4).transform.gameObject.SetActive(true);
            var hidenToggle = Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(4).transform.GetComponent<Toggle>();
            if (hidenToggle.isOn)
            {


                for (int k = 0; k < Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(1).childCount; k++)
                {
                    var childT = Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(1).GetChild(k).GetComponent<Toggle>();
                    if (childT != null && childT.interactable)
                        childT.isOn = true;
                }

                qrCodeNums.Clear();
                for (int k = 0; k < Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(1).childCount; k++)
                {
                    var child = Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(1).GetChild(k);
                    var tgl = child.GetComponent<Toggle>();
                    if (tgl != null && tgl.isOn)
                    {
                        var keyText = child.GetChild(1).GetChild(0).GetComponent<Text>().text;
                        if (qrCodeNumsNoChooseDic.ContainsKey(keyText))
                        {
                            qrCodeNums.Add(qrCodeNumsNoChooseDic[keyText]);
                        }
                    }
                }

                Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(4).transform.gameObject.SetActive(false);
                Alert.Instance.Tips_Toggle.SetActive(false);
                Alert.Instance.text_tipsToggle.text = "";
                SyncTeacherQRCodeSelection(qrCodeNums);

                if (qrCodeNums.Count > 0)
                    StartCoroutine(TeacherMainManager.instance.AegisAnimation(3));
                else
                {
                    TeacherMainManager.instance.studentAnswerQRnum = 0;
                    TeacherMainManager.instance.studentAnswerPageCodenum = 0;
                    TeacherMainManager.instance._remainingQRCodeCount = 0;
                    TeacherMainManager.instance._remainingPageCodeCount = 0;
                    StartCoroutine(TeacherMainManager.instance.AegisAnimation(8));
                }


            }
            else
            {
                if (Alert.Instance)
                {
                    Alert.Instance.ShowToggle("选择需要判分的题号");
                }

                // 所有请求返回后再统一设置 UI 和监听器（把之前在 StudentAnswerScanningButtonClick 中的相关代码放到这里）
                Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(3).transform.gameObject.SetActive(true);
                var allToggle = Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(3).transform.GetComponent<Toggle>();
                allToggle.isOn = false;
                allToggle.onValueChanged.RemoveAllListeners();
                allToggle.onValueChanged.AddListener((bool ison) =>
                {
                    for (int k = 0; k < Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(1).childCount; k++)
                    {
                        var childT = Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(1).GetChild(k).GetComponent<Toggle>();
                        if (childT != null && childT.interactable)
                            childT.isOn = ison;
                    }
                });

                // 启动自动超时全选
                StartAutoConfirm();

                Alert.Instance.button_yes_Toggle.onClick.RemoveAllListeners();
                Alert.Instance.button_yes_Toggle.onClick.AddListener(delegate ()
                {
                    // 用户手动点确定，停止超时协程
                    StopAutoConfirm();

                    qrCodeNums.Clear();
                    for (int k = 0; k < Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(1).childCount; k++)
                    {
                        var child = Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(1).GetChild(k);
                        var tgl = child.GetComponent<Toggle>();
                        if (tgl != null && tgl.isOn)
                        {
                            var keyText = child.GetChild(1).GetChild(0).GetComponent<Text>().text;
                            if (qrCodeNumsNoChooseDic.ContainsKey(keyText))
                            {
                                qrCodeNums.Add(qrCodeNumsNoChooseDic[keyText]);
                            }
                        }
                    }

                    Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(3).transform.gameObject.SetActive(false);
                    Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(4).transform.GetChild(0).GetComponent<Toggle>();
                    Alert.Instance.Tips_Toggle.SetActive(false);
                    Alert.Instance.text_tipsToggle.text = "";
                    SyncTeacherQRCodeSelection(qrCodeNums);

                    if (qrCodeNums.Count > 0)
                        StartCoroutine(TeacherMainManager.instance.AegisAnimation(3));
                });
            }


            yield break;
        }
        private IEnumerator ProcessAllPageCodes()
        {
            List<int> TiHaos = new List<int>();
            string part0 = "";
            // pending 应为要请求的二维码数量，而不是 TiHaos.Count（TiHaos 为空）
            int pending = qrCodeNumsNoChoose != null ? qrCodeNumsNoChoose.Count : 0;
            if (pending == 0) yield break;

            qrCodeNumsNoChooseDic.Clear();

            for (int i = 0; i < qrCodeNumsNoChoose.Count; i++)
            {
                string code = qrCodeNumsNoChoose[i];
                WWWForm wwwform = new WWWForm();
                wwwform.AddField("AreaResource", code);
                WebManager.Instance.GetStringFunc(Config.GetSingleProblemInfo, wwwform, delegate (string s)
                {
                    Debug.Log("返回题号：" + s);
                    if (s.Length == 0)
                    {
                        Debug.Log("返回题号失败,未识别到题目");
                        pending--; // 一定要减
                        return;
                    }
                    ReturnStateTeacher pt = JsonConvert.DeserializeObject<ReturnStateTeacher>(s);
                    switch (pt.code)
                    {
                        case 200:
                            Debug.Log("返回题号成功：" + pt.message);

                            GameObject togglechoose = Instantiate(toggle_Choose, Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(1));
                            togglechoose.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = pt.data[0].title_id;

                            string[] parts = pt.data[0].title_id.Split('-');
                            string[] partsht = pt.data[0].title_id.Split('(');
                            if (parts.Length > 1)
                            {

                                if (partsht.Length > 1)
                                {
                                    string[] partsht1 = partsht[1].Split(')');
                                    TiHaos.Add(int.Parse(partsht1[0]));
                                    Debug.Log("题号截取成功" + partsht1[0]);
                                    part0 = partsht[0];
                                }
                                else
                                {
                                    TiHaos.Add(int.Parse(parts[1]));
                                    Debug.Log("题号截取成功" + parts[1]);
                                    part0 = parts[0];
                                }

                            }
                            Transform parentTran = Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(1).transform;
                            TiHaos.Sort();
                            if (partsht.Length > 1)
                            {
                                string dictKey = part0 + "(" + partsht[1];
                                if (!qrCodeNumsNoChooseDic.ContainsKey(dictKey))
                                    qrCodeNumsNoChooseDic[dictKey] = pt.data[0].title_id;
                                else
                                    Debug.Log("Duplicate qrCode key skipped: " + dictKey);
                            }
                            else
                            {
                                string dictKey = part0 + "-" + parts[1];
                                if (!qrCodeNumsNoChooseDic.ContainsKey(dictKey))
                                    qrCodeNumsNoChooseDic[dictKey] = pt.data[0].title_id;
                                else
                                    Debug.Log("Duplicate qrCode key skipped: " + dictKey);
                            }

                            // 更新父节点中的显示文本（尽管顺序可能要更精细处理）
                            for (int j = 0; j < parentTran.childCount && j < TiHaos.Count; j++)
                            {
                                if (partsht.Length > 1)
                                {
                                    parentTran.GetChild(j).GetChild(1).GetChild(0).GetComponent<Text>().text = part0 + "(" + TiHaos[j].ToString() + ")";
                                }
                                else
                                {
                                    parentTran.GetChild(j).GetChild(1).GetChild(0).GetComponent<Text>().text = part0 + "-" + TiHaos[j].ToString();
                                }
                            }
                            pending--; // 成功也要减
                            break;
                        case 400:
                            Debug.Log("返回题目信息失败：" + pt.message);
                            pending--;
                            break;
                        case 404:
                            Debug.Log("登录失效：" + pt.message);
                            if (Alert.Instance)
                            {
                                Alert.Instance.HideToggle();
                                TeacherMainManager.instance.ErrorTipsClear("登录失效,请返回登陆界面重新登录", true, true);
                            }
                            pending--;
                            break;
                        default:
                            pending--;
                            break;
                    }
                });
            }

            // 等待所有回调完成（注意：如果某些回调永远不调用，会一直等，必要时可加超时）
            yield return new WaitUntil(() => pending == 0);
            Debug.Log("yield return new WaitUntil(() => pending == 0)：");
            Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(4).transform.gameObject.SetActive(true);
            var hidenToggle = Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(4).transform.GetComponent<Toggle>();
            if (hidenToggle.isOn)
            {


                for (int k = 0; k < Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(1).childCount; k++)
                {
                    var childT = Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(1).GetChild(k).GetComponent<Toggle>();
                    if (childT != null && childT.interactable)
                        childT.isOn = true;
                }

                qrCodeNums.Clear();
                for (int k = 0; k < Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(1).childCount; k++)
                {
                    var child = Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(1).GetChild(k);
                    var tgl = child.GetComponent<Toggle>();
                    if (tgl != null && tgl.isOn)
                    {
                        var keyText = child.GetChild(1).GetChild(0).GetComponent<Text>().text;
                        if (qrCodeNumsNoChooseDic.ContainsKey(keyText))
                        {
                            qrCodeNums.Add(qrCodeNumsNoChooseDic[keyText]);
                        }
                    }
                }

                Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(4).transform.gameObject.SetActive(false);
                Alert.Instance.Tips_Toggle.SetActive(false);
                Alert.Instance.text_tipsToggle.text = "";
                SyncTeacherQRCodeSelection(qrCodeNums);

                if (qrCodeNums.Count > 0)
                    StartCoroutine(TeacherMainManager.instance.AegisAnimation(3));
                else
                    TeacherMainManager.instance.InitPanfen();


            }
            else
            {
                if (Alert.Instance)
                {
                    Alert.Instance.ShowToggle("选择需要判分的题号");
                }

                // 所有请求返回后再统一设置 UI 和监听器（把之前在 StudentAnswerScanningButtonClick 中的相关代码放到这里）
                Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(3).transform.gameObject.SetActive(true);
                var allToggle = Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(3).transform.GetComponent<Toggle>();
                allToggle.isOn = false;
                allToggle.onValueChanged.RemoveAllListeners();
                allToggle.onValueChanged.AddListener((bool ison) =>
                {
                    for (int k = 0; k < Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(1).childCount; k++)
                    {
                        var childT = Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(1).GetChild(k).GetComponent<Toggle>();
                        if (childT != null && childT.interactable)
                            childT.isOn = ison;
                    }
                });

                // 启动自动超时全选
                StartAutoConfirm();

                Alert.Instance.button_yes_Toggle.onClick.RemoveAllListeners();
                Alert.Instance.button_yes_Toggle.onClick.AddListener(delegate ()
                {
                    // 用户手动点确定，停止超时协程
                    StopAutoConfirm();

                    qrCodeNums.Clear();
                    for (int k = 0; k < Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(1).childCount; k++)
                    {
                        var child = Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(1).GetChild(k);
                        var tgl = child.GetComponent<Toggle>();
                        if (tgl != null && tgl.isOn)
                        {
                            var keyText = child.GetChild(1).GetChild(0).GetComponent<Text>().text;
                            if (qrCodeNumsNoChooseDic.ContainsKey(keyText))
                            {
                                qrCodeNums.Add(qrCodeNumsNoChooseDic[keyText]);
                            }
                        }
                    }

                    Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(3).transform.gameObject.SetActive(false);
                    Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(4).transform.GetChild(0).GetComponent<Toggle>();
                    Alert.Instance.Tips_Toggle.SetActive(false);
                    Alert.Instance.text_tipsToggle.text = "";
                    SyncTeacherQRCodeSelection(qrCodeNums);

                    if (qrCodeNums.Count > 0)
                        StartCoroutine(TeacherMainManager.instance.AegisAnimation(3));
                    else
                        TeacherMainManager.instance.InitPanfen();
                });
            }


            yield break;
        }
        #region ScanQRCode

        /*
         使用方法，类似如下：
         Result result = ZXingQRCodeWrapper.ScanQRCode(data, webCamTexture.width, webCamTexture.height);

             */

        //二维码识别类
        static BarcodeReader barcodeReader1;//库文件的对象（二维码信息保存的地方）

        /// <summary>
        /// 传入图片识别
        /// </summary>
        /// <param name="textureData"></param>
        /// <param name="textureDataWidth"></param>
        /// <param name="textureDataHeight"></param>
        /// <returns></returns>
        public Result ScanQRCode(Texture2D textureData, int textureDataWidth, int textureDataHeight)
        {
            textureData = ClipTexture(textureData, MainManager.instance.width, MainManager.instance.height, textureDataWidth, textureDataHeight);
            return ScanQRCode(textureData.GetPixels32(), textureDataWidth, textureDataHeight);
        }
        public Result[] ScanQRCodes(Texture2D textureData, int textureDataWidth, int textureDataHeight)
        {
            QRTexture.texture = textureData;

            return ScanQRCodeMult(textureData.GetPixels32(), textureDataWidth, textureDataHeight);
        }
        public Result ScanQRCode_Every(Texture2D textureData, int clipX, int clipY, int textureDataWidth, int textureDataHeight)
        {            
            clippedTexture = ClipTexture(clipTexture, clipX, clipY, textureDataWidth, textureDataHeight);
            Debug.Log("二维码获取图片数据1" + clippedTexture);           
            return ScanQRCode1(clippedTexture, textureDataWidth, textureDataHeight);
        }
        /// <summary>
        /// 传入图片像素识别
        /// </summary>
        /// <param name="textureData"></param>
        /// <param name="textureDataWidth"></param>
        /// <param name="textureDataHeight"></param>
        /// <returns></returns>
        public Result ScanQRCode(Color32[] textureData, int textureDataWidth, int textureDataHeight)
        {
            if (barcodeReader1 == null)
            {
                barcodeReader1 = new BarcodeReader();
            }
            Result result = barcodeReader1.Decode(textureData, textureDataWidth, textureDataHeight);

            return result;
        }
        public Result ScanQRCode1(Texture2D textureData, int textureDataWidth, int textureDataHeight)
        {
            if (barcodeReader1 == null)
            {
                barcodeReader1 = new BarcodeReader();
            }
            Result result = barcodeReader1.Decode(clippedTexture.GetPixels32(), textureDataWidth, textureDataHeight);
            StartCoroutine(DestroyTexture(scanPicTexture));
            StartCoroutine(DestroyTexture(clipTexture));
            StartCoroutine(DestroyTexture(clippedTexture));
            return result;
        }
        public Result[] ScanQRCodeMult(Color32[] textureData, int textureDataWidth, int textureDataHeight)
        {
            var hints = new Dictionary<DecodeHintType, object>();
            hints[DecodeHintType.TRY_HARDER] = true; // 提高解码灵敏度
            hints[DecodeHintType.POSSIBLE_FORMATS] = BarcodeFormat.QR_CODE; // 指定解码格式
            hints[DecodeHintType.CHARACTER_SET] = "UTF-8"; // 指定字符集

            // 创建解码器
            var reader = new BarcodeReader();
            reader.Options = new DecodingOptions
            {
                TryHarder = true,
                PossibleFormats = new[] { BarcodeFormat.QR_CODE }
            };

            // 解码图像
            var results = reader.DecodeMultiple(textureData, textureDataWidth, textureDataHeight);

            if (results != null)
            {
                foreach (var result in results)
                {
                    Debug.Log(result.Text);
                }
            }
            StartCoroutine(DestroyTexture(scanPicTexture));
            StartCoroutine(DestroyTexture(clipTexture));
            StartCoroutine(DestroyTexture(clippedTexture));
            return results;
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
        public Texture2D ClipTexture(Texture2D originalTexture, int clipX, int clipY, int clipWidth, int clipHeight)
        {
            clippedTexture = new Texture2D(clipWidth, clipHeight, originalTexture.format, true);
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
        /// <summary>
        ///修改texture分辨率
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targetWidth"></param>
        /// <param name="targetHeight"></param>
        /// <returns></returns>
        private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
        {
            clipTexture = new Texture2D(targetWidth, targetHeight, source.format, true);
            UnityEngine.Color[] rpixels = clipTexture.GetPixels(0);
            float incX = ((float)1 / source.width) * ((float)source.width / targetWidth);
            float incY = ((float)1 / source.height) * ((float)source.height / targetHeight);
            for (int px = 0; px < rpixels.Length; px++)
            {
                rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
            }

            clipTexture.SetPixels(rpixels, 0);
            clipTexture.Apply();
            return clipTexture;
        }

        public string GetPPTId_Fun(string PPTId)
        {
            string url = PPTId;
            string result = "";
            Debug.Log(url);
            if (url.Contains("="))
            {
                Debug.Log("=");
                string[] parts = url.Split('=');
                result = parts[parts.Length - 1];
            }
            else if (url.Contains("QRCode/"))
            {
                Debug.Log("QRCode/");
                string[] parts = url.Split('/');
                result = parts[parts.Length - 1];
            }
            else if (url.Contains("previewIndex/"))
            {
                Debug.Log("previewIndex/");
                string[] parts = url.Split('/');
                result = parts[parts.Length - 1];
            }

            Debug.Log(result);
            return result;

        }
        #endregion
        public IEnumerator DestroyTexture(Texture2D texture)
        {

            yield return new WaitForSeconds(1);
            Destroy(texture);
        }

        // ========== 自动超时全选功能 ==========
        /// <summary>
        /// 查找或创建倒计时文本组件
        /// </summary>
        private UnityEngine.UI.Text GetOrCreateCountdownText()
        {
            if (_countdownText != null) return _countdownText;
            // 尝试从 Tips_Toggle 的子物体中查找提示文本（通常是标题文本所在位置）
            Transform titleArea = Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(0);
            if (titleArea != null)
            {
                _countdownText = titleArea.GetComponent<UnityEngine.UI.Text>();
            }
            return _countdownText;
        }

        /// <summary>
        /// 自动超时确认协程
        /// 等待 delay 秒后，若面板仍在显示，则自动全选并执行确定
        /// </summary>
        private IEnumerator AutoConfirmCoroutine(float delay)
        {
            float remaining = delay;
            int lastSecond = Mathf.CeilToInt(delay);
            var originalText = "";
            var countdownText = GetOrCreateCountdownText();
            if (countdownText != null)
            {
                originalText = countdownText.text;
            }

            while (remaining > 0f)
            {
                // 如果面板已被用户手动关闭，直接退出
                if (Alert.Instance == null || !Alert.Instance.Tips_Toggle.activeSelf)
                {
                    _autoConfirmCoroutine = null;
                    yield break;
                }

                remaining -= Time.deltaTime;

                // 每秒更新一次倒计时显示
                int currentSecond = Mathf.CeilToInt(remaining);
                if (currentSecond != lastSecond && countdownText != null)
                {
                    lastSecond = currentSecond;
                    countdownText.text = originalText + $"（{currentSecond}s后自动全选）";
                }

                yield return null;
            }

            // 超时后再次检查面板状态
            if (Alert.Instance == null || !Alert.Instance.Tips_Toggle.activeSelf)
            {
                _autoConfirmCoroutine = null;
                yield break;
            }

            // 面板仍在显示 → 执行自动全选
            Transform allToggleTransform = Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(3);
            if (allToggleTransform != null && allToggleTransform.gameObject.activeSelf)
            {
                var allToggle = allToggleTransform.GetComponent<UnityEngine.UI.Toggle>();
                if (allToggle != null)
                {
                    allToggle.isOn = true;  // 触发 onValueChanged → 自动勾选所有子Toggle
                }
                // 延迟一帧确保 Toggle 事件处理完再点确定
                yield return null;
                // 再次检查（确保用户没有在延迟期间关闭面板）
                if (Alert.Instance != null && Alert.Instance.Tips_Toggle.activeSelf)
                {
                    Alert.Instance.button_yes_Toggle.onClick.Invoke();
                    Debug.Log("[自动超时] 已自动全选并点击确定");
                }
            }

            // 恢复原始文本
            if (countdownText != null)
            {
                countdownText.text = originalText;
            }

            _autoConfirmCoroutine = null;
        }

        /// <summary>
        /// 启动自动超时协程（调用前先停止旧协程）
        /// </summary>
        private void StartAutoConfirm()
        {
            if (_autoConfirmCoroutine != null)
            {
                StopCoroutine(_autoConfirmCoroutine);
            }
            _autoConfirmCoroutine = StartCoroutine(AutoConfirmCoroutine(AUTO_CONFIRM_DELAY));
        }

        /// <summary>
        /// 停止自动超时协程（用户已手动操作时调用）
        /// </summary>
        private void StopAutoConfirm()
        {
            if (_autoConfirmCoroutine != null)
            {
                StopCoroutine(_autoConfirmCoroutine);
                _autoConfirmCoroutine = null;
                Debug.Log("[自动超时] 用户手动操作，已取消超时");
            }
        }

        public bool AreListsEqual<T>(List<T> list1, List<T> list2) where T : IComparable<T>
        {
            if (list1 == null || list2 == null) return false;
            if (list1.Count != list2.Count) return false;

            // Use a dictionary to count occurrences of each element in list1
            var comparer = EqualityComparer<T>.Default;
            var counts = new Dictionary<T, int>(comparer);
            foreach (var item in list1)
            {
                if (counts.TryGetValue(item, out int c)) counts[item] = c + 1;
                else counts[item] = 1;
            }

            // Subtract counts based on list2; if any item missing or count mismatch, lists are not equal
            foreach (var item in list2)
            {
                if (!counts.TryGetValue(item, out int c)) return false;
                if (c == 1) counts.Remove(item);
                else counts[item] = c - 1;
            }

            return counts.Count == 0;
        }

    }
}
