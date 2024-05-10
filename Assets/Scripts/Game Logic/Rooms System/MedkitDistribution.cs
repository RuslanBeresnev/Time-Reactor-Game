using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Реализация распределения случайного количества аптечек в определённых точках комнаты
/// </summary>
public class MedkitDistribution : MonoBehaviour
{
    [SerializeField] private GameObject medkitPrefab;
    // Точки в комнате, где могут появляться аптечки
    [SerializeField] private List<Transform> medkitPositions = new();
    // Минимальное и максимальное количества аптечек, которые могут появиться в данной комнате
    [SerializeField] private int minCount = 0;
    [SerializeField] private int maxCount = 2;

    private System.Random random = new();

    private void Awake()
    {
        ShuffleMedkitPositions();
    }

    private void Start()
    {
       DistributeMedkits();
    }

    /// <summary>
    /// Перемешать позиции появления аптечек в списке medkitPositions
    /// </summary>
    private void ShuffleMedkitPositions()
    {
        for (int i = medkitPositions.Count - 1; i >= 1; i--)
        {
            int j = random.Next(i + 1);
            var temp = medkitPositions[j];
            medkitPositions[j] = medkitPositions[i];
            medkitPositions[i] = temp;
        }
    }

    /// <summary>
    /// Распределить аптечки в случайных позициях комнаты (среди начально заданных позиций в medkitPositions)
    /// </summary>
    private void DistributeMedkits()
    {
        int resultCount = random.Next(minCount, maxCount + 1);
        for (int i = 0; i < resultCount; i++)
        {
            Instantiate(medkitPrefab, medkitPositions[i].position, medkitPositions[i].rotation);
        }
    }
}