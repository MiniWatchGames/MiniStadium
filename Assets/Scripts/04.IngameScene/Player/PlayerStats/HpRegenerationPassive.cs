using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpRegenerationPassive : MonoBehaviour, IPassive
{
    Coroutine helthRegen;

    public void ApplyPassive(PlayerController playerController)
    {
        if (helthRegen == null)
        {
            helthRegen = StartCoroutine(HelthRegen(playerController));
        }
    }

    IEnumerator HelthRegen(PlayerController playerController) {
        while (true) { 
            if(playerController.CurrentHp < playerController.BaseMaxHp.Value)
            playerController.CurrentHp += 1;
            yield return new WaitForSeconds(2f);
        }
    }
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
