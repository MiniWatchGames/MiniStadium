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
            
            _swordController.AttackStart();
            _isAttacking = true;
            _comboActive = false;
            _currentCombo = 0;
            
            _playerController.Animator.SetTrigger(Attack);
        }

        public void Update(PlayerController playerController)
        {
            _stateInfo = _playerController.Animator.GetCurrentAnimatorStateInfo(2);
            float t = _stateInfo.normalizedTime % 1f;
            
            if (!_comboActive && t >= 0.3f && t <= 0.7f)
            {
                _comboActive = true;
            }
            else if (_comboActive && t > 0.7f)
            {
                _comboActive = false;
            }
            
            // 공격 완료 확인
            if (t >= 0.95f)
            {
                // 다음 콤보가 트리거되지 않았거나 마지막 콤보면 공격 종료
                if (_currentCombo >= _maxCombo - 1)
                {
                    _swordController.AttackEnd();
                    _isAttacking = false;
                }
                else
                {
                    // 다음 콤보로 전환될 때 상태 업데이트
                    _swordController.AttackEnd();
                    _currentCombo++;
                    _swordController.AttackStart();
                }
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
            // 공격 버튼이 눌렸고 콤보 윈도우가 활성화 상태이면 다음 콤보 트리거
            if (isFirePressed && _comboActive && _currentCombo < _maxCombo - 1)
            {
                _playerController.Animator.SetTrigger(NextAttack);
            }
        }

        public bool IsComplete()
        {
            return !_isAttacking;
        }
    }

