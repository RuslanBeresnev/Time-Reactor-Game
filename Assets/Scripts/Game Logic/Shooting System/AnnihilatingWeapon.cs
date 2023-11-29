using System.Linq;
using UnityEngine;

public class AnnihilatingWeapon : LaserTypeWeapon
{
    [SerializeField] private string[] annihilatingTags;

    /// <summary>
    /// Тэги, означающие, что может быть уничтожено аннигилирующим оружием
    /// </summary>
    public string[] AnnihilatingTags 
    { 
        get => annihilatingTags; 
        set => annihilatingTags = value; 
    }

    public override void Shoot()
    {
        ShotSound.Play();

        RaycastHit hit = GetRaycastHit();
        if (hit.collider == null)
            return;

        MakeLaser(hit);

        var target = hit.transform.gameObject;
        if (AnnihilatingTags.Contains(target.tag.ToString()))
        {
            Destroy(target.gameObject);
        }
    }

    private void Awake()
    {
        Type = Type.Annihilating;

        //Компонент предка-объекта 
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
                                                  { "Damage:", "Full HP" } };

        grandpaWeapon.ObjectInfoParameters = ObjectInfoParameters;
    }
}
