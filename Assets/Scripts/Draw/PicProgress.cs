using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace jxzt
{
    /// <summary>
    /// 照片处理进度管理器
    /// 调用外部Python脚本处理学生作业照片，管理处理进度
    /// 支持分步进度显示（相机准备、拍照、矫正等）
    /// </summary>
    public class PicProgress : MonoBehaviour
    {
        public static PicProgress instance;
        public string exePath;
        public string argument1;
        public string argument2;
        public string argument3;
        public string argument4;
        public string argument5;
        public string xueShengDaAnName;
        public string ocrPageCode;
        /// <summary>
        /// 0:相机准备，1：相机开始拍照，2：相机拍照完成，3：照片开始矫正，4：照片矫正完成
        /// </summary>
        public float picProgress;//照片处理进度

        // ========== 新增：异步执行相关 ==========
        /// <summary>
        /// 异步任务取消标记
        /// </summary>
        private CancellationTokenSource _asyncCts;
        /// <summary>
        /// 异步任务是否正在运行
        /// </summary>
        public bool IsAsyncRunning { get; private set; } = false;
        /// <summary>
        /// 异步执行完成回调（Action: 是否成功）
        /// </summary>
        private Action<bool> _asyncCallback;
        /// <summary>
        /// 异步执行进度回调（Action: 进度百分比字符串）
        /// </summary>
        private Action<string> _asyncProgressCallback;
        /// <summary>
        /// 上次输出行数（用于显示动画）
        /// </summary>
        private int _lastOutputCount = 0;

        private void Awake()
        {
            instance = this;
        }
        void Start()
        {
            exePath = Application.streamingAssetsPath + "/main.exe";
        }

        void OpenExe()
        {
            Process process = new Process();
            process.StartInfo.FileName = exePath;
            process.StartInfo.Arguments = $"{argument1} {argument2}";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            while (!process.StandardOutput.EndOfStream)
            {
                string output = process.StandardOutput.ReadLine();
                UnityEngine.Debug.Log("Output: " + output);
            }

            process.WaitForExit();
            process.Close();

        }

        // ========== 新增：异步非阻塞外部程序执行 ==========
        /// <summary>
        /// 异步执行外部程序（非阻塞）
        /// </summary>
        /// <param name="exePath">程序路径</param>
        /// <param name="arg1">参数1：原图路径</param>
        /// <param name="arg2">参数2：去题干不变粗</param>
        /// <param name="arg3">参数3：截取黑框带题干</param>
        /// <param name="arg4">参数4：去题干变粗</param>
        /// <param name="arg5">参数5：附加参数</param>
        /// <param name="onComplete">执行完成回调（bool: 是否成功）</param>
        /// <param name="onProgress">进度更新回调（string: 进度信息）</param>
        public void StartExternalProgramAsync(
            string exePath,
            string arg1,
            string arg2,
            string arg3,
            string arg4,
            string arg5,
            Action<bool> onComplete = null,
            Action<string> onProgress = null)
        {
            // 如果已有任务在运行，先取消
            if (IsAsyncRunning)
            {
                CancelAsyncTask();
            }

            _asyncCts = new CancellationTokenSource();
            _asyncCallback = onComplete;
            _asyncProgressCallback = onProgress;
            IsAsyncRunning = true;
            _lastOutputCount = 0;

            // 在线程池中执行，避免阻塞主线程
            // 使用数组捕获 success 值，避免闭包问题
            bool[] successHolder = new bool[1];
            Task.Run(() =>
            {
                bool success = false;
                try
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo(exePath, arg1 + " " + arg2 + " " + arg3 + " " + arg4 + " " + arg5);
                    startInfo.UseShellExecute = false;
                    startInfo.RedirectStandardOutput = true;
                    startInfo.CreateNoWindow = true;

                    using (Process process = new Process())
                    {
                        process.StartInfo = startInfo;
                        process.Start();

                        int lineCount = 0;
                        // 实时读取输出，同时检测是否取消
                        while (!process.HasExited || !process.StandardOutput.EndOfStream)
                        {
                            // 检查取消标记
                            if (_asyncCts.Token.IsCancellationRequested)
                            {
                                try { process.Kill(); } catch { }
                                break;
                            }

                            if (!process.StandardOutput.EndOfStream)
                            {
                                string output = process.StandardOutput.ReadLine();
                                if (!string.IsNullOrEmpty(output))
                                {
                                    lineCount++;
                                    // 每5行更新一次进度
                                    if (lineCount % 5 == 0 && _asyncProgressCallback != null)
                                    {
                                        string dots = "";
                                        for (int i = 0; i < (lineCount / 5) % 4; i++) dots += ".";
                                        string progressMsg = "处理中" + dots;
                                        UnityEngine.Debug.Log("[异步执行] " + progressMsg);
                                    }
                                }
                            }
                            else if (process.HasExited)
                            {
                                break;
                            }
                            else
                            {
                                // 没有输出时短暂休眠，避免CPU空转
                                Thread.Sleep(50);
                            }
                        }

                        process.WaitForExit();
                        successHolder[0] = process.ExitCode == 0;
                    }
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError("[异步执行异常] " + ex.Message);
                    successHolder[0] = false;
                }

                // 在主线程执行回调
                UnityEngine.Debug.Log("[异步执行] 完成，结果: " + (successHolder[0] ? "成功" : "失败"));
            }, _asyncCts.Token).ContinueWith(t =>
            {
                IsAsyncRunning = false;
                if (_asyncCallback != null)
                {
                    _asyncCallback(successHolder[0]);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// 取消正在运行的异步任务
        /// </summary>
        public void CancelAsyncTask()
        {
            if (_asyncCts != null && !_asyncCts.IsCancellationRequested)
            {
                _asyncCts.Cancel();
                UnityEngine.Debug.Log("[异步执行] 任务已取消");
            }
            IsAsyncRunning = false;
        }

        /// <summary>
        /// 异步版本：带协程包装器，兼容现有协程代码
        /// </summary>
        public IEnumerator ExecuteExternalProgramAsync_Coroutine(
            string exePath,
            string arg1,
            string arg2,
            string arg3,
            string arg4,
            string arg5)
        {
            bool completed = false;
            bool result = false;
            int dotCount = 0;

            StartExternalProgramAsync(exePath, arg1, arg2, arg3, arg4, arg5,
                (success) =>
                {
                    result = success;
                    completed = true;
                },
                (progress) =>
                {
                    // 进度更新可以在这里处理UI
                });

            // 等待完成或取消，同时显示加载动画
            while (!completed && !_asyncCts.Token.IsCancellationRequested)
            {
                yield return new WaitForSeconds(0.3f);
                dotCount = (dotCount + 1) % 4;
                string dots = new string('.', dotCount);
                // 可选：更新UI显示加载动画
            }

            yield return result;
        }

        /// <summary>
        /// arg1 = 原图路径
        /// arg2 = 去题干不变粗
        /// arg3 = 截取黑框带题干
        /// arg4 = 去题干变粗（用于去噪点判线段类型）
        /// </summary>
        public class ExecuteExternalProgram
        {
            public static void StartExternalProgram(string exePath, string arg1, string arg2, string arg3,string arg4, string arg5)
            {
                using (ScoringPerf.Scope("ExternalProgram.WaitForExit", $"exe={Path.GetFileName(exePath)};input={Path.GetFileName(arg1)}"))
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo(exePath, arg1 + " " + arg2 + " " + arg3 + " " + arg4 + " " + arg5);
                    startInfo.UseShellExecute = false;
                    Process process = Process.Start(startInfo);
                    process.WaitForExit();
                    process.Dispose();
                }
            }
        }
        // Update is called once per frame
        void Update()
        {
        }
    }
}

