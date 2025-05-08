using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PassiveFactory 
{
    //작성이 완료된 패시브클래스를 이곳에 추가시켜주어야 함
    public void CreatePassive(PlayerController target, (int, string)[] passives) {
        if(passives is null) return;
        foreach (var passive in passives) {
            switch (passive.Item1) {
                //"HpRegenerationPassive"
                case 1: 
                    target.PassiveList.Add(target.AddComponent<HpRegenerationPassive>());
                break;
                //"IncreasingRandomStatEvery20Seconds"
                case 2:
                    target.PassiveList.Add(target.AddComponent<IncreasingRandomStatEvery20Seconds>());
                break;    
                case 3:
                    target.PassiveList.Add(target.AddComponent<IncreasingDamagePassive>());
                break;
            }
        }
    }
}
