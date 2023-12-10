using System.Linq;
using UnityEngine;

public class AnnihilatingWeapon : LaserTypeWeapon
{
    [SerializeField, HideInInspector] private string[] annihilatingTags;
    [SerializeField, HideInInspector] private GameObject annihilationFX;

    /// <summary>
    /// Тэги, означающие, что может быть уничтожено аннигилирующим оружием
    /// </summary>
    public string[] AnnihilatingTags 
    { 
        get => annihilatingTags; 
        set => annihilatingTags = value; 
    }

    /// <summary>
    /// Визуальный эффект при аннигиляции
    /// </summary>
    public GameObject AnnihilationFX
    {
        get => annihilationFX;
        set => annihilationFX = value;
    }

    public override void Shoot()
    {
        RaycastHit hit = GetRaycastHit();
        if (hit.collider == null)
            return;

        MakeLaser(hit);

        var target = hit.transform.gameObject;
        if (AnnihilatingTags.Contains(target.tag.ToString()))
        {
            if (annihilationFX != null)
            {
                var annihilFXGO = Instantiate(annihilationFX, hit.point, Quaternion.identity);
                Destroy(annihilFXGO, 2f);
            }
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
            if (clipName == "Laser Tower Attack")
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
