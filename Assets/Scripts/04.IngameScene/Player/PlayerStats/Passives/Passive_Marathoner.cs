using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passive_Marathoner : MonoBehaviour, IPassive, IStatObserver
{
    private PlayerController controller;
    private float additionalSpeed;
    private int modifierIndex;
    private bool affected;
    public void ApplyPassive(PlayerController playerController)
    {
        controller = playerController;
        controller.CurrentHp.AddObserver(this);
        additionalSpeed = 0;
        //additionalSpeed = controller.BaseMoveSpeed.Value * 0.2f;
        controller.AddStatDecorate(StatType.MoveSpeed, additionalSpeed);
        modifierIndex = controller.BaseMoveSpeed.Modifiers.Count - 1;
        affected = true;
    }
    public void WhenStatChanged((float, string) data)
    {
        if (data.Item2 == "currentHp")
        {
            controller.RemoveStatTargetDecorate(StatType.MoveSpeed, modifierIndex);
            affected = false;
            
            additionalSpeed = Mathf.Lerp(0f, controller.BaseMoveSpeed.Value * 0.2f, data.Item1 / controller.BaseMaxHp.Value);
            
            controller.AddStatDecorate(StatType.MoveSpeed, additionalSpeed);
            modifierIndex = controller.BaseMoveSpeed.Modifiers.Count - 1;
            affected = false;
        }
    }
    
    private void OnDestroy()
    {
        if (affected)
            controller.RemoveStatTargetDecorate(StatType.MoveSpeed, modifierIndex);
    }
}
