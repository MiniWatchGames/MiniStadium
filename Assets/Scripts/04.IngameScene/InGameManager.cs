using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


public class InGameManager : MonoBehaviour
{
    #region States
    public enum GameState
    {
        StartGame,
        InGame,
        EndGame
    }
    public enum RoundState
    {
        RoundStart,
        InRound,
        SuddenDeath,
        RoundEnd
    }
    public enum WinLoseState
    {
        Default,
        Win,
        Lose,
        Duse,
        Draw
    }

    public enum Team
    {
        Player,
        Enemy,
        Count,
    }

    public enum MapSetting
    {
        Map1,
        Map2,
        Map3,
        Count
    }
    #endregion

    #region 변수


    [SerializeField] private GameObject RepairShopUI;
    [SerializeField] private int prizeWinner;
    [SerializeField] private int prizeLoser;
    // [SerializeField] private GameObject FinishRoundUI;
    [SerializeField] private GameObject GameRoundInfoUI;
    [SerializeField] private GameState currentGameState;
    [SerializeField] private WinLoseState currentWinLoseState;
    [SerializeField] private Team currentTeam;
    [SerializeField] private RoundState currentRoundState;
    [SerializeField] private GameObject Damagefield;

    //맵 세팅용 딕셔너리 start()문에서 적용
    Dictionary<GameObject, List<Spawner>> mapSpawners = new Dictionary<GameObject, List<Spawner>>();
    //현재 켜져있는 맵 저장
    GameObject currentMap;
    //맵 센터찾기


    //팀 세팅용 딕셔너리
    Dictionary<GameObject, Team> teamDictionary = new Dictionary<GameObject, Team>();


    [SerializeField] private Timer gameTimer;
    [SerializeField] private GameObject PlayerPrefab;
    [SerializeField] private GameObject EnemyPrefab;
    [SerializeField] private RepairShopTimer RepairShopTimer;
    [SerializeField] private float repairShopTime = 5;
    [SerializeField] private float InRoundTime = 5;
    [SerializeField] private PlayerHud playerHud;
    public PlayerController playerContoroller;
    private GameObject player;
    private GameObject enemyPlayer;
    
    private GameObject playerSpawner;
    private GameObject enemySpawner;
    [SerializeField] public int currentRound = 0;
    public int BlueWinCount = 0;
    public int RedWinCount = 0;
    [SerializeField] private int currentGameTime = 0;
    private List<GameObject> maps = new List<GameObject>();
    public Action inGameUIAction;
    public float timer { get => gameTimer.currentTime; }

    public GameObject GameRoundUI { get => GameRoundInfoUI; }
    public RoundState roundstate { get => currentRoundState; }
    #endregion
    #region StateChangeFunction

    void SetGameState(GameState state)
    {
        switch (state)
        {
            case GameState.StartGame:
                currentGameState = GameState.StartGame;
                SetRoundState(RoundState.RoundStart);
                break;
            case GameState.InGame:
                currentGameState = GameState.InGame;
                break;
            case GameState.EndGame:
                currentGameState = GameState.EndGame;
                SceneManager.LoadScene("MainmenuScene");
                break;
        }
    }
    void SetRoundState(RoundState state)
    {

        switch (state)
        {
            case RoundState.RoundStart:
                currentRound++;
                
                //플레이어 초기화
                if (playerContoroller is not null)
                {
                    playerContoroller.ResetCharacter();
                    
                }
                
                GameRoundInfoUI.gameObject.SetActive(false);
                playerHud.GetComponent<CanvasGroup>().alpha = 1;
                //자기장 필드 크기 초기화 및 대기
                Damagefield.GetComponent<SafeZone>().Reset();
                //RepairShopUI.SetActive(true);
                //현재 라운드 상태 저장
                currentRoundState = RoundState.RoundStart;
                //정비소 타이머 리셋
                RepairShopTimer.SetTime = repairShopTime;
                //시간초가 지난후 다음 게임스테이트로 넘어가기
                SetGameTime(repairShopTime, RoundState.InRound);
                RepairShopUI.GetComponent<RepairShop>().SetRoundText(currentRound);
                break;
            case RoundState.InRound:
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

                inGameUIAction?.Invoke();
                

                // 무기 미구매 시 기본 무기 상점 내 적용
                RepairShopUI.GetComponent<RepairShop>().RepairShopWeapon.NoEmptyHands();

                // 플레이어 없다면 생성, 플레이어에 구매 내역을 넘기고, 플레이어 구매 내역 적용
                if (player is null)
                {
                    player = Instantiate(PlayerPrefab, mapSpawners[currentMap][0].transform.position, Quaternion.identity);
                    playerContoroller = player.GetComponent<PlayerController>();
                }
                SetPlayerTeam(player);
                playerContoroller.PurchaseManager.PurchasedPlayerItems = RepairShopUI.GetComponent<RepairShop>()?.Receipt.PlayerItems.DeepCopy();
                playerContoroller.ReInit();
                playerHud.init(playerContoroller);
                playerContoroller.SetSkillGage(playerHud.GetSkillGage());
                //RepairShopUI.SetActive(!RepairShopUI.activeSelf);
                player.GetComponent<CharacterController>().enabled = false;
                player.gameObject.transform.position = playerSpawner.transform.position;
                
                player.GetComponent<CharacterController>().enabled = true;
                Debug.Log("player spawned ");
                
                currentRoundState = RoundState.InRound;
                SetGameTime(InRoundTime, RoundState.SuddenDeath);
                break;
            case RoundState.SuddenDeath:
                currentRoundState = RoundState.SuddenDeath;

                SetGameTime(100, RoundState.SuddenDeath);

                break;
            case RoundState.RoundEnd:
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                
                currentRoundState = RoundState.RoundEnd;

                playerContoroller.CleanupBeforeReInit();    // 입력 방지, 상태 초기화 
                playerHud.GetComponent<CanvasGroup>().alpha = 0;

                if (BlueWinCount == 4 || RedWinCount == 4)
                {
                    StartCoroutine(EndGame());
                    inGameUIAction?.Invoke();
                    break;
                }

                if (currentGameState == GameState.EndGame)
                {

                    //TODO: 이겼는지 졌는지 UI 띄우기
                }

                // FinishRoundUI.SetActive(true);
                inGameUIAction?.Invoke();
                SetGameTime(5, RoundState.RoundStart);
                RepairShop repairShop = RepairShopUI.GetComponent<RepairShop>();
                repairShop.currentMoney 
                    += currentWinLoseState == WinLoseState.Win? prizeWinner : prizeLoser;
                repairShop.UpdateMoneyText(repairShop.totalPrice);
                break;
        }
    }

    IEnumerator EndGame() {
        yield return new WaitForSeconds(5);
        SetGameState(GameState.EndGame);
    }

    void SetWinLoseState(WinLoseState state)
    {
        switch (state)
        {
            case WinLoseState.Default:
                // Debug.Log("Default");
                currentWinLoseState = WinLoseState.Default;
                break;
            case WinLoseState.Win:
                //Debug.Log("Win");

                currentWinLoseState = WinLoseState.Win;

                break;
            case WinLoseState.Lose:
                //Debug.Log("Lose");
                currentWinLoseState = WinLoseState.Lose;
                break;

        }
    }
    public void SetGameTime(float time, RoundState state)
    {

        if (currentRoundState == RoundState.InRound)
        {
            RepairShopUI.SetActive(false);
        }

        if (currentRoundState == RoundState.RoundStart)
        {
            RepairShopUI.SetActive(true);

            SetWinLoseState(WinLoseState.Default);
        }


        gameTimer.SetTimer(time, Timer.TimerType.Decrease, () =>
        {
            //Debug.Log("Game Time End");
            //gameTimer.OnTimerEndDelegate = null;
            SetRoundState(state);
        }, (time) =>
        {
            if (currentRoundState == RoundState.SuddenDeath)
            {
                //Debug.Log("Sudden Death");
                Damagefield.GetComponent<SafeZone>().UpdateMagneticField(time);
            }

        });
    }

    void SetTeam(Team team, GameObject playerStat)
    {
        switch (team)
        {
            case Team.Player:
                teamDictionary[playerStat] = Team.Player;
                break;
            case Team.Enemy:
                //Debug.Log("Red Team");
                teamDictionary[playerStat] = Team.Enemy;
                break;
        }
    }

    #endregion

    void FindAndMappingSpawner()
    {
        //모든 맵의 스포너 찾기
        List<Spawner> countSpanwer = new List<Spawner>
            (FindObjectsByType<Spawner>(FindObjectsInactive.Include, FindObjectsSortMode.None));

        for (int i = 0; i < countSpanwer.Count; i++)
        {
            //각 스포너의 부모 오브젝트를 가져온다. 그리고 그 오브젝트의 스포너를 맵핑한다
            if (mapSpawners.ContainsKey(countSpanwer[i].transform.parent.gameObject))
            {
                mapSpawners[countSpanwer[i].transform.parent.gameObject].Add(countSpanwer[i]);
                if (countSpanwer[i].transform.parent.gameObject.activeSelf)
                {
                    if(countSpanwer[i].team == Team.Player)
                    {
                        playerSpawner = countSpanwer[i].gameObject;
                    }
                    else if(countSpanwer[i].team == Team.Enemy)
                    {
                        enemySpawner = countSpanwer[i].gameObject;
                    }
                }
            }
            else
            {
                mapSpawners.Add(countSpanwer[i].transform.parent.gameObject, new List<Spawner>());
                mapSpawners[countSpanwer[i].transform.parent.gameObject].Add(countSpanwer[i]);
                if (countSpanwer[i].transform.parent.gameObject.activeSelf)
                {
                    if(countSpanwer[i].team == Team.Player)
                    {
                        playerSpawner = countSpanwer[i].gameObject;
                    }
                    else if(countSpanwer[i].team == Team.Enemy)
                    {
                        enemySpawner = countSpanwer[i].gameObject;
                    }
                }
            }
            
            
        }
        maps.AddRange(GameObject.FindGameObjectsWithTag("Map"));
        for (int i = 0; i < maps.Count; i++)
        {
            if (maps[i].activeSelf == true)
            {
                currentMap = maps[i];
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        ResetRound();

        FindAndMappingSpawner();

        //실제 게임이 실행되게 하는 문장. 게임 상태를 스타트로 만들어준다.
        SetGameState(GameState.StartGame);

        //이거는 없어도 될 듯 하다.쓰이는데가 없어요..

    }

    // Update is called once per frame
    void Update()
    {
        
        // if (currentRound == 3)
        // {
        //     SetGameState(GameState.EndGame);
        // }
    }

    //플레이어가 죽을때 또는 죽였을 때 호출되야한다.
    public void EndRound(GameObject Loser)
    {
        
        
        //라운드가 끝날때 마다 라운트 인포를 켜줘야 하니까.
        GameRoundInfoUI.gameObject.SetActive(true);
        switch (teamDictionary[Loser])
        {
            case Team.Player:
                Debug.Log("Player Lose");
                RedWinCount++;
                break;
            case Team.Enemy:
                BlueWinCount++;
                break;
        }
        SetRoundState(RoundState.RoundEnd);
        Debug.Log($"RedWin{RedWinCount} : Blue Win : {BlueWinCount}");
        //ResetRound();
    }

    public void SetMap(MapSetting map)
    {
        switch (map)
        {
            case MapSetting.Map1:
                string mapName = "Map1";
                Spawner playerSpawn = mapSpawners[maps.Find(x => x.name == mapName).gameObject][0];
                if (playerSpawn.playerPrefab is null)
                {
                    playerSpawn.playerPrefab = PlayerPrefab;

                }
                //Debug.Log("Map1");
                break;
            case MapSetting.Map2:
                //Debug.Log("Map2");
                break;
            case MapSetting.Map3:
                //Debug.Log("Map3");
                break;
        }
    }
    public void SetPlayerTeam(GameObject player)
    {
        if (player is null)
        {
            Debug.Log("Player is null");
            return;
        }
        int tmpRandom = Random.Range(0, 1);

        
        GameObject EnemyPlayer = GameObject.FindWithTag("Enemy");
        
        
        if (EnemyPlayer == null) {
            EnemyPlayer = Instantiate(EnemyPrefab, enemySpawner.transform.position, Quaternion.identity);
        }    
        EnemyPlayer.GetComponent<CharacterController>().enabled = false;
        EnemyPlayer.GetComponent<PlayerController>().InitDummy();
        
        EnemyPlayer.transform.position = enemySpawner.transform.position;
        EnemyPlayer.GetComponent<CharacterController>().enabled = true;
        //if(EnemyPlayer == null) return;
        EnemyPlayer.tag = "Enemy";
        Debug.Log("Player got the Team");

        teamDictionary[player.gameObject] = Team.Player;

        teamDictionary[EnemyPlayer] = Team.Enemy;

        //플레이어가 죽었을때 패배 표시
        player.GetComponent<PlayerController>().OnPlayerDie = (player) =>
        {
            if (currentWinLoseState == WinLoseState.Win)
            {
                //적을 먼저 죽였을때 그 후 죽어도 승리 확정
                return;
            }
            LoseRound(player);
            SetWinLoseState(WinLoseState.Lose);
            
        };
        EnemyPlayer.GetComponent<PlayerController>().OnPlayerDie = (enemy) =>
        {
            if (currentWinLoseState == WinLoseState.Lose)
            {
                //적을 먼저 죽였을때 그 후 죽어도 승리 확정
                return;
            }
            LoseRound(enemy);
            SetWinLoseState(WinLoseState.Win);
            
        };/*
        EnemyPlayer.GetComponent<DummyController>().OnDieCallBack = (enemy) =>
        {
            if (currentWinLoseState == WinLoseState.Lose)
            {
                //적이 나를 먼저 죽였을때 그후 적이 죽으면 상관없이 패배 확정
                return;
            }
            WinRound(enemy);
            SetWinLoseState(WinLoseState.Win);
        };*/
    }
    public void WinRound(GameObject Enemy)
    {
        //UIPopU[pFor Winning Screen
        EndRound(Enemy);

    }
    public void LoseRound(GameObject player)
    {
        //UIPopUpFor Losing Screen
        
        EndRound(player);
    }

    public void ResetRound()
    {

        currentRound = 0;
        BlueWinCount = 0;
        RedWinCount = 0;
    }
}