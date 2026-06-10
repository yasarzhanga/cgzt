using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
namespace jxzt
{
    /// <summary>
    /// 点击打开项目/外部程序
    /// 用于调用外部程序（如Unity编辑器）打开指定项目
    /// 支持传入参数，用于快速启动相关工具
    /// </summary>
    public class ClickOpenProject : MonoBehaviour
    {
        public static ClickOpenProject instance;
        public Button clickBtn;
        public string exePath;
        public string argument1;
        public int picProgress;//照片处理进度

        private void Awake()
        {
            instance = this;
        }
        void Start()
        {

            exePath = @"D:\InstallUnity\2019.4.0f1\Editor\Unity.exe";
            argument1 = @"D:\kfpt\VirtualSimulationKSKFPT7.5Dll2019EVRC\VirtualSimulationKSKFPT7.5Dll2019EVRC";

            clickBtn.onClick.AddListener(() =>
            {
                OpenProjectFun();


            });
        }


        public void OpenProjectFun()
        {
            var bytes = File.ReadAllBytes(Path.Combine(Application.streamingAssetsPath, "FilePath"));
            string savejson = System.Text.Encoding.Default.GetString(bytes);
            JsonResponse response = JsonConvert.DeserializeObject<JsonResponse>(savejson);
            if (!string.IsNullOrEmpty(response.exePath) && !string.IsNullOrEmpty(response.projectPath))
            {
                ExecuteExternalProgram.StartExternalProgram(response.exePath, response.projectPath);
            }


        }
        public void ExportJson()
        {
            JsonResponse response = new JsonResponse();
            response.exePath = exePath;
            response.projectPath = argument1;

            FileStream fs = File.Create(Path.Combine(Application.streamingAssetsPath, "FilePath"));
            using (fs)
            {
                var bytes = System.Text.Encoding.Default.GetBytes(JsonConvert.SerializeObject(response));
                fs.Write(bytes, 0, bytes.Length);
            }
        }
        void OpenExe()
        {
            Process process = new Process();
            process.StartInfo.FileName = exePath;
            process.StartInfo.Arguments = $"{argument1}";
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
        public class ExecuteExternalProgram
        {
            public static void StartExternalProgram(string exePath, string arg1)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(exePath, $"-projectPath " + arg1);


                startInfo.UseShellExecute = true;
                Process process = Process.Start(startInfo);
                Application.Quit();
            }
        }
        public class JsonResponse
        {
            public string exePath;
            public string projectPath;
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}

