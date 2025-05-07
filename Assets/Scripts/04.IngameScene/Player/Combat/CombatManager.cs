using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class CombatManager : NetworkBehaviour
{
    [SerializeField] private PlayerController playerController;
    private Dictionary<WeaponType, IAttackStrategy> _attackStrategies = new Dictionary<WeaponType, IAttackStrategy>();
    private IAttackStrategy _currentStrategy;
    private WeaponType _currentWeaponType;
    public GameObject CurrentWeapon { get; set; }

    private int _upperBodyLayerIndex = 2;
    private float attackStartTransitionTime = 0.15f; // 공격 시작 시 전환 시간
    private float attackEndTransitionTime = 0.3f;   // 공격 종료 시 전환 시간
    private Coroutine _layerTransitionCoroutine; // 레이어 전환 코루틴
    
    private void Awake()
    {
        InitializeStrategies();
    }

    private void Start()
    {
        playerController.Animator.SetLayerWeight(_upperBodyLayerIndex, 0f);
    }
    
    private void InitializeStrategies()
    {
        _attackStrategies[WeaponType.Sword] = new SwordAttackStrategy();
        _attackStrategies[WeaponType.Gun] = new GunAttackStrategy();
    }
    
    public void SetWeaponType(WeaponType type)
    {
        _currentWeaponType = type;
        _currentStrategy = _attackStrategies[type];
    }
    
    public void StartAttack()
    {
        if (_currentStrategy != null)
        {
            _currentStrategy.Enter(playerController, CurrentWeapon);
            
            // 상체 레이어 가중치 증가 시작
            FadeInUpperBodyLayer();
        }
    }
    
    public void UpdateAttack()
    {
        if (_currentStrategy != null)
        {
            _currentStrategy.Update(playerController);
        }
    }
    
    public void ProcessInput(bool isFirePressed, bool isFireHeld)
    {
        if (_currentStrategy != null)
        {
            _currentStrategy.HandleInput(isFirePressed, isFireHeld);
        }
    }
    
    public bool IsAttackComplete()
    {
        return _currentStrategy.IsComplete();
    }
    
    public void EndAttack()
    {
        _currentStrategy?.Exit();
        // 상체 레이어 가중치 감소 시작
        FadeOutUpperBodyLayer();
    }
    
    // 상체 레이어 가중치 증가 (공격 시작)
    public void FadeInUpperBodyLayer()
    {
        // 이전 코루틴 중지
        if (_layerTransitionCoroutine != null)
        {
            StopCoroutine(_layerTransitionCoroutine);
        }
        
        // 새 코루틴 시작
        _layerTransitionCoroutine = StartCoroutine(LayerWeightTransition(0f, 1f, attackStartTransitionTime));
    }
    
    // 상체 레이어 가중치 감소 (공격 종료)
    public void FadeOutUpperBodyLayer()
    {
        // 이전 코루틴 중지
        if (_layerTransitionCoroutine != null)
        {
            StopCoroutine(_layerTransitionCoroutine);
        }
        
        // 새 코루틴 시작
        _layerTransitionCoroutine = StartCoroutine(LayerWeightTransition(1f, 0f, attackEndTransitionTime));
    }
    
    // 레이어 가중치 전환 코루틴
    private IEnumerator LayerWeightTransition(float startWeight, float targetWeight, float duration)
    {
        if (playerController.Animator == null) yield break;
        
        // 시작 가중치 설정
        playerController.Animator.SetLayerWeight(_upperBodyLayerIndex, startWeight);
        
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            
            // 부드러운 보간을 위한 Smooth Step 함수
            float smoothT = t * t * (3f - 2f * t);
            
            // 가중치 보간
            float newWeight = Mathf.Lerp(startWeight, targetWeight, smoothT);
            playerController.Animator.SetLayerWeight(_upperBodyLayerIndex, newWeight);
            
            yield return null;
        }
        
        // 최종 값 설정
        playerController.Animator.SetLayerWeight(_upperBodyLayerIndex, targetWeight);
    }
}