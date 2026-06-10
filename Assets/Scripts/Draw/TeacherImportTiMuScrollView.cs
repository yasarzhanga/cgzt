using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
namespace jxzt
{
    /// <summary>
    /// 教师导入题目滚动视图
    /// 从本地目录读取题目图片，展示题目列表供教师选择
    /// 点击题目时加载对应的标准答案
    /// </summary>
    public class TeacherImportTiMuScrollView : MonoBehaviour
    {
        public int height;
        public string picName;
        public GameObject studentLayer_pre;
        public List<string> dataFileList;
        // Start is called before the first frame update
        void Start()
        {
            height = Mathf.FloorToInt(transform.childCount / 5) * 110;
            transform.GetComponent<RectTransform>().sizeDelta = new Vector2(0, height);

            dataFileList = new List<string>();
            GetAllFilesAndDertorys(Application.streamingAssetsPath + "/TiMu/");
            if (transform.childCount < dataFileList.Count)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetComponent<LoadImageTitle>().LoadImage(Application.streamingAssetsPath + "/TiMu/" + dataFileList[i]);
                    transform.GetChild(i).GetComponent<Image>().sprite = transform.GetComponent<LoadImageTitle>().imageSprite;
                    transform.GetChild(i).GetComponent<SaveTexture>().texture = transform.GetComponent<LoadImageTitle>().texture;
                    //绑定图片名字
                    string input = dataFileList[i];
                    string[] parts = input.Split('.');
                    transform.GetChild(i).GetComponent<SaveTexture>().teacherAnswerName = parts[0];
                }
            }
            if (transform.childCount >= dataFileList.Count)
            {
                for (int i = 0; i < dataFileList.Count; i++)
                {
                    transform.GetComponent<LoadImageTitle>().LoadImage(Application.streamingAssetsPath + "/TiMu/" + dataFileList[i]);
                    transform.GetChild(i).GetComponent<Image>().sprite = transform.GetComponent<LoadImageTitle>().imageSprite;
                    transform.GetChild(i).GetComponent<SaveTexture>().texture = transform.GetComponent<LoadImageTitle>().texture;
                    //绑定图片名字
                    string input = dataFileList[i];
                    string[] parts = input.Split('.');
                    transform.GetChild(i).GetComponent<SaveTexture>().teacherAnswerName = parts[0];
                }
            }


        }
        public void GetAllFilesAndDertorys(string _path)
        {
            //判断路径是否存在
            if (Directory.Exists(_path))
            {
                DirectoryInfo dir = new DirectoryInfo(_path);
                FileInfo[] files = dir.GetFiles("*");

                foreach (var item in files)
                {
                    //忽略.meta
                    if (item.Name.EndsWith(".meta")) continue;
                    dataFileList.Add(item.Name);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

