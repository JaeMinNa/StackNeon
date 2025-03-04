using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameController : MonoBehaviour
{
    #region Cashed Object
    [SerializeField] private GameObject[] Obj_Neons;
    #endregion

    [Header("Controller")]
    public float SpawnHeight = 6f;
    public float GravityScale = 0.1f;
    public float MaxSize = 8f;
    public float MinSize = 7f;
    public float CameraHeight = 5f;
    public float RotationSpeed = 100f;

    // 이미 땅에 쌓여진 Neon
    private List<GameObject> m_SpawnedNeons = new List<GameObject>();

    private Vector3 m_SpawnPosition;
    public bool IsSpawn { get; set; } = false;

    #region Get/Set
    #endregion

    #region Unity Method
    private void Awake()
    {
        SetSpawnPosition();
    }

    private void Start()
    {
        SpawnNeon();
    }

    private void Update()
    {
        SetSpawnPosition();

        if(m_SpawnedNeons.Count > 0 && m_SpawnedNeons[m_SpawnedNeons.Count - 1].GetComponent<Neon>().IsStopped())
        {
            SpawnNeon();
        }
    }
    #endregion

    public void SpawnNeon()
    {
        IsSpawn = true;

        // Neon 랜덤 선택
        int RandomNeon = Random.Range(0, 3);

        // 랜덤 Size
        float RandomWidth = Random.Range(MinSize, MaxSize);
        float RandomHeight = Random.Range(MinSize, MaxSize);

        GameObject NeonObj = Instantiate(Obj_Neons[RandomNeon], m_SpawnPosition, Quaternion.identity);
        NeonObj.GetComponent<Neon>().InitNeon(RandomWidth, RandomHeight, GravityScale, RotationSpeed);
            
        m_SpawnedNeons.Add(NeonObj);

        if(m_SpawnedNeons.Count > 2) 
            SetCameraPosition();
    }

    private void SetSpawnPosition()
    {
        m_SpawnPosition = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + SpawnHeight, 0);
    }

    private void SetCameraPosition()
    {
        // 가장 마지막에 쌓여진 Neon의 위치에 따라 카메라 위치 설정
        Camera.main.transform.position = new Vector3(0, m_SpawnedNeons[m_SpawnedNeons.Count - 2].transform.position.y + CameraHeight, -10);
    }
}
