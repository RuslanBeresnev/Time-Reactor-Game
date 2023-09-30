using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Реализация создания случайных врагов в определённых точках комнат
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    // Точки в комнате, где могут появляться враги
    [SerializeField] private List<SpawnPoint> spawnPositions = new();
    // Количество врагов, которые должны появиться в одной комнате при заходе в неё
    [SerializeField] private int enemyCount;

    private System.Random random = new();

    private void Awake()
    {
        ShuffleSpawnPositions();
    }

    /// <summary>
    /// Перемешать позиции спавна врагов в списке spawnPositions
    /// </summary>
    private void ShuffleSpawnPositions()
    {
        for (int i = spawnPositions.Count - 1; i >= 1; i--)
        {
            int j = random.Next(i + 1);
            var temp = spawnPositions[j];
            spawnPositions[j] = spawnPositions[i];
            spawnPositions[i] = temp;
        }
    }

    /// <summary>
    /// Создать случайных врагов в случайных позициях комнаты (среди начально заданных позиций в spawnPositions)
    /// </summary>
    public void InstantiateRandomEnemies()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            // Список префабов врагов, которые могут повиться в данной точке
            var pointPrefabs = spawnPositions[i].EnemyPrefabs;
            var randomEnemyPrefab = pointPrefabs[random.Next(pointPrefabs.Count)];
            Instantiate(randomEnemyPrefab, spawnPositions[i].transform.position, Quaternion.identity);
        }
    }
}