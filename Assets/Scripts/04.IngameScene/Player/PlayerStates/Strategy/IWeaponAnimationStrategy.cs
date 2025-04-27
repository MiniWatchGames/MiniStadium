using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeaponAnimationStrategy
{
    string GetAnimationName(string baseAniKey);
}
