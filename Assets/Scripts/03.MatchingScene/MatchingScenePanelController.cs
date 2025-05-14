using System;
using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Connection;
using FishNet.Demo.AdditiveScenes;
using FishNet.Managing;
using FishNet.Managing.Client;
using FishNet.Transporting;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MatchingScenePanelController : PanelController
{
    [Header("[PopupPanel] MatchMaking")]
    [SerializeField] private TextMeshProUGUI matchingText; // "매칭이진행중입니다..." 문구 텍스트
    [SerializeField] private TextMeshProUGUI gameTipMessage; // 게임팁 문구 텍스트
    [SerializeField] private Image progressBar; // Fill 타입 Image로 설정된 진행바

    [SerializeField] private TextMeshProUGUI _playerName;
    [SerializeField] private TextMeshProUGUI _enemyName;

    [Header("[PopupPanel] MatchFinish")]
    [SerializeField] private GameObject popupMatchFinish; // [PopupPanel] MatchFinish 

    public float fillSpeed = 0.3f; // 진행바 진행 속도
    public float targetProgress = 1f;
    
    private float currentProgress = 0f;
    private bool isFinished = false;

    private PlayerManager _playerManager;
    private DataManager _dataManager;
    // 랜덤으로 보여줄 게임TIP 문구 리스트
    private List<string> tipMessages = new List<string>()
    {
        "확인용 팁 메시지 1",
        "확인용 팁 메시지 2",
        "확인용 팁 메시지 3",
        "확인용 팁 메시지 4",
        "확인용 팁 메시지 5"
    };

    private void Start()
    {
        SetRandomTipMessages(); // 매칭 문구 설정
        _dataManager = FindObjectOfType<DataManager>();
        InstanceFinder.ClientManager.OnClientConnectionState += HandleClientConnectionState;

        // "매칭진행중입니다..." 텍스트 점 개수 반복
        StartCoroutine(AnimationDots());
    }
    private void HandleClientConnectionState(ClientConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Started)
        {
            //_playerManager = FindObjectOfType<PlayerManager>();
            StartCoroutine(WaitForPlayerManager());
        }
    }
    private IEnumerator WaitForPlayerManager()
    {
        while (!InstanceFinder.ClientManager.Connection.IsValid || (_playerManager = FindObjectOfType<PlayerManager>()) == null)
            yield return null;
        _playerManager = FindObjectOfType<PlayerManager>();
        NetworkConnection myConn = InstanceFinder.ClientManager.Connection;
        _dataManager.currentUserAccount.conn = myConn;
        _playerManager.RequestUserIn(_dataManager.currentUserAccount);
        _playerManager.OnChangedStartState += OnMatchComplete;
    }
    
    private void SetRandomTipMessages()
    {
        if (gameTipMessage != null && tipMessages.Count > 0)
        {
            int randomIndex = Random.Range(0, tipMessages.Count);
            gameTipMessage.text = tipMessages[randomIndex];
        }
    }

    private void OnMatchComplete()
    {
        isFinished = true;
        popupMatchFinish.SetActive(true); // [PopupPanel] MatchFinish 패널 띄우기
        _playerManager.RequestJoinUserList(_dataManager.currentUserAccount.conn);
        _playerManager.OnCanGetList += SetEnemyName;
    }

    private void SetEnemyName(Dictionary<string, UserAccountData> list)
    {
        foreach (var userAccount in list)
        {
            if (userAccount.Key == _dataManager.currentUserAccount.playerId)
            {
                _dataManager.currentUserAccount.enemyNickname = userAccount.Value.enemyNickname;
                _playerName.text = _dataManager.currentUserAccount.playerNickname;
                _enemyName.text = _dataManager.currentUserAccount.enemyNickname;
                StartCoroutine(WaitAndLoadInGame());
            }
        }
    }

    #region 코루틴
    
    IEnumerator AnimationDots()
    {
        string baseText = "매칭이진행중입니다";
        int dotCount = 0;

        while (!isFinished)
        {
            dotCount = (dotCount % 3) + 1;
            matchingText.text = baseText + new string('.', dotCount);

            yield return new WaitForSeconds(0.5f); // 점 변경 시간 간격
        }
    }

    IEnumerator WaitAndLoadInGame()
    {
        yield return new WaitForSeconds(5f); // 5초 대기
        _playerManager.RequestSceneChange("InGameScene");
    }

    #endregion
}
