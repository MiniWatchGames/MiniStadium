using UnityEngine;

public class GunFirstSkillStrategy : IWeaponSkillStrategy
{
    private PlayerController _playerController;
    private GunController _gunController;
    private bool _isSkillActive = false;
    private bool _isComplete = false;
    public void Enter(PlayerController playerController, GameObject weaponObject)
    {
        _playerController = playerController;
        _gunController = weaponObject.GetComponent<GunController>();
        _isSkillActive = true;
        _isComplete = false;
        
        _gunController.FirstSkillStart();
    }

    public void Update(PlayerController playerController)
    {
        
    }

    public void Exit()
    {
        // 스킬 종료 시 정리 작업
        if (_isSkillActive)
        {
            _gunController.FirstSkillEnd();
            _isSkillActive = false;
        }
        
        _isComplete = true;
    }

    public void HandleInput(bool isSkillPressed, bool isSkillHeld)
    {
        if (!isSkillHeld && _isSkillActive)
        {
            _gunController.FirstSkillEnd();
            _isSkillActive = false;
            _isComplete = true;
        }
    }

    public bool IsComplete()
    {
        return _isComplete;
        
    }
}
