using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using LitJson;
using DG.Tweening.Core.Easing;

public class BackendManager : MonoBehaviour
{
    public static BackendManager Instance;

    void Awake()
    {
        Instance = this;

        Init();
    }

    public void Init()
    {
        BackendSetup();
    }

    public void Release()
    {

    }

    #region BackEnd
    private void BackendSetup()
    {
        BackendReturnObject bro = Backend.Initialize(); // �ڳ� �ʱ�ȭ

        // �ڳ� �ʱ�ȭ�� ���� ���䰪
        if (bro.IsSuccess())
        {
            Debug.Log("�ڳ� ���� ���� ���� : " + bro); // ������ ��� statusCode 204 Success
        }
        else
        {
            Debug.LogError("�ڳ� ���� ���� ���� : " + bro); // ������ ��� statusCode 400�� ���� �߻�
        }
    }

    // param : �����͸� �ۼ����� �� ����ϴ� class
    private Param GetUserDataParam()
    {
        Param param = new Param();
        //param.Add("UserName", PlayerPrefs.GetString("NickName"));
        param.Add("RankScore", 0);



        // ��ŷ �����Ϳ��� ���� �������� �߰� �׸� ��� ��
        //param.Add("WinLose", GameManager.I.DataManager.GameData.Win.ToString() + "|" + GameManager.I.DataManager.GameData.Lose.ToString());

        return param;
    }

    #region Insert
    // ȸ�� ������ �� ��, ������ ���̺� �߰��ϴ� �Լ�
    public void InsertData()
    {
        Param param = GetUserDataParam();
        BackendReturnObject bro = Backend.GameData.Insert("UserData", param); // ������ ���̺��� �̸�
        bro = Backend.BMember.UpdateNickname(PlayerPrefs.GetString("NickName"));    // �г��� ����

        if (bro.IsSuccess())
        {
            Debug.Log("������ �߰��� �����߽��ϴ�");
        }
        else
        {
            Debug.Log("������ �߰��� �����߽��ϴ�");
        }
    }
    #endregion

    #region Load
    // �����κ��� �����͸� �ҷ��ͼ� Parsing�ϴ� �Լ�
    public void Load()
    {
        if (!Backend.IsInitialized)
        {
            Debug.LogError("���� ������ ������ ������ϴ�.");
            return;
        }

        BackendReturnObject bro = Backend.GameData.GetMyData("UserData", new Where());

        if (bro.IsSuccess())
        {
            Debug.Log("������ �ε� �����߽��ϴ�.");
            ParsingData(bro.GetReturnValuetoJSON()["rows"][0]);
            // �������� �ҷ��� Json �����͸� �Ľ�
            // Json ������ ��, rows�� ���� ������
        }
        else
        {
            Debug.Log("������ �ε� �����߽��ϴ�.");
        }
    }

    private void ParsingData(JsonData json)
    {
        // �Ľ̵� �����͸� GameData�� �ҷ�����
        //GameManager.I.DataManager.GameData.UserName = json["UserName"][0].ToString();
        //GameManager.I.DataManager.GameData.RankPoint = int.Parse(json["RankPoint"][0].ToString());

        // ��ŷ �����Ϳ��� ���� �������� �߰� �׸� ��� ��
        //string[] extraData = json["extraData"].ToString().Split("|");
        //GameManager.I.DataManager.GameData.Win = int.Parse(extraData[0].ToString());
        //GameManager.I.DataManager.GameData.Lose = int.Parse(extraData[1].ToString());

        //GameData�� ������ �迭�̶�� ?
        //for (int i = 0; i < json["Items"]["L"].Count; i++)
        //{
        //    GameManager.I.DataManager.GameData.Items[i] = int.Parse(json["Items"]["L"][i][0].ToString());
        //}
    }
    #endregion

    #region Save
    public void Save()
    {
        if (!Backend.IsInitialized)
        {
            Debug.LogError("���� ������ ������ ������ϴ�.");
            return;
        }

        Param param = GetUserDataParam();
        BackendReturnObject bro = Backend.GameData.Update("UserData", new Where(), param);

        if (bro.IsSuccess())
        {
            Debug.Log("������ ���� �����߽��ϴ�.");
        }
        else
        {
            Debug.Log("������ ���� �����߽��ϴ�.");
        }
    }
    #endregion

    #endregion
}