using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net; // TLS
using System.Net.Security; // Cert validation
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using static jxzt.Info;
using static WebManager;

namespace jxzt
{
    /// <summary>
    /// OCR 光学字符识别管理器
    /// 集成百度OCR API，支持手写文字识别、姓名学号检测、题号识别等功能
    /// 用于从学生作业图片中提取文字信息，辅助判分和学号匹配
    /// </summary>
    public class OCR : MonoBehaviour
    {
        public List<PositionInt> namePositionInts = new List<PositionInt>();
        public List<PositionInt> numberPositionInts = new List<PositionInt>();
        public List<List<PositionInt>> picPositionInts = new List<List<PositionInt>>();
        public HandwritingOcr generalOcr;
        public List<string> dataFileList;
        public Dictionary<string, string> dictitleIdNumAndPageCodes = new();
        // 路径到你的txt文件
        private string fileName = "tihaoyema_zy.txt";
        float picWidth = 4480;// 4360;
        float picHeight = 2880;// 3040;

        // ========= Added: last detected image direction info =========
        // Baidu OCR 返回的图像方向（单位度，标准化为 0/90/180/270，顺时针更正角度）
        public int lastDirectionDegrees = 0;
        // 是否从最近一次 OCR 响应中成功解析出方向
        public bool hasDirection = false;
        // ========= End =========

        //以下信息于百度开发者中心创建应用获取
        private const string appID = "117708122";
        private const string apiKey = "Q38gMc6sOxcYKoMke3y09fsO";
        private const string secretKey = "wc6eriScGtDnW6O3v2r0Gzf4hN4NbZvj";
        private string access_token = "24.6f0f2d90ce4ade5d5d66972c877abffe.2592000.1745204298.282335-117708122";

        private string grantType = "client_credentials";

        private void Awake()
        {
            // 初始化 - 配置TLS协议和证书校验，确保与百度OCR服务器的HTTPS连接正常
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            }
            catch { }
            // 临时放宽证书校验，解决某些环境下根证书缺失导致的 UNITYTLS_INTERNAL_ERROR
            // 注意：生产环境应正确安装根证书并移除此回调！
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, errors) =>
                {
                    // 可选：只对百度域名放行
                    try
                    {
                        var req = sender as HttpWebRequest;
                        if (req != null)
                        {
                            var host = req.RequestUri.Host.ToLowerInvariant();
                            if (host.Contains("baidu") || host.Contains("baidubce")) return true;
                        }
                    }
                    catch { }
                    return true; // 最宽松（开发/联调用）
                };
            }
            catch { }
        }

        private void Start()
        {
            float drawWidth = 4480;//4360
            float drawHeight = 2880;//3040

            float scaleX = picWidth / drawWidth;
            float scaleY = picHeight / drawHeight;

            namePositionInts.Add(new PositionInt((int)((2549)), (int)((220))));
            namePositionInts.Add(new PositionInt((int)((2559)), (int)((264))));
            namePositionInts.Add(new PositionInt((int)((3202)), (int)((81))));
            namePositionInts.Add(new PositionInt((int)((3212)), (int)((125))));


            numberPositionInts.Add(new PositionInt((int)((3208)), (int)((237))));
            numberPositionInts.Add(new PositionInt((int)((3215)), (int)((274))));
            numberPositionInts.Add(new PositionInt((int)((3990)), (int)((92))));
            numberPositionInts.Add(new PositionInt((int)((3997)), (int)((129))));
        }

        /// <summary>
        /// 手写文字识别
        /// 调用百度OCR API识别图片中的手写文字，返回识别结果
        /// </summary>
        /// <param name="bytes">图片字节数据</param>
        /// <param name="language">识别语言类型 默认CHN_ENG中英文混合</param>
        /// <param name="detectDirection">是否检测图像朝向</param>
        /// <param name="probability">是否返回识别结果中每一行的置信度</param>
        /// <param name="detectLanguage">是否检测语言</param>
        /// <returns></returns>
        private bool CanResolveHost(string host)
        {
            try
            {
                System.Net.Dns.GetHostEntry(host);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[OCR] DNS解析失败: " + host + " => " + ex.Message);
                return false;
            }
        }

        public void GetHandwriting(byte[] bytes, string language = "CHN_ENG", bool detectDirection = false, bool probability = false, bool detectLanguage = false)
        {
            ScoringPerf.Start("OCR.Handwriting", $"bytes={(bytes == null ? 0 : bytes.Length)};language={language};detectDirection={detectDirection};probability={probability};detectLanguage={detectLanguage}");
            if (bytes == null || bytes.Length == 0)
            {
                Debug.LogWarning("[OCR] 输入图片为空，跳过识别");
                generalOcr = null; hasDirection = false; lastDirectionDegrees = 0; picPositionInts.Clear();
                ScoringPerf.End("OCR.Handwriting", "result=emptyInput");
                return;
            }
            // DNS预检，避免后续握手长时间阻塞
            if (!CanResolveHost("aip.baidubce.com"))
            {
                Debug.LogWarning("[OCR] 无法解析百度OCR域名，跳过识别并继续流程");
                generalOcr = null; hasDirection = false; lastDirectionDegrees = 0; picPositionInts.Clear();
                ScoringPerf.End("OCR.Handwriting", "result=dnsFailed");
                return;
            }
            var client = new Baidu.Aip.Ocr.Ocr(apiKey, secretKey);
            try { client.Timeout = 60000; } catch { }
            try
            {
                var options = new Dictionary<string, object>
                {
                    { "language_type", language },
                    { "detect_direction", detectDirection},
                    { "detect_language", detectLanguage},
                    { "probability", probability}
                };
                var response = client.Handwriting(bytes, options);
                if (response == null)
                {
                    Debug.LogWarning("[OCR] Handwriting response is null");
                    generalOcr = null; hasDirection = false; lastDirectionDegrees = 0; picPositionInts.Clear();
                    ScoringPerf.End("OCR.Handwriting", "result=nullResponse");
                    return;
                }
                ParseOcrResponse(response);
                ScoringPerf.End("OCR.Handwriting", $"result=ok;words={(generalOcr == null ? 0 : generalOcr.words_result_num)};direction={lastDirectionDegrees};hasDirection={hasDirection}");
            }
            catch (Exception error)
            {
                Debug.LogError("[OCR] Baidu SDK 调用失败，尝试 REST 降级: " + error.Message);
                if (error.ToString().Contains("SecureChannelFailure") || error.ToString().Contains("TLS") || error.ToString().Contains("Handshake failed") || error.ToString().Contains("NameResolutionFailure"))
                {
                    ScoringPerf.End("OCR.Handwriting", "result=sdkErrorFallback");
                    StartCoroutine(FallbackHandwriting(bytes, language, detectDirection, probability, detectLanguage));
                }
                else
                {
                    generalOcr = null; hasDirection = false; lastDirectionDegrees = 0; picPositionInts.Clear();
                    ScoringPerf.End("OCR.Handwriting", "result=sdkError");
                }
            }
        }

        /// <summary>
        /// 解析OCR响应结果
        /// 将百度OCR返回的JSON结果反序列化为HandwritingOcr对象，提取文字内容和位置信息
        /// </summary>
        /// <param name="response">百度OCR API返回的JSON对象</param>
        private void ParseOcrResponse(Newtonsoft.Json.Linq.JObject response)
        {
            try
            {
                generalOcr = JsonConvert.DeserializeObject<HandwritingOcr>(response.ToString());
                int wordsCount = 0; try { var wr = response["words_result"]; if (wr != null && wr.Type != JTokenType.Null) wordsCount = wr.Count(); } catch { }
                Debug.Log(response);
                if (generalOcr != null)
                {
                    Debug.Log("ocr返回数据结构" + generalOcr.words_result_num);
                    Debug.Log("ocr返回数据结构" + generalOcr.log_id);
                    Debug.Log("ocr-方向" + generalOcr.direction);
                }
                Debug.Log("ocr-words_result" + wordsCount);
                hasDirection = false; lastDirectionDegrees = 0;
                try
                {
                    JToken dirTok = response["direction"]; if (dirTok != null && dirTok.Type != JTokenType.Null)
                    {
                        int dirVal; if (!int.TryParse(dirTok.ToString(), out dirVal)) dirVal = 0;
                        if (dirVal == -1) { hasDirection = false; lastDirectionDegrees = 0; }
                        else if (dirVal >= 0 && dirVal <= 3) 
                        { 
                            lastDirectionDegrees = dirVal * 90; hasDirection = true; 
                        }
                        else if (dirVal == 0 || dirVal == 90 || dirVal == 180 || dirVal == 270) 
                        {
                            lastDirectionDegrees = dirVal; hasDirection = true; 
                        }
                        else 
                        { 
                            hasDirection = false; lastDirectionDegrees = 0;
                        }
                        Debug.Log($"[OCR] detect_direction(normalized): {lastDirectionDegrees}° (hasDirection={hasDirection})");
                    }
                }
                catch (Exception ex) { Debug.LogWarning("解析 OCR 方向失败:" + ex.Message); hasDirection = false; lastDirectionDegrees = 0; }
                picPositionInts.Clear();
                if (generalOcr == null || generalOcr.words_result == null || generalOcr.words_result.Length == 0) { Debug.LogWarning("[OCR] words_result is null or empty"); return; }
                for (int i = 0; i < generalOcr.words_result.Length; i++)
                {
                    var item = generalOcr.words_result[i]; if (item == null || item.location == null) continue;
                    Vector2 pos0 = new Vector2(item.location.left, item.location.top);
                    Vector2 pos1 = new Vector2(item.location.left + item.location.width, item.location.top);
                    Vector2 pos2 = new Vector2(item.location.left + item.location.width, item.location.top + item.location.height);
                    Vector2 pos3 = new Vector2(item.location.left, item.location.top + item.location.height);
                    List<PositionInt> positionInt = new List<PositionInt> { new PositionInt((int)pos0.x, (int)(picHeight - pos0.y)), new PositionInt((int)pos1.x, (int)(picHeight - pos1.y)), new PositionInt((int)pos2.x, (int)(picHeight - pos2.y)), new PositionInt((int)pos3.x, (int)(picHeight - pos3.y)) };
                    picPositionInts.Add(positionInt);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("[OCR] 解析响应失败:" + ex.Message); generalOcr = null; hasDirection = false; lastDirectionDegrees = 0; picPositionInts.Clear();
            }
        }

        private class AllowAllCertsHandler : CertificateHandler { protected override bool ValidateCertificate(byte[] certificateData) { return true; } }

        private IEnumerator FallbackHandwriting(byte[] bytes, string language, bool detectDirection, bool probability, bool detectLanguage)
        {
            if (bytes == null || bytes.Length == 0) yield break;
            string host = "aip.baidubce.com";
            if (!CanResolveHost(host))
            {
                Debug.LogError("[OCR Fallback] DNS仍无法解析，终止降级请求");
                yield break;
            }
            string url = "https://" + host + "/rest/2.0/ocr/v1/handwriting";
            string base64 = Convert.ToBase64String(bytes);
            WWWForm form = new WWWForm();
            form.AddField("image", base64);
            form.AddField("language_type", language);
            form.AddField("detect_direction", detectDirection ? "true" : "false");
            form.AddField("probability", probability ? "true" : "false");
            if (detectLanguage) form.AddField("detect_language", "true");
            string finalUrl = url + "?access_token=" + access_token;

            int attempts = 2;
            while (attempts-- > 0)
                using (UnityWebRequest req = UnityWebRequest.Post(finalUrl, form))
                {
                    req.timeout = 60;
                    req.certificateHandler = new AllowAllCertsHandler();
                    yield return req.SendWebRequest();
                    if (req.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError("[OCR Fallback] 请求失败(" + (2 - attempts) + "/2):" + req.error);
                        if (attempts <= 0)
                        {
                            generalOcr = null; hasDirection = false; lastDirectionDegrees = 0; picPositionInts.Clear();
                        }
                        else
                        {
                            yield return new WaitForSecondsRealtime(0.5f);
                        }
                    }
                    else
                    {
                        try
                        {
                            var json = req.downloadHandler.text;
                            var jobt = JObject.Parse(json);
                            ParseOcrResponse(jobt);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError("[OCR Fallback] 解析失败:" + ex.Message); generalOcr = null; hasDirection = false; lastDirectionDegrees = 0; picPositionInts.Clear();
                        }
                        yield break; // 成功直接退出
                    }
                }
        }

        // ========= Baidu QR Code recognition via REST API (UnityWebRequest only) =========
        private string cachedAccessToken = string.Empty;
        private DateTime accessTokenExpireAt = DateTime.MinValue;
        // 当前是否有二维码识别请求在进行中（供外部轮询或阻塞等待）
        public bool IsQRCodeRequestRunning { get; private set; } = false;

        private IEnumerator FetchAccessToken(Action<string> onDone)
        {
            string host = "aip.baidubce.com";
            if (!CanResolveHost(host)) { Debug.LogError("[OCR QR] 无法解析域名，放弃获取access_token"); onDone?.Invoke(string.Empty); yield break; }
            string url = "https://" + host + "/oauth/2.0/token";
            WWWForm form = new WWWForm();
            form.AddField("grant_type", "client_credentials");
            form.AddField("client_id", apiKey);
            form.AddField("client_secret", secretKey);
            using (UnityWebRequest req = UnityWebRequest.Post(url, form))
            {
                req.timeout = 30;
                req.certificateHandler = new AllowAllCertsHandler();
                yield return req.SendWebRequest();
                if (req.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("[OCR QR] 获取access_token失败:" + req.error);
                    onDone?.Invoke(string.Empty);
                }
                else
                {
                    try
                    {
                        var json = req.downloadHandler.text;
                        var jobj = JObject.Parse(json);
                        var token = jobj.Value<string>("access_token");
                        var expiresIn = jobj.Value<int>("expires_in");
                        cachedAccessToken = token;
                        accessTokenExpireAt = DateTime.UtcNow.AddSeconds(Math.Max(0, expiresIn - 60));
                        onDone?.Invoke(cachedAccessToken);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("[OCR QR] 解析access_token失败:" + ex.Message);
                        onDone?.Invoke(string.Empty);
                    }
                }
            }
        }

        private bool IsAccessTokenValid()
        {
            return !string.IsNullOrEmpty(cachedAccessToken) && DateTime.UtcNow < accessTokenExpireAt;
        }

        public IEnumerator RecognizeQRCodeByUrl(string imageUrl, bool includeLocation, Action<string> onJson)
        {
            if (!IsAccessTokenValid())
            {
                yield return FetchAccessToken(_ => { });
            }
            if (!IsAccessTokenValid())
            {
                onJson?.Invoke(string.Empty); yield break;
            }
            string host = "aip.baidubce.com";
            if (!CanResolveHost(host)) { Debug.LogError("[OCR QR] 无法解析域名"); onJson?.Invoke(string.Empty); yield break; }
            string url = $"https://{host}/rest/2.0/ocr/v1/qrcode?access_token={cachedAccessToken}";
            WWWForm form = new WWWForm();
            form.AddField("url", imageUrl);
            form.AddField("location", includeLocation ? "true" : "false");
            using (UnityWebRequest req = UnityWebRequest.Post(url, form))
            {
                req.timeout = 30;
                req.certificateHandler = new AllowAllCertsHandler();
                yield return req.SendWebRequest();
                if (req.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("[OCR QR] 识别失败:" + req.error);
                    onJson?.Invoke(string.Empty);
                }
                else
                {
                    onJson?.Invoke(req.downloadHandler.text);
                }
            }
        }

        public IEnumerator RecognizeQRCodeByBytes(byte[] imageBytes, bool includeLocation, Action<string> onJson)
        {
            Debug.Log("ZXing 未识别到二维码，使用百度OCR二维码接口兜底识别2");
            if (!IsAccessTokenValid())
            {
                yield return FetchAccessToken(_ => { });
            }
            if (!IsAccessTokenValid())
            {
                onJson?.Invoke(string.Empty);
                yield break;
            }
            Debug.Log("ZXing 未识别到二维码，使用百度OCR二维码接口兜底识别3");
            string host = "aip.baidubce.com";
            if (!CanResolveHost(host)) { Debug.LogError("[OCR QR] 无法解析域名"); onJson?.Invoke(string.Empty); IsQRCodeRequestRunning = false; yield break; }
            string url = $"https://{host}/rest/2.0/ocr/v1/qrcode?access_token={cachedAccessToken}";
            Debug.Log("ZXing 未识别到二维码，使用百度OCR二维码接口兜底识别3.5" + url);
            string base64 = Convert.ToBase64String(imageBytes);
            WWWForm form = new WWWForm();
            form.AddField("image", base64);
            form.AddField("location", includeLocation ? "true" : "false");
            using (UnityWebRequest req = UnityWebRequest.Post(url, form))
            {
                Debug.Log("ZXing 未识别到二维码，使用百度OCR二维码接口兜底识别4");
                req.timeout = 30;
                req.certificateHandler = new AllowAllCertsHandler();
                yield return req.SendWebRequest();
                if (req.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("[OCR QR] 识别失败:" + req.error);
                    onJson?.Invoke(string.Empty);
                }
                else
                {
                    onJson?.Invoke(req.downloadHandler.text);
                    Debug.Log("ZXing 未识别到二维码，使用百度OCR二维码接口兜底识别5" + req.downloadHandler.text);
                    string results = req.downloadHandler.text;
                    Debug.Log("ZXing 未识别到二维码，使用百度OCR二维码接口兜底识别6" + results);
                    try
                    {
                        if (true)//ocr.IsQRCodeRequestRunning == false
                        {
                            Debug.Log("ZXing 未识别到二维码，使用百度OCR二维码接口兜底识别7" + results);
                            if (results != null)
                            {
                                Debug.Log("ZXing 未识别到二维码，使用百度OCR二维码接口兜底识别8" + results);
                                // 修复：使用 JObject 解析，兼容 text 为字符串数组或字符串
                                var jo = JObject.Parse(results);
                                var arr = jo["codes_result"] as JArray;
                                int resultNum = jo.Value<int?>("codes_result_num") ?? (arr?.Count ?? 0);
                                Debug.Log("ZXing 未识别到二维码，使用百度OCR二维码接口兜底识别10 resultNum=" + resultNum);
                                List<string> qrCodeNumsFromPic = new List<string>();
                                if (resultNum>0&&arr != null)
                                {
                                    foreach (var item in arr)
                                    {
                                        var txtTok = item["text"];
                                        string txt = null;
                                        if (txtTok is JArray jarr && jarr.Count > 0)
                                        {
                                            txt = jarr[0]?.ToString();
                                        }
                                        else
                                        {
                                            txt = txtTok?.ToString();
                                        }
                                        if (!string.IsNullOrEmpty(txt))
                                        {
                                            Debug.Log("二维码ID" + txt);
                                            string pptid = TeacherMainManager.instance.cameraCompare_btn.transform.GetComponent<ZXingQRCodeWrapper_ScanQRCode>().GetPPTId_Fun(txt);
                                            qrCodeNumsFromPic.Add(pptid);
                                        }
                                    }
                                }
                                else
                                {
                                    Debug.Log("兜底识别失败");
                                    TeacherMainManager.instance.studentAnswerQRnum = 0;
                                    TeacherMainManager.instance.studentAnswerPageCodenum = 0;
                                    TeacherMainManager.instance._remainingQRCodeCount = 0;
                                    TeacherMainManager.instance._remainingPageCodeCount = 0;
                                    StartCoroutine(TeacherMainManager.instance.AegisAnimation(8)); 
                                }
                                if (qrCodeNumsFromPic.Count != 0 && TeacherMainManager.instance.cameraCompare_btn.transform.GetComponent<ZXingQRCodeWrapper_ScanQRCode>().AreListsEqual(qrCodeNumsFromPic, TeacherMainManager.instance.cameraCompare_btn.transform.GetComponent<ZXingQRCodeWrapper_ScanQRCode>().qrCodeNumsNoChoose))
                                {
                                    Debug.Log("The lists contain the same elements in different order.");

                                    Alert.Instance.Tips_Toggle.SetActive(false);
                                    Alert.Instance.text_tipsToggle.text = "";
                                    TeacherMainManager.instance.cameraCompare_btn.transform.GetComponent<ZXingQRCodeWrapper_ScanQRCode>()
                                        .SyncTeacherQRCodeSelection(TeacherMainManager.instance.cameraCompare_btn.transform.GetComponent<ZXingQRCodeWrapper_ScanQRCode>().qrCodeNums);
                                    Debug.Log("批量选择相同题目赋值" + TeacherMainManager.instance.cameraCompare_btn.transform.GetComponent<ZXingQRCodeWrapper_ScanQRCode>().qrCodeNums.Count);
                                    //开启协程
                                    StartCoroutine(TeacherMainManager.instance.AegisAnimation(3));
                                }
                                else
                                {
                                    Debug.Log("The lists do not contain the same elements.");
                                    TeacherMainManager.instance.cameraCompare_btn.transform.GetComponent<ZXingQRCodeWrapper_ScanQRCode>().qrCodeNums.Clear();
                                    TeacherMainManager.instance.cameraCompare_btn.transform.GetComponent<ZXingQRCodeWrapper_ScanQRCode>().qrCodeNumsNoChoose.Clear();
                                    TeacherMainManager.instance.cameraCompare_btn.transform.GetComponent<ZXingQRCodeWrapper_ScanQRCode>().qrCodeNumsNoChoose = qrCodeNumsFromPic;


                                    int childCount = Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(1).transform.childCount;
                                    if (childCount > 0)
                                    {
                                        for (int i = 0; i < childCount; i++)
                                        {
                                            Destroy(Alert.Instance.Tips_Toggle.transform.GetChild(0).GetChild(1).transform.GetChild(i).gameObject);
                                        }
                                    }
                                    StartCoroutine(TeacherMainManager.instance.cameraCompare_btn.transform.GetComponent<ZXingQRCodeWrapper_ScanQRCode>().ProcessAllQRCodes());

                                }
                            }
                            else
                            {
                                Debug.Log("兜底识别失败");
                                TeacherMainManager.instance.studentAnswerQRnum = 0;
                                TeacherMainManager.instance.studentAnswerPageCodenum = 0;
                                TeacherMainManager.instance._remainingQRCodeCount = 0;
                                TeacherMainManager.instance._remainingPageCodeCount = 0;
                                StartCoroutine(TeacherMainManager.instance.AegisAnimation(8));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning("[ScoringProcess2] OCR二维码兜底识别异常: " + ex.Message);
                        TeacherMainManager.instance.studentAnswerQRnum = 0;
                        TeacherMainManager.instance.studentAnswerPageCodenum = 0;
                        TeacherMainManager.instance._remainingQRCodeCount = 0;
                        TeacherMainManager.instance._remainingPageCodeCount = 0;
                        StartCoroutine(TeacherMainManager.instance.AegisAnimation(8));
                    }
                }

            }
        }

        // 阻塞式包装：等待二维码识别完成后再继续
        public IEnumerator RecognizeQRCodeAndWait(byte[] imageBytes, bool includeLocation, Action<List<string>> onCodes)
        {
            List<string> codes = null;
            yield return RecognizeQRCodeByBytes(imageBytes, includeLocation, json =>
            {
                codes = new List<string>();
                if (!string.IsNullOrEmpty(json))
                {
                    try
                    {
                        var jo = JObject.Parse(json);
                        var errCodeTok = jo["error_code"]; var errMsgTok = jo["error_msg"];
                        if (errCodeTok != null || errMsgTok != null)
                        {
                            Debug.LogWarning($"[OCR QR] 接口错误: code={errCodeTok}, msg={errMsgTok}");
                        }
                        var arr = jo["codes_result"] as JArray;
                        if (arr != null)
                        {
                            foreach (var codeItem in arr)
                            {
                                string txt = codeItem.Value<string>("text");
                                if (!string.IsNullOrEmpty(txt)) codes.Add(txt);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning("[OCR QR] 解析返回JSON失败:" + ex.Message);
                    }
                }
            });
            // 确保在请求完成后才触发回调
            onCodes?.Invoke(codes ?? new List<string>());
        }

        public IEnumerator RecognizeQRCodeSyncWait(byte[] imageBytes, bool includeLocation, Action<List<string>> onCodes)
        {
            List<string> codes = new List<string>();
            yield return RecognizeQRCodeByBytes(imageBytes, includeLocation, json =>
            {
                codes.Clear();
                if (!string.IsNullOrEmpty(json))
                {
                    try
                    {
                        var jo = JObject.Parse(json);
                        var arr = jo["codes_result"] as JArray;
                        if (arr != null)
                        {
                            foreach (var codeItem in arr)
                            {
                                string txt = codeItem.Value<string>("text");
                                if (!string.IsNullOrEmpty(txt)) codes.Add(txt);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning("[OCR QR] 解析返回JSON失败:" + ex.Message);
                    }
                }
            });
            onCodes?.Invoke(codes);
        }

        public void RecognizeQRCodeSync(byte[] imageBytes, bool includeLocation, Action<string> onCodes)
        {
            // 保留原始异步风格（回调式），不阻塞当前逻辑
            StartCoroutine(RecognizeQRCodeByBytes(imageBytes, includeLocation, json =>
            {
                onCodes?.Invoke(json);
            }));
        }

        // 在类中添加一个工具方法
        private static string NormalizePageCode(string code)
        {
            if (string.IsNullOrEmpty(code)) return string.Empty;
            // 提取连续数字（如果包含小数也需要，可改为允许'.'）
            var digits = new string(code.Where(char.IsDigit).ToArray());
            return digits;
        }


        public static string ConvertBytesToBase64(byte[] bytes)
        {
            if (bytes == null)
            {
                Debug.LogError("bytes对象为空");
                return string.Empty;
            }

            try
            {
                return Convert.ToBase64String(bytes);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"转换bytes为Base64时出错: {ex.Message}");
                return string.Empty;
            }
        }


        /**
       * 获取文件base64编码
       * @param path 文件路径
       * @return base64编码信息，不带文件头
       */
        static string GetFileContentAsBase64(string path)
        {
            using (FileStream filestream = new FileStream(path, FileMode.Open))
            {
                byte[] arr = new byte[filestream.Length];
                filestream.Read(arr, 0, (int)filestream.Length);
                string base64 = Convert.ToBase64String(arr);
                return base64;
            }
        }
        public static string ParseQueryString(Dictionary<string, string> querys)
        {
            if (querys.Count == 0)
            {
                return "";
            }
            return (from pair in querys
                    select pair.Key + "=" + pair.Value).Aggregate((string a, string b) => a + "&" + b);
        }

        public string GetOcrNameAndNumberStr(bool isOnlyGetTitle = false)
        {
            fileName = "tihaoyema_" + TeacherMainManager.instance.config.sign + ".txt";
            string studentMessage = "";

            string string_name = "";
            string string_num = "";

            // 保护性检查：无 OCR 结果直接返回空
            if (generalOcr == null || generalOcr.words_result == null || generalOcr.words_result.Length == 0)
            {
                Debug.LogWarning("[OCR] GetOcrNameAndNumberStr called with empty OCR result");
                return studentMessage;
            }
            // 安全读取工具
            string SafeWordAt(int idx)
            {
                if (idx >= 0 && idx < generalOcr.words_result.Length)
                {
                    var item = generalOcr.words_result[idx];
                    return item != null ? (item.words ?? string.Empty) : string.Empty;
                }
                return string.Empty;
            }
            // 保护取值
            string lastTest = SafeWordAt(generalOcr.words_result.Length - 1);
            PicProgress.instance.ocrPageCode = lastTest;
            Debug.Log("文字识别页码" + PicProgress.instance.ocrPageCode);
            // 匹配整数或浮点数（包括负数）
            string patternTest = @"^-?\d+(\.\d+)?$";
            if (string.IsNullOrEmpty(PicProgress.instance.ocrPageCode) || !Regex.IsMatch(PicProgress.instance.ocrPageCode, patternTest)) {
                string try2Test = SafeWordAt(generalOcr.words_result.Length - 2);
                if (!string.IsNullOrEmpty(try2Test) && Regex.IsMatch(try2Test, patternTest)) {
                    PicProgress.instance.ocrPageCode = try2Test;
                    Debug.Log("文字识别页码1" + PicProgress.instance.ocrPageCode);
                }
                else {
                    string try3Test = SafeWordAt(generalOcr.words_result.Length - 3);
                    if (!string.IsNullOrEmpty(try3Test) && Regex.IsMatch(try3Test, patternTest)) {
                        PicProgress.instance.ocrPageCode = try3Test;
                        Debug.Log("文字识别页码2" + PicProgress.instance.ocrPageCode);
                    }
                }
            }
            if (!isOnlyGetTitle)
            {
                for (int i = 0; i < generalOcr.words_result.Length; i++)
                {
                    var current = generalOcr.words_result[i];
                    if (current == null || string.IsNullOrEmpty(current.words)) continue;

                    if (current.words.Contains("姓名"))
                    {
                        string[] parts = current.words.Split('名');
                        if (parts.Length > 1 && !string.IsNullOrEmpty(parts[1]))
                        {
                            if (!parts[1].Contains("学号"))
                            {
                                string_name = parts[1];
                                Debug.Log("文字识别姓名获取1" + string_name);
                            }
                            else
                            {
                                string[] parts1 = parts[1].Split('学');
                                string_name = parts1.Length > 0 ? parts1[0] : string.Empty;
                                Debug.Log("文字识别姓名遍历获取Contains(\"学号\")1" + string_name);
                            }
                        }
                        else
                        {
                            var nextWords = SafeWordAt(i + 1);
                            if (!string.IsNullOrEmpty(nextWords) && !nextWords.Contains("学号"))
                            {
                                string_name = nextWords;
                                Debug.Log("文字识别姓名遍历获取2" + string_name);
                            }
                            else if (!string.IsNullOrEmpty(nextWords))
                            {
                                string[] parts1 = nextWords.Split('学');
                                string_name = parts1.Length > 0 ? parts1[0] : string.Empty;
                                Debug.Log("文字识别姓名遍历获取Contains(\"学号\")2" + string_name);
                            }
                        }

                    }
                    if (current.words.Contains("学号"))
                    {
                        string[] parts = current.words.Split('号');
                        if (parts.Length > 1 && !string.IsNullOrEmpty(parts[1]))
                        {
                            string_num = parts[1];
                            Debug.Log("文字识别学号获取1" + string_num);
                        }
                        else
                        {
                            string_num = SafeWordAt(i + 1);
                            Debug.Log("文字识别学号获取2" + string_num);
                        }

                    }
                    if (TeacherMainManager.instance.config.sign == "pb") {
                        if (current.words.Contains("班级")) { 
                            string[] parts = current.words.Split('级');
                            string pattern = @"-?\d+(\.\d+)?";
                            if (parts.Length > 1 && !string.IsNullOrEmpty(parts[1])) {
                                var match = Regex.Match(parts[1], pattern);
                                if (!string.IsNullOrEmpty(match.Value)) { 
                                    string_num = match.Value;
                                    Debug.Log("pb班级转学号1" + string_num);
                                }
                            }
                            else {
                                var nextWords = SafeWordAt(i + 1);
                                var match = Regex.Match(nextWords, pattern);
                                if (!string.IsNullOrEmpty(match.Value)) {
                                    string_num = match.Value;
                                    Debug.Log("pb班级转学号2" + string_num);
                                }
                            }
                        }
                    }

                }
                if (!string.IsNullOrEmpty(string_name) && !string.IsNullOrEmpty(string_num))
                {
                    string_num = new string(string_num.Where(c => char.IsDigit(c)).ToArray());
                    studentMessage = string_name + "-" + string_num;
                    Debug.Log("文字识别位置获取姓名学号" + studentMessage);
                }
                else
                {
                    Debug.Log("文字识别位置未获取姓名学号");
                }
            }
            else
            {
                if (TeacherMainManager.instance.width == 4480 && TeacherMainManager.instance.height == 2880)
                {
                    // 保护取值
                    string last = SafeWordAt(generalOcr.words_result.Length - 1);
                    PicProgress.instance.ocrPageCode = last;
                    Debug.Log("文字识别页码" + PicProgress.instance.ocrPageCode);
                    // 匹配整数或浮点数（包括负数）
                    string pattern = @"^-?\d+(\.\d+)?$";
                    if (string.IsNullOrEmpty(PicProgress.instance.ocrPageCode) || !Regex.IsMatch(PicProgress.instance.ocrPageCode, pattern))
                    {
                        string try2 = SafeWordAt(generalOcr.words_result.Length - 2);
                        if (!string.IsNullOrEmpty(try2) && Regex.IsMatch(try2, pattern))
                        {
                            PicProgress.instance.ocrPageCode = try2;
                            Debug.Log("文字识别页码1" + PicProgress.instance.ocrPageCode);
                        }
                        else
                        {
                            string try3 = SafeWordAt(generalOcr.words_result.Length - 3);
                            if (!string.IsNullOrEmpty(try3) && Regex.IsMatch(try3, pattern))
                            {
                                PicProgress.instance.ocrPageCode = try3;
                                Debug.Log("文字识别页码2" + PicProgress.instance.ocrPageCode);
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(PicProgress.instance.ocrPageCode))
                    {
                        ReadConfigFile(Application.streamingAssetsPath + "/Config/" + fileName);
                        TeacherMainManager.instance.titleIdNums.Clear();
                        TeacherMainManager.instance.studentAnswerQRnum = -1;
                        TeacherMainManager.instance.studentAnswerPageCodenum = -1;
                        if (dictitleIdNumAndPageCodes.Count > 0)
                        {
                            foreach (var item in dictitleIdNumAndPageCodes)
                            {
                                if (PicProgress.instance.ocrPageCode == item.Value)
                                {
                                    TeacherMainManager.instance.titleIdNums.Add(item.Key);
                                    Debug.Log("ocr页码题号选择" + item.Key);
                                }
                            }
                            Debug.Log("ocr页码题号titleIdNums.Count" + TeacherMainManager.instance.titleIdNums.Count);
                            TeacherMainManager.instance.studentAnswerPageCodenum = TeacherMainManager.instance.titleIdNums.Count;
                        }
                    }
                }
                else if (TeacherMainManager.instance.width == 4360 && TeacherMainManager.instance.height == 3040)
                {
                    // 保护取值
                    string last = SafeWordAt(generalOcr.words_result.Length - 1);
                    PicProgress.instance.ocrPageCode = last;
                    Debug.Log("文字识别页码" + PicProgress.instance.ocrPageCode);
                    // 匹配整数或浮点数（包括负数）
                    string pattern = @"^-?\d+(\.\d+)?$";
                    if (string.IsNullOrEmpty(PicProgress.instance.ocrPageCode) || !Regex.IsMatch(PicProgress.instance.ocrPageCode, pattern))
                    {
                        string try2 = SafeWordAt(generalOcr.words_result.Length - 2);
                        if (!string.IsNullOrEmpty(try2) && Regex.IsMatch(try2, pattern))
                        {
                            PicProgress.instance.ocrPageCode = try2;
                            Debug.Log("文字识别页码1" + PicProgress.instance.ocrPageCode);
                        }
                        else
                        {
                            string try3 = SafeWordAt(generalOcr.words_result.Length - 3);
                            if (!string.IsNullOrEmpty(try3) && Regex.IsMatch(try3, pattern))
                            {
                                PicProgress.instance.ocrPageCode = try3;
                                Debug.Log("文字识别页码2" + PicProgress.instance.ocrPageCode);
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(PicProgress.instance.ocrPageCode) && Regex.IsMatch(PicProgress.instance.ocrPageCode, pattern))
                    {
                        ReadConfigFile(Application.streamingAssetsPath + "/Config/" + fileName);
                        TeacherMainManager.instance.titleIdNums.Clear();
                        TeacherMainManager.instance.studentAnswerQRnum = -1;
                        TeacherMainManager.instance.studentAnswerPageCodenum = -1;
                        if (dictitleIdNumAndPageCodes.Count > 0)
                        {
                            foreach (var item in dictitleIdNumAndPageCodes)
                            {
                                if (PicProgress.instance.ocrPageCode == item.Value)
                                {
                                    TeacherMainManager.instance.titleIdNums.Add(item.Key);
                                    Debug.Log("ocr页码题号选择" + item.Key);
                                }
                            }
                            Debug.Log("ocr页码题号titleIdNums.Count" + TeacherMainManager.instance.titleIdNums.Count);
                            TeacherMainManager.instance.studentAnswerPageCodenum = TeacherMainManager.instance.titleIdNums.Count;
                        }
                    }
                    else
                    {
                        PicProgress.instance.ocrPageCode = "";
                    }

                }
                else if (TeacherMainManager.instance.width == 4290 && TeacherMainManager.instance.height == 2860)
                {
                    if (TeacherMainManager.instance.config.sign == "pb") {
                        int classWorldIndex = -1;
                        for (int i = generalOcr.words_result.Length - 1; i >= 0; i--) {
                            string currentWord = SafeWordAt(i);
                            Debug.Log("倒序检查OCR结果: " + currentWord);
                            if (!string.IsNullOrEmpty(currentWord)) {
                                //匹配班级文字
                                if (currentWord.Contains("班级")) {
                                    classWorldIndex = i;
                                    Debug.Log("找到班级相关文字: " + currentWord + " at index " + i);
                                    break;
                                }
                            }
                        }
                        if (classWorldIndex != -1) {
                            string pageWorld = SafeWordAt(classWorldIndex - 1);
                            string pattern = @"^-?\d+(\.\d+)?$";
                            if (!string.IsNullOrEmpty(pageWorld) && Regex.IsMatch(pageWorld, pattern)) {
                                PicProgress.instance.ocrPageCode = pageWorld;
                                Debug.Log("文字识别页码（基于班级定位）" + PicProgress.instance.ocrPageCode);
                            }
                        }
                    }
                    else {
                        for (int i = 1; i <= 5; i++) { 
                            string currentWord = SafeWordAt(generalOcr.words_result.Length - i);
                            Debug.Log("倒序检查OCR结果: " + currentWord);
                            string pattern = @"\d+";
                            var match = Regex.Match(currentWord, pattern);
                            Debug.Log("倒序检查OCR结果匹配: " + match.Value);
                            if (!string.IsNullOrEmpty(match.Value)) {
                                PicProgress.instance.ocrPageCode = match.Value;
                                Debug.Log("文字识别页码（基于倒序检查）" + PicProgress.instance.ocrPageCode);
                                break;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(PicProgress.instance.ocrPageCode))
                    {
                        // 统一规范化：去掉两侧点，仅保留中间数字
                        PicProgress.instance.ocrPageCode = NormalizePageCode(PicProgress.instance.ocrPageCode);

                        if (!string.IsNullOrEmpty(PicProgress.instance.ocrPageCode))
                        {
                            ReadConfigFile1(Application.streamingAssetsPath + "/Config/" + fileName);
                            TeacherMainManager.instance.titleIdNums.Clear();
                            TeacherMainManager.instance.studentAnswerQRnum = -1;
                            TeacherMainManager.instance.studentAnswerPageCodenum = -1;
                            if (dictitleIdNumAndPageCodes.Count > 0)
                            {
                                foreach (var item in dictitleIdNumAndPageCodes)
                                {
                                    if (PicProgress.instance.ocrPageCode == item.Value)
                                    {
                                        TeacherMainManager.instance.titleIdNums.Add(item.Key);
                                        Debug.Log("ocr页码题号选择" + item.Key);
                                    }
                                }
                                Debug.Log("ocr页码题号titleIdNums.Count" + TeacherMainManager.instance.titleIdNums.Count);
                                TeacherMainManager.instance.studentAnswerPageCodenum = TeacherMainManager.instance.titleIdNums.Count;
                            }
                        }
                        else
                        {
                            PicProgress.instance.ocrPageCode = "";
                        }
                    }
                }
            }

            return studentMessage;
        }

        public List<string> GetDetectedTitleIdsFromOcrWordsForCurrentSign()
        {
            List<string> titleIds = new List<string>();
            if (generalOcr == null || generalOcr.words_result == null || generalOcr.words_result.Length == 0)
            {
                return titleIds;
            }

            string sign = TeacherMainManager.instance != null && TeacherMainManager.instance.config != null
                ? TeacherMainManager.instance.config.sign
                : string.Empty;
            if (!string.Equals(sign, "ht", StringComparison.OrdinalIgnoreCase))
            {
                return titleIds;
            }

            HashSet<string> seen = new HashSet<string>();
            Regex titleRegex = new Regex(@"(?<!\d)(\d{1,2})\s*[-－—]\s*(\d{1,3})(?!\d)");
            foreach (var item in generalOcr.words_result)
            {
                string words = item != null ? item.words : string.Empty;
                if (string.IsNullOrEmpty(words))
                {
                    continue;
                }

                Match match = titleRegex.Match(words);
                if (!match.Success)
                {
                    continue;
                }

                string titleId = $"{match.Groups[1].Value}-{match.Groups[2].Value}-{sign}";
                if (seen.Add(titleId))
                {
                    titleIds.Add(titleId);
                    Debug.Log("[OCRTitleDetect] " + titleId + " words=" + words);
                }
            }

            return titleIds;
        }

        int IsMarkPositionOk(List<List<PositionInt>> studentMarkData, List<List<PositionInt>> standardMarkData)
        {
            foreach (var item in standardMarkData)
            {
                int standard_width_min = item[0].x;
                int standard_width_max = item[0].x;
                int standard_height_min = item[0].y;
                int standard_height_max = item[0].y;

                for (int i = 1; i < item.Count; i++)
                {
                    if (item[i].x < standard_width_min)
                    {
                        standard_width_min = item[i].x;
                    }
                    if (item[i].x > standard_width_max)
                    {
                        standard_width_max = item[i].x;
                    }
                    if (item[i].y < standard_height_min)
                    {
                        standard_height_min = item[i].y;
                    }
                    if (item[i].y > standard_height_max)
                    {
                        standard_height_max = item[i].y;
                    }
                }
                for (int s = 0; s < studentMarkData.Count; s++)
                {
                    for (int i = 0; i < studentMarkData[s].Count; i++)
                    {
                        if (studentMarkData[s][i].x >= standard_width_min && studentMarkData[s][i].x <= standard_width_max && studentMarkData[s][i].y >= standard_height_min && studentMarkData[s][i].y <= standard_height_max)
                        {
                            Debug.Log("返回索引" + s);
                            return s;
                        }
                    }
                }
            }

            return -1;
        }
        public List<int> IsOCRPositionOk(List<List<PositionInt>> studentMarkData, List<PositionInt> standardMarkData)
        {
            float drawWidth = TeacherMainManager.instance.width;
            float drawHeight = TeacherMainManager.instance.height;

            float scaleX = picWidth / drawWidth;
            float scaleY = picHeight / drawHeight;
            List<PositionInt> changedPosData = new List<PositionInt>();
            foreach (var item in standardMarkData)
            {
                changedPosData.Add(new PositionInt((int)((item.x) * scaleX), (int)((item.y) * scaleY)));
            }

            int standard_width_min = changedPosData[0].x;
            int standard_width_max = changedPosData[0].x;
            int standard_height_min = changedPosData[0].y;
            int standard_height_max = changedPosData[0].y;

            for (int i = 1; i < changedPosData.Count; i++)
            {
                if (changedPosData[i].x < standard_width_min)
                {
                    standard_width_min = changedPosData[i].x;
                }
                if (changedPosData[i].x > standard_width_max)
                {
                    standard_width_max = changedPosData[i].x;
                }
                if (changedPosData[i].y < standard_height_min)
                {
                    standard_height_min = changedPosData[i].y;
                }
                if (changedPosData[i].y > standard_height_max)
                {
                    standard_height_max = changedPosData[i].y;
                }
            }
            Debug.Log("standard_width_min" + standard_width_min + "standard_width_max" + standard_width_max + "standard_height_min" + standard_height_min + "standard_height_max" + standard_height_max);
            int pointMaxNums = 0;
            int pointNumValue = 0;
            List<int> selectedPointNums = new List<int>();
            for (int s = 0; s < studentMarkData.Count; s++)
            {
                int pointNums = 0;

                int student_width_min = studentMarkData[s][0].x;
                int student_width_max = studentMarkData[s][2].x;
                int student_height_min = studentMarkData[s][2].y;
                int student_height_max = studentMarkData[s][0].y;

                Vector2 student_CenterPos = GetMidpoint(new Vector2(student_width_min, student_height_min), new Vector2(student_width_max, student_height_max));
                if (student_CenterPos.x >= standard_width_min && student_CenterPos.x <= standard_width_max && student_CenterPos.y >= standard_height_min && student_CenterPos.y <= standard_height_max)
                {
                    Debug.Log("文字识别判分中心点" + s);
                    pointNums = 4;
                }
                else
                {
                    for (int i = 0; i < studentMarkData[s].Count; i++)
                    {
                        if (studentMarkData[s][i].x >= standard_width_min && studentMarkData[s][i].x <= standard_width_max && studentMarkData[s][i].y >= standard_height_min && studentMarkData[s][i].y <= standard_height_max)
                        {

                            pointNums++;
                            Debug.Log("pointNums++" + pointNums);
                        }
                    }
                }

                if (pointNums > 0)
                {
                    selectedPointNums.Add(s);
                }

                if (pointNums > pointMaxNums)
                {
                    pointMaxNums = pointNums;
                    pointNumValue = s;
                }
            }
            selectedPointNums.Sort((a, b) => b.CompareTo(a));
            return selectedPointNums;
        }
        private Vector2 GetMidpoint(Vector2 a, Vector2 b)
        {
            return new Vector2((a.x + b.x) / 2, (a.y + b.y) / 2);
        }
        public void ReadConfigFile(string _path)
        {
            // 检查文件是否存在
            if (System.IO.File.Exists(_path))
            {
                // 读取所有行
                string[] lines = System.IO.File.ReadAllLines(_path);
                if (dictitleIdNumAndPageCodes != null)
                {
                    dictitleIdNumAndPageCodes.Clear();
                }

                // 遍历每一行
                foreach (string item in lines)
                {
                    if (item.Contains("zy"))
                    {
                        if (item.Contains(")"))
                        {
                            string[] parts1 = item.Split(')');
                            dictitleIdNumAndPageCodes.Add(parts1[0] + ")", parts1[1]);
                        }
                        else
                        {
                            string[] parts = item.Split('y');
                            dictitleIdNumAndPageCodes.Add(parts[0] + "y", parts[1]);
                        }

                    }
                    else if (item.Contains("wz"))
                    {
                        if (item.Contains(")"))
                        {
                            string[] parts1 = item.Split(')');
                            dictitleIdNumAndPageCodes.Add(parts1[0] + ")", parts1[1]);
                        }
                        else
                        {
                            string[] parts = item.Split('z');
                            dictitleIdNumAndPageCodes.Add(parts[0] + "z", parts[1]);
                        }

                    }
                }


            }
            else
            {
                Debug.LogError("File not found at path: " + _path);
            }

        }
        public void ReadConfigFile1(string _path)
        {
            // 检查文件是否存在
            if (System.IO.File.Exists(_path))
            {
                // 读取所有行
                string[] lines = System.IO.File.ReadAllLines(_path);
                if (dictitleIdNumAndPageCodes != null)
                {
                    dictitleIdNumAndPageCodes.Clear();
                }

                // 遍历每一行
                foreach (string item in lines)
                {
                    if (item.Contains("a"))
                    {
                        if (item.Contains(")"))
                        {
                            string[] parts1 = item.Split(')');
                            dictitleIdNumAndPageCodes.Add(parts1[0] + ")", parts1[1]);
                        }
                        else
                        {
                            string[] parts = item.Split('a');
                            dictitleIdNumAndPageCodes.Add(parts[0] + "a", parts[1]);
                        }

                    }else if (item.Contains("pb")) {
                        if (item.Contains(")")) {
                            string[] parts1 = item.Split(')');
                            dictitleIdNumAndPageCodes.Add(parts1[0] + ")", parts1[1]);
                        }
                        else {
                            string[] parts = item.Split('b');
                            dictitleIdNumAndPageCodes.Add(parts[0] + "b", parts[1]);
                        }

                    }
                }


            }
            else
            {
                Debug.LogError("File not found at path: " + _path);
            }

        }

    }
}
