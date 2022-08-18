using UnityEngine;

/// <summary>
/// Реализация полёта пули, урона от неё, взаимодействия с объектами и её уничтожения
/// </summary>
public class Bullet : MonoBehaviour
{
    private Rigidbody rigidBody;

    Vector3 previousPosition;
    // Дальность луча, исходящего в обратную сторону по траектории пули(для небольших скоростей лучше не ставить больше, чем 0.5f)
    private float backRayDistance = 0.5f;
    private float timeToDestruct = 3f;

    public int damage = 1;
    public int velocity = 15;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        previousPosition = transform.position;
        Destroy(gameObject, timeToDestruct);
    }

    void FixedUpdate()
    {
        var hitInfo = CheckCollision();
        if (hitInfo != null)
        {
            Destroy(gameObject);
            PerformCollisionEffects((RaycastHit)hitInfo);
        }
        previousPosition = transform.position;
    }

    /// <summary>
    /// Придать пуле кинетическую энергию
    /// </summary>
    public void GiveBulletKineticEnergy(Vector3 bulletDirection)
    {
        rigidBody.velocity = bulletDirection * velocity;
    }

    /// <summary>
    /// Проверка на столкновение с другими объектами (при большой скорости объектов)
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
    private void PerformCollisionEffects(RaycastHit hitObject)
    {
        var healthSystem = hitObject.collider.gameObject.GetComponent<EntityHealth>();
        if (healthSystem != null)
        {
            healthSystem.TakeDamage(damage);
        }
    }
}