using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Managing.Server;
using FishNet.Object;
using Unity.VisualScripting;
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

public class PlayerWeapon : NetworkBehaviour
{
    [SerializeField]private WeaponType weaponType;
    public WeaponType WeaponType { get => weaponType; set => weaponType = value; }
    
    [SerializeField] private List<WeaponData> weaponDataList = new List<WeaponData>();
    private Dictionary<WeaponType, WeaponData> weaponDataDict = new Dictionary<WeaponType, WeaponData>();
    
    private GameObject currentWeapon; // 현재 장착된 무기
    public GameObject CurrentWeapon => currentWeapon;
    
    private void Awake()
    {
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
        {
            GetComponent<PlayerWeapon>().enabled = false;
        }
        InitWeaponDictionary();
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        InitWeaponDictionary();

    }

    private void InitWeaponDictionary()
    {
        weaponDataDict.Clear();

        weaponDataDict[WeaponType.Sword] = weaponDataList[0];
        weaponDataDict[WeaponType.Gun] = weaponDataList[1];
        //weaponDataDict[WeaponType.Bow] = weaponDataList[2];
    }

    public bool TryGetWeaponData(WeaponType weaponType, out WeaponData data)
    {
        bool result = weaponDataDict.TryGetValue(weaponType, out WeaponData weaponData);
        data = weaponData;
        return result;
    }
    
    public void CreateWeapon(WeaponType weapon, PlayerWeapon playerWeapon)
    {
        // 기존 무기 제거
        if (currentWeapon != null)
        {
            Debug.Log("Despawn currentWeapon");
        }

        if (currentWeapon== null && weaponDataDict.TryGetValue(weaponType, out WeaponData weaponData))
        {
            SpawnWeapon(weapon);
        }
    }
    [ServerRpc]
    public void SpawnWeapon(WeaponType weapon)
    {
        if (!weaponDataDict.TryGetValue(weaponType, out WeaponData weaponData)) return;
        GameObject spawned = Instantiate(weaponData.weaponPrefab);
        NetworkObject spawnedNetworkObject = spawned.GetComponent<NetworkObject>();
        base.Spawn(spawned, Owner);
        SetSpawnedObject(spawnedNetworkObject);
    }

    [ObserversRpc]
    public void SetSpawnedObject(NetworkObject spawnedObject)
    {
        GameObject weapon = spawnedObject.gameObject;
        CombatManager combatManager = gameObject.GetComponent<CombatManager>();
        currentWeapon = weapon;
        combatManager.CurrentWeapon = CurrentWeapon;
    }

}