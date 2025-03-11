using BackEnd;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using LitJson;
using TMPro;

public class Popup_Ranking : MonoBehaviour
{
    [Header("Rank")]
    [SerializeField] private TMP_Text Text_MyRank = null;
    [SerializeField] private TMP_Text Text_MyRankScore = null;
    [SerializeField] private TMP_Text Text_MyNickName = null;
    [SerializeField] private GameObject Obj_UserRankContent = null;
    [SerializeField] private GameObject[] Obj_Medals = null;
    private TMP_Text Text_UserRank = null;
    private TMP_Text Text_UserRankScore = null;
    private TMP_Text Text_UserNickName = null;
    private GameObject Obj_Country = null;
    private GameObject Obj_UserImage = null;

    [Header("Button")]
    [SerializeField] private Button Btn_Exit = null;
    [SerializeField] private Button Btn_Replay = null;
    [SerializeField] private Button Btn_Delete = null;

    private GameController m_GameController = null;

    // 복사한 UUID 
    private const string RANK_UUID = "01958586-730a-7c37-8683-034a79987acf";
    private const int MAX_RANK_LIST = 6;

    #region Unity Method
    private void Awake()
    {
        Btn_Exit.onClick.AddListener(OnClick_Exit);
        Btn_Replay.onClick.AddListener(OnClick_RePlay);
        Btn_Delete.onClick.AddListener(OnClick_Delete);

        m_GameController = GameObject.FindWithTag("GameController").transform.GetComponent<GameController>();
    }

    private void Start()
    {
        bool IsSuccess = BackendManager.Instance.Load();

        if(IsSuccess)
        {
            if(User.BestScore < m_GameController.GetScore())
            {
                UpdateRank(m_GameController.GetScore());
            }

            GetMyRank();
            GetRankList();
        }
        else
        {
            Text_MyRank.text = "-";
            Text_MyRankScore.text = "-";
            Text_MyNickName.text = "-";

            for (int i = 0; i < MAX_RANK_LIST; i++)
            {
                // 랭킹 데이터 비활성화
                Text_UserRank = Obj_UserRankContent.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>();
                Text_UserRankScore = Obj_UserRankContent.transform.GetChild(i).GetChild(4).GetComponent<TextMeshProUGUI>();
                Text_UserNickName = Obj_UserRankContent.transform.GetChild(i).GetChild(3).GetComponent<TextMeshProUGUI>();
                Obj_Country = Obj_UserRankContent.transform.GetChild(i).GetChild(2).gameObject;
                Obj_UserImage = Obj_UserRankContent.transform.GetChild(i).GetChild(1).GetChild(0).GetChild(0).gameObject;

                Text_UserRank.text = "-";
                Text_UserRankScore.text = "-";
                Text_UserNickName.text = "-";
                Obj_Country.SetActive(false);
                Obj_UserImage.SetActive(false);
            }
        }
    }
    #endregion

    #region Rank
    private void UpdateRank(float Value)
    {
        UpdateMyRankData(Value);
    }

    private void UpdateMyRankData(float Value)
    {
        string rowInDate = string.Empty;

        // 랭킹 데이터를 업데이트하려면 게임 데이터에서 사용하는 데이터의 inDate 값 필요
        BackendReturnObject bro = Backend.GameData.GetMyData("UserData", new Where());

        if (!bro.IsSuccess())
        {
            Debug.LogError("랭킹 업데이트를 위한 데이터 조회 중 문제가 발생했습니다.");
            return;
        }

        Debug.Log("랭킹 업데이트를 위한 데이터 조회에 성공했습니다.");

        if (bro.FlattenRows().Count > 0)
        {
            rowInDate = bro.FlattenRows()[0]["inDate"].ToString();
        }
        else
        {
            Debug.LogError("데이터가 존재하지 않습니다.");
        }

        Param param = new Param()
        {
            {"RankScore",  Value}
        };

        // 해당 데이터테이블의 데이터를 갱신하고, 랭킹 데이터 정보 갱신
        bro = Backend.URank.User.UpdateUserScore(RANK_UUID, "UserData", rowInDate, param);

        if (bro.IsSuccess())
        {
            Debug.Log("랭킹 등록에 성공했습니다.");
        }
        else
        {
            Debug.LogError("랭킹 등록에 실패했습니다.");
        }
    }

    public void GetMyRank()
    {
        // 내 랭킹 정보 불러오기
        BackendReturnObject bro = Backend.URank.User.GetMyRank(RANK_UUID);

        if (bro.IsSuccess())
        {
            try
            {
                JsonData rankDataJson = bro.FlattenRows();

                // 받아온 데이터의 개수가 0 -> 데이터가 없음
                if (rankDataJson.Count <= 0)
                {
                    Debug.Log("나의 랭킹 데이터가 존재하지 않습니다.");

                    Text_MyRank.text = "-";
                    Text_MyRankScore.text = "-";
                    Text_MyNickName.text = "-";

                }
                else
                {
                    Text_MyRank.text = rankDataJson[0]["rank"].ToString();
                    Text_MyRankScore.text = rankDataJson[0]["score"].ToString() + " M";
                    Text_MyNickName.text = rankDataJson[0]["nickname"].ToString();
                    SetMyRankMedal(int.Parse(rankDataJson[0]["rank"].ToString()));

                    // 복수 데이터의 추가 항목 사용 시
                    //string[] extraData = rankDataJson[i]["WinLose"].ToString().Split("|");
                    //_win = int.Parse(extraData[0].ToString());
                    //_lose = int.Parse(extraData[1].ToString());

                    // 랭킹 데이터가 보이도록 설정
                }
            }
            // 나의 랭킹 정보 JSON 데이터 파싱에 실패했을 때
            catch (System.Exception e)
            {
                Debug.LogError(e);

                Text_MyRank.text = "-";
                Text_MyRankScore.text = "-";
                Text_MyNickName.text = "-";
            }
        }
        else
        {
            // 나의 랭킹 정보를 불러오는데 실패했을 때

            Text_MyRank.text = "-";
            Text_MyRankScore.text = "-";
            Text_MyNickName.text = "-";
        }
    }

    public void GetRankList()
    {
        // 랭킹 테이블에 있는 유저의 offset ~ offset + limit 순위 랭킹 정보를 불러옴
        BackendReturnObject bro = Backend.URank.User.GetRankList(RANK_UUID, MAX_RANK_LIST, 0);

        if (bro.IsSuccess())
        {
            // JSON 데이터 파싱 성공
            try
            {
                Debug.Log("랭킹 조회에 성공했습니다.");
                JsonData rankDataJson = bro.FlattenRows();

                // 받아온 데이터의 개수가 0 -> 데이터가 없음
                if (rankDataJson.Count <= 0)
                {
                    Debug.Log("랭킹 데이터가 존재하지 않습니다.");

                    for (int i = 0; i < MAX_RANK_LIST; i++)
                    {
                        // 랭킹 데이터 비활성화
                        Text_UserRank = Obj_UserRankContent.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>();
                        Text_UserRankScore = Obj_UserRankContent.transform.GetChild(i).GetChild(4).GetComponent<TextMeshProUGUI>();
                        Text_UserNickName = Obj_UserRankContent.transform.GetChild(i).GetChild(3).GetComponent<TextMeshProUGUI>();
                        Obj_Country = Obj_UserRankContent.transform.GetChild(i).GetChild(2).gameObject;
                        Obj_UserImage = Obj_UserRankContent.transform.GetChild(i).GetChild(1).GetChild(0).GetChild(0).gameObject;

                        Text_UserRank.text = "-";
                        Text_UserRankScore.text = "-";
                        Text_UserNickName.text = "-";
                        Obj_Country.SetActive(false);
                        Obj_UserImage.SetActive(false);
                    }
                }
                else
                {
                    int rankCount = rankDataJson.Count;

                    // 받아온 rank 데이터의 숫자만큼 데이터 출력
                    for (int i = 0; i < rankCount; i++)
                    {
                        Text_UserRank = Obj_UserRankContent.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>();
                        Text_UserRankScore = Obj_UserRankContent.transform.GetChild(i).GetChild(4).GetComponent<TextMeshProUGUI>();
                        Text_UserNickName = Obj_UserRankContent.transform.GetChild(i).GetChild(3).GetComponent<TextMeshProUGUI>();
                        Obj_Country = Obj_UserRankContent.transform.GetChild(i).GetChild(2).gameObject;
                        Obj_UserImage = Obj_UserRankContent.transform.GetChild(i).GetChild(1).GetChild(0).GetChild(0).gameObject;

                        Text_UserRank.text = rankDataJson[i]["rank"].ToString();
                        Text_UserRankScore.text = rankDataJson[i]["score"].ToString() + " M";
                        Text_UserNickName.text = rankDataJson[i]["nickname"].ToString();
                        Obj_Country.SetActive(true);
                        Obj_UserImage.SetActive(true);

                        // 복수 데이터의 추가 항목 사용 시
                        //string[] extraData = rankDataJson[i]["WinLose"].ToString().Split("|");
                        //_win = int.Parse(extraData[0].ToString());
                        //_lose = int.Parse(extraData[1].ToString());

                        // 랭킹 데이터 활성화

                    }
                    // rankCount가 Max값만큼 존재하지 않을 때, 나머지 랭킹
                    for (int i = rankCount; i < MAX_RANK_LIST; i++)
                    {
                        // 랭킹 데이터 비활성화
                        Text_UserRank = Obj_UserRankContent.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>();
                        Text_UserRankScore = Obj_UserRankContent.transform.GetChild(i).GetChild(4).GetComponent<TextMeshProUGUI>();
                        Text_UserNickName = Obj_UserRankContent.transform.GetChild(i).GetChild(3).GetComponent<TextMeshProUGUI>();
                        Obj_Country = Obj_UserRankContent.transform.GetChild(i).GetChild(2).gameObject;
                        Obj_UserImage = Obj_UserRankContent.transform.GetChild(i).GetChild(1).GetChild(0).GetChild(0).gameObject;

                        Text_UserRank.text = "-";
                        Text_UserRankScore.text = "-";
                        Text_UserNickName.text = "-";
                        Obj_Country.SetActive(false);
                        Obj_UserImage.SetActive(false);
                    }
                }
            }
            // JSON 데이터 파싱 실패
            catch (System.Exception e)
            {
                Debug.LogError(e);

                for (int i = 0; i < MAX_RANK_LIST; i++)
                {
                    // 랭킹 데이터 비활성화
                    Text_UserRank = Obj_UserRankContent.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>();
                    Text_UserRankScore = Obj_UserRankContent.transform.GetChild(i).GetChild(4).GetComponent<TextMeshProUGUI>();
                    Text_UserNickName = Obj_UserRankContent.transform.GetChild(i).GetChild(3).GetComponent<TextMeshProUGUI>();
                    Obj_Country = Obj_UserRankContent.transform.GetChild(i).GetChild(2).gameObject;
                    Obj_UserImage = Obj_UserRankContent.transform.GetChild(i).GetChild(1).GetChild(0).GetChild(0).gameObject;

                    Text_UserRank.text = "-";
                    Text_UserRankScore.text = "-";
                    Text_UserNickName.text = "-";
                    Obj_Country.SetActive(false);
                    Obj_UserImage.SetActive(false);
                }
            }
        }
        else
        {
            Debug.LogError("랭킹 조회에 실패했습니다.");

            for (int i = 0; i < MAX_RANK_LIST; i++)
            {
                // 랭킹 데이터 비활성화
                Text_UserRank = Obj_UserRankContent.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>();
                Text_UserRankScore = Obj_UserRankContent.transform.GetChild(i).GetChild(4).GetComponent<TextMeshProUGUI>();
                Text_UserNickName = Obj_UserRankContent.transform.GetChild(i).GetChild(3).GetComponent<TextMeshProUGUI>();
                Obj_Country = Obj_UserRankContent.transform.GetChild(i).GetChild(2).gameObject;
                Obj_UserImage = Obj_UserRankContent.transform.GetChild(i).GetChild(1).GetChild(0).GetChild(0).gameObject;

                Text_UserRank.text = "-";
                Text_UserRankScore.text = "-";
                Text_UserNickName.text = "-";
                Obj_Country.SetActive(false);
                Obj_UserImage.SetActive(false);
            }
        }
    }
    #endregion

    #region Button
    private void OnClick_Exit()
    {
        SoundManager.Instance.StartSFX("Click");

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
        SoundManager.Instance.StartSFX("Click");
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");
    }

    // 유저 데이터를 초기화
    private void OnClick_Delete()
    {
        SoundManager.Instance.StartSFX("Click");
        PlayerPrefs.DeleteAll();
        OnClick_Exit();
    }
    #endregion

    private void SetMyRankMedal(int Rank)
    {
        for (int i = 0; i < Obj_Medals.Length; i++)
        {
            Obj_Medals[i].gameObject.SetActive(false);
        }

        if(Rank == 1)
            Obj_Medals[0].gameObject.SetActive(true);
        else if (Rank == 2)
            Obj_Medals[1].gameObject.SetActive(true);
        else if (Rank == 3)
            Obj_Medals[2].gameObject.SetActive(true);
    }
}
