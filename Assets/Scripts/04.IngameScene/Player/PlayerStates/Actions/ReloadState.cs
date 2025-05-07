using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadState : PlayerActionState
{
    private static int aniName;
    public ReloadState() : base() { }
  
    public override void Enter(PlayerController playerController)
    {
        base.Enter(playerController);
        _playerController.StartCoroutine(Reloading());
    }
    public override void Exit()
    {
        base.Exit();
        var gun = ((GunAttackStrategy)_playerController.CombatManager.CurrentStrategy);
        gun.ReloadAmmo();
        gun.CanAttack = true;
    }
    public override void StateUpdate()
    {
    }
    IEnumerator Reloading() {
        float f= 0;
        while (f < 5) {
            f += Time.deltaTime;
            Debug.Log($"{f}초 재장전 중");
            yield return  null;
        }
        _playerController.CanChangeState = true;
        _playerController.SetActionState("Idle");
    }
}