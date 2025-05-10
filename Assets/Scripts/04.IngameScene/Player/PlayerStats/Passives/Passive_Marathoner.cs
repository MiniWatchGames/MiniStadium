using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passive_Marathoner : MonoBehaviour, IPassive
{
    Coroutine marathoner;

    public void ApplyPassive(PlayerController playerController)
    {
        if (marathoner == null)
        {
            marathoner = StartCoroutine(Marathoner(playerController));
        }
    }

    IEnumerator Marathoner(PlayerController playerController) {
        while (true) {
            if(playerController.ActionFsm.CurrentState == ActionState.Hit)
                yield return new WaitForSeconds(3f);
            if (playerController.CurrentHp.Value < playerController.BaseMaxHp.Value)
                playerController.CurrentHp.Value += 5;
            yield return new WaitForSeconds(1f);
        }
    }
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
