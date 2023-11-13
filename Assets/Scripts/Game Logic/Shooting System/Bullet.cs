using UnityEngine;
using System.Collections;

/// <summary>
/// Пуля
/// </summary>
public class Bullet : Projectile
{
    private void OnEnable()
    {
        StartCoroutine(DestroyBulletAfterLifeTime());
    }

    private IEnumerator DestroyBulletAfterLifeTime()
    {
        yield return StartCoroutine(TimeScale.SharedInstance.WaitForSeconds(Lifetime));
        pool.ReturnObject(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Bullet>() == null)
        {
            pool.ReturnObject(gameObject);
            PerformCollisionEffects(other);
        }
    }

    /// <summary>
    /// Произвести действия над объектом после столкновения пули с ним
    /// </summary>
    protected override void PerformCollisionEffects(Collider hitObjectCollider)
    {
        var entity = hitObjectCollider.gameObject.GetComponent<Entity>();
        if (entity != null)
        {
            entity.TakeDamage(Damage);
        }
    }
}