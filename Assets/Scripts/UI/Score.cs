using UnityEngine;

public class Score : MonoBehaviour
{
    private Vector3 m_Pos = new Vector3(1000, -350, 0);

    private void OnEnable()
    {
        //transform.position = m_Pos;
    }

    private void OnDisable()
    {
        //transform.position = m_Pos;
    }

    public void SetActiveFalse()
    {
        transform.GetComponent<RectTransform>().anchoredPosition = m_Pos;
        gameObject.SetActive(false);
    }
}
