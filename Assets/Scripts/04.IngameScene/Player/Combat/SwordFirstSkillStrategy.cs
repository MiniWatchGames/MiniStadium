using System.Collections;
using UnityEngine;

public class SwordFirstSkillStrategy : IWeaponSkillStrategy
{
    private PlayerController _playerController;
    private SwordController _swordController;
    private bool _isSkillActive = false;
    private bool _isComplete = false;
    
    public void Enter(PlayerController playerController, GameObject weaponObject)
    {
        _playerController = playerController;
        _swordController = weaponObject.GetComponent<SwordController>();
        _isSkillActive = true;
        _isComplete = false;
        
        _swordController.FirstSkillStart();
    }

    public void Update(PlayerController playerController)
    {
        
    }

    public void Exit()
    {
        // 스킬 종료 시 정리 작업
        if (_isSkillActive)
        {
            _swordController.FirstSkillEnd();
            _isSkillActive = false;
        }
        
        _isComplete = true;
    }

    public void HandleInput(bool isSkillPressed, bool isSkillHeld)
    {
        if (!isSkillHeld && _isSkillActive)
        {
            _playerController.Animator.SetTrigger("FirstSkill");
            _swordController.FirstSkillEnd();
            _isSkillActive = false;
            //_isComplete = true;
            CompleteWithDelay(500);
        }
    }

    public bool IsComplete()
    {
        return _isComplete;
    }

    public async void CompleteWithDelay(int delay)
    {
        await System.Threading.Tasks.Task.Delay(delay);
        _isComplete = true;
    }
}
