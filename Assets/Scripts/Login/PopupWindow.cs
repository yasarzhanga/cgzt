using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace jxzt
{
    /// <summary>
    /// 弹窗管理器
    /// 通用弹窗组件，用于显示提示信息、注册成功提示等
    /// 支持显示/隐藏弹窗面板和设置弹窗文本
    /// </summary>
    public class PopupWindow : MonoBehaviour
    {
        public Button closeBtn;//弹窗关闭按钮
        public GameObject popupPanel;//弹窗的Panel对象
        public Text popupText;//弹窗显示文本

        // Start is called before the first frame update
        void Start()
        {
            closeBtn.onClick.AddListener(HidePopup);
        }
        public void ShowPopup(string message)
        {
            popupText.text = message;
            popupPanel.SetActive(true);
        }
        public void HidePopup()
        {
            popupPanel.SetActive(false);
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}

