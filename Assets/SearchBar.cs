using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SearchBar : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public List<TMP_Dropdown.OptionData> studentAnswersOptions = new List<TMP_Dropdown.OptionData>();
    public TMP_InputField inputField;
    /// <summary>
    /// �����б���Ĭ�� ��ʼ�����б�
    /// </summary>
    public List<TMP_Dropdown.OptionData> LibraryList = new List<TMP_Dropdown.OptionData>();
    /// <summary>
    /// ������ ���ص������б�
    /// </summary>
    private List<TMP_Dropdown.OptionData> ResultList = new List<TMP_Dropdown.OptionData>();

    private void Start()
    {
        
        
    }
    private void Update()
    {
        if (inputField.isFocused && inputField.placeholder.gameObject.activeSelf == false)
        {
            ShowInputField();
        }
    }
    public void Init()
    {
        foreach (var option in studentAnswersOptions)
        {
            LibraryList.Add(option);
        }
        LibraryList.ForEach(i => ResultList.Add(i));//��������һ��lambda����ʽ��i����������LibraryList�����е�һ��Ԫ��
        SetPlaceholder("������...");
        inputField.onEndEdit.AddListener(delegate
        {
            Filter();
            ShowResult();
        });
    }
    /// <summary>
    /// ��ʼ�� �����б�
    /// </summary>
    private void InitDropDown()
    {
        dropdown.ClearOptions();//���
        dropdown.options.Add(new TMP_Dropdown.OptionData());
        foreach (var item in LibraryList)
        {
            TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();
            dropdown.options.Add(optionData);
        }

    }

    public void HideInputField()
    {
        inputField.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        inputField.placeholder.gameObject.SetActive(false);
        inputField.textComponent.gameObject.SetActive(false);
    }

    private void ShowInputField()
    {
        inputField.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        inputField.placeholder.gameObject.SetActive(true);
        inputField.textComponent.gameObject.SetActive(true);
    }
    /// <summary>
    /// ����Ĭ����ʾ����
    /// </summary>
    /// <param name="str"></param>
    public void SetPlaceholder(string str)
    {
        inputField.placeholder.GetComponent<TMP_Text>().text = str;
    }
    /// <summary>
    /// ���������б���Ĭ������
    /// </summary>
    /// <param name="_list"></param>
    public void SetLibraryList(List<string> _list)
    {
    }
    /// <summary>
    /// ɸѡ�ַ�
    /// </summary>
    private void Filter()
    {       
        ResultList = TextLenovo(inputField.text, LibraryList);
    }
    /// <summary>
    /// ��ʾ�������
    /// </summary>
    private void ShowResult()
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(ResultList);       
        if (ResultList.Count != 0)
        {            
            dropdown.Show();            
        }
    }
    /// <summary>
    /// �ı�����
    /// </summary>
    /// <param ������ַ�="text_item"></param>
    /// <param ������="TextLibraryList"></param>
    /// <param ���ؽ������="temp_list"></param>
    /// <returns></returns>
    public List<TMP_Dropdown.OptionData> TextLenovo(string text_item, List<TMP_Dropdown.OptionData> TextLibraryList)
    {
        List<TMP_Dropdown.OptionData> temp_list = new List<TMP_Dropdown.OptionData>();
        temp_list.Add(new TMP_Dropdown.OptionData("--��ѡ��--"));
        foreach (var item in TextLibraryList)
        {
            if (item.text.Contains(text_item))
            {
                temp_list.Add(item);
            }
        }
        return temp_list;
    }
}

