using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    [Serializable]
    public class SwordTriggerZone
    {
        public Vector3 position;
        public float radius;
    }
    
    [SerializeField] private SwordTriggerZone[] _triggerZones;
    
    [SerializeField] private int attackPower = 10;
    [SerializeField] private LayerMask targetLayerMask;
    
    // 충돌 처리
    private Vector3[] _previousPositions;
    private HashSet<Collider> _hitColliders;
    private Ray _ray = new Ray();
    private RaycastHit[] _hits = new RaycastHit[10];
    private bool _isAttacking = false;

    private void Start()
    {
        _previousPositions = new Vector3[_triggerZones.Length];
        _hitColliders = new HashSet<Collider>();
    }

    // 공격 시작 함수
    public void AttackStart()
    {
        _isAttacking = true;
        _hitColliders.Clear();

        for (int i = 0; i < _triggerZones.Length; i++)
        {
            _previousPositions[i] = transform.position + transform.TransformVector(_triggerZones[i].position);
        }
    }

    // 공격 종료 함수
    public void AttackEnd()
    {

        _isAttacking = false;
    }

    private void FixedUpdate()
    {
        if (_isAttacking)
        {
            for (int i = 0; i < _triggerZones.Length; i++)
            {
                var worldPosition = transform.position + 
                                    transform.TransformVector(_triggerZones[i].position);
                var direction = worldPosition - _previousPositions[i];
                _ray.origin = _previousPositions[i];
                _ray.direction = direction;
                
                var hitCount = Physics.SphereCastNonAlloc(_ray, 
                    _triggerZones[i].radius, _hits, 
                    direction.magnitude, targetLayerMask,
                    QueryTriggerInteraction.UseGlobal);
                
                for (int j = 0; j < hitCount; j++)
                {
                    var hit = _hits[j];
                    if (!_hitColliders.Contains(hit.collider))
                    {
                        _hitColliders.Add(hit.collider);
                        
                        // 피격 객체에 데미지 적용
                        ApplyDamage(hit.collider.gameObject, hit.point);
                    }
                }
                _previousPositions[i] = worldPosition;
            }
        }
    }

    // 피격 처리 함수
    private void ApplyDamage(GameObject target, Vector3 hitPoint)
    {
        // IDamageable 인터페이스 구현체 확인
        IDamageable damageable = target.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            // 데미지 정보 생성
            DamageInfo damageInfo = new DamageInfo
            {
                attacker = gameObject,
                damage = attackPower,
                hitPoint = hitPoint,
                hitDirection = (target.transform.position - transform.position).normalized
            };

            // 데미지 적용
            damageable.TakeDamage(damageInfo);

            // 효과음 재생
            // AudioManager.Instance.PlaySoundEffect("SwordHit", hitPoint);

            // 시각 효과 생성
            // EffectManager.Instance.CreateEffect("SwordHitSpark", hitPoint, Quaternion.LookRotation(damageInfo.hitDirection));

            //Debug.Log($"Hit {target.name} for {attackPower} damage at {hitPoint}");
        }
    }
#if UNITY_EDITOR
    
    private void OnDrawGizmos()
    {
        if (_isAttacking && _previousPositions != null)
        {
            for (int i = 0; i < _triggerZones.Length; i++)
            {
                var worldPosition = transform.position +
                                    transform.TransformVector(_triggerZones[i].position);
                var direction = worldPosition - _previousPositions[i];
                
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(worldPosition, _triggerZones[i].radius);
                
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(_previousPositions[i], _triggerZones[i].radius);
                
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(_previousPositions[i], worldPosition);
            }
        }
        else
        {
            foreach (var triggerZone in _triggerZones)
            {
                Gizmos.color = Color.green;
                var worldPos = transform.position + transform.TransformVector(triggerZone.position);
                Gizmos.DrawWireSphere(worldPos, triggerZone.radius);
            }   
        }
    }
    
#endif
}