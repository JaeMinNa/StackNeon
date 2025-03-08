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

    // 매개변수는 string만 사용 가능
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
