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

    void OnFirstWeaponSkillPressed();
    void OnFirstWeaponSkillReleased();

    void OnSecondWeaponSkillPressed();
    void OnSecondWeaponSkillReleased();

    void OnFirstMoveSkillPressed();
    void OnFirstMoveSkillReleased();

    void OnSecondMoveSkillPressed();
    void OnSecondMoveSkillReleased();
}
