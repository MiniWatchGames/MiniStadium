using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform head; // 플레이어의 머리 
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 0, 0); // 머리 위치에서의 카메라 오프셋
    [SerializeField] private Transform Hips;
    [SerializeField] private float _yaw = 0f;
    [SerializeField] private Transform focus;

    private Transform _target;
    private Transform _Spinetarget;
    private float _pitch = 0f;
    private float passyaw = 0f;
    [SerializeField] private float accumulatedYaw = 0f;
    private bool isAlreadyPlay = false;
    private Vector3 LastTargetRotation;

    public delegate bool WalkingChecker(out Action turningStep, float accumulatedYaw);
    public WalkingChecker IsIdle;
    public Action TurningStep;
    
    

    private void LateUpdate()
    {
        if (!_Spinetarget || !_target)
            return;

        // 카메라를 머리 위치로 이동
        transform.position = head.position + _target.TransformDirection(cameraOffset);

        if (!isAlreadyPlay)
        {
            LastTargetRotation = Hips.rotation.eulerAngles;
            isAlreadyPlay = true;
        }

        // 상체 회전 적용
        _target.rotation = Quaternion.Euler(0, _yaw, 0);
        Hips.rotation = Quaternion.Euler(LastTargetRotation);
        _Spinetarget.rotation = Quaternion.Euler(_pitch, _yaw, 0);

        // 회전 누적값 계산
        float deltaYaw = Mathf.DeltaAngle(passyaw, _yaw);
        accumulatedYaw += deltaYaw;
        passyaw = _yaw;
       TurningStep?.Invoke();
        if (!IsIdle(out TurningStep,accumulatedYaw)) {
            // 회전 제한 처리
            if (accumulatedYaw < -30f || accumulatedYaw > 90f)
            {
                isAlreadyPlay = false;
                Vector3 spineEuler = _Spinetarget.rotation.eulerAngles;
                Vector3 hipsEuler = Hips.rotation.eulerAngles;
                
                hipsEuler.y += _yaw;
                spineEuler.y -= _yaw;
                _Spinetarget.rotation = Quaternion.Euler(spineEuler);
                Hips.rotation = Quaternion.Euler(hipsEuler);
                TurningStep?.Invoke();
                accumulatedYaw = 0f; // 누적값 초기화
            }
        }
        else
        {
            TurningStep?.Invoke();
            isAlreadyPlay = false;  
            accumulatedYaw = 0f; // 누적값 초기화
        }
        // 카메라 회전 적용
        //transform.rotation = Quaternion.Euler(_pitch, _yaw, 0);
        transform.LookAt(focus.position);
    }

    public void SetTarget(Transform target)
    {
        _target = target;

        // 초기 위치 설정 (타겟이 있을 때만 수행)
        if (head != null && _target != null)
        {
            transform.position = head.position + _target.TransformDirection(cameraOffset);
        }
    }

    public void SetSpineTarget(Transform target)
    {
        _Spinetarget = target;
    }

    // PlayerController에서 Pitch 값을 받아 설정하는 함수
    public void SetPitch(float pitch)
    {
        _pitch = pitch;
    }

    // PlayerController에서 yaw 값을 받아 설정하는 함수
    public void SetYaw(float yaw)
    {
        _yaw = yaw;
    }
}