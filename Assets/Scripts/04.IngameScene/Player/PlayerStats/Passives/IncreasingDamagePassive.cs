using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreasingDamagePassive : MonoBehaviour, IPassive
{
    private PlayerController controller;
    private int modifierIndex;
    public void ApplyPassive(PlayerController playerController)
    {
        controller = playerController;
        controller.AddStatDecorate(StatType.Damage, 10f);
        modifierIndex = controller.BaseMoveSpeed.Modifiers.Count - 1;
    }
    private void OnDestroy()
    {
        controller.RemoveStatTargetDecorate(StatType.Damage, modifierIndex);
        
    }
}
