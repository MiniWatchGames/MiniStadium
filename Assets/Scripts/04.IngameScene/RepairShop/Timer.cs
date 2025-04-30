using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Image timer;
    public TMP_Text text;
    
    public delegate void TimerDelegate();
    public TimerDelegate OnTimerEndDelegate;
    public TimerDelegate OnTimerStartDelegate;

    public enum TimerType{Decrease, Increase}
    public TimerType timerType;
    public float timeLimit;
    public float currentTime;
    public RepairShopTimer repairShopTimer;
    private bool _isPaused;
    // Start is called before the first frame update
    void Start()
    {
        ResetTimer();
        repairShopTimer = GameObject.FindObjectOfType<RepairShopTimer>();
        _isPaused = false;
    }

    void Update()
    {
        
        if(_isPaused) return;
        if (timerType == TimerType.Decrease)
        {
            currentTime -=  Time.deltaTime * 2f;
            repairShopTimer.time = currentTime;
            if (currentTime <= 0)
            {
                OnTimerEndDelegate?.Invoke();
            }
        }
        if (timerType == TimerType.Increase)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= timeLimit)
            {
                OnTimerEndDelegate?.Invoke();
                //_isPaused= true;
            }
            
        }
        timer.fillAmount = currentTime/timeLimit;
        text.text = FormatSeconds(currentTime);
    }
    public void ResetTimer()
    {
        if (timerType == TimerType.Decrease)
        {
            currentTime = timeLimit;
        }
        else
        {
            currentTime = 0;
        }
        //_isPaused= true;
    }

    public void PauseTimer()
    {
        _isPaused = true;
    }
    
    public void ResumeTimer()
    {
        _isPaused = false;
        
    }
    public void SetTimer(float timeLimit, TimerType timerType, TimerDelegate timerDelegate)
    {
        OnTimerEndDelegate = timerDelegate;
        this.timeLimit = timeLimit;
        this.timerType = timerType;
        ResetTimer();
    }
    public void SetEndTimerDelegate(TimerDelegate timerDelegate)
    {
        OnTimerEndDelegate = timerDelegate;
    }
    string FormatSeconds(float elapsed)
    {
        
        int minutes = (int)(elapsed / 60);
        int seconds = (int)(elapsed % 60);
            
        
        if (5f <elapsed  && elapsed < 60f)
        {
            minutes = 0;
            seconds = (int)(elapsed % 60);
            return $"{minutes:00}:{seconds:00}";
        }

        if (elapsed < 5f)
        {
            return $"{elapsed:F2}";
        }
       
        return $"{minutes:00}:{seconds:00}";
        
    }
}



