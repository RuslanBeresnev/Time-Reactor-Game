using UnityEngine;
using System;

/// <summary>
/// Абстракция определённой живой сущности
/// </summary>
public abstract class Entity : ObjectWithInformation, ISerializationCallbackReceiver
{
    [SerializeField] protected float health = 100f;
    [SerializeField] protected float maxHealth = 100f;
    // Коэффициент сложности в сражении с сущностью
    [SerializeField] protected float entityHardcoreCoefficient = 1f;

    private void Start()
    {
        // Синхронизация перед спавном сущности количества здоровья с максимальным количеством здоровья
        Health = MaxHealth;
    }

    /// <summary>
    /// Текущее здоровье сущности
    /// </summary>
    public virtual float Health
    {
        get { return health; }
        protected set
        {
            if (value <= 0f)
            {
                OnDeath();
            }
            else if (value > MaxHealth)
            {
                return;
            }
            health = value;
            if (HealthChanged != null)
            {
                HealthChanged(Health);
            }
        }
    }

    /// <summary>
    /// Максимальное здоровье сущности
    /// </summary>
    public virtual float MaxHealth
    {
        get { return maxHealth; }
        protected set { maxHealth = value; }
    }

    /// <summary>
    /// Событие изменения здоровья сущности
    /// </summary>
    public Action<float> HealthChanged { get; set; }

    public virtual void OnBeforeSerialize()
    {
        health = Health;
        maxHealth = MaxHealth;
    }

    public virtual void OnAfterDeserialize()
    {
        Health = health;
        MaxHealth = maxHealth;
    }

    /// <summary>
    /// Получить урон
    /// </summary>
    public virtual void TakeDamage(float damage)
    {
        Health -= damage;
    }

    /// <summary>
    /// Действия при смерти сущности
    /// </summary>
    public virtual void OnDeath()
    {
        // Core-механика: при победе над сущностями игрок увеличивает максимальный коэффициент способности
        // замедлять время по формуле, зависящей от количества здоровья сущности и её хардкорности
        TimeManagerController.SharedInstance.TimeSlowdownFactor += MaxHealth * entityHardcoreCoefficient * 0.01f;

        Destroy(gameObject);
        Destroy(createdPanel);
    }
}