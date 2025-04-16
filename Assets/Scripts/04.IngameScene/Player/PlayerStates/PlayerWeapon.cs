using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Sword,
    Gun,
}
public class PlayerWeapon : MonoBehaviour
{
    [SerializeField]
    private WeaponType weaponType;

    public WeaponType WeaponType { get => weaponType; set => weaponType = value; }
}