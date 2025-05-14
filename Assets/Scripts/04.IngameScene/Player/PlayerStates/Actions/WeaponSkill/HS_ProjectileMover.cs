using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HS_ProjectileMover : MonoBehaviour
{
    [SerializeField] protected float speed = 15f;
    [SerializeField] protected float hitOffset = 0f;
    [SerializeField] protected bool UseFirePointRotation;
    [SerializeField] protected Vector3 rotationOffset = new Vector3(0, 0, 0);
    [SerializeField] protected GameObject hit;
    [SerializeField] protected ParticleSystem hitPS;
    [SerializeField] protected GameObject flash;
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected Collider col;
    [SerializeField] protected Light lightSourse;
    [SerializeField] protected GameObject[] Detached;
    [SerializeField] protected ParticleSystem projectilePS;
    //사운드 클립, 일단 여기에 넣어뒀지만
    //차라리 HS_ProjectileMover를 상속 받는 편이 더 좋을 듯
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip flyingSound;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private bool isSmart;
    private bool startChecker = false;
    protected bool notDestroy = false;
    private Transform ParentsTransform;
    private Vector3 missileDirection;
    private float damage;
    public void SetParentsTransform(Transform _transform) { 
        ParentsTransform = _transform;
    }
    public void SetDamage(float _damage)
    {
        damage = _damage;
    }
    public void SetMissileDirection(Vector3 _direction)
    {
        missileDirection = _direction;
    }
    protected virtual void Start()
    {
        audioSource.volume = 0.4f;
        audioSource.PlayOneShot(shootSound);
        audioSource.volume = 0.05f;
        audioSource.clip = flyingSound;
        audioSource.Play();
        if (!startChecker)
        {
            /*lightSourse = GetComponent<Light>();
            rb = GetComponent<Rigidbody>();
            col = GetComponent<Collider>();
            if (hit != null)
                hitPS = hit.GetComponent<ParticleSystem>();*/
            if (flash != null)
            {
                flash.transform.parent = null;
            }
        }
        if (notDestroy)
            StartCoroutine(DisableTimer(7));
        else
            StartCoroutine(DestroyTimer(7));
        startChecker = true;
    }

    IEnumerator DestroyTimer(float time)
    {
        yield return new WaitForSeconds(time-0.5f);
        audioSource.Stop();
        audioSource.clip = null;
        audioSource.PlayOneShot(explosionSound);
        Destroy(gameObject, 0.5f);
        yield break;
    }
    protected virtual IEnumerator DisableTimer(float time)
    {
        yield return new WaitForSeconds(time);
        if(gameObject.activeSelf)
            gameObject.SetActive(false);
        yield break;
    }

    protected virtual void OnEnable()
    {
        if (startChecker)
        {
            if (flash != null)
            {
                flash.transform.parent = null;
            }
            if (lightSourse != null)
                lightSourse.enabled = true;
            col.enabled = true;
            rb.constraints = RigidbodyConstraints.None;
        }
    }

    protected virtual void FixedUpdate()
    {
        if (speed != 0 && (missileDirection != null))
        {
            rb.velocity = missileDirection * speed;      
        }
    }

    //https ://docs.unity3d.com/ScriptReference/Rigidbody.OnCollisionEnter.html
    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (isSmart)
        {
            if (collision.collider.CompareTag("Obstacle"))
            {
                return;
            }
        }
        audioSource.Stop();
        audioSource.clip = null;
        audioSource.volume = 0.4f;
        audioSource.PlayOneShot(explosionSound);
        //Lock all axes movement and rotation
        rb.constraints = RigidbodyConstraints.FreezeAll;
        //speed = 0;
        if (lightSourse != null)
            lightSourse.enabled = false;
        col.enabled = false;
        if (projectilePS)
        {
            projectilePS.Stop();
            projectilePS.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point + contact.normal * hitOffset;

        //Spawn hit effect on collision
        if (hit != null)
        {
            hit.transform.rotation = rot;
            hit.transform.position = pos;
            if (UseFirePointRotation) { hit.transform.rotation = gameObject.transform.rotation * Quaternion.Euler(0, 180f, 0); }
            else if (rotationOffset != Vector3.zero) { hit.transform.rotation = Quaternion.Euler(rotationOffset); }
            else { hit.transform.LookAt(contact.point + contact.normal); }
            hitPS.Play();
        }

        //Removing trail from the projectile on cillision enter or smooth removing. Detached elements must have "AutoDestroying script"
        foreach (var detachedPrefab in Detached)
        {
            if (detachedPrefab != null)
            {
                ParticleSystem detachedPS = detachedPrefab.GetComponent<ParticleSystem>();
                detachedPS.Stop();
            }
        }
        if (notDestroy)
            StartCoroutine(DisableTimer(hitPS.main.duration));
        else
        {
            if (hitPS != null)
            {
                Destroy(gameObject, hitPS.main.duration);
            }
            else
                Destroy(gameObject, 1);
        }

        var hitColliders = Physics.OverlapSphere(transform.position, 3f, targetLayer);
        if (hitColliders != null) { 
            foreach (var hit in hitColliders)
            {
                ApplyDamage(hit.gameObject, ParentsTransform, 10);
            }
        }
        ApplyDamage(collision.gameObject, ParentsTransform, 20);

    }
    private void ApplyDamage(GameObject target, Transform _hitPoint ,float skillMount)
    {
        IDamageable damageable = target.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            DamageInfo damageInfo = new DamageInfo
            {
                attacker = _hitPoint.gameObject,
                damage = skillMount,
                hitPoint = _hitPoint.position,
                hitDirection = (target.transform.position - _hitPoint.transform.position).normalized
            };

            damageable.TakeDamage(damageInfo);
        }
    }
}