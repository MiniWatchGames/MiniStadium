using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObservableFloat : IStatPublisher
{

    private float result;
    private List<IStatObserver> observers;
    private string name;
    private float passResult;
    public float Value
    {
        get {
            return result;
        }
        set
        {
            result = value;
            if (result != passResult)
            {
                NotifyObservers(new(result,name));
                passResult = result;
            }
        }
    }

    /// <summary>
    /// 생성자
    /// </summary>
    public ObservableFloat(float result, string floatName)
    {
        this.result = result;
        observers = new List<IStatObserver>();
        this.name = floatName;
    }

    public void AddObserver(IStatObserver observer)
    {
        if (observers == null)
        {
            observers = new List<IStatObserver>();
        }
        if (observers.Contains(observer)) return;
        observers.Add(observer);
    }

    public void RemoveAllObservers()
    {
        observers.Clear();
    }

    public void RemoveObserver(IStatObserver observer)
    {
        observers.Remove(observer);
    }

    public void NotifyObservers((float,string) value)
    {
        if (observers == null) return;
        foreach (IStatObserver observer in observers)
        {
            observer.WhenStatChanged(value);
        }
    }
}