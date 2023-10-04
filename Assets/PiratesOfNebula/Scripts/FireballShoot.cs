using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballShoot : ExplosiveShoot
{
    public float TimerReset = 0.5f;
    float Timer;
    float AfterTime;
    public float SpinSpeed = 360;
    int Direction = 1;
    public GameObject[] trails;

    public override void OnObjectSpawn()
    {
        transform.rotation *= Quaternion.Euler(0, -90, 0);
        StartCoroutine(ChooseWhoToDamage());
        Timer = TimerReset;
        Direction = 1;
        AfterTime = 0;
        foreach (GameObject g in trails)
        {
            g.SetActive(false);
        }
    }


    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        Timer -= Time.deltaTime;
        if (AfterTime > 0.1f) {
            foreach (GameObject g in trails)
            {
                g.SetActive(true);
            }
        }
        else AfterTime += Time.deltaTime;
        
        transform.Rotate(0, SpinSpeed * Direction * Time.deltaTime, 0);
        if (Timer <= 0) { if (Direction==1) { Direction = -1; Timer = TimerReset; } else { Direction = 1; Timer = TimerReset; } }
    }
}
