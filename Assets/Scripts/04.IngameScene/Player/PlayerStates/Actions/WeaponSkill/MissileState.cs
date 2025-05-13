using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileState : PlayerActionState, ISkillData
{
    private ObservableFloat _coolTime;
    private bool _isNeedPresse;
    private ObservableFloat _needPressTime;
    private float _skillMount;
    private ObservableFloat _pressTime;
    public ObservableFloat CoolTime => _coolTime;
    public ObservableFloat NeedPressTime => _needPressTime;
    public ObservableFloat PressTime => _pressTime;
    public bool IsNeedPresse => _isNeedPresse;
    public float SkillMount => _skillMount;
    private GameObject Missile;
    private Transform spawnPoint;
    private GunController gunController;

    private string path = "Prefabs/IngameScene/Player/Weapon/Missile";
    public void ResetSkill()
    {
    }

    private void Awake()
    {
        _coolTime = new ObservableFloat(5, "_coolTime");
        _skillMount = 30;
    }
    public override void Enter(PlayerController playerController)
    {
        //playerController.Animator.Play(aniName);
        base.Enter(playerController);
        if(Missile == null)
            Missile = Resources.Load<GameObject>(path);
        if (gunController == null)
            gunController = _playerController.PlayerWeapon.CurrentWeapon.GetComponent<GunController>();
        if (spawnPoint == null)
            spawnPoint = (gunController.SkillFirePoint);
        Vector3 cameraPosition = gunController.CameraPosition.normalized;
        var missileObj =Instantiate(Missile, spawnPoint.position, _playerController.transform.rotation);
        var missileControlelr = missileObj.GetComponent<HS_ProjectileMover>();
        missileControlelr.SetMissileDirection(cameraPosition);
        missileControlelr.SetParentsTransform(gameObject.transform);
        missileControlelr.SetDamage(_skillMount);
        _playerController.SetActionState("Idle");
    }
    public override void Exit()
    {
    }
    public override void StateUpdate()
    {

    }

   

}
