using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 0, 0); // 머리 위치에서의 카메라 오프셋
    [SerializeField] private float positionSmoothSpeed = 12f; // 위치 부드러움 정도
    [SerializeField] private float rotationSmoothSpeed = 30f; // 회전 부드러움 정도
    
    private Transform _head; // 플레이어의 머리
    private float _pitch = 0f;
    private float _yaw = 0f;
    public float Pitch => _pitch;
    public float Yaw => _yaw;
    private Vector3 _currentVelocity; // 위치 스무딩용 속도 변수
    private Quaternion _targetRotation;
    
    
    private void LateUpdate()
    {
        if (_head == null)
            return;
        
        // 카메라 회전 타겟 설정
        _targetRotation = Quaternion.Euler(_pitch, _yaw, 0);
        
        // 카메라 회전 스무딩 적용
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime * rotationSmoothSpeed);
        
        // 눈 위치 계산 (머리 위치 + 눈 오프셋)
        Vector3 eyePosition = _head.position + _head.TransformDirection(cameraOffset);
        
        // 위치 스무딩 적용 (SmoothDamp 사용 - 가속/감속 효과)
        transform.position = Vector3.SmoothDamp(
            transform.position, 
            eyePosition, 
            ref _currentVelocity, 
            1f / positionSmoothSpeed
        );
    }

    public void ResetCamera(Transform head)
    {
        _head = head;
        transform.position = _head.position + cameraOffset;
    }
    
    // 마우스 입력에서 호출될 메서드
    public void UpdateCamera(float pitch, float yaw)
    {
        _pitch = pitch;
        _yaw = yaw;
    }
}