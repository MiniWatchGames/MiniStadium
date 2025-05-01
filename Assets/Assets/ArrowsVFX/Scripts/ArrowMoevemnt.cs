using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ArrowMoevemnt : MonoBehaviour
{
    public float movementSpeed;
    public float lifeTime;
    public VisualEffect VFXGraph;
    public GameObject muzzle;
    public GameObject impact;
    public Transform muzzleTransform;
    void Start()
    {
        VFXGraph.SetFloat("ArrowLife", lifeTime);
        Instantiate(muzzle, muzzleTransform);
        Destroy(gameObject, lifeTime);
    }
    

    // Update is called once per frame
    void Update()
    {
       // if ()
        {
           // transform.localPosition += Vector3.forward * Time.deltaTime * movementSpeed;
            transform.Translate(0, 0, movementSpeed * Time.deltaTime);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {

        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point;
        Instantiate(impact, pos, rot);

        Destroy(gameObject,2f);
    }
}
