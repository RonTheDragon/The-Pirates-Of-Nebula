using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipsAI : SpaceShips
{
    public float Speed;
    public float RotationSpeed;
    public float DetectionRange;
    public GameObject[] Weapons;
  //  public GameObject[] Cannons;
    float RandomTimer;
    int RandomDirection;
    int CombatStyle;
    float ScanCooldown;
  //  Cannon[] CS;
    public GameObject Target;
    Hookable hookAble;
    Health hp;

    // Start is called before the first frame update
    void Start()
    {
        CS = new Weapon[Cannons.Length];
        hp = gameObject.GetComponent<Health>();
        hookAble = gameObject.GetComponent<Hookable>();       
    }
        // Update is called once per frame
        new void Update()
    {
        base.Update();
        if (!hookAble.hookable)
        {
            if (!health.Froze)
            {
                Movement();
                Combat();
            }
        }
    }
    public void Scan(float Range)
    {
        if (Target == null)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, Range);
            foreach (Collider hit in colliders)
            {
                if (hit.transform.tag == "Player") { Target = hit.gameObject; hookAble.Player = Target; break; }
            }
        }
    }
    void CheckForDodge()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + transform.right * -1.5f, transform.forward, out hit, 20))
        {
            //Debug.DrawRay(transform.position + transform.right * -1.5f, transform.forward * hit.distance, Color.yellow);
            Dodge(hit, 40);
        }
        else
        {
            RaycastHit hit2;
            if (Physics.Raycast(transform.position + transform.right * 1.5f, transform.forward, out hit2, 20))
            {
                // Debug.DrawRay(transform.position + transform.right * 1.5f, transform.forward * hit2.distance, Color.yellow);
                Dodge(hit2, -40);
            }
        }
    }
    void Dodge(RaycastHit hit,float turn)
    {
        if (Target == null || hit.transform != Target.transform)
        {
            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < 20) { transform.Rotate(0, RotationSpeed * turn * Time.deltaTime, 0); }
        }
        else 
        {
            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < 10) { transform.Rotate(0, RotationSpeed * turn * Time.deltaTime, 0); }
        }
    }

    protected override void Movement()
    {
        CheckForDodge();
        float TempetureEffectSpeed = 1;
        if (health.Tempeture < 0) TempetureEffectSpeed += (health.Tempeture * 0.01f);
        if (health.Froze) TempetureEffectSpeed = 0;
        gameObject.transform.position = gameObject.transform.position + gameObject.transform.forward * Speed * TempetureEffectSpeed * Time.deltaTime; //Go Forward

        if (Target == null) //Rotate Randomly
        {
            if (RandomTimer > 0) { RandomTimer -= Time.deltaTime; }
            else
            {
                RandomTimer = Random.Range(1, 3);
                RandomDirection = Random.Range(0, 4);
            }
            if (RandomDirection ==2) { transform.Rotate(0, RotationSpeed * 10 * Time.deltaTime, 0);  }
            else if (RandomDirection == 3) { transform.Rotate(0, -RotationSpeed *10* Time.deltaTime, 0); }
        }
        else //Combat
        {
            if (RandomTimer > 0) { RandomTimer -= Time.deltaTime; }
            else
            {
                int max = 2;
                if (hp.Hp < hp.MaxHps[hp.Phase] / 2) max = 4;
                RandomTimer = Random.Range(1, 5);
                CombatStyle = Random.Range(0, max);
            }
            switch (CombatStyle)
            {
                case 0:
                    followTarget(1);
                    break;
                case 1:
                    followTarget(0.5f);
                    break;
                case 2:
                    RunAway(1);
                    break;
                case 3:
                    RunAway(0.5f);
                    break;
            }

            
        }
        
    }

    void followTarget(float RS)
    {
            var targetRotation = Quaternion.LookRotation(Target.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * RS * Time.deltaTime);
    }
    void RunAway(float RS)
    {
        float opX = transform.position.x - Target.transform.position.x;
        float opZ = transform.position.z - Target.transform.position.z;

        Vector3 OppositeSide = new Vector3()
        { x = transform.position.x + opX, y = transform.position.y, z = transform.position.z+ opZ };


        var targetRotation = Quaternion.LookRotation(OppositeSide - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * RS * Time.deltaTime);
    }

    protected override void Combat()
    {
        if (Target == null) //Scanning For Target
        {
            if (ScanCooldown > 0) { ScanCooldown -= Time.deltaTime; }
            else
            {
                ScanCooldown = 1;
                Scan(DetectionRange);
            }
        }
        else //Attacking Target
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
                    if (hit.transform == Target.transform && OverHeated==false)
                    {
                        CS[count].Shoot(gameObject);       
                    }
                }
                count++;
            }

            float dist = Vector3.Distance(Target.transform.position, transform.position);
            if (dist > DetectionRange * 4) { Target = null; } //Losing Target
        }
    }

    public void SwitchWeapons()
    {
        int n = 0;
        foreach(GameObject c in Cannons)
        {
            ChangeWeapon(Weapons[Random.Range(0, Weapons.Length)], n);
            n++;
        }
    }
   
}
