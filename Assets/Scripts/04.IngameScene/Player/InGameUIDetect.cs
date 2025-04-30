using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

public class InGameUIDetect : INotifyPropertyChanged
{
    private TestStat playerStat;
    private InGameManager _inGameManager;
    public InGameUIDetect(InGameManager inGameManager)
    {
        
        this._inGameManager = inGameManager;
        gameRound = inGameManager.currentRound;
        blueWinCount = inGameManager.BlueWinCount;
        redWinCount = inGameManager.RedWinCount;
        //gameTime = inGameManager.timer;
    }

   // private float gameTime;
    private int gameRound;
    private int blueWinCount;
    private int redWinCount;

    // public float GameTime
    // {
    //     get => gameTime;
    //     set
    //     {
    //         if (gameTime != value)
    //         {
    //             gameTime = value;
    //             OnPropertyChanged("GameTime");
    //         }
    //     }
    // }
    public int GameRound
    {
        get => gameRound;
        set
        {
            if (gameRound != value)
            {
                gameRound = value;
                OnPropertyChanged("GameRound");
            }
        }
    }
    public int BlueWinCount
    {
        get => blueWinCount;
        set
        {
            if (blueWinCount != value)
            {
                blueWinCount = value;
                OnPropertyChanged("BlueWinCount");
            }
        }
    }
    public int RedWinCount
    {
        get => redWinCount;
        set
        {
            if (redWinCount != value)
            {
                redWinCount = value;
                OnPropertyChanged("RedWinCount");
            }
        }
    }
    
    
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
