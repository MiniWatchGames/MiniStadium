using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyAttacking : MonoBehaviour
{
    [SerializeField] private PlayerController dummy;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(3f);
        while (true)
        {
            yield return new WaitForSeconds(3f);
            dummy.SetActionStateServer("Attack", dummy);
            yield return new WaitForSeconds(3f);
            dummy.SetActionStateServer("Idle", dummy);
        }
    }

}
