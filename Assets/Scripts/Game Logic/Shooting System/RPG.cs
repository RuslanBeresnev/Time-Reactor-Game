using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPG : ProjectileTypeWeapon
{
    protected override void Awake()
    {
        base.Awake();

        //РПГ не может вести автоматическую стрельбу
        SemiAutoShooting = true;
    }
}
