using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPing : MonoBehaviour
{
    /// <summary>
    /// 网络延迟检测
    /// 使用 Ping 类检测与服务器的连接延迟
    /// 用于检查网络连接状态
    /// </summary>
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Pingip());
    }

    // Update is called once per frame
    void Update()
    {

    }


    IEnumerator Pingip()
    {
        string ip = Config.IP;
        float pingTime = 0;
        Ping ping = new Ping(ip);
        while (!ping.isDone)
        {
            yield return new WaitForSeconds(0.1f);
            if (pingTime > 3.0)
            {
                StartCoroutine(Pingip());
                break;
            }
            pingTime += 0.1f;
        }
        if (ping.isDone)
        {
            yield return new WaitForSeconds(2f);
            StartCoroutine(Pingip());
        }
    }
}
