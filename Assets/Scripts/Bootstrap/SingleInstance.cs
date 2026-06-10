using System;
using System.Threading;
using UnityEngine;

namespace jxzt
{
    /// <summary>
    /// 单实例强制检查
    /// 在场景加载前通过互斥体检查是否已有实例运行
    /// 仅在 Windows 打包后生效，防止重复启动应用
    /// </summary>
    internal static class SingleInstance
    {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        private static Mutex _mutex;
        private static bool _hasMutex;
        // Use Global\ prefix to ensure cross-session uniqueness on Windows
        private const string MutexName = @"Global\KemIng365_SingleInstance_GradingApp";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Enforce()
        {
            try
            {
                _mutex = new Mutex(initiallyOwned: true, name: MutexName, createdNew: out bool createdNew);
                _hasMutex = createdNew;
                if (!_hasMutex)
                {
                    Debug.LogWarning("[SingleInstance] Another instance detected. Exiting.");
                    Application.Quit();
                    try { System.Diagnostics.Process.GetCurrentProcess().Kill(); } catch { }
                    return;
                }

                Application.quitting += Release;
            }
            catch (Exception ex)
            {
                Debug.LogError("[SingleInstance] Failed to create mutex: " + ex.Message);
            }
        }

        private static void Release()
        {
            try
            {
                if (_hasMutex && _mutex != null)
                {
                    _mutex.ReleaseMutex();
                    _mutex.Dispose();
                }
            }
            catch { }
            finally
            {
                _mutex = null;
                _hasMutex = false;
            }
        }
#endif
    }
}
