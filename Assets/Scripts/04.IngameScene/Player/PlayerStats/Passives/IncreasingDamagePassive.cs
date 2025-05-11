using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreasingDamagePassive : MonoBehaviour, IPassive
{
    
    public void ApplyPassive(PlayerController playerController)
    {
        playerController.AddStatDecorate(StatType.Damage, 10f);
    }
}
