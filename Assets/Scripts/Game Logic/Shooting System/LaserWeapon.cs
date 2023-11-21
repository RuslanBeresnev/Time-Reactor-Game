using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserWeapon : LaserTypeWeapon
{
    protected override void Fire(RaycastHit hit)
    {
        base.Fire(hit);

        var entity = hit.transform.GetComponent<Entity>();
        if (entity != null)
        {
            entity.TakeDamage(LaserDamage);
        }
    }
}
