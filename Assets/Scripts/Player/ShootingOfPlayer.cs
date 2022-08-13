using UnityEngine;

/// <summary>
/// Реализация механизма стрельбы игроком
/// </summary>
public class ShootingOfPlayer : MonoBehaviour
{
    public new Camera camera;
    public GameObject bulletPrefab;
    public Transform gunEnd;

    private float rayDistance = 100f;
    private int damage = 1;
    private int bulletVelocity = 100;

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
        else if (!Input.GetMouseButton(0))
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

        // Эта переменная отвечает за направление пули к центру экрана (прицелу);
        // Из центра камеры выпускается нормированный луч, пересекает какой-то объект, и задаётся направление от дула оружия до точки
        // соприкосновения луча с поверхностью в виде единичного вектора
        Vector3 bulletDirection;

        if (Physics.Raycast(rayToScreenCenter.origin, rayToScreenCenter.direction * rayDistance, out hit, rayDistance, defaultLayerMask, QueryTriggerInteraction.Ignore))
        {
            EntityHealth healthSystem = hit.collider.gameObject.GetComponent<EntityHealth>();
            if (healthSystem != null)
            {
                healthSystem.TakeDamage(damage);
            }

            bulletDirection = (hit.point - gunEnd.position) / Vector3.Distance(hit.point, gunEnd.position);
        }
        else
        {
            bulletDirection = (rayToScreenCenter.origin + rayToScreenCenter.direction * rayDistance - gunEnd.position) /
                Vector3.Distance(gunEnd.position, rayToScreenCenter.origin + rayToScreenCenter.direction * rayDistance);
        }

        FireABullet(bulletDirection);
    }

    /// <summary>
    /// Создать пулю и придать ей направленную скорость
    /// </summary>
    private void FireABullet(Vector3 bulletDirection)
    {
        var bulletRotation = Quaternion.FromToRotation(bulletPrefab.transform.forward, bulletDirection);
        var bullet = Instantiate(bulletPrefab, gunEnd.position, bulletRotation);

        var bulletRigidBody = bullet.GetComponent<Rigidbody>();
        bulletRigidBody.velocity = bulletDirection * bulletVelocity;
    }
}