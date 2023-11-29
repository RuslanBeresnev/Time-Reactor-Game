using UnityEngine;

public class LaserWeapon : LaserTypeWeapon
{
    public override void Shoot()
    {
        ShotSound.Play();

        RaycastHit hit = GetRaycastHit();
        if (hit.collider == null)
            return;

        MakeLaser(hit);

        var entity = hit.transform.GetComponent<Entity>();
        if (entity != null)
        {
            entity.TakeDamage(LaserDamage);
        }
    }

    private void Awake()
    {
        Type = Type.Laser;

        //Компонент предка-объекта Weapon
        var grandpaWeapon = transform.parent.parent.GetComponent<Weapon>();
        if (Type != grandpaWeapon.Type)
        {
            return;
        }

        RedrawAmmoScreen();


        foreach (var audioSource in GetComponents<AudioSource>())
        {
            var clipName = audioSource.clip.name;
            if (clipName.EndsWith("Shot"))
            {
                ShotSound = audioSource;
            }
            else if (clipName.EndsWith("Reloading"))
            {
                ReloadingSound = audioSource;
            }
            else if (clipName.StartsWith("Weapon Hitting On Surface"))
            {
                WeaponHitingOnSurfaceSounds.Add(audioSource);
            }
            else if (clipName == "Weapon Picking Up")
            {
                PickUpSound = audioSource;
            }
        }

        grandpaWeapon.WeaponHitingOnSurfaceSounds = WeaponHitingOnSurfaceSounds;

        InitializeInfoPanelPrefab();

        ObjectInfoParameters = new string[5, 2] { { "Name:", Name },
                                                  { "Shooting type:", "Automatic" },
                                                  { "Firing Frequency:", "N/A" },
                                                  { "Bullet velocity:", "N/A" },
                                                  { "Damage:", LaserDamage.ToString() } };

        grandpaWeapon.ObjectInfoParameters = ObjectInfoParameters;
    }
}
