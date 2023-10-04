using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCaptainControl : PlayerControl
{
    // Start is called before the first frame update
    void Start()
    {
        CS = new Weapon[Cannons.Length];
        ShootingSide = new bool[Cannons.Length];
        for (int i = 0; i < Cannons.Length; i++)
        {
            ChangeWeapon(Cannons[i], i);
        }
    }

    // Update is called once per frame
    new void Update()
    {
        
        base.Update();
        Movement();
        Combat();
        ComputerControlsSupport();


    }
}
