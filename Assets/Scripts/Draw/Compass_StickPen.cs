using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
namespace jxzt
{
    /// <summary>
    /// 圆规画笔杆
    /// 圆规带画笔的一端，处理拖拽画圆操作
    /// 记录画圆方向（顺时针/逆时针），判断是否为整圆
    /// </summary>
    public class Compass_StickPen : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        public RectTransform bar;
        public RectTransform stick;
        public RectTransform penstick;


        public Vector2 startEdge;
        public Vector2 endEdge;


        Vector2 stickPosition;
        Vector2 penPosition;
        int dir1;//画线方向，0是无法判断，1是顺时针画，-1是逆时针
        int isCircle;//判断是否画了个整圆
        float dirValue;//判断是否转向的值
        public LayerManager currentLayer;
        private Vector2 last_position;
        private bool click_state;
        public void Start()
        {
        }
        List<Arc> arcs;
        public void OnDrag(PointerEventData eventData)
        {
            SetDraggedRotation(eventData);

            if (click_state)
            {
                Vector2 penPosition = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, penstick.position);
                int slicenum = (int)(Vector2.Distance(last_position, penPosition) / 5 + 1);
                for (int i = 1; i <= slicenum; i++)
                {
                    if (MainManager.instance.currentBrush != null)
                    {
                        currentLayer.Draw(Vector2.Lerp(last_position, penPosition, i / (float)slicenum), MainManager.instance.currentBrush);
                    }
                }

                currentLayer.UpdateTex();
                last_position = penPosition;
                CursorManager.instance.OnPointerMove(penPosition);
            }


        }
        private void InitPointData(PointerEventData eventData, out Vector2 Apoint, out Vector2 Apoint1)
        {
            int center_x = (int)stickPosition.x;
            int center_y = (int)stickPosition.y;
            Vector2 penPosition = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, penstick.position);
            Vector2 orignPoint = new Vector2(center_x, center_y);
            Apoint = (startEdge - orignPoint).normalized;
            Apoint1 = (penPosition - orignPoint).normalized;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (currentLayer)
            {
                if (currentLayer.active)
                {
                    click_state = true;
                    last_position = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, penstick.position);
                    CursorManager.instance.gameObject.SetActive(click_state);
                }
            }

        }

        public void OnPointerUp(PointerEventData eventData)
        {

            click_state = false;
            CursorManager.instance.gameObject.SetActive(click_state);


        }

        private void SetDraggedRotation(PointerEventData eventData)
        {
            Vector2 curScreenPosition = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, penstick.position);
            Vector2 stickPosition = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, stick.position);


            Vector2 from = curScreenPosition - stickPosition;
            Vector2 to = eventData.position - stickPosition;


            bar.transform.RotateAround(stick.position, Vector3.forward, Quaternion.FromToRotation(from.normalized
                , to.normalized).eulerAngles.z);

            Vector2 currentEdge = (stickPosition - penPosition).normalized;


        }


    }
}

