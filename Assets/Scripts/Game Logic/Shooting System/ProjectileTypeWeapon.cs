using UnityEngine;

public class ProjectileTypeWeapon : Weapon
{
    /// <summary>
    /// Префаб снаряда
    /// </summary>
    [field: SerializeField] public GameObject ProjectilePrefab { get; set; }

    /// <summary>
    /// Пул снарядов
    /// </summary>
    [field: SerializeField] public Pool Pool { get; set; }

    public override void OnBeforeSerialize()
    {
        // Создавать GO - так себе идея
        if (ProjectilePrefab == null)
        {
            ProjectilePrefab = new GameObject();
        }
    }

    protected override void RedrawAmmoScreen()
    {
        //Придётся снова для всех создавать AmmoScreen в инспекторе
        //AmmoScreen.text = BulletsCountInMagazine.ToString() + " / " + BulletsCountInReserve.ToString();;
    }

    public override void Shoot()
    {
        if (BulletsCountInMagazine == 0)
        {
            return;
        }
        BulletsCountInMagazine--;

        //Надо брать shotsound из родителя -- объекта Weapon
        //ShotSound.Play();

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

    protected override void Awake()
    {
        base.Awake();

        if (BulletsCountInMagazine > MagazinCapacity)
        {
            BulletsCountInMagazine = MagazinCapacity;
        }

        var projectileComponent = ProjectilePrefab.GetComponent<Projectile>();
        if (projectileComponent == null)
            return;

        string damage = projectileComponent.Damage.ToString();
        string velocity = projectileComponent.Velocity.ToString();

        ObjectInfoParameters = new string[5, 2] { { "Name:", Name },
                                                  { "Shooting type:", SemiAutoShooting ? "Semi-Automatic" : "Automatic" },
                                                  { "Firing Frequency:", System.Math.Round(1 / IntervalBetweenShoots).ToString() + " per sec." },
                                                  { "Bullet velocity:", velocity  + " m/s" },
                                                  { "Damage:", damage + " HP" } };
    }
}
