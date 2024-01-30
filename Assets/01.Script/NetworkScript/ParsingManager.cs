using System.Collections;
using System.Collections.Generic;
using System.Text;
using BackEnd;
using BackEnd.Tcp;
using UnityEngine;
using UnityEngine.Tilemaps;

#region ParsingClass
public class ParsingData{
    public ParsingType type;
    public string data;
    //data는 type에 맞춰서 각 클래스를 data로 파싱.

    public ParsingData(ParsingType _type, string _data){
        type = _type;
        data = _data;
    }
}

public class DicePosData{
    public ParsingType type;
    public Vector3 data;

    public DicePosData(ParsingType _type, Vector3 _data){
        type = _type;
        data = _data;
    }
}

#endregion

#region Enum_ParsingType
public enum ParsingType{
    Turn,
    Dice
}
#endregion


//데이터가 송신될때의 클래스 및 데이터 파싱형태를 지정합니다.

public class ParsingManager : MonoBehaviour
{
    #region Instance_Parsing
    private static ParsingManager _instance;

    public static ParsingManager Instance{
        get{
            if(_instance == null)
                _instance = FindObjectOfType(typeof(ParsingManager)) as ParsingManager;

            return _instance;
        }
    }
    #endregion

    public void ParisngRecvData(MatchRelayEventArgs args){
        //받는 함수(받는 데이터는 byte[]로 받음.
        //수신이벤트에서 각 클래스로 변환하는 함수.
        byte[] data = args.BinaryUserData;
        ParsingData pData = JsonUtility.FromJson<ParsingData>(Encoding.Default.GetString(data));
        //pData.type : 데이터의 타입, pData.data : string데이터 (클래스별 데이터라 각 클래스에 맞는 파싱과정 필요)
        //데이터의 타입으로 스위치문 결정, 데이터를 다시 위와 같은 과정으로 알맞은 클래스로 변환 후 사용.
        switch(pData.type){
            case ParsingType.Turn:
                TurnCard tData = JsonUtility.FromJson<TurnCard>(pData.data);
                GameManager.Instance.playerCount.Add(1);
                GameManager.Instance.turnCards[tData.turncardIdx].SetActive(false); 
                if(GameManager.Instance.playerCount.Count > 1){
                    GameManager.Instance.turnCardParent.SetActive(false);
                }
            break;
        }
    }

    public byte[] ParsingSendData(ParsingType _type, string _jsonData){
        //전달하는 함수. 전달값은 byte[]로 전달.
        //클래스를 선언한 다음 이 함수 사용
        //string jsonData = JsonUtility.ToJson(data);로 바뀐 데이터를 전달.
        ParsingData data = new ParsingData(_type,_jsonData);
        string jsonData = JsonUtility.ToJson(data);
        return Encoding.UTF8.GetBytes(jsonData); //반환값을 Backend.Match.SendDataToinGameRoom으로 전달.
    }


}

