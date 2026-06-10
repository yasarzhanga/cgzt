using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace jxzt
{
    /// <summary>
    /// 圆规横杆
    /// 处理圆规横杆的鼠标交互（点击、拖拽），控制圆规的开合
    /// 实现IPointerClickHandler等接口，响应UI事件
    /// </summary>
    public class Compass_Bar : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
    {
        bool barClickState = false;
        Vector2 lastPointerPosition;
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {

        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                barClickState = true;
                lastPointerPosition = eventData.position;
            }

        }

        void IPointerMoveHandler.OnPointerMove(PointerEventData eventData)
        {
            if (barClickState)
            {
                ((RectTransform)transform).anchoredPosition += eventData.position - lastPointerPosition;
                lastPointerPosition = eventData.position;
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            barClickState = false;
        }

    }
}

