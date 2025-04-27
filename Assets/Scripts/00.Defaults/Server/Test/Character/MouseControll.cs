using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class MouseControll : NetworkBehaviour
{
    [SerializeField] private bool disable;

    public override void OnStartClient()
    {
        if (!IsOwner && disable)
        {
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public override void OnStopClient()
    {
        if (!IsOwner || disable)
        {
            return;
        }
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
