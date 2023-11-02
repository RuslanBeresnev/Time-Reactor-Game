using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject explosionPrefab;
    public Rigidbody rigidBody;

    public float damage;
    public float range;
    public float speed;
    public float maxLifetime;
    public float explosionForce;

    private float lifetime;

    void Start()
    {
        rigidBody.useGravity = true;
    }

    // Update is called once per frame
    void Update()
    {
        lifetime += Time.deltaTime;
        if (lifetime > maxLifetime)
            Explode();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("ONCOLLISION");
        if (collision.collider.GetComponent<Bomb>() != null) return;
        Explode();
    }

    void Explode()
    {
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Collider[] colliders = Physics.OverlapSphere(transform.position, range);

        foreach (Collider col in colliders)
        {
            if (col.GetComponent<Bomb>()) continue;

            if (col.GetComponent<Entity>() != null)
                col.GetComponent<Entity>().TakeDamage(damage);

            //@works incorrectly: clones lots of explosions
            //col.GetComponent<Rigidbody>()?.AddExplosionForce(explosionForce, transform.position, range);
        }

        Destroy(gameObject);

        //else it won't stop playing particles
        //@ fix it
        Destroy(explosion, 2f);
    }

    public void GiveKineticEnergy(Vector3 direction)
    {
        rigidBody.velocity = direction * speed * TimeScale.SharedInstance.Scale;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
