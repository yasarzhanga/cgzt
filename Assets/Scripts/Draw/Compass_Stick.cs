using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace jxzt
{
    /// <summary>
    /// 圆规支杆
    /// 处理圆规支杆的拖拽操作，控制圆规半径大小
    /// 与横杆联动，实现圆规开合效果
    /// </summary>
    public class Compass_Stick : MonoBehaviour, IDragHandler
    {
        public RectTransform pen;

        private void Start()
        {
        }

        public void OnDrag(PointerEventData eventData)
        {
            SetDraggedRotation(eventData);
        }

        private void SetDraggedRotation(PointerEventData eventData)
        {
            Vector2 curScreenPosition = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, pen.transform.position);
            Vector2 directionTo = curScreenPosition - eventData.position;
            Vector2 directionFrom = directionTo - eventData.delta;
            this.transform.rotation *= Quaternion.FromToRotation(directionTo, directionFrom);
            pen.rotation *= Quaternion.FromToRotation(directionFrom, directionTo);

        }

    }

}
