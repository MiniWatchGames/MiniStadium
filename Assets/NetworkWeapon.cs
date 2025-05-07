using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class NetworkWeapon : NetworkBehaviour
{
    public WeaponType weaponType;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!Owner.IsValid) return;

        var players = FindObjectsOfType<PlayerWeapon>();
        foreach (var pw in players)
        {
            if (pw.Owner == Owner && pw.TryGetWeaponData(weaponType, out WeaponData weaponData))
            {
                transform.SetParent(weaponData.weaponSocket);
                transform.localPosition = weaponData.positionOffset;
                transform.localRotation = Quaternion.Euler(weaponData.rotationOffset);
                break;
            }
        }
    }
}