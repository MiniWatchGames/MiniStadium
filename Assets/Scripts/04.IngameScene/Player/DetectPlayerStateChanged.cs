using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DetectPlayerStateChanged : INotifyPropertyChanged
{
    //이 인터페이스를 상속받는 클래스는 StatUpdate와 StatReset 메소드를 구현해야 한다.
    //StatUpdate는 캐릭터의 스텟이 변경될 때 호출된다.
    //StatReset은 캐릭터의 스텟이 초기화될 때 호출된다.
    //StatUpdate와 StatReset 메소드는 매개변수로 TestStat을 받는다.
    //TestStat은 캐릭터의 스텟을 담고 있는 클래스이다.
    //StatUpdate와 StatReset 메소드는 TestStat을 매개변수로 받아서 캐릭터의 스텟을 업데이트하거나 초기화한다.
    private TestStat playerStat;
    
    public DetectPlayerStateChanged(TestStat stat)
    {
        playerStat = stat;
        playerName_Base = playerStat.name;
        playerHp_Base = playerStat.health;
        playerMaxHp_Base = playerStat.maxHealth;
    }
    private string playerName_Base;
    public string playerName
    {
        get =>playerName_Base;
        set
        {
            if(playerName_Base != value)
            {
                playerName_Base = value;
                OnPropertyChanged("playerName");
            }
            
            
        }
    }

    private float playerHp_Base;
    public float playerHp
    {
        get => playerHp_Base;
        set
        {
            if(playerHp_Base != value)
            {
                Debug.Log("playerHp" + value);
                playerHp_Base = value;
                OnPropertyChanged("playerHp");
            }
            
        }
    }

    private float playerMaxHp_Base;
    public float playerMaxHp
    {
        get => playerMaxHp_Base;
        set
        {
            if(playerMaxHp_Base != value)
            {
                playerMaxHp_Base = value;
                OnPropertyChanged("playerMaxHp");
            }
            
        }
    }
    
    
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    
}

