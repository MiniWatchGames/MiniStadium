using UnityEngine;

public class SwordAttackStrategy : IAttackStrategy
{
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int NextAttack = Animator.StringToHash("NextAttack");
    private PlayerController _playerController;
    private AnimatorStateInfo _stateInfo;
    private bool _isAttacking;
    private bool _comboActive;
    private int _currentCombo = 0;
    private int _maxCombo = 2;
    private SwordController _swordController;
    
    public void Enter(PlayerController playerController, GameObject weaponObject)
    {
        _playerController = playerController;
        _swordController = weaponObject.GetComponent<SwordController>();
        
        _currentCombo = 0;
        _swordController.AttackStart();
        _isAttacking = true;
        _comboActive = false;
        
        _playerController.Animator.SetTrigger(Attack);
        
        // 첫번째 공격 이펙트
        _swordController.PlaySlashEffect(0);
    }

    public void Update(PlayerController playerController)
    {
        _stateInfo = _playerController.Animator.GetCurrentAnimatorStateInfo(2);
        float t = _stateInfo.normalizedTime % 1f;
        
        if (!_comboActive && t >= 0.5f && t <= 0.95f)
        {
            _comboActive = true;
        }
        else if (_comboActive && t > 0.7f)
        {
            _comboActive = false;
        }
        
        // 공격 상태 종료
        if (t > 0.95f && _isAttacking)
        {
            _swordController.AttackEnd();
            _isAttacking = false;
        }
    }

    public void Exit()
    {
        _swordController.AttackEnd();
        _isAttacking = false;
        _comboActive = false;
        _playerController.Animator.ResetTrigger(Attack);
        _playerController.Animator.ResetTrigger(NextAttack);
    }

    public void HandleInput(bool isFirePressed, bool isFireHeld)
    {
        // 공격 버튼이 눌렸고 콤보 윈도우가 활성화 상태이면 다음 콤보로 즉시 진행
        if (isFirePressed && _comboActive && _currentCombo < _maxCombo - 1)
        {
            // 콤보 카운터 증가
            _currentCombo++;
            _currentCombo = Mathf.Clamp(_currentCombo, 0, _maxCombo - 1);
            
            // 소드 컨트롤러에 현재 콤보 인덱스 설정
            _swordController.SetComboIndex(_currentCombo);
            
            // 다음 공격 애니메이션 트리거
            _playerController.Animator.SetTrigger(NextAttack);
            
            // 콤보 공격 이펙트 
            _swordController.PlaySlashEffect(_currentCombo);
            
            // 콤보 활성 상태 리셋 (새 콤보의 콤보 윈도우를 기다리기 위해)
            _comboActive = false;
        }
    }

    public bool IsComplete()
    {
        return !_isAttacking;
    }
}