using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreasingRandomStatEvery20Seconds : MonoBehaviour, IPassive
{
    private PlayerController controller;
    private int rand;
    private int modifierIndex_HP;
    private int modifierIndex_MS;
    private int modifierIndex_JP;
    private int modifierIndex_AR;
    private PassiveEffects effects;
    private GameObject effect;
    //20초 마다 랜덤 스텟 증가
    Coroutine RandomStat;

    public void ApplyPassive(PlayerController playerController)
    {
        effects = playerController.GetComponent<PassiveEffects>();
        effect = Instantiate(effects.effect2, playerController.CameraController.transform);
        effect.transform.localPosition = new Vector3(0, -0.5f, 0.15f);
        controller = playerController;
        if (RandomStat == null)
        {
            RandomStat = StartCoroutine(RandomAttribute());
        }
    }

    IEnumerator RandomAttribute()
    {
        while (true)
        {
            rand = Random.Range(0, 3);
            effect?.SetActive(true);
            switch (rand) {
                case 0:
                    //최대 체력 20 증가, 체력 20 회복
                    controller.AddStatDecorate(StatType.MaxHp, 20);
                    controller.CurrentHp.Value += 20;
                    modifierIndex_HP = controller.BaseMaxHp.Modifiers.Count - 1;
                    break;
                case 1:
                    //방어력 증가
                    controller.AddStatDecorate(StatType.Defence, 5f);
                    modifierIndex_AR = controller.BaseDefence.Modifiers.Count - 1;
                    break;
            }
            yield return new WaitForSeconds(1f);
            effect?.SetActive(false);
            yield return new WaitForSeconds(19f);
            switch (rand)
            {
                case 0:
                    controller.RemoveStatTargetDecorate(StatType.MaxHp, modifierIndex_HP);
                    break;
                case 1:
                    controller.RemoveStatTargetDecorate(StatType.Defence, modifierIndex_AR);
                    break;
            }
        }
    }
    private void OnDestroy()
    {
        switch (rand)
        {
            case 0:
                controller.RemoveStatTargetDecorate(StatType.MaxHp, modifierIndex_HP);
                break;
            case 1:
                controller.RemoveStatTargetDecorate(StatType.Defence, modifierIndex_AR);
                break;
        }
        StopAllCoroutines(); 
        Destroy(effect?.gameObject);
    }
}
