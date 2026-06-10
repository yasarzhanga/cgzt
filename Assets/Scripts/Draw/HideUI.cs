using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace jxzt
{
    /// <summary>
    /// 隐藏UI
    /// 控制侧边栏或相关UI面板的显示/隐藏，支持滑动动画效果
    /// 用于最大化绘图区域，隐藏不必要的UI元素
    /// </summary>
    public class HideUI : MonoBehaviour
    {
        public Vector2 startPos;
        public Vector2 endPos;//-945    

        // Start is called before the first frame update
        void Start()
        {
            transform.GetComponent<Button>().onClick.AddListener(() =>
            {
                Transform layers = GameObject.Find("Layers").transform;
                for (int i = 0; i < layers.childCount; i++)
                {
                    if (layers.GetChild(i).GetComponent<Toggle>().isOn)
                    {
                        foreach (Transform item in GameObject.Find("PaintBoard").transform)
                        {
                            if (item.GetComponent<StudentLayerManager>())
                            {

                            }
                            else if (item.GetComponent<TeacherLayerManager>().layerNum == (i + 1))
                            {
                                item.GetComponent<TeacherLayerManager>().lineshape = (lineshape)System.Enum.Parse(typeof(lineshape), transform.GetChild(0).GetComponent<TMP_Text>().text);
                                transform.parent.parent.parent.GetChild(0).GetChild(1).GetComponent<Compass_StickPen>().currentLayer = item.GetComponent<LayerManager>();
                            }

                        }
                    }
                }
                transform.parent.gameObject.SetActive(false);
            });
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

