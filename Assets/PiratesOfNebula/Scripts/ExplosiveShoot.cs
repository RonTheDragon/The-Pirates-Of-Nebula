using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveShoot : Shoot
{
    public float ExplosionRadius;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (damagecooldown <= 0)
        {
            

            if (CanIDamageIt(collision.gameObject))
            {                                             
                    Health hp = collision.gameObject.GetComponent<Health>();
                    if (hp != null)
                    {
                        Collider[] colliders = Physics.OverlapSphere(transform.position, ExplosionRadius);
                        foreach (Collider c in colliders)
                        {

                            if (CanIDamageIt(c.gameObject))
                            {                               
                                
                                    Health hps = c.gameObject.GetComponent<Health>();
                                    if (hps != null)
                                    {
                                        float dist = Vector3.Distance(transform.position, c.transform.position);
                                        float EffectByRange = 1 - (dist / ExplosionRadius);
                                        hps.Damage(AttackDamage * EffectByRange, Knockback * EffectByRange, gameObject, TempetureEffect * EffectByRange, RadiationEffect * EffectByRange);
                                    }
                                
                            }
                        }
                      
                        if (DestroyOnImpact)
                        {
                            foreach (string s in SpawnOnDeath)
                            {
                                objectPooler.SpawnFromPool(s, transform.position, transform.rotation, transform.localScale);
                            }
                            gameObject.SetActive(false);
                        }
                        damagecooldown = DamageCooldown;
                    }               
            }
        }
    }
}
