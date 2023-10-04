using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : Weapon
{
    ObjectPooler objectPooler;
    public string PrefabShot;

    // Start is called before the first frame update
    void Start()
    {
        objectPooler = ObjectPooler.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (cooldown > 0) { cooldown -= Time.deltaTime; }
    }

    public override void Shooting(GameObject Owner)
    {
        objectPooler.SpawnFromPool(PrefabShot, GunPoint.transform.position, GunPoint.transform.rotation).GetComponent<Shoot>().ShootBy = Owner;
    }
}
