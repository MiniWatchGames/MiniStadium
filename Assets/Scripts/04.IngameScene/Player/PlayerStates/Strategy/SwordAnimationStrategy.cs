using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAnimationStrategy : IWeaponAnimationStrategy
{
    public string GetAnimationName(string baseKey) => $"Sword_{baseKey}";
}
