using UnityEngine;

// 피격 가능한 개체 인터페이스
public interface IDamageable
{
    void TakeDamage(DamageInfo damageInfo);
}

// 데미지 정보 구조체
public struct DamageInfo
{
    public GameObject attacker;
    public float damage;
    public Vector3 hitPoint;
    public Vector3 hitDirection;
    // 필요에 따라 추가 속성(크리티컬, 속성 등)
}
