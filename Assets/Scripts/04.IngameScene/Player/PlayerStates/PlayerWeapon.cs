using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Sword,
    Gun,
}

public struct RangedWeaponStat
{
    public bool isInfiniteAmmo;
    public Stat maxAmmo;
    public float reloadDelayTime;

    public RangedWeaponStat(bool _isInfiniteAmo, int _maxAmmo, float _reloadDelayTime)
    {
        isInfiniteAmmo = _isInfiniteAmo;
        maxAmmo = new Stat(_maxAmmo,"maxAmmo");
        reloadDelayTime = _reloadDelayTime;
    }
}

public struct WeaponStat
{
    private int id;
    private string name;
    private string description;
    private Sprite iconImage;
    private Stat attackSpeed;
    private Stat damage;
    private Stat knockbackStrength;
    private RangedWeaponStat rangedWeaponStat;

    public WeaponStat(int _id, string _name, string _description, Sprite _iconImage, float _attackSpeed, float _damage, float _knockbackStrength, RangedWeaponStat _rangedWeaponstat) { 
        id = _id;
        name = _name;
        description = _description;
        iconImage = _iconImage;
        attackSpeed = new Stat(_attackSpeed,"attackSpeed");
        damage = new Stat(_damage,"damage");
        knockbackStrength = new Stat(_knockbackStrength,"knockbackStrength");
        rangedWeaponStat = _rangedWeaponstat;
    }
}

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField]
    private WeaponType weaponType;

    private string filedata;
    public WeaponStat weaponStat;
    //받은 값 대입
    public void InitWeapon() { 

        //filedata 내용을 토대로 생성자에 대입
        RangedWeaponStat rangedWeaponStat = new RangedWeaponStat(false, 10, 1.0f);
        weaponStat = new WeaponStat(1, "Sword", "A basic sword.", null, 1.0f, 10.0f, 5.0f, rangedWeaponStat);
    }

    public WeaponType WeaponType { get => weaponType; set => weaponType = value; }
}