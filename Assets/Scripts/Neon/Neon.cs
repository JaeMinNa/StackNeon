using UnityEngine;

public class Neon : MonoBehaviour
{
    private SpriteRenderer m_SpriteRender = null;
    private Rigidbody2D m_Rigdbody = null;
    private BoxCollider2D m_BoxCollider = null;
    private GameController m_GameController = null;
    private float m_RotationSpeed = 0f;
    private bool IsLeft = true;

    #region Untiy Method
    private void Awake()
    {
        m_SpriteRender = GetComponent<SpriteRenderer>();
        m_Rigdbody = GetComponent<Rigidbody2D>();
        m_BoxCollider = GetComponent<BoxCollider2D>();
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

    private void Update()
    {
        //if(IsStopped() && !m_GameController.IsSpawn)
        //{
        //    m_GameController.IsSpawn = false;
        //    m_GameController.SpawnNeon();
        //}

        //if(IsLeft)
        //{
        //    m_Rigdbody.angularVelocity = m_RotationSpeed;
        //}
        //else
        //{
        //    m_Rigdbody.angularVelocity = -m_RotationSpeed;
        //}
    }
    #endregion

    public void InitNeon(float Width, float Height, float Gravity, float RotationSpeed)
    {
        SetSize(Width, Height);
        SetGravityScale(Gravity);
        SetRotation(RotationSpeed);
    }

    private void SetSize(float Width, float Height)
    {
        // Neon Size
        m_SpriteRender.size = new Vector2(Width, Height);

        // Neon Collider
        m_BoxCollider.size = new Vector2(Width - 0.6f, Height - 0.6f);
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

    public bool IsStopped()
    {
        return m_Rigdbody.IsSleeping();
    }
}
