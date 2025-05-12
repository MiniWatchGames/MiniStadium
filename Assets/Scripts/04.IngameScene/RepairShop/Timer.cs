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

    public delegate void TimerDelegateWithFloat(float time);
    public TimerDelegate OnTimerEndDelegate;
    public TimerDelegate OnTimerStartDelegate;
    public TimerDelegateWithFloat OnTimerDelegate;
    public enum TimerType { Decrease, Increase }
    public TimerType timerType;
    public float timeLimit;
    public float currentTime;
    private bool _isPaused;
    // Start is called before the first frame update
    void Start()
    {
        _isPaused = false;
    }

    void Update()
    {
        if (_isPaused) return;
        if (timerType == TimerType.Decrease)
        {
            
            currentTime -= Time.deltaTime;
            OnTimerDelegate?.Invoke(currentTime);
            if (currentTime <= 0)
            {
                Debug.Log("Timer End");
                OnTimerEndDelegate?.Invoke();

            }
        }
        if (timerType == TimerType.Increase)
        {
            currentTime += Time.deltaTime;
            OnTimerDelegate?.Invoke(currentTime);
            if (currentTime >= timeLimit)
            {
                OnTimerEndDelegate?.Invoke();
                //_isPaused= true;
            }

        }
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
        Debug.Log("Timer Reset");
    }

    public void PauseTimer()
    {
        _isPaused = true;
    }

    public void ResumeTimer()
    {
        _isPaused = false;

    }
    public void SetTimer(float timeLimit, TimerType timerType, TimerDelegate timerDelegate, TimerDelegateWithFloat timerDelegateWithFloat = null)
    {
        //시간이 지날때마다 실행되는 함수
        OnTimerDelegate = timerDelegateWithFloat;
        //시간이 끝났을때 실행되는 함수
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


        if (5f < elapsed && elapsed < 60f)
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