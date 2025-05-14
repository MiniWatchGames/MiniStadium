using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Passive_Marathoner : MonoBehaviour, IPassive, IStatObserver
{
    private PassiveEffects effects;
    private GameObject effect;
    private PlayerController controller;
    private float additionalSpeed;
    private int modifierIndex;
    private bool affected;
    private ParticleSystem particle;
    public void ApplyPassive(PlayerController playerController)
    {
        effects = playerController.GetComponent<PassiveEffects>();
        effect = Instantiate(effects.effect6, playerController.GetComponentInChildren<Camera>().transform);
        particle = effect.GetComponent<ParticleSystem>();
        effect.transform.localPosition = new Vector3(0, -0.5f, 1.5f);
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
            var main = particle.main;
            main.simulationSpeed = 3*Mathf.Lerp(0f, 1f, data.Item1 / controller.BaseMaxHp.Value);
            additionalSpeed = Mathf.Lerp(0f, controller.BaseMoveSpeed.Value * 0.2f, data.Item1 / controller.BaseMaxHp.Value);
            controller.AddStatDecorate(StatType.MoveSpeed, additionalSpeed);
            modifierIndex = controller.BaseMoveSpeed.Modifiers.Count - 1;
            affected = false;
        }
    }

    private void Update()
    {
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0 || Mathf.Abs(Input.GetAxis("Vertical")) > 0)
        {
            effect.SetActive(true);
        }
        else {
            effect.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        effect?.SetActive(false);
        if (affected)
            controller.RemoveStatTargetDecorate(StatType.MoveSpeed, modifierIndex);
    }
}
