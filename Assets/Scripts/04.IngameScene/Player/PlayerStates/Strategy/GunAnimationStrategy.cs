using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAnimationStrategy : IWeaponAnimationStrategy
{    public string GetAnimationName(string baseKey) => $"Gun_{baseKey}";
}
