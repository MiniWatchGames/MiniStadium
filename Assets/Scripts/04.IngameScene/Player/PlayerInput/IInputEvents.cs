using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputEvents
{
    void OnMove(Vector2 dir);
    void OnLook(Vector2 delta);
    void OnJumpPressed();
    void OnFirePressed();
    void OnFireReleased();
    void OnCrouchPressed();
}
