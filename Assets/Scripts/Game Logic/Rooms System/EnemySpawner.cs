using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// –еализаци€ создани€ случайных врагов в определЄнных точках комнат
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    // “очки в комнате, где могут по€вл€тьс€ враги
    [SerializeField] private List<SpawnPoint> spawnPositions = new();
    //  оличество врагов, которые должны по€витьс€ в одной комнате при заходе в неЄ
    [SerializeField] private int enemyCount;

    private System.Random random = new();

    private void Awake()
    {
        ShuffleSpawnPositions();
    }

    /// <summary>
    /// ѕеремешать позиции спавна врагов в списке spawnPositions
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
    /// —оздать случайных врагов в случайных позици€х комнаты (среди начально заданных позиций в spawnPositions)
    /// </summary>
    public void InstantiateRandomEnemies()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            // —писок префабов врагов, которые могут повитьс€ в данной точке
            var pointPrefabs = spawnPositions[i].EnemyPrefabs;
            var randomEnemyPrefab = pointPrefabs[random.Next(pointPrefabs.Count)];
            Instantiate(randomEnemyPrefab, spawnPositions[i].transform.position, Quaternion.identity);
        }
    }
}