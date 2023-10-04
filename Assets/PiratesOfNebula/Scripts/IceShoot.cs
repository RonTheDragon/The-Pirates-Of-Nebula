using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceShoot : ExplosiveShoot
{
    public float TimerReset = 0.5f;
    public float SpinSpeed = 360;
    float NowSpeed;
    float HalfSpeed;
    float NowSpin;
    float HalfSpin;
    float Timer;
    int Direction = 1;
    bool FirstSpin = true;
    public GameObject[] trails;

    
    public override void OnObjectSpawn()
    {
        StartCoroutine(ChooseWhoToDamage());
        FirstSpin = true;
        Timer = TimerReset * 0.5f;
        Direction = Random.Range(0, 2);
        if (Direction == 0) Direction = -1;
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        foreach(GameObject g in trails)
        {
            g.SetActive(false);
        }
    }

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        NowSpeed = Speed;
        HalfSpeed = Speed / 4;
       // NowSpin = SpinSpeed;
       // HalfSpin = SpinSpeed* 4;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        Timer -= Time.deltaTime;
        if (TempetureEffect > -300)
        {
            TempetureEffect -= Time.deltaTime * 50;
        }
        transform.Rotate(0, SpinSpeed * Time.deltaTime * Direction, 0);
        if (FirstSpin)
        {
            Speed = NowSpeed;
           // SpinSpeed = NowSpin;
        }
        else
        {
            Speed = HalfSpeed;
           // SpinSpeed = HalfSpin;
        }
        if (Timer <= 0 ) { if (FirstSpin) { FirstSpin = false; Timer = TimerReset; } else { FirstSpin = true; Timer = TimerReset; } } 
        
        if (transform.localScale.x < 1.5f) { transform.localScale += new Vector3(0.4f, 0.4f, 0.4f) * Time.deltaTime; }

        if (transform.localScale.x > 0.2f)
        {
            if (trails[0].activeSelf == false)
            {
                foreach (GameObject g in trails)
                {
                    g.SetActive(true);
                }
            }
        }
    }
}
