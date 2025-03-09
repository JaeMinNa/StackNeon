using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Popup_Ranking : MonoBehaviour
{
    [SerializeField] private Button Btn_Exit = null;
    [SerializeField] private Button Btn_Replay = null;
    [SerializeField] private Button Btn_Delete = null;

    private void Awake()
    {
        Btn_Exit.onClick.AddListener(OnClick_Exit);
        Btn_Replay.onClick.AddListener(OnClick_RePlay);
        Btn_Delete.onClick.AddListener(OnClick_Delete);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Button
    private void OnClick_Exit()
    {
        SoundManager.Instance.StartSFX("Click");

        // ���� ���� ȯ���� �������̸� ������ �÷��̸�� ����
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

        // ���� ���� ȯ���� �����Ͱ� �ƴϸ� ���α׷� ����
        #else
        Application.Quit();
        #endif
    }

    private void OnClick_RePlay()
    {
        SoundManager.Instance.StartSFX("Click");
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");
    }

    // ���� �����͸� �ʱ�ȭ
    private void OnClick_Delete()
    {
        SoundManager.Instance.StartSFX("Click");
        PlayerPrefs.DeleteAll();
        OnClick_Exit();
    }
    #endregion
}
