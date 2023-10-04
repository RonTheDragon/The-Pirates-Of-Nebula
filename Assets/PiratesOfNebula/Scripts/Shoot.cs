using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : Damager , IpooledObject
{
    public float Speed;
    public bool DestroyOnImpact;
    public bool DeclaredByShooter;
    public bool CantHitSelf;
    public GameObject ShootBy;
    public string[] SpawnOnDeath;
    protected ObjectPooler objectPooler;

    protected string HitableTag;
    protected string HitableTag2;

    // Start is called before the Active

    public void Start()
    {
        ChooseWhoToDamage();
        objectPooler = ObjectPooler.Instance;
        
    }

    // Update is called once per frame
    public void Update()
    {
        transform.position += transform.forward * Speed * Time.deltaTime;
        transform.position = new Vector3() { x = transform.position.x, y = 0, z = transform.position.z };
        if (damagecooldown > 0) { damagecooldown -= Time.deltaTime; }
    }
    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (damagecooldown <= 0)
        {          
            if (CanIDamageIt(collision.gameObject))
            {               
                    Health hp = collision.gameObject.GetComponent<Health>();
                    if (hp != null)
                    {
                        hp.Damage(AttackDamage, Knockback, gameObject,TempetureEffect,RadiationEffect);
                        if (DestroyOnImpact) {
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
    protected IEnumerator ChooseWhoToDamage()
    {
        yield return null;
        HitableTag = null;
        HitableTag2 = null;

        if (DeclaredByShooter)
        {
            if (ShootBy.gameObject.tag == "Player")
            {
                CantHitEnemy = false;
                CantHitPlayer = true;
            }
            else if (ShootBy.gameObject.tag == "Enemy")
            {
                CantHitEnemy = true;
                CantHitPlayer = false;
            }
        }

        if (!CantHitPlayer)
        {
            HitableTag = "Player";
        }
        if (!CantHitEnemy)
        {
            HitableTag2 = "Enemy";

        }
    }
    protected bool CanIDamageIt(GameObject g)
    {
        if (g.tag == HitableTag || g.tag == HitableTag2)
        {
            if (CantHitSelf == false || g != ShootBy)
            {
                return true;
            }
        }
        return false;
    }

    public virtual void OnObjectSpawn()
    {
        StartCoroutine(ChooseWhoToDamage());    
    }
}
