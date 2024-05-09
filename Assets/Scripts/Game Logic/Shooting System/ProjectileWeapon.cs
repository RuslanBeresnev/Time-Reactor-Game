using UnityEngine;

/// <summary>
/// Оружие, стреляющее снарядами типа Projectile
/// </summary>
public class ProjectileWeapon : Weapon
{
    /// <summary>
    /// Префаб снаряда
    /// </summary>
    [field: HideInInspector][field: SerializeField] public GameObject ProjectilePrefab { get; set; }

    /// <summary>
    /// Пул снарядов
    /// </summary>
    [field: HideInInspector][field: SerializeField] public Pool Pool { get; set; }

    protected override void RedrawAmmoScreen()
    {
        AmmoScreen.text = BulletsCountInMagazine.ToString() + " / " + BulletsCountInReserve.ToString();;
    }

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

    /// <summary>
    /// Создать снаряд при выстреле
    /// </summary>
    protected void FireProjectile(Vector3 bulletDirection)
    {
        var projectile = Pool.GetObject();
        if (projectile != null)
        {
            var rotation = Quaternion.FromToRotation(ProjectilePrefab.transform.forward, bulletDirection);
            projectile.transform.position = WeaponEnd.position;
            projectile.transform.rotation = rotation;
        }

        var projectileComponent = projectile.GetComponent<Projectile>();
        projectileComponent.GiveKineticEnergy(bulletDirection);
    }

    private void Awake()
    {
        Type = Type.Projectile;

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

        if (BulletsCountInMagazine > MagazinCapacity)
        {
            BulletsCountInMagazine = MagazinCapacity;
        }

        var projectileComponent = ProjectilePrefab.GetComponent<Projectile>();
        if (projectileComponent == null)
        {
            return;
        }

        string damage = projectileComponent.Damage.ToString();
        string velocity = projectileComponent.Velocity.ToString();

        ObjectInfoParameters = new string[5, 2] { { "Name:", Name },
                                                  { "Shooting type:", SemiAutoShooting ? "Semi-Automatic" : "Automatic" },
                                                  { "Firing Frequency:", System.Math.Round(1 / IntervalBetweenShoots).ToString() + " per sec." },
                                                  { "Bullet velocity:", velocity  + " m/s" },
                                                  { "Damage:", damage + " HP" } };

        grandpaWeapon.ObjectInfoParameters = ObjectInfoParameters;
    }
}
