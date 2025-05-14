using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpRegenerationPassive : MonoBehaviour, IPassive
{
    private PassiveEffects effects;
    private GameObject effect;
    Coroutine helthRegen;

    public void ApplyPassive(PlayerController playerController)
    {
        effects = playerController.GetComponent<PassiveEffects>();
        effect = Instantiate(effects.effect1, playerController.CameraController.transform);
        effect.transform.localPosition = new Vector3(0, -0.5f, 0.15f);
        if (helthRegen == null)
        {
            helthRegen = StartCoroutine(HelthRegen(playerController));
        }
    }

    IEnumerator HelthRegen(PlayerController playerController) {
        while (true) {
            if (playerController.CurrentHp.Value < playerController.BaseMaxHp.Value) { 
                effect?.SetActive(true);
                playerController.CurrentHp.Value += 1;
            }
            yield return new WaitForSeconds(2f);
            effect?.SetActive(false);
        }
    }
    private void OnDestroy()
    {
        StopAllCoroutines(); 
        Destroy(effect?.gameObject);
    }
}
