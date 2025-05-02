using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon 
{
   public ObservableFloat Damage { get;}

   public ObservableFloat CurrentAmmo { get; }
   public ObservableFloat MaxAmmo { get;}
}
