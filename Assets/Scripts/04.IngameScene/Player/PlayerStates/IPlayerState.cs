using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerState 
{
    void Enter(PlayerController playerController);

    void StateUpdate();

    void Exit();
}
