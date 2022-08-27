using UnityEngine;

/// <summary>
/// Абстракция определённой живой сущности
/// </summary>
public abstract class Entity : MonoBehaviour
{
    /// <summary>
    /// Здоровье сущности
    /// </summary>
    public abstract float Health { get; protected set; }

    /// <summary>
    /// Получить урон
    /// </summary>
    public virtual void TakeDamage(float damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            OnDeath();
        }
    }

    /// <summary>
    /// Действия при смерти сущности
    /// </summary>
    public virtual void OnDeath()
    {
        Destroy(gameObject);
    }
}