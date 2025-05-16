using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public struct entityInField
{
    public GameObject entity;
    public bool isInField;
    public Action outoffieldAction;
    public float timer;
} 

public class SafeZone : MonoBehaviour, IStatObserver
{
    public Collider magneticFieldCollider;
    public Vector3 DefaultScale = new Vector3(100, 100, 100);
    private float defaultShrinkSpeed = 1f;
    private float defaultMagneticFieldDamage = 1f;
    public InGameManager.GameState currentState;
    
    public float magneticShrinkSpeed = 3f;
    public float magneticFieldDamage = 1f;
    
    private Dictionary<GameObject, entityInField> objectsInField = new Dictionary<GameObject, entityInField>();
    private Dictionary<GameObject, Coroutine> activeCoroutines = new Dictionary<GameObject, Coroutine>();
    
    [SerializeField] private Volume purpleVolume; // 보라빛 효과 볼륨
   
    
    //public HashSet<GameObject> objectsInField = new HashSet<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        magneticFieldCollider = GetComponent<Collider>();
        if (purpleVolume != null)
            purpleVolume.weight = 0;
    }

    // Update is called once per frame
    

    public void UpdateMagneticField(float time)
    {
        if(gameObject.transform.localScale.x <= 0)
        {
            return;
        }
        
        gameObject.transform.localScale -= new Vector3(1f,0f,1f) * (magneticShrinkSpeed * Time.deltaTime);
        

        
    }
    public void Reset()
    {
        transform.localScale = DefaultScale;
        magneticShrinkSpeed = defaultShrinkSpeed;
        magneticFieldDamage = defaultMagneticFieldDamage;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (purpleVolume != null)
            purpleVolume.weight = 0;
        Debug.Log("OnTriggerEnter" + other.name);
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            
            if (objectsInField.ContainsKey(other.gameObject))
            {
                var entityInField = objectsInField[other.gameObject];
                entityInField.isInField = true;
                objectsInField[other.gameObject] = entityInField;
                if (activeCoroutines.ContainsKey(other.gameObject))
                {
                    StopCoroutine(activeCoroutines[other.gameObject]);
                    //activeCoroutines.Remove(other.gameObject);
                }
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
                        ObservableFloat CurrentHp = other.gameObject.GetComponent<PlayerController>().CurrentHp;
                        CurrentHp.Value -= other.gameObject.GetComponent<PlayerController>().BaseMaxHp.Value * 0.1f;
                        
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
        if (purpleVolume != null)
            purpleVolume.weight = 1;
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            var entityInField = objectsInField[other.gameObject];
            entityInField.isInField = false;
            objectsInField[other.gameObject] = entityInField;
            if (activeCoroutines.ContainsKey(other.gameObject))
            {
                StopCoroutine(activeCoroutines[other.gameObject]);
            }
            activeCoroutines[other.gameObject] = StartCoroutine(CountTimer(objectsInField[other.gameObject]));
            
            //other.gameObject.GetComponent<TestStat>().ChangedHp -= magneticFieldDamage;
            //Debug.Log("OnTriggerEnter" + other.name);
        }
        if (other.CompareTag("PlayerCamera"))
        {
            if (purpleVolume != null)
                purpleVolume.weight = 1;
        }
    }

    public void WhenStatChanged((float, string) data)
    {
        
    }
}

