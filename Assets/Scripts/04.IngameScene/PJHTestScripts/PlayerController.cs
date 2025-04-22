using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct PlayerInfo
{
    public string name;
    public float MaxHP;
    public InGameManager.Team team;
    public float HP;
    public float Speed;
    public ItemInfo weapon;
    public skill[] skill;
}
public class PlayerController : MonoBehaviour
{
    public PlayerInfo playerInfo;
    // Start is called before the first frame update
    void Start()
    {
        playerInfo = new PlayerInfo();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
