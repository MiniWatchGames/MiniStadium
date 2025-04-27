using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TestStat : MonoBehaviour
{
    public float health;
    public float maxHealth;
    public int winCount;
    public InGameManager.Team team;
    public GameObject inGameManager_obj;
    public Action onStatUpdate;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GainHp()
    {
        health += 10;
    }
    public void LoseHp()
    {
        health -= 10;
        Debug.Log("LoseHp"+ health);
    }

    public void Reset()
    {
        health = maxHealth;
        if (inGameManager_obj.GetComponent<InGameManager>().currentRound > 7)
        {
            winCount = 0;
        }
    }
}
