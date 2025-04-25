using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStatPublisher 
{
   public void AddObserver(IStatObserver observer);

    public void RemoveObserver(IStatObserver observer);

    public void NotifyObservers((float,string) value);
}
