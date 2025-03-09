using UnityEngine;

public enum NeonType
{
    Square,
    Circle,

    Max
}


public class Neon : MonoBehaviour
{
    private NeonType m_NeonType = NeonType.Max;
    private int m_NeonNum = -1;
    private SpriteRenderer m_SpriteRender = null;
    private Rigidbody2D m_Rigdbody = null;
    private BoxCollider2D m_BoxCollider = null;
    private CircleCollider2D m_CircleCollider = null;
    private GameController m_GameController = null;
    private float m_RotationSpeed = 0f;
    private bool IsLeft = true;

    #region Untiy Method
    private void Awake()
    {
        m_SpriteRender = GetComponent<SpriteRenderer>();
        m_Rigdbody = GetComponent<Rigidbody2D>();
        m_GameController = GameObject.FindWithTag("GameController").transform.GetComponent<GameController>();
    }

    private void Start()
    {
        if (IsLeft)
        {
            m_Rigdbody.angularVelocity = m_RotationSpeed;
        }
        else
        {
            m_Rigdbody.angularVelocity = -m_RotationSpeed;
        }
    }
    #endregion

    public void InitSquareNeon(float Width, float Height, float Gravity, float RotationSpeed)
    {
        m_NeonType = NeonType.Square;
        m_BoxCollider = GetComponent<BoxCollider2D>();
        m_NeonNum = m_GameController.GetSpawnedNeonCount();

        SetSize(Width, Height);
        SetGravityScale(Gravity);
        SetRotation(RotationSpeed);
    }

    public void InitCircleNeon(float Width, float Gravity)
    {
        m_NeonType = NeonType.Circle;
        m_CircleCollider = GetComponent<CircleCollider2D>();
        m_NeonNum = m_GameController.GetSpawnedNeonCount();

        SetSize(Width, Width);
        SetGravityScale(Gravity);
    }

    private void SetSize(float Width, float Height)
    {
        // Neon Size
        m_SpriteRender.size = new Vector2(Width, Height);

        // Square Neon 일 때
        if(m_NeonType == NeonType.Square)
        {
            m_BoxCollider.size = new Vector2(Width - 0.6f, Height - 0.6f);
        }
        else
        {
            m_CircleCollider.radius = Width * 0.428f;
        }
    }

    private void SetGravityScale(float Gravity)
    {
        m_Rigdbody.gravityScale = Gravity;
    }

    private void SetRotation(float RotationSpeed)
    {
        m_RotationSpeed = RotationSpeed;

        int RandomValue = Random.Range(0, 2);

        if (RandomValue == 0) IsLeft = true;
        else IsLeft = false;
    }

    // 네온이 완전히 멈춘 상태인지 판단
    public bool IsStopped()
    {
        return m_Rigdbody.IsSleeping();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 다른 네온이나 바닥에 닿을 때 더 이상 조작을 못하도록
        if (collision.transform.CompareTag("Neon") || collision.transform.CompareTag("Floor"))
        {
            m_GameController.IsFalling = false;
        }

        // 네온이 벽에 닿으면 게임 오버
        if (collision.transform.CompareTag("Wall"))
        {
            m_GameController.GameOver();
        }

        // 제일 마지막에 생성된 네온이 네온이나 바닥과 충돌할 때
        if (m_NeonNum == m_GameController.GetSpawnedNeonCount())
        {
            if (collision.transform.CompareTag("Neon") || collision.transform.CompareTag("Floor"))
            {
                int RandomValue = Random.Range(0, 2);

                if(RandomValue == 0)
                    SoundManager.Instance.StartSFX("Neon");
                else
                    SoundManager.Instance.StartSFX("Neon2");
            }
        }
    }
}
