using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPG : Weapon
{
    public override void Shoot()
    {
        if (BulletsCountInMagazine == 0)
        {
            return;
        }
        BulletsCountInMagazine--;

        ShotSound.Play();

        Vector3 direction = GetShootingDirection();
        FireProjectile(direction);
    }
}
