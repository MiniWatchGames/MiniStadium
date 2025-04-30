using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PassiveFactory : MonoBehaviour
{
    //�ۼ��� �Ϸ�� �нú�Ŭ������ �̰��� �߰������־�� ��
    public void CreatePassive(PlayerController target, (int, string)[] passiveNames) {
        foreach (var passiveName in passiveNames) {
            switch (passiveName.Item1) {
                //"HpRegenerationPassive"
                case 1: 
                    target.PassiveList.Add(target.AddComponent<HpRegenerationPassive>());
                break;

                //"IncreasingRandomStatEvery20Seconds"
                case 2:
                    target.PassiveList.Add(target.AddComponent<IncreasingRandomStatEvery20Seconds>());
                break;
            }
        }
    }
}
