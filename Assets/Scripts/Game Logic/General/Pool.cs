using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Реализация пула объектов (для оптимизации механик с большим количеством создаваемых объектов)
/// </summary>
public class Pool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int amount;
    private List<GameObject> objects;

    private void Awake()
    {
        objects = new List<GameObject>();
        GameProperties.GeneralPool.Add(gameObject.name, this);
    }

    private void Start()
    {
        for (int i = 0; i < amount; i++)
        {
            var poolObject = Instantiate(prefab);
            poolObject.SetActive(false);
            objects.Add(poolObject);
            poolObject.transform.SetParent(gameObject.transform);
        }
    }

    /// <summary>
    /// Получить один из свободных для использования объектов
    /// </summary>
    public GameObject GetObject()
    {
        for (int i = 0; i < amount; i++)
        {
            if (!objects[i].activeInHierarchy)
            {
                objects[i].SetActive(true);
                return objects[i];
            }
        }

        return null;
    }

    /// <summary>
    /// Вернуть объект в пул
    /// </summary>
    public void ReturnObject(GameObject poolObject)
    {
        poolObject.SetActive(false);
    }
}