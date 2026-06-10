using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace jxzt
{
    /// <summary>
    /// 按题目获取学生答案缩略图按钮
    /// 挂载在学生答案缩略图按钮上，存储学生答案相关信息和错误点
    /// 点击时加载学生答案图片到画板，供教师批改
    /// </summary>
    public class GetAllAnswerByTitleSaveTexture : MonoBehaviour
    {
        public Texture2D texture;
        public int answerID;//答案ID
        public string username;//姓名
        public string userclasses;//班级
        public string userstudentid;//学号
        public string PersonalAnswer;//个人答案图片url
        public string WrongPoint;//错误点
        public string pptid;
        // Start is called before the first frame update
        void Start()
        {            
            if (transform.GetComponent<Button>() != null)
            {
                transform.GetComponent<Button>().onClick.AddListener(() =>
                {
                    if (!string.IsNullOrEmpty(answerID.ToString()))
                    {
                        GameObject.Find("PaintBoard").transform.GetComponent<RawImage>().texture = texture;
                        GameObject.Find("PaintBoard").transform.GetComponent<TeacherMainManager>().answerID = answerID;
                    }
                    transform.parent.parent.parent.gameObject.SetActive(!transform.parent.parent.parent.gameObject.activeSelf);
                });
            }
           
        }
        public void GetBackPic(string titlePicUrl)
        {            
            WebManager.Instance.GetTextureFunc(titlePicUrl, delegate (Texture2D texture2D)
            {
                texture = texture2D;
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                transform.GetComponent<Image>().sprite = sprite;
            });
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}

