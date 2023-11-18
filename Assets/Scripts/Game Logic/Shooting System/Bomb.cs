using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// Бомба
/// </summary>
public class Bomb : Projectile
{
    private GameObject explosionPrefab;
    private float explosionRange;

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

            //@works incorrectly: clones lots of explosions
            //col.GetComponent<Rigidbody>()?.AddExplosionForce(explosionForce, transform.position, range);
        }

        //else it won't stop playing particles
        //@ fix it
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
