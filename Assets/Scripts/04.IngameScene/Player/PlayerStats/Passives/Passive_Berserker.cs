using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passive_Berserker : MonoBehaviour, IPassive, IStatObserver
{
    private PlayerController controller;
    private float baseDamage;
    private float additionalDamage;
    private bool activated;
    public void ApplyPassive(PlayerController playerController)
    {
        controller = playerController;
        controller.CurrentHp.AddObserver(this);
        baseDamage = controller.Damage.Value;
        additionalDamage = controller.Damage.Value * 0.2f;
    }

    public void WhenStatChanged((float, string) data)
    {
        if (data.Item2 == "currentHp")
        {
            if (data.Item1 <= controller.BaseMaxHp.Value / 2 && activated == false)
            {
                controller.AddStatDecorate(StatType.Damage, additionalDamage);
                activated = true;
            }
            else if (data.Item1 > controller.BaseMaxHp.Value / 2 && activated == true)
            {
                controller.AddStatDecorate(StatType.Damage, -additionalDamage);
                activated = false;
            }
        }
    }
    
    private void OnDestroy()
    {
        if (Mathf.Approximately(baseDamage + additionalDamage, controller.Damage.Value))
        {
            controller.AddStatDecorate(StatType.Damage, -additionalDamage);
            activated = false;
        }
    }
}
