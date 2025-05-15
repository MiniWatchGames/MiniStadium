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
    [SerializeField] private GameObject _previewPrefab; // 미리보기 프리팹 
    [SerializeField] private float _minWallDistance = 2f; // 최소 벽 생성 거리
    [SerializeField] private float _maxWallDistance = 5f; // 최대 벽 생성 거리
    [SerializeField] private float _wallDuration = 10f; // 벽 지속 시간
    [SerializeField] private float _wallRiseSpeed = 5f; // 벽이 올라오는 속도
    [SerializeField] private AnimationCurve _riseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // 올라오는 애니메이션 커브
    
    private GameObject _previewObject; // 미리보기 오브젝트
    private GameObject _currentWall; // 현재 생성된 벽
    private PlayerController _playerController; // 플레이어 컨트롤러 참조
    private Vector3 _wallSpawnPosition; // 벽 생성 위치 저장
    private Quaternion _wallSpawnRotation; // 벽 생성 회전 저장
    
    [Header("Effects")]
    [SerializeField] private ParticleSystem[] slashEffectPrefabs;
    [SerializeField] private ParticleSystem firstSkillEffectPrefab;
    [SerializeField] private Vector3[] slashEffectRotations;
    [SerializeField] private AudioClip[] slashSounds;
    [SerializeField] private AudioClip firstSkillSound;
    private ParticleSystem[] _slashEffects;
    private ParticleSystem _firstSkillEffect;
    private AudioSource _audioSource;
    
    // 충돌 처리
    private Vector3[] _previousPositions;
    private HashSet<GameObject>[] _comboHitGameObjects;
    private Ray _ray = new Ray();
    private RaycastHit[] _hits = new RaycastHit[10];
    private bool _isAttacking = false;

    public Stat Damage { get => _attackPower;}
    public ObservableFloat CurrentAmmo { get => null;}
    public ObservableFloat MaxAmmo { get => null; }

    public bool IsAttacking => _isAttacking;
    
    private int _maxCombo = 2;
    private int _currentComboIndex = 0;

    private PlayerController pc;
    private CameraController _camera;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _attackPower = new Stat(6f, "_attackPower");
        _currentAmmo = new ObservableFloat(10, "SwordCurrentAmmo");
        _maxAmmo = new ObservableFloat(10, "SwordMaxAmmo");
    }
    
    private void Start()
    {
        pc = GetComponentInParent<PlayerController>();
        _camera = pc.CameraController;
        
        _previousPositions = new Vector3[_triggerZones.Length];
        
        _comboHitGameObjects = new HashSet<GameObject>[_maxCombo];
        for (int i = 0; i < _maxCombo; i++)
        {
            _comboHitGameObjects[i] = new HashSet<GameObject>();
        }
        
        _slashEffects = new ParticleSystem[slashEffectPrefabs.Length];
        for (int i = 0; i < slashEffectPrefabs.Length; i++)
        {
            var rotation = Quaternion.Euler(slashEffectRotations[i]);
            var effect = Instantiate(slashEffectPrefabs[i], transform.root, false);
            effect.transform.localRotation = rotation;
            _slashEffects[i] = effect;
            Debug.Log("effect instantiated");
        }
        _firstSkillEffect = Instantiate(firstSkillEffectPrefab);
    }
    
    public void SetComboIndex(int index)
    {
        _currentComboIndex = Mathf.Clamp(index, 0, _maxCombo - 1);
        // 새 콤보에 대해 충돌 데이터 초기화
        _comboHitGameObjects[_currentComboIndex].Clear();
        
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
        _comboHitGameObjects[_currentComboIndex].Clear();
        
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
        var currentHitGameObjects = _comboHitGameObjects[_currentComboIndex];
        
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
                var hitGameObject = hit.collider.gameObject;
                if (!currentHitGameObjects.Contains(hitGameObject))
                {
                    currentHitGameObjects.Add(hitGameObject);
                     ApplyDamage(hitGameObject, hit.point);
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
                hitDirection = (target.transform.position - pc.transform.position).normalized
            };
            
            // 내 검에 내가 맞지 않도록 처리 
            if (target.transform.root != gameObject.transform.root)
            {
                // 데미지 적용
                damageable.TakeDamage(damageInfo);
            }
        }
    }

    public void PlaySlashEffect(int index)
    {
        AudioSource.PlayClipAtPoint(slashSounds[index], transform.position);
        _slashEffects[_currentComboIndex].Play();
    }

    public void FirstSkillStart()
    {
        // 이미 미리보기가 있다면 제거
        if (_previewObject != null)
        {
            Destroy(_previewObject);
        }
        
        // 미리보기 오브젝트 생성
        _previewObject = Instantiate(_previewPrefab);
        _previewObject.SetActive(true);
        
        // 미리보기 위치 업데이트 코루틴 시작
        StartCoroutine(UpdatePreviewRoutine());
    }

    // 미리보기 위치 업데이트 코루틴
    private IEnumerator UpdatePreviewRoutine()
    {
        while (_previewObject != null)
        {
            UpdatePreviewPosition();
            yield return null;
        }
    }
    
    // 미리보기 벽 위치 업데이트
    private void UpdatePreviewPosition()
    {
        if (_previewObject != null)
        {
            float wallDistance = _maxWallDistance / 2; // 기본 거리
            var pitch = _camera.Pitch;
            float normalizedPitch = 1- ((pitch + 90) / 180f); // 0~1
            wallDistance = Mathf.Lerp(_minWallDistance, _maxWallDistance, normalizedPitch);
            
            // 플레이어(루트) 트랜스폼 가져오기
            Transform rootTransform = transform.root;
            
            // 플레이어 앞에 위치
            Vector3 playerForward = rootTransform.forward;
            Vector3 spawnPosition = rootTransform.position + playerForward * wallDistance;
            
            // 바닥 높이에 배치 (약간 위로 올려서 Z-fighting 방지)
            spawnPosition.y = 0.01f;
            
            // 위치 설정
            _previewObject.transform.position = spawnPosition;
            
            // 플레이어 방향에 맞춰 회전 (바닥에 평평하게)
            _previewObject.transform.rotation = Quaternion.Euler(90, rootTransform.eulerAngles.y, 0);
            
            // 생성될 벽의 위치와 회전 저장
            _wallSpawnPosition = spawnPosition;
            _wallSpawnRotation = Quaternion.LookRotation(playerForward);
        }
    }
    
    public void FirstSkillEnd()
    {
        // 미리보기 제거
        if (_previewObject != null)
        {
            Destroy(_previewObject);
            _previewObject = null;
        }
        
        // 이전 벽 제거
        if (_currentWall != null)
        {
            Destroy(_currentWall);
        }
        
        // 실제 벽 생성
        _currentWall = Instantiate(_wallPrefab, _wallSpawnPosition, _wallSpawnRotation);
        _currentWall.layer = LayerMask.NameToLayer("Ground");

        // 벽 이펙트 재생
        PlayWallEffect();
        
        // 바닥에서 올라오는 애니메이션 시작
        StartCoroutine(RiseWallAnimation());
    }

    private void PlayWallEffect()
    {
        _firstSkillEffect.transform.position = _wallSpawnPosition;
        _firstSkillEffect.Play();
        AudioSource.PlayClipAtPoint(firstSkillSound, transform.position);
    }
    
    // 벽이 바닥에서 올라오는 애니메이션
    private IEnumerator RiseWallAnimation()
    {
        if (_currentWall == null) yield break;
        
        // 벽의 초기 위치와 최종 위치 계산
        Vector3 startPosition = _wallSpawnPosition;
        Vector3 endPosition = _wallSpawnPosition;
        float wallHeight = _currentWall.transform.localScale.y;
        
        // 시작 위치는 바닥 아래
        startPosition.y = -wallHeight / 2;
        
        endPosition.y = 0f;
        
        // 벽을 시작 위치에 배치
        _currentWall.transform.position = startPosition;
        
        // 이펙트 재생 (바닥 위치)
        
        
        // 올라오는 애니메이션
        float elapsedTime = 0f;
        float duration = 1f / _wallRiseSpeed; // 속도에 따른 지속 시간
        
        while (elapsedTime < duration)
        {
            if (_currentWall == null) yield break;
            
            float t = elapsedTime / duration;
            float curveValue = _riseCurve.Evaluate(t);
            
            // 벽 위치 보간
            Vector3 currentPosition = Vector3.Lerp(startPosition, endPosition, curveValue);
            _currentWall.transform.position = currentPosition;
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // 최종 위치 확정
        if (_currentWall != null)
        {
            _currentWall.transform.position = endPosition;
        }
        
        // 일정 시간 후 벽 제거
        if (_currentWall != null)
        {
            Destroy(_currentWall, _wallDuration);
        }
    }
    
    private void OnDestroy()
    {
        // 미리보기 벽 제거
        if (_previewObject != null)
        {
            Destroy(_previewObject);
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

        if (_firstSkillEffect != null)
        {
            Destroy(_firstSkillEffect.gameObject);
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