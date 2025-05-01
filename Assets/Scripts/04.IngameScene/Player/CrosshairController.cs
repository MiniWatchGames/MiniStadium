using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    private Image _crosshairImage;

    private void Awake()
    {
        _crosshairImage = this.GetComponent<Image>();
    }

    private void OnEnable()
    {
        CombatEvents.OnTargetHit += ShowHitEffect;
    }

    private void OnDisable()
    {
        CombatEvents.OnTargetHit -= ShowHitEffect;
    }

    private void ShowHitEffect(GameObject target)
    {
        // 피격 효과 (애니메이션, 크기 변화, 색 변화 등)
        Debug.Log("크로스헤어 히트 피드백 실행!");
    }
}
