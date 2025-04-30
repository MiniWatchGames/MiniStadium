using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreasingRandomStatEvery20Seconds : MonoBehaviour, IPassive
{
    //20초 마다 랜덤 스텟 증가

    Coroutine RandomStat;

    public void ApplyPassive(PlayerController playerController)
    {
        if (RandomStat == null)
        {
            RandomStat = StartCoroutine(HelthRegen(playerController));
        }
    }

    IEnumerator HelthRegen(PlayerController playerController)
    {
        while (true)
        {
            int rand = Random.Range(0, 3);
            switch (rand) {
                case 0:
                    //최대 체력 증가, 체력 5 회복
                    playerController.AddStatDecorate(StatType.MaxHp, 5);
                    playerController.CurrentHp += 5;
                    break;
                case 1:
                    //이동 속도 증가
                    playerController.AddStatDecorate(StatType.MoveSpeed, 0.5f);
                    break;
                case 2:
                    //점프 높이 증가
                    playerController.AddStatDecorate(StatType.JumpPower, 0.5f);
                    break;
                case 3:
                    //방어력 증가
                    playerController.AddStatDecorate(StatType.Defence, 0.5f);
                    break;
            }    
            yield return new WaitForSeconds(20f);
            switch (rand)
            {
                case 0:
                    //최대 체력 증가
                    playerController.RemoveStatDecorate(StatType.MaxHp);
                    break;
                case 1:
                    playerController.RemoveStatDecorate(StatType.MoveSpeed);
                    break;
                case 2:
                    playerController.RemoveStatDecorate(StatType.JumpPower);
                    break;
                case 3:
                    playerController.RemoveStatDecorate(StatType.Defence);
                    break;
            }
        }
    }
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
