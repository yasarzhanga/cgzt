using jxzt;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CorrectWrongReasonManager : MonoBehaviour
{
    /// <summary>
    /// 判分错误原因管理器
    /// 管理学生答案的错误原因下拉选择，列出所有可标记的错误类型
    /// 配合AnswerCheck判分系统，记录每条图线的错误原因
    /// </summary>
    public List<string> dataFileList;
    public TMP_Dropdown studentAnswerserrorPointDrapdown;
    List<TMP_Dropdown.OptionData> studentAnswerserrorPointOptions = new List<TMP_Dropdown.OptionData>();    
    string initValue;
    void Start()
    {        
        dataFileList.Clear();
        studentAnswerserrorPointDrapdown.ClearOptions();
        studentAnswerserrorPointOptions.Clear();
        dataFileList.Add("--请选择--");
        studentAnswerserrorPointOptions.Add(new TMP_Dropdown.OptionData("--请选择--"));
        foreach (ErrorReson errorPoint in Enum.GetValues(typeof(ErrorReson)))
        {
            dataFileList.Add(errorPoint.ToString());
            studentAnswerserrorPointOptions.Add(new TMP_Dropdown.OptionData(errorPoint.ToString()));            
        }
        studentAnswerserrorPointDrapdown.AddOptions(studentAnswerserrorPointOptions);
        studentAnswerserrorPointDrapdown.onValueChanged.RemoveAllListeners();
        studentAnswerserrorPointDrapdown.onValueChanged.AddListener((int n) =>
        {
            TeacherMainManager.instance.isCorrected = true;
            transform.parent.GetChild(1).GetComponent<TMP_Text>().text = dataFileList[n];                        


        });

    }

    // Update is called once per frame
    void Update()
    {

    }
}
