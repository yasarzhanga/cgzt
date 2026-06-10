using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static jxzt.Info;
namespace jxzt
{
    /// <summary>
    /// 保存纹理类（教师答案缩略图按钮）
    /// 挂载在教师答案缩略图按钮上，存储纹理信息和题目ID
    /// 点击时设置当前教师答案、扫描二维码获取PPT ID，并上传题目到服务器
    /// </summary>
    public class SaveTextureClass : MonoBehaviour
    {
        public Texture2D texture;
        public Texture2D clipTexture;
        public Texture2D clippedTexture;
        public string teacherAnswerName;
        public string studentAnswerName;
        public int problemId;
        public string ScanQRCodePPTId;
        // Start is called before the first frame update
        void Start()
        {

            if (transform.GetComponent<Button>() != null)
            {
                transform.GetComponent<Button>().onClick.AddListener(() =>
                {
                    if (!string.IsNullOrEmpty(teacherAnswerName))
                    {                        

                        GameObject.Find("PaintBoard").transform.GetComponent<TeacherMainManager>().TeacherAnswerFileName = teacherAnswerName;
                        Debug.Log("导入老师答案" + GameObject.Find("PaintBoard").transform.GetComponent<TeacherMainManager>().TeacherAnswerFileName);
                        //二维码扫描  
                        transform.GetComponent<ZXingQRCodeWrapper_ScanQRCode>().scanPicTexture = texture;
                        string path = Application.streamingAssetsPath + "/TeacherAnswer/" + teacherAnswerName + ".save";
                        Debug.Log("save文件路径"+path);
                        if (File.Exists(path))
                        {
                            GameObject.Find("PaintBoard").transform.GetComponent<TeacherMainManager>().GetqrCodePosData();
                        }                        
                            List<List<PositionInt>> qrCodeData = GameObject.Find("PaintBoard").transform.GetComponent<TeacherMainManager>().qrCodeData;                         
                        if (qrCodeData.Count!=0)
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
                            if (!string.IsNullOrEmpty(ScanQRCodePPTId) && ScanQRCodePPTId != " ")
                            {
                                WWWForm wwwform = new WWWForm();

                                wwwform.AddBinaryData("title", File.ReadAllBytes(Application.streamingAssetsPath + "/TeacherAnswerPic/" + teacherAnswerName + ".png"), teacherAnswerName + ".png", "image/png");
                                wwwform.AddBinaryData("title_resize", File.ReadAllBytes(Application.streamingAssetsPath + "/TeacherAnswerPic/" + teacherAnswerName + ".png"), teacherAnswerName + ".png", "image/png");
                                wwwform.AddBinaryData("StandardAnswer", File.ReadAllBytes(Application.streamingAssetsPath + "/TeacherAnswer/" + teacherAnswerName + ".save"), teacherAnswerName + ".save", "application/json");
                                wwwform.AddField("Details", " ");
                                wwwform.AddField("AreaResource", ScanQRCodePPTId);
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
                                    }
                                    transform.parent.parent.parent.parent.GetChild(0).gameObject.SetActive(!transform.parent.parent.parent.parent.GetChild(0).gameObject.activeSelf);
                                });
                            }
                            else
                            {
                                Debug.Log("找不到二维码数据"+ ScanQRCodePPTId);
                            }


                        }                                                    
                     

                    }


                });
            }

        }
        
            // Update is called once per frame
            void Update()
        {

        }
    }
}

