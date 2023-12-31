﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedMechanic : MonoBehaviour, IpooledObject
{
    public float DespawnWhenAtDistant = 100;
    public GameObject SummonedByThePlayer;
    public GameObject DistantBody;
    public Health health;
    public Hookable hookable;
    public SpaceshipsAI spaceshipsAI;
    public Obstacle obstacle;
    public Star star;
    public Turret turret;

    public void OnObjectSpawn() //When Spawned
    {
        if (DistantBody != null)
        {
            DistantBody.transform.position = new Vector3(transform.position.x, DistantBody.transform.position.y, transform.position.z);
        }
        //Debug.Log("works");
        if (health != null)
        {
            health.Radiation = 0;
            health.Phase = 0;
            health.RemoveElements();
            health.Hp = health.MaxHps[health.Phase];
            health.Shield = health.MaxShields[health.Phase];
            
            //Debug.Log(health.gameObject.name + " " +health.Hp);
            foreach (GameObject g in health.TurnOffWhenDeath)
            {
                g.SetActive(true);
            }
        }
        if (hookable != null)
        {
            hookable.Money = Random.Range(hookable.MinStartMoney, hookable.MaxStartMoney+1);
            hookable.hookable = false;
            hookable.Buttons[0].SetActive(false);
            hookable.Buttons[1].SetActive(false);
        }
        if (spaceshipsAI != null)
        {
            spaceshipsAI.SwitchWeapons();
        }      
        if (obstacle != null)
        {
            obstacle.Speed = Random.Range(0, obstacle.MovementSpeed);
            obstacle.transform.rotation = Quaternion.Euler(new Vector3() { x = transform.rotation.x, z = transform.rotation.z, y = Random.Range(0, 360) });
            obstacle.Rotation = new Vector3()
            {
                x = Random.Range(-obstacle.RotationSpeed, obstacle.RotationSpeed),
                y = Random.Range(-obstacle.RotationSpeed, obstacle.RotationSpeed),
                z = Random.Range(-obstacle.RotationSpeed, obstacle.RotationSpeed), };
            float s = Random.Range(obstacle.MinScale, obstacle.MaxScale);
            obstacle.transform.localScale = new Vector3() { x = s, y = s, z = s };
        }
        
        if (star != null)
        {
            star.SpawnStar();
        }
        

        if (turret != null)
        {
            turret.SwitchWeapons();
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        if (DistantBody != null && SummonedByThePlayer != null )
        {
            float dist = Vector3.Distance(DistantBody.transform.position, SummonedByThePlayer.transform.position);
            if (dist > DespawnWhenAtDistant)
            {
                gameObject.SetActive(false);

                if (health != null)
                {
                    health.RemoveElements();
                }
            }
        }
    }

}
