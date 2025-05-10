using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passive_Healer : MonoBehaviour, IPassive
{
    Coroutine healer;

    public void ApplyPassive(PlayerController playerController)
    {
        if (healer == null)
        {
            healer = StartCoroutine(Healer(playerController));
        }
    }

    IEnumerator Healer(PlayerController playerController) {
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
