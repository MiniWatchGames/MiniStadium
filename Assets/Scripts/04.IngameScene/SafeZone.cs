using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class SafeZone : MonoBehaviour
{
    public Collider magneticFieldCollider;
    public Vector3 DefaultScale = new Vector3(100, 100, 100);
    
    public float magneticFieldDanage;
    // Start is called before the first frame update
    void Start()
    {
        magneticFieldCollider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.localScale -= new Vector3(1f,1f,1f) * Time.deltaTime;
        if (gameObject.transform.localScale == Vector3.zero)
        {
            transform.localScale = DefaultScale;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter" + other.name);
    }
    public void OnTriggerStay(Collider other)
    {
        //Debug.Log("OnTriggerStay" + other.name);
    }
    public void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit" + other.name);
    }
}

