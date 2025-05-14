using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreasingDamagePassive : MonoBehaviour, IPassive
{
    private PassiveEffects effects;
    private GameObject effect;
    private PlayerController controller;
    private int modifierIndex;
    public void ApplyPassive(PlayerController playerController)
    {
        effects = playerController.GetComponent<PassiveEffects>();
        effect = Instantiate(effects.effect3, playerController.CameraController.transform);
        effect.transform.localPosition = new Vector3(0, -0.5f, 0.15f);
        effect.SetActive(true);
        controller = playerController;
        controller.AddStatDecorate(StatType.Damage, 10f);
        modifierIndex = controller.BaseMoveSpeed.Modifiers.Count - 1;
    }
    private void OnDestroy()
    {
        effect?.SetActive(false);
        controller.RemoveStatTargetDecorate(StatType.Damage, modifierIndex); 
        StopAllCoroutines();
        Destroy(effect?.gameObject);
    }
}
