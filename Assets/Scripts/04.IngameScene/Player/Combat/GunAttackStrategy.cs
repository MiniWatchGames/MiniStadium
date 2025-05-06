using UnityEngine;

public class GunAttackStrategy : IAttackStrategy ,  IStatObserver
    {
        private static readonly int IsFiring = Animator.StringToHash("IsFiring");
        private PlayerController _playerController;
        private AnimatorStateInfo _stateInfo;
        private bool _isAttacking;
        private float _lastFireTime;
        private float _fireRate = 0.2f; // 발사 간격 (초)
        private GunController _gunController;
        private bool _CanAttack = true;    
        public bool CanAttack { get => _CanAttack; set => _CanAttack = value; }

        
        public void Enter(PlayerController playerController, GameObject weaponObject)
        {
           _playerController = playerController;

            if (_gunController == null) {     
               _gunController = weaponObject.GetComponent<GunController>();
                ReloadAmmo();
        }
            _CanAttack = true;
        
            _gunController.CurrentAmmo.AddObserver(this); // Observer 등록 ,Observer 당하는 곳에서 중복 처리

            //currentAmmo에 전략 등록

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
            if (!_CanAttack) return;
            // 탄환 생성 로직 (필요시 구현)
            _gunController.Fire();
             Debug.Log(_gunController.CurrentAmmo.Value  + " / " + _gunController.MaxAmmo.Value);
            // 발사 시간 기록
            _lastFireTime = Time.time;
        }

        public void ReloadAmmo() {
            _gunController.CurrentAmmo.Value = _gunController.MaxAmmo.Value; 
        }
        public void WhenStatChanged((float, string) data)
        {
            //총알이 0이 되면 
            if (data.Item1 <= 0) {
                _CanAttack = false;
                _playerController.SetActionState("Reload");
            }
        }
}
