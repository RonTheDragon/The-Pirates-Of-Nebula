using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Map
{
    public string Name;
    public int MaxAmount;
    public Spawnable[] Spawnables;
}
[System.Serializable]
public class Spawnable
{
    public string ObjectName;
    public int ChanceToSpawn = 100;
    public float SpawnFartherAwayBy = 0;
}

public class RandomSpawn : MonoBehaviour
{
    public GameObject Spaceship;
    public Map[] Maps;
    ObjectPooler objectPooler;
    public float MinDist;
    public float MaxDist;
    Currency currency;

    // Start is called before the first frame update
    void Start()
    {
        currency = GetComponent<Currency>();
        objectPooler = ObjectPooler.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length < Maps[currency.Level-1].MaxAmount)
        {
            int ChosenObject = Random.Range(0, Maps[currency.Level-1].Spawnables.Length);
            if (Maps[currency.Level - 1].Spawnables[ChosenObject].ChanceToSpawn > Random.Range(0,100))
            Spawn(Maps[currency.Level - 1].Spawnables[ChosenObject].ObjectName, Maps[currency.Level - 1].Spawnables[ChosenObject].SpawnFartherAwayBy);
        }
    }

    void Spawn(string prefab,float farther)
    {
        
        float x = Random.Range(MinDist+farther, MaxDist+farther + 1);
        if (Random.Range(0,2) == 0) { x *= -1; }
        float z = Random.Range(MinDist+farther, MaxDist+farther + 1);
        if (Random.Range(0, 2) == 0) { z *= -1; }
        float r = Random.Range(0, 361);
        Vector3 pos = new Vector3(Spaceship.transform.position.x+x, Spaceship.transform.position.y, Spaceship.transform.position.z+z);
        GameObject ob = objectPooler.SpawnFromPool(prefab, pos, transform.rotation,true);
        ob.GetComponent<SpawnedMechanic>().SummonedByThePlayer = Spaceship;
        
       // ob.transform.Rotate(0, r, 0);
        
    }
}
