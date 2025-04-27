using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject playerPrefab;
    private Transform spawnPoint;
    public InGameManager.Team team;
    // Start is called before the first frame update
    void Start()
    {
        spawnPoint = gameObject.transform;
        //playerPrefab = Resources.Load<GameObject>("Player");
    }

    public void SetSpawnObject(GameObject prefab)
    {
        playerPrefab = prefab;
    }
    
    public void Spawn()
    {
        GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
