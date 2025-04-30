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
        Blue,
        Red,
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
    [SerializeField] private GameObject GameRoundInfoUI;
    [SerializeField] private GameState currentGameState;
    [SerializeField] private WinLoseState currentWinLoseState;
    [SerializeField] private Team currentTeam;
    [SerializeField] private RoundState currentRoundState;
    [SerializeField] private GameObject Damagefield;
    Dictionary<GameObject, List<Spawner>> mapSpawners = new Dictionary<GameObject, List<Spawner>>();
    Dictionary<GameObject,Team> teamDictionary = new Dictionary<GameObject,Team>();
    [SerializeField] private Timer gameTimer;
    
    [SerializeField] public int currentRound = 0;
    public int BlueWinCount = 0;
    public int RedWinCount = 0;
    [SerializeField] private int currentGameTime = 0;
    private List<GameObject> maps = new List<GameObject>();
    public Action inGameUIAction;
    public float timer{get => gameTimer.currentTime;}
    public RoundState roundstate{get => currentRoundState;}
    #endregion
    #region StateChangeFunction
    
    void SetGameState(GameState state)
    {
        switch (state)
        {
            case GameState.StartGame:
                //Debug.Log("Start Game");
                currentGameState = GameState.StartGame;
                SetRoundState(RoundState.RoundStart);
                break;
            case GameState.InGame:
                //Debug.Log("In Game");
                currentGameState = GameState.InGame;
                GameRoundInfoUI.GetComponent<TMP_Text>().text = "Game Start";
                break;
            case GameState.EndGame:
               // Debug.Log("End Game");
                currentGameState = GameState.EndGame;
                GameRoundInfoUI.GetComponent<TMP_Text>().text = "Game Start";
                break;
        }
    }
    void SetRoundState(RoundState state)
    {
        inGameUIAction?.Invoke();
        switch (state)
        {
            case RoundState.RoundStart:
                //Debug.Log("Round Start");
                currentRoundState = RoundState.RoundStart;
                SetGameTime(5, RoundState.InRound);
                break;
            case RoundState.InRound:
                //Debug.Log("In Round");
                currentRoundState = RoundState.InRound;
                SetGameTime(5, RoundState.RoundEnd);
                break;
            case RoundState.RoundEnd:
                //Debug.Log("Round End");
                currentRoundState = RoundState.RoundEnd;
                currentRound++;
                if (currentGameState == GameState.EndGame)
                {
                    //TODO: 이겼는지 졌는지 UI 띄우기
                }
                SetGameTime(5, RoundState.RoundStart);
                
                break;
        }
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
    void SetGameTime(float time, RoundState state)
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
                
                SetRoundState(state);
                
            });
    }
    
    void SetTeam(Team team, GameObject playerStat)
    
    {
        switch (team)
        {
            case Team.Blue:
                teamDictionary[playerStat] = Team.Blue;
                break;
            case Team.Red:
                //Debug.Log("Red Team");
                teamDictionary[playerStat] = Team.Red;
                break;
        }
    }
    
    #endregion
    
    // Start is called before the first frame update
    void Start()
    {
        ResetRound();
        
        GameObject playerStat = GameObject.FindWithTag("Player");
        List<Spawner> countSpanwer = new List<Spawner>
            (FindObjectsByType<Spawner>(FindObjectsInactive.Include, FindObjectsSortMode.None));
       
        for (int i = 0; i < countSpanwer.Count; i++)
        {
            if (mapSpawners.ContainsKey(countSpanwer[i].transform.parent.gameObject))
            {
                mapSpawners[countSpanwer[i].transform.parent.gameObject].Add(countSpanwer[i]);
            }
            else
            {
                mapSpawners.Add(countSpanwer[i].transform.parent.gameObject, new List<Spawner>());
                mapSpawners[countSpanwer[i].transform.parent.gameObject].Add(countSpanwer[i]);
            }
        }
        SetPlayerTeam(playerStat);
        SetGameState(GameState.StartGame);
        maps.AddRange(GameObject.FindGameObjectsWithTag("Map"));
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.B) && currentRoundState ==  RoundState.RoundStart)
        {
            RepairShopUI.SetActive(!RepairShopUI.activeSelf);
        }
        
        if(currentRound == 7 && currentRoundState == RoundState.RoundEnd)
        {
            SetGameState(GameState.EndGame);
        }
        
        
        
    }

    //플레이어가 죽을때 또는 죽였을 때 호출되야한다.
    public void EndRound(GameObject Loser)
    {
        SetRoundState(RoundState.RoundEnd);
        switch(teamDictionary[Loser])
        {
            case Team.Blue:
                RedWinCount++;
                break;
            case Team.Red:
                BlueWinCount++;
                break;
        }

        //ResetRound();
    }
    public void SetPlayerTeam(GameObject player)
    {
        int tmpRandom = Random.Range(0, 1);
        int Enemy = tmpRandom == 0 ? 1 : 0;
        GameObject EnemyPlayer = GameObject.FindWithTag("Enemy");
        
        
        teamDictionary[player.gameObject] = (Team)tmpRandom;
        //playerStat.team = (Team)tmpRandom;
        teamDictionary[EnemyPlayer] = (Team)Enemy;
        //EnemyPlayer.GetComponent<TestStat>().team = (Team)Enemy;
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
                //적이 나를 먼저 죽였을때 그후 적이 죽으면 상관없이 패배 확정
                return;
            }
            WinRound(enemy);
            SetWinLoseState(WinLoseState.Win);
        };
        

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
        
        currentRound = 1;
        BlueWinCount = 0;
        RedWinCount = 0;
    }
}
