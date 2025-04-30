using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

//프로그램 시작시 위 경로에서 불러오는 LocalGameData
public class LocalGameData
{
    public bool bKeepLoginEnabled;
    public struct settings
    {
        public float mouseSensitivity;
    }
}
/// <summary>
/// 로그인 완료 시 서버를 통해 불러오는 PlayerData, 
/// 게임 매칭 시 상대방의 PlayerData ( Ingame에서 게임 완료 시 Clear)
/// </summary>

public class PlayerData
{
    public int playerID;
    public string playerNickname;
    public string playerEmailAddress;
    public string playerPassword;
    public int playerTierScore;
}

public class DataManager : Singleton<DataManager>
{
    //로컬 데이터 경로 %appdata%/LocalLow/DefaultCompany/MiniStadium
    public string localDataPath;

    public LocalGameData localGameData;
    public PlayerData playerData;
    public PlayerData enemyData;

    #region DataManager Initialization

    private void Start()
    {
        localDataPath = Application.persistentDataPath + "/LocalGameData.json";
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

    }

    #endregion
    
    public void SaveLocalGameData()
    {
        string data = JsonUtility.ToJson(localGameData);
        File.WriteAllText(localDataPath, data);
    }

    public void LoadLocalGameData()
    {
        if (File.Exists(localDataPath))
        {
            string data = File.ReadAllText(localDataPath);
            localGameData = JsonUtility.FromJson<LocalGameData>(data);   
        }
        else
        {
            localGameData = new LocalGameData();
        }
    }

    public void GetPlayerDataFromServer()
    {
        
    }

    public void ClearPlayerData()
    {
        
    }

    public void GetEnemyDataFromServer()
    {
        
    }
    
    public void ClearEnemyData()
    {
        
    }

}
