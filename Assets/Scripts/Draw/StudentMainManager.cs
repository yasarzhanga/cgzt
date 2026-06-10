using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography;
using TMPro;
using System;
namespace jxzt
{
    /// <summary>
    /// 学生端主管理器
    /// 负责学生界面的核心逻辑：导入题目、绘图、保存答案、与教师端对比等
    /// 继承自 MainManager，扩展了学生专用的业务逻辑
    /// </summary>
    public class StudentMainManager : MainManager
    {
        public Button import_btn;
        public List<LayerManager> standardlayer_manager;
        public GameObject layer_pre;
        public GameObject studentLayer_pre;
        public Button compare_btn;
        public Button answer_btn;
        public Button savePng_btn;
        public Button saveOng_OKbtn;
        public string TeacherAnswerFileName;
        public Button circleTypeBtn;

        public new void Start()
        {
            base.Start();

            standardlayer_manager = new List<LayerManager>();


            import_btn.onClick.AddListener(() =>
            {
                ClearData();
                import_btn.transform.GetChild(1).gameObject.SetActive(!import_btn.transform.GetChild(1).gameObject.activeSelf);


            });
            circleTypeBtn.GetComponent<Button>().onClick.AddListener(() =>
            {
                circleTypeBtn.transform.GetChild(0).gameObject.SetActive(!circleTypeBtn.transform.GetChild(0).gameObject.activeSelf);
                circleTypeBtn.transform.GetChild(1).GetChild(0).gameObject.SetActive(circleTypeBtn.transform.GetChild(0).gameObject.activeSelf);


            });
            bool isRun = false;
            compare_btn.onClick.AddListener(() =>
            {
                if (!string.IsNullOrEmpty(TeacherAnswerFileName))
                {

                    ImportData();


                }
                else
                {
                    compare_btn.transform.GetChild(1).gameObject.SetActive(true);
                }

                if (transform.childCount > 0)
                {
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        if (transform.GetChild(i).GetComponent<StudentLayerManager>())
                        {
                            LayerManager manager = transform.GetChild(i).GetComponent<LayerManager>();

                        }

                    }
                }

                compare_btn.transform.GetComponent<AnswerCheck>().Check();
                if (!isRun)
                {

                    for (int i = 0; i < standardlayer_manager.Count; i++)
                    {
                        foreach (var item in standardlayer_manager[i].data)
                        {
                            int xpos = (int)(item).x;
                            int ypos = (int)(item).y;
                            standardlayer_manager[i].Image_colors[xpos + ypos * standardlayer_manager[i].LayerSize.width] = standardlayer_manager[i].ActiveColor;
                        }
                        standardlayer_manager[i].UpdateTex();


                    }
                    isRun = true;
                }

                foreach (Transform child in transform)
                {
                    if (child.GetComponent<StudentLayerManager>())
                    {
                        child.GetComponent<StudentLayerManager>().enabled = false;
                    }
                    if (!child.GetComponent<StudentLayerManager>())
                    {
                        if (child.GetComponent<TeacherLayerManager>().score != 0 || child.GetComponent<TeacherLayerManager>().layerError == "正确")
                        {
                            child.gameObject.SetActive(false);
                        }
                        else
                        {
                            child.GetComponent<TeacherLayerManager>().enabled = false;
                            child.gameObject.SetActive(true);
                            child.GetChild(0).gameObject.SetActive(true);
                            child.GetChild(1).gameObject.SetActive(true);
                            child.GetComponent<RawImage>().enabled = false;
                        }

                    }
                }

                if (LayerToggleManager.instance.ToggleGroup.transform.childCount > 0)
                {
                    int all = LayerToggleManager.instance.ToggleGroup.transform.childCount;
                    int score = 0;
                    for (int i = 0; i < LayerToggleManager.instance.ToggleGroup.transform.childCount; i++)
                    {
                        if (!LayerToggleManager.instance.ToggleGroup.transform.GetChild(i).gameObject.activeSelf)
                        {
                            score++;
                        }
                    }
                    if (score == all)
                    {
                        compare_btn.transform.GetChild(2).gameObject.SetActive(true);
                    }
                }
            });

            answer_btn.onClick.AddListener(() =>
            {
                foreach (Transform child in transform)
                {
                    if (!child.GetComponent<StudentLayerManager>())
                    {
                        if (child.GetComponent<TeacherLayerManager>().score != 0 || child.GetComponent<TeacherLayerManager>().layerError == "正确")
                        {
                            child.gameObject.SetActive(false);
                        }
                        else
                        {
                            child.GetComponent<RawImage>().enabled = !child.GetComponent<RawImage>().enabled;
                            child.GetChild(1).gameObject.SetActive(!child.GetComponent<RawImage>().enabled);
                        }
                    }
                    else
                    {
                        child.SetAsFirstSibling();
                    }
                }
            });
            savePng_btn.onClick.AddListener(() =>
            {
                savePng_btn.transform.Find("savepopupPanel").gameObject.SetActive(true);
            });
            saveOng_OKbtn.onClick.AddListener(() =>
            {
                String saveName = saveOng_OKbtn.transform.parent.GetChild(0).GetComponent<InputField>().text;
                if (!string.IsNullOrEmpty(saveName))
                {
                    foreach (Transform child in transform)
                    {
                        if (child.GetComponent<StudentLayerManager>())
                        {
                            savePng_btn.gameObject.GetComponent<StudentSavePic>().InputSave(saveName);

                        }
                    }
                    saveOng_OKbtn.transform.parent.GetChild(0).GetComponent<InputField>().text = "";
                    savePng_btn.transform.GetChild(0).gameObject.SetActive(false);
                }

            });
        }

        public void ImportData()
        {
            var bytes = File.ReadAllBytes(Path.Combine(Application.streamingAssetsPath, "TeacherAnswer/" + TeacherAnswerFileName));
            string savejson = System.Text.Encoding.Default.GetString(bytes);
            AnswerFile exportFile = JsonConvert.DeserializeObject<AnswerFile>(savejson);


            for (int i = 0; i < exportFile.data.Count; i++)
            {
                LoadStandardAnswerLayerButton();
                if (exportFile.data[i].data != null)
                {
                    standardlayer_manager[i].data = exportFile.data[i].data;
                    standardlayer_manager[i].layerNum = exportFile.data[i].layerNum;
                    standardlayer_manager[i].layerError = exportFile.data[i].layerError;
                    standardlayer_manager[i].score = exportFile.data[i].score;
                    standardlayer_manager[i].frameSelectData = exportFile.data[i].frameSelectData;
                    standardlayer_manager[i].lineType = (linetype)System.Enum.Parse(typeof(linetype), exportFile.data[i].lineType);
                    standardlayer_manager[i].xuhaoPosX = exportFile.data[i].xuhaoPosX;
                    standardlayer_manager[i].xuhaoPosY = exportFile.data[i].xuhaoPosY;
                    standardlayer_manager[i].lineshape = (lineshape)System.Enum.Parse(typeof(lineshape), exportFile.data[i].lineshape);
                    standardlayer_manager[i].circleData = exportFile.data[i].circleData;
                    standardlayer_manager[i].arcData = exportFile.data[i].arcData;
                    standardlayer_manager[i].ellipseData = exportFile.data[i].ellipseData;
                    standardlayer_manager[i].markData = exportFile.data[i].markData;
                }


            }
            compare_btn.transform.GetComponent<AnswerCheck>().standardlayer_manager = standardlayer_manager;
        }

        void LoadStandardAnswerLayerButton()
        {
            GameObject layer = Instantiate(layer_pre, transform);
            layer.transform.SetAsLastSibling();
            layer.transform.GetChild(0).gameObject.SetActive(false);
            LayerManager manager = layer.GetComponent<LayerManager>();
            standardlayer_manager.Add(manager);
            manager.LayerSize = new Size(width, height);
        }
        private void CreateStudentLayerManager()
        {
            foreach (Transform child in transform)
            {
                if (child.GetComponent<StudentLayerManager>())
                {
                    LayerManager manager = child.GetComponent<LayerManager>();
                    manager.LayerSize = new Size(width, height);
                }
            }

        }

        public void ClearData()
        {
            compare_btn.transform.GetChild(2).gameObject.SetActive(false);
            standardlayer_manager.Clear();

            if (LayerToggleManager.instance.ToggleGroup.transform.childCount > 0)
            {
                for (int i = 0; i < LayerToggleManager.instance.ToggleGroup.transform.childCount; i++)
                {
                    Destroy(LayerToggleManager.instance.ToggleGroup.transform.GetChild(i).gameObject);
                }
            }
            for (int i = 0; i < transform.childCount; i++)
            {
                if (!transform.GetChild(i).GetComponent<StudentLayerManager>())
                {
                    Destroy(transform.GetChild(i).gameObject);
                }
                else
                {

                }
            }
            TeacherAnswerFileName = "";
        }
    }

}
