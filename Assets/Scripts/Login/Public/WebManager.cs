using jxzt;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net; // TLS control
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using static jxzt.Info;

/// <summary>
/// Web 请求管理器
/// 封装 HTTP 请求逻辑，处理与后端 API 的通信
/// 管理会话 ID、超时检测、心跳保活等
/// </summary>
public class WebManager : MonoBehaviour
{
    private static WebManager instance;

    public static WebManager Instance { get => instance; }
    public Dictionary<string, string> headers = new Dictionary<string, string>();
    string sessionid;
    private float lastActivityTime;
    private const float TIMEOUT = 2 * 60f; // 25分钟定时时间 
    private const float TIMEOUT1 = 3 * 60f; // 25分钟超时时间

    // Certificate handler to bypass SSL validation (use only if server cert cannot be validated on client)
    private class BypassCertificateHandler : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true; // ACCEPT ALL (testing only)
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Ensure TLS 1.2 is enabled as early as possible
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
    }
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    //以下是获取string类型的方法 get
    public void GetStringFunc(string url, Action<string> stringAction)
    {        
        StartCoroutine(GetString(url, stringAction));
    }
    private IEnumerator GetString(string url, Action<string> stringAction)
    {
        string abc = "";
        UnityWebRequestEvent unityWebRequestEvent = (sender, args) =>
        {
            try { abc = args.UnityWebRequestResult.downloadHandler.text; } catch { abc = string.Empty; }
        };
        yield return StartCoroutine(CommitToServerByGetMethod(url, unityWebRequestEvent));
        stringAction(abc);
    }


    //以下是获取string类型的方法 post
    public void GetStringFunc(string url, List<IMultipartFormSection> wwwForm, Action<string> stringAction)// List<IMultipartFormSection>
    {
        StartCoroutine(GetString(url, wwwForm, stringAction));
    }
    public void GetStringFunc(string url, List<IMultipartFormSection> wwwForm, string username, string passWord, Action<string> stringAction)// List<IMultipartFormSection>
    {
        StartCoroutine(GetString(url, wwwForm, stringAction));
    }
    public void GetStringFunc(string url, WWWForm wwwForm, Action<string> stringAction, string authorization = null)// List<IMultipartFormSection>
    {
        StartCoroutine(GetString(url, wwwForm, stringAction,authorization));
    }

    public void GetStringFunc(string url, object body, Action<string> stringAction, string authorization = null) {
        StartCoroutine(GetStringByJson(url, body, stringAction,authorization));
    }

    private IEnumerator GetStringByJson(string url, object body, Action<string> stringAction, string authorization = null) {
        string resp = string.Empty;
        yield return CommunicateToServerByPostMethodJson(url, JsonConvert.SerializeObject(body), (s, args) => {
            try { resp = args.UnityWebRequestResult.downloadHandler.text; } catch { resp = string.Empty; }
        },authorization);
        stringAction?.Invoke(resp);
    }


    private IEnumerator GetString(string url, WWWForm wwwForm, Action<string> stringAction, string authorization = null)//List<IMultipartFormSection>
    {
        string abc = "";

        UnityWebRequestEvent unityWebRequestEvent = (sender, args) =>
        {
            try { abc = args.UnityWebRequestResult.downloadHandler.text; } catch { abc = string.Empty; }
        };

        yield return CommunicateToServerByPostMethodWithImage(url, wwwForm, unityWebRequestEvent, authorization);//
        stringAction(abc);
    }
    private IEnumerator GetString(string url, List<IMultipartFormSection> wwwForm, Action<string> stringAction)//List<IMultipartFormSection>
    {
        string abc = "";

        UnityWebRequestEvent unityWebRequestEvent = (sender, args) =>
        {
            try { abc = args.UnityWebRequestResult.downloadHandler.text; } catch { abc = string.Empty; }
        };

        yield return StartCoroutine(CommunicateToServerByPostMethod(url, wwwForm, unityWebRequestEvent));
        stringAction(abc);
    }
    //以下是获取图片的方法
    public void GetTextureFunc(string url, Action<Texture2D> textureAction)
    {
        StartCoroutine(GetTexture(url, textureAction));
    }

    private IEnumerator GetTexture(string url, Action<Texture2D> textureAction)
    {
        Texture2D texture2D = new Texture2D(0, 0);
        UnityWebRequestEvent unityWebRequestEvent = (sender, args) =>
        {
            texture2D = ((DownloadHandlerTexture)args.UnityWebRequestResult.downloadHandler).texture;
        };
        yield return StartCoroutine(CommitToServerByGetMethodIcon(url, unityWebRequestEvent));
        textureAction(texture2D);
    }
    private IEnumerator CommitToServerByGetMethodIcon(string url, UnityWebRequestEvent unityWebRequestEvent)
    {
        int retries = 2;
        while (true)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            www.certificateHandler = new BypassCertificateHandler();
            www.disposeCertificateHandlerOnDispose = true;
            www.timeout = 60;
            // 可选：为 GET 图片也附带鉴权
            string id = PlayerPrefs.GetString("sessionid");
            if (!string.IsNullOrEmpty(id))
            {
                www.SetRequestHeader("Authorization", "Bearer " + id);
            }
            yield return www.SendWebRequest();
            if (www.isDone && www.error == null)
            {
                unityWebRequestEvent(this, new UnityWebRequestEventArgs(www));
                www.Dispose();
                yield break;
            }
            else
            {
                string err = www.error;
                string body = www.downloadHandler != null ? www.downloadHandler.text : string.Empty;
                Debug.Log(body);
                Debug.Log(err);
                www.Dispose();
                if (retries-- > 0 && !string.IsNullOrEmpty(err)) { yield return new WaitForSeconds(0.5f); continue; }
                else { yield break; }
            }
        }
    }


    public delegate void UnityWebRequestEvent(object sender, UnityWebRequestEventArgs args);

    public class UnityWebRequestEventArgs
    {
        public UnityWebRequest UnityWebRequestResult;

        public UnityWebRequestEventArgs(UnityWebRequest unityWebRequestResult)
        {
            UnityWebRequestResult = unityWebRequestResult;
        }
    }


    private IEnumerator CommitToServerByGetMethod(string url, UnityWebRequestEvent unityWebRequestEvent)
    {
        int retries = 2;
        while (true)
        {
            UnityWebRequest www = UnityWebRequest.Get(url);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.certificateHandler = new BypassCertificateHandler();
            www.disposeCertificateHandlerOnDispose = true;
            www.timeout = 300; // add timeout to avoid hanging
            // 附带鉴权（若存在）
            string id = PlayerPrefs.GetString("sessionid");
            if (!string.IsNullOrEmpty(id))
            {
                www.SetRequestHeader("Authorization", "Bearer " + id);
            }

            yield return www.SendWebRequest();

            if (www.isDone && www.error == null)
            {
                unityWebRequestEvent(this, new UnityWebRequestEventArgs(www));
                www.Dispose();
                yield break;
            }
            else
            {
                string err = www.error;
                string body = www.downloadHandler != null ? www.downloadHandler.text : string.Empty;
                Debug.Log(body);
                Debug.Log(err);
                www.Dispose();
                // Retry on transient curl errors (e.g., 18) once or twice
                if (retries-- > 0 && !string.IsNullOrEmpty(err) && err.Contains("Curl error"))
                {
                    yield return new WaitForSeconds(0.5f);
                    continue;
                }
                else
                {
                    yield break;
                }
            }
        }
    }

    private IEnumerator CommunicateToServerByPostMethod(string url, List<IMultipartFormSection> wwwForm, UnityWebRequestEvent unityWebRequestEvent)
    {
        UnityWebRequest www = UnityWebRequest.Post(url, wwwForm);
        www.certificateHandler = new BypassCertificateHandler();
        www.disposeCertificateHandlerOnDispose = true;
        www.timeout = 60;

        yield return www.SendWebRequest();

        if (www.isDone && www.error == null)
        {

            // 获取session id
            sessionid = www.GetResponseHeader("Set-Cookie");
            if (sessionid != null)
            {
                string cookieValue = sessionid.Split(';')[0];
                sessionid = cookieValue.Split('=')[1];
                Debug.Log("Cookie值：" + cookieValue);
            }
            else
            {
                Debug.Log("服务器未设置'Set-Cookie'响应头");
            }


            headers["Cookie"] = sessionid;
            PlayerPrefs.SetString("sessionid", sessionid);
            unityWebRequestEvent(this, new UnityWebRequestEventArgs(www));
            www.Dispose();
        }
        else
        {
            Debug.Log(www.downloadHandler != null ? www.downloadHandler.text : string.Empty);
            Debug.Log(www.error);
            www.Dispose();
        }
    }

    private IEnumerator CommunicateToServerByPostMethodJson(string url, string jsonBody, UnityWebRequestEvent unityWebRequestEvent,string authorization = null) {
        byte[] bodyRaw = string.IsNullOrEmpty(jsonBody) ? Array.Empty<byte>() : System.Text.Encoding.UTF8.GetBytes(jsonBody);
        UnityWebRequest www = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        www.certificateHandler = new BypassCertificateHandler();
        www.disposeCertificateHandlerOnDispose = true;
        www.timeout = 60;

        string id = PlayerPrefs.GetString("sessionid");
        if (!string.IsNullOrEmpty(id)) {
            www.SetRequestHeader("Authorization", "Bearer " + id);
        }
        if (!string.IsNullOrEmpty(authorization)) { 
            www.SetRequestHeader("Authorization", "Bearer " + authorization);
        }

        yield return www.SendWebRequest();

        if (www.isDone && www.error == null) {
            unityWebRequestEvent(this, new UnityWebRequestEventArgs(www));
            www.Dispose();
        }
        else {
            Debug.Log(www.downloadHandler != null ? www.downloadHandler.text : string.Empty);
            Debug.Log(www.error);
            www.Dispose();
        }
    }

    private IEnumerator CommunicateToServerByPostMethodWithImage(string url, WWWForm wwwForm, UnityWebRequestEvent unityWebRequestEvent,string authorization = null)//List<IMultipartFormSection>
    {
        UnityWebRequest www = UnityWebRequest.Post(url, wwwForm);
        www.certificateHandler = new BypassCertificateHandler();
        www.disposeCertificateHandlerOnDispose = true;
        www.timeout = 120;
        string id = PlayerPrefs.GetString("sessionid");
        if (id != null)
        {
            www.SetRequestHeader("Authorization", "Bearer " + id);
            Debug.Log("session添加到头文件中");
        }
        else
        {
            Debug.Log("session为空");
        }
        if (!string.IsNullOrEmpty(authorization)) 
        {
            www.SetRequestHeader("Authorization", "Bearer " + authorization);
            Debug.Log("额外的Authorization添加到头文件中");
        }

        yield return www.SendWebRequest();

        if (www.isDone && www.error == null)
        {
            unityWebRequestEvent(this, new UnityWebRequestEventArgs(www));
            www.Dispose();
        }
        else
        {
            Debug.Log(www.downloadHandler != null ? www.downloadHandler.text : string.Empty);
            Debug.Log(www.error);
            www.Dispose();
        }
    }

    public void GetFileFunc(string url, FileStream fs, List<IMultipartFormSection> wwwForm, Action<double> processAction, Action completeAction)
    {
        StartCoroutine(DownloadFileToServerByGetMethod(url, fs, wwwForm, processAction, completeAction));
    }
    public IEnumerator DownloadFileToServerByGetMethod(string url, FileStream fs, List<IMultipartFormSection> wwwForm, Action<double> processAction, Action completeAction)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.certificateHandler = new BypassCertificateHandler();
        request.disposeCertificateHandlerOnDispose = true;
        request.timeout = 120;
        yield return request.SendWebRequest();
        if (request.isDone)
        {
            int packLength = 1024 * 20;
            byte[] data = request.downloadHandler.data;
            int nReadSize = 0;
            byte[] nbytes = new byte[packLength];
            using (Stream netStream = new MemoryStream(data))
            {
                nReadSize = netStream.Read(nbytes, 0, packLength);
                while (nReadSize > 0)
                {
                    fs.Write(nbytes, 0, nReadSize);
                    nReadSize = netStream.Read(nbytes, 0, packLength);
                    double dDownloadedLength = fs.Length * 1.0 / (1024 * 1024);
                    double dTotalLength = data.Length * 1.0 / (1024 * 1024);
                    processAction(dDownloadedLength);
                    yield return null;
                }
            }
        }
        completeAction();
    }

    public IEnumerator KeepLogin()
    {
        // 初始化最后活动时间
        lastActivityTime = Time.time;
        while (true)
        {            
            yield return new WaitForSeconds(TIMEOUT); // 每25min发送一次请求以保持登录状态           
            // 检查是否超时
            if (IsTimeout())
            {                
                // 超时退出
                Exit();
                break;
            }

            GetHartBeat();
        }
    }
    // 检查是否超时
    private bool IsTimeout()
    {
        return Time.time - lastActivityTime > TIMEOUT1;
    }
    void GetHartBeat()
    {        
        WebManager.Instance.GetStringFunc(Config.GetHartBeat, delegate (string s)
        {
            if (s.Length == 0)
            {
                Debug.Log("获取心跳失败");
            }
            response rs = JsonConvert.DeserializeObject<response>(s);

            switch (rs.code)
            {
                case 200:
                    Debug.Log("获取成功：" + rs.status);
                    // 初始化最后活动时间
                    lastActivityTime = Time.time;
                    break;
                case 400:
                    Debug.Log("获取失败：" + rs.status);
                    Exit();
                    break;
            }

        });
    }
    public void Exit()
    {
        int loginstate = 0;
        WebManager.Instance.GetStringFunc(Config.Exit, delegate (string s)
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
        StopCoroutine(KeepLogin());
        TeacherMainManager.instance.InitPanfen();
        SceneManager.LoadScene("Login");
    }


}
