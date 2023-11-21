using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnnihilatingWeapon : LaserTypeWeapon
{
    /// <summary>
    /// Тэг, означающий, что может быть уничтожено аннигилирующим оружием
    /// </summary>
    public string AnnihilatingTag { get; set; }

    protected override void Fire(RaycastHit hit)
    {
        base.Fire(hit);

        var target = hit.transform.gameObject;
        if (target.CompareTag(AnnihilatingTag))
        {
            Destroy(target.gameObject);
        }
    }

    
}
