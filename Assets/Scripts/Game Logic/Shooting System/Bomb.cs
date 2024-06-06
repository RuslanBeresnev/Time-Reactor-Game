using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Бомба
/// </summary>
public class Bomb : Projectile
{
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionRange;
    [SerializeField] private float explosionForce;

    private void OnEnable()
    {
        StartCoroutine(DestroyAfterLifeTime());
    }

    protected override void PerformCollisionEffects(Collider hitObjectCollider)
    {
        Explode();
    }

    /// <summary>
    /// Взрыв бомбы с нанесением урона по радиусу
    /// </summary>
    private void Explode()
    {
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRange);
        // Множество компонентов Entity, к которым уже применён урон от взрыва. Это нужно, так как взрыв может коснуться
        // объекта, состоящего из нескольких коллайдеров, тогда UsefulFeatures.GetFirstEntityComponentInObjectHierarchy()
        // вернёт один и тот же корневой компонент Entity для всех частей объекта, что неверно.
        var entitiesWithAppliedExplodeEffect = new HashSet<Entity>();

        foreach (Collider col in colliders)
        {
            if (col.GetComponent<Bomb>())
                continue;

            var entityComponent = UsefulFeatures.GetFirstEntityComponentInObjectHierarchy(col.transform);
            if (entityComponent != null)
            {
                bool itIsNewEntity = entitiesWithAppliedExplodeEffect.Add(entityComponent);
                if (itIsNewEntity)
                {
                    entityComponent.TakeDamage(Damage);
                }
            }

            var rb = col.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRange);
            }
        }

        Destroy(explosion, 2f);
    }

    protected override IEnumerator DestroyAfterLifeTime()
    {
        yield return StartCoroutine(TimeScale.SharedInstance.WaitForSeconds(Lifetime));
        pool.ReturnObject(gameObject);
        Explode();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
