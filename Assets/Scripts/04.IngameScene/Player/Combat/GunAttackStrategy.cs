using UnityEngine;

public class GunAttackStrategy : IAttackStrategy
    {
        private static readonly int IsFiring = Animator.StringToHash("IsFiring");
        private PlayerController _playerController;
        private AnimatorStateInfo _stateInfo;
        private bool _isAttacking;
        private float _lastFireTime;
        private float _fireRate = 0.2f; // 발사 간격 (초)
        private GunController _gunController;
        
        public void Enter(PlayerController playerController, GameObject weaponObject)
        {
           _playerController = playerController;
           _gunController = weaponObject.GetComponent<GunController>();
           
           _isAttacking = true;
           
           _playerController.Animator.SetBool(IsFiring, true);
        }

        public void Update(PlayerController playerController)
        {
            if (!_isAttacking) return;
        
            // 연속 발사 처리
            if (Time.time >= _lastFireTime + _fireRate)
            {
                FireBullet();
            }
        }

        public void Exit()
        {
            _isAttacking = false;
            _playerController.Animator.SetBool(IsFiring, false);
        }

        public void HandleInput(bool isFirePressed, bool isFireHeld)
        {
            // 마우스 버튼을 떼면 발사 중지
            if (!isFireHeld)
            {
                _isAttacking = false;
                _playerController.Animator.SetBool(IsFiring, false);
            }
        }

        public bool IsComplete()
        {
            return !_isAttacking;
        }
        
        private void FireBullet()
        {
            // 탄환 생성 로직 (필요시 구현)
            _gunController.Fire();
        
            // 발사 시간 기록
            _lastFireTime = Time.time;
        }
    }
