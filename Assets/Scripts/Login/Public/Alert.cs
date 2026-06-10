using jxzt;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Alert : MonoBehaviour
{
    /// <summary>
    /// 提示框管理器（单例）
    /// 通用提示对话框，支持普通提示、带选项的确认提示
    /// 用于显示操作结果、确认上传等交互
    /// </summary>
    private static Alert instance;

    public static Alert Instance { get => instance; }

    public GameObject Tips;//提示UI
    public GameObject Tips_Toggle;//提示选项
    public GameObject TipsBtn_Toggle;//提示选项俩按钮    


    public Text text_tips, text_tipsToggle;//提示文字
    public Text texttwoBtn_tips;//提示俩按钮
 

    public Button button_yes;//提示确定
    public Button button_upload;//上传按钮
    public Button button_delete;//删除按钮
    public Button button_yes_Toggle;//提示Toggle确定

    public GameObject Tips_loding;
    public Text text_loding;//加载

    public GameObject Tips_Request;//带监听
    public Text text_tipsRequest;
    public Button button_Request;
    private Action action;

    public GameObject TipsWithCloseButton;
    public Text text_TipsWithCloseButton;
    public Button Confirm_Button;
    public Button Close_Button;

    [Space]
    [Header("学生查询面板相关")]
    public GameObject StudentInfoPanel;
    public Button StudentInfoPanelConfirmButton;
    public Text StudentInfoPanelTitleText;
    public GameObject StudentInfoPanelScrollView;
    public GameObject StudentInfoPanelScrollViewContent;
    public GameObject StudentInfoPrefab;
    public Dropdown StudentInfoPanelDropdown;


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

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        button_Request.onClick.AddListener(delegate ()
        {
            if (action != null)
            {
                action();
            }
        });

        Close_Button.onClick.AddListener(delegate ()
        {
            text_TipsWithCloseButton.text = "";
            TipsWithCloseButton.SetActive(false);
        });
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowTips(string str, bool isSelfOKEvent = true)
    {
        text_tips.text = str;
        if (Tips.activeSelf == false)
        {
            Tips.SetActive(true);
        }
        if (isSelfOKEvent)
        {
            button_yes.onClick.AddListener(delegate ()
            {
                HideTips();
                text_tips.text = "";
            });
        }
    }
    public void ShowTips_TwoBtn(string str, bool isSelfOKEvent = true)
    {
        texttwoBtn_tips.text = str;
        if (TipsBtn_Toggle.activeSelf == false)
        {
            TipsBtn_Toggle.SetActive(true);
        }
        else
        {
            TipsBtn_Toggle.SetActive(false);
        }
    }

    public void ShowTipsWithCloseButton(string str, bool isSelfOKEvent = true) {
        text_TipsWithCloseButton.text = str;
        if (TipsWithCloseButton.activeSelf == false) {
            TipsWithCloseButton.SetActive(true);
        }
        if (isSelfOKEvent) {
            Confirm_Button.onClick.AddListener(delegate () {
                if (TipsWithCloseButton.activeSelf) {
                    Confirm_Button.onClick.RemoveAllListeners();
                    TipsWithCloseButton.SetActive(false);
                }
                text_TipsWithCloseButton.text = "";
            });
        }
    }

    public void HideTips()
    {
        if (Tips.activeSelf)
        {
            button_yes.onClick.RemoveAllListeners();
            Tips.SetActive(false);
            
        }
    }

    public void ShowTipsRequest(string str, Action act)
    {
        text_tipsRequest.text = str;
        Tips_Request.SetActive(true);
        action = act;
    }
    public void HideTipsRequest()
    {
        Tips_Request.SetActive(false);
    }

    public void ShowToggle(string str)
    {
        text_tipsToggle.text = str;
        Tips_Toggle.SetActive(true);
    }
    public void DestroyToggleChilds()
    {                
        int childCount = Tips_Toggle.transform.GetChild(0).GetChild(1).transform.childCount;
        
        if (childCount > 0)
        {
            for (int i = 0; i < childCount; i++)
            {
                Destroy(Tips_Toggle.transform.GetChild(0).GetChild(1).transform.GetChild(i).gameObject);
            }
        }        
    }
    public void HideToggle()
    {
        Tips_Toggle.SetActive(false);
    }



    public void ShowLoding(string str)
    {
        text_loding.text = str;
        Tips_loding.SetActive(true);
        StartCoroutine(ChangeLoading(str));
    }
    IEnumerator ChangeLoading(string s)
    {
        int index = 0;
        string str = "";
        while (index <= 3)
        {
            yield return new WaitForSeconds(0.3f);
            str = "";
            for (int i = 0; i < index; i++)
            {
                str += ".";
            }
            text_loding.text = s + str;
            index++;
            if (index == 4)
            {
                index = 0;
            }
        }
    }
    public void HideLoding()
    {
        StopAllCoroutines();
        Tips_loding.SetActive(false);
        text_loding.text = "";
    }

    // 新增：仅更新提示文本，不绑定/修改 OK 事件的便捷方法
    public void SetTipsText(string str)
    {
        if (Tips != null)
        {
            if (!Tips.activeSelf) Tips.SetActive(true);
            if (text_tips != null) text_tips.text = str;
        }
    }

    public void ShowStudentInfoPanel_ClassSelect(List<string> classInfos) { 
        StudentInfoPanel.SetActive(true);
        StudentInfoPanelScrollView.SetActive(false);

        StudentInfoPanelTitleText.text = "请选择班级";

        StudentInfoPanelDropdown.options.Clear();
        StudentInfoPanelDropdown.options.AddRange(classInfos.ConvertAll(info => new Dropdown.OptionData(info)));

        StudentInfoPanelDropdown.gameObject.SetActive(true);
    }

    public void ShowStudentInfoPanel_StudentInfo(List<string> studentInfos) {
        StudentInfoPanel.SetActive(true);
        StudentInfoPanelDropdown.gameObject.SetActive(false);
        StudentInfoPanelTitleText.text = "查询中..";

        // 清空滚动视图内容
        for (int i = 0; i < StudentInfoPanelScrollViewContent.transform.childCount; i++) {
            Destroy(StudentInfoPanelScrollViewContent.transform.GetChild(i).gameObject);
        }

        foreach (var i in studentInfos) { 
            var studentInfoItem = Instantiate(StudentInfoPrefab, StudentInfoPanelScrollViewContent.transform);
            studentInfoItem.GetComponentInChildren<Text>().text = i;
        }

        StudentInfoPanelTitleText.text = 
            $"已录入：{TeacherMainManager.instance.Dic_NoScoredStudentAnswerData.Count} " +
            $"此班级未录入：{studentInfos.Count} ";

        SetActionOnceOnStudentInfoPanelConfirmButton(() => {
            StudentInfoPanel.SetActive(false);
            StudentInfoPanelDropdown.gameObject.SetActive(false);
            StudentInfoPanelScrollView.SetActive(false);
        });

        StudentInfoPanelScrollView.SetActive(true);
    }

    public void SetActionOnceOnStudentInfoPanelConfirmButton(UnityAction action) {
        StudentInfoPanelConfirmButton.onClick.RemoveAllListeners();
        StudentInfoPanelConfirmButton.onClick.AddListener(() => { 
            action.Invoke();
            StudentInfoPanelConfirmButton.onClick.RemoveAllListeners();
        });
    }
}
