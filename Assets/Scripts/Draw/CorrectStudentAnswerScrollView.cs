using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static jxzt.Info;

namespace jxzt
{
    /// <summary>
    /// 批改学生答案滚动视图管理器
    /// 用于教师端展示学生答案列表，支持按章节/题目筛选、分页浏览学生提交的答案
    /// 配合 TeacherMainManager 实现答案批改流程
    /// </summary>
    public class CorrectStudentAnswerScrollView : MonoBehaviour
    {
        public List<string> dataFileList;

        public TMP_Dropdown chapterDrapdown;        
        public TMP_Dropdown chapterChildDrapdown;
        public TMP_Dropdown studentAnswersDrapdown;
        List<TMP_Dropdown.OptionData> chapterlistOptions = new List<TMP_Dropdown.OptionData>();        
        List<TMP_Dropdown.OptionData> chapterOptions = new List<TMP_Dropdown.OptionData>();
        List<TMP_Dropdown.OptionData> studentAnswersOptions = new List<TMP_Dropdown.OptionData>();                

        int showPageIconItemCountPerPage = 6;        
        public Button ConfirmSelection_Button;        
        public TMP_InputField inputThePage;

        private int currentPage = 0;
        private int totalPage = 0;
        private int PageIconListCount = 0;           
        public Dictionary<int, List<int>> studentAnswerPicObjPageDic = new Dictionary<int, List<int>>();
        Dictionary<string, List<string>> Dic_TitleIds = new Dictionary<string, List<string>>();
        Dictionary<string, titleIdData> Dic_TitleIdAndAreaResources = new Dictionary<string, titleIdData>();
        List<StudentAnswerDataByTitle> List_StudentAnswerDatas = new List<StudentAnswerDataByTitle>();

        // Start is called before the first frame update 
        void Start()
        {
            GetTitleID();
        }
        public titleIdData[] allTitleIdData;//所有题目ID
        void GetTitleID()
        {
            Debug.Log("修改答案");
            chapterDrapdown.ClearOptions();
            chapterlistOptions.Clear();
            Dic_TitleIds.Clear();
            Dic_TitleIdAndAreaResources.Clear();
            WebManager.Instance.GetStringFunc(Config.GetAllProblemID, delegate (string s)
            {
                Debug.Log(s);
                if (s.Length == 0)
                {
                    Debug.Log("获取题目ID失败");
                    return;
                }
                else
                {
                    IList<titleIdData> rs = new List<titleIdData>();
                    rs = JsonConvert.DeserializeObject<IList<titleIdData>>(s);
                    var NumRegex = new Regex(@"\d");
                    Debug.Log("获取成功：" + rs);
                    foreach (var item in rs)
                    {
                        string input = item.title_id;
                        string[] parts = input.Split('-');
                        if (NumRegex.IsMatch(parts[0]) && NumRegex.IsMatch(parts[1]))
                        {
                            if (!Dic_TitleIds.ContainsKey(parts[0]))
                            {
                                List<string> chapter = new List<string>();                                                                                                
                                chapter.Add(item.title_id);
                                Dic_TitleIds.Add(parts[0], chapter);                                                                
                                Dic_TitleIdAndAreaResources.Add(item.title_id, item);
                            }
                            else
                            {                                                                
                                Dic_TitleIds[parts[0]].Add(item.title_id);
                                Dic_TitleIdAndAreaResources.Add(item.title_id, item);
                            }
                        }
                    }
                    foreach (var item in Dic_TitleIds)
                    {
                        if (!chapterlistOptions.Contains(new TMP_Dropdown.OptionData(item.Key)))
                        {
                            chapterlistOptions.Add(new TMP_Dropdown.OptionData(item.Key));
                        }
                    }
                    chapterDrapdown.AddOptions(chapterlistOptions);
                    //初始化
                    #region 初始化
                    chapterChildDrapdown.ClearOptions();
                    chapterOptions.Clear();
                    foreach (var item in Dic_TitleIds[chapterlistOptions[chapterDrapdown.value].text])
                    {
                        if (!chapterOptions.Contains(new TMP_Dropdown.OptionData(item)))
                        {
                            chapterOptions.Add(new TMP_Dropdown.OptionData(item));
                        }
                    }
                    
                    chapterChildDrapdown.AddOptions(chapterOptions);
                    GetAnswerDataByTitle(Dic_TitleIdAndAreaResources[chapterOptions[chapterChildDrapdown.value].text].id, Dic_TitleIdAndAreaResources[chapterOptions[chapterChildDrapdown.value].text].title_id);
                    #endregion


                    chapterDrapdown.onValueChanged.AddListener((int n) =>
                    {
                        chapterChildDrapdown.ClearOptions();
                        chapterOptions.Clear();
                        foreach (var item in Dic_TitleIds[chapterlistOptions[n].text])
                        {
                            if (!chapterOptions.Contains(new TMP_Dropdown.OptionData(item)))
                            {
                                chapterOptions.Add(new TMP_Dropdown.OptionData(item));
                            }
                        }
                        chapterChildDrapdown.AddOptions(chapterOptions);
                    });
                    chapterChildDrapdown.onValueChanged.AddListener((int n) =>
                    {

                        Debug.Log("获取题目" + chapterOptions[n].text + "下的所有答案");
                        GetAnswerDataByTitle(Dic_TitleIdAndAreaResources[chapterOptions[n].text].id, Dic_TitleIdAndAreaResources[chapterOptions[n].text].title_id);

                    });
                }                             

            });
        }
        void GetAnswerDataByTitle(int problemId,string title_id)
        {           
            string fullUrl = $"{Config.GetAllAnswerByProblemInfo}?{"problem"}={problemId}";
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
                        
                        studentAnswersDrapdown.ClearOptions();
                        studentAnswersOptions.Clear();
                        List_StudentAnswerDatas.Clear();
                        studentAnswersOptions.Add(new TMP_Dropdown.OptionData("--请选择--"));
                        foreach (var item in rs.data)
                        {
                            List_StudentAnswerDatas.Add(item);
                            studentAnswersOptions.Add(new TMP_Dropdown.OptionData(item.userstudentid + item.username));                            
                        }                        
                        studentAnswersDrapdown.AddOptions(studentAnswersOptions);
                        


                        transform.GetComponent<SearchBar>().studentAnswersOptions = studentAnswersOptions;
                        transform.GetComponent<SearchBar>().Init();
                        studentAnswersDrapdown.Show();


                        studentAnswersDrapdown.onValueChanged.RemoveAllListeners();
                        studentAnswersDrapdown.onValueChanged.AddListener((int n) =>
                        {
                            Debug.Log("点击学生姓名"+"n"+n+ "List_StudentAnswerDatas[n+1]"+ List_StudentAnswerDatas[n-1]+ "List_StudentAnswerDatas.Count"+ List_StudentAnswerDatas.Count);
                            Debug.Log(studentAnswersDrapdown.transform.Find("Label").GetComponent<TMP_Text>().text);
                            transform.GetComponent<SearchBar>().inputField.text = studentAnswersDrapdown.transform.Find("Label").GetComponent<TMP_Text>().text;
                            transform.GetComponent<SearchBar>().HideInputField();

                            ConfirmSelection_Button.onClick.RemoveAllListeners();
                            ConfirmSelection_Button.onClick.AddListener(()=> 
                            {
                                                                
                                GameObject.Find("PaintBoard").transform.GetComponent<TeacherMainManager>().CorrectStudentAnswerMethod(title_id, List_StudentAnswerDatas[n-1]);
                                
                            });
                          

                        });
                        break;
                    case 400:
                        studentAnswersDrapdown.ClearOptions();
                        studentAnswersOptions.Clear();
                        Debug.Log("获取失败：" + rs.message);
                        break;
                    case 404:
                        TeacherMainManager.instance.ErrorTipsClear("登录失效,请返回登陆界面重新登录", true, true);
                        Debug.Log("登录失效：" + rs.message);
                        break;
                }


            });
        }
        void GetTitleData(int currentPage = 0)
        {
            string page = currentPage.ToString();
            string maxSize = showPageIconItemCountPerPage.ToString();
            string fullUrl = $"{Config.GetProblemPartList}?{"page"}={page}&{"maxSize"}={maxSize}";
            WebManager.Instance.GetStringFunc(fullUrl, delegate (string s)
            {
                Debug.Log(s);
                if (s.Length == 0)
                {
                    Debug.Log("获取题目失败");
                    return;
                }
                ReturnAllTitleDataTeacher rs = JsonConvert.DeserializeObject<ReturnAllTitleDataTeacher>(s);
               
                switch (rs.code)
                {
                    case 200:                        
                        Debug.Log("获取成功：" + rs.message);
                        PageIconListCount = rs.sum;
                        totalPage = (PageIconListCount / showPageIconItemCountPerPage) + ((PageIconListCount % showPageIconItemCountPerPage) == 0 ? 0 : 1);
                        List<int> studentAnswerPicObjLists = new List<int>();
                        int picbtnNum = 0;
                        for (int i = 0; i < rs.allTitleData.Length; i++)
                        {
                            CreatePicBtn(rs.allTitleData, i);
                            picbtnNum = transform.childCount - 1;
                            studentAnswerPicObjLists.Add(picbtnNum);
                        }
                        studentAnswerPicObjPageDic.Add(currentPage, studentAnswerPicObjLists);
                        if (totalPage == 1)
                        {
                        }
                        break;
                    case 400:
                        if (Alert.Instance != null)
                        {
                            Alert.Instance.ShowTips(rs.message);
                        }
                        Debug.Log("获取失败：" + rs.message);
                        break;
                }


            });
        }

        private void CreatePicBtn(titleData[] data, int i)
        {
        }
        public Texture2D GetBackPic(string titlePicUrl)
        {
            Texture2D texture = null;
            WebManager.Instance.GetTextureFunc(titlePicUrl, delegate (Texture2D texture2D)
            {
               texture =  texture2D;
                
            });
            return texture;
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}

