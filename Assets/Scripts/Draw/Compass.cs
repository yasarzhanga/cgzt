using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace jxzt
{
    /// <summary>
    /// 圆规组件主控制器
    /// 管理圆规的各个部件（横杆、支杆、画笔杆），协调圆规的整体行为
    /// 用于绘图时绘制圆形的辅助工具
    /// </summary>
    public class Compass : MonoBehaviour
    {
        public GameObject bar;
        public GameObject left_stick;
        public GameObject right_stick_pen;

        private void Start()
        {

        }
    }
}
