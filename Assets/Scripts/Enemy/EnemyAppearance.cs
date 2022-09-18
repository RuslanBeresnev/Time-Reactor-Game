using UnityEngine;

/// <summary>
/// Создание врага с определённым шансом
/// </summary>
public class EnemyAppearance : MonoBehaviour
{
    [SerializeField] private GameObject enemy;
    [SerializeField] private Transform enemySpawner;
    [SerializeField] private PlayerController playerControllerScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            if (playerControllerScript.PlayerVelocity.z > 0f && !GameProperties.PassedFloors.Contains(GameProperties.FloorNumber))
            {
                GameProperties.PassedFloors.Add(GameProperties.FloorNumber);

                var random = new System.Random();
                float generatedFloat = (float)random.NextDouble();
                if (generatedFloat < GameProperties.EnemyAppearanceChance)
                {
                    Instantiate(enemy, enemySpawner.position, Quaternion.identity);
                }
            }
        }
    }
}