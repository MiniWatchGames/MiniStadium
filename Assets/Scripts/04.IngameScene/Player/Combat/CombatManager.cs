using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    private Dictionary<WeaponType, IAttackStrategy> _attackStrategies = new Dictionary<WeaponType, IAttackStrategy>();
    private IAttackStrategy _currentStrategy;
    private WeaponType _currentWeaponType;
    
    private void Awake()
    {
        InitializeStrategies();
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
            _currentStrategy.Enter(playerController);
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
        return _currentStrategy?.IsComplete() ?? true;
    }
    
    public void EndAttack()
    {
        _currentStrategy?.Exit();
    }
}
