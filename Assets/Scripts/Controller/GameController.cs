using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System;
using static UnityEngine.GraphicsBuffer;

public class GameController : MonoBehaviour
{
    #region Cashed Object
    [SerializeField] private GameObject[] Obj_SquareNeons;
    [SerializeField] private GameObject[] Obj_CircleNeons;
    [SerializeField] private TMP_Text Text_Score;
    #endregion

    [Header("Sqaure")]
    public float SquareMaxSize = 8f;
    public float SquareMinSize = 7f;
    public float SquareMinRotationSpeed = 20;
    public float SquareMaxRotationSpeed = 100;

    [Header("Circle")]
    public float CircleMaxSize = 3f;
    public float CircleMinSize = 1f;

    [Header("Controller")]
    public int SpawnCircleNeonRate = 50;    // Circle�� ������ Ȯ�� (0 ~ 100)
    public float SpawnHeight = 6f;
    public float GravityScale = 0.1f;
    public float CameraMoveStartStandardHeight = 1f;
    public float CameraMoveHeight = 1f;
    public float CameraMoveTime = 0.3f;

    [Header("Popup")]
    [SerializeField] private GameObject Obj_PopupNickName = null;
    [SerializeField] private GameObject Obj_PopupRanking = null;

    public bool IsFalling { get; set; } = false;    // �׿��� ���� ������ �������� ���� ��������

    // �̹� ���� �׿��� Neon
    private List<GameObject> m_SpawnedNeons = new List<GameObject>();

    private float m_LastCameraMoveHeight = 0;
    private Vector3 m_SpawnPosition;
    private bool m_IsDragging = false;  // �׿��� �巡�� ������
    private bool m_IsCameraMove = false;
    private Vector3 m_Velocity = Vector3.zero;

    #region Get/Set
    public int GetSpawnedNeonCount()
    {
        return m_SpawnedNeons.Count;
    }

    // ���� �������� �ִ� �׿��� �����ϰ� ���� �����ִ� �׿��� Y���� ������
    private float GetHighestNeonPosY()
    {
        float PosY = float.MinValue;

        for (int Index = 0; Index < m_SpawnedNeons.Count; ++Index)
        {
            if (Index == m_SpawnedNeons.Count - 1) break;

            if (PosY < m_SpawnedNeons[Index].transform.position.y)
                PosY = m_SpawnedNeons[Index].transform.position.y;
        }

        return PosY;
    }

    private float GetScore()
    {
        float Score = (float)Math.Round(GetHighestNeonPosY() + 4f, 1);
        if (Score < 0) Score = 0;

        return Score;
    }
    #endregion

    #region Unity Method
    private void Awake()
    {
        SetSpawnPosition();
    }

    private void Start()
    {
        // �г����� ���ٸ� �г��� ����
        if (PlayerPrefs.GetString("NickName") == "")
            SetNickName();

        SpawnNeon();
    }

    private void Update()
    {
        SetSpawnPosition();

        // �׿� ����
        if(m_SpawnedNeons.Count > 0 && m_SpawnedNeons[m_SpawnedNeons.Count - 1].GetComponent<Neon>().IsStopped())
        {
            SpawnNeon();
        }

        // ������ �׿� �����̱�
        if (IsFalling) MoveNeon();

        // ī�޶� �̵�
        if (GetHighestNeonPosY() - m_LastCameraMoveHeight > CameraMoveStartStandardHeight)
            SetCameraPosition();
    }
    #endregion

    public void SpawnNeon()
    {
        IsFalling = true;

        int RandomValue = UnityEngine.Random.Range(1, 101);
        int RandomNeon = UnityEngine.Random.Range(0, 3);

        // Spawn Square Neon
        if (RandomValue > SpawnCircleNeonRate)
        {
            // ���� Size
            float RandomWidth = UnityEngine.Random.Range(SquareMinSize, SquareMaxSize);
            float RandomHeight = UnityEngine.Random.Range(SquareMinSize, SquareMaxSize);

            // ���� Rotation
            float RandomRotation = UnityEngine.Random.Range(SquareMinRotationSpeed, SquareMaxRotationSpeed);

            GameObject NeonObj = Instantiate(Obj_SquareNeons[RandomNeon], m_SpawnPosition, Quaternion.identity);
            m_SpawnedNeons.Add(NeonObj);
            NeonObj.GetComponent<Neon>().InitSquareNeon(RandomWidth, RandomHeight, GravityScale, RandomRotation);
        }
        else
        {
            // ���� Size
            float RandomWidth = UnityEngine.Random.Range(CircleMinSize, CircleMaxSize);

            GameObject NeonObj = Instantiate(Obj_CircleNeons[RandomNeon], m_SpawnPosition, Quaternion.identity);
            m_SpawnedNeons.Add(NeonObj);
            NeonObj.GetComponent<Neon>().InitCircleNeon(RandomWidth, GravityScale);
        }

        // ���ھ� Ȱ��ȭ
        if(m_SpawnedNeons.Count > 1)
            PlayScoreAnimation();
    }

    private void SetSpawnPosition()
    {
        m_SpawnPosition = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + SpawnHeight, 0);
    }

    // ���� ���� ��ġ�� ���� Neon�� ��ġ�� ���� ī�޶� ��ġ ����
    private void SetCameraPosition()
    {
        float Distance = 0.1f;
        Vector3 TargetPosition = new Vector3(0, GetHighestNeonPosY() + CameraMoveHeight, -10);

        // ��ǥ ��ġ���� �ε巴�� �̵��� �� ����ϴ� �޼ҵ�
        Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, TargetPosition, ref m_Velocity, CameraMoveTime);

        if (Vector3.Distance(Camera.main.transform.position, TargetPosition) < Distance)
        {
            m_LastCameraMoveHeight = GetHighestNeonPosY();
        }

    }

    private void MoveNeon()
    {
        Vector3 Offset = Vector3.zero;
        GameObject NeonObj = m_SpawnedNeons[m_SpawnedNeons.Count - 1];

        // ���콺 Ŭ�� �Ǵ� ��ġ ����
        // Ŭ���� ������Ʈ�� �������� ������ Neon �� ��
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.CompareTag("Neon") && hit.collider.gameObject == NeonObj)
            {
                m_IsDragging = true;
                Offset = NeonObj.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }

        // �巡�� ���� ��
        if (m_IsDragging && Input.GetMouseButton(0))
        {
            Vector3 NewPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Offset;
            NeonObj.transform.position = new Vector3(NewPosition.x, NeonObj.transform.position.y, NeonObj.transform.position.z); // Y�� ����
        }

        // ���콺�� ���ų� ��ġ�� ���� ��
        if (Input.GetMouseButtonUp(0))
        {
            m_IsDragging = false;
        }
    }

    private void PlayScoreAnimation()
    {
        Text_Score.text = GetScore().ToString() + " M";
        Text_Score.gameObject.SetActive(true);
        //Text_Score.GetComponent<DOTweenAnimation>().DOPlay();

        //StartCoroutine(StopScoreAnimation());
    }

    private void SetNickName()
    {
        Time.timeScale = 0f;
        Obj_PopupNickName.SetActive(true);
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        Obj_PopupRanking.SetActive(true);
    }
}
