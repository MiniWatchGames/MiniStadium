using System;
using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Object;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager instance { get; private set; }
    
    #region 유저 진입 및 퇴장

    //서버 프로퍼티
    private Dictionary<string, UserAccountData> joinedUserList = new Dictionary<string, UserAccountData>();

    //서버 클라이언트 공유 프로퍼티
    public bool _canStartGame = false;

    //클라이언트 프로퍼티
    public Action OnChangedStartState;
    
    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        instance = this;
    }

    public override void OnStopNetwork()
    {
        base.OnStopNetwork();
        if (instance == this)
            instance = null;
    }
    
    public override void OnStartClient()
    {
        base.OnStartClient();
        
        if (!IsOwner)
        {
            // 자신이 소유한 객체가 아니라면 행동 차단 (필요 없다면 제거 가능)
            enabled = false;
            return;
        }

        if (instance == null)
            instance = this;
    }
    public override void OnStopClient()
    {
        base.OnStopClient();
        if (instance == this)
            instance = null;
    }
    public void ClearJoinedUserList()
    {
        joinedUserList.Clear();
    }

    public void DebugPlayerManager()
    {
        Debug.Log("hello");
    }

    /// <summary>
    /// 유저가 서버에 진입하면 서버에 진입 요청을 보냄
    /// </summary>
    /// <param name="user"></param>
    [ServerRpc(RequireOwnership = false)]
    public void RequestUserIn(UserAccountData user)
    {
        Debug.Log(user.ToString());
        HandleUserIn(user);
    }
    
    /// <summary>
    /// 유저가 서버에 퇴장하면 서버에 퇴장 요청을 보냄
    /// </summary>
    /// <param name="user"></param>
    [ServerRpc(RequireOwnership = false)]
    public void RequestUserOut(UserAccountData user)
    {
        HandleUserOut(user);
    }
    
    /// <summary>
    /// 서버에서 유저 입장 처리
    /// </summary>
    [Server]
    private void HandleUserIn(UserAccountData user)
    {
        if (!joinedUserList.ContainsKey(user.playerId))
        {
            joinedUserList.Add(user.playerId, user);
            Debug.Log($"User Joined: {user.playerId}");
            CheckCanStartGame();
        }
    }
    
    /// <summary>
    /// 서버에서 유저 퇴장 처리
    /// </summary>
    [Server]
    private void HandleUserOut(UserAccountData user)
    {
        if (joinedUserList.ContainsKey(user.playerId))
        {
            joinedUserList.Remove(user.playerId);
            Debug.Log($"User Left: {user.playerId}");
            _canStartGame = false;
            CheckCanStartGame();
        }
    }
    
    /// <summary>
    /// 게임 시작 조건 확인 (서버)
    /// </summary>
    [Server]
    private void CheckCanStartGame()
    {
        Debug.Log($"[Server] Joined Count: {joinedUserList.Count}");

        bool newCanStartState = (joinedUserList.Count == 2);
        if (_canStartGame != newCanStartState)
        {
            _canStartGame = newCanStartState;
            NotifyClientsCanStartGame(_canStartGame);
        }
    }

    /// <summary>
    /// 서버가 모든 클라이언트에 게임 시작 가능 상태를 알림
    /// </summary>
    [ObserversRpc]
    private void NotifyClientsCanStartGame(bool canStartGame)
    {
        _canStartGame = canStartGame;
        Debug.Log($"[Client] CanStartGame = {_canStartGame}");

        if (_canStartGame)
            OnChangedStartState?.Invoke();
    }

    #endregion
}
