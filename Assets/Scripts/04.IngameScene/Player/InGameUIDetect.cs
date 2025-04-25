using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

public class InGameUIDetect : INotifyPropertyChanged
{
    private TestStat playerStat;
    private InGameManager inGameManager;
    public InGameUIDetect(InGameManager inGameManager)
    {
        
        this.inGameManager = inGameManager;
        gameRound = inGameManager.currentRound;
        blueWinCount = 0;
        
    }

    private int gameRound;
    private int blueWinCount;
    private int redWinCount;
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
