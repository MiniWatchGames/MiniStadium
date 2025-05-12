using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadState : PlayerActionState
{
    private static int aniName;
    private GunController gunController;
    public ReloadState() : base() { }
  
    private Coroutine _layerTransitionCoroutine; // 레이어 전환 코루틴
    private int _upperBodyOverrideLayerIndex=3;


    public override void Enter(PlayerController playerController)
    {
        base.Enter(playerController);
        _playerController.CombatManager.FadeInUpperBodyLayer(_upperBodyOverrideLayerIndex);
        if (gunController == null) { 
            gunController = _playerController.PlayerWeapon.CurrentWeapon.GetComponent<GunController>();
        }
        _playerController.Animator.Play("Reload_Gun");
        gunController.ReloadSoundPlay();
        _playerController.StartCoroutine(Reloading());
    }
    public override void Exit()
    {
        base.Exit();
        gunController.SoundStop();
        _playerController.CombatManager.FadeOutUpperBodyLayer(_upperBodyOverrideLayerIndex);
        var gun = ((GunAttackStrategy)_playerController.CombatManager.CurrentStrategy);
        gun.ReloadAmmo();
        gun.CanAttack = true;
    }
    public override void StateUpdate()
    {
    }
    IEnumerator Reloading() {
        float f= 1;
        while (f < 3) {
            f += Time.deltaTime;
            Debug.Log($"{f}초 재장전 중");
            yield return  null;
        }
        _playerController.IsReloadFinished = true;
        _playerController.CanChangeState = true;
        _playerController.SetActionState("Idle");
    }
}