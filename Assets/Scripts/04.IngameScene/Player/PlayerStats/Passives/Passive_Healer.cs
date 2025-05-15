using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passive_Healer : MonoBehaviour, IPassive, IStatObserver
{
    private PassiveEffects effects;
    private GameObject effect;
    Coroutine healer;
    private float prevHp;
    private bool isDamaged;
    private PlayerController _playerController;
    public void ApplyPassive(PlayerController playerController)
    {
        _playerController = playerController;
        effects = playerController.GetComponent<PassiveEffects>();
        effect = Instantiate(effects.effect4Healer, playerController.CameraController.transform);
        effect.transform.localPosition = new Vector3(0, -0.5f, 0.15f);
        playerController.CurrentHp.AddObserver(this);
        prevHp = playerController.CurrentHp.Value;
        if (healer == null)
        {
            healer = StartCoroutine(Healer(playerController));
        }
    }

    public void WhenStatChanged((float, string) data)
    {
        if (this == null) return;

        if (prevHp > data.Item1) {
            isDamaged = true;
            prevHp = data.Item1;
        }
    }

    IEnumerator Healer(PlayerController playerController) {
        ObservableFloat currentHp = playerController?.CurrentHp;
        Stat maxHp = playerController?.BaseMaxHp;
        if (currentHp != null) {
            while (true)
            {
                if (this == null) yield break;
                if (isDamaged)
                {
                    effect?.SetActive(false);
                    yield return new WaitForSeconds(3f);
                    isDamaged = false;
                }
                else if (currentHp.Value >= 50)
                {
                    effect?.SetActive(false);
                }
                else if (currentHp.Value + 1 < maxHp.Value)
                {
                    effect?.SetActive(true);
                    currentHp.Value += 1;
                }
                else
                {
                    effect?.SetActive(true);
                    currentHp.Value = maxHp.Value;
                }
                yield return new WaitForSeconds(1f);
                effect?.SetActive(false);
            }
        }
    }
    private void OnDestroy()
    {
        _playerController?.CurrentHp.RemoveObserver(this);
        StopAllCoroutines();
        Destroy(effect?.gameObject);
    }
}
