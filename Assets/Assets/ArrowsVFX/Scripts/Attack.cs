using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public Transform shootPosition;
    public Animator bowAnim;

    public GameObject[] arrows;
    int index = 0;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

              bowAnim.SetTrigger("shoot");

            Invoke("SpawnArrow", 0.15f);
            
        }
    }

    void SpawnArrow()
    {
        if(index< arrows.Length)
        {
            GameObject arrowObject = Instantiate(arrows[index], shootPosition);
            arrowObject.transform.SetParent(null);
        }

        index++;
        if (index == arrows.Length)
            index = 0;
    }
}
