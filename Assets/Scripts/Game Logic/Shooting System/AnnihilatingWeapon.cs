using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnnihilatingWeapon : LaserTypeWeapon
{
    /// <summary>
    /// Тэг, означающий, что может быть уничтожено аннигилирующим оружием
    /// </summary>
    [field: HideInInspector][field: SerializeField] public string AnnihilatingTag { get; set; }

    public override void Shoot()
    {
        //ShotSound.Play();

        RaycastHit hit = GetRaycastHit();
        if (hit.collider == null)
            return;

        MakeLaser(hit);

        var target = hit.transform.gameObject;
        if (target.CompareTag(AnnihilatingTag))
        {
            Destroy(target.gameObject);
        }
    }

    private void Awake()
    {
        Type = Type.Annihilating;

        var weapon = transform.parent.parent.GetComponent<Weapon>();
        if (Type != weapon.Type)
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

        InitializeInfoPanelPrefab();

        ObjectInfoParameters = new string[5, 2] { { "Name:", Name },
                                                  { "Shooting type:", "Automatic" },
                                                  { "Firing Frequency:", "N/A" },
                                                  { "Bullet velocity:", "N/A" },
                                                  { "Damage:", "Full HP" } };
    }
}
