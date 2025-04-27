using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using UnityEngine.Events;

public class OwnerActive : NetworkBehaviour
{
    [SerializeField] private UnityEvent OnOwner;

    public override void OnStartClient()
    {
        if (IsOwner)
        {
            OnOwner.Invoke();
        }
    }
}
