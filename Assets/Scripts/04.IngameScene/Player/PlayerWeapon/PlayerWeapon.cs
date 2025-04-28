using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Sword,
    Gun,
    Bow
}

[Serializable]
public class WeaponData
{
    public GameObject weaponPrefab;
    public Transform weaponSocket;
    public Vector3 positionOffset;
    public Vector3 rotationOffset;
}

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private WeaponType weaponType;
    public WeaponType WeaponType { get => weaponType; set => weaponType = value; }
    
    [SerializeField] private List<WeaponData> weaponDataList = new List<WeaponData>();
    private Dictionary<WeaponType, WeaponData> weaponDataDict = new Dictionary<WeaponType, WeaponData>();
    
    private GameObject currentWeapon; // 현재 장착된 무기
    public GameObject CurrentWeapon => currentWeapon;
    
    private void Awake()
    {
        InitWeaponDictionary();
    }

    private void InitWeaponDictionary()
    {
        weaponDataDict.Clear();

        weaponDataDict[WeaponType.Sword] = weaponDataList[0];
        weaponDataDict[WeaponType.Gun] = weaponDataList[1];
        //weaponDataDict[WeaponType.Bow] = weaponDataList[2];
    }

    public void CreateWeapon(WeaponType weapon)
    {
        // 기존 무기 제거
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
        }

        if (weaponDataDict.TryGetValue(weapon, out WeaponData weaponData))
        {
            currentWeapon = Instantiate(weaponData.weaponPrefab, weaponData.weaponSocket);
            currentWeapon.transform.localPosition = weaponData.positionOffset;
            currentWeapon.transform.localRotation = Quaternion.Euler(weaponData.rotationOffset);
        }
    }
}