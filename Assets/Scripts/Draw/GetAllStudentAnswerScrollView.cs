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
    /// 获取所有学生答案滚动视图
    /// 展示指定题目的所有学生答案列表，支持分页浏览和筛选
    /// 用于教师端查看某道题的所有学生作答情况
    /// </summary>
    public class GetAllStudentAnswerScrollView : MonoBehaviour
    {
        public List<string> dataFileList;
        public GameObject picBtn_Obj;
        public int problemId;

        int showPageIconItemCountPerPage = 6;
        public Button LeftPage_Button;
        public Button RightPage_Button;
        public Button TurnThePage_Button;
        public TMP_Text PageCount_Text;
        public TMP_InputField inputThePage;

        private int currentPage = 0;
        private int totalPage = 0;
        private int PageIconListCount = 0;        
        public string areaResource;
        public Dictionary<int, List<int>> studentAnswerPicObjPageDic = new Dictionary<int, List<int>>();
        // Start is called before the first frame update 
        void Start()
        {

            // 设置初始当前页为 1
            currentPage = 1;
            studentAnswerPicObjPageDic.Clear();           

            #region 页数和翻页按钮的初始化
            if (!studentAnswerPicObjPageDic.ContainsKey(currentPage))
            {                
                GetAnswerDataByTitle(currentPage);               
            }
            
            // 最前頁，把左翻頁隱藏
            LeftPage_Button.gameObject.SetActive(false);           
            #endregion

            RightPage_Button.onClick.AddListener(() =>
            {
                // 最后一頁，不能右翻頁        
                if ((currentPage) == totalPage)
                {
                    return;
                }
                else
                {
                    // 向右翻頁 左翻頁若隱藏即可顯示
                    if (LeftPage_Button.gameObject.activeSelf == false)
                    {
                        LeftPage_Button.gameObject.SetActive(true);
                    }

                    currentPage = currentPage + 1;
                    if (currentPage == (totalPage))
                    {
                        // 把右翻頁隱藏
                        RightPage_Button.gameObject.SetActive(false);
                    }
                    //隐藏当前显示图片
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        if (transform.GetChild(i).gameObject.activeSelf)
                        {
                            transform.GetChild(i).gameObject.SetActive(false);
                        }

                    }
                    //加载图片
                    if (!studentAnswerPicObjPageDic.ContainsKey(currentPage))
                    {
                        GetAnswerDataByTitle(currentPage);
                    }
                    else
                    {
                        foreach (var item in studentAnswerPicObjPageDic[currentPage])
                        {
                            transform.GetChild(item).gameObject.SetActive(true);
                        }
                    }                    

                }
                //更新頁碼
                PageCount_Text.text = currentPage + "/" + totalPage;

            });


            LeftPage_Button.onClick.AddListener(() =>
            {
                // 最前一頁，不能左翻頁        
                if ((currentPage) == 1)
                {
                    return;
                }
                else
                {
                    // 向左翻頁 右翻頁若隱藏即可顯示
                    if (RightPage_Button.gameObject.activeSelf == false)
                    {
                        RightPage_Button.gameObject.SetActive(true);
                    }

                    currentPage = currentPage - 1;
                    // 最前頁，把左翻頁隱藏
                    if (currentPage == 1)
                    {

                        LeftPage_Button.gameObject.SetActive(false);
                    }
                    //隐藏当前显示图片
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        if (transform.GetChild(i).gameObject.activeSelf)
                        {
                            transform.GetChild(i).gameObject.SetActive(false);
                        }

                    }
                    //加载图片
                    if (!studentAnswerPicObjPageDic.ContainsKey(currentPage))
                    {
                        GetAnswerDataByTitle(currentPage);
                    }
                    else
                    {
                        foreach (var item in studentAnswerPicObjPageDic[currentPage])
                        {
                            transform.GetChild(item).gameObject.SetActive(true);
                        }
                    }


                }
                //更新頁碼
                PageCount_Text.text = currentPage + "/" + totalPage;

            });

            TurnThePage_Button.onClick.AddListener(() =>
            {
                currentPage = int.Parse(inputThePage.text);
                // 最后一頁，不能右翻頁        

                // 最前頁，把左翻頁隱藏
                if (currentPage == 1)
                {

                    LeftPage_Button.gameObject.SetActive(false);
                }
                else
                {
                    LeftPage_Button.gameObject.SetActive(true);
                }
                if (currentPage == totalPage)
                {

                    RightPage_Button.gameObject.SetActive(false);
                }
                else
                {
                    RightPage_Button.gameObject.SetActive(true);
                }

                // 隱藏所有，保障多餘的不會顯示
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (transform.GetChild(i).gameObject.activeSelf)
                    {
                        transform.GetChild(i).gameObject.SetActive(false);
                    }

                }
                //加载图片
                if (!studentAnswerPicObjPageDic.ContainsKey(currentPage))
                {

                    GetAnswerDataByTitle(currentPage);
                }
                else
                {
                    foreach (var item in studentAnswerPicObjPageDic[currentPage])
                    {
                        transform.GetChild(item).gameObject.SetActive(true);
                    }
                }

                //更新頁碼
                PageCount_Text.text = currentPage + "/" + totalPage;
            });
        }      

        void GetAnswerDataByTitle(int currentPage = 0)
        {
            string page = currentPage.ToString();
            string maxSize = showPageIconItemCountPerPage.ToString();
            string problem = problemId.ToString();
            string fullUrl = $"{Config.GetAllAnswerByProblemInfo}?{"page"}={page}&{"maxSize"}={maxSize}&{"problem"}={problem}";
            WebManager.Instance.GetStringFunc(fullUrl, delegate (string s)
            {
                Debug.Log(s);
                if (s.Length == 0)
                {
                    Debug.Log("获取答案失败");
                    return;
                }
                ReturnAllStudentAnswerDataByTitle rs = JsonConvert.DeserializeObject<ReturnAllStudentAnswerDataByTitle>(s);
                switch (rs.code)
                {
                    case 200:
                        Debug.Log("获取成功：" + rs.message);
                        PageIconListCount = rs.sum;
                        totalPage = (PageIconListCount / showPageIconItemCountPerPage) + ((PageIconListCount % showPageIconItemCountPerPage) == 0 ? 0 : 1);
                        List<int> studentAnswerPicObjLists = new List<int>();
                        int picbtnNum = 0;
                        for (int i = 0; i < rs.data.Length; i++)
                        {
                            CreatePicBtn(rs.data, i);
                            picbtnNum = transform.childCount - 1;
                            studentAnswerPicObjLists.Add(picbtnNum);
                        }
                        studentAnswerPicObjPageDic.Add(currentPage, studentAnswerPicObjLists);
                        //更新頁碼
                        PageCount_Text.text = currentPage + "/" + totalPage;
                        // 只有一页，右翻页也隐藏
                        if (totalPage == 1)
                        {
                            RightPage_Button.gameObject.SetActive(false);
                        }
                        break;
                    case 400:
                        if (Alert.Instance != null)
                        {
                            Alert.Instance.ShowTips(rs.message);
                        }
                        Debug.Log("获取失败：" + rs.message);
                        break;
                    case 404:
                        TeacherMainManager.instance.ErrorTipsClear("登录失效,请返回登陆界面重新登录", true, true);
                        Debug.Log("登录失效：" + rs.message);
                        break;
                }


            });
        }

        private void CreatePicBtn(StudentAnswerDataByTitle[] data, int i)
        {
            GameObject btn_obj = Instantiate(picBtn_Obj, transform);
            btn_obj.transform.GetChild(0).GetComponent<TMP_Text>().text = "【 " + data[i].userclasses + data[i].username + data[i].userstudentid + " 】";
            btn_obj.GetComponent<GetAllAnswerByTitleSaveTexture>().GetBackPic(data[i].PersonalAnswer);
            btn_obj.GetComponent<GetAllAnswerByTitleSaveTexture>().pptid = areaResource;
            btn_obj.GetComponent<GetAllAnswerByTitleSaveTexture>().WrongPoint = data[i].WrongPoint;
            btn_obj.GetComponent<GetAllAnswerByTitleSaveTexture>().answerID = data[i].id;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

