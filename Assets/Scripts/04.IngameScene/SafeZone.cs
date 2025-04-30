using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public struct entityInField
{
    public GameObject entity;
    public bool isInField;
    public Action outoffieldAction;
    public float timer;
} 
public class SafeZone : MonoBehaviour
{
    public Collider magneticFieldCollider;
    public Vector3 DefaultScale = new Vector3(100, 100, 100);
    public float magneticShrinkSpeed = 2f;
    private Dictionary<GameObject, entityInField> objectsInField = new Dictionary<GameObject, entityInField>();
    public float magneticFieldDamage = 1f;
    public Action onMagneticFieldDamage;
    private Task _task;
    //public HashSet<GameObject> objectsInField = new HashSet<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        magneticFieldCollider = GetComponent<Collider>();
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.localScale -= new Vector3(1f,1f,1f) * (magneticShrinkSpeed * Time.deltaTime);
        if (gameObject.transform.localScale == Vector3.zero)
        {
            transform.localScale = DefaultScale;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter" + other.name);
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            if (objectsInField.ContainsKey(other.gameObject))
            {
                var entityInField = objectsInField[other.gameObject];
                entityInField.isInField = true;
                objectsInField[other.gameObject] = entityInField;
                StopCoroutine(CountTimer(entityInField));
                return;
            }
            else
            {
                objectsInField.Add(other.gameObject, new entityInField()
                {
                    entity = other.gameObject,
                    isInField = true,
                    outoffieldAction = () =>
                    {
                        //other.gameObject.GetComponent<PlayerController>().CurrentHp -= magneticFieldDamage;
                    },
                    timer = 3f
                });
                
            }
        }
    }
    
    IEnumerator CountTimer(entityInField entity)
    {   
        
        yield return new WaitForSeconds(entity.timer);
        while (!entity.isInField)
        {
            entity.outoffieldAction?.Invoke();
            Debug.Log("Out of field" + entity.entity.name + " " + magneticFieldDamage);
            yield return new WaitForSeconds(1f);
        }
    }
    
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            var entityInField = objectsInField[other.gameObject];
            entityInField.isInField = false;
            objectsInField[other.gameObject] = entityInField;
            StartCoroutine(CountTimer(entityInField));
            
            //other.gameObject.GetComponent<TestStat>().ChangedHp -= magneticFieldDamage;
            //Debug.Log("OnTriggerEnter" + other.name);
        }
    }
}

