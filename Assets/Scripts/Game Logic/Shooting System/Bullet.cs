using UnityEngine;
using System.Collections;

/// <summary>
/// Пуля
/// </summary>
public class Bullet : Projectile
{
    private void OnEnable()
    {
        StartCoroutine(DestroyAfterLifeTime());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Bullet>() == null)
        {
            pool.ReturnObject(gameObject);
            PerformCollisionEffects(other);
        }
    }

    protected override void PerformCollisionEffects(Collider hitObjectCollider)
    {
        var entityComponent = UsefulFeatures.GetFirstEntityComponentInObjectHierarchy(hitObjectCollider.transform);
        if (entityComponent != null)
        {
            entityComponent.TakeDamage(Damage);
        }
    }
}