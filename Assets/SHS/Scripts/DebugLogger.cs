using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// 全局日志拦截器：将所有 Debug.Log/LogWarning/LogError 同时写入 XML 文件，
/// 方便事后分析问题（尤其是运行时无法查看 Console 的场景）。
/// 
/// 使用方式：在任意 Start() 中调用 DebugLogger.Initialize() 即可。
/// 输出文件路径：Application.streamingAssetsPath/DebugLogs/yyyy-MM-dd_HH-mm-ss.xml
/// </summary>
public class DebugLogger : MonoBehaviour
{
    private static DebugLogger _instance;
    private static string _logFilePath;
    private static readonly object _lock = new object();
    private static StringBuilder _pendingXml = new StringBuilder();
    private static int _pendingLogCount = 0;
    private static bool _logClosed = false;

    // 控制是否启用（发布时可关闭）
    private static bool _enabled = true;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        DebugLogger.Initialize();
    }

    /// <summary>
    /// 初始化全局日志拦截。建议在 Bootstrap 或 MainManager 的 Awake 中调用一次。
    /// </summary>
    /// <param name="enabled">设为 false 可禁用文件输出（但仍会 Debug.Log）</param>
    public static void Initialize(bool enabled = true)
    {
        _enabled = enabled;
        if (!enabled) return;

        // 找到或创建 GameObject
        if (_instance == null)
        {
            var go = new GameObject("[DebugLogger]");
            _instance = go.AddComponent<DebugLogger>();
            DontDestroyOnLoad(go);
        }

        // 构造文件路径（输出到 Assets/StreamingAssets/DebugLogs/）
        string logDir = Path.Combine(Application.streamingAssetsPath, "DebugLogs");
        Directory.CreateDirectory(logDir);
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        _logFilePath = Path.Combine(logDir, $"{timestamp}.xml");

        var header = new StringBuilder();
        header.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        header.AppendLine($"<!-- Debug Log created at {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} -->");
        header.AppendLine($"<!-- Unity version: {Application.unityVersion} -->");
        header.AppendLine($"<!-- Product: {Application.productName} -->");
        header.AppendLine("<LogEntries>");
        File.WriteAllText(_logFilePath, header.ToString(), Encoding.UTF8);
        _pendingXml.Clear();
        _pendingLogCount = 0;
        _logClosed = false;

        // 注册拦截回调
        Application.logMessageReceivedThreaded -= OnLogReceived;
        Application.logMessageReceivedThreaded += OnLogReceived;

        Debug.Log($"[DebugLogger] 日志文件已创建: {_logFilePath}");
    }

    private static void OnLogReceived(string logString, string stackTrace, LogType type)
    {
        if (!_enabled || _logClosed) return;

        lock (_lock)
        {
            string time = DateTime.Now.ToString("HH:mm:ss.fff");
            string escapedMsg = EscapeXml(logString);
            string escapedStack = EscapeXml(stackTrace);
            string typeStr = type.ToString();

            _pendingXml.AppendLine($"  <Log>");
            _pendingXml.AppendLine($"    <Time>{time}</Time>");
            _pendingXml.AppendLine($"    <Type>{typeStr}</Type>");
            _pendingXml.AppendLine($"    <Message>{escapedMsg}</Message>");
            // stackTrace 仅对 Error/Warning 记录，避免文件过大
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Warning)
            {
                _pendingXml.AppendLine($"    <StackTrace>{escapedStack}</StackTrace>");
            }
            _pendingXml.AppendLine($"  </Log>");

            _pendingLogCount++;
            // 每 20 条 Flush 一次，减少内存积压；避免每条日志都 ToString 全量扫描。
            if (_pendingLogCount >= 20)
            {
                FlushToFile();
            }
        }
    }

    /// <summary>
    /// 手动刷新缓冲区到磁盘（OnApplicationQuit 自动调用）。
    /// </summary>
    public static void FlushToFile()
    {
        FlushToFile(false);
    }

    private static void FlushToFile(bool closeLog)
    {
        if (string.IsNullOrEmpty(_logFilePath)) return;
        lock (_lock)
        {
            try
            {
                if (_pendingXml.Length == 0 && (!closeLog || _logClosed))
                {
                    return;
                }

                var content = new StringBuilder();
                content.Append(_pendingXml);
                if (closeLog && !_logClosed)
                {
                    content.AppendLine("</LogEntries>");
                    _logClosed = true;
                }

                File.AppendAllText(_logFilePath, content.ToString(), Encoding.UTF8);
                _pendingXml.Clear();
                _pendingLogCount = 0;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[DebugLogger] 写入文件失败: {ex.Message}");
            }
        }
    }

    private void OnApplicationQuit()
    {
        FlushToFile(true);
    }

    private void OnDestroy()
    {
        Application.logMessageReceivedThreaded -= OnLogReceived;
    }

    private static string EscapeXml(string s)
    {
        if (string.IsNullOrEmpty(s)) return string.Empty;
        return s.Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;");
    }

}
