using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup_NickName : MonoBehaviour
{
    [SerializeField] private Button Btn_Ok = null;
    [SerializeField] private TMP_Text Text_Name = null;
    [SerializeField] private TMP_InputField InputField;

    private int m_UID = 0;

    private void Awake()
    {
        Btn_Ok.onClick.AddListener(OnClick_Ok);
    }

    private void Start()
    {
        int RandomValue = Random.Range(0, 100000000);
        m_UID = RandomValue;
        InputField.text = m_UID.ToString();
    }

    // 매개변수는 string만 사용 가능
    public void NameInput()
    {
        Text_Name.text = InputField.text;
    }

    public void OnClick_Ok()
    {
        if (Text_Name.text == string.Empty)
            return;

        SoundManager.Instance.StartSFX("Click");

        PlayerPrefs.SetString("NickName", Text_Name.text);
        BackendManager.Instance.SignUp();
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
}
