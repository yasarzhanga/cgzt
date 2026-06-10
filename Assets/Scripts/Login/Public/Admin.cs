using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static jxzt.Info;

public class Admin : MonoBehaviour
{
    /// <summary>
    /// 登录信息管理器（单例）
    /// 全局保存登录用户信息（学生或教师），跨场景持久化
    /// 用于判断当前登录用户类型和基本信息
    /// </summary>
    private static Admin instance;

    public static Admin Instance { get => instance; }

    public StudentInfo studentInfo;//登录的学生信息
    public TeacherInfo teacherInfo;//登录的教师信息

    public bool isTeacher = false;
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
        }
    }
    private void Start()
    {

    }
    public void StartXinTiao()
    {
        StartCoroutine(XinTiao());
    }
    public void StopXinTiao()
    {
        StopAllCoroutines();
    }
    IEnumerator XinTiao()
    {
        while (true)
        {
            string str = Config.UserHeartbeat + studentInfo.id + "/heartbeat/";
            WebManager.Instance.GetStringFunc(str, delegate (string s)
            {
            });
            yield return new WaitForSeconds(2f);
        }
    }
}
