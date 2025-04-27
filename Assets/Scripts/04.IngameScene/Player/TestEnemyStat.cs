using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TestEnemyStat : MonoBehaviour
{
    public float health;
    public float maxHealth;
    //public int winCount;
    //public InGameManager.Team team;
    //public GameObject inGameManager_obj;
    public Action onStatUpdate;
    public Action<TestEnemyStat> OnEnemyDie;// TODO:인자로 본인을 죽인 적 정보 넘기기
    public Action<TestEnemyStat> OnPlayerKilled;
    private float detectHpChange;
    
    public float ChangedHp
    {
        get => detectHpChange;
        set
        {
            if (detectHpChange != value)
            {
                detectHpChange = value;
                if (detectHpChange >= value)
                {
                    
                }
                if(detectHpChange <= 0)
                {
                    // TODO:인자로 본인을 죽인 적 정보 넘기기
                    //
                    OnEnemyDie?.Invoke(this);
                }
                onStatUpdate?.Invoke();
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        detectHpChange = health;
        
    }

    
    // Update is called once per frame
    

    public void GainHp()
    {   if(health >= maxHealth) return;
        health += 10;
        detectHpChange = health;
    }
    public void LoseHp()
    {
        if (health <= 0) return;
            
        health -= 10;
        detectHpChange = health;
        Debug.Log("LoseHp"+ health);
    }

    public void Reset()
    {
        health = maxHealth;
        detectHpChange = health;
    }
    
}
