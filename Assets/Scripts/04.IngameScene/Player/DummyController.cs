using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyController : MonoBehaviour, IDamageable
{
    private int _currentHp;
    public Action<GameObject> OnDieCallBack;
    private int CurrentHp
    {
        get => _currentHp;
        set
        {
            if (CurrentHp < 0) CurrentHp = 0;
            OnDieCallBack.Invoke(gameObject);
            Debug.Log($"dummy is dead");
        }
    }

    private void Start()
    {
        CurrentHp = 100;
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        var damage = damageInfo.damage;
        CurrentHp -= damage;
        Debug.Log($"current Hp = {CurrentHp}");
    }
    public void ResetHp()
    {
        CurrentHp = 100;
    }
}
