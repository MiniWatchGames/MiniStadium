using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon 
{
   public Stat Damage { get;}

   public ObservableFloat CurrentAmmo { get; }
   public ObservableFloat MaxAmmo { get;}
}
