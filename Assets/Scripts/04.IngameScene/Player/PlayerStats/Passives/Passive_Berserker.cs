using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Passive_Berserker : MonoBehaviour, IPassive, IStatObserver
{
    private PassiveEffects effects;
    private GameObject effect;
    private PlayerController controller;
    private float additionalDamage;
    private int modifierIndex;
    private bool activated;
    public void ApplyPassive(PlayerController playerController)
    {
        controller = playerController;
        controller.CurrentHp.AddObserver(this);
        effects = controller.GetComponent<PassiveEffects>();
        
        additionalDamage = 0;
        
        effect = Instantiate(effects.effect5Berserker, controller.transform);
        effect.SetActive(false);
    }

    public void WhenStatChanged((float, string) data)
    {
        if (data.Item2 == "currentHp")
        {
            if (data.Item1 <= controller.BaseMaxHp.Value / 2 && !activated)
            {
                additionalDamage = controller.Damage.Value * 0.2f;
                controller.AddStatDecorate(StatType.Damage, additionalDamage);
                modifierIndex = controller.BaseMoveSpeed.Modifiers.Count - 1;
                activated = true;
            }
            else if (data.Item1 > controller.BaseMaxHp.Value / 2 && activated)
            {
                controller.RemoveStatTargetDecorate(StatType.Damage, modifierIndex);
                activated = false;
            }
            effect.SetActive(activated);
        }
    }

    private void GetDamageValue()
    {
        // TODO:라운드가 넘어가고 데미지를 강화했을때를 대비
    }
    
    private void OnDestroy()
    {
        if (activated)
        {
            controller.RemoveStatTargetDecorate(StatType.Damage, modifierIndex);
            activated = false;
        }
        Destroy(effect.gameObject);
    }
}
