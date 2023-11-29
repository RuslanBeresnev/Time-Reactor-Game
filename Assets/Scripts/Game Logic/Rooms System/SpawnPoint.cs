using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Реализация точки, в которой может появиться враг
/// </summary>
public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemyPrefabs = new();

    /// <summary>
    /// Список префабов врагов, которые могут появиться в данной точке
    /// </summary>
    public List<GameObject> EnemyPrefabs { get => enemyPrefabs; }
}