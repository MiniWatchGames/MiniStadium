using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FishNet.Connection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;
    #region 플레이어 데이터 만들기
    
    private string dataPath;
    private string accountPath;
    private List<UserAccountData> userAccountList = new List<UserAccountData>();
    public UserAccountData currentUserAccount;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        dataPath = Application.persistentDataPath;
    }
    
    private void Start()
    {
        userAccountList = LoadAccountsData();
    }
    
    /// <summary>
    /// 유저 데이터 List를 '외부에' 저장
    /// </summary>
    private void SaveAccountsData()
    {
        try
        {
            AccountsSavePath();
            UserAccountListWrapper wrapper = new UserAccountListWrapper { accounts = userAccountList };
            Debug.Log(wrapper.accounts[0].playerNickname);
            string data = JsonUtility.ToJson(wrapper, true);
            File.WriteAllText(accountPath, data);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save account data: " + e.Message);
        }
    }
    
    /// <summary>
    /// 유저 데이터 List 래퍼
    /// </summary>
    [Serializable]
    public class UserAccountListWrapper
    {
        public List<UserAccountData> accounts;
    }
    
    /// <summary>
    /// 유저데이터 저장경로
    /// </summary>
    private void AccountsSavePath()
    {
        accountPath = Path.Combine(dataPath, "userAccount.json");
    }
    
    /// <summary>
    /// 유저 데이터 List를 '외부에서'로드
    /// 프로그램 시작시 최초 1회 시행
    /// </summary>
    private List<UserAccountData> LoadAccountsData()
    {
        AccountsSavePath();
        if (!File.Exists(accountPath))
        {
            return new List<UserAccountData>();
        }
        try
        {
            string data = File.ReadAllText(accountPath);
            UserAccountListWrapper loadedData = JsonUtility.FromJson<UserAccountListWrapper>(data);
            return loadedData?.accounts ?? new List<UserAccountData>();
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load account data: " + e.Message);
            return new List<UserAccountData>();
        }
    }
    
    /// <summary>
    /// 유저 데이터 List에서 '클라이언트로' 플레이어 정보를 로드
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public void SetCurrentUserAccountData(string email)
    {
        for (int i = 0; i < userAccountList.Count; i++)
        {
            if (userAccountList[i].playerEmail == email)
            {
                currentUserAccount = userAccountList[i];
            }
        }
    }
    
    /// <summary>
    /// 회원가입 시 이미 이메일이 있는 경우
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public bool CheckEmailAlreadyExists(string email)
    {
        if (userAccountList != null)
        {
            foreach (var userData in userAccountList)
            {
                if (userData.playerEmail == email) return true;
            }
        }

        return false;
    }
    
    /// <summary>
    /// 회원가임 시 초기 유저 데이터 생성 및 리스트에 저장
    /// </summary>
    /// <param name="playerName"></param>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public bool AddUserAccountData(string playerName, string email, string password, string passwordCheck , out int errorCode)
    {
        errorCode = 0;
        if (string.IsNullOrWhiteSpace(email) 
            || string.IsNullOrWhiteSpace(playerName)
            || string.IsNullOrWhiteSpace(password) 
            || string.IsNullOrWhiteSpace(passwordCheck))
        {
            errorCode = 1;
            return false;
        }
        if (CheckEmailAlreadyExists(email))
        {
            errorCode = 2;
            return false;
        }
        if (password != passwordCheck)
        {
            errorCode = 3;
            return false;
        }
        
        currentUserAccount = new UserAccountData();
        currentUserAccount.userIndex = userAccountList.Count;
        currentUserAccount.playerNickname = playerName;
        currentUserAccount.playerEmail = email;
        currentUserAccount.playerPassword = password;
        currentUserAccount.playerId = RandomIDGenerator.GenerateClientId(10);
        userAccountList.Insert(currentUserAccount.userIndex, currentUserAccount);
        SaveAccountsData();
        return true;
    }
    
    /// <summary>
    /// 클라이언트에서 현재 유저 정보를 변경할 때
    /// </summary>
    /// <param name="userIndex"></param>
    /// <returns></returns>
    public UserAccountData ChangeCurrentUserAccountData(int userIndex)
    {
        return userAccountList[userIndex] = currentUserAccount;
    }
    
    /// <summary>
    /// 비밀번호와 이메일이 맞는지 확인(로그인 가능한지)
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public bool ComparePassword(string email, string password)
    {
        foreach (var userData in userAccountList)
        {
            if (userData.playerEmail == email)
            {
                if (userData.playerPassword == password)
                {
                    SetCurrentUserAccountData(email);
                    return true;
                }
            }
        }

        return false;
    }
    
    #endregion
    
}

[Serializable]
public class UserAccountData
{
    public int userIndex;
    public string playerId;
    public string playerNickname;
    public string playerEmail;
    public string playerPassword;
    public int playerTierScore;
    public int Score = 0;
    public NetworkConnection conn;
    public string enemyNickname;
}

public static class RandomIDGenerator
{
    private static readonly string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private static System.Random random = new System.Random();

    public static string GenerateClientId(int length)
    {
        StringBuilder sb = new StringBuilder(length);
        for (int i = 0; i < length; i++)
        {
            int index = random.Next(chars.Length);
            sb.Append(chars[index]);
        }
        return sb.ToString();
    }
}
