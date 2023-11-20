using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserWeapon : Weapon
{
    public override void Shoot()
    {
        ShotSound.Play();

        RaycastHit direction = new RaycastHit();
        if (GetRaycastHit(ref direction))
        {
            FireLaser(direction);
        }
    }

    /// <summary>
    /// Испускать лазер при выстреле
    /// </summary>
    private void FireLaser(RaycastHit hit)
    {
        MakeLaser(hit);

        var entity = hit.transform.GetComponent<Entity>();
        if (entity != null)
        {
            entity.TakeDamage(LaserDamage);
        }
    }
}
