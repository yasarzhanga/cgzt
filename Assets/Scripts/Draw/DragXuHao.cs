using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace jxzt
{
    /// <summary>
    /// 拖拽序号
    /// 处理绘图图层/图线的拖拽排序操作
    /// 实现IPointer系列接口，支持鼠标交互调整顺序
    /// </summary>
    public class DragXuHao : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
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

