using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : Player
{
    public int PlayerId {get; set;}
    private NetworkManager _networkManager;
    void Start()
    {
        StartCoroutine("CoSendPacket");
        _networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }
    
    IEnumerator CoSendPacket()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.25f);
            
            C_Move movePacket = new C_Move();
            movePacket.posX = UnityEngine.Random.Range(-20, 20);
            movePacket.posY = 0;
            movePacket.posZ = UnityEngine.Random.Range(-20, 20);
            
            _networkManager.Send(movePacket.Write());
        }
    }
    
}
