using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirearmWeapon : ProjectileTypeWeapon
{
    public override bool ReloadingCanBePerformed()
    {
        return BulletsCountInMagazine != MagazinCapacity && BulletsCountInReserve != 0;

    }

    public override void ReloadWeapon()
    {
        if (!ReloadingCanBePerformed())
        {
            return;
        }

        var bulletsCountToFillMagazine = MagazinCapacity - BulletsCountInMagazine;
        if (BulletsCountInReserve < bulletsCountToFillMagazine)
        {
            BulletsCountInMagazine += BulletsCountInReserve;
            BulletsCountInReserve = 0;
        }
        else
        {
            BulletsCountInReserve -= bulletsCountToFillMagazine;
            BulletsCountInMagazine = MagazinCapacity;
        }
    }
}
