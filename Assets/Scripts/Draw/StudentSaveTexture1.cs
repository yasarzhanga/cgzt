using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace jxzt
{
    /// <summary>
    /// 学生答案缩略图按钮（扩展版）
    /// 功能同 StudentSaveTexture，用于补录界面
    /// 勾选时将该答案加入待删除/补录列表
    /// </summary>
    public class StudentSaveTexture1 : MonoBehaviour
    {

        public Texture2D texture;
        public string studentAnswerName;
        public string studentName;
        public string studentNumber;

        private void Awake()
        {
            if (transform.GetComponent<Toggle>() != null)
            {
                transform.GetComponent<Toggle>().onValueChanged.AddListener((bool ison) =>
                {
                    if (!string.IsNullOrEmpty(studentAnswerName))
                    {
                        if (ison)
                        {
                            transform.parent.GetChild(0).gameObject.SetActive(true);
                            Debug.Log("导入学生答案" + GameObject.Find("PaintBoard").transform.GetComponent<TeacherMainManager>().StudentAnswerFileNames);
                            if (!TeacherMainManager.instance.Dic_NoScoredStudentAnswerData.ContainsKey(studentAnswerName))
                            {
                                string currentPageStr = transform.parent.parent.transform.GetComponent<StudentImportAnswerScrollView1>().NoScoredStudentAnswerName_Input.text + "-" + transform.parent.parent.transform.GetComponent<StudentImportAnswerScrollView1>().NoScoredStudentAnswerNumber_Input.text;
                                TeacherMainManager.instance.Dic_NoScoredStudentAnswerData.Add(studentAnswerName, currentPageStr);
                                PendingXueShengZuoYeStorage.Save(TeacherMainManager.instance.Dic_NoScoredStudentAnswerData);
                                Debug.Log("Dic_NoScoredStudentAnswerData saved, 补录count=" + TeacherMainManager.instance.Dic_NoScoredStudentAnswerData.Count);
                                
                            }


                        }
                        else
                        {
                            transform.parent.GetChild(0).gameObject.SetActive(false);
                            if (TeacherMainManager.instance.Dic_NoScoredStudentAnswerData.ContainsKey(studentAnswerName))
                            {                                
                                TeacherMainManager.instance.Dic_NoScoredStudentAnswerData.Remove(studentAnswerName);
                                PendingXueShengZuoYeStorage.Save(TeacherMainManager.instance.Dic_NoScoredStudentAnswerData);
                                Debug.Log("Dic_NoScoredStudentAnswerData saved,删除学生答案 count=" + TeacherMainManager.instance.Dic_NoScoredStudentAnswerData.Count);                                                              
                            }

                        }

                    }
                });
            }
        }
        // Start is called before the first frame update

        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

