using UnityEngine;

/// <summary>
/// Реализация полёта пули, урона от неё, взаимодействия с объектами и её уничтожения
/// </summary>
public class Bullet : MonoBehaviour, ISerializationCallbackReceiver
{
    private Rigidbody rigidBody;
    Vector3 previousPosition;

    [SerializeField] private int damage = 1;
    [SerializeField] private int velocity = 15;
    // Дальность луча, исходящего в обратную сторону по траектории пули(для небольших скоростей лучше не ставить больше, чем 0.5f)
    [SerializeField] private float backRayDistance = 0.5f;

    /// <summary>
    /// Пул патронов, которому принадлежит данный патрон
    /// </summary>
    public PoolOfBullets Pool { get; set; }

    /// <summary>
    /// Количество получаемого сущностью урона
    /// </summary>
    public int Damage { get; private set; }

    /// <summary>
    /// Скорость полёта пули
    /// </summary>
    public int Velocity { get; private set; }

    public void OnBeforeSerialize()
    {
        velocity = Velocity;
        damage = Damage;
    }

    public void OnAfterDeserialize()
    {
        Velocity = velocity;
        Damage = damage;
    }

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        previousPosition = transform.position;
    }

    void FixedUpdate()
    {
        var hitInfo = CheckCollision();
        if (hitInfo != null)
        {
            Pool.ReturnBullet(gameObject);
            PerformCollisionEffects(((RaycastHit)hitInfo).collider);
        }
        previousPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        Pool.ReturnBullet(gameObject);
        PerformCollisionEffects(other);
    }

    /// <summary>
    /// Придать пуле кинетическую энергию
    /// </summary>
    public void GiveBulletKineticEnergy(Vector3 bulletDirection)
    {
        rigidBody.velocity = bulletDirection * velocity;
    }

    /// <summary>
    /// Проверка на столкновение с другими объектами (для объектов с большой скоростью)
    /// </summary>
    private RaycastHit? CheckCollision()
    {
        Vector3 currentTrajectory = (transform.position - previousPosition) / Vector3.Distance(transform.position, previousPosition);
        var backRay = new Ray(transform.position, -currentTrajectory);
        RaycastHit hit;
        int defaultLayerMask = 1;

        if (Physics.Raycast(backRay, out hit, backRayDistance, defaultLayerMask, QueryTriggerInteraction.Ignore))
        {
            return hit;
        }
        return null;
    }

    /// <summary>
    /// Произвести действия над объектом после столкновения пули с ним
    /// </summary>
    private void PerformCollisionEffects(Collider hitObjectCollider)
    {
        var entity = hitObjectCollider.gameObject.GetComponent<Entity>();
        if (entity != null)
        {
            entity.TakeDamage(damage);
        }
    }
}