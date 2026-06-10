using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;  //��Ҫ����������ʿռ䣬����DataReceivedEventArg 
using UnityEngine.UI;
using Newtonsoft.Json;

namespace jxzt
{
    public class JsonResponse
    {
        public Box[] boxs;

        public JsonResponse(Box[] boxs)
        {
            this.boxs = boxs;
        }
    }

    public class Box
    {
        /// <summary>
        /// 
        /// </summary>
        public int index;
        /// <summary>
        /// 
        /// </summary>
        public int[] box;

        public Box(int index, int[] box)
        {
            this.index = index;
            this.box = box;
        }
    }
    public class LoadPython : MonoBehaviour
    {
        string sArguments = @"/testdeploy.py";//������python���ļ�����
        public string fileName = "2024-03-10-05-48-39.png";
        public static List<List<PositionInt>> markPositionInts;
        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        public static void RunPythonScript(string args, string sArgName = @"/testdeploy.py")
        {
            Process p = new Process();
            //python�ű���·��
            string path = Application.streamingAssetsPath + sArgName;
            string sArguments = path + " " + args;


            //(ע�⣺�õĻ���Ҫ�����Լ���)û���价�������Ļ���������������дpython.exe�ľ���·��
            //(�õĻ���Ҫ�����Լ���)��������ˣ�ֱ��д"python.exe"����//C:\Users\Administrator\AppData\Local\Programs\Python\Python311 
            p.StartInfo.FileName = @"C:\Users\Admin\AppData\Local\Programs\Python\Python311\python.exe";//C:\Users\Admin\AppData\Local\Programs\Python\Python311   
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.Arguments = sArguments;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.OutputDataReceived += new DataReceivedEventHandler(Out_RecvData);
            p.Start();
            p.BeginOutputReadLine();


            p.WaitForExit();
        }

        static void Out_RecvData(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {

                JsonResponse response = JsonConvert.DeserializeObject<JsonResponse>(e.Data);


                List<List<PositionInt>> positionInts = new List<List<PositionInt>>();

                foreach (var item in response.boxs)
                {
                    List<PositionInt> positionInt = new List<PositionInt>();
                    positionInt.Add(new PositionInt(item.box[0], item.box[1]));
                    positionInt.Add(new PositionInt(item.box[0], item.box[3]));
                    positionInt.Add(new PositionInt(item.box[2], item.box[3]));
                    positionInt.Add(new PositionInt(item.box[2], item.box[1]));
                    positionInts.Add(positionInt);
                }
                markPositionInts = positionInts;
                UnityEngine.Debug.Log("markPositionInts" + markPositionInts.Count);

            }
        }


    }

}
