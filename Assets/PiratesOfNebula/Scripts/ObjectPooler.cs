﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }
    [System.Serializable]
    public class Group
    {
        public string tag;
        public List<Pool> pools;
    }
    #region Singleton
    public static ObjectPooler Instance;

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    public GameObject Player;
    public List<Group> Groups;
    public Dictionary<string, Queue<GameObject>> poolDictionary;


    // Start is called before the first frame update
    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        foreach (Group g in Groups)
        {
            foreach (Pool pool in g.pools)
           {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab,gameObject.transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.tag, objectPool);
           }
        }
        
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation , bool DontSpawnIfActive=false)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't excist.");
            return null;
        }
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        if (DontSpawnIfActive)
        {
            int Count = 0;
            while(objectToSpawn.activeSelf == true && Count<200)
            {
                poolDictionary[tag].Enqueue(objectToSpawn);
                objectToSpawn = poolDictionary[tag].Dequeue();
                Count++;
            }
            if (Count == 200) { poolDictionary[tag].Enqueue(objectToSpawn); return objectToSpawn; }
        }
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        IpooledObject pooledObj = objectToSpawn.GetComponent<IpooledObject>();
        if (pooledObj != null)
        {
            pooledObj.OnObjectSpawn();
        }

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation , Vector3 Scale, bool DontSpawnIfActive = false)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't excist.");
            return null;
        }
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        if (DontSpawnIfActive)
        {
            int Count = 0;
            while (objectToSpawn.activeSelf == true && Count < 200)
            {
                poolDictionary[tag].Enqueue(objectToSpawn);
                objectToSpawn = poolDictionary[tag].Dequeue();
                Count++;               
            }           
        }
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.transform.localScale = Scale;

        IpooledObject pooledObj = objectToSpawn.GetComponent<IpooledObject>();
        if (pooledObj != null)
        {
            pooledObj.OnObjectSpawn();
        }

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
