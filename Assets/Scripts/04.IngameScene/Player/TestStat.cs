using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStat : MonoBehaviour
{
    public float health;
    public float maxHealth;
    public InGameManager.Team team;
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
    }
}
