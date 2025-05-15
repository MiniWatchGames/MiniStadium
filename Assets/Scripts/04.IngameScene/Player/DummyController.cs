using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RepairShopEnums;

public class DummyController : MonoBehaviour, IDamageable
{
    [SerializeField] private LayerMask groundLayer;
    Animator Animator;
    private CharacterController _characterController;
    private const float Gravity = -9.81f;
    private Vector3 _velocity = Vector3.zero;
  
    public float _maxHp = 100;
    private ObservableFloat _currentHp;
    public Action<GameObject> OnDieCallBack; public bool IsGrounded
    {
        get { return GetDistanceToGround() <= 0.03f; }
    }
    public ObservableFloat CurrentHp
    {
        get => _currentHp;
    }
    
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>(); 
        Animator = GetComponent<Animator>();
        _currentHp = new ObservableFloat(_maxHp, "Dummy");
    }
   
    public void TakeDamage(DamageInfo damageInfo)
    {
        var damage = damageInfo.damage;
        CurrentHp.Value -= damage;
        if (CurrentHp.Value <= 0)
        {
            //CurrentHp = 0;
            Animator.Play("Dead");
            Destroy(this.gameObject, 3f);
            OnDieCallBack?.Invoke(gameObject);
        }

        Debug.Log($"current H" +
           $"p = {CurrentHp.Value}");
    }

    private void OnAnimatorMove()
    {
        Vector3 movePosition;
        movePosition = Animator.deltaPosition;


        if (IsGrounded)
        {
            movePosition = Animator.deltaPosition;
        }
        else
        {
            movePosition = _characterController.velocity * Time.deltaTime;
        }

        // 중력 적용
        _velocity.y += Gravity * Time.deltaTime;
        movePosition.y = _velocity.y * Time.deltaTime;
        _characterController.Move(movePosition);
    }
    // public void ResetHp()
    // {
    //     CurrentHp = 100;
    // }
    public float GetDistanceToGround()
    {
        float maxDistance = 10f;
        if (Physics.Raycast(_characterController.transform.position,
                Vector3.down, out RaycastHit hit, maxDistance, groundLayer))
        {
            return hit.distance;
        }
        else
        {
            return maxDistance;
        }
    }
}
