using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Абстрактный класс снарядов, реализующий полёт и обработку коллизий
/// </summary>
public abstract class Projectile : MonoBehaviour, ISerializationCallbackReceiver
{
    protected Rigidbody rigidBody;
    protected Pool pool;
    protected Vector3 previousPosition;

    [SerializeField] private int damage = 1;
    [SerializeField] private int velocity = 15;
    [SerializeField] private float lifetime = 3f;
    // Дальность луча, исходящего в обратную сторону по траектории пули(для небольших скоростей лучше не ставить больше, чем 0.5f)
    [SerializeField] protected float backRayDistance = 0.5f;
    // Название пула объектов, в котором хранятся экземпляры данного снаряда
    [SerializeField] private string poolName;

    /// <summary>
    /// Количество получаемого сущностью урона
    /// </summary>
    public int Damage { get; private set; }

    /// <summary>
    /// Скорость полёта снаряда
    /// </summary>
    public int Velocity { get; private set; }

    /// <summary>
    /// Время жизни снаряда
    /// </summary>
    public float Lifetime { get; private set; }

    public void OnBeforeSerialize()
    {
        velocity = Velocity;
        damage = Damage;
        lifetime = Lifetime;
    }

    public void OnAfterDeserialize()
    {
        Velocity = velocity;
        Damage = damage;
        Lifetime = lifetime;
    }

    public void GiveKineticEnergy(Vector3 direction)
    {
        rigidBody.velocity = direction * velocity * TimeScale.SharedInstance.Scale;

        Debug.Log("Giving:" + rigidBody.velocity + " == " + rigidBody.velocity.magnitude + " " +
            direction + " " + velocity + " " + TimeScale.SharedInstance.Scale);
    }

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        previousPosition = transform.position;
        pool = GameProperties.GeneralPool[poolName];

    }

    private void FixedUpdate()
    {
        var hitInfo = CheckCollision();
        if (hitInfo != null)
        {
            // Снаряд не уничтожается при столкновении со снарядом
            if (hitInfo.Value.collider.gameObject.GetComponent<Projectile>() == null)
            {
                pool.ReturnObject(gameObject);
                PerformCollisionEffects(((RaycastHit)hitInfo).collider);
            }
        }
        previousPosition = transform.position;
    }

    protected RaycastHit? CheckCollision()
    {
        Vector3 currentTrajectory = (transform.position - previousPosition).normalized;
        var backRay = new Ray(transform.position, -currentTrajectory);
        RaycastHit hit;
        int defaultLayerMask = 1;

        if (Physics.Raycast(backRay, out hit, backRayDistance, defaultLayerMask, QueryTriggerInteraction.Ignore))
        {
            return hit;
        }
        return null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Projectile>() == null)
        {
            pool.ReturnObject(gameObject);
            PerformCollisionEffects(collision.collider);
        }
    }

    /// <summary>
    /// Уничтожение снаряда по истечении времени жизни
    /// </summary>
    protected virtual IEnumerator DestroyAfterLifeTime()
    {
        yield return StartCoroutine(TimeScale.SharedInstance.WaitForSeconds(Lifetime));
        pool.ReturnObject(gameObject);
    }

    /// <summary>
    /// Произвести действия над объектом после столкновения снаряда с ним
    /// </summary>
    protected abstract void PerformCollisionEffects(Collider hitObjectCollider);
}
