using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace jxzt
{
    /// <summary>
    /// 光标管理器
    /// 管理绘图时的光标样式和状态，支持切换不同绘图工具的光标
    /// 单例模式，全局控制光标显示
    /// </summary>
    public class CursorManager : MonoBehaviour
    {
        public RawImage image;
        public Texture2D texture;
        public static CursorManager instance;
        public GameObject Compass;
        private void Start()
        {
        }
        private void Update()
        {

        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                image = GetComponent<RawImage>();
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void ChangeSize(int e)
        {
            GetComponent<RectTransform>().sizeDelta = Vector2.one * e * 2;
        }

        public void OnPointerMove(Vector2 eventDataPosition)
        {
            transform.position = eventDataPosition - GetComponent<RectTransform>().rect.size / 2;
        }
    }
}

