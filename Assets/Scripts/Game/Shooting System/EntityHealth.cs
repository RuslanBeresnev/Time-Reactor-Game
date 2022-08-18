using UnityEngine;

/// <summary>
/// Система здоровья для какой-либо сущности
/// </summary>
public class EntityHealth : MonoBehaviour
{
    public int health;

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}