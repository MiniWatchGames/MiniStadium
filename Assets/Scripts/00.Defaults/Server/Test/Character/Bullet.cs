using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 2f;
    [SerializeField] private float damage = 10f;
    
    [SerializeField] private Rigidbody rb;
    [SerializeField] private NetworkConnection owner;

    private void Reset() => rb = GetComponent<Rigidbody>();

    private void OnTriggerEnter(Collider other)
    {
        if (IsServerInitialized)
        {
            return;
        }

        if (other.gameObject.TryGetComponent<NetworkObject>(out var netwporkObject))
        {
            if (NetworkObject.Owner == owner)
            {
                return;
            }
        }

        foreach (var component in other.gameObject.GetComponents<Component>())
        {
            Debug.Log(component.name);
        }
        
        if (other.gameObject.TryGetComponent<Health>(out var health))
        {
            health.ChangeHp(-damage);
        }
    }
    public void SetOwner(NetworkConnection owner) => this.owner = owner;

    public override void OnStartNetwork()
    {
        rb.velocity = transform.forward * speed;

        StartCoroutine(Destroy());
    }

    private IEnumerator Destroy()
    {
        yield return new WaitForSeconds(lifeTime);
        Despawn();
    }
}
