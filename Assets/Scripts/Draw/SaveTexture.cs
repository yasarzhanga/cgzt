using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace jxzt
{
    /// <summary>
    /// 保存纹理（教师答案缩略图）
    /// 挂载在教师答案缩略图按钮上，点击时设置当前选中的教师答案
    /// 用于题目导入时选择标准答案
    /// </summary>
    public class SaveTexture : MonoBehaviour
    {
        public Texture2D texture;
        public string teacherAnswerName;
        // Start is called before the first frame update
        void Start()
        {            
            transform.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (!string.IsNullOrEmpty(teacherAnswerName))
                {
                    GameObject.Find("PaintBoard").transform.GetComponent<TeacherMainManager>().TeacherAnswerFileName = teacherAnswerName;               //StudentMainManager 
                }
                GameObject.Find("PaintBoard").transform.GetComponent<RawImage>().texture = texture;
                transform.parent.parent.parent.parent.GetChild(1).gameObject.SetActive(!transform.parent.parent.parent.parent.GetChild(1).gameObject.activeSelf);
            });
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

