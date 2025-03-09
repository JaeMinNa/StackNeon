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
        BackendReturnObject bro = Backend.Initialize(); // 뒤끝 초기화

        // 뒤끝 초기화에 대한 응답값
        if (bro.IsSuccess())
        {
            Debug.Log("뒤끝 서버 연동 성공 : " + bro); // 성공일 경우 statusCode 204 Success
        }
        else
        {
            Debug.LogError("뒤끝 서버 연동 실패 : " + bro); // 실패일 경우 statusCode 400대 에러 발생
        }
    }

    // param : 데이터를 송수신할 때 사용하는 class
    private Param GetUserDataParam()
    {
        Param param = new Param();
        //param.Add("UserName", PlayerPrefs.GetString("NickName"));
        param.Add("RankScore", 0);



        // 랭킹 데이터에서 복수 데이터의 추가 항목 사용 시
        //param.Add("WinLose", GameManager.I.DataManager.GameData.Win.ToString() + "|" + GameManager.I.DataManager.GameData.Lose.ToString());

        return param;
    }

    #region Insert
    // 회원 가입을 할 때, 데이터 테이블에 추가하는 함수
    public void InsertData()
    {
        Param param = GetUserDataParam();
        BackendReturnObject bro = Backend.GameData.Insert("UserData", param); // 데이터 테이블의 이름
        bro = Backend.BMember.UpdateNickname(PlayerPrefs.GetString("NickName"));    // 닉네임 설정

        if (bro.IsSuccess())
        {
            Debug.Log("데이터 추가를 성공했습니다");
        }
        else
        {
            Debug.Log("데이터 추가를 실패했습니다");
        }
    }
    #endregion

    #region Load
    // 서버로부터 데이터를 불러와서 Parsing하는 함수
    public void Load()
    {
        if (!Backend.IsInitialized)
        {
            Debug.LogError("현재 서버와 연결이 끊겼습니다.");
            return;
        }

        BackendReturnObject bro = Backend.GameData.GetMyData("UserData", new Where());

        if (bro.IsSuccess())
        {
            Debug.Log("데이터 로드 성공했습니다.");
            ParsingData(bro.GetReturnValuetoJSON()["rows"][0]);
            // 서버에서 불러온 Json 데이터를 파싱
            // Json 데이터 중, rows의 값만 가져옴
        }
        else
        {
            Debug.Log("데이터 로드 실패했습니다.");
        }
    }

    private void ParsingData(JsonData json)
    {
        // 파싱된 데이터를 GameData로 불러오기
        //GameManager.I.DataManager.GameData.UserName = json["UserName"][0].ToString();
        //GameManager.I.DataManager.GameData.RankPoint = int.Parse(json["RankPoint"][0].ToString());

        // 랭킹 데이터에서 복수 데이터의 추가 항목 사용 시
        //string[] extraData = json["extraData"].ToString().Split("|");
        //GameManager.I.DataManager.GameData.Win = int.Parse(extraData[0].ToString());
        //GameManager.I.DataManager.GameData.Lose = int.Parse(extraData[1].ToString());

        //GameData의 변수가 배열이라면 ?
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
            Debug.LogError("현재 서버와 연결이 끊겼습니다.");
            return;
        }

        Param param = GetUserDataParam();
        BackendReturnObject bro = Backend.GameData.Update("UserData", new Where(), param);

        if (bro.IsSuccess())
        {
            Debug.Log("데이터 저장 성공했습니다.");
        }
        else
        {
            Debug.Log("데이터 저장 실패했습니다.");
        }
    }
    #endregion

    #endregion
}