using UnityEngine;

/// <summary>
/// Реализация точки, в которой с заданной вероятностью появятся боеприпасы
/// (находится в Weapon_Box_1/Weapon_Box_2 в "Added Files To Imported Assets")
/// </summary>
public class AmmoPoint : MonoBehaviour
{
    [SerializeField] private GameObject ammoPrefab;
    [SerializeField] private float probability = 0.75f;

    private static System.Random random = new();

    private void Awake()
    {
        TryToGenerateAmmo();
    }

    /// <summary>
    /// Произвести генерацию боеприпасов в точке, если случайное число на отрезке [0; 1] меньше заданной вероятности
    /// </summary>
    private void TryToGenerateAmmo()
    {
        var randonFloat = (float)random.NextDouble();
        if (randonFloat < probability)
        {
            Instantiate(ammoPrefab, transform.position, transform.rotation);
        }
    }
}