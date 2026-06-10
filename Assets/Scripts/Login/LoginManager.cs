using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace jxzt
{
    /// <summary>
    /// 登录管理器（简化版）
    /// 处理用户注册和登录的UI交互，获取输入的账号密码信息
    /// 弹窗显示注册成功信息
    /// </summary>
    public class LoginManager : MonoBehaviour
    {
        public Text accountText;
        public Text passwordText;
        public Button registerBtn;
        public Button loginBtn;
        // Start is called before the first frame update
        void Start()
        {
            registerBtn.onClick.AddListener(RegisterClick);
            loginBtn.onClick.AddListener(LoginClick);
        }
        public void RegisterClick()
        {
            string data = "您的帐号是：" + accountText.text + "\n您的密码是：" + passwordText.text;
            GameObject.Find("Bg").GetComponent<PopupWindow>().ShowPopup(data);
            Debug.Log("注册成功");
        }
        public void LoginClick()
        {
            ToLogin(accountText.text, passwordText.text);
            GameObject.Find("Bg").GetComponent<PopupWindow>().ShowPopup("登录成功");
        }
        void ToLogin(string id, string passWord)
        {
            var form = new List<IMultipartFormSection>
        {
            new MultipartFormDataSection("CardID", id),//用户名
            new MultipartFormDataSection("Password",passWord),//密码
        };
            WebManager.Instance.GetStringFunc(Config.LoginServerAddress, form, delegate (string s)
            {
                Debug.Log("登录");
                Debug.Log(s);
                if (s.Length == 0)
                {
                    return;
                }
                SceneManager.LoadScene("TestList");

            });
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}

