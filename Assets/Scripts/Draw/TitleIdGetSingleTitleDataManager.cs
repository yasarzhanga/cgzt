using jxzt;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleIdGetSingleTitleDataManager : MonoBehaviour
{
    /// <summary>
    /// 题目ID获取单个题目数据管理器
    /// 从配置文件读取题目编号列表，下拉选择后获取对应题目的详细信息
    /// 用于教师端按题号查询题目数据
    /// </summary>
    public List<string> dataFileList;
    public TMP_Dropdown drapdown;
    List<TMP_Dropdown.OptionData> listOptions = new List<TMP_Dropdown.OptionData>();
    // 路径到你的txt文件
    private string fileName = "tihaoyema.txt";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private void OnEnable()
    {
        ReadConfigFile(Application.streamingAssetsPath + "/TeacherAnswer/" + fileName);

        drapdown.onValueChanged.RemoveAllListeners();
        drapdown.onValueChanged.AddListener((int n) =>
        {
            TeacherMainManager.instance.titleIdNums.Clear();
            TeacherMainManager.instance.studentAnswerQRnum = -1;
            TeacherMainManager.instance.titleIdNums.Add(listOptions[n].text);
            Debug.Log("手动题号选择" + listOptions[n].text);

            //开启协程
            StartCoroutine(TeacherMainManager.instance.AegisAnimation(3));
            
            

        });
    }

    public void ReadConfigFile(string _path)
    {
        // 检查文件是否存在
        if (File.Exists(_path))
        {
            // 读取所有行
            string[] lines = File.ReadAllLines(_path);
            drapdown.ClearOptions();
            listOptions.Clear();
            listOptions.Add(new TMP_Dropdown.OptionData("请选择---"));
            // 遍历每一行
            foreach (string line in lines)
            {
                listOptions.Add(new TMP_Dropdown.OptionData(line));
                Debug.Log(line); // 打印每一行内容到控制台
            }
            drapdown.AddOptions(listOptions);
        }
        else
        {
            Debug.LogError("File not found at path: " + _path);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
