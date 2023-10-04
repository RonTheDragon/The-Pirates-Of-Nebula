using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : Star
{

    Collider[] hitColliders;
    public float OverlapSphereCooldown = 1;
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

        if (_cooldown <= 0)
        {
            _cooldown = OverlapSphereCooldown;
            hitColliders = Physics.OverlapSphere(transform.position, size * 1.5f);
        }
        else { _cooldown -= Time.deltaTime; }

       
        foreach (Collider c in hitColliders)
        {
            GameObject Victim = c.gameObject;
            Health health = Victim.GetComponent<Health>();
            if (health != null)
            {
                if (health.Hp > 0)
                {
                    float dist = Vector3.Distance(Victim.transform.position, transform.position);
                    if (dist < size / 2) { health.Radiation = 100; }
                    else
                    {
                        health.Tempeture += (size * 1.5f - dist) * 5 * Time.deltaTime;
                        health.Radiation += (size * 1.5f - dist) * 0.1f * Time.deltaTime;
                    }

                    
                }
            }
        }
    }

    public override void SpawnStar()
    {
        
        Despawn = 1;       
        ChangeSize();
        
    }

    public void ChangeSize()
    {
        if (planetModel != null)
        {
            size = Random.Range(MinSize, MaxSize);
            planetModel.transform.localScale = new Vector3(size, size*0.75f, size);
        }
    }

}
