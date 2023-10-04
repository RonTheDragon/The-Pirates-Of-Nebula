using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackhole : Star
{
    Collider[] hitColliders;
    public float OverlapSphereCooldown;
    float _cooldown;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        {
            transform.Rotate(0, 200 * Time.deltaTime, 0);

            if (_cooldown <= 0)
            {
                _cooldown = OverlapSphereCooldown;
                hitColliders = Physics.OverlapSphere(transform.position, 100);
            }
            else { _cooldown -= Time.deltaTime; }

            foreach (Collider c in hitColliders)
            {
                GameObject Victim = c.gameObject;
                Health hp = Victim.GetComponent<Health>();
                if (hp != null)
                {
                    hp.Radiation -= 2 * Time.deltaTime;


                    float dist = Vector3.Distance(Victim.transform.position, transform.position);

                    Suck(Victim, dist, 300);

                    if (dist < 1)
                    {
                        hp.Hp = 0;
                        hp.Shield = 0;
                    }
                }
            }


        }
        void Suck(GameObject Victim, float dist, float SuckPower)
        {
            Victim.transform.position = Vector3.MoveTowards(Victim.transform.position, transform.position, (SuckPower * Time.deltaTime) / dist);
        }

    }
}
