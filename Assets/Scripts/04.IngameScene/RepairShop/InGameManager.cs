using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class InGameManager : MonoBehaviour
{
    #region UI
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
        Red
    }
   
    [SerializeField] private GameObject RepairShopUI;
    [SerializeField] private GameObject GameRoundInfoUI;
    [SerializeField] private GameState currentGameState;
    [SerializeField] private WinLoseState currentWinLoseState;
    [SerializeField] private Team currentTeam;
    [SerializeField] private RoundState currentRoundState;

    [SerializeField] private Timer gameTimer;
    [SerializeField] private GameObject PlayerPrefab;
    
    [SerializeField] public int currentRound = 0;
    [SerializeField] private int currentGameTime = 0;
    
    void SetGameState(GameState state)
    {
        switch (state)
        {
            case GameState.StartGame:
                Debug.Log("Start Game");
                currentGameState = GameState.StartGame;
                SetRoundState(RoundState.RoundStart);
                break;
            case GameState.InGame:
                Debug.Log("In Game");
                currentGameState = GameState.InGame;
                GameRoundInfoUI.GetComponent<TMP_Text>().text = "Game Start";
                break;
            case GameState.EndGame:
                Debug.Log("End Game");
                currentGameState = GameState.EndGame;
                GameRoundInfoUI.GetComponent<TMP_Text>().text = "Game Start";
                break;
        }
    }
    void SetRoundState(RoundState state)
    {
        switch (state)
        {
            case RoundState.RoundStart:
                Debug.Log("Round Start");
                currentRoundState = RoundState.RoundStart;
                SetGameTime(40, RoundState.InRound);
                break;
            case RoundState.InRound:
                Debug.Log("In Round");
                currentRoundState = RoundState.InRound;
                SetGameTime(120, RoundState.RoundEnd);
                break;
            case RoundState.RoundEnd:
                Debug.Log("Round End");
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
                Debug.Log("Default");
                currentWinLoseState = WinLoseState.Default;
                break;
            case WinLoseState.Win:
                Debug.Log("Win");
                currentWinLoseState = WinLoseState.Win;
                break;
            case WinLoseState.Lose:
                Debug.Log("Lose");
                currentWinLoseState = WinLoseState.Lose;
                break;
            case WinLoseState.Draw:
                Debug.Log("Draw");
                currentWinLoseState = WinLoseState.Draw;
                break;
            case WinLoseState.Duse:
                Debug.Log("Duse");
                currentWinLoseState = WinLoseState.Duse;
                break;
        }
    }
    void SetGameTime(float time, RoundState state)
    {
        
        gameTimer.SetTimer(time, Timer.TimerType.Decrease, () =>
            {
                Debug.Log("Game Time End");
                SetRoundState(state);
              
            });
    }
    
    void SetTeam(Team team)
    {
        switch (team)
        {
            case Team.Blue:
                Debug.Log("Blue Team");
                break;
            case Team.Red:
                Debug.Log("Red Team");
                break;
        }
    }
    
    #endregion
    
    // Start is called before the first frame update
    void Start()
    {
       SetGameState(GameState.StartGame);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.B) && currentRoundState ==  RoundState.RoundStart)
        {
            RepairShopUI.SetActive(!RepairShopUI.activeSelf);
            PurchaseManager.PurchasedPlayerItems = RepairShopUI.GetComponent<RepairShop>()?.Receipt.PlayerItems.DeepCopy();
            Instantiate(PlayerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        }
        
        if(currentRound == 7 && !(currentWinLoseState == WinLoseState.Duse || currentWinLoseState == WinLoseState.Draw))
        {
            SetGameState(GameState.EndGame);
        }
        
        
    }

    public void NextRound(TestStat player)
    {
        currentRound++;
        
    }
    
}
