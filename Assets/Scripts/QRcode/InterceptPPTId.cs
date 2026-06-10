using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace jxzt
{
    /// <summary>
    /// 截取PPT ID
    /// 从URL中解析提取PPT ID（最后一个=后面的字符串）
    /// 用于处理二维码扫描结果，获取题目标识
    /// </summary>
    public class InterceptPPTId : MonoBehaviour
    {
        string PPTId;
        // Start is called before the first frame update
        void Start()
        {
            PPTId = "http://keming365.com/user/showPPT?appliId=733999265981923328";
            InterceptPPTId_Fun();
        }

        // Update is called once per frame
        void Update()
        {

        }
        void InterceptPPTId_Fun()
        {
            string url = PPTId;
            string[] parts = url.Split('=');
            string result = parts[parts.Length - 1];
            Debug.Log(result);

        }
        string GetPPTId_Fun(string PPTId)
        {
            string url = PPTId;
            string[] parts = url.Split('=');
            string result = parts[parts.Length - 1];
            Debug.Log(result);
            return result;

        }
    }
}

