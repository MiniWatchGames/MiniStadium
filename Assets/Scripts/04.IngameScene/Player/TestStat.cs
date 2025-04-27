using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TestStat : MonoBehaviour
{
    public float health;
    public float maxHealth;
    //public int winCount;
    public InGameManager.Team team;
    //public GameObject inGameManager_obj;
    public Action onStatUpdate;
    public Action<TestStat> OnPlayerDie;// TODO:인자로 본인을 죽인 적 정보 넘기기
    public Action<TestStat> OnEnemyKilled;
    private float detectHpChange;
    public InGameManager inGameManager;
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
                    OnPlayerDie?.Invoke(this);
                }
                //onStatUpdate?.Invoke();
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        ChangedHp = health;
        
    }

    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.GetComponent<TestStat>())
                {
                    if (inGameManager.roundstate == InGameManager.RoundState.RoundStart)
                    {
                        return;
                    }
                    
                    TestStat go = hit.collider.GetComponent<TestStat>();
                    go.LoseHp(go);
                    
                }
            }
        }
    }
    // Update is called once per frame
    

    public void GainHp()
    {   if(health >= maxHealth) return;
        health += 10;
        ChangedHp = health;
    }
    public void LoseHp(TestStat enemy)
    {
        if (health <= 0)
        {
            //OnEnemyKilled?.Invoke(this);
            return;
        }
            
        health -= 10;
        ChangedHp = health;
        
        //Debug.Log("LoseHp"+ health + gameObject.name);
    }

    public void Reset()
    {
        health = maxHealth;
        ChangedHp = health;
    }
    public void KillEnemy()
    {
        OnEnemyKilled?.Invoke(GameObject.FindGameObjectWithTag("Enemy").GetComponent<TestStat>());
    }
}
