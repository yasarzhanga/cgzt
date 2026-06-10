using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace jxzt
{
    /// <summary>
    /// 学生答案缩略图按钮（Toggle版本）
    /// 挂载在学生答案缩略图Toggle上，存储学生答案相关信息
    /// 勾选时将该答案加入待处理列表
    /// </summary>
    public class StudentSaveTexture : MonoBehaviour
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
                            GameObject.Find("PaintBoard").transform.GetComponent<TeacherMainManager>().StudentAnswerFileNames.Add(studentAnswerName);
                            Texture2D texture1 = new Texture2D(texture.width, texture.height, texture.format, true);
                            GameObject.Find("PaintBoard").transform.GetComponent<TeacherMainManager>().Dic_StudentAnswerTextures.Add(studentAnswerName, texture1);
                            Debug.Log("导入学生答案" + GameObject.Find("PaintBoard").transform.GetComponent<TeacherMainManager>().StudentAnswerFileNames);


                        }
                        else
                        {
                            transform.parent.GetChild(0).gameObject.SetActive(false);
                            GameObject.Find("PaintBoard").transform.GetComponent<TeacherMainManager>().StudentAnswerFileNames.Remove(studentAnswerName);
                            GameObject.Find("PaintBoard").transform.GetComponent<TeacherMainManager>().Dic_StudentAnswerTextures.Remove(studentAnswerName);
                            Debug.Log("删除学生答案" + GameObject.Find("PaintBoard").transform.GetComponent<TeacherMainManager>().StudentAnswerFileNames);

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

