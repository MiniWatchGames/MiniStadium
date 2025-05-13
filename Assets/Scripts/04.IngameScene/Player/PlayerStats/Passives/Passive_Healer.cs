using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passive_Healer : MonoBehaviour, IPassive
{
    private PassiveEffects effects;
    private GameObject effect;
    Coroutine healer;

    public void ApplyPassive(PlayerController playerController)
    {
        effects = playerController.GetComponent<PassiveEffects>();
        effect = Instantiate(effects.effect4Healer, playerController.transform);
        
        if (healer == null)
        {
            healer = StartCoroutine(Healer(playerController));
        }
    }

    IEnumerator Healer(PlayerController playerController) {
        while (true) {
            if (playerController.ActionFsm.CurrentState == ActionState.Hit)
            {
                effect.SetActive(false);
                yield return new WaitForSeconds(3f);
            }
            if (playerController.CurrentHp.Value < playerController.BaseMaxHp.Value)
            {
                effect.SetActive(true);
                playerController.CurrentHp.Value += 5;
            }
            yield return new WaitForSeconds(1f);
            effect.SetActive(false);
        }
    }
    private void OnDestroy()
    {
        StopAllCoroutines();
        Destroy(effect.gameObject);
    }
}
