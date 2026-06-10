using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static jxzt.Info;

namespace jxzt
{
    /// <summary>
    /// 获取所有题目缩略图按钮
    /// 挂载在题目缩略图按钮上，存储题目纹理和ID信息
    /// 点击时切换当前选中的题目，并加载对应的标准答案
    /// </summary>
    public class GetAllTiMuSaveTexture : MonoBehaviour
    {
        public Texture2D texture;
        public string teacherAnswerName; 
        public string studentAnswerName;
        public int problemId;
        public string title_id;
        public string areaResource;
        int showPageIconItemCountPerPage = 6;
        public Button LeftPage_Button;
        public Button RightPage_Button;
        public TMP_Text PageCount_Text;

        private int currentPage = 0;
        private int totalPage = 0;
        private int PageIconListCount = 0;
        List<int> loadPageNumLists = new List<int>();
        public GameObject picBtn_Obj;
        private GameObject correctTeacherAnswer_Obj;
        // Start is called before the first frame update
        void Start()
        {           

            if (transform.GetComponent<Button>() != null)
            {
                transform.GetComponent<Button>().onClick.AddListener(() =>
                {
                    if (!string.IsNullOrEmpty(problemId.ToString()))
                    {
                        correctTeacherAnswer_Obj = GameObject.Find("correctTeacherAnswer_Btn").gameObject;                        
                        correctTeacherAnswer_Obj.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<GetAllStudentAnswerScrollView>().problemId = problemId;
                        correctTeacherAnswer_Obj.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<GetAllStudentAnswerScrollView>().areaResource = areaResource;
                        correctTeacherAnswer_Obj.transform.GetChild(1).gameObject.SetActive(true);



                        correctTeacherAnswer_Obj.transform.GetChild(0).gameObject.SetActive(false);            
                    }                    
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

