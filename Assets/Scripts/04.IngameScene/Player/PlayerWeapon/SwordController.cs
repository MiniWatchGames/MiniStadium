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
    [SerializeField] private Stat _attackPower;
    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] private ObservableFloat _currentAmmo;
    [SerializeField] private ObservableFloat _maxAmmo;
    
    [Header("First Skill - Wall Creation")]
    [SerializeField] private GameObject _wallPrefab; // 생성할 벽 프리팹
    [SerializeField] private float _minWallDistance = 2f; // 최소 벽 생성 거리
    [SerializeField] private float _maxWallDistance = 5f; // 최대 벽 생성 거리
    [SerializeField] private float _wallDuration = 10f; // 벽 지속 시간
    
    private GameObject _previewWall; // 미리보기 벽
    private GameObject _currentWall; // 현재 생성된 벽
    
    [Header("Effects")]
    [SerializeField] private ParticleSystem[] slashEffectPrefabs;
    [SerializeField] private Vector3[] slashEffectRotations;
    [SerializeField] private AudioClip[] slashSounds;
    private ParticleSystem[] _slashEffects;
    private AudioSource _audioSource;
    
    // 충돌 처리
    private Vector3[] _previousPositions;
    private HashSet<Collider>[] _comboHitColliders;
    private Ray _ray = new Ray();
    private RaycastHit[] _hits = new RaycastHit[10];
    private bool _isAttacking = false;

    public Stat Damage { get => _attackPower;}
    public ObservableFloat CurrentAmmo { get => null;}
    public ObservableFloat MaxAmmo { get => null; }

    public bool IsAttacking => _isAttacking;
    
    private int _maxCombo = 2;
    private int _currentComboIndex = 0;
    
    private CameraController _camera;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _attackPower = new Stat(10, "_attackPower");
        _currentAmmo = new ObservableFloat(10, "SwordCurrentAmmo");
        _maxAmmo = new ObservableFloat(10, "SwordMaxAmmo");
    }
    
    private void Start()
    {
        var pc = GetComponentInParent<PlayerController>();
        _camera = pc.CameraController;
        
        _previousPositions = new Vector3[_triggerZones.Length];
        
        _comboHitColliders = new HashSet<Collider>[_maxCombo];
        for (int i = 0; i < _maxCombo; i++)
        {
            _comboHitColliders[i] = new HashSet<Collider>();
        }
        
        _slashEffects = new ParticleSystem[slashEffectPrefabs.Length];
        for (int i = 0; i < slashEffectPrefabs.Length; i++)
        {
            var rotation = Quaternion.Euler(slashEffectRotations[i]);
            var effect = Instantiate(slashEffectPrefabs[i], transform.position, rotation);
            _slashEffects[i] = effect;
        }
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
                damage = _attackPower.Value,
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

    public void FirstSkillStart()
    {
        Debug.Log("Sword -- First Skill Start");
        // 이미 미리보기가 있다면 제거
        if (_previewWall != null)
        {
            Destroy(_previewWall);
        }
        
        // 미리보기 벽 생성
        _previewWall = Instantiate(_wallPrefab);
        
        // 모든 렌더러 컴포넌트를 찾아 반투명하게 설정
        MeshRenderer[] renderers = _previewWall.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)
        {
            // 기존 머티리얼의 복사본 생성
            Material previewMaterial = new Material(renderer.material);
            
            // 알파값 조정 (50% 투명)
            Color color = previewMaterial.color;
            color.a = 0.5f;
            previewMaterial.color = color;
            
            // 새 머티리얼 적용
            renderer.material = previewMaterial;
        }
        
        // 콜라이더 비활성화 (미리보기에서는 충돌이 일어나지 않도록)
        Collider[] colliders = _previewWall.GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }
        
        // 미리보기 위치 업데이트 코루틴 시작
        StartCoroutine(UpdatePreviewWallRoutine());
    }

    // 미리보기 벽 위치 업데이트 코루틴
    private IEnumerator UpdatePreviewWallRoutine()
    {
        while (_previewWall != null)
        {
            UpdatePreviewWallPosition();
            yield return null;
        }
    }
    
    // 미리보기 벽 위치 업데이트
    private void UpdatePreviewWallPosition()
    {
        if (_previewWall != null)
        {
            float wallDistance = _maxWallDistance / 2; // 기본 거리
            var pitch = _camera.Pitch;
            float normalizedPitch = 1- ((pitch + 90) / 180f); // 0~1
            wallDistance = Mathf.Lerp(_minWallDistance, _maxWallDistance, normalizedPitch);
            
            //Debug.Log($"Camera Pitch: {pitch}, Normalized: {normalizedPitch}, Wall Distance: {wallDistance}");
            
            // 플레이어(루트) 트랜스폼 가져오기
            Transform rootTransform = transform.root;
            
            // 플레이어 앞에 위치
            Vector3 playerForward = rootTransform.forward;
            Vector3 spawnPosition = rootTransform.position + playerForward * wallDistance;
            
            // 벽 높이 조정 (바닥에서부터 적절한 높이)
            spawnPosition.y = _previewWall.transform.localScale.y / 2;
            
            // 위치와 회전 설정
            _previewWall.transform.position = spawnPosition;
            _previewWall.transform.rotation = Quaternion.LookRotation(playerForward);
        }
    }
    
    public void FirstSkillEnd()
    {
        Debug.Log("Sword -- First Skill End");
        // 실제 벽 생성 위치 저장 (미리보기 위치와 동일)
        Vector3 wallPosition = Vector3.zero;
        Quaternion wallRotation = Quaternion.identity;
        Vector3 wallScale = Vector3.one;
        
        if (_previewWall != null)
        {
            wallPosition = _previewWall.transform.position;
            wallRotation = _previewWall.transform.rotation;
            wallScale = _previewWall.transform.localScale;
            
            // 미리보기 제거
            Destroy(_previewWall);
            _previewWall = null;
        }
        
        // 이전 벽 제거
        if (_currentWall != null)
        {
            Destroy(_currentWall);
        }
        
        // 실제 벽 생성
        _currentWall = Instantiate(_wallPrefab, wallPosition, wallRotation);
        _currentWall.transform.localScale = wallScale;
        
        // 콜라이더 활성화 (미리보기와 달리 실제 벽은 충돌이 일어나도록)
        Collider[] colliders = _currentWall.GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = true;
        }
        
        // 모든 렌더러의 투명도 복원
        MeshRenderer[] renderers = _currentWall.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)
        {
            Color color = renderer.material.color;
            color.a = 1.0f; // 완전 불투명
            renderer.material.color = color;
        }
        
        // 벽 효과음 재생
        
        
        // 이펙트 재생
        
        // 일정 시간 후 벽 제거
        Destroy(_currentWall, _wallDuration);
    }
    
    private void OnDestroy()
    {
        // 미리보기 벽 제거
        if (_previewWall != null)
        {
            Destroy(_previewWall);
        }
        
        // 실제 벽 제거
        if (_currentWall != null)
        {
            Destroy(_currentWall);
        }
        
        if (_slashEffects != null)
        {
            foreach (var slashEffect in _slashEffects)
            {
                Destroy(slashEffect.gameObject);
            }
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