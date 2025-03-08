using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup_NickName : MonoBehaviour
{
    [SerializeField] private Button Btn_Ok = null;
    [SerializeField] private TMP_Text Text_Name = null;
    [SerializeField] private TMP_InputField InputField;

    private void Awake()
    {
        Btn_Ok.onClick.AddListener(OnClick_Ok);
    }

    // �Ű������� string�� ��� ����
    public void NameInput()
    {
        Text_Name.text = InputField.text;
    }

    public void OnClick_Ok()
    {
        //User.NickName = Text_Name.text;
        PlayerPrefs.SetString("NickName", Text_Name.text);
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
}
