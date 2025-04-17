using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct weapon
{
    public string name;
    public int price;
    public string description;
    public List<string> skills;
}

public struct skill
{
    public string name;
    public string description;
    public int damage;
    public int cooldown;
    public int range;
    public int duration;
    
}
public class WeaponStruct_ex : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
