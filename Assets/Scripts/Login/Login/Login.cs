using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static jxzt.Info;
namespace jxzt
{
    public class AA
    {
        public string name;
    }
    /// <summary>
    /// 登录管理器
    /// 处理用户登录、验证码校验、密码找回等认证逻辑
    /// 连接后端API，验证账号密码，管理登录态
    /// </summary>
    public class Login : MonoBehaviour
    {
        public InputField inputField_zhanghao;//账号
        public InputField inputField_mima;//密码
        public InputField inputField_yazhengma;//验证码
        private string uuid;//验证码对应的UUID
        public RawImage verificationCodeRawImage;//验证码图片
        public Button button_Login, button_LoginTeacher;//登录
        public Button button_ForgetPwd;//忘记密码
        public Button changeVerificationCodeImgBtn;//切换二维码图片按钮
        public Button LoginPageExitBtn;//登录页退出按钮
        public GameObject bg_Obj;//背景对象
        public InputField inputField_phoneNum;//验证手机号
        public InputField inputField_pwdyanzhengma;//更改密码验证码
        public InputField inputField_newpassword;//新密码
        public InputField inputField_secondpassword;//确认密码
        public GameObject tips_MiMaDiff;//密码不一致提示
        int id_user;//用户id
        public Text countdownText;
        private float timeLeft = 60f;
        string sessionId;
        public GameObject PicLoading;
        private const double ExpiryDurationInSeconds = 3 * 24 * 60 * 60; // 三天的秒数

        void Start()
        {
            
            if (PlayerPrefs.HasKey("username"))
            {
                inputField_zhanghao.text = PlayerPrefs.GetString("username", "");
            }
            if (PlayerPrefs.HasKey("password"))//&& rememberMeToggle.isOn
            {
                inputField_mima.text = PlayerPrefs.GetString("password", "");
            }
            else
            {
                Debug.Log("加载密码为空");
            }


            Debug.Log("加载用户名密码" + PlayerPrefs.GetString("username", "") + ":" + PlayerPrefs.GetString("password", ""));

            GetVerificationCodePic(Config.VerificationCode);

            button_Login.onClick.AddListener(delegate ()
            {
                LoginType("学生");
            });
            button_LoginTeacher.onClick.AddListener(delegate ()
            {
                LoginType("教师");
            });
            changeVerificationCodeImgBtn.onClick.AddListener(() =>
            {
                GetVerificationCodePic(Config.VerificationCode);
            });
            button_ForgetPwd.onClick.AddListener(() =>
            {
                ForgetPassWord_Btn();
            });
            LoginPageExitBtn.onClick.AddListener(delegate ()
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
               Application.Quit();
#endif

            });

        }
        //加密
        public string ToBase64String(string value)
        {
            if (value == null || value == "")
            {
                return "";
            }
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(bytes);
        }
        //解密
        public static string UnBase64String(string value)
        {
            if (value == null || value == "")
            {
                return "";
            }
            byte[] bytes = Convert.FromBase64String(value);
            return Encoding.Default.GetString(bytes);
        }

        public class A
        {
            public string token;
        }


        // Update is called once per frame
        void Update()
        {
            // 适配新 Input System：检测 Enter 键（主键盘或小键盘）
            if (Keyboard.current != null)
            {
                if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame)
                {
                    LoginType("学生");
                }
            }
        }
        void LoginType(string str)
        {
            var phoneNumRegex = new Regex(Config.PhoneNum);
            var mima = new Regex(Config.MiMa);
            if (string.IsNullOrWhiteSpace(inputField_zhanghao.text) || inputField_zhanghao.text.Contains(" "))
            {
                Alert.Instance.ShowTips("账号为空或者含有空字符");
                return;
            }
            if (!phoneNumRegex.IsMatch(inputField_zhanghao.text))
            {
                Alert.Instance.ShowTips("账号长度11位数字");
                return;
            }
            if (string.IsNullOrWhiteSpace(inputField_mima.text))
            {
                Alert.Instance.ShowTips("密码为空或者含有空字符");
                return;
            }
            if (!mima.IsMatch(inputField_mima.text))
            {
                Alert.Instance.ShowTips("密码至少包含一个大写字母、一个小写字母、一个特殊字符，并且长度在8至20个字符");
                return;
            }
            if (string.IsNullOrWhiteSpace(inputField_yazhengma.text))
            {
                Alert.Instance.ShowTips("验证码为空或者含有空字符");
                return;
            }


            if (str == "教师")
            {
                ToLoginTeacher(inputField_zhanghao.text, inputField_mima.text);
            }
            if (str == "学生")
            {
                ToLogin(inputField_zhanghao.text, inputField_mima.text, inputField_yazhengma.text, uuid);
            }
        }

        public void ToLogin(string username, string password, string captcha, string uuid)
        {
            //检查登陆状态
            WebManager.Instance.GetStringFunc(Config.IsLogin, delegate (string s)
            {
                Debug.Log("检查登录" + s);
                ReturnState rs = JsonConvert.DeserializeObject<ReturnState>(s);
                switch (rs.code)
                {
                    case 200:
                        if (rs.username == username)
                        {
                            Debug.Log("已存在登录状态" + rs.message);
                            // 开始加载场景
                            StartCoroutine(LoadSceneWithLoadingScreen());
                        }
                        else
                        {
                            Debug.Log("账号登录状态不一致");
                            UnityWebRequest.ClearCookieCache();
                            Exit(Config.Exit);

                            captcha = SendLoginMessage(username, password, captcha, uuid);
                        }


                        break;
                    case 400:
                        Debug.Log("登陆失败" + rs.message);
                        captcha = SendLoginMessage(username, password, captcha, uuid);
                        break;
                    case 401:

                        Debug.Log("检查登录" + rs.message);
                        captcha = SendLoginMessage(username, password, captcha, uuid);
                        break;
                }
            });
        }

        private string SendLoginMessage(string username, string password, string captcha, string uuid)
        {
            captcha = captcha.ToUpper();
            var form = new List<IMultipartFormSection>
        {
            new MultipartFormDataSection("username", username),//用户名
            new MultipartFormDataSection("password",password),//密码
            new MultipartFormDataSection("captcha",captcha),//验证码
            new MultipartFormDataSection("uuid",uuid),//验证码对应uuid
            new MultipartFormDataSection("unity","True"),//验证参数unity=true
        };
            WebManager.Instance.GetStringFunc(Config.LoginServerAddress, form, username, password, delegate (string s)
            {
                Debug.Log("登录");
                Debug.Log(s);
                if (s.Length == 0)
                {
                    Alert.Instance.ShowTips("登录失败");
                    return;
                }
                ReturnState rs = JsonConvert.DeserializeObject<ReturnState>(s);
                switch (rs.code)
                {

                    case 201:
                        Admin.Instance.isTeacher = false;
                        Admin.Instance.studentInfo = rs.data;
                        PlayerPrefs.SetString("id", rs.id.ToString());
                        PlayerPrefs.SetString("Identitypermissions", rs.Identitypermissions.ToString());
                        PlayerPrefs.SetString("username", username);
                        PlayerPrefs.SetString("password", password);
                        Debug.Log("保存用户名密码" + username + ":" + password);

                        // 开始加载场景
                        StartCoroutine(LoadSceneWithLoadingScreen());


                        break;
                    case 400:
                        Alert.Instance.ShowTips("用户名或密码错误");
                        GetVerificationCodePic(Config.VerificationCode);
                        break;
                    case 401:
                        Alert.Instance.ShowTips("验证码错误");
                        GetVerificationCodePic(Config.VerificationCode);
                        break;
                    case 402:
                        Alert.Instance.ShowTips("密码错误,还可尝试" + rs.attempts + "次");
                        GetVerificationCodePic(Config.VerificationCode);
                        break;
                }
            });
            return captcha;
        }

        void ToLoginTeacher(string id, string passWord)
        {
            var form = new List<IMultipartFormSection>
        {
            new MultipartFormDataSection("CardID", id),//用户名
            new MultipartFormDataSection("Password",passWord),//密码
        };
            WebManager.Instance.GetStringFunc(Config.LoginUrl, form, delegate (string s)
            {
                Debug.Log("登录");
                Debug.Log(s);
                if (s.Length == 0)
                {
                    Alert.Instance.ShowTips("登录失败");
                    return;
                }
                ReturnStateTeacher rs = JsonConvert.DeserializeObject<ReturnStateTeacher>(s);
                switch (rs.code)
                {
                    case 201:
                        SceneManager.LoadScene("TestList");
                        break;
                    case 202:
                        SceneManager.LoadScene("TestList");
                        break;
                    case 400:
                        Alert.Instance.ShowTips("密码错误");
                        break;
                    case 401:
                        Alert.Instance.ShowTips("账号不存在");
                        break;
                }
            });
        }


        void GetVerificationCodePic(string ip)
        {
            uuid = System.Guid.NewGuid().ToString();
            Debug.Log("UUID： " + uuid);
            Debug.Log("UUID1： " + ip + uuid + "/");
            WebManager.Instance.GetTextureFunc(ip + uuid + "/", delegate (Texture2D texture2D)
            {
                verificationCodeRawImage.texture = texture2D;
            });

        }
        int CheckIsLogin(string url)
        {
            int loginstate = 0;
            WebManager.Instance.GetStringFunc(url, delegate (string s)
            {
                Debug.Log("检查登录" + s);
                ReturnState rs = JsonConvert.DeserializeObject<ReturnState>(s);
                loginstate = rs.code;
            });
            return loginstate;

        }
        public void Exit(string url)
        {
            int loginstate = 0;
            WebManager.Instance.GetStringFunc(url, delegate (string s)
            {
                Debug.Log("退出" + s);
                ReturnState rs = JsonConvert.DeserializeObject<ReturnState>(s);
                loginstate = rs.code;
                switch (rs.code)
                {
                    case 201:
                        Debug.Log("退出成功");
                        break;
                    case 400:
                        Debug.Log("退出失败");
                        break;
                    case 401:
                        Debug.Log("未登录");
                        break;
                }
            });


        }
        void ForgetPassWord_Btn()
        {
            bg_Obj.transform.GetChild(0).gameObject.SetActive(false);
            bg_Obj.transform.GetChild(1).gameObject.SetActive(true);
            var phoneNumRegex = new Regex(Config.PhoneNum);
            var mima = new Regex(Config.MiMa);
            id_user = 0;
            bg_Obj.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(1).GetComponent<Button>().onClick.AddListener(() =>
            {
                if (string.IsNullOrWhiteSpace(inputField_phoneNum.text) || inputField_phoneNum.text.Contains(" "))
                {
                    Alert.Instance.ShowTips("手机号为空或者含有空字符");
                    return;
                }
                if (!phoneNumRegex.IsMatch(inputField_phoneNum.text))
                {
                    Alert.Instance.ShowTips("请输入11位手机号数字");
                    return;
                }
                var form = new List<IMultipartFormSection>
        {
            new MultipartFormDataSection("Phone", inputField_phoneNum.text),//手机号           
        };
                WebManager.Instance.GetStringFunc(Config.SendCodeByPhoneNumber, form, delegate (string s)
                {
                    Debug.Log("根据手机号发送验证码");
                    Debug.Log(s);
                    if (s.Length == 0)
                    {
                        Alert.Instance.ShowTips("发送验证码失败");
                        return;
                    }
                    ReturnStateTeacher rs = JsonConvert.DeserializeObject<ReturnStateTeacher>(s);
                    switch (rs.code)
                    {
                        case 201:
                            Debug.Log(rs.message);//发送成功
                            bg_Obj.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(2).gameObject.SetActive(true);
                            StartCoroutine(Countdown());

                            break;
                        case 400:
                            Alert.Instance.ShowTips(rs.message);
                            break;
                        case 404:
                            Alert.Instance.ShowTips(rs.message);
                            break;
                    }
                });
            });
            bg_Obj.transform.GetChild(1).GetChild(0).GetChild(2).GetComponent<Button>().onClick.AddListener(() =>
            {
                if (string.IsNullOrWhiteSpace(inputField_phoneNum.text) || inputField_phoneNum.text.Contains(" "))
                {
                    Alert.Instance.ShowTips("手机号为空或者含有空字符");
                    return;
                }
                if (!phoneNumRegex.IsMatch(inputField_phoneNum.text))
                {
                    Alert.Instance.ShowTips("请输入11位手机号数字");
                    return;
                }
                if (string.IsNullOrWhiteSpace(inputField_pwdyanzhengma.text) || inputField_pwdyanzhengma.text.Contains(" "))
                {
                    Alert.Instance.ShowTips("验证码为空或者含有空字符");
                    return;
                }
                var form = new List<IMultipartFormSection>
        {
            new MultipartFormDataSection("Phone", inputField_phoneNum.text),//手机号
            new MultipartFormDataSection("Code", inputField_pwdyanzhengma.text),//验证码
        };
                WebManager.Instance.GetStringFunc(Config.CodeAndPhoneNumberVerifition, form, delegate (string s)
                {
                    Debug.Log("验证手机号和验证码");
                    Debug.Log(s);
                    if (s.Length == 0)
                    {
                        Alert.Instance.ShowTips("手机号验证码验证失败");
                        return;
                    }
                    ReturnStateTeacher rs = JsonConvert.DeserializeObject<ReturnStateTeacher>(s);
                    switch (rs.code)
                    {
                        case 201:
                            bg_Obj.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
                            bg_Obj.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);//更改密码UI界面显示
                            id_user = rs.id;
                            Debug.Log(rs.message);//验证成功
                            break;
                        case 400:
                            Alert.Instance.ShowTips(rs.message);
                            Debug.Log(rs.message);
                            break;
                        case 401:
                            Alert.Instance.ShowTips(rs.message);
                            Debug.Log(rs.message);
                            break;
                        case 402:
                            Alert.Instance.ShowTips(rs.message);
                            Debug.Log(rs.message);
                            break;
                        case 404:
                            Alert.Instance.ShowTips(rs.message);
                            Debug.Log(rs.message);
                            break;
                    }
                });
            });
            bg_Obj.transform.GetChild(1).GetChild(1).GetChild(2).GetComponent<Button>().onClick.AddListener(() =>
            {
                if (string.IsNullOrWhiteSpace(inputField_newpassword.text) || inputField_newpassword.text.Contains(" "))
                {
                    Alert.Instance.ShowTips("新密码为空或者含有空字符");
                    return;
                }
                if (!mima.IsMatch(inputField_newpassword.text))
                {
                    Alert.Instance.ShowTips("新密码至少包含一个大写字母、一个小写字母、一个特殊字符，并且长度在8至20个字符");
                    return;
                }
                if (string.IsNullOrWhiteSpace(inputField_secondpassword.text) || inputField_secondpassword.text.Contains(" "))
                {
                    Alert.Instance.ShowTips("确认密码为空或者含有空字符");
                    return;
                }
                if (!mima.IsMatch(inputField_secondpassword.text))
                {
                    Alert.Instance.ShowTips("确认密码至少包含一个大写字母、一个小写字母、一个特殊字符，并且长度在8至20个字符");
                    return;
                }
                if (inputField_newpassword.text != inputField_secondpassword.text)
                {
                    tips_MiMaDiff.SetActive(true);
                    if (tips_MiMaDiff)
                    {
                        tips_MiMaDiff.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() =>
                        {
                            tips_MiMaDiff.SetActive(false);
                        });
                    }

                    return;
                }
                var form = new List<IMultipartFormSection>
        {
            new MultipartFormDataSection("newPassword", inputField_newpassword.text),//新密码
            new MultipartFormDataSection("employid", id_user.ToString()),//用户id
        };
                WebManager.Instance.GetStringFunc(Config.ChangePassword, form, delegate (string s)
                {
                    Debug.Log("修改密码");
                    Debug.Log(s);
                    if (s.Length == 0)
                    {
                        Alert.Instance.ShowTips("修改失败");
                        return;
                    }
                    ReturnStateTeacher rs = JsonConvert.DeserializeObject<ReturnStateTeacher>(s);
                    switch (rs.code)
                    {
                        case 201:
                            Alert.Instance.ShowTips(rs.message);
                            bg_Obj.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
                            bg_Obj.transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
                            bg_Obj.transform.GetChild(1).gameObject.SetActive(false);//更改密码UI界面隐藏
                            if (tips_MiMaDiff)
                            {
                                tips_MiMaDiff.SetActive(false);
                            }

                            bg_Obj.transform.GetChild(0).gameObject.SetActive(true);

                            Debug.Log(rs.message);//修改成功
                            break;
                        case 401:
                            Alert.Instance.ShowTips(rs.message);
                            Debug.Log(rs.message);
                            break;
                    }
                });
            });

        }
        IEnumerator Countdown()
        {
            while (timeLeft > 0)
            {
                countdownText.text = Mathf.Round(timeLeft).ToString();
                yield return new WaitForSeconds(1);
                timeLeft -= 1;
            }
            bg_Obj.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(2).gameObject.SetActive(false);
        }

        IEnumerator LoadSceneWithLoadingScreen()
        {
            // 显示加载界面
            PicLoading.SetActive(true);

            // 异步加载场景
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Teachersaomiao1080");

            // 等待场景加载完成
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            // 隐藏加载界面
            PicLoading.SetActive(false);
        }
    }
}


