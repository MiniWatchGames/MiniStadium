using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform playerBody; // 플레이어 전체 몸통
    [SerializeField] private Transform head; // 플레이어의 머리
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 0, 0); // 머리 위치에서의 카메라 오프셋

    [SerializeField] private float positionSmoothSpeed = 12f; // 위치 부드러움 정도
    [SerializeField] private float rotationSmoothSpeed = 10f; // 회전 부드러움 정도
    
    private float _pitch = 0f;
    private float _yaw = 0f;
    private Vector3 _currentVelocity; // 위치 스무딩용 속도 변수
    private Quaternion _targetRotation;
    
    private void Start()
    {
        // 초기 카메라 위치 설정
        if (head != null)
        {
            transform.position = head.position + cameraOffset;
        }
    }
    
    private void LateUpdate()
    {
        if (playerBody == null || head == null)
            return;
        
        // 플레이어 몸통 전체를 yaw 방향으로 회전 (여기도 스무딩 가능)
        Quaternion targetBodyRotation = Quaternion.Euler(0, _yaw, 0);
        playerBody.rotation = Quaternion.Slerp(playerBody.rotation, targetBodyRotation, Time.deltaTime * rotationSmoothSpeed);
        
        // 카메라 회전 타겟 설정
        _targetRotation = Quaternion.Euler(_pitch, _yaw, 0);
        
        // 카메라 회전 스무딩 적용
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime * rotationSmoothSpeed);
        
        // 눈 위치 계산 (머리 위치 + 눈 오프셋)
        Vector3 eyePosition = head.position + head.TransformDirection(cameraOffset);
        
        // 위치 스무딩 적용 (SmoothDamp 사용 - 가속/감속 효과)
        transform.position = Vector3.SmoothDamp(
            transform.position, 
            eyePosition, 
            ref _currentVelocity, 
            1f / positionSmoothSpeed
        );
    }
    
    // 마우스 입력에서 호출될 메서드
    public void UpdateCamera(float pitch, float yaw)
    {
        _pitch = pitch;
        _yaw = yaw;
    }
}