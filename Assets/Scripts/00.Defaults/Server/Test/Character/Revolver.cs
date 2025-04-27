using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Example.ColliderRollbacks;
using FishNet.Object;
using UnityEngine;

public class Revolver : FireWeapon { }

public abstract class FireWeapon : Weapon
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate;

    private bool _attacking;
    private Coroutine _fireLoop;

    public override void Attack() => StartCoroutine(Fire());

    private IEnumerator Fire()
    {
        if (_attacking)
        {
            yield break;
        }
        _attacking = true;
        Debug.Log("Fire");
        
        var obj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        obj.SetOwner(Owner);
        InstanceFinder.NetworkManager.ServerManager.Spawn(obj.NetworkObject, Owner);
        yield return new WaitForSeconds(fireRate);
        _attacking = false;
    }
}

public abstract class Weapon : NetworkBehaviour
{
    public abstract void Attack();
}
