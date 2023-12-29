using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// ����� ������, ����������� ������������ ������ ������� �������� ������
/// </summary>
public class ShootingSystemTests
{
    /// <summary>
    /// ����������� �� ����������� ��� ������ �� �������� �� ������
    /// </summary>
    /// <returns>player - ������ ������, weapon - ��������� "Weapon", 
    /// poolObjects - ������ �������� ����</returns>
    private (GameObject player, Weapon weapon, GameObject pool, List<GameObject> poolObjects) PrepareProjectile()
    {
        // �������� ������ � ��������� ��������� � ����
        var player = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Assets For Tests/PlayerProjectile"),
            new Vector3(0, 0, 0), Quaternion.identity);
        // �������� ���������� "Weapon" � ������ (��� ��� ����� �������� GameObject'��
        // � ����������� "Weapon" ������ �� �������)
        var weapon = player.GetComponentsInChildren<Weapon>(false)[0];

        // �������� ���� ��� �������� ������
        var pool = new GameObject();
        pool.transform.name = "Pool";
        var poolComponent = pool.AddComponent<Pool>();

        // ��������� ���� (� �������������� ���������)
        var poolClassType = typeof(Pool);
        FieldInfo bulletPrefabField = poolClassType.GetField("prefab", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo bulletAmountfField = poolClassType.GetField("amount", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo poolObjectsField = poolClassType.GetField("objects", BindingFlags.NonPublic | BindingFlags.Instance);
        var bulletPrefab = Resources.Load<GameObject>("Assets For Tests/Laser Pistol Bullet");
        bulletPrefabField.SetValue(poolComponent, bulletPrefab);
        bulletAmountfField.SetValue(poolComponent, 1);
        var poolObjects = (List<GameObject>)poolObjectsField.GetValue(poolComponent);

        // ��������� ���������� "Pool" ��� ������
        var pistol = weapon.gameObject.GetComponentInChildren<ProjectileWeapon>();
        var weaponClassType = typeof(ProjectileWeapon);
        FieldInfo pistolPoolField = weaponClassType.GetField("pool", BindingFlags.NonPublic | BindingFlags.Instance);
        pistolPoolField.SetValue(pistol, poolComponent);

        return (player, weapon, pool, poolObjects);
    }

    [UnityTest]
    public IEnumerator AfterShotBulletIsCreatedAndFliesWithCorrectSpeed()
    {
        (var player, var weapon, var pool, var poolObjects) = PrepareProjectile();

        var projectileWeapon = weapon.GetComponentInChildren<ProjectileWeapon>();
        
        // ��������, ����� ����� ��������� ������� ������ ��������� � ����
        yield return new WaitForSeconds(0.1f);
        projectileWeapon.Shoot();

        yield return null;

        var firedBullet = poolObjects[0];
        // ��������, ��� ���� ������������� ���� �������� (� ���� ����� ��������)
        Assert.True(firedBullet.activeSelf);
        // ��������, ��� �������� ���������� ���� �����, ����� ������ ���� � � ���������������
        Assert.That((int)firedBullet.GetComponent<Rigidbody>().velocity.magnitude == firedBullet.GetComponent<Bullet>().Velocity);

        GameProperties.GeneralPool.Clear();
        MonoBehaviour.Destroy(player);
        MonoBehaviour.Destroy(pool);
    }

    [UnityTest]
    public IEnumerator ShotMustNotBePerformedIfNoBulletsInMagazine()
    {
        (var player, var weapon, var pool, var poolObjects) = PrepareProjectile();

        var projectileWeapon = weapon.GetComponentInChildren<ProjectileWeapon>();

        var weaponClassType = typeof(Weapon);
        FieldInfo bulletsCountInMagazineField = weaponClassType.GetField("bulletsCountInMagazine",
            BindingFlags.NonPublic | BindingFlags.Instance);
        bulletsCountInMagazineField.SetValue(projectileWeapon, 0);

        // ��������, ����� ����� ��������� ������� ������ ��������� � ����
        yield return new WaitForSeconds(0.1f);
        projectileWeapon.Shoot();

        yield return null;

        var bulletToFire = poolObjects[0];
        // ��������, ��� ���� �� ���� ��������
        Assert.False(bulletToFire.activeSelf);
        // ��������, ��� ���-�� �������� � �������� �� ����� ������ ����
        Assert.AreEqual(0, bulletsCountInMagazineField.GetValue(weapon));

        GameProperties.GeneralPool.Clear();
        MonoBehaviour.Destroy(player);
        MonoBehaviour.Destroy(pool);
    }

    [UnityTest]
    public IEnumerator WeaponMustNotBeReloadedIfMagazinIsFull()
    {
        // �������� ������ � ��������� ��������� � ����
        var player = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Assets For Tests/PlayerProjectile"),
            new Vector3(0, 0, 0), Quaternion.identity);
        // �������� ���������� "Weapon" � ��������� ��������� ������ (��� ��� ����� �������� GameObject'��
        // � ����������� "Weapon" ������ �� �������)
        var projectileWeapon = player.GetComponentsInChildren<ProjectileWeapon>(false)[0];

        // ��������� � ������� ��������� ���������� �������� � �������� � � ������
        var weaponClassType = typeof(Weapon);
        FieldInfo bulletsCountInMagazineField = weaponClassType.GetField("bulletsCountInMagazine", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo bulletsCountInReserveField = weaponClassType.GetField("bulletsCountInReserve", BindingFlags.NonPublic | BindingFlags.Instance);
        var bulletsCountInMagazin = bulletsCountInMagazineField.GetValue(projectileWeapon);
        var bulletsCountInReserve = bulletsCountInReserveField.GetValue(projectileWeapon);

        projectileWeapon.ReloadWeapon();
        yield return null;

        // �������� �� ��, ��� ���������� �������� �� ����������
        Assert.AreEqual(bulletsCountInMagazin, bulletsCountInMagazineField.GetValue(projectileWeapon));
        Assert.AreEqual(bulletsCountInReserve, bulletsCountInReserveField.GetValue(projectileWeapon));

        MonoBehaviour.Destroy(player);
    }

    [UnityTest]
    public IEnumerator ReloadingMustBePerformedIfMagazinIsNotFullAndThereAreBulletsInReserve()
    {
        // �������� ������ � ��������� ��������� � ����
        var player = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Assets For Tests/Player"),
            new Vector3(0, 0, 0), Quaternion.identity);
        // �������� ���������� "Weapon" � ��������� ��������� ������ (��� ��� ����� �������� GameObject'��
        // � ����������� "Weapon" ������ �� �������)
        var projectileWeapon = player.GetComponentsInChildren<ProjectileWeapon>(false)[0];

        // ��������� � ������� ��������� ���������� �������� � �������� � � ������
        var weaponClassType = typeof(Weapon);
        FieldInfo bulletsCountInMagazineField = weaponClassType.GetField("bulletsCountInMagazine", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo bulletsCountInReserveField = weaponClassType.GetField("bulletsCountInReserve", BindingFlags.NonPublic | BindingFlags.Instance);

        int bulletsInMagazineBeginning = 3;
        int bulletsInReserveBeginning = 30;
        bulletsCountInMagazineField.SetValue(projectileWeapon, bulletsInMagazineBeginning);
        bulletsCountInReserveField.SetValue(projectileWeapon, bulletsInReserveBeginning);

        projectileWeapon.ReloadWeapon();
        yield return null;

        FieldInfo magazinCapacityField = weaponClassType.GetField("magazinCapacity", BindingFlags.NonPublic | BindingFlags.Instance);
        int magazinCapacity = (int)magazinCapacityField.GetValue(projectileWeapon);
        int bulletsInMagazine = (int)bulletsCountInMagazineField.GetValue(projectileWeapon);
        int bulletsInReserve = (int)bulletsCountInReserveField.GetValue(projectileWeapon);
        Assert.AreEqual(magazinCapacity, bulletsInMagazine);
        Assert.AreEqual(bulletsInReserveBeginning - magazinCapacity + bulletsInMagazineBeginning, bulletsInReserve);

        MonoBehaviour.Destroy(player);
    }

    [UnityTest]
    public IEnumerator ReloadingMustNotBePerformedIfBulletsCountInReserveIsZero()
    {
        // �������� ������ � ��������� ��������� � ����
        var player = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Assets For Tests/Player"),
            new Vector3(0, 0, 0), Quaternion.identity);
        // �������� ���������� "Weapon" � ��������� ��������� ������ (��� ��� ����� �������� GameObject'��
        // � ����������� "Weapon" ������ �� �������)
        var projectileWeapon = player.GetComponentsInChildren<ProjectileWeapon>(false)[0];

        // ��������� � ������� ��������� ���������� �������� � �������� � � ������
        var weaponClassType = typeof(Weapon);
        FieldInfo bulletsCountInMagazineField = weaponClassType.GetField("bulletsCountInMagazine", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo bulletsCountInReserveField = weaponClassType.GetField("bulletsCountInReserve", BindingFlags.NonPublic | BindingFlags.Instance);

        int bulletsInMagazineBeginning = 5;
        int bulletsInReserveBeginning = 0;
        bulletsCountInMagazineField.SetValue(projectileWeapon, bulletsInMagazineBeginning);
        bulletsCountInReserveField.SetValue(projectileWeapon, bulletsInReserveBeginning);

        projectileWeapon.ReloadWeapon();
        yield return null;

        Assert.AreEqual(bulletsInMagazineBeginning, bulletsCountInMagazineField.GetValue(projectileWeapon));
        Assert.AreEqual(bulletsInReserveBeginning, bulletsCountInReserveField.GetValue(projectileWeapon));

        MonoBehaviour.Destroy(player);
    }
}   