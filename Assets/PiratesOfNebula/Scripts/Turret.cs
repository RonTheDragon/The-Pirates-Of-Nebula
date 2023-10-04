using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : SpaceShips
{
    public float Range;
    public GameObject TheShooter;

    public GameObject Target;
    public float RotationSpeed;
    float scanCool;
    public GameObject[] Weapons;

    protected override void Combat()
    {
        if (scanCool > 0) { scanCool -= Time.deltaTime; }
        else
        {
            scanCool = 2;
            if (Target == null)
            {
                Scan(Range);
            }
            else
            {
                if (Vector3.Distance(Target.transform.position, transform.position) > Range) Target = null;
            }
        }

        if (Target != null)
        {
            followTarget();
            fight();
        }


    }

    protected override void Movement()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        CS = new Weapon[Cannons.Length];
        health = gameObject.GetComponent<Health>();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        if (!health.Froze)
        {
            Combat();
        }
    }

    public void Scan(float Range)
    {
        if (Target == null)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, Range);
            foreach (Collider hit in colliders)
            {
                if (hit.transform.tag == "Player") { Target = hit.gameObject; break; }
            }
        }
    }
    void followTarget()
    {
        var targetRotation = Quaternion.LookRotation(Target.transform.position - transform.position);
        TheShooter.transform.rotation = Quaternion.Slerp(TheShooter.transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
    }
    void fight()
    {
        int count = 0;
        foreach (GameObject C in Cannons)
        {
            if (CS[count] == null)
            {
                CS[count] = C.GetComponent<Weapon>();

            }

            RaycastHit hit;
            if (Physics.Raycast(CS[count].GunPoint.transform.position, CS[count].GunPoint.transform.forward, out hit, Mathf.Infinity))
            {
                //Debug.DrawRay(C.transform.position, C.transform.forward * hit.distance, Color.yellow);
                if (hit.transform == Target.transform && OverHeated == false)
                {
                    CS[count].Shoot(gameObject);
                }
            }
            count++;
        }
    }
    public void SwitchWeapons()
    {
        int n = 0;
        foreach (GameObject c in Cannons)
        {
            ChangeWeapon(Weapons[Random.Range(0, Weapons.Length)], n);
            n++;
        }
    }

    public void DamageSecurity()
    {
        transform.parent.parent.parent.GetComponent<Planet>().DamagedSecurity();
    }
}
