using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace jxzt
{
    /// <summary>
    /// 学生答案导入滚动视图
    /// 用于教师端批量导入学生答案，支持分页浏览和批量选择
    /// 配合 DeletePanel 实现未识别答案的补录
    /// </summary>
    public class StudentImportAnswerScrollView : MonoBehaviour
    {

        public List<string> dataFileList;
        public GameObject picBtn_Obj;
        public Toggle CheckAllToggle;

        int showPageIconItemCountPerPage = 9;
        public Button LeftPage_Button;
        public Button RightPage_Button;
        public TMP_Text PageCount_Text;

        public int currentPage = 0;
        public int totalPage = 0;
        public int PageIconListCount = 0;
        public List<int> loadPageNumLists = new List<int>();

        private void Awake()
        {
            dataFileList = new List<string>();
        }

        private void OnEnable()
        {
            
            GetAllFilesAndDertorys(OpenFileWindow.choosedFilefolderPath);

            // 设置初始当前页为 1
            currentPage = 1;
            PageIconListCount = dataFileList.Count;
            totalPage = (PageIconListCount / showPageIconItemCountPerPage) + ((PageIconListCount % showPageIconItemCountPerPage) == 0 ? 0 : 1);
            Debug.Log("导入学生答案总页数PageIconListCount" + PageIconListCount);
            Debug.Log("导入学生答案总页数totalPage" + totalPage);
            #region 页数和翻页按钮的初始化


            if (!loadPageNumLists.Contains(currentPage))
            {
                for (int i = 0; i < showPageIconItemCountPerPage; i++)
                {
                    CreateAnswerPicBtnObj(i);
                }
                loadPageNumLists.Add(currentPage);
            }

            //更新頁碼
            PageCount_Text.text = currentPage + "/" + totalPage;
            // 最前頁，把左翻頁隱藏
            LeftPage_Button.gameObject.SetActive(false);
            // 只有一页，右翻页也隐藏
            if (totalPage == 1)
            {
                RightPage_Button.gameObject.SetActive(false);
            }
            else
            {
                RightPage_Button.gameObject.SetActive(true);
            }

            #endregion                        
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
                    if (LeftPage_Button.gameObject.activeSelf == false)
                    {
                        LeftPage_Button.gameObject.SetActive(true);
                    }

                    currentPage = currentPage + 1;

                    int startInt = ((currentPage - 1) * showPageIconItemCountPerPage);
                    //隐藏当前显示图片
                    for (int i = 0; i < showPageIconItemCountPerPage; i++)
                    {
                        if (transform.GetChild(startInt - showPageIconItemCountPerPage + i))
                        {
                            transform.GetChild(startInt - showPageIconItemCountPerPage + i).gameObject.SetActive(false);
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
                                transform.GetChild(startInt + i).gameObject.SetActive(true);
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
                                transform.GetChild(startInt + i).gameObject.SetActive(true);
                            }
                        }
                    }

                }
                //更新頁碼
                PageCount_Text.text = currentPage + "/" + totalPage;

            });
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

                    int startInt = ((currentPage - 1) * showPageIconItemCountPerPage);

                    // 当前是最后一頁
                    if (currentPage + 1 == (totalPage))
                    {
                        int endInt = PageIconListCount - (startInt + showPageIconItemCountPerPage);
                        for (int i = 0; i < endInt; i++)
                        {
                            if (transform.GetChild(startInt + showPageIconItemCountPerPage + i))
                            {
                                transform.GetChild(startInt + showPageIconItemCountPerPage + i).gameObject.SetActive(false);
                            }

                        }

                        for (int i = 0; i < showPageIconItemCountPerPage; i++)
                        {
                            transform.GetChild(startInt + i).gameObject.SetActive(true);
                        }


                    }
                    else
                    { // 翻到不是最后一頁
                      //隐藏当前显示图片
                        for (int i = 0; i < showPageIconItemCountPerPage; i++)
                        {
                            if (transform.GetChild(startInt + showPageIconItemCountPerPage + i))
                            {
                                transform.GetChild(startInt + showPageIconItemCountPerPage + i).gameObject.SetActive(false);
                            }

                        }

                        for (int i = 0; i < showPageIconItemCountPerPage; i++)
                        {
                            transform.GetChild(startInt + i).gameObject.SetActive(true);
                        }

                    }

                }
                //更新頁碼
                PageCount_Text.text = currentPage + "/" + totalPage;

            });


            CheckAllToggle.isOn = false;
            CheckAllToggle.onValueChanged.AddListener((bool ison) =>
            {
                if (loadPageNumLists.Count != dataFileList.Count)
                {
                    LoadAllPageImage();
                }
                InitStudentPicToggle(ison);

            });
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
            if (i < dataFileList.Count)
            {
                if (i < dataFileList.Count && File.Exists(OpenFileWindow.choosedFilefolderPath + "/" + dataFileList[i]))
                {
                    GameObject btn_obj = Instantiate(picBtn_Obj, transform);
                    string input = dataFileList[i];
                    string[] parts = input.Split('.');
                    btn_obj.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = "【 " + parts[0] + " 】";
                    transform.GetComponent<LoadImageTitle>().LoadImage(OpenFileWindow.choosedFilefolderPath + "/" + dataFileList[i]);
                    transform.GetChild(i).GetChild(1).GetComponent<Image>().sprite = transform.GetComponent<LoadImageTitle>().imageSprite;
                    transform.GetChild(i).GetChild(1).GetComponent<StudentSaveTexture>().texture = transform.GetComponent<LoadImageTitle>().texture;

                    transform.GetChild(i).GetChild(1).GetComponent<StudentSaveTexture>().studentAnswerName = OpenFileWindow.choosedFilefolderPath + "/" + dataFileList[i];//dataFileList[i];
                    btn_obj.SetActive(isfade);
                }
            }
        }

        public void GetAllFilesAndDertorys(string _path)
        {
            //判断路径是否存在
            if (Directory.Exists(_path))
            {
                DirectoryInfo dir = new DirectoryInfo(_path);
                FileInfo[] files = dir.GetFiles("*");

                foreach (var item in files)
                {
                    //忽略.meta
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
                if (transform.GetChild(i).GetChild(1).GetComponent<Toggle>() != null)
                {
                    transform.GetChild(i).GetChild(1).GetComponent<Toggle>().isOn = ison;
                }

            }
        }

        private void LoadAllPageImage()
        {
            int saveCurrentPage = currentPage;
            for (int a = 1; a < totalPage - saveCurrentPage + 1; a++)
            {
                Debug.Log("加载学生图片页" + a);
                // 最后一頁，不能右翻頁        
                if ((currentPage) == totalPage)
                {
                    return;
                }
                else
                {

                    currentPage = currentPage + 1;
                    int startInt = ((currentPage - 1) * showPageIconItemCountPerPage);
                    // 翻到最后一頁（要計算展示個數）
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
                    { // 翻到不是最后一頁                           
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
    }
}

