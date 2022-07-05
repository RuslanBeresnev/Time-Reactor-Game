using UnityEngine;

/// <summary>
/// Создание врага с определённым шансом
/// </summary>
public class EnemyAppearance : MonoBehaviour
{
    public GameObject enemy;
    private Vector3 enemyOffsetRelativeToTrigger = new Vector3(-2f, -0.6f, 3.5f);

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            if (PlayerController.ZAxisDirection == PlayerController.AxisDirection.Forward)
            {
                var random = new System.Random();
                float generatedFloat = (float)random.NextDouble();
                if (generatedFloat < GameProperties.AppearanceChance)
                {
                    var enemyPosition = transform.position + enemyOffsetRelativeToTrigger;
                    Instantiate(enemy, enemyPosition, Quaternion.identity);
                }
            }
        }
    }
}