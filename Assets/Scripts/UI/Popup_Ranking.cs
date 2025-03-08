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
        // 현재 실행 환경이 에디터이면 에디터 플레이모드 종료
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

        // 현재 실행 환경이 에디터가 아니면 프로그램 종료
        #else
        Application.Quit();
        #endif
    }

    private void OnClick_RePlay()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");
    }

    // 유저 데이터를 초기화
    private void OnClick_Delete()
    {
        PlayerPrefs.DeleteAll();
        OnClick_Exit();
    }
    #endregion
}
