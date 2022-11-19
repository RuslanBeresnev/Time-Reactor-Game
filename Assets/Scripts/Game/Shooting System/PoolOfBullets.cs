using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Реализация пула патронов (для оптимизации системы стрельбы)
/// </summary>
public class PoolOfBullets : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int amount;
    // Хранилище в сцене, куда при создании все патроны данного типа будут помещены как дочерние
    [SerializeField] private GameObject bulletsStorage;
    private List<GameObject> bullets;

    private void Awake()
    {
        bullets = new List<GameObject>();

        for (int i = 0; i < amount; i++)
        {
            var bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bullets.Add(bullet);
            bullet.GetComponent<Bullet>().Pool = this;
            bullet.transform.SetParent(bulletsStorage.transform);
        }
    }

    /// <summary>
    /// Получить один из свободных для использования патронов
    /// </summary>
    public GameObject GetBullet()
    {
        for (int i = 0; i < amount; i++)
        {
            if (!bullets[i].activeInHierarchy)
            {
                return bullets[i];
            }
        }

        return null;
    }

    /// <summary>
    /// Вернуть патрон в пул патронов
    /// </summary>
    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
    }
}