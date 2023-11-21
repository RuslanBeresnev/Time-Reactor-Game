using UnityEngine;

public class ProjectileTypeWeapon : Weapon
{
    /// <summary>
    /// Префаб снаряда
    /// </summary>
    public GameObject ProjectilePrefab { get; set; }

    /// <summary>
    /// Пул снарядов
    /// </summary>
    public Pool Pool { get; set; }

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
        var projectile = pool.GetObject();
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

        string damage = projectilePrefab.GetComponent<Projectile>().Damage.ToString();
        string velocity = projectilePrefab.GetComponent<Projectile>().Velocity.ToString();

        ObjectInfoParameters = new string[5, 2] { { "Name:", Name },
                                                  { "Shooting type:", SemiAutoShooting ? "Semi-Automatic" : "Automatic" },
                                                  { "Firing Frequency:", System.Math.Round(1 / IntervalBetweenShoots).ToString() + " per sec." },
                                                  { "Bullet velocity:", velocity  + " m/s" },
                                                  { "Damage:", damage + " HP" } };

        if (BulletsCountInMagazine > MagazinCapacity)
        {
            BulletsCountInMagazine = MagazinCapacity;
        }
    }
}
