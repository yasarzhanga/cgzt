using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace jxzt
{
    /// <summary>
    /// 标识测试脚本（调试用）
    /// 用于测试 YOLO 模型识别和进度条显示
    /// 挂载在测试按钮上，加载图片并执行目标检测
    /// </summary>
    public class biaoshitest : MonoBehaviour
    {
        public Button button;
        public Slider progressBar; // 或者 public Image progressBar;
        // Start is called before the first frame update
        void Start()
        {
            float totalDuration = 10.0f; // 总共10秒
            StartCoroutine(UpdateProgressBar(totalDuration));


            button.onClick.AddListener(() =>
            {
                button.transform.GetComponent<LoadImageTitle>().LoadImage(Application.streamingAssetsPath + @"/XueShengDaAn/2024-03-15-03-06-41.png");
                button.transform.GetComponent<RunYOLO>().texture2D = button.transform.GetComponent<LoadImageTitle>().texture;
                button.transform.GetComponent<RunYOLO>().ExecuteML();
            });
        }

        // Update is called once per frame
        void Update()
        {

        }
        IEnumerator UpdateProgressBar(float totalDuration)
        {
            float startTime = Time.time;
            float endTime = startTime + totalDuration;
            float currentStepStartTime = startTime;
            int currentStep = 0;

            while (currentStep < 3)
            {
                // 计算当前步骤的进度（0到1之间）
                float progress = (Time.time - startTime) / totalDuration;
                progressBar.value = progress;

                // 等待一小段时间，使进度条看起来更平滑
                yield return new WaitForEndOfFrame();

            }

            // 确保进度条最终值为1
            progressBar.value = 1.0f;
        }

        float GetStepDuration(int step)
        {
            switch (step)
            {
                case 0:
                    return 2.0f; // 第一个步骤持续2秒
                case 1:
                    return 4.0f; // 第二个步骤持续4秒
                case 2:
                    return 4.0f; // 第三个步骤持续4秒
                default:
                    return 0.0f;
            }
        }

        void MethodOne()
        {
            // 模拟一些工作
            Debug.Log("Executing Method One");
        }

        void MethodTwo()
        {
            // 模拟一些工作
            Debug.Log("Executing Method Two");
        }

        void MethodThree()
        {
            // 模拟一些工作
            Debug.Log("Executing Method Three");
        }
    }
}

