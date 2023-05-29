using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// Набор тестов, проверяющих корректность работы системы стрельбы игрока
/// </summary>
public class ShootingSystemTests
{
    /// <summary>
    /// Подготовить всё необходимое для тестов на выстрелы из оружия
    /// </summary>
    /// <returns>player - объект игрока, laserPistol - компонент "Weapon" у лазерного пистолета, 
    /// poolObjects - список патронов пула</returns>
    private (GameObject player, Weapon laserPistol, GameObject pool, List<GameObject> poolObjects) PrepareSystemForFiring()
    {
        // Создание игрока и получение пистолета у него
        var player = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Assets For Tests/Player"),
            new Vector3(0, 0, 0), Quaternion.identity);
        // Получить компоненты "Weapon" у лазерного пистолета игрока (так как среди дочерних GameObject'ов
        // с компонентом "Weapon" только он активен)
        var laserPistol = player.GetComponentsInChildren<Weapon>(false)[0];

        // Создание пула для патронов пистолета
        var pool = new GameObject();
        pool.transform.name = "Pool";
        var poolComponent = pool.AddComponent<Pool>();

        // Настройка пула (с использованием рефлексии)
        var poolClassType = typeof(Pool);
        FieldInfo bulletPrefabField = poolClassType.GetField("prefab", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo bulletAmountfField = poolClassType.GetField("amount", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo poolObjectsField = poolClassType.GetField("objects", BindingFlags.NonPublic | BindingFlags.Instance);
        var bulletPrefab = Resources.Load<GameObject>("Assets For Tests/Laser Pistol Bullet");
        bulletPrefabField.SetValue(poolComponent, bulletPrefab);
        bulletAmountfField.SetValue(poolComponent, 1);
        var poolObjects = (List<GameObject>)poolObjectsField.GetValue(poolComponent);

        // Настройка компонента "Pool" для пистолета
        var weaponClassType = typeof(Weapon);
        FieldInfo pistolPoolField = weaponClassType.GetField("pool", BindingFlags.NonPublic | BindingFlags.Instance);
        pistolPoolField.SetValue(laserPistol, poolComponent);

        return (player, laserPistol, pool, poolObjects);
    }

    [UnityTest]
    public IEnumerator AfterShotBulletIsCreatedAndFliesWithCorrectSpeed()
    {
        (var player, var laserPistol, var pool, var poolObjects) = PrepareSystemForFiring();

        // Задержка, чтобы перед выстрелом патроны успели создаться в пуле
        yield return new WaitForSeconds(0.1f);
        laserPistol.Shoot();

        yield return null;

        var firedBullet = poolObjects[0];
        // Проверка, что пуля действительно была выпущена (в пуле стала активной)
        Assert.True(firedBullet.activeSelf);
        // Проверка, что скорость выпущенной пули такая, какая должна быть в её характеристиках
        Assert.That((int)firedBullet.GetComponent<Rigidbody>().velocity.magnitude == firedBullet.GetComponent<Bullet>().Velocity);

        GameProperties.GeneralPool.Clear();
        MonoBehaviour.Destroy(player);
        MonoBehaviour.Destroy(pool);
    }

    [UnityTest]
    public IEnumerator ShotMustNotBePerformedIfNoBulletsInMagazine()
    {
        (var player, var laserPistol, var pool, var poolObjects) = PrepareSystemForFiring();

        var weaponClassType = typeof(Weapon);
        FieldInfo bulletsCountInMagazineField = weaponClassType.GetField("bulletsCountInMagazine",
            BindingFlags.NonPublic | BindingFlags.Instance);
        bulletsCountInMagazineField.SetValue(laserPistol, 0);

        // Задержка, чтобы перед выстрелом патроны успели создаться в пуле
        yield return new WaitForSeconds(0.1f);
        laserPistol.Shoot();

        yield return null;

        var bulletToFire = poolObjects[0];
        // Проверка, что пуля не была выпущена
        Assert.False(bulletToFire.activeSelf);
        // Проверка, что кол-во патронов в магазине не стало меньше нуля
        Assert.AreEqual(0, bulletsCountInMagazineField.GetValue(laserPistol));

        GameProperties.GeneralPool.Clear();
        MonoBehaviour.Destroy(player);
        MonoBehaviour.Destroy(pool);
    }

    [UnityTest]
    public IEnumerator WeaponMustNotBeReloadedIfMagazinIsFull()
    {
        // Создание игрока и получение пистолета у него
        var player = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Assets For Tests/Player"),
            new Vector3(0, 0, 0), Quaternion.identity);
        // Получить компоненты "Weapon" у лазерного пистолета игрока (так как среди дочерних GameObject'ов
        // с компонентом "Weapon" только он активен)
        var laserPistol = player.GetComponentsInChildren<Weapon>(false)[0];

        // Получение с помощью рефлексии количества патронов в магазине и в запасе
        var weaponClassType = typeof(Weapon);
        FieldInfo bulletsCountInMagazineField = weaponClassType.GetField("bulletsCountInMagazine", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo bulletsCountInReserveField = weaponClassType.GetField("bulletsCountInReserve", BindingFlags.NonPublic | BindingFlags.Instance);
        var bulletsCountInMagazin = bulletsCountInMagazineField.GetValue(laserPistol);
        var bulletsCountInReserve = bulletsCountInReserveField.GetValue(laserPistol);

        laserPistol.ReloadWeapon();
        yield return null;

        // Проверка на то, что количество патронов не изменилось
        Assert.AreEqual(bulletsCountInMagazin, bulletsCountInMagazineField.GetValue(laserPistol));
        Assert.AreEqual(bulletsCountInReserve, bulletsCountInReserveField.GetValue(laserPistol));

        MonoBehaviour.Destroy(player);
    }

    [UnityTest]
    public IEnumerator ReloadingMustBePerformedIfMagazinIsNotFullAndThereAreBulletsInReserve()
    {
        // Создание игрока и получение пистолета у него
        var player = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Assets For Tests/Player"),
            new Vector3(0, 0, 0), Quaternion.identity);
        // Получить компоненты "Weapon" у лазерного пистолета игрока (так как среди дочерних GameObject'ов
        // с компонентом "Weapon" только он активен)
        var laserPistol = player.GetComponentsInChildren<Weapon>(false)[0];

        // Изменение с помощью рефлексии количества патронов в магазине и в запасе
        var weaponClassType = typeof(Weapon);
        FieldInfo bulletsCountInMagazineField = weaponClassType.GetField("bulletsCountInMagazine", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo bulletsCountInReserveField = weaponClassType.GetField("bulletsCountInReserve", BindingFlags.NonPublic | BindingFlags.Instance);

        bulletsCountInMagazineField.SetValue(laserPistol, 3);
        bulletsCountInReserveField.SetValue(laserPistol, 30);

        laserPistol.ReloadWeapon();
        yield return null;

        FieldInfo magazinCapacityField = weaponClassType.GetField("magazinCapacity", BindingFlags.NonPublic | BindingFlags.Instance);
        var magazinCapacity = magazinCapacityField.GetValue(laserPistol);
        Assert.AreEqual(magazinCapacity, bulletsCountInMagazineField.GetValue(laserPistol));
        Assert.AreEqual(23, bulletsCountInReserveField.GetValue(laserPistol));

        MonoBehaviour.Destroy(player);
    }

    [UnityTest]
    public IEnumerator ReloadingMustNotBePerformedIfBulletsCountInReserveIsZero()
    {
        // Создание игрока и получение пистолета у него
        var player = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Assets For Tests/Player"),
            new Vector3(0, 0, 0), Quaternion.identity);
        // Получить компоненты "Weapon" у лазерного пистолета игрока (так как среди дочерних GameObject'ов
        // с компонентом "Weapon" только он активен)
        var laserPistol = player.GetComponentsInChildren<Weapon>(false)[0];

        // Изменение с помощью рефлексии количества патронов в магазине и в запасе
        var weaponClassType = typeof(Weapon);
        FieldInfo bulletsCountInMagazineField = weaponClassType.GetField("bulletsCountInMagazine", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo bulletsCountInReserveField = weaponClassType.GetField("bulletsCountInReserve", BindingFlags.NonPublic | BindingFlags.Instance);

        bulletsCountInMagazineField.SetValue(laserPistol, 5);
        bulletsCountInReserveField.SetValue(laserPistol, 0);

        laserPistol.ReloadWeapon();
        yield return null;

        Assert.AreEqual(5, bulletsCountInMagazineField.GetValue(laserPistol));
        Assert.AreEqual(0, bulletsCountInReserveField.GetValue(laserPistol));

        MonoBehaviour.Destroy(player);
    }
}