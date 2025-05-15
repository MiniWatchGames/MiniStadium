using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Passive_Jumper : MonoBehaviour, IPassive, IStatObserver
{
    private PassiveEffects effects;
    private GameObject effect;
    private PlayerController controller;
    private float additionalSpeed;
    private int modifierIndex;
    private bool affected;
    private ParticleSystem particle;
    private ActionState actionState;
    private Coroutine jumpEffect;
    public void ApplyPassive(PlayerController playerController)
    {
        effects = playerController.GetComponent<PassiveEffects>();
        effect = Instantiate(effects.effect6, playerController.CameraController.transform);
        effect.transform.localPosition = new Vector3(0, -0.5f, 0.15f);
        particle = effect.GetComponent<ParticleSystem>();
        controller = playerController;
        controller.CurrentHp.AddObserver(this);
        additionalSpeed = 0;
        //additionalSpeed = controller.BaseMoveSpeed.Value * 0.2f;
        controller.AddStatDecorate(StatType.JumpPower, additionalSpeed);
        modifierIndex = controller.BaseJumpPower.Modifiers.Count - 1;
        affected = true;
    }
    public void WhenStatChanged((float, string) data)
    {
        if (this == null) return;
        if (data.Item2 == "currentHp")
        {
            if (controller == null) return;
            controller.RemoveStatTargetDecorate(StatType.JumpPower, modifierIndex);
            if (particle != null) { 
                var main = particle.main;
                main.simulationSpeed = 3*Mathf.Lerp(0f, 1f, data.Item1 / controller.BaseMaxHp.Value);
            }
            additionalSpeed = Mathf.Lerp(0f, controller.BaseJumpPower.Value * 0.2f, data.Item1 / controller.BaseMaxHp.Value);
            controller.AddStatDecorate(StatType.JumpPower, additionalSpeed);
            modifierIndex = controller.BaseJumpPower.Modifiers.Count - 1;
            affected = false;
        }
    }

    private void Update()
    {
        if (!controller.IsGrounded) { 
            if (controller.MovementFsm.CurrentState == MovementState.Jump)
            {
                if(jumpEffect == null)
                    jumpEffect = StartCoroutine(ActivateForOneSecond());
            }
        }
    }
    IEnumerator ActivateForOneSecond()
    {
        if (this == null) yield break;
        effect?.SetActive(true);
        yield return new WaitForSeconds(2f);
        effect?.SetActive(false);
        jumpEffect = null;
    }
    private void OnDestroy()
    {
        effect?.SetActive(false);
        if (affected)
            controller.RemoveStatTargetDecorate(StatType.JumpPower, modifierIndex);
        controller?.CurrentHp.RemoveObserver(this);
        StopAllCoroutines();
        Destroy(effect?.gameObject);
    }
}
