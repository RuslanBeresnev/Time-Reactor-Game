using System.Collections;
using UnityEngine;

/// <summary>
/// Бомба
/// </summary>
public class Bomb : Projectile
{
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionRange;

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
    void Explode()
    {
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRange);

        foreach (Collider col in colliders)
        {
            if (col.GetComponent<Bomb>()) continue;

            if (col.GetComponent<Entity>() != null)
                col.GetComponent<Entity>().TakeDamage(Damage);

            //почему-то создаёт лишние взрывы
            //col.GetComponent<Rigidbody>()?.AddExplosionForce(explosionForce, transform.position, range);
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
