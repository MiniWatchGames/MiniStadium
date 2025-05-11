using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private GameObject Prefab;
    private int poolSize;
    private Transform parentsTransform;

    private Queue<GameObject> _pool;

    public void MakePool(GameObject _prefab, int _poolSize, Transform _parentsTransform)
    {
        SetPoolData(_prefab, _poolSize, _parentsTransform);

        _pool = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject();
        }
    }

    private void SetPoolData(GameObject _prefab, int _poolSize, Transform _parentsTransform) {
        Prefab = _prefab;
        poolSize = _poolSize;
        parentsTransform = _parentsTransform;
    }

    private void CreateNewObject()
    {
        GameObject newObject = Instantiate(Prefab);
        //처음 생성될 때 position을 유지하지 않고 부모에 들어가기 위해 뒤의 인자값을 false로 둠
        newObject.transform.SetParent(parentsTransform, false);
        newObject.SetActive(false);
        _pool.Enqueue(newObject);
    }

    public GameObject GetObject()
    {
        if (_pool.Count == 0) { CreateNewObject(); }
        GameObject dequeObject = _pool.Dequeue();
        dequeObject.SetActive(true);
        return dequeObject;
    }

    public void ReturnObject(GameObject returnObject)
    {
        returnObject.SetActive(false);
        _pool.Enqueue(returnObject);
    }
}
