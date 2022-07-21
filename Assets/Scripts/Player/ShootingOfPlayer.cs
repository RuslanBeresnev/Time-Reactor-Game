using UnityEngine;

/// <summary>
/// Реализация механизма стрельбы игроком
/// </summary>
public class ShootingOfPlayer : MonoBehaviour
{
    public new Camera camera;
    public GameObject bullet;

    private float rayDistance = 100f;
    private int damage = 1;

    private bool semiAutoShooting = true;
    private bool stopShooting = false;

    private void FixedUpdate()
    {
        if (Input.GetMouseButton(0) && !stopShooting)
        {
            Shoot();

            if (semiAutoShooting)
            {
                stopShooting = true;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            stopShooting = false;
        }
    }

    /// <summary>
    /// Сделать выстрел из оружия
    /// </summary>
    private void Shoot()
    {
        Ray rayToScreenCenter = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        int defaultLayerMask = 1;

        if (Physics.Raycast(rayToScreenCenter.origin, rayToScreenCenter.direction * rayDistance, out hit, rayDistance, defaultLayerMask, QueryTriggerInteraction.Ignore))
        {
            EntityHealth healthSystem = hit.collider.gameObject.GetComponent<EntityHealth>();
            if (healthSystem != null)
            {
                healthSystem.TakeDamage(damage);
            }
        }

        var bulletObj = Instantiate(bullet, camera.transform.position + new Vector3(0, 0, -3), Quaternion.identity);
        var bulletRigidBody = bulletObj.GetComponent<Rigidbody>();
        bulletRigidBody.AddForce(-transform.forward * 100);
    }
}