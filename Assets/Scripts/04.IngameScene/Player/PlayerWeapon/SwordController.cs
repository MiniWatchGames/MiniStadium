using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SwordController : MonoBehaviour, IWeapon
{
    [Serializable]
    public class SwordTriggerZone
    {
        public Vector3 position;
        public float radius;
    }
    
    [SerializeField] private SwordTriggerZone[] _triggerZones;
    [SerializeField] private int _attackPower;
    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] private ObservableFloat _currentAmmo;
    [SerializeField] private ObservableFloat _maxAmmo;
    
    [Header("Effects")]
    [SerializeField] private ParticleSystem[] slashEffectPrefabs;
    [SerializeField] private AudioClip[] slashSounds;
    private ParticleSystem[] _slashEffects;
    private AudioSource _audioSource;
    
    // 충돌 처리
    private Vector3[] _previousPositions;
    private HashSet<Collider>[] _comboHitColliders;
    private Ray _ray = new Ray();
    private RaycastHit[] _hits = new RaycastHit[10];
    private bool _isAttacking = false;

    public int Damage { get => _attackPower;}
    public ObservableFloat CurrentAmmo { get => _currentAmmo;}
    public ObservableFloat MaxAmmo { get => _maxAmmo; }

    public bool IsAttacking => _isAttacking;
    
    private int _maxCombo = 2;
    private int _currentComboIndex = 0;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    
    private void Start()
    {
        _previousPositions = new Vector3[_triggerZones.Length];
        
        _comboHitColliders = new HashSet<Collider>[_maxCombo];
        for (int i = 0; i < _maxCombo; i++)
        {
            _comboHitColliders[i] = new HashSet<Collider>();
        }
        
        _slashEffects = new ParticleSystem[slashEffectPrefabs.Length];
        for (int i = 0; i < slashEffectPrefabs.Length; i++)
        {
            _slashEffects[i] = Instantiate(slashEffectPrefabs[i], transform.position, Quaternion.identity);
        }
        _slashEffects[0].gameObject.transform.rotation = Quaternion.Euler(0, -90, 0);

        _attackPower = 10;
        _currentAmmo = new ObservableFloat(10, "SwordCurrentAmmo");
        _maxAmmo = new ObservableFloat(10, "SwordMaxAmmo");
    }
    
    public void SetComboIndex(int index)
    {
        _currentComboIndex = Mathf.Clamp(index, 0, _maxCombo - 1);
        // 새 콤보에 대해 충돌 데이터 초기화
        _comboHitColliders[_currentComboIndex].Clear();
        
        // 이전 위치 초기화
        for (int i = 0; i < _triggerZones.Length; i++)
        {
            _previousPositions[i] = transform.position + transform.TransformVector(_triggerZones[i].position);
        }   
    }

    // 공격 시작 함수
    public void AttackStart()
    {
        _isAttacking = true;
        _currentComboIndex = 0;
        _comboHitColliders[_currentComboIndex].Clear();
        
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

    private void Update()
    {
        if (_isAttacking)
        {
            CheckCollisions();
        }
    }
    
    private void CheckCollisions()
    {
        var currentHitColliders = _comboHitColliders[_currentComboIndex];
        
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
                if (!currentHitColliders.Contains(hit.collider))
                {
                    currentHitColliders.Add(hit.collider);
                    
                    // 피격 객체에 데미지 적용
                    ApplyDamage(hit.collider.gameObject, hit.point);
                }
            }
            _previousPositions[i] = worldPosition;
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
                damage = _attackPower,
                hitPoint = hitPoint,
                hitDirection = (target.transform.position - transform.position).normalized
            };
            
            // 내 검에 내가 맞지 않도록 처리 
            if (target.transform.root != gameObject.transform.root)
            {
                // 데미지 적용
                damageable.TakeDamage(damageInfo);
                
                // 크로스헤어 알림용
                //CombatEvents.OnTargetHit?.Invoke(target);

                _slashEffects[_currentComboIndex].transform.position = hitPoint;
                _slashEffects[_currentComboIndex].Play();
            }
        }
    }

    public void PlaySlashSound(int index)
    {
        _audioSource.PlayOneShot(slashSounds[index], 1f);
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