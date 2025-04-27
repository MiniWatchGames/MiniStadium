using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class Health : NetworkBehaviour
{
    public readonly SyncVar<float> hp = new(100f);

    private void Awake()
    {
        hp.OnChange += OnHpChange;
    }

    private void OnHpChange(float prev, float next, bool asserver)
    {
        if (next <= 0f)
        {
            Debug.Log("Dead");
            Despawn();
        }
    }

    //서버만 데이터를 가짐
    [ServerRpc]
    public void ChangeHp(float amount)
    {
        hp.Value += amount;
    }
}
