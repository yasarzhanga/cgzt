using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace jxzt
{
    /// <summary>
    /// 圆规旋转控制
    /// 处理圆规整体的旋转拖拽操作，实现画圆时的角度调整
    /// 实现IDragHandler接口，响应拖拽事件
    /// </summary>
    public class Compass_rotate : MonoBehaviour, IDragHandler
    {
        public RectTransform composs;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        private void SetDraggedRotation(PointerEventData eventData)
        {
            Vector2 curScreenPosition = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, composs.transform.position);
            Vector2 directionTo = curScreenPosition - eventData.position;
            Vector2 directionFrom = directionTo - eventData.delta;
            composs.transform.rotation *= Quaternion.FromToRotation(directionTo, directionFrom);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (Input.GetMouseButton(1))
            {
                SetDraggedRotation(eventData);
            }

        }
    }
}

