using UnityEngine;

/// <summary>
/// Класс, реализующий каждое оружие в игре
/// </summary>
public class Weapon : MonoBehaviour
{
    public new Camera camera;
    public GameObject bulletPrefab;
    public Transform gunEnd;

    private float rayDistance = 100f;
    public bool semiAutoShooting = true;
    public string weaponName;

    /// <summary>
    /// Произвести выстрел из оружия
    /// </summary>
    public void Shoot()
    {
        Ray rayToScreenCenter = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        int defaultLayerMask = 1;

        // Эта переменная отвечает за направление пули к центру экрана (прицелу);
        // Из центра камеры выпускается нормированный луч, пересекает какой-то объект, и задаётся направление от дула оружия до точки
        // соприкосновения луча с поверхностью в виде единичного вектора
        Vector3 bulletDirection;

        if (Physics.Raycast(rayToScreenCenter, out hit, rayDistance, defaultLayerMask, QueryTriggerInteraction.Ignore))
        {
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
    /// Создать пулю при выстреле
    /// </summary>
    private void FireABullet(Vector3 bulletDirection)
    {
        var bulletRotation = Quaternion.FromToRotation(bulletPrefab.transform.forward, bulletDirection);
        var bullet = Instantiate(bulletPrefab, gunEnd.position, bulletRotation);

        var bulletComponent = bullet.GetComponent<Bullet>();
        bulletComponent.GiveBulletKineticEnergy(bulletDirection);
    }
}