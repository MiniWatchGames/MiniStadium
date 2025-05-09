using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static RepairShopEnums;

public class TeleportSkillState : PlayerActionState, ISkillData
{   
    public TeleportSkillState() : base() { }

    private int Jump = Animator.StringToHash("Jump");
    private ObservableFloat _coolTime;
    private bool _isNeedPresse;
    private ObservableFloat _needPressTime;
    private float _skillMount;
    private ObservableFloat _pressTime;
    private CharacterController _characterController;
    public ObservableFloat CoolTime => _coolTime;
    public ObservableFloat NeedPressTime => _needPressTime;
    public ObservableFloat PressTime => _pressTime;
    public bool IsNeedPresse => _isNeedPresse;
    public float SkillMount => _skillMount;


    private void Awake()
    {
        _coolTime = new ObservableFloat(10, "_coolTime");
        _needPressTime = new ObservableFloat(3, "_needPressTime");
        _pressTime = new ObservableFloat(0, "_pressTime");
        _isNeedPresse = true;
        _skillMount = 5;        
    }
    public override void Enter(PlayerController playerController)
    {
        //playerController.Animator.Play(aniName);
        base.Enter(playerController);
        if(_characterController == null)
            _characterController = _playerController.GetComponent<CharacterController>();
        _playerController.PlayTeleportGain();
    }
    public override void Exit()
    {
        _playerController.StopSound();
        if (_pressTime.Value >= _needPressTime.Value)
        {
            _playerController.PlayTeleport();
            _characterController.Move(_playerController.transform.forward * _skillMount);

        }
        else { 
            _playerController.PlayTeleportGainRe();
        }
        _playerController.skillGageReset(this);
        _pressTime.Value = 0;
    }
    public override void StateUpdate()
    {
        _pressTime.Value += Time.deltaTime;
        _pressTime.Value = Mathf.Min(_pressTime.Value, _needPressTime.Value);
        Debug.Log(_pressTime.Value);
    }
}