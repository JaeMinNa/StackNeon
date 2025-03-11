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

    // ������ UUID 
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
                // ��ŷ ������ ��Ȱ��ȭ
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

        // ��ŷ �����͸� ������Ʈ�Ϸ��� ���� �����Ϳ��� ����ϴ� �������� inDate �� �ʿ�
        BackendReturnObject bro = Backend.GameData.GetMyData("UserData", new Where());

        if (!bro.IsSuccess())
        {
            Debug.LogError("��ŷ ������Ʈ�� ���� ������ ��ȸ �� ������ �߻��߽��ϴ�.");
            return;
        }

        Debug.Log("��ŷ ������Ʈ�� ���� ������ ��ȸ�� �����߽��ϴ�.");

        if (bro.FlattenRows().Count > 0)
        {
            rowInDate = bro.FlattenRows()[0]["inDate"].ToString();
        }
        else
        {
            Debug.LogError("�����Ͱ� �������� �ʽ��ϴ�.");
        }

        Param param = new Param()
        {
            {"RankScore",  Value}
        };

        // �ش� ���������̺��� �����͸� �����ϰ�, ��ŷ ������ ���� ����
        bro = Backend.URank.User.UpdateUserScore(RANK_UUID, "UserData", rowInDate, param);

        if (bro.IsSuccess())
        {
            Debug.Log("��ŷ ��Ͽ� �����߽��ϴ�.");
        }
        else
        {
            Debug.LogError("��ŷ ��Ͽ� �����߽��ϴ�.");
        }
    }

    public void GetMyRank()
    {
        // �� ��ŷ ���� �ҷ�����
        BackendReturnObject bro = Backend.URank.User.GetMyRank(RANK_UUID);

        if (bro.IsSuccess())
        {
            try
            {
                JsonData rankDataJson = bro.FlattenRows();

                // �޾ƿ� �������� ������ 0 -> �����Ͱ� ����
                if (rankDataJson.Count <= 0)
                {
                    Debug.Log("���� ��ŷ �����Ͱ� �������� �ʽ��ϴ�.");

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

                    // ���� �������� �߰� �׸� ��� ��
                    //string[] extraData = rankDataJson[i]["WinLose"].ToString().Split("|");
                    //_win = int.Parse(extraData[0].ToString());
                    //_lose = int.Parse(extraData[1].ToString());

                    // ��ŷ �����Ͱ� ���̵��� ����
                }
            }
            // ���� ��ŷ ���� JSON ������ �Ľ̿� �������� ��
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
            // ���� ��ŷ ������ �ҷ����µ� �������� ��

            Text_MyRank.text = "-";
            Text_MyRankScore.text = "-";
            Text_MyNickName.text = "-";
        }
    }

    public void GetRankList()
    {
        // ��ŷ ���̺� �ִ� ������ offset ~ offset + limit ���� ��ŷ ������ �ҷ���
        BackendReturnObject bro = Backend.URank.User.GetRankList(RANK_UUID, MAX_RANK_LIST, 0);

        if (bro.IsSuccess())
        {
            // JSON ������ �Ľ� ����
            try
            {
                Debug.Log("��ŷ ��ȸ�� �����߽��ϴ�.");
                JsonData rankDataJson = bro.FlattenRows();

                // �޾ƿ� �������� ������ 0 -> �����Ͱ� ����
                if (rankDataJson.Count <= 0)
                {
                    Debug.Log("��ŷ �����Ͱ� �������� �ʽ��ϴ�.");

                    for (int i = 0; i < MAX_RANK_LIST; i++)
                    {
                        // ��ŷ ������ ��Ȱ��ȭ
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

                    // �޾ƿ� rank �������� ���ڸ�ŭ ������ ���
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

                        // ���� �������� �߰� �׸� ��� ��
                        //string[] extraData = rankDataJson[i]["WinLose"].ToString().Split("|");
                        //_win = int.Parse(extraData[0].ToString());
                        //_lose = int.Parse(extraData[1].ToString());

                        // ��ŷ ������ Ȱ��ȭ

                    }
                    // rankCount�� Max����ŭ �������� ���� ��, ������ ��ŷ
                    for (int i = rankCount; i < MAX_RANK_LIST; i++)
                    {
                        // ��ŷ ������ ��Ȱ��ȭ
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
            // JSON ������ �Ľ� ����
            catch (System.Exception e)
            {
                Debug.LogError(e);

                for (int i = 0; i < MAX_RANK_LIST; i++)
                {
                    // ��ŷ ������ ��Ȱ��ȭ
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
            Debug.LogError("��ŷ ��ȸ�� �����߽��ϴ�.");

            for (int i = 0; i < MAX_RANK_LIST; i++)
            {
                // ��ŷ ������ ��Ȱ��ȭ
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
