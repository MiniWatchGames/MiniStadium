using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SingleTon<T> where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<T>();
                if (instance == null)
                {
                    GameObject singleton = new GameObject();
                    instance = singleton.AddComponent<T>();
                    singleton.name = typeof(T).ToString() + " (Singleton)";
                }
            }
            return instance;
        }
    }
    
}

