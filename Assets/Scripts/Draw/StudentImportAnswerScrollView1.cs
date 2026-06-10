using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static jxzt.Info;
using static jxzt.Login;
namespace jxzt
{
    /// <summary>
    /// 学生答案导入滚动视图（扩展版）
    /// 功能同 StudentImportAnswerScrollView，用于补录界面展示未识别的学生答案
    /// 支持分页浏览、手动输入姓名学号
    /// </summary>
    public class StudentImportAnswerScrollView1 : MonoBehaviour
    {

        public List<string> dataFileList;
        public GameObject picBtn_Obj;
        public Toggle CheckAllToggle;

        int showPageIconItemCountPerPage = 1;
        public Button LeftPage_Button;
        public Button RightPage_Button;
        public TMP_Text PageCount_Text;

        public int currentPage = 0;
        public int totalPage = 0;
        public int PageIconListCount = 0;
        public List<int> loadPageNumLists = new List<int>();
        public TMP_InputField NoScoredStudentAnswerName_Input;
        public TMP_InputField NoScoredStudentAnswerNumber_Input;
        public string NoScoredStudentData = "";        
        public Button OK_Button;
        [SerializeField]
        private bool hideThumbnails = true;

        private void Awake()
        {
            dataFileList = new List<string>();
        }

        private void OnEnable()
        {
            // 弹出补录界面时暂停判分流程，待用户补录完再点击“判分”继续
            if (TeacherMainManager.instance != null)
            {
                TeacherMainManager.instance.pauseScoringFlow = true;
            }

            NoScoredStudentAnswerName_Input.text = "";
            NoScoredStudentAnswerNumber_Input.text = "";
            dataFileList = GameObject.Find("PaintBoard").transform.GetComponent<TeacherMainManager>().NotScoredStudentAnswerFileList;
            currentPage = 1;
            PageIconListCount = dataFileList.Count;
            totalPage = (PageIconListCount / showPageIconItemCountPerPage) + ((PageIconListCount % showPageIconItemCountPerPage) == 0 ? 0 : 1);
            Debug.Log("导入学生答案总页数PageIconListCount" + PageIconListCount);
            Debug.Log("导入学生答案总页数totalPage" + totalPage);

            // 统一在补录界面展示所有待补录图片，并按原有逻辑分页
            // 如果之前通过 Inspector 勾选了 hideThumbnails，则这里关闭该模式，走分页展示逻辑
            if (hideThumbnails)
            {
                hideThumbnails = false;
            }

            // 以下为显示缩略图的旧逻辑（分页展示所有待补录图片）
            #region 页数和翻页按钮的初始化
            if (!loadPageNumLists.Contains(currentPage))
            {
                Debug.Log("加载学生图片页" + dataFileList.Count);
                for (int i = 0; i < showPageIconItemCountPerPage; i++)
                {
                    CreateAnswerPicBtnObj(i);
                }
                loadPageNumLists.Add(currentPage);
            }

            //更新頁碼
            if (PageCount_Text) PageCount_Text.text = currentPage + "/" + totalPage;
            // 最前頁，把左翻頁隱藏
            if (LeftPage_Button) LeftPage_Button.gameObject.SetActive(false);
            // 只有一页，右翻页也隐藏
            if (RightPage_Button)
            {
                if (totalPage == 1)
                {
                    RightPage_Button.gameObject.SetActive(false);
                }
                else
                {
                    RightPage_Button.gameObject.SetActive(true);
                }
            }

            #endregion                        
            if (RightPage_Button)
            {
                RightPage_Button.onClick.RemoveAllListeners();
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
                        if (LeftPage_Button && LeftPage_Button.gameObject.activeSelf == false)
                        {
                            LeftPage_Button.gameObject.SetActive(true);
                        }

                        currentPage = currentPage + 1;

                        int startInt = ((currentPage - 1) * showPageIconItemCountPerPage);
                        //隐藏当前显示图片
                        for (int i = 0; i < showPageIconItemCountPerPage; i++)
                        {
                            int idx = startInt - showPageIconItemCountPerPage + i;
                            if (idx >= 0 && idx < transform.childCount)
                            {
                                transform.GetChild(idx).gameObject.SetActive(false);
                            }
                        }
                        // 翻到最后一頁（要計算展示個數）
                        if (currentPage == (totalPage))
                        {
                            int endInt = PageIconListCount - startInt;
                            if (!loadPageNumLists.Contains(currentPage))
                            {
                                for (int i = 0; i < endInt; i++)
                                {
                                    CreateAnswerPicBtnObj(startInt + i);
                                }
                                loadPageNumLists.Add(currentPage);
                            }
                            else
                            {
                                for (int i = 0; i < endInt; i++)
                                {
                                    int idx = startInt + i;
                                    if (idx >= 0 && idx < transform.childCount)
                                    {
                                        transform.GetChild(idx).gameObject.SetActive(true);
                                    }
                                }
                            }

                            // 把右翻頁隱藏
                            RightPage_Button.gameObject.SetActive(false);
                        }
                        else
                        { // 翻到不是最后一頁

                            if (!loadPageNumLists.Contains(currentPage))
                            {
                                for (int i = 0; i < showPageIconItemCountPerPage; i++)
                                {
                                    CreateAnswerPicBtnObj(startInt + i);
                                }
                                loadPageNumLists.Add(currentPage);
                            }
                            else
                            {
                                for (int i = 0; i < showPageIconItemCountPerPage; i++)
                                {
                                    int idx = startInt + i;
                                    if (idx >= 0 && idx < transform.childCount)
                                    {
                                        transform.GetChild(idx).gameObject.SetActive(true);
                                    }
                                }
                            }
                        }

                    }
                    //更新頁碼
                    if (PageCount_Text) PageCount_Text.text = currentPage + "/" + totalPage;
                    if (TeacherMainManager.instance.Dic_NoScoredStudentAnswerData.ContainsKey(dataFileList[currentPage - 1]))
                    {
                        Debug.Log("Dic_NoScoredStudentAnswerData1" + TeacherMainManager.instance.Dic_NoScoredStudentAnswerData.Count);
                        NoScoredStudentData = TeacherMainManager.instance.Dic_NoScoredStudentAnswerData[dataFileList[currentPage - 1]];
                        string[] parts = NoScoredStudentData.Split('-');
                        NoScoredStudentAnswerName_Input.text = parts.Length > 0 ? parts[0] : "";
                        NoScoredStudentAnswerNumber_Input.text = parts.Length > 1 ? parts[1] : "";
                    }
                    else
                    {
                        Debug.Log("Dic_NoScoredStudentAnswerData2" + TeacherMainManager.instance.Dic_NoScoredStudentAnswerData.Count);
                        NoScoredStudentAnswerName_Input.text = "";
                        NoScoredStudentAnswerNumber_Input.text = "";                    
                    }
                });
            }
            if (LeftPage_Button)
            {
                LeftPage_Button.onClick.RemoveAllListeners();
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
                        if (RightPage_Button && RightPage_Button.gameObject.activeSelf == false)
                        {
                            RightPage_Button.gameObject.SetActive(true);
                        }

                        currentPage = currentPage - 1;
                        // 最前頁，把左翻頁隱藏
                        if (currentPage == 1)
                        {

                            LeftPage_Button.gameObject.SetActive(false);
                        }

                        int startInt = ((currentPage - 1) * showPageIconItemCountPerPage);

                        // 当前是最后一頁
                        if (currentPage + 1 == (totalPage))
                        {
                            int endInt = PageIconListCount - (startInt + showPageIconItemCountPerPage);
                            for (int i = 0; i < endInt; i++)
                            {
                                int idx = startInt + showPageIconItemCountPerPage + i;
                                if (idx >= 0 && idx < transform.childCount)
                                {
                                    transform.GetChild(idx).gameObject.SetActive(false);
                                }
                            }

                            for (int i = 0; i < showPageIconItemCountPerPage; i++)
                            {
                                int idx = startInt + i;
                                if (idx >= 0 && idx < transform.childCount)
                                {
                                    transform.GetChild(idx).gameObject.SetActive(true);
                                }
                            }
                        }
                        else
                        { // 翻到不是最后一頁
                          //隐藏当前显示图片
                            for (int i = 0; i < showPageIconItemCountPerPage; i++)
                            {
                                int idx = startInt + showPageIconItemCountPerPage + i;
                                if (idx >= 0 && idx < transform.childCount)
                                {
                                    transform.GetChild(idx).gameObject.SetActive(false);
                                }
                            }
                            for (int i = 0; i < showPageIconItemCountPerPage; i++)
                            {
                                int idx = startInt + i;
                                if (idx >= 0 && idx < transform.childCount)
                                {
                                    transform.GetChild(idx).gameObject.SetActive(true);
                                }
                            }
                        }
                    }
                    //更新頁碼
                    if (PageCount_Text) PageCount_Text.text = currentPage + "/" + totalPage;
                    if (TeacherMainManager.instance.Dic_NoScoredStudentAnswerData.ContainsKey(dataFileList[currentPage - 1]))
                    {
                        Debug.Log("Dic_NoScoredStudentAnswerData1" + TeacherMainManager.instance.Dic_NoScoredStudentAnswerData.Count);
                        NoScoredStudentData = TeacherMainManager.instance.Dic_NoScoredStudentAnswerData[dataFileList[currentPage - 1]];
                        string[] parts = NoScoredStudentData.Split('-');
                        NoScoredStudentAnswerName_Input.text = parts.Length > 0 ? parts[0] : "";
                        NoScoredStudentAnswerNumber_Input.text = parts.Length > 1 ? parts[1] : "";
                    }
                    else
                    {
                        Debug.Log("Dic_NoScoredStudentAnswerData2" + TeacherMainManager.instance.Dic_NoScoredStudentAnswerData.Count);
                        NoScoredStudentAnswerName_Input.text = "";
                        NoScoredStudentAnswerNumber_Input.text = "";
                    }
                });
            }
            OK_Button.onClick.RemoveAllListeners();
            OK_Button.onClick.AddListener(() => 
            {
                if (NoScoredStudentAnswerName_Input.text == "" && NoScoredStudentAnswerNumber_Input.text != "") {
                    var fullUrl = $"{Config.GetStudentByStudentID}?{"student_id"}={NoScoredStudentAnswerNumber_Input.text}";
                    WebManager.Instance.GetStringFunc(fullUrl, (s) => {
                        Debug.Log("获取学生信息" + s);
                        if (string.IsNullOrEmpty(s)) {
                            TeacherMainManager.instance.NoScoredStudentMessageOKBtn("未查询到用户信息，请重新录入！");
                        }
                        else {
                            ReturnStudentInfo rs = JsonConvert.DeserializeObject<ReturnStudentInfo>(s);
                            switch (rs.code) {
                                case 200:
                                    Debug.Log("用户存在：" + rs.message + "rs.data：" + rs.data);
                                    if (rs.data == null) {
                                        TeacherMainManager.instance.NoScoredStudentMessageOKBtn("未查询到用户信息，请重新录入！");
                                    }
                                    else {
                                        NoScoredStudentAnswerName_Input.text = rs.data.student_name;
                                        NoScoredStudentData = rs.data.student_name + "-" + rs.data.student_id;

                                        if (!TeacherMainManager.instance.Dic_NoScoredStudentAnswerData.ContainsKey(dataFileList[currentPage - 1])) {
                                            TeacherMainManager.instance.Dic_NoScoredStudentAnswerData.Add(dataFileList[currentPage - 1], NoScoredStudentData);
                                        }
                                        else {
                                            TeacherMainManager.instance.Dic_NoScoredStudentAnswerData[dataFileList[currentPage - 1]] = NoScoredStudentData;
                                        }

                                        PendingXueShengZuoYeStorage.Save(TeacherMainManager.instance.Dic_NoScoredStudentAnswerData);
                                        Debug.Log("Dic_NoScoredStudentAnswerData saved, 补录count=" + TeacherMainManager.instance.Dic_NoScoredStudentAnswerData.Count);
                                        if (currentPage - 1 >= 0 && currentPage - 1 < transform.childCount) {
                                            var toggle = transform.GetChild(currentPage - 1).GetChild(1).GetComponent<Toggle>();
                                            if (toggle != null) {
                                                toggle.isOn = true;
                                            }
                                        }

                                        TeacherMainManager.instance.NoScoredStudentMessageOKBtn(
                                            $"补录成功！\n" +
                                            $"姓名:{rs.data.student_name}\n" +
                                            $"学号:{rs.data.student_id}\n" +
                                            $"班级:{rs.data.class_name}\n" +
                                            $"学校:{rs.data.school_name}"
                                        );
                                    }
                                    break;
                                case 400:
                                case 404:
                                    TeacherMainManager.instance.NoScoredStudentMessageOKBtn("未查询到用户信息，请重新录入！");
                                    Debug.Log("学号必填：" + rs.message);
                                    break;
                            }
                        }
                    });
                }
                else { 
                    //输入姓名和学号，校验然后补录
                    NoScoredStudentData = NoScoredStudentAnswerName_Input.text + "-"+ NoScoredStudentAnswerNumber_Input.text;
                    CheckStudentExistence(NoScoredStudentData);
                }

            });

            if (CheckAllToggle)
            {
                CheckAllToggle.isOn = false;
                CheckAllToggle.onValueChanged.RemoveAllListeners();
                CheckAllToggle.onValueChanged.AddListener((bool ison) =>
                {
                    if (loadPageNumLists.Count != dataFileList.Count)
                    {
                        LoadAllPageImage();
                    }
                    InitStudentPicToggle(ison);
                });
            }
        }

        private void OnDisable()
        {
            dataFileList.Clear();
            loadPageNumLists.Clear();
            foreach (Transform item in transform)
            {
                Destroy(item.gameObject);
            }
        }
        // Start is called before the first frame update
        void Start()
        {                        
            

        }

        private void CreateAnswerPicBtnObj(int i, bool isfade = true)
        {
            Debug.Log("加载学生图片页" + dataFileList.Count);
            if (i < 0 || i >= dataFileList.Count) return;

            string input = dataFileList[i];
            var btn_obj = Instantiate(picBtn_Obj, transform);

            // 显示文件名（不包含扩展名），兼容各种路径分隔符
            string fileNameNoExt = Path.GetFileNameWithoutExtension(input);
            var nameText = btn_obj.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>();
            if (nameText != null)
            {
                nameText.text = "【 " + fileNameNoExt + " 】";
            }

            // 加载图片并设置到实例对象上
            var loader = GetComponent<LoadImageTitle>();
            var imgComp = btn_obj.transform.GetChild(1).GetComponent<UnityEngine.UI.Image>();
            var saveTex = btn_obj.transform.GetChild(1).GetComponent<StudentSaveTexture1>();
            if (loader != null)
            {
                loader.LoadImage(input);
                if (imgComp != null)
                {
                    imgComp.sprite = loader.imageSprite;
                }
                if (saveTex != null)
                {
                    saveTex.texture = loader.texture;
                    saveTex.studentAnswerName = input;
                }
            }
            else
            {
                // 兜底：未挂载 LoadImageTitle 时，直接从磁盘读取图片
                try
                {
                    if (File.Exists(input))
                    {
                        byte[] bytes = File.ReadAllBytes(input);
                        var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                        if (tex.LoadImage(bytes))
                        {
                            if (imgComp != null)
                            {
                                var sp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                                imgComp.sprite = sp;
                            }
                            if (saveTex != null)
                            {
                                saveTex.texture = tex;
                                saveTex.studentAnswerName = input;
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning("加载图片失败: " + input + ", err: " + ex.Message);
                }
            }

            btn_obj.SetActive(isfade);
        }

        public void GetAllFilesAndDertorys(string _path)
        {
            if (Directory.Exists(_path))
            {
                DirectoryInfo dir = new DirectoryInfo(_path);
                FileInfo[] files = dir.GetFiles("*");

                foreach (var item in files)
                {
                    if (item.Name.EndsWith(".meta")) continue;
                    if (item.Name.EndsWith(".png"))
                    {
                        dataFileList.Add(item.Name);
                    }
                }
            }
        }
        // Update is called once per frame
        void Update()
        {

        }
        public void InitStudentPicToggle(bool ison = false)
        {
            for (int i = 0; i < dataFileList.Count; i++)
            {
                if (i >= 0 && i < transform.childCount)
                {
                    var toggleHolder = transform.GetChild(i).GetChild(1);
                    var toggle = toggleHolder != null ? toggleHolder.GetComponent<Toggle>() : null;
                    if (toggle != null)
                    {
                        toggle.isOn = ison;
                    }
                }
            }
        }

        private void LoadAllPageImage()
        {
            int saveCurrentPage = currentPage;
            for (int a = 1; a < totalPage - saveCurrentPage + 1; a++)
            {
                Debug.Log("加载学生图片页" + a);
                if ((currentPage) == totalPage)
                {
                    return;
                }
                else
                {

                    currentPage = currentPage + 1;
                    int startInt = ((currentPage - 1) * showPageIconItemCountPerPage);
                    if (currentPage == (totalPage))
                    {
                        int endInt = PageIconListCount - startInt;
                        if (!loadPageNumLists.Contains(currentPage))
                        {
                            for (int i = 0; i < endInt; i++)
                            {
                                CreateAnswerPicBtnObj(startInt + i, false);
                            }
                            loadPageNumLists.Add(currentPage);
                        }
                    }
                    else
                    {                            
                        if (!loadPageNumLists.Contains(currentPage))
                        {
                            for (int i = 0; i < showPageIconItemCountPerPage; i++)
                            {
                                CreateAnswerPicBtnObj(startInt + i, false);
                            }
                            loadPageNumLists.Add(currentPage);
                        }
                    }

                }
            }
            currentPage = saveCurrentPage;
        }
        public bool CheckStudentExistence(string xueShengMessage)
        {
            bool isExist = false;
            WWWForm wwwform = new WWWForm();
            string[] parts = xueShengMessage.Split('-');
            if (parts.Length == 2)
            {
                string fullUrl = $"{Config.GetCheckStudentExistence}?{"first_name"}={parts[0]}&{"studentID"}={parts[1]}";
                Debug.Log("姓名学号fullUrl" + fullUrl);
                WebManager.Instance.GetStringFunc(fullUrl, delegate (string s)
                {
                    Debug.Log("校验姓名学号" + s);
                    if (string.IsNullOrEmpty(s))
                    {
                        isExist = false;
                        TeacherMainManager.instance.NoScoredStudentMessageOKBtn("未查询到用户信息，请重新录入！");
                    }
                    else
                    {
                        ReturnStudentMessageIsExist rs = JsonConvert.DeserializeObject<ReturnStudentMessageIsExist>(s);
                        switch (rs.code)
                        {
                            case 200:
                                Debug.Log("用户存在：" + rs.message + "rs.data：" + rs.data);
                                if (!rs.data)
                                {
                                    isExist = false;
                                    TeacherMainManager.instance.NoScoredStudentMessageOKBtn("未查询到用户信息，请重新录入！");
                                }
                                else
                                {

                                   

                                    isExist = true;
                                    if (!TeacherMainManager.instance.Dic_NoScoredStudentAnswerData.ContainsKey(dataFileList[currentPage - 1]))
                                    {
                                        TeacherMainManager.instance.Dic_NoScoredStudentAnswerData.Add(dataFileList[currentPage - 1], NoScoredStudentData);
                                        PendingXueShengZuoYeStorage.Save(TeacherMainManager.instance.Dic_NoScoredStudentAnswerData);
                                        Debug.Log("Dic_NoScoredStudentAnswerData saved, 补录count=" + TeacherMainManager.instance.Dic_NoScoredStudentAnswerData.Count);
                                        if (currentPage - 1 >= 0 && currentPage - 1 < transform.childCount)
                                        {
                                            var toggle = transform.GetChild(currentPage - 1).GetChild(1).GetComponent<Toggle>();
                                            if (toggle != null)
                                            {
                                                toggle.isOn = true;
                                            }
                                        }
                                        TeacherMainManager.instance.NoScoredStudentMessageOKBtn("补录成功！");
                                    }

                                }

                                break;
                            case 400:
                                isExist = false;
                                TeacherMainManager.instance.NoScoredStudentMessageOKBtn("未查询到用户信息，请重新录入！");
                                Debug.Log("姓名学号必填：" + rs.message);
                                break;
                            case 404:
                                TeacherMainManager.instance.ErrorTipsClear("登录失效,请返回登陆界面重新登录", true, true);
                                Debug.Log("登录失效：" + rs.message);
                                break;
                        }
                    }


                });
            }
            else
            {
                isExist = false;
                TeacherMainManager.instance.NoScoredStudentMessageOKBtn("未查询到用户信息，请重新录入！");
                Debug.Log("学生信息为空");
            }

            return isExist;

        }
    }
}

