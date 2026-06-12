using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading; // added for ThreadPool
using TMPro;
using UnityEditor; // added for ConcurrentQueue
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static jxzt.Info;
using static jxzt.PicProgress;

namespace jxzt
{
    /// <summary>
    /// 教师端主管理器
    /// 负责教师界面的核心逻辑：题目导入、学生作业管理、自动判分流程、答案校对等功能
    /// 继承自 MainManager，扩展了教师专用的业务逻辑
    /// </summary>
    public class TeacherMainManager : MainManager
    {
        private sealed class StandardAnswerCacheEntry
        {
            public string TitleId;
            public string Url;
            public int RawLength;
            public AnswerFile AnswerFile;
        }

        // 单实例互斥体（仅 Windows 打包生效)
#if UNITY_STANDALONE_WIN
        private static Mutex _singleInstanceMutex;
        private static bool _hasMutex = false;
#endif

        public StudentLayerManager manager;

        public Button upLoadTitle_btn;
        public Button export_btn;
        public Button exportPopOKBtn;
        public Button savePng_btn;
        public Button saveOng_OKbtn;
        public Button importTeacherAnswer_btn;
        public Button importStudentAnswer_btn;
        public Button correctStudentAnswer_btn;
        public Button deleteButton;
        public Button checkButton;

        public Sprite xiugaidaan;
        public Sprite querenxiugai;
        public List<LayerManager> layerManager;
        public GameObject layer_pre;
        public GameObject StudentAnswerInquiry;
        public GameObject error_pre;
        public GameObject error_pre1;
        public GameObject right_pre;
        public Transform layerToggles;
        public new static TeacherMainManager instance;
        public GameObject circleTypeBtn;
        public Button answer_btn;
        public Button closezhengquechengjiPop_btn;
        public GameObject cameraCompare_btn;
        public List<LayerManager> standardlayer_manager;
        public Rect screenGrabRect;
        public string TeacherAnswerFileName;
        public int problemIdSelected;
        private const string ImportedStudentAnswerCacheFolder = "XueShengDaAnImportCache";
        private const string ImportedStudentAnswerCacheVersion = "exif-visible-right-90-v5";
        private const int MaxCachedStandardAnswers = 32;
        private readonly Dictionary<string, StandardAnswerCacheEntry> _standardAnswerCache = new Dictionary<string, StandardAnswerCacheEntry>();
        private readonly Queue<string> _standardAnswerCacheOrder = new Queue<string>();
        private readonly Stack<LayerManager> _standardAnswerLayerPool = new Stack<LayerManager>();
        private Transform _standardAnswerLayerPoolRoot;

        public string StudentAnswerFileName;
        public List<string> StudentAnswerFileNames;
        public Dictionary<string, Texture2D> Dic_StudentAnswerTextures = new();
        int studentAnswernum = 0;
        public int studentAnswerQRnum = 0;
        public int studentAnswerPageCodenum = 0;//无二维码图片题目数量
        public List<List<PositionInt>> qrCodeData = new List<List<PositionInt>>();
        public List<List<PositionInt>> studentMessagePosData = new List<List<PositionInt>>();
        public List<string> qrCodeNums = new List<string>();
        public List<string> titleIdNums = new List<string>();
        public int answerID;//修改学生答案上传需要的ID
        public Text displayTimeInProcessText;
        /// <summary>
        /// 未判分、无姓名学号的学生作业文件名列表
        /// </summary>
        public List<string> NotScoredStudentAnswerFileList = new();
        public List<string> WaitScoredStudentAnswerFileList = new(); //暂时无用
        /// <summary>
        /// 未判分、已有姓名学号的学生作业数据存储字典，Key：学生答案文件名，Value：学生姓名-学号
        /// </summary>
        public Dictionary<string, string> Dic_NoScoredStudentAnswerData = new Dictionary<string, string>();
        /// <summary>
        /// 作业页码存储字典，Key：学生答案文件名，Value：页码
        /// </summary>
        public Dictionary<string, string> Dic_NoScoredStudentTItleData = new Dictionary<string, string>();
        /// <summary>
        /// 进度条下方显示的文本
        /// </summary>
        [SerializeField]
        Text Aegis_text;
        /// <summary>
        /// 预估剩余时间文本（新增）
        /// </summary>
        [SerializeField]
        Text Aegis_remainingTime;
        /// <summary>
        /// 进度条
        /// </summary>        
        public Slider slider;
        /// <summary>
        /// 文字后方点数显示
        /// </summary>
        float pointCount;
        /// <summary>
        /// 当前进度
        /// </summary>
        float progress = 0;
        /// <summary>
        /// 进度条读取完成时间
        /// </summary>
        float total_time = 3f;
        /// <summary>
        /// 计时器
        /// </summary>
        float time = 0;
        /// <summary>
        /// 开始时间戳（用于计算预估剩余时间）
        /// </summary>
        float _progressStartTime = 0f;
        /// <summary>
        /// 上次预估时间更新时刻（避免频繁更新UI）
        /// </summary>
        float _lastTimeEstimateUpdate = 0f;
        /// <summary>
        /// 当前阶段开始时间（用于计算阶段内进度）
        /// </summary>
        float _stageStartTime = 0f;
        /// <summary>
        /// 当前阶段开始进度
        /// </summary>
        float _stageStartProgress = 0f;
        /// <summary>
        /// 当前阶段名称
        /// </summary>
        string _currentStageName = "";
        /// <summary>
        /// 当前阶段内进度（0-1）
        /// </summary>
        float _stageProgress = 0f;

        public bool isCorrected;//学生答案修改是否已执行       
        public bool isSameTitle;//是否是同一题号    

        // 新增标志：是否已将 WaitScored 列表赋值并开始使用（避免引用/并发问题）
        public bool useDic_NoScoredStudentAnswerData = false;

        // 进度动画起始时间（被 AegisAnimation 使用）
        private float startTime = 0f;

        // ========= Added: simple main-thread dispatcher and async import support =========
        private readonly ConcurrentQueue<Action> _mainThreadActions = new ConcurrentQueue<Action>();
        private void RunOnMainThread(Action action)
        {
            if (action != null)
            {
                _mainThreadActions.Enqueue(action);
            }
        }
        private bool IsScoring()
        {
            return PicProgress.instance != null && PicProgress.instance.picProgress > 0f && PicProgress.instance.picProgress < 8f;
        }
        /// <summary>
        /// 判分暂停标志：补录界面或导入学生作业时置为 true，点击"判分"继续时置为 false
        /// </summary>
        public bool pauseScoringFlow = false;

        // 正在等待的学生信息校验请求数（预采集阶段）
        private int _pendingImportExistChecks = 0;
        private void MarkImportCheckFinished()
        {
            if (_pendingImportExistChecks > 0) _pendingImportExistChecks--;
        }

#if UNITY_EDITOR
        private const string EditorTestStudentInfo = "张三-20240718";

        private bool TryApplyEditorTestStudentInfo(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return false;
            }

            string preparedPath = PrepareImportedStudentImageForScoring(filePath);
            if (string.IsNullOrEmpty(preparedPath) || !File.Exists(preparedPath))
            {
                ScoringPerf.Event("EditorTestStudentInfoSkipped", $"file={Path.GetFileName(filePath)};reason=prepareFailed");
                return false;
            }

            if (!string.Equals(preparedPath, filePath, StringComparison.OrdinalIgnoreCase))
            {
                Dic_NoScoredStudentAnswerData.Remove(filePath);
                NotScoredStudentAnswerFileList.Remove(filePath);
                ReplaceStudentAnswerFilePath(filePath, preparedPath);
                if (PicProgress.instance != null && string.Equals(PicProgress.instance.argument1, filePath, StringComparison.OrdinalIgnoreCase))
                {
                    PicProgress.instance.argument1 = preparedPath;
                }
            }

            Dic_NoScoredStudentAnswerData[preparedPath] = EditorTestStudentInfo;
            NotScoredStudentAnswerFileList.Remove(filePath);
            NotScoredStudentAnswerFileList.Remove(preparedPath);
            if (PicProgress.instance != null && string.Equals(PicProgress.instance.argument1, preparedPath, StringComparison.OrdinalIgnoreCase))
            {
                PicProgress.instance.xueShengDaAnName = EditorTestStudentInfo;
            }
            ScoringPerf.Event("EditorTestStudentInfo", $"source={Path.GetFileName(filePath)};work={Path.GetFileName(preparedPath)};student={EditorTestStudentInfo}");
            return true;
        }
#endif

        // 当前待上传的 titleId（在选择题目时设置，上传时直接使用，避免越界）
        private string _currentTitleIdForUpload = string.Empty;
        public string CurrentTitleIdForUpload 
        { 
            get => _currentTitleIdForUpload;
        }
        // 标记：当前图片是否已完成预处理（到达步骤4）。只有当该标记为 true 时，才允许在步骤8中直接跳转到 5
        private bool _preprocessedCurrentImage = false;

        // Snapshots for current image to avoid imported data overriding in-progress lists
        private List<string> _currentQRCodeList = new List<string>();
        private List<string> _currentTitleIdList = new List<string>();
        private string _currentImagePath = string.Empty;
        private string _cachedOriginalPersonalAnswerImagePath = string.Empty;
        private Texture2D _cachedOriginalPersonalAnswerTexture;
        private byte[] _cachedOriginalPersonalAnswerBytes;
        // Remaining counters per current image (decoupled from global counters)
        public int _remainingQRCodeCount = 0;
        public int _remainingPageCodeCount = 0;

        /// <summary>
        /// 外部同步：在题目列表更新（包括复用缓存）时，刷新当前图片的题号快照及计数
        /// </summary>
        public void SyncCurrentTitleIdSnapshot(List<string> newTitleIdList)
        {
            _currentTitleIdList = newTitleIdList != null ? new List<string>(newTitleIdList) : new List<string>();
            _remainingPageCodeCount = _currentTitleIdList.Count;
            studentAnswerPageCodenum = _remainingPageCodeCount;
            foreach (var item in _currentTitleIdList)
            {
                Debug.Log($"[SyncCurrentTitleIdSnapshot] 同步当前图片题号列表" + item);
            }

        }

        public void SyncCurrentQRCodeSnapshot(List<string> newQRCodeList)
        {
            _currentQRCodeList = newQRCodeList != null ? new List<string>(newQRCodeList) : new List<string>();
            _remainingQRCodeCount = _currentQRCodeList.Count;
            studentAnswerQRnum = _remainingQRCodeCount;
            foreach (var item in _currentQRCodeList)
            {
                Debug.Log($"[SyncCurrentQRCodeSnapshot] 同步当前图片二维码题号列表" + item);
            }
        }

        private void ResetCurrentImageTitleState()
        {
            _preprocessedCurrentImage = false;
            _currentQRCodeList.Clear();
            _currentTitleIdList.Clear();
            _currentImagePath = string.Empty;
            _remainingQRCodeCount = 0;
            _remainingPageCodeCount = 0;
            studentAnswerQRnum = 0;
            studentAnswerPageCodenum = 0;
            qrCodeNums.Clear();
            titleIdNums.Clear();
            if (PicProgress.instance != null)
            {
                PicProgress.instance.ocrPageCode = string.Empty;
            }
        }

        /// <summary>
        /// 异步导入：扫描文件夹中的学生作业图片，线程池执行IO，主线程预采集姓名学号并入库
        /// </summary>
        public void StartAsyncImportFromFolder(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
            {
                return;
            }
            // 导入学生作业时，立即暂停判分，等待导入和补录完成
            pauseScoringFlow = true;
            slider.transform.parent.gameObject.SetActive(false);
            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    var files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories)
                        .Where(p => p.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                                    || p.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                                    || p.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                                    || p.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    RunOnMainThread(() =>
                    {
                        // 再次确保暂停（若外部未设置）
                        pauseScoringFlow = true;

                        if (files.Count == 0)
                        {
                            NoScoredStudentMessageOKBtn("所选文件夹未找到图片文件");
                            return;
                        }
                        StartCoroutine(PrecollectImportedFiles(files));
                    });
                }
                catch (Exception ex)
                {
                    RunOnMainThread(() =>
                    {
                        ErrorTipsClear("导入学生作业失败: " + ex.Message, false);
                    });
                }
            });
        }
        // ========= End added =========

        /// <summary>
        /// 初始化教师端主管理器
        /// 设置按钮事件监听、加载未判分学生作业、初始化UI状态
        /// </summary>
        public new void Start()
        {
            base.Start();
            standardlayer_manager = new List<LayerManager>();
            layerManager = new List<LayerManager>();
            StudentAnswerFileNames = new List<string>();
            Dic_NoScoredStudentAnswerData = PendingXueShengZuoYeStorage.Load() ?? new Dictionary<string, string>();
            NormalizePendingStudentAnswerData();
            // 加载 OCR 页码的持久化字典
            Dic_NoScoredStudentTItleData = PendingTitleCodeStorage.Load() ?? new Dictionary<string, string>();
            if (Dic_NoScoredStudentAnswerData.Count > 0)
            {
                Debug.Log($"[TeacherMainManager] Loaded pending {Dic_NoScoredStudentAnswerData.Count} entries from disk: " + Application.streamingAssetsPath);
                NoScoredStudentMessageTipsDisplay("有未判分的学生作业，请点击确定处理！");
            }


            savePng_btn.onClick.AddListener(() =>
            {
                savePng_btn.transform.Find("savepopupPanel").gameObject.SetActive(true);
            });
            saveOng_OKbtn.onClick.AddListener(() =>
            {
                String saveName = saveOng_OKbtn.transform.parent.GetChild(0).GetComponent<InputField>().text;
                if (!string.IsNullOrEmpty(saveName))
                {
                    foreach (Transform child in transform)
                    {
                        if (child.GetComponent<StudentLayerManager>())
                        {
                            savePng_btn.gameObject.GetComponent<SavePng>().InputSave(saveName);
                        }
                    }
                    saveOng_OKbtn.transform.parent.GetChild(0).GetComponent<InputField>().text = "";
                    savePng_btn.transform.GetChild(0).gameObject.SetActive(false);
                }
            });
            export_btn.onClick.AddListener(() =>
            {
                export_btn.transform.Find("savepopupPanel").gameObject.SetActive(true);
            });
            exportPopOKBtn.GetComponent<Button>().onClick.AddListener(() =>
            {
                String FilePath = "TeacherAnswer/" + exportPopOKBtn.transform.parent.GetChild(0).GetComponent<InputField>().text;
                if (!string.IsNullOrEmpty(exportPopOKBtn.transform.parent.GetChild(0).GetComponent<InputField>().text))
                {
                    GetLineTypeData();
                    AnswerFile exportFile = new AnswerFile();
                    exportFile.width = width;
                    exportFile.height = height;

                    for (int i = 0; i < layerManager.Count; i++)
                    {
                        if (layerManager[i].GetData() != null)
                        {
                            LayerData layerData = new LayerData();
                            exportFile.data.Add(layerData);
                            exportFile.data[i].data = layerManager[i].GetData();
                            exportFile.data[i].layerNum = layerManager[i].layerNum;
                            exportFile.data[i].layerError = layerManager[i].layerError;
                            exportFile.data[i].score = layerManager[i].score;
                            standardlayer_manager[i].lineType = (linetype)System.Enum.Parse(typeof(linetype), exportFile.data[i].lineType);
                            exportFile.data[i].frameSelectData = layerManager[i].frameSelectData;
                            exportFile.data[i].xuhaoPosX = layerManager[i].xuhaoPosX;
                            exportFile.data[i].xuhaoPosY = layerManager[i].xuhaoPosY;
                            standardlayer_manager[i].lineshape = (lineshape)System.Enum.Parse(typeof(lineshape), exportFile.data[i].lineshape);
                            exportFile.data[i].circleData = layerManager[i].circleData;
                            exportFile.data[i].arcData = layerManager[i].arcData;
                            exportFile.data[i].ellipseData = layerManager[i].ellipseData;
                            exportFile.data[i].markData = layerManager[i].markData;
                        }
                    }
                    FileStream fs = File.Create(Path.Combine(Application.streamingAssetsPath, FilePath));
                    using (fs)
                    {
                        var bytes = System.Text.Encoding.Default.GetBytes(JsonConvert.SerializeObject(exportFile));
                        fs.Write(bytes, 0, bytes.Length);
                        Debug.Log("save");
                        export_btn.transform.GetChild(0).gameObject.SetActive(false);
                        export_btn.transform.GetChild(1).gameObject.SetActive(true);
                    }
                }
            });

            upLoadTitle_btn.onClick.AddListener(() =>
            {
                importStudentAnswer_btn.transform.GetChild(0).gameObject.SetActive(false);
                importTeacherAnswer_btn.transform.GetChild(0).gameObject.SetActive(!importTeacherAnswer_btn.transform.GetChild(0).gameObject.activeSelf);
            });

            importStudentAnswer_btn.onClick.AddListener(() =>
            {
                // 打开目录选择
                OpenFileWindow.OpenWinFile();

                // 若选择了目录，设置暂停标志，但不立即显示静态提示，等待文件枚举后用可更新的进度弹窗
                if (!string.IsNullOrEmpty(OpenFileWindow.choosedFilefolderPath))
                {
                    pauseScoringFlow = true;
                }

                // 当正在判分时，异步导入，避免打断当前流程（不调用 InitPanfen 等重置方法） 
                if (IsScoring())
                {
                    if (!string.IsNullOrEmpty(OpenFileWindow.choosedFilefolderPath))
                    {
                        StartAsyncImportFromFolder(OpenFileWindow.choosedFilefolderPath);
                    }
                    return;
                }

                bool hasPending = (Dic_NoScoredStudentAnswerData != null && Dic_NoScoredStudentAnswerData.Count > 0)
                                  || (NotScoredStudentAnswerFileList != null && NotScoredStudentAnswerFileList.Count > 0)
                                  || (StudentAnswerFileNames != null && StudentAnswerFileNames.Count > 0);
                if (!hasPending)
                {
                    isSameTitle = false;
                    studentAnswerQRnum = 0;
                    studentAnswerPageCodenum = 0;
                    _remainingQRCodeCount = 0;
                    _remainingPageCodeCount = 0;
                    studentAnswernum = 0;
                    PicProgress.instance.picProgress = 0;
                    slider.transform.parent.gameObject.SetActive(false);
                }

                if (importStudentAnswer_btn.transform.GetChild(1).gameObject.activeSelf)
                {
                    importStudentAnswer_btn.transform.GetChild(1).gameObject.SetActive(false);
                }
                if (importStudentAnswer_btn.transform.GetChild(0).gameObject.activeSelf)
                {
                    importStudentAnswer_btn.transform.GetChild(0).gameObject.SetActive(false);
                }

                if (!string.IsNullOrEmpty(OpenFileWindow.choosedFilefolderPath))
                {
                    importStudentAnswer_btn.transform.GetChild(0).gameObject.SetActive(true);
                    importTeacherAnswer_btn.transform.GetChild(0).gameObject.SetActive(false);
                    isSameTitle = false;

                    var files = Directory.GetFiles(OpenFileWindow.choosedFilefolderPath, "*.*", SearchOption.AllDirectories)
                        .Where(p => p.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                                    || p.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                                    || p.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                                    || p.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    // 使用动态进度弹窗 (0/总数)，后续由 PrecollectImportedFiles 协程更新
                    if (files.Count > 0 && Alert.Instance != null)
                    {
                        Alert.Instance.HideTips();
                        Alert.Instance.ShowTips($"采集学生作业中，请稍等... (0/{files.Count})");
                        // 隐藏确定按钮，保持原有不影响逻辑
                        Alert.Instance.button_yes.gameObject.SetActive(false);
                    }
                    StartCoroutine(PrecollectImportedFiles(files));
                }
            });

            correctStudentAnswer_btn.onClick.AddListener(() =>
            {
                if (LayerToggleManager.instance.ToggleGroup.transform.childCount > 0)
                {
                    if (isCorrected)
                    {
                        string str_ErrorPoints = "";
                        foreach (Transform item in LayerToggleManager.instance.ToggleGroup.transform)
                        {
                            switch (item.GetChild(0).GetChild(1).transform.GetComponent<TMP_Text>().text)
                            {
                                case "正确": str_ErrorPoints += "a"; break;
                                case "线型使用错误": str_ErrorPoints += "b"; break;
                                case "图线不在或偏离正确位置": str_ErrorPoints += "c"; break;
                                case "图线过长": str_ErrorPoints += "d"; break;
                                case "图线过短": str_ErrorPoints += "e"; break;
                                case "剖面线方向绘制错误": str_ErrorPoints += "f"; break;
                                case "剖面线不是45度线": str_ErrorPoints += "g"; break;
                                case "剖面线间距大小不一": str_ErrorPoints += "h"; break;
                                case "在标注位置未发现对应尺寸": str_ErrorPoints += "i"; break;
                                case "尺寸绘制不标准，格式错误": str_ErrorPoints += "j"; break;
                                case "尺寸符号错误": str_ErrorPoints += "k"; break;
                                case "尺寸数值错误": str_ErrorPoints += "l"; break;
                                case "尺寸标注数值位置不对": str_ErrorPoints += "m"; break;
                                case "标注了多余尺寸": str_ErrorPoints += "n"; break;
                                case "公差符号错用": str_ErrorPoints += "o"; break;
                                case "未标注公差": str_ErrorPoints += "p"; break;
                                case "公差数字错误": str_ErrorPoints += "q"; break;
                                case "公差格式错误": str_ErrorPoints += "r"; break;
                                case "基准要素标识位置错误": str_ErrorPoints += "s"; break;
                                case "基准要素标识未标识": str_ErrorPoints += "t"; break;
                                case "基准要素符号错误": str_ErrorPoints += "u"; break;
                                case "剖面图标识未注写": str_ErrorPoints += "v"; break;
                                case "未填写技术要求": str_ErrorPoints += "w"; break;
                                case "回答错误": str_ErrorPoints += "x"; break;
                            }
                        }
                        UpLoadCorrectedAnswerData(str_ErrorPoints, answerID);
                    }

                    transform.GetComponent<RawImage>().texture = null;
                    ClearData();
                    if (correctStudentAnswer_btn.transform.childCount == 3)
                    {
                        Destroy(correctStudentAnswer_btn.transform.GetChild(2).gameObject);
                    }
                }
                else
                {
                    if (correctStudentAnswer_btn.transform.childCount == 2)
                    {
                        GameObject studentAnswerInquiryObj = Instantiate(StudentAnswerInquiry, correctStudentAnswer_btn.transform);
                        isCorrected = false;
                    }
                    else if (correctStudentAnswer_btn.transform.childCount == 3)
                    {
                        Destroy(correctStudentAnswer_btn.transform.GetChild(2).gameObject);
                    }
                    correctStudentAnswer_btn.transform.GetChild(1).gameObject.SetActive(false);
                    importTeacherAnswer_btn.transform.GetChild(0).gameObject.SetActive(false);
                    importStudentAnswer_btn.transform.GetChild(0).gameObject.SetActive(false);
                    transform.GetComponent<RawImage>().texture = null;
                }
            });

            cameraCompare_btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                deleteButton.gameObject.SetActive(false);
                checkButton.gameObject.SetActive(false);
                // 解除判分暂停（若处于补录或导入暂停状态）
                pauseScoringFlow = false;
                importStudentAnswer_btn.transform.GetChild(0).gameObject.SetActive(false);
                importStudentAnswer_btn.transform.GetChild(1).gameObject.SetActive(false);
                importStudentAnswer_btn.transform.GetChild(2).gameObject.SetActive(false);
                Debug.Log("Dic_NoScoredStudentAnswerData.Count" + Dic_NoScoredStudentAnswerData.Count);
                if (Dic_NoScoredStudentAnswerData.Count > 0)
                {
                    useDic_NoScoredStudentAnswerData = true;
                    cameraCompare_btn.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "判分中，请等待。。。";
                    List<string> wait = new();
                    foreach (var item in Dic_NoScoredStudentAnswerData) wait.Add(item.Key);
                    StudentAnswerFileNames = new List<string>(wait);
                    StudentAnswerFileNames.Reverse();
                    studentAnswernum = StudentAnswerFileNames.Count;
                    if (PicProgress.instance.picProgress == 0) StartCoroutine(AegisAnimation(0.5f));
                }
                else if (StudentAnswerFileNames.Count > 0)
                {
                    StudentAnswerFileNames.Reverse();
                    cameraCompare_btn.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "判分中，请等待。。。";
                    studentAnswernum = StudentAnswerFileNames.Count;
                    if (PicProgress.instance.picProgress == 0) StartCoroutine(AegisAnimation(0.5f));
                }
                else
                {
                    cameraCompare_btn.transform.GetChild(1).gameObject.SetActive(true);
                }
            });

            // 判分按钮下手动选择题号展示栏显示或隐藏事件
            cameraCompare_btn.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() =>
            {
                cameraCompare_btn.transform.GetChild(4).GetChild(0).gameObject.SetActive(!cameraCompare_btn.transform.GetChild(4).GetChild(0).gameObject.activeSelf);
            });

            answer_btn.onClick.AddListener(() =>
            {
                foreach (Transform child in transform)
                {
                    if (!child.GetComponent<StudentLayerManager>())
                    {
                        if (child.GetComponent<TeacherLayerManager>().score != 0 || child.GetComponent<TeacherLayerManager>().layerError == "正确")
                        {
                            child.gameObject.SetActive(false);
                        }
                        else
                        {
                            child.GetComponent<RawImage>().enabled = !child.GetComponent<RawImage>().enabled;
                            child.GetChild(1).gameObject.SetActive(!child.GetComponent<RawImage>().enabled);
                            if (child.GetComponent<RawImage>().enabled)
                            {
                                child.GetComponent<LayerManager>().SetActive1();
                            }
                        }
                    }
                    else
                    {
                        child.SetAsFirstSibling();
                    }
                }
            });

            deleteButton.onClick.AddListener(() => {
                if (importStudentAnswer_btn) { 
                    var deletePanle = importStudentAnswer_btn.transform.GetChild(2).gameObject;
                    if (deletePanle) { 
                        deletePanle.gameObject.SetActive(!deletePanle.activeSelf);
                    }
                }
            });

            checkButton.onClick.AddListener(() => {
                if (!PlayerPrefs.HasKey("username")) {
                    Debug.Log("未登录，无法获取班级信息");
                    return;
                }
                var phone = PlayerPrefs.GetString("username", "");

                string fullurl = $"{Config.GetManagedClassesByPhone}?phone={phone}";
                WebManager.Instance.GetStringFunc(fullurl, (s) => {
                    Debug.Log("获取班级信息返回:" + s);
                    if (string.IsNullOrEmpty(s)) { 
                        Debug.Log("获取班级信息失败，返回为空");
                        return;
                    }

                    var response = JsonConvert.DeserializeObject<ReturnClassList>(s);
                    if (response.code != 200) { 
                        Debug.Log($"获取班级信息失败，返回 code {response.code}, message: {response.message}");
                        return;
                    }

                    var classList = response.data?.managed_classes;
                    var classNames = classList.Select(c => c.name).ToList();

                    if (Alert.Instance) { 
                        Alert.Instance.ShowStudentInfoPanel_ClassSelect(classNames);
                    } 

                    Alert.Instance.SetActionOnceOnStudentInfoPanelConfirmButton(() => { 
                        var index = Alert.Instance.StudentInfoPanelDropdown.value;
                        Debug.Log($"选择了班级索引 {index}, 班级名称: {classNames[index]}");
                        string fullurl2 = $"{Config.GetClassStudentsWithAnswerStatus}";

                        UpClassRequestInfo requestInfo = new UpClassRequestInfo(
                            new List<int>() { classList[index].id },
                            new List<int>() { 0 }
                            );

                        WebManager.Instance.GetStringFunc(fullurl2, requestInfo, (s) => { 
                            Debug.Log("获取学生信息返回:" + s);
                            if (string.IsNullOrEmpty(s)) { 
                                Debug.Log("获取学生信息失败，返回为空");
                                return;
                            }

                            var response = JsonConvert.DeserializeObject<ReturnStudentInfoList>(s);
                            if (response.code != 200) {
                                Debug.Log($"获取学生信息失败，返回 code {response.code}, message: {response.message}");
                                return;
                            }

                            var studentInfoList = response.data;
                            var studentIDList = studentInfoList.Select(s => s.student_id).ToList();
                            var targetStudentInfo = new List<ReturnStudentInfoList.Data>();
                            var studentAnswerID = Dic_NoScoredStudentAnswerData.Select(c => c.Value.Split("-")[1]).ToList();
                            for (int i = 0;i<studentInfoList.Count;i++) {
                                if (studentAnswerID.Contains(studentIDList[i])) {
                                    continue;
                                }
                                else {
                                    targetStudentInfo.Add(studentInfoList[i]);
                                }
                            }

                            if (Alert.Instance) {
                                targetStudentInfo.ForEach(student => {
                                    switch (student.student_name.Length) {
                                        case 2:
                                        case 3:
                                            student.student_name += "\t\t";
                                            break;
                                        case 4:
                                            student.student_name += "\t";
                                            break;
                                    }
                                });

                                var studentNameIDStringList = targetStudentInfo.Select(s => $"姓名：{s.student_name}学号：{s.student_id}").ToList();
                                for (int i = 0; i < studentNameIDStringList.Count; i++) {
                                    studentNameIDStringList[i] = $"{i + 1}.{studentNameIDStringList[i]}";
                                }
                                Alert.Instance.ShowStudentInfoPanel_StudentInfo(studentNameIDStringList);
                            }

                        });

                    });

                });


            });

            PicProgress.instance.picProgress = 0;
            instance = this;

            // 定时发送心跳更新登录状态
            StartCoroutine(WebManager.Instance.KeepLogin());
        }

        /// <summary>
        /// 初始化判分流程
        /// 清空所有状态数据：题号列表、二维码计数、学生答案纹理、QR码列表等
        /// 在开始新的一轮判分前调用
        /// </summary>
        public void InitPanfen()
        {
            isSameTitle = false;
            titleIdNums.Clear();
            studentAnswerQRnum = 0;
            studentAnswerPageCodenum = 0;
            _remainingQRCodeCount = 0;
            _remainingPageCodeCount = 0;
            studentAnswernum = 0;
            PicProgress.instance.picProgress = 0;
            slider.transform.parent.gameObject.SetActive(false);
            foreach (var item in Dic_StudentAnswerTextures) Destroy(item.Value);
            Dic_StudentAnswerTextures.Clear();
            cameraCompare_btn.transform.GetComponent<ZXingQRCodeWrapper_ScanQRCode>().qrCodeNumsNoChoose.Clear();
            cameraCompare_btn.transform.GetComponent<ZXingQRCodeWrapper_ScanQRCode>().qrCodeNumsNoChooseDic.Clear();
            cameraCompare_btn.transform.GetComponent<ZXingQRCodeWrapper_ScanQRCode>().qrCodeNums.Clear();
            cameraCompare_btn.transform.GetComponent<ZXingQRCodeWrapper_ScanQRCode>().titleIdNumsNoChoose.Clear();
            cameraCompare_btn.transform.GetComponent<ZXingQRCodeWrapper_ScanQRCode>().titleIdNumsNoChooseDic.Clear();
            cameraCompare_btn.transform.GetComponent<ZXingQRCodeWrapper_ScanQRCode>().titleIdNums.Clear();
            qrCodeNums.Clear();
            titleIdNums.Clear();
            _currentTitleIdForUpload = string.Empty;
            _preprocessedCurrentImage = false;
            _currentQRCodeList.Clear();
            _currentTitleIdList.Clear();
            _currentImagePath = string.Empty;
            _remainingQRCodeCount = 0;
            _remainingPageCodeCount = 0;
            ClearData();
        }

        public void GetqrCodePosData()
        {
            qrCodeData.Clear();
            Debug.Log("TeacherAnswerFileName" + TeacherAnswerFileName);
            var bytes = File.ReadAllBytes(Path.Combine(Application.streamingAssetsPath, "TeacherAnswer/" + TeacherAnswerFileName + ".save"));
            string savejson = System.Text.Encoding.Default.GetString(bytes);
            AnswerFile exportFile = JsonConvert.DeserializeObject<AnswerFile>(savejson);
            for (int i = 0; i < exportFile.data.Count; i++)
            {
                if (exportFile.data[i].lineshape == lineshape.二维码.ToString())
                {
                    qrCodeData = exportFile.data[i].qrCodeData;
                }
            }
            Debug.Log("获取save文件二维码位置数据" + qrCodeData.Count);
        }

        /// <summary>
        /// 从保存的 JSON 数据导入相机/题目数据
        /// 用于恢复之前的判分状态或加载已保存的作业数据
        /// </summary>
        /// <param name="savejson">包含题目数据的 JSON 字符串</param>
        public void CameraImportData(string savejson)
        {
            if (!TryParseStandardAnswer(savejson, out var exportFile))
            {
                return;
            }

            ImportStandardAnswerFile(exportFile);
        }

        private bool TryParseStandardAnswer(string savejson, out AnswerFile exportFile)
        {
            exportFile = null;
            if (string.IsNullOrEmpty(savejson))
            {
                Debug.LogWarning("CameraImportData received empty standard answer content");
                ErrorTipsClear("获取标准答案失败，请稍后重试");
                return false;
            }

            try
            {
                exportFile = JsonConvert.DeserializeObject<AnswerFile>(savejson);
            }
            catch (Exception ex)
            {
                Debug.LogWarning("标准答案解析失败: " + ex.Message);
                ErrorTipsClear("标准答案解析失败");
                return false;
            }

            if (exportFile != null && exportFile.data != null && exportFile.data.Count > 0)
            {
                return true;
            }

            Debug.LogWarning("标准答案内容为空或不完整");
            ErrorTipsClear("标准答案内容为空或不完整");
            return false;
        }

        private bool ImportStandardAnswerFile(AnswerFile exportFile)
        {
            if (exportFile == null || exportFile.data == null || exportFile.data.Count == 0)
            {
                Debug.LogWarning("标准答案内容为空或不完整");
                ErrorTipsClear("标准答案内容为空或不完整");
                return false;
            }

            for (int i = 0; i < exportFile.data.Count; i++)
            {
                if (layer_pre == null)
                {
                    Debug.LogError("layer_pre 未设置，无法实例化标准答案图层");
                    ErrorTipsClear("系统未配置答案图层预制体");
                    return false;
                }

                LoadStandardAnswerLayerButton();

                var src = exportFile.data[i];
                if (standardlayer_manager.Count <= i)
                {
                    Debug.LogError("标准答案层创建失败");
                    ErrorTipsClear("标准答案层创建失败");
                    return false;
                }

                var dst = standardlayer_manager[i];
                if (dst == null)
                {
                    Debug.LogError("标准答案层创建失败");
                    ErrorTipsClear("标准答案层创建失败");
                    return false;
                }

                ResetStandardAnswerLayerData(dst);
                dst.data = src.data;
                // 规范 layerNum：文件中的 layerNum 可能为 0 或非法，回填为 i+1
                int normalizedLayerNum = src.layerNum;
                if (normalizedLayerNum <= 0 || normalizedLayerNum >= transform.childCount)
                {
                    normalizedLayerNum = i + 1; // 第0个子物体是学生层，标准层从1开始
                }
                dst.layerNum = normalizedLayerNum;
                dst.layerError = src.layerError;
                dst.score = src.score;
                dst.frameSelectData = src.frameSelectData;
                if (!Enum.TryParse<linetype>(src.lineType, out var lt)) lt = linetype.unknown;
                dst.lineType = lt;
                dst.xuhaoPosX = src.xuhaoPosX;
                dst.xuhaoPosY = src.xuhaoPosY;
                if (!Enum.TryParse<lineshape>(src.lineshape, out var ls)) ls = lineshape.直线;
                dst.lineshape = ls;
                dst.circleData = src.circleData; // Circle type
                dst.arcData = src.arcData;       // Arc type
                dst.ellipseData = src.ellipseData; // Ellipse type
                dst.markData = src.markData ?? new List<List<PositionInt>>();
                dst.OCRData = src.OCRData ?? new List<List<PositionInt>>();
                dst.OCRString = src.OCRString;
                dst.ocr = src.ocr;
                if (!string.IsNullOrEmpty(src.biaoshi) && src.biaoshi.Contains("#"))
                {
                    dst.biaoshis = src.biaoshi;
                }
                else if (!string.IsNullOrEmpty(src.biaoshi))
                {
                    try { dst.biaoshi = (标识)Enum.Parse(typeof(标识), src.biaoshi); } catch { }
                }
                if (dst.lineshape == lineshape.判分区域)
                {
                    dst.JudgmentZoneData = src.JudgmentZoneData; // List<PositionInt>
                }
            }
            cameraCompare_btn.transform.GetComponent<AnswerCheck>().standardlayer_manager = standardlayer_manager;
            return true;
        }

        private void ResetStandardAnswerLayerData(LayerManager layer)
        {
            if (layer == null)
            {
                return;
            }

            layer.data = null;
            layer.frameSelectData = null;
            layer.JudgmentZoneData = null;
            layer.markData = new List<List<PositionInt>>();
            layer.qrCodeData = new List<List<PositionInt>>();
            layer.OCRData = new List<List<PositionInt>>();
            layer.OCRString = null;
            layer.ocr = null;
            layer.layerError = string.Empty;
            layer.score = 0;
            layer.lineType = linetype.unknown;
            layer.lineshape = lineshape.直线;
            layer.circleData = new Circle();
            layer.arcData = new Arc();
            layer.ellipseData = null;
            layer.biaoshi = default;
            layer.biaoshis = null;
            layer.xuhaoPosX = 0f;
            layer.xuhaoPosY = 0f;
            layer.displayError_position = Vector2.zero;
            layer.isRight = false;
            layer.active = false;
            ResetStandardAnswerLayerVisualState(layer);
        }

        private void ResetStandardAnswerLayerVisualState(LayerManager layer)
        {
            if (layer == null || layer.transform == null)
            {
                return;
            }

            Transform layerTransform = layer.transform;
            if (layerTransform.childCount > 0)
            {
                layerTransform.GetChild(0).gameObject.SetActive(false);
            }
            if (layerTransform.childCount > 1)
            {
                layerTransform.GetChild(1).gameObject.SetActive(false);
            }
            if (layerTransform.childCount > 2)
            {
                layerTransform.GetChild(2).gameObject.SetActive(false);
            }
        }

        private bool TryImportCachedStandardAnswer(string titleid, bool isAreaResource)
        {
            if (string.IsNullOrEmpty(titleid) || !_standardAnswerCache.TryGetValue(titleid, out var entry) || entry?.AnswerFile == null)
            {
                return false;
            }

            ScoringPerf.Event("StandardAnswerCacheHit", $"title={titleid};url={entry.Url};layers={entry.AnswerFile.data?.Count ?? 0};bytes={entry.RawLength}");
            try
            {
                using (ScoringPerf.Scope(ScoringPerf.TitleKey("StandardAnswerImport", titleid), $"cache=true;layers={entry.AnswerFile.data?.Count ?? 0};bytes={entry.RawLength}"))
                {
                    if (!ImportStandardAnswerFile(entry.AnswerFile))
                    {
                        throw new InvalidOperationException("cached standard answer import returned false");
                    }
                }
            }
            catch (Exception ex)
            {
                _standardAnswerCache.Remove(titleid);
                Debug.LogWarning($"[StandardAnswerCache] 缓存导入失败，移除缓存并回退网络加载: title={titleid}, err={ex.Message}");
                ScoringPerf.Event("StandardAnswerCacheInvalid", $"title={titleid};error={ex.Message}");
                return false;
            }

            CompleteStandardAnswerImport(titleid, isAreaResource);
            return true;
        }

        private void CacheStandardAnswer(string titleid, string url, int rawLength, AnswerFile answerFile)
        {
            if (string.IsNullOrEmpty(titleid) || answerFile == null || answerFile.data == null || answerFile.data.Count == 0)
            {
                return;
            }

            if (!_standardAnswerCache.ContainsKey(titleid))
            {
                _standardAnswerCacheOrder.Enqueue(titleid);
            }

            _standardAnswerCache[titleid] = new StandardAnswerCacheEntry
            {
                TitleId = titleid,
                Url = url ?? string.Empty,
                RawLength = rawLength,
                AnswerFile = answerFile
            };

            while (_standardAnswerCache.Count > MaxCachedStandardAnswers && _standardAnswerCacheOrder.Count > 0)
            {
                string expiredTitleId = _standardAnswerCacheOrder.Dequeue();
                if (expiredTitleId != titleid)
                {
                    _standardAnswerCache.Remove(expiredTitleId);
                }
            }

            ScoringPerf.Event("StandardAnswerCacheStore", $"title={titleid};url={url};layers={answerFile.data.Count};bytes={rawLength};cacheCount={_standardAnswerCache.Count}");
        }

        private void CompleteStandardAnswerImport(string titleid, bool isAreaResource)
        {
            if (isAreaResource)
            {
                studentAnswerQRnum--; if (_remainingQRCodeCount > 0) _remainingQRCodeCount--;
            }
            else
            {
                studentAnswerPageCodenum--; if (_remainingPageCodeCount > 0) _remainingPageCodeCount--;
            }

            bool isocr = false;
            for (int i = 0; i < standardlayer_manager.Count; i++)
            {
                if (!isocr && standardlayer_manager[i].lineshape == lineshape.文字识别 && standardlayer_manager[i].lineType == linetype.unknown && standardlayer_manager[i].ocr != 文字识别.姓名.ToString() && standardlayer_manager[i].ocr != 文字识别.学号.ToString()) isocr = true;
                else if (standardlayer_manager[i].lineshape == lineshape.判分区域)
                {
                    if (cameraCompare_btn.transform.GetComponent<RunYOLO>().markPositionInts.Count != 0)
                    {
                        cameraCompare_btn.transform.GetComponent<RunYOLO>().markPositionJudgedInts = cameraCompare_btn.transform.GetComponent<RunYOLO>().IsJudgmentZonePositionOk(cameraCompare_btn.transform.GetComponent<RunYOLO>().markPositionInts, standardlayer_manager[i].JudgmentZoneData);
                    }
                }
            }
            if (cameraCompare_btn.transform.GetComponent<RunYOLO>().markPositionJudgedInts != null && cameraCompare_btn.transform.GetComponent<RunYOLO>().markPositionJudgedInts.Count != 0)
            {
                List<List<PositionInt>> newMarkData = cameraCompare_btn.transform.GetComponent<RunYOLO>().markPositionJudgedInts;
                if (manager.transform.childCount > 0)
                {
                    Transform[] children = new Transform[manager.transform.childCount];
                    for (int j = 0; j < manager.transform.childCount; j++) children[j] = manager.transform.GetChild(j);
                    foreach (Transform child in children) Destroy(child.gameObject);
                }
                manager.markData = newMarkData;
                for (int m = 0; m < manager.markData.Count; m++)
                {
                    Vector2 studenterrorPos = cameraCompare_btn.transform.GetComponent<AnswerCheck>().GetMidpoint(new Vector2(manager.markData[m][0].x, manager.markData[m][0].y), new Vector2(manager.markData[m][2].x, manager.markData[m][2].y));
                    Vector2 errorPos = new Vector2(studenterrorPos.x * 0.6f + 612, studenterrorPos.y * 0.6f + 168);
                    GameObject error1 = Instantiate(error_pre1, manager.transform);
                    error1.SetActive(false);
                    float imageWidth = manager.markData[m][2].x - manager.markData[m][0].x;
                    float imageHeight = manager.markData[m][2].y - manager.markData[m][0].y;
                    RectTransform rectTransform = error1.transform.GetComponent<RectTransform>();
                    rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    rectTransform.sizeDelta = new Vector2(imageWidth * 0.6f, -imageHeight * 0.6f);
                    error1.transform.position = errorPos / 2;
                }
            }
            if (isocr)
            {
                cameraCompare_btn.transform.GetComponent<LoadImageTitle>().LoadImage(PicProgress.instance.argument3, true);
                byte[] imgByte = cameraCompare_btn.transform.GetComponent<LoadImageTitle>().textureChangeScale.EncodeToJPG();
                //判分区域OCR识别
                Debug.Log("判分区域OCR识别");
                using (ScoringPerf.Scope("OCR.JudgmentArea", $"title={titleid};bytes={imgByte.Length}"))
                {
                    GameObject.Find("Script").transform.GetComponent<OCR>().GetHandwriting(imgByte, "CHN_ENG", true, true, true);
                }
            }
            // 确保不被暂停阻塞
            pauseScoringFlow = false;
            StartCoroutine(AegisAnimation(6));
        }

        /// <summary>
        /// 加载标准答案图层按钮
        /// 复用或实例化图层预制体，将其添加到标准答案图层管理器列表中，并设置初始状态
        /// </summary>
        public void LoadStandardAnswerLayerButton()
        {
            LayerManager manager = GetReusableStandardAnswerLayer();
            if (manager == null)
            {
                return;
            }

            standardlayer_manager.Add(manager);
        }

        private LayerManager GetReusableStandardAnswerLayer()
        {
            LayerManager manager = null;
            while (_standardAnswerLayerPool.Count > 0 && manager == null)
            {
                manager = _standardAnswerLayerPool.Pop();
            }

            if (manager == null)
            {
                if (layer_pre == null)
                {
                    return null;
                }

                GameObject layer = Instantiate(layer_pre, transform);
                manager = layer.GetComponent<LayerManager>();
                if (manager == null)
                {
                    Destroy(layer);
                    return null;
                }

                EnsureStandardAnswerLayerBuffer(manager);
            }
            else
            {
                manager.transform.SetParent(transform, false);
                manager.gameObject.SetActive(true);
                EnsureStandardAnswerLayerBuffer(manager);
            }

            PrepareStandardAnswerLayerForImport(manager);
            return manager;
        }

        private void PrepareStandardAnswerLayerForImport(LayerManager manager)
        {
            GameObject layer = manager.gameObject;
            layer.transform.SetAsLastSibling();
            ResetStandardAnswerLayerVisualState(manager);

            RawImage rawImage = layer.GetComponent<RawImage>();
            if (rawImage != null)
            {
                rawImage.enabled = false;
            }
            manager.active = false;
        }

        private void EnsureStandardAnswerLayerBuffer(LayerManager manager)
        {
            RawImage rawImage = manager.GetComponent<RawImage>();
            Size layerSize = manager.LayerSize;
            int expectedLength = width * height;
            bool hasValidBuffer = layerSize != null
                                  && layerSize.width == width
                                  && layerSize.height == height
                                  && manager.Image_colors != null
                                  && manager.Image_colors.Length == expectedLength
                                  && rawImage != null
                                  && rawImage.texture != null;

            if (hasValidBuffer)
            {
                return;
            }

            manager.ClearTextureMemory();
            manager.LayerSize = new Size(width, height);
        }

        private Transform StandardAnswerLayerPoolRoot
        {
            get
            {
                if (_standardAnswerLayerPoolRoot == null)
                {
                    GameObject root = new GameObject("StandardAnswerLayerPool");
                    root.SetActive(false);
                    _standardAnswerLayerPoolRoot = root.transform;
                }

                return _standardAnswerLayerPoolRoot;
            }
        }

        private void RecycleStandardAnswerLayers()
        {
            if (standardlayer_manager == null || standardlayer_manager.Count == 0)
            {
                return;
            }

            for (int i = 0; i < standardlayer_manager.Count; i++)
            {
                RecycleStandardAnswerLayer(standardlayer_manager[i]);
            }
        }

        private void RecycleStandardAnswerLayer(LayerManager layer)
        {
            if (layer == null || layer.GetComponent<StudentLayerManager>() != null)
            {
                return;
            }

            ResetStandardAnswerLayerData(layer);
            RawImage rawImage = layer.GetComponent<RawImage>();
            if (rawImage != null)
            {
                rawImage.enabled = false;
            }

            layer.gameObject.SetActive(false);
            layer.transform.SetParent(StandardAnswerLayerPoolRoot, false);
            _standardAnswerLayerPool.Push(layer);
        }

            private bool isPaused = false;

        /// <summary>
        /// 每帧更新
        /// 执行主线程队列中的任务（来自后台线程的回调）
        /// 检测补录界面状态，恢复导入按钮交互
        /// </summary>
        void Update()
        {
            // 执行主线程队列中的任务（来自后台线程）
            while (_mainThreadActions.TryDequeue(out var action))
            {
                try
                {
                    action?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogError("执行主线程动作失败: " + ex);
                }
            }
            // 补录界面关闭后恢复导入按钮交互
            if (importStudentAnswer_btn != null && importStudentAnswer_btn.transform.childCount > 1)
            {
                var buLuPanel = importStudentAnswer_btn.transform.GetChild(1).gameObject;
                if (!buLuPanel.activeSelf && importStudentAnswer_btn.interactable == false)
                {
                    importStudentAnswer_btn.interactable = true;
                }
            }
        }

        // ========= Added: pre-collect and import-time student info check =========
        /// <summary>
        /// 预采集导入的学生作业文件
        /// 协程：逐文件读取、OCR识别学生姓名学号、更新进度UI
        /// 用于在正式判分前采集学生信息，避免判分流程被打断
        /// </summary>
        /// <param name="files">学生作业文件路径列表</param>
        private IEnumerator PrecollectImportedFiles(List<string> files)
        {
            // 先让出一帧确保UI有时间渲染出弹窗
            yield return null;
            yield return new WaitForEndOfFrame();

            if (files == null || files.Count == 0) yield break;
            // 初始化弹窗显示总数
            int total = files.Count;
            int processed = 0;
            if (Alert.Instance != null)
            {
                Alert.Instance.HideTips();
                Alert.Instance.ShowTips($"采集学生作业中，请稍等... (0/{total})");
                // 隐藏确定按钮，保持原有不影响逻辑
                Alert.Instance.button_yes.gameObject.SetActive(false);
            }
            foreach (var sourceFile in files)
            {
                if (!File.Exists(sourceFile)) { processed++; goto UPDATE_UI; }
                string file = null;
                try
                {
                    file = PrepareImportedStudentImageForScoring(sourceFile);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"学生作业规范化失败: {sourceFile}, err: {ex.Message}");
                    if (!NotScoredStudentAnswerFileList.Contains(sourceFile)) NotScoredStudentAnswerFileList.Add(sourceFile);
                    processed++;
                    goto UPDATE_UI;
                }

                if (string.IsNullOrEmpty(file) || !File.Exists(file)) { processed++; goto UPDATE_UI; }
                if (!string.Equals(file, sourceFile, StringComparison.OrdinalIgnoreCase))
                {
                    Dic_NoScoredStudentAnswerData.Remove(sourceFile);
                    NotScoredStudentAnswerFileList.Remove(sourceFile);
                }
                if (Dic_NoScoredStudentAnswerData.ContainsKey(file))
                {
#if UNITY_EDITOR
                    TryApplyEditorTestStudentInfo(file);
#endif
                    processed++;
                    goto UPDATE_UI;
                }
                Texture2D tex = null;
                try
                {
                    byte[] raw = File.ReadAllBytes(file);
                    tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                    tex.LoadImage(raw, markNonReadable: false);
#if !UNITY_EDITOR
                    var ocr = GameObject.Find("Script").transform.GetComponent<OCR>();
                    var pngBytesForOcr = tex.EncodeToPNG();
                    if (pngBytesForOcr != null)
                    {
                        ocr.GetHandwriting(pngBytesForOcr, "CHN_ENG", false, true, true);
                    }
#endif
#if UNITY_EDITOR
                    TryApplyEditorTestStudentInfo(file);
#else
                    string nameId = GameObject.Find("Script").transform.GetComponent<OCR>().GetOcrNameAndNumberStr();
                    if (!string.IsNullOrEmpty(nameId)) { _pendingImportExistChecks++; CheckStudentExistenceForImport(file, nameId); }
                    else { if (!NotScoredStudentAnswerFileList.Contains(file)) NotScoredStudentAnswerFileList.Add(file); }
#endif
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"预采集失败: {file}, err: {ex.Message}");
                    if (!NotScoredStudentAnswerFileList.Contains(file)) NotScoredStudentAnswerFileList.Add(file);
                }
                finally { if (tex != null) { UnityEngine.Object.Destroy(tex); } }
                processed++;
                UPDATE_UI:
                if (Alert.Instance != null)
                {
                    Alert.Instance.SetTipsText($"采集学生作业中，请稍等... ({processed}/{total})");
                    Alert.Instance.button_yes.gameObject.SetActive(false);
                }
                // 每张图片处理完立即让出一帧，保证 UI 刷新
                yield return null;
            }

#if UNITY_EDITOR
            PendingXueShengZuoYeStorage.Save(Dic_NoScoredStudentAnswerData);
#endif

            // 等待所有异步校验回调完成后再判断 UI 走向
            while (_pendingImportExistChecks > 0)
            {
                yield return null;
            }

            if (NotScoredStudentAnswerFileList.Count > 0)
            {
                ReminderForStudentMessageTipsDisplay("有无法识别到学生信息的作业，请手动输入对应学生信息进行补录！");
                if (importStudentAnswer_btn != null && importStudentAnswer_btn.transform.childCount > 1)
                {
                    importStudentAnswer_btn.transform.GetChild(1).gameObject.SetActive(true);
                    importStudentAnswer_btn.interactable = false; // 禁用按钮直到补录界面关闭
                    //设置删除和检查功能
                    if(config.enableDeletet)
                        deleteButton.gameObject.SetActive(true);
                    if(config.enableCheck)
                        checkButton.gameObject.SetActive(true);
                    
                }
            }
            else if (Dic_NoScoredStudentAnswerData.Count > 0)
            {
                NoScoredStudentMessageOKBtn($"已预采集 {Dic_NoScoredStudentAnswerData.Count} 份作业信息，可点击判分继续");
                //设置删除和检查功能
                if (config.enableDeletet)
                    deleteButton.gameObject.SetActive(true);
                if (config.enableCheck)
                    checkButton.gameObject.SetActive(true);
            }
            else
            {
                // 采集中全部失败或没有有效项，保持原有弹窗隐藏逻辑
                if (Alert.Instance != null)
                {
                    Alert.Instance.text_tips.text = "";
                    Alert.Instance.HideTips();
                }
            }
        }

        /// <summary>
        /// 检查导入的学生是否存在（异步）
        /// 通过API验证学生的姓名和学号是否匹配，匹配则存入待判分字典，不匹配则加入未识别列表
        /// </summary>
        /// <param name="filePath">学生作业文件路径</param>
        /// <param name="xueShengMessage">学生姓名-学号 格式字符串</param>
        private void CheckStudentExistenceForImport(string filePath, string xueShengMessage)
        {
#if UNITY_EDITOR
            if (TryApplyEditorTestStudentInfo(filePath))
            {
                PendingXueShengZuoYeStorage.Save(Dic_NoScoredStudentAnswerData);
                MarkImportCheckFinished();
                return;
            }
#endif
            string[] parts = xueShengMessage.Split('-');
            if (parts.Length != 2)
            {
                if (!NotScoredStudentAnswerFileList.Contains(filePath)) NotScoredStudentAnswerFileList.Add(filePath);
                MarkImportCheckFinished();
                return;
            }

            string fullUrl = $"{Config.GetCheckStudentExistence}?{"first_name"}={parts[0]}&{"studentID"}={parts[1]}";

            WebManager.Instance.GetStringFunc(fullUrl, delegate (string s)
            {
                try
                {
                    if (string.IsNullOrEmpty(s))
                    {
                        if (!NotScoredStudentAnswerFileList.Contains(filePath)) NotScoredStudentAnswerFileList.Add(filePath);
                        return;
                    }
                    ReturnStudentMessageIsExist rs = JsonConvert.DeserializeObject<ReturnStudentMessageIsExist>(s);
                    switch (rs.code)
                    {
                        case 200:
                            if (!rs.data)
                            {
                                if (!NotScoredStudentAnswerFileList.Contains(filePath)) NotScoredStudentAnswerFileList.Add(filePath);
                            }
                            else
                            {
                                Dic_NoScoredStudentAnswerData[filePath] = xueShengMessage;
                                PendingXueShengZuoYeStorage.Save(Dic_NoScoredStudentAnswerData);
                            }
                            break;
                        case 400:
                            if (!NotScoredStudentAnswerFileList.Contains(filePath)) NotScoredStudentAnswerFileList.Add(filePath);
                            break;
                        case 404:
                            TeacherMainManager.instance.ErrorTipsClear("登录失效,请返回登陆界面重新登录", true, true);
                            break;
                    }
                }
                finally
                {
                    MarkImportCheckFinished();
                }
            });
        }

        /// <summary>
        /// 判分流程 - 步骤8：处理完成后检查是否继续
        /// 判断是否有多张图片需要处理，若有则继续下一张，否则结束判分
        /// </summary>
        private void ScoringProcess8()
        {
            if (PicProgress.instance.picProgress == 8)
            {
                Debug.Log("_remainingPageCodeCount8" + _remainingPageCodeCount);
                // Use per-image remaining counters to determine whether to loop within the same image
                if (_preprocessedCurrentImage && (_remainingQRCodeCount > 0 || _remainingPageCodeCount > 0))
                {
                    ScoringPerf.Event("ImageNextTitle", $"qrRemaining={_remainingQRCodeCount};pageRemaining={_remainingPageCodeCount};image={Path.GetFileName(PicProgress.instance.argument1)}");
                    ClearData();
                    //开启协程
                    StartCoroutine(AegisAnimation(5));
                }
                else
                {

                    ScoringPerf.EndImage($"remainingAfter={Math.Max(0, studentAnswernum - 1)}");
                    studentAnswernum--;
                    _preprocessedCurrentImage = false;
                    _currentQRCodeList.Clear();
                    _currentTitleIdList.Clear();
                    _currentImagePath = string.Empty;
                    _remainingQRCodeCount = 0;
                    _remainingPageCodeCount = 0;
                    // 处理完成后，如本次文件存在于补录字典，则移除，避免残留导致后续继续提示
                    if (Dic_NoScoredStudentAnswerData.ContainsKey(PicProgress.instance.argument1))
                    {
                        Dic_NoScoredStudentAnswerData.Remove(PicProgress.instance.argument1);
                        PendingXueShengZuoYeStorage.Save(Dic_NoScoredStudentAnswerData);
                    }
                    if (Dic_NoScoredStudentTItleData.ContainsKey(PicProgress.instance.argument1))
                    {
                        Dic_NoScoredStudentTItleData.Remove(PicProgress.instance.argument1);
                        PendingTitleCodeStorage.Save(Dic_NoScoredStudentAnswerData);
                    }
                    Debug.Log("学生答案待判分数量" + studentAnswernum);
                    ClearData();
                    _currentTitleIdForUpload = string.Empty;
                    if (studentAnswernum > 0)
                    {
                        isSameTitle = true;
                        //开启协程
                        StartCoroutine(AegisAnimation(0.5f));
                    }
                    else
                    {
                        InitPanfen();
                        //开启协程
                        StartCoroutine(AegisAnimation(0));
                    }

                }

            }
        }

        /// <summary>
        /// 判分流程 - 步骤7：上传判分结果
        /// 将学生的判分结果上传到服务器
        /// </summary>
        private void ScoringProcess7()
        {
            if (PicProgress.instance.picProgress == 7 && !savePng_btn.gameObject.GetComponent<SavePng>().capture && savePng_btn.gameObject.GetComponent<LoadImageTitle>().saveImgByte != null)
            {
                UpLoadAnswerData(PicProgress.instance.xueShengDaAnName);

                PicProgress.instance.picProgress = 100;
                UnityEngine.Debug.Log("picProgress" + 8);
            }
        }

        /// <summary>
        /// 判分流程 - 步骤6：启动相机对比协程
        /// 调用 CameraCompare 协程进行学生答案与标准答案的对比
        /// </summary>
        private void ScoringProcess6()
        {
            if (PicProgress.instance.picProgress == 6)
            {
                StartCoroutine(CameraCompare());
            }
        }

        /// <summary>
        /// 判分流程 - 步骤5：获取并加载题目数据
        /// 根据二维码或题号获取题目标准答案数据，加载到对比界面
        /// </summary>
        private void ScoringProcess5()
        {
            if (PicProgress.instance.picProgress == 5)
            {
                Debug.Log("_remainingPageCodeCount51" + _remainingPageCodeCount);
                // 如果是本图第一次进入第5步，懒加载快照，避免中途导入修改全局列表
                if (string.Equals(_currentImagePath, PicProgress.instance.argument1, StringComparison.OrdinalIgnoreCase))
                {
                    if (_currentQRCodeList.Count == 0 && qrCodeNums.Count > 0)
                    {
                        _currentQRCodeList = new List<string>(qrCodeNums);
                        _remainingQRCodeCount = _currentQRCodeList.Count;
                    }
                    if (_currentTitleIdList.Count == 0 && titleIdNums.Count > 0)
                    {
                        _currentTitleIdList = new List<string>(titleIdNums);
                        _remainingPageCodeCount = _currentTitleIdList.Count;
                    }
                }

                if (_currentQRCodeList.Count != 0 && _remainingQRCodeCount > 0)
                {
                    int idx = Mathf.Clamp(_currentQRCodeList.Count - _remainingQRCodeCount, 0, _currentQRCodeList.Count - 1);
                    Debug.Log("二维码id" + _currentQRCodeList[idx]);
                    _currentTitleIdForUpload = _currentQRCodeList[idx];
                    cameraCompare_btn.transform.GetComponent<ZXingQRCodeWrapper_ScanQRCode>().pptid = _currentTitleIdForUpload;
                    ScoringPerf.Event("TitleQueueNext", $"title={_currentTitleIdForUpload};type=qr;index={idx + 1}/{_currentQRCodeList.Count};qrRemaining={_remainingQRCodeCount};pageRemaining={_remainingPageCodeCount}");
                    GetTitleDataByTitleId(_currentTitleIdForUpload, true);

                }
                else if (_currentTitleIdList.Count > 0 && _remainingPageCodeCount > 0)
                {
                    foreach (var item in _currentTitleIdList)
                    {
                        Debug.Log($"[SyncCurrentTitleIdSnapshot] 同步当前图片题号列表1" + item);
                    }
                    int idx2 = Mathf.Clamp(_currentTitleIdList.Count - _remainingPageCodeCount, 0, _currentTitleIdList.Count - 1);
                    Debug.Log("titleIdNums" + _currentTitleIdList[idx2] + "idx2" + idx2);
                    _currentTitleIdForUpload = _currentTitleIdList[idx2];
                    ScoringPerf.Event("TitleQueueNext", $"title={_currentTitleIdForUpload};type=page;index={idx2 + 1}/{_currentTitleIdList.Count};qrRemaining={_remainingQRCodeCount};pageRemaining={_remainingPageCodeCount}");
                    GetTitleDataByTitleId(_currentTitleIdForUpload);
                }
                else
                {
                    // 快照为空或计数为0，回退安全路径，防止越界
                    Debug.LogWarning("ScoringProcess5 缺少可用题号列表或计数，回退至流程1");
                    StartCoroutine(AegisAnimation(1));
                }

            }
        }

        /// <summary>
        /// 判分流程 - 步骤4：加载学生作业图像并预处理
        /// 加载学生作业图片，提取OCR数据，设置图层大小，准备与标准答案对比
        /// </summary>
        private void ScoringProcess4()
        {
            if (PicProgress.instance.picProgress == 4)
            {
                if (transform.childCount > 0)
                {
                    if (GameObject.Find("Script").transform.GetComponent<OCR>().picPositionInts.Count > 0)
                    {
                        manager.OCRData = GameObject.Find("Script").transform.GetComponent<OCR>().picPositionInts;
                    }

                    manager.LayerSize = new Size(width, height);

                    cameraCompare_btn.transform.GetComponent<AnswerCheck>().studentlayer_manager = manager;


                    if (File.Exists(PicProgress.instance.argument4))
                    {
                        using (ScoringPerf.Scope("StudentLayer.LoadImage", $"file={Path.GetFileName(PicProgress.instance.argument4)}"))
                        {
                            cameraCompare_btn.transform.GetComponent<LoadImageTitle>().LoadImage(PicProgress.instance.argument4, true);
                        }
                    }
                    manager.transform.GetComponent<RawImage>().texture = cameraCompare_btn.transform.GetComponent<LoadImageTitle>().texture;
                    using (ScoringPerf.Scope("StudentLayer.ColorArrayUpdate", $"size={width}x{height}"))
                    {
                        manager.ColorArrayUpdate();
                    }

                    manager.SetActive();

                }

                _preprocessedCurrentImage = true;
                //开启协程
                StartCoroutine(AegisAnimation(5));
            }
        }

        /// <summary>
        /// 判分流程 - 步骤3：执行YOLO标识检测
        /// 使用YOLO模型检测学生作业中的标识元素（尺寸标注、符号等）
        /// </summary>
        private void ScoringProcess3()
        {
            if (PicProgress.instance.picProgress == 3)
            {
                if (File.Exists(PicProgress.instance.argument2))
                {
                    //隐藏手动选择题号下拉框
                    cameraCompare_btn.transform.GetChild(4).gameObject.SetActive(false);

                    using (ScoringPerf.Scope("YOLO.Stage3", $"file={Path.GetFileName(PicProgress.instance.argument3)}"))
                    {
                        cameraCompare_btn.transform.GetComponent<LoadImageTitle>().LoadImage(PicProgress.instance.argument3, true);
                        cameraCompare_btn.transform.GetComponent<RunYOLO>().texture2D = cameraCompare_btn.transform.GetComponent<LoadImageTitle>().textureChangeScale;// 

                        cameraCompare_btn.transform.GetComponent<RunYOLO>().ExecuteML();
                    }
                    UnityEngine.Debug.Log("picProgress" + 4 + "获取标识");

                    //开启协程
                    StartCoroutine(AegisAnimation(4));
                }
                else
                {
                    ErrorTipsClear("图像无法处理");
                }
            }
        }

        /// <summary>
        /// 判分流程 - 步骤2：预处理学生作业图像
        /// 去除黑边、旋转校正等预处理操作
        /// </summary>
        private void ScoringProcess2()
        {
            if (PicProgress.instance.picProgress == 2 && File.Exists(PicProgress.instance.argument5))
            {
                using (ScoringPerf.Scope("ImagePostprocess.RemoveBlackFrame", $"input={Path.GetFileName(PicProgress.instance.argument3)};mask={Path.GetFileName(PicProgress.instance.argument5)}"))
                {
                    cameraCompare_btn.transform.GetComponent<LoadImageTitle>().GetTextureRemoveBlackFrame(PicProgress.instance.argument3, PicProgress.instance.argument5);
                }
                if (TeacherMainManager.instance.width == 4480 && TeacherMainManager.instance.height == 2880)
                {

                    if (titleIdNums.Count == 0)
                    {

                        studentAnswerQRnum = 0;
                        studentAnswerPageCodenum = 0;
                        _remainingQRCodeCount = 0;
                        _remainingPageCodeCount = 0;
                        StartCoroutine(AegisAnimation(8));
                    }
                    else
                    {
                        cameraCompare_btn.transform.GetComponent<ZXingQRCodeWrapper_ScanQRCode>().StudentAnswerPageCodenumButtonClick(isSameTitle);
                        // 快照 OCR 页码对应题号
                        if (titleIdNums.Count > 0)
                        {
                            _currentTitleIdList = new List<string>(titleIdNums);
                            _remainingPageCodeCount = _currentTitleIdList.Count;
                            studentAnswerPageCodenum = _remainingPageCodeCount; // keep UI in sync
                            Debug.Log("_remainingPageCodeCount2" + _remainingPageCodeCount);
                        }
                    }
                }
                else if (width == 4360 && height == 3040)
                {
                    if (titleIdNums.Count > 0) {
                        _currentTitleIdList = new List<string>(titleIdNums);
                        _remainingPageCodeCount = _currentTitleIdList.Count;
                        studentAnswerPageCodenum = _remainingPageCodeCount; // keep UI in sync
                        Debug.Log("_remainingPageCodeCount2" + _remainingPageCodeCount);
                        //开启协程
                        StartCoroutine(AegisAnimation(3));
                    }
                    else {
                        //二维码识别题号
                        cameraCompare_btn.transform.GetComponent<ZXingQRCodeWrapper_ScanQRCode>().scanPicTexture = cameraCompare_btn.transform.GetComponent<LoadImageTitle>().textureQRScan;

                        cameraCompare_btn.transform.GetComponent<ZXingQRCodeWrapper_ScanQRCode>().StudentAnswerScanningButtonClick();

                        // 快照二维码题号（如果有）
                        if (qrCodeNums.Count > 0) {
                            _currentQRCodeList = new List<string>(qrCodeNums);
                            _remainingQRCodeCount = _currentQRCodeList.Count;
                            studentAnswerQRnum = _remainingQRCodeCount; // keep UI in sync
                        }
                    }
                }
                else if (width == 4290 && height == 2860)
                {
                    if (titleIdNums.Count == 0)
                    {
                        studentAnswerQRnum = 0;
                        studentAnswerPageCodenum = 0;
                        _remainingQRCodeCount = 0;
                        _remainingPageCodeCount = 0;
                        StartCoroutine(AegisAnimation(8));
                    }
                    else
                    {
                        _currentTitleIdList = new List<string>(titleIdNums);
                        _remainingPageCodeCount = _currentTitleIdList.Count;
                        studentAnswerPageCodenum = _remainingPageCodeCount; // keep UI in sync
                        Debug.Log("_remainingPageCodeCount2" + _remainingPageCodeCount);
                        //开启协程
                        StartCoroutine(AegisAnimation(3));
                    }
                }
            }
        }

        private void ScoringProcess1()
        {
            if (PicProgress.instance.picProgress == 1)
            {
                if (studentAnswernum > 0)
                {
                    ResetCurrentImageTitleState();

                    PicProgress.instance.argument1 = StudentAnswerFileNames[studentAnswernum - 1];
                    _currentImagePath = PicProgress.instance.argument1;
                    ScoringPerf.BeginImage(PicProgress.instance.argument1, studentAnswernum, StudentAnswerFileNames.Count);
                    if (useDic_NoScoredStudentAnswerData)
                    {
                        if (!Dic_NoScoredStudentAnswerData.TryGetValue(PicProgress.instance.argument1, out PicProgress.instance.xueShengDaAnName))
                        {
                            Debug.LogWarning("待判分字典中缺少键，回退至 OCR 流程: " + PicProgress.instance.argument1);
                            cameraCompare_btn.transform.GetComponent<LoadImageTitle>().LoadImage(PicProgress.instance.argument1, true);
                            byte[] imgByte0 = cameraCompare_btn.transform.GetComponent<LoadImageTitle>().textureChangeScale.EncodeToJPG();
                            using (ScoringPerf.Scope("OCR.StudentInfo", $"file={Path.GetFileName(PicProgress.instance.argument1)};bytes={imgByte0.Length}"))
                            {
                                GameObject.Find("Script").transform.GetComponent<OCR>().GetHandwriting(imgByte0, "CHN_ENG", false, true, true);
                            }
                            PicProgress.instance.xueShengDaAnName = "";

                            if (GameObject.Find("Script").transform.GetComponent<OCR>().picPositionInts.Count > 0)
                            {
                                PicProgress.instance.xueShengDaAnName = GameObject.Find("Script").transform.GetComponent<OCR>().GetOcrNameAndNumberStr();
                                Debug.Log("文字识别位置获取姓名学号" + PicProgress.instance.xueShengDaAnName);
                            }

                            if (!string.IsNullOrEmpty(PicProgress.instance.xueShengDaAnName))
                            {
                                // 保存姓名学号
                                CheckStudentExistence(PicProgress.instance.xueShengDaAnName);
                                // 额外保存 OCR 页码（如有）
                                if (!string.IsNullOrEmpty(PicProgress.instance.ocrPageCode))
                                {
                                    Dic_NoScoredStudentTItleData[PicProgress.instance.argument1] = PicProgress.instance.ocrPageCode;
                                    PendingTitleCodeStorage.Save(Dic_NoScoredStudentTItleData);
                                }
                            }
                        }
                        else
                        {
                            // 新增：文件路径保护判断，避免 FileNotFoundException
                            if (string.IsNullOrEmpty(PicProgress.instance.argument1) || !File.Exists(PicProgress.instance.argument1))
                            {
                                Debug.LogWarning($"[LoadImageTitle] 文件不存在或路径为空: {PicProgress.instance.argument1}");
                                TeacherMainManager.instance.studentAnswerQRnum = 0;
                                TeacherMainManager.instance.studentAnswerPageCodenum = 0;
                                TeacherMainManager.instance._remainingQRCodeCount = 0;
                                TeacherMainManager.instance._remainingPageCodeCount = 0;
                                StartCoroutine(TeacherMainManager.instance.AegisAnimation(8));
                                return;
                            }


                            cameraCompare_btn.transform.GetComponent<LoadImageTitle>().LoadImage(PicProgress.instance.argument1, true);
                            byte[] imgByte0 = cameraCompare_btn.transform.GetComponent<LoadImageTitle>().textureChangeScale.EncodeToJPG();
                            using (ScoringPerf.Scope("OCR.StudentInfo", $"file={Path.GetFileName(PicProgress.instance.argument1)};bytes={imgByte0.Length}"))
                            {
                                GameObject.Find("Script").transform.GetComponent<OCR>().GetHandwriting(imgByte0, "CHN_ENG", false, true, true);
                            }


                            GameObject.Find("Script").transform.GetComponent<OCR>().GetOcrNameAndNumberStr(true);
                            if (!string.IsNullOrEmpty(PicProgress.instance.ocrPageCode))
                            {
                                Dic_NoScoredStudentTItleData[PicProgress.instance.argument1] = PicProgress.instance.ocrPageCode;
                                PendingTitleCodeStorage.Save(Dic_NoScoredStudentTItleData);
                                Debug.Log("文字识别保存题号titleIdNums.Count" + titleIdNums.Count);
                            }
                            Debug.Log("文字识别位置获取题号titleIdNums.Count" + titleIdNums.Count);

                        }

                        Debug.Log("Dic_NoScoredStudentAnswerData,count" + Dic_NoScoredStudentAnswerData.Count);
                        Debug.Log("StudentAnswerFileNames,count" + StudentAnswerFileNames.Count);
                        Debug.Log("studentAnswernum,count" + studentAnswernum);
                        PicProgress.instance.argument2 = Application.streamingAssetsPath + "/XueShengDaAn/" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + ".png";
                        PicProgress.instance.argument3 = Application.streamingAssetsPath + "/XueShengDaAn/" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + "tg" + ".png";
                        PicProgress.instance.argument4 = Application.streamingAssetsPath + "/XueShengDaAn/" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + "bc" + ".png";
                        PicProgress.instance.argument5 = Application.streamingAssetsPath + "/XueShengDaAn/" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + "hui" + ".png";
                        using (ScoringPerf.Scope("ExternalPreprocess", $"file={Path.GetFileName(PicProgress.instance.argument1)}"))
                        {
                            ExecuteExternalProgram.StartExternalProgram(PicProgress.instance.exePath, PicProgress.instance.argument1, PicProgress.instance.argument2, PicProgress.instance.argument3, PicProgress.instance.argument4, PicProgress.instance.argument5);//
                        }
                        Debug.Log("picProgress" + 2 + "处理图片数据");
                        Debug.Log(PicProgress.instance.argument1);
                        Debug.Log(PicProgress.instance.xueShengDaAnName);
                        StartCoroutine(AegisAnimation(2));
                    }
                    else
                    {
                        cameraCompare_btn.transform.GetComponent<LoadImageTitle>().LoadImage(PicProgress.instance.argument1, true);
                        byte[] imgByte = cameraCompare_btn.transform.GetComponent<LoadImageTitle>().textureChangeScale.EncodeToJPG();
                        using (ScoringPerf.Scope("OCR.StudentInfo", $"file={Path.GetFileName(PicProgress.instance.argument1)};bytes={imgByte.Length}"))
                        {
                            GameObject.Find("Script").transform.GetComponent<OCR>().GetHandwriting(imgByte, "CHN_ENG", true, true, true);
                        }
                        PicProgress.instance.xueShengDaAnName = "";

                        if (GameObject.Find("Script").transform.GetComponent<OCR>().picPositionInts.Count > 0)
                        {
                            PicProgress.instance.xueShengDaAnName = GameObject.Find("Script").transform.GetComponent<OCR>().GetOcrNameAndNumberStr();
                            Debug.Log("文字识别位置获取姓名学号" + PicProgress.instance.xueShengDaAnName);
                        }

                        if (!string.IsNullOrEmpty(PicProgress.instance.xueShengDaAnName))
                        {
                            CheckStudentExistence(PicProgress.instance.xueShengDaAnName);
                        }
                        else
                        {
                            if (!NotScoredStudentAnswerFileList.Contains(PicProgress.instance.argument1)) NotScoredStudentAnswerFileList.Add(PicProgress.instance.argument1);
                            Debug.Log("学生信息为空");
                            studentAnswerQRnum = 0;
                            studentAnswerPageCodenum = 0;
                            _remainingQRCodeCount = 0;
                            _remainingPageCodeCount = 0;
                            StartCoroutine(AegisAnimation(8));
                        }
                    }
                }
            }
        }

        // ==== Restored methods (original tail) ====
        /// <summary>
        /// 清空判分数据
        /// 释放所有图层、纹理资源，清空标准答案和学生答案数据，重置UI状态
        /// 在切换题目或结束判分时调用
        /// </summary>
        public void ClearData()
        {
            cameraCompare_btn.transform.GetChild(2).gameObject.SetActive(false);
            cameraCompare_btn.transform.GetChild(4).gameObject.SetActive(false);
            RecycleStandardAnswerLayers();
            standardlayer_manager.Clear();
            var answerCheck = cameraCompare_btn.transform.GetComponent<AnswerCheck>();
            if (answerCheck.standardlayer_manager != null && !ReferenceEquals(answerCheck.standardlayer_manager, standardlayer_manager))
            {
                answerCheck.standardlayer_manager.Clear();
            }
            answerCheck.standardlayer_manager = standardlayer_manager;
            if (Alert.Instance != null) Alert.Instance.HideTips();
            if (studentAnswernum == 0)
            {
                TeacherAnswerFileName = "";
                problemIdSelected = 0;
                StudentAnswerFileNames.Clear();
            }
            if (studentAnswerQRnum <= 0 && studentAnswerPageCodenum <= 0)
            {
                layerManager.Clear();
                Destroy(cameraCompare_btn.transform.GetComponent<ZXingQRCodeWrapper_ScanQRCode>().scanPicTexture);
                Destroy(cameraCompare_btn.transform.GetComponent<LoadImageTitle>().texture);
                Destroy(cameraCompare_btn.transform.GetComponent<LoadImageTitle>().textureChangeScale);
                Destroy(cameraCompare_btn.transform.GetComponent<LoadImageTitle>().textureRemovedBlackFrame);
                Destroy(cameraCompare_btn.transform.GetComponent<LoadImageTitle>().textureQRScan);
                Destroy(cameraCompare_btn.transform.GetComponent<RunYOLO>().texture2D);
                Destroy(transform.GetComponent<RawImage>().texture);
                Alert.Instance.DestroyToggleChilds();
            }
            if (LayerToggleManager.instance.ToggleGroup.transform.childCount > 0)
            {
                for (int i = 0; i < LayerToggleManager.instance.ToggleGroup.transform.childCount; i++)
                {
                    Destroy(LayerToggleManager.instance.ToggleGroup.transform.GetChild(i).gameObject);
                }
            }
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<LayerManager>().ClearTextureMemory();
                if (!transform.GetChild(i).GetComponent<StudentLayerManager>())
                {
                    Destroy(transform.GetChild(i).gameObject);
                }
                else
                {
                    manager.markData.Clear();
                    if (manager.transform.childCount > 0)
                    {
                        Transform[] children = new Transform[manager.transform.childCount];
                        for (int j = 0; j < manager.transform.childCount; j++) children[j] = manager.transform.GetChild(j);
                        foreach (Transform child in children) Destroy(child.gameObject);
                    }
                    if (studentAnswerQRnum <= 0 && studentAnswerPageCodenum <= 0)
                    {
                        transform.GetChild(i).GetComponent<RawImage>().texture = null;
                        Destroy(transform.GetChild(i).GetComponent<RawImage>().texture);
                        transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }
            StudentAnswerFileName = "";
            Resources.UnloadUnusedAssets();
        }

        /// <summary>
        /// 相机对比协程
        /// 逐条对比学生答案与标准答案，调用 AnswerCheck.Check 进行线型匹配和位置校验
        /// </summary>
        private IEnumerator CameraCompare()
        {
            while (pauseScoringFlow)
            {
                Aegis_text.text = "判分已暂停，点击判分继续...";
                yield return null;
            }
            // 如果标准答案层为空，回退到 5 重新尝试，避免停在"判分中...0%"
            if (cameraCompare_btn.transform.GetComponent<AnswerCheck>().standardlayer_manager == null ||
                cameraCompare_btn.transform.GetComponent<AnswerCheck>().standardlayer_manager.Count == 0)
            {
                Debug.LogWarning("[CameraCompare] 标准答案层为空，回退至步骤5重试");
                StartCoroutine(AegisAnimation(5));
                yield break;
            }
            manager.gameObject.SetActive(true);
            transform.GetChild(0).GetComponent<RawImage>().texture = cameraCompare_btn.transform.GetComponent<LoadImageTitle>().textureRemovedBlackFrame;
            if (manager.transform.childCount > 0)
            {
                for (int i = 0; i < manager.transform.childCount; i++)
                {
                    manager.transform.GetChild(i).gameObject.SetActive(true);
                }
            }
            var answerCheck = cameraCompare_btn.transform.GetComponent<AnswerCheck>();
            int correctCount = 0;
            float lastProgressUpdate = 0f;
            float c = 0;
            List<LayerManager> standardSnapshot = null;
            ScoringPerf.Start(ScoringPerf.TitleKey("AnswerCheck", _currentTitleIdForUpload), $"layers={answerCheck.standardlayer_manager.Count};image={Path.GetFileName(PicProgress.instance.argument1)}");
            try
            {
                using (ScoringPerf.Scope(ScoringPerf.TitleKey("AnswerCheck.Setup", _currentTitleIdForUpload), $"layers={answerCheck.standardlayer_manager.Count}"))
                {
                    answerCheck.Check();
                }

                // ========== 新增：设置进度回调 ==========
                answerCheck.OnCheckProgress = null;
                answerCheck.OnSingleCheckComplete = null;
                // ====== 修改结束 ======

                answerCheck.OnCheckProgress += (current, total, layer) =>
                {
                    // 每0.1秒更新一次UI，避免过于频繁
                    if (Time.time - lastProgressUpdate < 0.1f) return;
                    lastProgressUpdate = Time.time;

                    // 计算预估剩余时间
                    float remaining = answerCheck.GetEstimatedRemainingTime(current, total);
                    string timeStr = remaining < 60 ? $"{remaining:F0}秒" : $"{remaining / 60:F1}分钟";
                    Debug.Log($"[判分进度] {current + 1}/{total} 正确:{correctCount} 预估剩余:{timeStr}");
                };

                answerCheck.OnSingleCheckComplete += (layer, isCorrect, error) =>
                {
                    if (isCorrect) correctCount++;
                };

                // 使用快照避免遍历过程中集合被修改
                standardSnapshot = new List<LayerManager>(answerCheck.standardlayer_manager);
                for (int si = 0; si < standardSnapshot.Count; si++) {
                    var stand_answer = standardSnapshot[si];
                    long layerStart = ScoringPerf.NowMs;
                    answerCheck.CheckEveryLine(answerCheck.markCount, stand_answer);
                    ScoringPerf.LayerEnd(si + 1, standardSnapshot.Count, stand_answer.layerNum, stand_answer.layerError, layerStart);
                    c++;
                    
                    StartCoroutine(AegisAnimation(6.5f, (c / standardSnapshot.Count), Time.realtimeSinceStartup));
                    yield return null;
                }

                // 触发完成回调
                answerCheck.OnAllCheckComplete?.Invoke(correctCount, (int)c);
                ScoringPerf.End(ScoringPerf.TitleKey("AnswerCheck", _currentTitleIdForUpload), $"layers={(int)c};correct={correctCount}");
            }
            finally
            {
                answerCheck?.ReleaseScoringRuntimeResources(false);
                Debug.Log("[MemoryCleanup] ReleaseScoringRuntimeResources called after one submission");
            }
            yield return null;

            bool isAllRight;
            using (ScoringPerf.Scope(ScoringPerf.TitleKey("PostCheck.DisplayErrors", _currentTitleIdForUpload), $"layers={standardSnapshot.Count}"))
            {
                isAllRight = cameraCompare_btn.transform.GetComponent<AnswerCheck>().DisplayErrorflog();
            }

            using (ScoringPerf.Scope(ScoringPerf.TitleKey("PostCheck.ClearStudentMarkers", _currentTitleIdForUpload), $"children={manager.transform.childCount}"))
            {
                if (manager.transform.childCount > 0)
                {
                    Transform[] children = new Transform[manager.transform.childCount];
                    for (int j = 0; j < manager.transform.childCount; j++) children[j] = manager.transform.GetChild(j);
                    foreach (Transform child in children) Destroy(child.gameObject);
                }
            }

            using (ScoringPerf.Scope(ScoringPerf.TitleKey("PostCheck.LoadToggles", _currentTitleIdForUpload), $"layers={standardlayer_manager.Count}"))
            {
                LayerToggleManager.instance.loadToggle(standardlayer_manager);
            }

            using (ScoringPerf.Scope(ScoringPerf.TitleKey("PostCheck.Scrollbar", _currentTitleIdForUpload)))
            {
                GameObject.Find("togglelayerScrollView").transform.GetChild(2).GetComponent<Scrollbar>().value = 1;
            }

            using (ScoringPerf.Scope(ScoringPerf.TitleKey("PostCheck.LayerVisibility", _currentTitleIdForUpload), $"children={transform.childCount}"))
            {
                foreach (Transform child in transform)
                {
                    if (!child.GetComponent<StudentLayerManager>())
                    {
                        if (child.GetComponent<TeacherLayerManager>().layerError == "正确" || child.GetComponent<TeacherLayerManager>().lineshape == lineshape.二维码 || child.GetComponent<TeacherLayerManager>().lineshape == lineshape.判分区域 || child.GetComponent<TeacherLayerManager>().ocr == 文字识别.姓名.ToString() || child.GetComponent<TeacherLayerManager>().ocr == 文字识别.学号.ToString())
                        {
                            child.gameObject.SetActive(false);
                        }
                        else
                        {
                            child.GetComponent<TeacherLayerManager>().enabled = false;
                            child.gameObject.SetActive(true);
                            child.GetComponent<RawImage>().enabled = true;
                        }
                    }
                    else
                    {
                        child.SetAsFirstSibling();
                    }
                }
            }
            string saveName = "studentAnswerPng" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss");
            if (!string.IsNullOrEmpty(saveName))
            {
                using (ScoringPerf.Scope(ScoringPerf.TitleKey("PostCheck.StartScreenshot", _currentTitleIdForUpload), $"name={saveName}"))
                {
                    savePng_btn.gameObject.GetComponent<SavePng>().InputSave(saveName, false);
                }
            }
            PicProgress.instance.picProgress = 100;
        }

        private void GetLineTypeData()
        {
            if (layerManager.Count != 0)
            {
                layerManager[^1].lineType = layerManager[^1].GetLineType();
                layerManager[^1].xuhaoPosX = transform.GetChild(layerManager.Count - 1).GetChild(1).position.x;
                layerManager[^1].xuhaoPosY = transform.GetChild(layerManager.Count - 1).GetChild(1).position.y;
                layerManager[^1].frameSelectData = Algorithm.GetOBBPixels(layerManager[^1].LayerSize, layerManager[^1].Image_colors).ToArray();
                switch (layerManager[^1].lineshape)
                {
                    case lineshape.圆:
                        layerManager[^1].circleData = layerManager[^1].GetCircleData(layerManager[^1].LayerSize, layerManager[^1].Image_colors); break;
                    case lineshape.圆弧:
                        layerManager[^1].arcData = layerManager[^1].GetArcData(layerManager[^1].LayerSize, layerManager[^1].Image_colors); break;
                    case lineshape.椭圆:
                        layerManager[^1].ellipseData = layerManager[^1].GetEllipseData(layerManager[^1].LayerSize, layerManager[^1].Image_colors); break;
                    case lineshape.标识:
                        layerManager[^1].markData.Clear();
                        layerManager[^1].markData.Add(layerManager[^1].GetMarkData(layerManager[^1]));
                        break;
                    default: layerManager[^1].lineshape = lineshape.直线; break;
                }
                string txt = layerManager[^1].lineshape == lineshape.直线 ? layerManager[^1].lineType.ToString() : layerManager[^1].lineType + layerManager[^1].lineshape.ToString();
                layerToggles.transform.GetChild(layerManager[^1].layerNum - 1).transform.GetChild(1).GetChild(2).GetComponent<TMP_InputField>().text = txt;
            }
        }

        void GetTitleDataByTitleId(string titleid, bool isAreaResource = false)
        {
            WWWForm wwwform = new WWWForm();
            wwwform.AddField("titleid", titleid);
            Debug.Log("titleid：" + titleid);
            ScoringPerf.BeginTitle(titleid, $"isAreaResource={isAreaResource};image={Path.GetFileName(PicProgress.instance.argument1)}");
            if (TryImportCachedStandardAnswer(titleid, isAreaResource))
            {
                return;
            }

            ScoringPerf.Start(ScoringPerf.TitleKey("TitleInfoRequest", titleid), $"url={Config.GetSingleProblemInfoByTitleId}");
            WebManager.Instance.GetStringFunc(Config.GetSingleProblemInfoByTitleId, wwwform, delegate (string s)
            {
                ScoringPerf.End(ScoringPerf.TitleKey("TitleInfoRequest", titleid), $"bytes={(s == null ? 0 : s.Length)}");
                Debug.Log("获取题目信息：" + s);
                if (string.IsNullOrEmpty(s))
                {
                    // 服务器无响应或返回空，直接结束当前图片并进入收尾
                    ScoringPerf.EndTitle(titleid, "result=emptyTitleInfo");
                    _preprocessedCurrentImage = false;
                    if (isAreaResource) { studentAnswerQRnum = 0; } else { studentAnswerPageCodenum = 0; }
                    studentAnswerQRnum = 0;
                    studentAnswerPageCodenum = 0;
                    _remainingQRCodeCount = 0;
                    _remainingPageCodeCount = 0;
                    StartCoroutine(AegisAnimation(8));
                    return;
                }
                ReturnStateTeacher pt = JsonConvert.DeserializeObject<ReturnStateTeacher>(s);
                switch (pt.code)
                {
                    case 200:
                        if (pt.data == null || pt.data.Length == 0 || string.IsNullOrEmpty(pt.data[0].StandardAnswer))
                        {
                            Debug.LogWarning("题目信息缺少 StandardAnswer，回退至步骤5重试");
                            ScoringPerf.EndTitle(titleid, "result=missingStandardAnswer");
                            StartCoroutine(AegisAnimation(5));
                            return;
                        }
                        Debug.Log("下载标准答案URL：" + pt.data[0].StandardAnswer);
                        ScoringPerf.Start(ScoringPerf.TitleKey("StandardAnswerDownload", titleid), $"url={pt.data[0].StandardAnswer}");

                        // 超时兜底控制，防止网络长期无响应导致回调不触发
                        bool requestHandled = false;
                        float stdAnsTimeoutSeconds = 300f; // 可按需要调整

                        IEnumerator TimeoutWatch()
                        {
                            Debug.Log("下载标准答案URL：1");
                            float t = 0f;
                            while (!requestHandled && t < stdAnsTimeoutSeconds)
                            {
                                t += Time.unscaledDeltaTime;
                                yield return null;
                            }
                            if (!requestHandled)
                            {
                                requestHandled = true;
                                ScoringPerf.End(ScoringPerf.TitleKey("StandardAnswerDownload", titleid), "timeout=true");
                                ScoringPerf.EndTitle(titleid, "result=standardAnswerTimeout");
                                Debug.LogWarning($"[GetTitleDataByTitleId] 标准答案获取超时({stdAnsTimeoutSeconds}s)，跳过当前题并进入收尾");
                                _preprocessedCurrentImage = false;
                                if (isAreaResource) { studentAnswerQRnum = 0; } else { studentAnswerPageCodenum = 0; }
                                studentAnswerQRnum = 0;
                                studentAnswerPageCodenum = 0;
                                _remainingQRCodeCount = 0;
                                _remainingPageCodeCount = 0;
                                StartCoroutine(AegisAnimation(8));
                            }
                        }
                        Debug.Log("下载标准答案URL：2");
                        StartCoroutine(TimeoutWatch());
                        Debug.Log("下载标准答案URL：3");
                        WebManager.Instance.GetStringFunc(pt.data[0].StandardAnswer, delegate (string s2)
                        {
                            Debug.Log("下载标准答案URL：4");
                            if (requestHandled) return; // 超时已处理或已完成
                            requestHandled = true;
                            ScoringPerf.End(ScoringPerf.TitleKey("StandardAnswerDownload", titleid), $"bytes={(s2 == null ? 0 : s2.Length)}");

                            Debug.Log("标准答案下载长度：" + (s2 == null ? 0 : s2.Length));
                            if (string.IsNullOrEmpty(s2))
                            {
                                Debug.LogWarning("标准答案下载失败或为空，回退至步骤5重试");
                                ScoringPerf.EndTitle(titleid, "result=emptyStandardAnswer");
                                // 不清零计数，允许重试-
                                StartCoroutine(AegisAnimation(5));
                                return;
                            }

                            AnswerFile parsedStandardAnswer = null;
                            try
                            {
                                using (ScoringPerf.Scope(ScoringPerf.TitleKey("StandardAnswerImport", titleid), $"cache=false;bytes={s2.Length}"))
                                {
                                    if (!TryParseStandardAnswer(s2, out parsedStandardAnswer) || !ImportStandardAnswerFile(parsedStandardAnswer))
                                    {
                                        throw new InvalidOperationException("standard answer import returned false");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.LogError("导入标准答案失败：" + ex.Message + "，回退至步骤5重试");
                                ScoringPerf.EndTitle(titleid, "result=standardAnswerImportError");
                                StartCoroutine(AegisAnimation(5));
                                return;
                            }

                            CacheStandardAnswer(titleid, pt.data[0].StandardAnswer, s2.Length, parsedStandardAnswer);
                            CompleteStandardAnswerImport(titleid, isAreaResource);
                        });
                        break;
                    case 400:
                        ErrorTipsClear(pt.message);
                        ScoringPerf.EndTitle(titleid, $"result=titleInfoCode400;message={pt.message}");
                        break;
                    case 404:
                        TeacherMainManager.instance.ErrorTipsClear("登录失效,请返回登录界面重新登录", true, true);
                        ScoringPerf.EndTitle(titleid, "result=titleInfoCode404");
                        break;
                }
            });
        }    

        public void CorrectStudentAnswerMethod(string title_id, StudentAnswerDataByTitle studentAnswerData)
        {
            char[] strarray = studentAnswerData.WrongPoint.ToCharArray();
            WWWForm wwwform = new WWWForm();
            wwwform.AddField("titleid", title_id);
            WebManager.Instance.GetStringFunc(Config.GetSingleProblemInfoByTitleId, wwwform, delegate (string s)
            {
                if (s.Length == 0) return;
                ReturnStateTeacher pt = JsonConvert.DeserializeObject<ReturnStateTeacher>(s);
                if (pt.code == 200)
                {
                    WebManager.Instance.GetStringFunc(pt.data[0].StandardAnswer, delegate (string s2)
                    {
                        WebManager.Instance.GetTextureFunc(studentAnswerData.PersonalAnswer, t => transform.GetComponent<RawImage>().texture = t);
                        answerID = studentAnswerData.id;
                        CameraImportData(s2);
                        int n = 0;
                        for (int i = 0; i < standardlayer_manager.Count; i++)
                        {
                            if (standardlayer_manager[i].lineshape != lineshape.二维码 && standardlayer_manager[i].lineshape != lineshape.判分区域 && standardlayer_manager[i].ocr != 文字识别.姓名.ToString() && standardlayer_manager[i].ocr != 文字识别.学号.ToString())
                            {
                                LayerToggleManager.instance.loadToggle_ForCorrect(standardlayer_manager[i], strarray[n], n + 1);
                                n++;
                            }
                        }
                        GameObject.Find("togglelayerScrollView").transform.GetChild(2).GetComponent<Scrollbar>().value = 1;
                        foreach (Transform child in transform)
                        {
                            if (child.GetComponent<StudentLayerManager>()) child.GetComponent<StudentLayerManager>().enabled = true;
                            if (!child.GetComponent<StudentLayerManager>())
                            {
                                child.GetComponent<TeacherLayerManager>().enabled = false;
                                child.gameObject.SetActive(true);
                                child.GetChild(0).gameObject.SetActive(false);
                                child.GetChild(1).gameObject.SetActive(false);
                                child.GetComponent<RawImage>().enabled = false;
                            }
                            else child.SetAsFirstSibling();
                        }
                    });
                }
                else if (pt.code == 404)
                {
                    TeacherMainManager.instance.ErrorTipsClear("登录失效,请返回登录界面重新登录", true, true);
                }
                correctStudentAnswer_btn.transform.GetChild(2).gameObject.SetActive(false);
            });
        }

        /// <summary>
        /// 判分动画协程
        /// 控制判分进度条的动画，根据 picProgressNum 驱动整个判分流程的步骤切换
        /// 步骤说明：0.5→1→2→3→4→5→6→7→8 对应预处理→下载→识别→处理→导入→判分→上传流程
        /// </summary>
        /// <param name="picProgressNum">目标进度值（0-8对应各判分步骤）</param>
        /// <param name="progressAddValue">进度条增量</param>
        /// <param name="duration">动画持续时间</param>
        /// <param name="isStartSliderMove">是否启动进度条滑块动画</param>
        public IEnumerator AegisAnimation(float picProgressNum, float progressAddValue = 0, float duration = 0, bool isStartSliderMove = false)
        {
            yield return null;
            PicProgress.instance.picProgress = picProgressNum;
            if (pauseScoringFlow && picProgressNum > 0 && picProgressNum < 8)
            {
                Aegis_text.text = "判分已暂停，点击判分继续...";
                while (pauseScoringFlow) { yield return null; }
            }
            if (PicProgress.instance.picProgress == 0.5f)
            {
                startTime = Time.realtimeSinceStartup;
                _progressStartTime = Time.time;
                _lastTimeEstimateUpdate = Time.time;
                _stageStartTime = Time.time;
                _stageStartProgress = 0f;
                slider.transform.parent.gameObject.SetActive(true);
                progress = 0.04f;
                StartCoroutine(AegisAnimation(1));
            }
            if (PicProgress.instance.picProgress == 0)
            {
                progress = 0;
                if (NotScoredStudentAnswerFileList.Count > 0) ReminderForStudentMessageTipsDisplay("有无法识别到学生信息的作业，请手动输入对应学生信息进行补录！");
                else if (Dic_NoScoredStudentAnswerData.Count > 0)
                {
                    useDic_NoScoredStudentAnswerData = true;
                    cameraCompare_btn.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "判分中，请等待。。。";
                    List<string> wait = new(); foreach (var item in Dic_NoScoredStudentAnswerData) wait.Add(item.Key);
                    StudentAnswerFileNames = new List<string>(wait); StudentAnswerFileNames.Reverse(); studentAnswernum = StudentAnswerFileNames.Count; if (PicProgress.instance.picProgress == 0) StartCoroutine(AegisAnimation(0.5f));
                }
            }
            else if (PicProgress.instance.picProgress == 1) { 
                _stageStartTime = Time.time; _stageStartProgress = 0.11f; _currentStageName = "下载题目数据";
                progress = 0.11f; ScoringProcess1(); 
            }
            else if (PicProgress.instance.picProgress == 2) { 
                _stageStartTime = Time.time; _stageStartProgress = 0.31f; _currentStageName = "识别符号";
                progress = 0.31f; ScoringProcess2(); 
            }
            else if (PicProgress.instance.picProgress == 3) { 
                _stageStartTime = Time.time; _stageStartProgress = 0.41f; _currentStageName = "处理学生数据";
                progress = 0.41f; ScoringProcess3(); 
            }
            else if (PicProgress.instance.picProgress == 4) { 
                _stageStartTime = Time.time; _stageStartProgress = 0.51f; _currentStageName = "导入答案数据";
                progress = 0.51f; ScoringProcess4(); 
            }
            else if (PicProgress.instance.picProgress == 5) { 
                slider.transform.parent.gameObject.SetActive(true); 
                _stageStartTime = Time.time; _stageStartProgress = 0.61f; _currentStageName = "图片处理";
                progress = 0.61f; ScoringProcess5(); 
            }
            else if (PicProgress.instance.picProgress > 6 && PicProgress.instance.picProgress < 7) { 
                progress = 0.61f + progressAddValue * 0.3f; 
            }
            else if (PicProgress.instance.picProgress == 6) { 
                _stageStartTime = Time.time; _stageStartProgress = 0.61f; _currentStageName = "判分中";
                progress = 0.61f; ScoringProcess6(); 
            }
            else if (PicProgress.instance.picProgress == 7) { 
                _stageStartTime = Time.time; _stageStartProgress = 0.96f; _currentStageName = "上传答案";
                progress = 0.96f; ScoringProcess7(); 
            }
            else if (PicProgress.instance.picProgress == 8) { 
                progress = 1; slider.transform.parent.gameObject.SetActive(false); ScoringProcess8(); 
            }

            slider.value = progress;
            float f = slider.value; string reminder = "";
            if (f < 0.1f) reminder = "处理图片数据中...";
            else if (f < 0.3f) reminder = "下载题目数据中...";
            else if (f < 0.4f) reminder = "识别符号中...";
            else if (f < 0.5f) reminder = "处理学生数据中...";
            else if (f < 0.6f) reminder = "导入答案数据中...";
            else if (f < 0.9f) reminder = "判分中..." + (int)(progressAddValue * 100) + "%";
            else if (f < 0.95f) reminder = "上传答案中...";
            else reminder = "上传答案完成...";
            pointCount++; if (pointCount == 7) pointCount = 0; for (int i = 0; i < pointCount; i++) reminder += ".";
            Aegis_text.text = reminder;

            // ========== 新增：预估剩余时间显示 ==========
            UpdateEstimatedRemainingTime();
        }

        /// <summary>
        /// 更新预估剩余时间显示（新增）
        /// 根据当前进度和已用时间估算剩余时间
        /// </summary>
        private void UpdateEstimatedRemainingTime()
        {
            if (Aegis_remainingTime == null) return;
            
            // 避免频繁更新UI（每0.5秒更新一次）
            if (Time.time - _lastTimeEstimateUpdate < 0.5f && progress > 0.05f && progress < 0.98f)
            {
                return;
            }

            _lastTimeEstimateUpdate = Time.time;

            // 计算预估剩余时间
            float elapsed = Time.time - _progressStartTime;

            if (progress > 0.05f && progress < 0.98f && elapsed > 1f)
            {
                // 根据当前进度估算总时间
                float estimatedTotal = elapsed / progress;
                float remaining = estimatedTotal - elapsed;
                
                if (remaining > 0)
                {
                    string timeStr;
                    if (remaining < 60)
                    {
                        timeStr = $"约{remaining:F0}秒";
                    }
                    else
                    {
                        float minutes = remaining / 60;
                        timeStr = $"约{minutes:F1}分钟";
                    }
                    Aegis_remainingTime.text = $"预计剩余: {timeStr}";
                }
                else
                {
                    Aegis_remainingTime.text = "";
                }
            }
            else if (progress >= 0.98f)
            {
                Aegis_remainingTime.text = "";
            }
            else
            {
                Aegis_remainingTime.text = "正在计算...";
            }
        }

        /// <summary>
        /// 更新阶段内进度（新增）
        /// 用于在长时间操作中显示更精细的进度
        /// </summary>
        /// <param name="stageProgress">阶段内进度（0-1）</param>
        /// <param name="stageName">阶段名称</param>
        public void UpdateStageProgress(float stageProgress, string stageName = null)
        {
            if (!string.IsNullOrEmpty(stageName))
            {
                _currentStageName = stageName;
            }
            _stageProgress = Mathf.Clamp01(stageProgress);
            
            // 计算总体进度 = 阶段开始进度 + 阶段内进度 * 阶段跨度
            float stageSpan = 0.3f; // 假设阶段跨度为30%
            float baseProgress = _stageStartProgress;
            progress = baseProgress + _stageProgress * stageSpan;
            progress = Mathf.Clamp01(progress);
            
            slider.value = progress;
            _stageStartTime = Time.time; // 重置阶段计时
            
            UpdateEstimatedRemainingTime();
        }

        /// <summary>
        /// 显示错误提示并清除数据
        /// 弹出错误提示框，用户确认后可选择清除判分数据或返回登录界面
        /// </summary>
        /// <param name="errortips">错误提示文本</param>
        /// <param name="isClearData">确认后是否清除数据</param>
        /// <param name="isReturnLogin">确认后是否返回登录界面</param>
        public void ErrorTipsClear(string errortips, bool isClearData = true, bool isReturnLogin = false)
        {
            Debug.Log(errortips);
            if (Alert.Instance != null)
            {
                Alert.Instance.ShowTips(errortips);
                Alert.Instance.button_yes.onClick.AddListener(delegate ()
                {
                    Alert.Instance.text_tips.text = ""; slider.transform.parent.gameObject.SetActive(false);
                    if (isClearData) { InitPanfen(); }
                    Alert.Instance.button_yes.onClick.RemoveAllListeners(); StartCoroutine(AegisAnimation(0));
                    if (isReturnLogin) { SceneManager.LoadScene("Login"); }
                });
            }
        }

        /// <summary>
        /// 显示未判分学生作业提示（带确定按钮）
        /// 弹窗提示信息，用户可点击确定关闭
        /// </summary>
        /// <param name="tips">提示文本</param>
        /// <param name="isHidenOKBtn">是否隐藏确定按钮</param>
        public void NoScoredStudentMessageOKBtn(string tips, bool isHidenOKBtn = false)
        {
            Debug.Log(tips);
            if (Alert.Instance != null)
            {
                Alert.Instance.ShowTips(tips);
                if (isHidenOKBtn) Alert.Instance.button_yes.gameObject.SetActive(false);
                else
                {
                    Alert.Instance.button_yes.gameObject.SetActive(true);
                    Alert.Instance.button_yes.onClick.AddListener(delegate () { Alert.Instance.text_tips.text = ""; Alert.Instance.button_yes.onClick.RemoveAllListeners(); });
                }
            }
        }

        /// <summary>
        /// 显示未判分学生作业提示（点击后开始判分）
        /// 弹窗提示并点击确定后，若有待判分作业则开始判分流程
        /// </summary>
        /// <param name="tips">提示文本</param>
        public void NoScoredStudentMessageTipsDisplay(string tips)
        {
            Debug.Log(tips);
            if (Alert.Instance != null)
            {
                Alert.Instance.ShowTips(tips);
                Alert.Instance.button_yes.onClick.AddListener(delegate ()
                {
                    Alert.Instance.text_tips.text = "";
                    if (Dic_NoScoredStudentAnswerData.Count > 0)
                    {
                        useDic_NoScoredStudentAnswerData = true;
                        var txt = cameraCompare_btn?.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>();
                        if (txt != null) txt.text = "判分中，请等待。。。";
                        List<string> waitList = new(); foreach (var item in Dic_NoScoredStudentAnswerData) waitList.Add(item.Key);
                        StudentAnswerFileNames = new List<string>(waitList); StudentAnswerFileNames.Reverse(); studentAnswernum = StudentAnswerFileNames.Count; if (PicProgress.instance.picProgress == 0) StartCoroutine(AegisAnimation(0.5f));
                    }
                    Alert.Instance.button_yes.onClick.RemoveAllListeners();
                });
            }
        }
        /// <summary>
        /// 显示学生信息补录提示（点击后打开补录界面）
        /// 用于提示有无法识别学生信息的作业，需要手动输入补录
        /// </summary>
        /// <param name="tips">提示文本</param>
        public void ReminderForStudentMessageTipsDisplay(string tips)
        {
            Debug.Log(tips);
            if (Alert.Instance != null)
            {
                Alert.Instance.HideTips(); Alert.Instance.ShowTips(tips); Alert.Instance.button_yes.gameObject.SetActive(true);
                Alert.Instance.button_yes.onClick.AddListener(delegate () { Alert.Instance.text_tips.text = ""; importStudentAnswer_btn.transform.GetChild(1).gameObject.SetActive(true); importStudentAnswer_btn.interactable = false; Alert.Instance.button_yes.onClick.RemoveAllListeners(); });
            }
        }

        /// <summary>
        /// 显示未识别题目标题提示
        /// 用于提示无法识别的题目标题信息
        /// </summary>
        /// <param name="tips">提示文本</param>
        public void NoRemeberTitleDisplayMessage(string tips)
        {
            Debug.Log(tips);
            if (Alert.Instance != null)
            {
                Alert.Instance.ShowTips(tips); if (Alert.Instance.Tips_Toggle.activeSelf) Alert.Instance.Tips_Toggle.SetActive(false);
                Alert.Instance.button_yes.onClick.AddListener(delegate () { Alert.Instance.text_tips.text = ""; Alert.Instance.button_yes.onClick.RemoveAllListeners(); Alert.Instance.Tips_Toggle.SetActive(true); });
            }
        }

        /// <summary>
        /// 上传学生判分结果数据
        /// 将学生姓名、学号、成绩、错误点、答案截图等数据上传到服务器
        /// </summary>
        /// <param name="xueShengMessage">学生姓名-学号 格式字符串</param>
        void UpLoadAnswerData(string xueShengMessage)
        {
            WWWForm wwwform = new WWWForm();
            string[] parts = xueShengMessage.Split('-');
            if (parts.Length == 2)
            {
                wwwform.AddField("user_name", parts[0]);
                wwwform.AddField("user_studentid", parts[1]);
            }
            byte[] textureByte;
            using (ScoringPerf.Scope("UploadPrepare.EncodeScreenshot", $"title={_currentTitleIdForUpload}"))
            {
                textureByte = savePng_btn.gameObject.GetComponent<SavePng>().mtexture.EncodeToPNG();
            }
            if (textureByte == null) { ErrorTipsClear("学生答案截图获取失败"); }
            string wpstr = cameraCompare_btn.transform.GetComponent<AnswerCheck>().str_ErrorPoints;
            if (wpstr == "") wpstr = "文字识别";
            else
            {
                // 统计 str_ErrorPoints 中 'a'（正确）的个数，计算10分制得分
                int a = wpstr.Count(ch => ch == 'a');
                int totalScoreInt = (int)((a / (float)wpstr.Length) * 10f);
                wwwform.AddField("score", totalScoreInt);
            }
            wwwform.AddField("WrongPoint", wpstr);
            string titleIdToUse = _currentTitleIdForUpload;
            if (string.IsNullOrEmpty(titleIdToUse))
            {
                if (_currentTitleIdList != null && _currentTitleIdList.Count > 0)
                {
                    int idx = Math.Clamp(_currentTitleIdList.Count - _remainingPageCodeCount, 0, _currentTitleIdList.Count - 1); titleIdToUse = _currentTitleIdList[idx];
                }
                else if (_currentQRCodeList != null && _currentQRCodeList.Count > 0)
                {
                    int idx = Math.Clamp(_currentQRCodeList.Count - _remainingQRCodeCount, 0, _currentQRCodeList.Count - 1); titleIdToUse = _currentQRCodeList[idx];
                }
                else titleIdToUse = string.Empty;
            }
            wwwform.AddField("title_id", titleIdToUse ?? string.Empty);
            wwwform.AddField("UserName", PlayerPrefs.GetString("id"));
            wwwform.AddBinaryData("PersonalAnswer", textureByte, "PersonalAnswer.png", "image/png");
            wwwform.AddBinaryData("PersonalAnswer_resize", textureByte, "PersonalAnswer_resize.png", "image/png");
            LoadImageTitle originalLoader = cameraCompare_btn.transform.GetComponent<LoadImageTitle>();
            if (originalLoader != null && originalLoader.textureChangeScale != null)
            {
                byte[] imgByte = GetOriginalPersonalAnswerBytes(originalLoader.textureChangeScale, PicProgress.instance.argument1, titleIdToUse);
                if (imgByte != null)
                {
                    wwwform.AddBinaryData("OriginalPersonalAnswer", imgByte, "OriginalPersonalAnswer.png", "image/png");
                }
            }
            ScoringPerf.Start(ScoringPerf.TitleKey("UploadAnswer", titleIdToUse), $"wrongPoints={wpstr};personalBytes={(textureByte == null ? 0 : textureByte.Length)}");
            WebManager.Instance.GetStringFunc(Config.AnswerUpLoad, wwwform, delegate (string s)
            {
                Debug.Log("上传答案返回数据：" + s);
                ReturnStateTeacher rs = JsonConvert.DeserializeObject<ReturnStateTeacher>(s);
                Debug.Log("上传答案返回：" + rs.code);
                ScoringPerf.End(ScoringPerf.TitleKey("UploadAnswer", titleIdToUse), $"code={rs.code};bytes={(s == null ? 0 : s.Length)}");
                ScoringPerf.EndTitle(titleIdToUse, $"uploadCode={rs.code}");
                if (rs.code == 404) TeacherMainManager.instance.ErrorTipsClear("登录失效,请返回登陆界面重新登录", true, true);
                StartCoroutine(AegisAnimation(8));
            });
        }

        private byte[] GetOriginalPersonalAnswerBytes(Texture2D texture, string imagePath, string titleId)
        {
            string key = imagePath ?? string.Empty;
            if (_cachedOriginalPersonalAnswerBytes != null
                && _cachedOriginalPersonalAnswerTexture == texture
                && string.Equals(_cachedOriginalPersonalAnswerImagePath, key, StringComparison.OrdinalIgnoreCase))
            {
                using (ScoringPerf.Scope("UploadPrepare.EncodeOriginal", $"title={titleId};cache=true;bytes={_cachedOriginalPersonalAnswerBytes.Length}"))
                {
                }
                return _cachedOriginalPersonalAnswerBytes;
            }

            using (ScoringPerf.Scope("UploadPrepare.EncodeOriginal", $"title={titleId};cache=false"))
            {
                _cachedOriginalPersonalAnswerBytes = texture.EncodeToPNG();
            }
            _cachedOriginalPersonalAnswerTexture = texture;
            _cachedOriginalPersonalAnswerImagePath = key;
            return _cachedOriginalPersonalAnswerBytes;
        }

        void UpLoadCorrectedAnswerData(string wrongpoint, int answerID)
        {
            WWWForm wwwform = new WWWForm(); wwwform.AddField("wrongpoint", wrongpoint); wwwform.AddField("answerID", answerID);
            WebManager.Instance.GetStringFunc(Config.CorrectedAnswerUpLoad, wwwform, delegate (string s)
            {
                if (s.Length == 0) return; ReturnStateTeacher rs = JsonConvert.DeserializeObject<ReturnStateTeacher>(s); if (Alert.Instance != null) Alert.Instance.ShowTips(rs.message);
            });
        }
        public void CheckStudentExistence(string xueShengMessage)
        {
#if UNITY_EDITOR
            if (TryApplyEditorTestStudentInfo(PicProgress.instance.argument1))
            {
                PendingXueShengZuoYeStorage.Save(Dic_NoScoredStudentAnswerData);
                PicProgress.instance.xueShengDaAnName = EditorTestStudentInfo;
                studentAnswerQRnum = 0;
                studentAnswerPageCodenum = 0;
                StartCoroutine(AegisAnimation(8));
                return;
            }
#endif
            string[] parts = xueShengMessage.Split('-'); if (parts.Length != 2) { StartCoroutine(AegisAnimation(8)); return; }
            string fullUrl = $"{Config.GetCheckStudentExistence}?first_name={parts[0]}&studentID={parts[1]}";
            WebManager.Instance.GetStringFunc(fullUrl, delegate (string s)
            {
                if (string.IsNullOrEmpty(s)) { if (!NotScoredStudentAnswerFileList.Contains(PicProgress.instance.argument1)) NotScoredStudentAnswerFileList.Add(PicProgress.instance.argument1); studentAnswerQRnum = 0; studentAnswerPageCodenum = 0; StartCoroutine(AegisAnimation(8)); return; }
                ReturnStudentMessageIsExist rs = JsonConvert.DeserializeObject<ReturnStudentMessageIsExist>(s);
                if (rs.code == 200)
                {
                    if (!rs.data) { if (!NotScoredStudentAnswerFileList.Contains(PicProgress.instance.argument1)) NotScoredStudentAnswerFileList.Add(PicProgress.instance.argument1); }
                    else { Dic_NoScoredStudentAnswerData[PicProgress.instance.argument1] = PicProgress.instance.xueShengDaAnName; PendingXueShengZuoYeStorage.Save(Dic_NoScoredStudentAnswerData); }
                    studentAnswerQRnum = 0; studentAnswerPageCodenum = 0; StartCoroutine(AegisAnimation(8));
                }
                else if (rs.code == 400)
                {
                    if (!NotScoredStudentAnswerFileList.Contains(PicProgress.instance.argument1)) NotScoredStudentAnswerFileList.Add(PicProgress.instance.argument1); studentAnswerQRnum = 0; studentAnswerPageCodenum = 0; StartCoroutine(AegisAnimation(8));
                }
                else if (rs.code == 404) { TeacherMainManager.instance.ErrorTipsClear("登录失效,请返回登录界面重新登录", true, true); }
            });
        }
        private static Texture2D RotateTextureByDegrees(Texture2D source, int degrees)
        {
            if (source == null) return null; int d = ((degrees % 360) + 360) % 360; if (d == 0) return source;
            int srcW = source.width; int srcH = source.height; int dstW = (d == 90 || d == 270) ? srcH : srcW; int dstH = (d == 90 || d == 270) ? srcW : srcH; var dst = new Texture2D(dstW, dstH, TextureFormat.RGBA32, false);
            for (int x = 0; x < dstW; x++) for (int y = 0; y < dstH; y++) { Color c; switch (d) { case 90: c = source.GetPixel(srcW - 1 - y, x); break; case 180: c = source.GetPixel(srcW - 1 - x, srcH - 1 - y); break; case 270: c = source.GetPixel(y, srcH - 1 - x); break; default: c = source.GetPixel(x, y); break; } dst.SetPixel(x, y, c); }
            dst.Apply(); return dst;
        }
        private static string PrepareImportedStudentImageForScoring(string sourcePath)
        {
            if (string.IsNullOrEmpty(sourcePath) || !File.Exists(sourcePath)) return sourcePath;

            string cacheRoot = Path.Combine(Application.streamingAssetsPath, ImportedStudentAnswerCacheFolder);
            string cacheDir = Path.Combine(cacheRoot, ImportedStudentAnswerCacheVersion);
            if (IsPathUnderDirectory(sourcePath, cacheDir))
            {
                ScoringPerf.Event("StudentImportImagePrepared", $"source={Path.GetFileName(sourcePath)};cache=true;alreadyPrepared=true");
                return sourcePath;
            }
            if (IsPathUnderDirectory(sourcePath, cacheRoot))
            {
                ScoringPerf.Event("StudentImportImagePrepared", $"source={Path.GetFileName(sourcePath)};cache=true;oldCacheVersion=true;expected={ImportedStudentAnswerCacheVersion};action=reject");
                return null;
            }

            Directory.CreateDirectory(cacheDir);
            FileInfo info = new FileInfo(sourcePath);
            string cacheKey = ComputeImportCacheKey(sourcePath, info.Length, info.LastWriteTimeUtc.Ticks);
            string baseName = SanitizeFileNamePart(Path.GetFileNameWithoutExtension(sourcePath));
            string outputPath = Path.Combine(cacheDir, $"{baseName}_{cacheKey}.png");
            if (File.Exists(outputPath))
            {
                ScoringPerf.Event("StudentImportImagePrepared", $"source={Path.GetFileName(sourcePath)};work={Path.GetFileName(outputPath)};cache=true");
                return outputPath;
            }

            Texture2D sourceTex = null;
            Texture2D outputTex = null;
            try
            {
                byte[] raw = File.ReadAllBytes(sourcePath);
                sourceTex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                sourceTex.LoadImage(raw, markNonReadable: false);
                int exifOrientation = GetJpegExifOrientation(raw);
                int visibleDegrees = GetExifVisibleClockwiseDegrees(exifOrientation);
                int rotateDegrees = (visibleDegrees + 90) % 360;
                outputTex = RotateTextureByDegrees(sourceTex, rotateDegrees);
                SaveTextureToFile(outputTex, outputPath);
                ScoringPerf.Event("StudentImportImagePrepared", $"source={Path.GetFileName(sourcePath)};work={Path.GetFileName(outputPath)};cache=false;rotated=true;degrees={rotateDegrees};exifOrientation={exifOrientation};visibleDegrees={visibleDegrees};from={sourceTex.width}x{sourceTex.height};to={outputTex.width}x{outputTex.height}");
                return outputPath;
            }
            finally
            {
                if (outputTex != null && !ReferenceEquals(outputTex, sourceTex)) Destroy(outputTex);
                if (sourceTex != null) Destroy(sourceTex);
            }
        }
        private static string ComputeImportCacheKey(string sourcePath, long length, long writeTicks)
        {
            string rawKey = $"{Path.GetFullPath(sourcePath).ToLowerInvariant()}|{length}|{writeTicks}";
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(rawKey));
                return BitConverter.ToString(hash, 0, 8).Replace("-", "").ToLowerInvariant();
            }
        }
        private static int GetExifVisibleClockwiseDegrees(int orientation)
        {
            switch (orientation)
            {
                case 3:
                case 4:
                    return 180;
                case 5:
                case 6:
                    return 90;
                case 7:
                case 8:
                    return 270;
                default:
                    return 0;
            }
        }
        private static int GetJpegExifOrientation(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 4 || bytes[0] != 0xFF || bytes[1] != 0xD8)
            {
                return 1;
            }

            int index = 2;
            while (index + 4 <= bytes.Length)
            {
                while (index < bytes.Length && bytes[index] == 0xFF) index++;
                if (index >= bytes.Length) break;

                byte marker = bytes[index++];
                if (marker == 0xDA || marker == 0xD9) break;
                if (index + 2 > bytes.Length) break;

                int segmentLength = ReadUInt16BigEndian(bytes, index);
                if (segmentLength < 2 || index + segmentLength > bytes.Length) break;

                int segmentStart = index + 2;
                int segmentEnd = index + segmentLength;
                if (marker == 0xE1 && TryReadExifOrientation(bytes, segmentStart, segmentEnd, out int orientation))
                {
                    return orientation;
                }

                index += segmentLength;
            }

            return 1;
        }
        private static bool TryReadExifOrientation(byte[] bytes, int segmentStart, int segmentEnd, out int orientation)
        {
            orientation = 1;
            if (segmentStart + 14 > segmentEnd) return false;
            if (bytes[segmentStart] != 0x45 || bytes[segmentStart + 1] != 0x78 || bytes[segmentStart + 2] != 0x69 || bytes[segmentStart + 3] != 0x66 || bytes[segmentStart + 4] != 0 || bytes[segmentStart + 5] != 0)
            {
                return false;
            }

            int tiffStart = segmentStart + 6;
            bool littleEndian;
            if (bytes[tiffStart] == 0x49 && bytes[tiffStart + 1] == 0x49)
            {
                littleEndian = true;
            }
            else if (bytes[tiffStart] == 0x4D && bytes[tiffStart + 1] == 0x4D)
            {
                littleEndian = false;
            }
            else
            {
                return false;
            }

            if (ReadUInt16(bytes, tiffStart + 2, littleEndian) != 42) return false;
            uint ifdOffset = ReadUInt32(bytes, tiffStart + 4, littleEndian);
            int ifdStart = tiffStart + (int)ifdOffset;
            if (ifdStart < tiffStart || ifdStart + 2 > segmentEnd) return false;

            int entryCount = ReadUInt16(bytes, ifdStart, littleEndian);
            int entryStart = ifdStart + 2;
            for (int i = 0; i < entryCount; i++)
            {
                int entry = entryStart + i * 12;
                if (entry + 12 > segmentEnd) break;

                int tag = ReadUInt16(bytes, entry, littleEndian);
                if (tag != 0x0112) continue;

                int type = ReadUInt16(bytes, entry + 2, littleEndian);
                uint count = ReadUInt32(bytes, entry + 4, littleEndian);
                if (type != 3 || count < 1) return false;

                orientation = ReadUInt16(bytes, entry + 8, littleEndian);
                return orientation >= 1 && orientation <= 8;
            }

            return false;
        }
        private static int ReadUInt16BigEndian(byte[] bytes, int index)
        {
            return (bytes[index] << 8) | bytes[index + 1];
        }
        private static int ReadUInt16(byte[] bytes, int index, bool littleEndian)
        {
            if (littleEndian) return bytes[index] | (bytes[index + 1] << 8);
            return (bytes[index] << 8) | bytes[index + 1];
        }
        private static uint ReadUInt32(byte[] bytes, int index, bool littleEndian)
        {
            if (littleEndian)
            {
                return (uint)(bytes[index] | (bytes[index + 1] << 8) | (bytes[index + 2] << 16) | (bytes[index + 3] << 24));
            }
            return (uint)((bytes[index] << 24) | (bytes[index + 1] << 16) | (bytes[index + 2] << 8) | bytes[index + 3]);
        }
        private static string SanitizeFileNamePart(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "student_answer";
            char[] invalidChars = Path.GetInvalidFileNameChars();
            StringBuilder builder = new StringBuilder(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                builder.Append(invalidChars.Contains(c) || char.IsWhiteSpace(c) ? '_' : c);
            }
            string result = builder.ToString().Trim('_');
            if (string.IsNullOrEmpty(result)) result = "student_answer";
            return result.Length > 48 ? result.Substring(0, 48) : result;
        }
        private static bool IsPathUnderDirectory(string path, string directory)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(directory)) return false;
            string fullPath = Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string fullDirectory = Path.GetFullPath(directory).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
            return fullPath.StartsWith(fullDirectory, StringComparison.OrdinalIgnoreCase);
        }
        private void NormalizePendingStudentAnswerData()
        {
            if (Dic_NoScoredStudentAnswerData == null || Dic_NoScoredStudentAnswerData.Count == 0) return;

            bool changed = false;
            Dictionary<string, string> normalized = new Dictionary<string, string>();
            foreach (var item in Dic_NoScoredStudentAnswerData)
            {
                string sourcePath = item.Key;
                if (string.IsNullOrEmpty(sourcePath) || !File.Exists(sourcePath))
                {
                    changed = true;
                    ScoringPerf.Event("PendingStudentAnswerDropped", $"file={Path.GetFileName(sourcePath)};reason=missing");
                    continue;
                }

                string preparedPath = PrepareImportedStudentImageForScoring(sourcePath);
                if (string.IsNullOrEmpty(preparedPath) || !File.Exists(preparedPath))
                {
                    changed = true;
                    ScoringPerf.Event("PendingStudentAnswerDropped", $"file={Path.GetFileName(sourcePath)};reason=prepareFailed");
                    continue;
                }

                normalized[preparedPath] = item.Value;
                if (!string.Equals(preparedPath, sourcePath, StringComparison.OrdinalIgnoreCase))
                {
                    changed = true;
                    ScoringPerf.Event("PendingStudentAnswerNormalized", $"source={Path.GetFileName(sourcePath)};work={Path.GetFileName(preparedPath)}");
                }
            }

            if (!changed) return;
            Dic_NoScoredStudentAnswerData = normalized;
            PendingXueShengZuoYeStorage.Save(Dic_NoScoredStudentAnswerData);
        }
        private void ReplaceStudentAnswerFilePath(string oldPath, string newPath)
        {
            if (StudentAnswerFileNames == null || string.IsNullOrEmpty(oldPath) || string.IsNullOrEmpty(newPath)) return;
            for (int i = 0; i < StudentAnswerFileNames.Count; i++)
            {
                if (string.Equals(StudentAnswerFileNames[i], oldPath, StringComparison.OrdinalIgnoreCase))
                {
                    StudentAnswerFileNames[i] = newPath;
                }
            }
        }
        private static void SaveTextureToFile(Texture2D tex, string path)
        {
            if (tex == null || string.IsNullOrEmpty(path)) return; try { string dir = Path.GetDirectoryName(path); if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir); byte[] bytes; string ext = Path.GetExtension(path).ToLowerInvariant(); bytes = (ext == ".jpg" || ext == ".jpeg") ? tex.EncodeToJPG(95) : tex.EncodeToPNG(); File.WriteAllBytes(path, bytes); } catch (Exception ex) { Debug.LogWarning("保存图片失败: " + ex.Message); }
        }

        public void RemoveImportedStudentFile(string filePath) {
            if (Dic_NoScoredStudentAnswerData.ContainsKey(filePath)) {
                Dic_NoScoredStudentAnswerData.Remove(filePath);
                PendingXueShengZuoYeStorage.Save(Dic_NoScoredStudentAnswerData);
                Debug.Log($"删除学生作答: {filePath}\nDic_NoScoredStudentAnswerData.count=" + Dic_NoScoredStudentAnswerData.Count);
            }
        }

        public void RemoveImportedStudentFile(List<string> filePath) {
            foreach (var i in filePath) {
                RemoveImportedStudentFile(i);
            }
        }
    }
}

