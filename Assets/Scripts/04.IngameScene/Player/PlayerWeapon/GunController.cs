using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GunController : NetworkBehaviour, IWeapon
{
    // 이펙트 유형 정의
    protected enum EffectType
    {
        MuzzleFlash,
        HitEffect,
        Bullet
    }
    
    // 이펙트 프리팹 구조체
    [Serializable]
    protected struct EffectPrefab
    {
        public EffectType type;
        public GameObject prefab;
        public int poolSize;
        public float duration; // 자동 반환 시간
    }
    
    // 풀에 있는 오브젝트 관리를 위한 내부 클래스
    private class PooledObject
    {
        public GameObject gameObject;
        public EffectType type;
        
        public PooledObject(GameObject gameObject, EffectType type)
        {
            this.gameObject = gameObject;
            this.type = type;
        }
    }
    
    [SerializeField] private Stat damage;
    [SerializeField] private float range = 100f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private LayerMask targetMask;
    
    [Header("Effects")]
    [SerializeField] private EffectPrefab[] effectPrefabs; // 구조체 배열
    [SerializeField] private float bulletSpeed = 100f;
    [SerializeField ]private AudioClip shotSound;
    [SerializeField ]private AudioClip ReloadSound;
    private AudioSource _audioSource;
    
    // 풀 관리
    private Dictionary<EffectType, Transform> _poolParents = new Dictionary<EffectType, Transform>();
    private Dictionary<EffectType, List<PooledObject>> _objectPools = new Dictionary<EffectType, List<PooledObject>>();
    private Transform _poolContainer;
    
    private CameraController _camera;
    private RaycastHit _hitInfo;
    private ObservableFloat _currentAmmo;
    private ObservableFloat _maxAmmo;

    public Stat Damage { get => damage; }
    public ObservableFloat CurrentAmmo { get => _currentAmmo; }
    public ObservableFloat MaxAmmo { get => _maxAmmo; }

    private void Awake()
    {
        // 풀 컨테이너 생성
        _poolContainer = new GameObject("EffectPools").transform;
        _poolContainer.SetParent(transform);
        _audioSource = GetComponent<AudioSource>();
        damage = new Stat(20, "GunDamage");
        _currentAmmo = new ObservableFloat(30, "GunCurrentAmmo");
        _maxAmmo = new ObservableFloat(30, "GunMaxAmmo");
    }

    private void Start()
    {
        var players = FindObjectsOfType<PlayerController>();
        foreach (var pc in players)
        {
            if (pc.Owner == Owner)
            {
                _camera = pc.CameraController;
                break;
            }
        }
        
        // 풀 초기화
        InitializeObjectPools();
    }
    
    private void InitializeObjectPools()
    {
        // 각 이펙트 타입별 풀 생성
        foreach (var effectPrefab in effectPrefabs)
        {
            if (effectPrefab.prefab == null)
                continue;
                
            // 풀 부모 객체 생성
            string poolName = effectPrefab.type.ToString() + "Pool";
            Transform poolParent = new GameObject(poolName).transform;
            poolParent.SetParent(_poolContainer);
            _poolParents[effectPrefab.type] = poolParent;
            
            // 풀 리스트 생성
            List<PooledObject> pool = new List<PooledObject>();
            _objectPools[effectPrefab.type] = pool;
            
            // 풀 객체 생성
            for (int i = 0; i < effectPrefab.poolSize; i++)
            {
                GameObject obj = Instantiate(effectPrefab.prefab, poolParent);
                obj.SetActive(false);
                pool.Add(new PooledObject(obj, effectPrefab.type));
            }
        }
    }
    
    // 풀에서 오브젝트 가져오기
    private GameObject GetPooledObject(EffectType type, Transform parent = null)
    {
        if (!_objectPools.ContainsKey(type))
            return null;
            
        List<PooledObject> pool = _objectPools[type];
        
        // 비활성화된 오브젝트 찾기
        foreach (var pooledObj in pool)
        {
            if (pooledObj.gameObject != null && !pooledObj.gameObject.activeInHierarchy)
            {
                // 부모 설정
                if (parent != null)
                {
                    pooledObj.gameObject.transform.SetParent(parent);
                }
                
                // 활성화
                pooledObj.gameObject.SetActive(true);
                return pooledObj.gameObject;
            }
        }
        
        // 사용 가능한 객체가 없으면 풀의 첫 번째 객체를 재사용
        if (pool.Count > 0)
        {
            var obj = pool[0].gameObject;
            
            // 부모 설정
            if (parent != null)
            {
                obj.transform.SetParent(parent);
            }
            
            // 활성화
            obj.SetActive(true);
            return obj;
        }
        
        return null;
    }
    
    // 오브젝트 풀로 반환
    protected void ReturnToPool(GameObject obj, EffectType type)
    {
        if (obj == null) 
            return;
            
        if (!_poolParents.ContainsKey(type))
            return;
            
        // 부모를 풀로 다시 변경
        obj.transform.SetParent(_poolParents[type]);
        
        // 비활성화
        obj.SetActive(false);
    }
    
    // 이펙트 프리팹 찾기
    private EffectPrefab? GetEffectPrefab(EffectType type)
    {
        foreach (var prefab in effectPrefabs)
        {
            if (prefab.type == type)
                return prefab;
        }
        return null;
    }

    [ServerRpc]
    public void Fire()
    {
        FireVisualize();
    }
    
    [ObserversRpc]
    public void FireVisualize()
    {
        // 총구 효과
        EffectPrefab? muzzleFlashPrefab = GetEffectPrefab(EffectType.MuzzleFlash);
        if (muzzleFlashPrefab.HasValue && muzzleFlashPrefab.Value.prefab != null)
        {
            GameObject muzzleFlash = GetPooledObject(EffectType.MuzzleFlash, firePoint);
            if (muzzleFlash != null)
            {
                muzzleFlash.transform.position = firePoint.position;
                muzzleFlash.transform.rotation = firePoint.rotation;
                
                // 일정 시간 후 자동 반환
                StartCoroutine(ReturnObjectAfterDelay(
                    muzzleFlash, 
                    EffectType.MuzzleFlash, 
                    muzzleFlashPrefab.Value.duration));
            }
        }
        
        // 발사 효과음
        _audioSource.PlayOneShot(shotSound, 1f);
        
        Vector3 hitPosition = firePoint.position + _camera.transform.forward * range;
        bool hitTarget = false;
        
        // 레이캐스트로 타격 확인
        if (Physics.Raycast(firePoint.position, _camera.transform.forward, out _hitInfo, range, targetMask))
        {
            hitPosition = _hitInfo.point;
            hitTarget = true;
            
            // 피격 효과
            EffectPrefab? hitEffectPrefab = GetEffectPrefab(EffectType.HitEffect);
            if (hitEffectPrefab.HasValue && hitEffectPrefab.Value.prefab != null)
            {
                GameObject hitEffect = GetPooledObject(EffectType.HitEffect);
                if (hitEffect != null)
                {
                    hitEffect.transform.position = _hitInfo.point;
                    hitEffect.transform.rotation = Quaternion.LookRotation(_hitInfo.normal);
                    
                    // 일정 시간 후 자동 반환
                    StartCoroutine(ReturnObjectAfterDelay(
                        hitEffect, 
                        EffectType.HitEffect, 
                        hitEffectPrefab.Value.duration));
                }
            }
            
            // 데미지 적용
            ApplyDamage(_hitInfo.collider.gameObject, _hitInfo.point, firePoint.forward);
        }
        
        _currentAmmo.Value -= 1;

        // 총알 시뮬레이션
        EffectPrefab? bulletPrefab = GetEffectPrefab(EffectType.Bullet);
        if (bulletPrefab.HasValue && bulletPrefab.Value.prefab != null)
        {
            StartCoroutine(SimulateBulletWithPooling(
                firePoint.position, 
                hitPosition, 
                bulletPrefab.Value.duration));
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
                damage = damage.Value,
                hitPoint = hitPoint,
                hitDirection = hitDirection
            };

            damageable.TakeDamage(damageInfo);
            
            // 크로스헤어 알림용
            //CombatEvents.OnTargetHit?.Invoke(target);
        }
    }
    
    // 일정 시간 후 풀로 반환
    private IEnumerator ReturnObjectAfterDelay(GameObject obj, EffectType type, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (obj != null && obj.activeInHierarchy)
        {
            ReturnToPool(obj, type);
        }
    }
    
    // 풀링을 사용한 총알 시뮬레이션
    private IEnumerator SimulateBulletWithPooling(Vector3 startPos, Vector3 endPos, float maxDuration)
    {
        // 풀에서 총알 가져오기
        GameObject bullet = GetPooledObject(EffectType.Bullet);
        if (bullet == null) yield break;
        
        // 총알 위치 설정
        bullet.transform.position = startPos;
        
        // 총알이 나아갈 방향 설정
        Vector3 direction = (endPos - startPos).normalized;
        bullet.transform.forward = direction;
        
        // 이동 거리와 시간 계산
        float distance = Vector3.Distance(startPos, endPos);
        float remainingDistance = distance;
        float travelTime = distance / bulletSpeed;
        
        // 시간 기반 이동
        float startTime = Time.time;
        Vector3 currentPos = startPos;
        
        // 총알 이동 시뮬레이션
        while (remainingDistance > 0 && Time.time - startTime < maxDuration)
        {
            // 총알이 비활성화됐는지 확인
            if (bullet == null || !bullet.activeInHierarchy)
            {
                yield break;
            }
            
            // 현재 위치 업데이트
            float timeRatio = (Time.time - startTime) / travelTime;
            currentPos = Vector3.Lerp(startPos, endPos, timeRatio);
            bullet.transform.position = currentPos;
            
            // 남은 거리 계산
            remainingDistance = Vector3.Distance(currentPos, endPos);
            
            yield return null;
        }
        
        // 최종 위치 설정 및 풀로 반환
        if (bullet != null && bullet.activeInHierarchy)
        {
            bullet.transform.position = endPos;
            yield return new WaitForSeconds(0.05f);
            
            ReturnToPool(bullet, EffectType.Bullet);
        }
    }

    public void ReloadSoundPlay() {
        _audioSource.PlayOneShot(ReloadSound);
    }

    // 게임 종료 시 풀 정리
    private void OnDestroy()
    {
        if (_poolContainer != null)
        {
            Destroy(_poolContainer.gameObject);
        }
    }
    
    public void FirstSkillStart()
    {
        Debug.Log("Gun -- First Skill Start");
    }

    public void FirstSkillEnd()
    {
        Debug.Log("Gun -- First Skill End");
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (firePoint != null && _camera != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(firePoint.position, _camera.transform.forward * range);
        }
    }
#endif
}