﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damager : MonoBehaviour
{
    public float AttackDamage;
    public float Knockback;
    public float TempetureEffect;
    public float RadiationEffect;

    public bool CantHitPlayer;
    public bool CantHitEnemy;

    public float DamageCooldown;
    protected float damagecooldown;
    
}
