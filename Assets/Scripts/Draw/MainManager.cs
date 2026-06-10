using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography;
using TMPro;
using System.Net; // Added to control TLS versions

namespace jxzt
{
    /// <summary>
    /// MainManager - 绘图系统主管理器
    /// 负责管理画布尺寸、画笔工具、配置文件加载、UI 按钮事件等
    /// 采用单例模式，确保全局只有一个实例
    /// </summary>
    public class MainManager : MonoBehaviour
    {
        // ========== 画布尺寸 ==========
        /// <summary>画布宽度（像素）- 从配置文件读取</summary>
        public int width;
        /// <summary>画布高度（像素）- 从配置文件读取</summary>
        public int height;
        /// <summary>绘图配置对象 - 从 draw_config.json 加载</summary>
        public DrawConfig config;
        /// <summary>背景 RawImage 组件引用</summary>
        public RawImage background_image;

        // ========== 画笔管理 ==========
        /// <summary>当前画笔类型（paint：绘画，eraser：擦除）</summary>
        public brushType brushtype;
        /// <summary>
        /// 当前使用的画笔（根据 brushtype 自动切换）
        /// - brushtype = paint 时返回 paint_brush
        /// - brushtype = eraser 时返回 era_brush
        /// </summary>
        public Brush currentBrush
        {
            get
            {
                switch (brushtype)
                {
                    case brushType.paint:
                        return paint_brush;
                    case brushType.eraser:
                        return era_brush;
                    default:
                        return paint_brush;
                }
            }
        }

        /// <summary>当前画笔的纹理贴图</summary>
        public Texture2D ECurTex;

        /// <summary>绘画画笔（实心圆）</summary>
        public Brush paint_brush;
        /// <summary>橡皮擦画笔（透明圆）</summary>
        public Brush era_brush;
        /// <summary>单例实例 - 用于全局访问</summary>
        public static MainManager instance;

        // ========== UI 按钮引用 ==========
        /// <summary>粗实线按钮（可调节线宽）</summary>
        public Button paint_btn_cu;
        /// <summary>圆弧按钮</summary>
        public Button paint_btn_arc;
        /// <summary>细实线按钮（固定线宽）</summary>
        public Button paint_btn_xi;
        /// <summary>橡皮擦按钮</summary>
        public Button era_btn;

        // ========== 内部变量 ==========
        int paintScale;  // 当前绘画线宽
        int eraScale;    // 当前橡皮擦大小

        // ========== 配置常量 ==========
        /// <summary>配置文件名</summary>
        private const string ConfigFileName = "draw_config.json";

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // Force TLS 1.2 early to avoid handshake failures on HTTPS endpoints
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // 在 Awake 尽早加载配置，这样其他使用 width/height 的逻辑可以可靠读取到正确值
            LoadConfig();
        }

        public void Start()
        {
            paintScale = 3;
            eraScale = 30;
            CursorManager.instance.gameObject.SetActive(false);
            paint_btn_arc.onClick.AddListener(() =>
            {
                paint_brush = Tools.CreateCircleBrush(paintScale);
                paint_brush.color = new Color32(255, 0, 0, 255);
                brushtype = brushType.paint;
                CursorManager.instance.ChangeSize(paintScale);
            });
            paint_btn_cu.onClick.AddListener(() =>
            {
                paint_brush = Tools.CreateCircleBrush(paintScale);
                paint_brush.color = new Color32(255, 0, 0, 255);
                brushtype = brushType.paint;
                CursorManager.instance.ChangeSize(paintScale);
                paint_btn_cu.transform.GetChild(1).GetChild(0).gameObject.SetActive(!paint_btn_cu.transform.GetChild(1).GetChild(0).gameObject.activeSelf);
                paint_btn_cu.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(paintScale * 2, paintScale * 2);
            });
            paint_btn_cu.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() =>
            {

            });
            paint_btn_cu.transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Scrollbar>().onValueChanged.AddListener((float v) =>
            {
                paintScale = (int)(3 * 10 * v);
                paint_brush = Tools.CreateCircleBrush(paintScale);
                paint_brush.color = new Color32(255, 0, 0, 255);
                brushtype = brushType.paint;
                CursorManager.instance.ChangeSize(paintScale);
                paint_btn_cu.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(paintScale * 2, paintScale * 2);
            });
            paint_btn_xi.onClick.AddListener(() =>
            {
                paint_brush = Tools.CreateCircleBrush(3);
                paint_brush.color = new Color32(255, 0, 0, 255);
                brushtype = brushType.paint;
                CursorManager.instance.ChangeSize(3);
            });

            era_btn.onClick.AddListener(() =>
            {
                brushtype = brushType.eraser;

                era_brush = Tools.CreateCircleBrush(eraScale);
                era_brush.color = new Color32(0, 0, 0, 0);
                CursorManager.instance.ChangeSize(eraScale);
                era_btn.transform.GetChild(1).GetChild(0).gameObject.SetActive(!era_btn.transform.GetChild(1).GetChild(0).gameObject.activeSelf);
                era_btn.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(eraScale * 2, eraScale * 2);
            });
            era_btn.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() =>
            {

            });
            era_btn.transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Scrollbar>().onValueChanged.AddListener((float v) =>
            {
                eraScale = (int)(10 * 10 * v);
                era_brush = Tools.CreateCircleBrush(eraScale);
                era_brush.color = new Color32(0, 0, 0, 0);
                brushtype = brushType.eraser;
                CursorManager.instance.ChangeSize(eraScale);
                era_btn.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(eraScale * 2, eraScale * 2);
            });

        }

        /// <summary>
        /// 重新加载配置文件
        /// 可在运行时调用以重新读取外部配置
        /// </summary>
        public void ReloadConfig()
        {
            LoadConfig();
        }

        /// <summary>
        /// 绘图配置数据结构 - 对应 draw_config.json
        /// </summary>
        public class DrawConfig
        {
            public string sign = "zy";
            /// <summary>是否启用删除功能</summary>
            public bool enableDeletet = false;
            /// <summary>是否启用检查功能</summary>
            public bool enableCheck = false;
            /// <summary>是否启用选择功能</summary>
            public bool enableSelect = false;
        }

        /// <summary>
        /// 加载绘图配置
        /// 优先从 StreamingAssets/Config 目录读取 draw_config.json
        /// 若文件不存在则创建默认配置文件
        /// 配置内容包括画布尺寸、功能开关等
        /// </summary>
        private void LoadConfig()
        {
            string streamingPath = Path.Combine(Application.streamingAssetsPath+"/Config/", ConfigFileName);
            string json = null;
            string usedPath = null;

            try
            {
                if (File.Exists(streamingPath))
                {
                    // 从 StreamingAssets 目录读取配置文件
                    json = File.ReadAllText(streamingPath);
                    usedPath = streamingPath;
                }
                else
                {
                    // 文件不存在时创建默认配置文件
                    DrawConfig defaultCfg = new DrawConfig();
                    json = JsonConvert.SerializeObject(defaultCfg, Formatting.Indented);
                    Directory.CreateDirectory(Path.GetDirectoryName(streamingPath) ?? Application.streamingAssetsPath);
                    File.WriteAllText(streamingPath, json);
                    usedPath = streamingPath;
                }

                if (!string.IsNullOrEmpty(json))
                {
                    DrawConfig cfg = JsonConvert.DeserializeObject<DrawConfig>(json);
                    if (cfg != null)
                    {
                        // 根据 sign 标识获取对应的画布尺寸
                        var size = GetSizeBySign(cfg.sign);
                        width = size.x;
                        height = size.y;
                        config = cfg;

                        // 根据配置设置选题功能的开关状态
                        var toggle = Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(4).transform.GetComponent<Toggle>();
                        toggle.isOn = !config.enableSelect;
                            
                        // 输出配置加载日志
                        Debug.Log($"[MainManager] Loaded draw config from: {usedPath} \n" +
                            $"(width={width}, height={height})\n" +
                            $"sign={config.sign}\n" +
                            $"enableDeletet={config.enableDeletet}\n" +
                            $"enableCheck={config.enableCheck}\n" +
                            $"enableSelect={config.enableSelect}\n");
                    }
                    else
                    {
                        Debug.LogWarning("[MainManager] 解析 draw_config.json 失败，使用默认值。");
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[MainManager] 读取或解析配置文件失败: {ex.Message}\n将使用当前脚本中设置的 width/height 值。");
                Debug.LogException(ex);
            }
        }

        /// <summary>
        /// 根据版本标识符获取画布尺寸
        /// 不同版本对应不同的画布分辨率
        /// </summary>
        /// <param name="sign">版本标识符（ht：张莹，zy：智慧制造，a/pb/sk：测试）</param>
        /// <returns>画布尺寸（x：宽度，y：高度）</returns>
        public Vector2Int GetSizeBySign(string sign) {
            var size = Vector2Int.zero;
            switch (sign) {
                case "ht":  //智慧制造
                    size.x = 4360;
                    size.y = 3040;
                    break;
                case "zy":  //张莹
                    size.x = 4480;
                    size.y = 2880;
                    break;
                case "a":   //A3测试
                case "pb":  //炮兵
                case "sk":  //山科
                    size.x = 4290;
                    size.y = 2860;
                    break;
                default:
                    Debug.LogWarning("未查询到外部sign配置，使用默认分辨率");
                    size.x = 4480;
                    size.y = 2880;
                    break;
            }
            return size;
        }
    }
}

