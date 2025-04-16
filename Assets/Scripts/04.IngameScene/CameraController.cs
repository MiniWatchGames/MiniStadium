using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private float headHeight = 1.6f; // 플레이어의 머리 높이
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 0, 0.015f); // 머리 위치에서의 카메라 오프셋
    
    private Transform _target;
    private float _pitch = 0f;
    
    private void LateUpdate()
    {
        if (!_target) return;
        
        // 카메라를 머리 위치로 이동
        Vector3 headPosition = _target.position + new Vector3(0, headHeight, 0);
        transform.position = headPosition + _target.TransformDirection(cameraOffset);
        
        // 카메라 회전 적용 (Yaw는 플레이어와 함께 회전)
        transform.rotation = Quaternion.Euler(_pitch, _target.eulerAngles.y, 0);
    }
    
    public void SetTarget(Transform target)
    {
        _target = target;
        
        // 초기 위치 설정
        Vector3 headPosition = _target.position + new Vector3(0, headHeight, 0);
        transform.position = headPosition + _target.TransformDirection(cameraOffset);
    }
    
    // PlayerController에서 Pitch 값을 받아 설정하는 함수
    public void SetPitch(float pitch)
    {
        _pitch = pitch;
    }
}