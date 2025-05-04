using System;
using System.Collections;
using UnityEngine;

public class GunController : MonoBehaviour , IWeapon
{
    [SerializeField] private int damage = 20;
    [SerializeField] private float range = 100f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private LayerMask targetMask;
    
    [Header("Effects")]
    //[SerializeField] private GameObject muzzleFlashPrefab;
    //[SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private TrailRenderer bulletTrailPrefab;
    [SerializeField] private float bulletSpeed = 100f;
    [SerializeField] private float trailDuration = 0.5f; // 시뮬레이션 지속 시간
    
    private CameraController _camera;
    private RaycastHit _hitInfo;
    private ObservableFloat _currentAmmo;
    private ObservableFloat _maxAmmo;

    public int Damage { get => damage; }
    public ObservableFloat CurrentAmmo { get => _currentAmmo; }
    public ObservableFloat MaxAmmo { get => _maxAmmo; }

    private void Start()
    {
        var pc = GetComponentInParent<PlayerController>();
        _camera = pc.CameraController;
        _currentAmmo = new ObservableFloat(30, "GunCurrentAmmo");
        _maxAmmo = new ObservableFloat(30, "GunMaxAmmo");
    }
    public void Fire()
    {
        // 총구 효과
        // if (muzzleFlashPrefab != null)
        // {
        //     Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation, firePoint);
        // }
        
        // 발사 효과음
        // AudioManager.Instance.PlaySound("GunShot", firePoint.position);
        
        Vector3 hitPosition = firePoint.position + _camera.transform.forward * range;
        bool hitTarget = false;
        
        // 레이캐스트로 타격 확인
        if (Physics.Raycast(firePoint.position, _camera.transform.forward, out _hitInfo, range, targetMask))
        {
            hitPosition = _hitInfo.point;
            hitTarget = true;
            
            // 피격 효과
            // if (hitEffectPrefab != null)
            // {
            //     Instantiate(hitEffectPrefab, _hitInfo.point, Quaternion.LookRotation(_hitInfo.normal));
            // }
            
            // 데미지 적용
            hitPosition = _hitInfo.point;
            hitTarget = true;

            ApplyDamage(_hitInfo.collider.gameObject, _hitInfo.point, firePoint.forward);
        }
        // 총알 궤적 시뮬레이션
        if (bulletTrailPrefab != null)
        {
            StartCoroutine(SimulateBulletTrail(firePoint.position, hitPosition));
        }
    }
    
    private void ApplyDamage(GameObject target, Vector3 hitPoint, Vector3 hitDirection)
    {
        IDamageable damageable = target.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            DamageInfo damageInfo = new DamageInfo
            {
                attacker = gameObject,
                damage = damage,
                hitPoint = hitPoint,
                hitDirection = hitDirection
            };

            damageable.TakeDamage(damageInfo);
            
            // 크로스헤어 알림용
            CombatEvents.OnTargetHit?.Invoke(target);

            //Debug.Log($"Hit {target.name} for {damage} damage at {hitPoint}");
        }
    }
    
    private IEnumerator SimulateBulletTrail(Vector3 startPos, Vector3 endPos)
    {
        // 트레일 생성
        TrailRenderer trail = Instantiate(bulletTrailPrefab, startPos, Quaternion.identity);
        
        // 이동 거리와 시간 계산
        float distance = Vector3.Distance(startPos, endPos);
        float remainingDistance = distance;
        float travelTime = distance / bulletSpeed;
        
        // 시간 기반 이동
        float startTime = Time.time;
        Vector3 currentPos = startPos;
        
        // 총알 이동 시뮬레이션
        while (remainingDistance > 0)
        {
            // 현재 위치 업데이트
            float timeRatio = (Time.time - startTime) / travelTime;
            currentPos = Vector3.Lerp(startPos, endPos, timeRatio);
            trail.transform.position = currentPos;
            
            // 남은 거리 계산
            remainingDistance = Vector3.Distance(currentPos, endPos);
            
            yield return null;
        }
        
        // 최종 위치
        trail.transform.position = endPos;
        
        // 트레일이 자연스럽게 사라질 때까지 대기
        yield return new WaitForSeconds(trail.time);
        
        // 객체 정리
        Destroy(trail.gameObject);
    }
    
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(firePoint.position, _camera.transform.forward * range);
        }
    }
#endif
}