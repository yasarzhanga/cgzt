using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static jxzt.Info;
namespace jxzt
{
    /// <summary>
    /// 教师导入答案滚动视图
    /// 用于教师端上传标准答案，支持二维码扫描获取PPT ID
    /// 上传题目图片和答案数据到服务器
    /// </summary>
    public class TeacherImportAnswerScrollView : MonoBehaviour
    {
        public List<string> dataFileList;
        public TMP_Dropdown drapdown;
        List<TMP_Dropdown.OptionData> listOptions = new List<TMP_Dropdown.OptionData>();
        public int problemId;
        public string ScanQRCodePPTId;
        public string teacherAnswerName;

        private void OnEnable()
        {
            Debug.Log("上传题目");
            teacherAnswerName = "";
            ScanQRCodePPTId = "";
            GetAllFilesAndDertorys(Application.streamingAssetsPath + "/TeacherAnswer/");
            drapdown.onValueChanged.RemoveAllListeners();
            drapdown.onValueChanged.AddListener((int n) =>
            {

                if (Alert.Instance != null)
                {

                    teacherAnswerName = listOptions[n].text;

                    string n_str_TeacherAnswerPic = Application.streamingAssetsPath + "/TeacherAnswerPic/" + teacherAnswerName + ".png";
                    string n_str_ShowAnswerPic = Application.streamingAssetsPath + "/ShowAnswerPic/" + teacherAnswerName + ".png";
                    string n_str_TeacherAnswerSave = Application.streamingAssetsPath + "/TeacherAnswer/" + teacherAnswerName + ".save";
                    if (File.Exists(n_str_TeacherAnswerPic) && File.Exists(n_str_ShowAnswerPic) && File.Exists(n_str_TeacherAnswerSave))
                    {
                        Alert.Instance.ShowTips_TwoBtn(teacherAnswerName);
                        Alert.Instance.button_upload.onClick.RemoveAllListeners();
                        Alert.Instance.button_upload.onClick.AddListener(delegate ()
                        {
                            transform.GetComponent<LoadImageTitle>().LoadImage(Application.streamingAssetsPath + "/TeacherAnswerPic/" + teacherAnswerName + ".png");
                            GameObject.Find("PaintBoard").transform.GetComponent<TeacherMainManager>().TeacherAnswerFileName = teacherAnswerName;
                            Debug.Log("导入老师答案" + teacherAnswerName);
                            //二维码扫描  
                            transform.GetComponent<ZXingQRCodeWrapper_ScanQRCode>().scanPicTexture = transform.GetComponent<LoadImageTitle>().texture;
                            Debug.Log("transform.GetComponent<LoadImageTitle>().texture" + transform.GetComponent<LoadImageTitle>().texture);
                            string path = Application.streamingAssetsPath + "/TeacherAnswer/" + teacherAnswerName + ".save";
                            Debug.Log("save文件路径" + path);
                            if (File.Exists(path))
                            {
                                GameObject.Find("PaintBoard").transform.GetComponent<TeacherMainManager>().GetqrCodePosData();
                            }
                            List<List<PositionInt>> qrCodeData = GameObject.Find("PaintBoard").transform.GetComponent<TeacherMainManager>().qrCodeData;
                            if (qrCodeData.Count != 0)
                            {
                                int clitWidth = (int)Vector2.Distance(new Vector2(qrCodeData[0][0].x, qrCodeData[0][0].y), new Vector2(qrCodeData[0][3].x, qrCodeData[0][3].y));
                                transform.GetComponent<ZXingQRCodeWrapper_ScanQRCode>().clipWidth = clitWidth;
                                transform.GetComponent<ZXingQRCodeWrapper_ScanQRCode>().clipHeight = clitWidth;
                                if (qrCodeData[0][0].x < qrCodeData[0][2].x)
                                {
                                    transform.GetComponent<ZXingQRCodeWrapper_ScanQRCode>().clipX = qrCodeData[0][0].x;
                                    transform.GetComponent<ZXingQRCodeWrapper_ScanQRCode>().clipY = qrCodeData[0][2].y;
                                }
                                else
                                {
                                    transform.GetComponent<ZXingQRCodeWrapper_ScanQRCode>().clipX = qrCodeData[0][2].x;
                                    transform.GetComponent<ZXingQRCodeWrapper_ScanQRCode>().clipY = qrCodeData[0][0].y;
                                }
                                transform.GetComponent<ZXingQRCodeWrapper_ScanQRCode>().ScanningButtonClick();
                                ScanQRCodePPTId = transform.GetComponent<ZXingQRCodeWrapper_ScanQRCode>().pptid;
                                Debug.Log("上传答案AreaResource" + ScanQRCodePPTId);

                            }
                            WWWForm wwwform = new WWWForm();

                            wwwform.AddBinaryData("title", File.ReadAllBytes(Application.streamingAssetsPath + "/TeacherAnswerPic/" + teacherAnswerName + ".png"), teacherAnswerName + ".png", "image/png");
                            Debug.Log("上传答案title" + teacherAnswerName + ".png");
                            wwwform.AddBinaryData("title_resize", File.ReadAllBytes(Application.streamingAssetsPath + "/TeacherAnswerPic/" + teacherAnswerName + ".png"), teacherAnswerName + ".png", "image/png");
                            Debug.Log("上传答案title_resize" + teacherAnswerName + ".png");
                            wwwform.AddBinaryData("StandardAnswer", File.ReadAllBytes(Application.streamingAssetsPath + "/TeacherAnswer/" + teacherAnswerName + ".save"), teacherAnswerName + ".save", "application/json");
                            Debug.Log("上传答案StandardAnswer" + teacherAnswerName + ".save");
                            wwwform.AddField("Details", " ");
                            wwwform.AddField("Answer_number", ImportLoacalSaveFile());
                            Debug.Log("上传答案Answer_number" + ImportLoacalSaveFile());
                            wwwform.AddField("AreaResource", ScanQRCodePPTId);
                            Debug.Log("上传答案AreaResource" + ScanQRCodePPTId);
                            wwwform.AddBinaryData("ShowAnswer", File.ReadAllBytes(Application.streamingAssetsPath + "/ShowAnswerPic/" + teacherAnswerName + ".png"), teacherAnswerName + ".png", "image/png");
                            WebManager.Instance.GetStringFunc(Config.UpLoadProblem, wwwform, delegate (string s)
                            {
                                Debug.Log("上传答案" + s);
                                if (s.Length == 0)
                                {
                                    Debug.Log("上传答案失败");
                                    return;
                                }
                                ReturnStateTeacher rs = JsonConvert.DeserializeObject<ReturnStateTeacher>(s);
                                switch (rs.code)
                                {
                                    case 201:
                                        Alert.Instance.ShowTips(rs.message);
                                        Debug.Log("上传成功：" + rs.message);
                                        break;
                                    case 400:
                                        Alert.Instance.ShowTips(rs.message);
                                        Debug.Log("上传失败：" + rs.message);
                                        break;
                                    case 404:
                                        TeacherMainManager.instance.ErrorTipsClear("登录失效,请返回登陆界面重新登录", true, true);
                                        Debug.Log("登录失效：" + rs.message);
                                        break;
                                }

                            });


                            Alert.Instance.ShowTips_TwoBtn("标准答案上传");
                        });
                        Alert.Instance.button_delete.onClick.RemoveAllListeners();
                        Alert.Instance.button_delete.onClick.AddListener(delegate ()
                        {
                            // 新增：本地删除导入的作业（图片 + 展示图 + save 文件）
                            try
                            {
                                if (File.Exists(n_str_TeacherAnswerPic)) File.Delete(n_str_TeacherAnswerPic);
                                if (File.Exists(n_str_ShowAnswerPic)) File.Delete(n_str_ShowAnswerPic);
                                if (File.Exists(n_str_TeacherAnswerSave)) File.Delete(n_str_TeacherAnswerSave);
                            }
                            catch (System.Exception ex)
                            {
                                Debug.LogError("删除本地标准答案文件失败: " + ex.Message);
                                if (Alert.Instance != null)
                                {
                                    Alert.Instance.ShowTips("本地删除失败: " + ex.Message);
                                }
                            }

                            // 刷新下拉列表，移除已删除项
                            GetAllFilesAndDertorys(Application.streamingAssetsPath + "/TeacherAnswer/");
                            if (drapdown != null)
                            {
                                drapdown.value = 0;
                                drapdown.RefreshShownValue();
                            }

                            if (Alert.Instance != null)
                            {
                                Alert.Instance.ShowTips("本地标准答案已删除");
                            }
                        });

                    }
                    else
                    {
                        TeacherMainManager.instance.ErrorTipsClear(teacherAnswerName + "未找到");
                    }

                }
            });
        }


        void Start()
        {

            


        }

        private void CreateSaveBtnObj(int i)
        {
        }


        public void GetAllFilesAndDertorys(string _path)
        {
            //判断路径是否存在
            if (Directory.Exists(_path))
            {
                DirectoryInfo dir = new DirectoryInfo(_path);
                FileInfo[] files = dir.GetFiles("*");
                drapdown.ClearOptions();
                listOptions.Clear();
                listOptions.Add(new TMP_Dropdown.OptionData("请选择---"));
                foreach (var item in files)
                {
                    //忽略.meta
                    if (item.Name.EndsWith(".meta")) continue;
                    if (item.Name.EndsWith(".save"))
                    {
                        string input = item.Name;
                        string[] parts = input.Split('.');
                        listOptions.Add(new TMP_Dropdown.OptionData(parts[0]));
                    }

                }
                drapdown.AddOptions(listOptions);
            }
        }
        // Update is called once per frame
        void Update()
        {

        }

        public void GetBackPic(string titlePicUrl)
        {
            WebManager.Instance.GetTextureFunc(titlePicUrl, delegate (Texture2D texture2D)
            {
            });
        }

        /// <summary>
        /// 向前（左）翻頁
        /// </summary>

        private void HidePageItems()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        public int ImportLoacalSaveFile()
        {
            var bytes = File.ReadAllBytes(Application.streamingAssetsPath + "/TeacherAnswer/" + teacherAnswerName + ".save");
            string savejson = System.Text.Encoding.Default.GetString(bytes);
            AnswerFile exportFile = JsonConvert.DeserializeObject<AnswerFile>(savejson);

            int Answer_number = 0;
            for (int i = 0; i < exportFile.data.Count; i++)
            {
                if ((lineshape)System.Enum.Parse(typeof(lineshape), exportFile.data[i].lineshape) != lineshape.二维码 && (lineshape)System.Enum.Parse(typeof(lineshape), exportFile.data[i].lineshape) != lineshape.判分区域 && exportFile.data[i].ocr != 文字识别.姓名.ToString() && exportFile.data[i].ocr != 文字识别.学号.ToString())
                {
                    Answer_number++;
                }

            }
            Debug.Log("导入的标准答案判分点数量Answer_number" + Answer_number);
            return Answer_number;
        }
    }
}

