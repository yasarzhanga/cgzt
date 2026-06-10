using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static jxzt.Info;
namespace jxzt
{
    /// <summary>
    /// 退出场景/返回登录
    /// 处理退出当前场景的逻辑，保存判分数据并返回登录界面
    /// 点击时调用退出接口，清理当前场景状态
    /// </summary>
    public class quit : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            transform.GetComponent<Button>().onClick.AddListener(() =>
            {
                Exit(Config.Exit);
                TeacherMainManager.instance.InitPanfen();                                                
                SceneManager.LoadScene("Login");                
            });
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
                        Alert.Instance.ShowTips("退出成功");
                        break;
                    case 400:
                        Alert.Instance.ShowTips("退出失败");
                        break;
                    case 401:
                        Alert.Instance.ShowTips("未登录");
                        break;
                }
           });           
            
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}

