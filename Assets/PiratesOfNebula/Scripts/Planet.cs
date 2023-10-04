using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : Star
{
    
    ObjectPooler objectPooler;

    public int MaxTurrentsAmount = 3;
    public int MaxBuildingsAmount = 3;
    public float DistantBetweenBuildings = 1;

    public string[] Turrets;
    public string[] Buildings;

    [HideInInspector]
    public int Security;

    public List<GameObject> turrets = new List<GameObject>();
    public List<GameObject> buildings = new List<GameObject>();



    // Start is called before the first frame update
    new void Start()
    {

        objectPooler = ObjectPooler.Instance;
        base.Start();

    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    public override void SpawnStar()
    {
        objectPooler = ObjectPooler.Instance;
        Despawn = 1;
        ChangeColor();
        ChangeSize();
        ChangeBuildings();
    }

    public void ChangeColor()
    {
        if (planetModel != null)
        {
            planetModel.GetComponent<Renderer>().material.color = new Color(Random.value, Random.value, Random.value);
        }
    }
    public void ChangeSize()
    {
        if (planetModel != null)
        {
            size = Random.Range(MinSize,MaxSize);
            planetModel.transform.localScale = new Vector3(size,planetModel.transform.localScale.y,size);
        }
    }
    public void ChangeBuildings()
    {
        foreach (GameObject g in turrets) //Remove All Turrents
        {
            g.gameObject.SetActive(false);
            g.transform.SetParent(objectPooler.gameObject.transform);
        }
        turrets.Clear();

        foreach (GameObject g in buildings) //Remove All Buildings
        {
            g.gameObject.SetActive(false);
            g.transform.SetParent(objectPooler.gameObject.transform);
        }      
        buildings.Clear();


        int R = Random.Range(1, MaxTurrentsAmount+1);
        Security = R;
        for (int i = 0; i < R; i++)
        {
            int n = Random.Range(0, Turrets.Length);
            GameObject g = Build(Turrets[n]);
            turrets.Add(g);         
        }

        R = Random.Range(1, MaxBuildingsAmount + 1);
        for (int i = 0; i < R; i++)
        {
            int n = Random.Range(0, Buildings.Length);
            GameObject g = Build(Buildings[n]);
            buildings.Add(g);
        }


    }

    GameObject Build(string Name)
    {
            GameObject T = objectPooler.SpawnFromPool(Name, transform.position, transform.rotation,true);
            if (T != null)
            {
                float Rx = Random.Range(-size / Mathf.PI, size / Mathf.PI);
                float Rz = Random.Range(-size / Mathf.PI, size / Mathf.PI);


            for (int i = 0; i < 30; i++)
            {
                if (i == 29) { Debug.Log("Buildings may have Merged"); }
                if (!CheckIfCanBuild(Rx, Rz))
                {
                    Rx = Random.Range(-size / Mathf.PI+(i/2), size / Mathf.PI+i);
                    Rz = Random.Range(-size / Mathf.PI+(i/2), size / Mathf.PI+i);
                }
                else break;
            }

                T.transform.position = new Vector3(T.transform.position.x + Rx, T.transform.position.y, T.transform.position.z + Rz); 
                T.transform.SetParent(transform.GetChild(1));
            return T;
            } 
        Debug.LogWarning("Not a Valid Building");
        return null;
    }

    bool CheckIfCanBuild(float x, float z)
    {
        Vector2 V2 = new Vector2(x, z);
        foreach(GameObject g in turrets)
        {
            if (Vector2.Distance(V2, GetPosOnPlanet(g.transform.position)) < DistantBetweenBuildings) return false;
        }
        foreach (GameObject g in buildings)
        {
            if (Vector2.Distance(V2, GetPosOnPlanet(g.transform.position)) < DistantBetweenBuildings) return false;
        }

        return true;
    }
    Vector2 GetPosOnPlanet(Vector3 pos)
    {
        return new Vector2() { x = pos.x - transform.position.x, y = pos.z - transform.position.z };
    }

    

    public void DamagedSecurity()
    {
        Security--;
        if (Security <= 0)
        {
            foreach(GameObject g in buildings)
            {
                g.transform.GetChild(0).GetComponent<Health>().Hp = -10;
            }
        }
    }
}
