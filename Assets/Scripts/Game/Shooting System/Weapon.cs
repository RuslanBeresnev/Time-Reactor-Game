using UnityEngine;

/// <summary>
///  ласс, реализующий каждое оружие в игре
/// </summary>
public class Weapon : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private Transform weaponStart;
    [SerializeField]
    private Transform weaponEnd;

    [SerializeField]
    private string weaponName;
    [SerializeField]
    private int weaponNumberInGame;
    [SerializeField]
    private float intervalBetweenShoots = 0.1f;
    [SerializeField]
    private bool semiAutoShooting = true;
    [SerializeField]
    private float rayDistance = 100f;

    /// <summary>
    /// Ќазвание оружи€
    /// </summary>
    public string WeaponName { get; private set; }

    /// <summary>
    /// ѕор€дковый номер оружи€ в игре (среди всего оружи€)
    /// </summary>
    public int WeaponNumberInGame { get; private set; }
    
    /// <summary>
    /// ћинимальный интервал между выстрелами
    /// </summary>
    public float IntervalBetweenShoots { get; private set; } = 0.1f;

    /// <summary>
    /// ≈сли указано true, то оружие будет вести полуавтоматическую стрельбу (пистолет), иначе автоматическую (винтовка)
    /// </summary>
    public bool SemiAutoShooting { get; private set; } = true;

    public void OnBeforeSerialize()
    {
        weaponName = WeaponName;
        weaponNumberInGame = WeaponNumberInGame;
        intervalBetweenShoots = IntervalBetweenShoots;
        semiAutoShooting = SemiAutoShooting;
    }

    public void OnAfterDeserialize()
    {
        WeaponName = weaponName;
        WeaponNumberInGame = weaponNumberInGame;
        IntervalBetweenShoots = intervalBetweenShoots;
        SemiAutoShooting = semiAutoShooting;
    }

    /// <summary>
    /// ѕроизвести выстрел из оружи€
    /// </summary>
    public void Shoot()
    {
        Ray rayToScreenCenter = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        int defaultLayerMask = 1;

        // Ёта переменна€ отвечает за направление пули к центру экрана (прицелу);
        // »з центра камеры выпускаетс€ нормированный луч, пересекает какой-то объект, и задаЄтс€ направление от дула оружи€ до точки
        // соприкосновени€ луча с поверхностью в виде единичного вектора
        Vector3 bulletDirection;

        if (Physics.Raycast(rayToScreenCenter, out hit, rayDistance, defaultLayerMask, QueryTriggerInteraction.Ignore))
        {
            bulletDirection = (hit.point - weaponEnd.position) / Vector3.Distance(hit.point, weaponEnd.position);
        }
        else
        {
            bulletDirection = (rayToScreenCenter.origin + rayToScreenCenter.direction * rayDistance - weaponEnd.position) /
                Vector3.Distance(weaponEnd.position, rayToScreenCenter.origin + rayToScreenCenter.direction * rayDistance);
        }

        FireABullet(bulletDirection);
    }

    /// <summary>
    /// —оздать пулю при выстреле
    /// </summary>
    private void FireABullet(Vector3 bulletDirection)
    {
        var bulletRotation = Quaternion.FromToRotation(bulletPrefab.transform.forward, bulletDirection);
        var bullet = Instantiate(bulletPrefab, weaponEnd.position, bulletRotation);

        var bulletComponent = bullet.GetComponent<Bullet>();
        bulletComponent.GiveBulletKineticEnergy(bulletDirection);
    }

    /// <summary>
    /// —мещение оружи€ назад, если оно застр€ло в стене
    /// </summary>
    public void PushOutWeaponFromWall(float distanceFromWhichToPushWeapon)
    {
        var layerMask = 1;
        var weaponDisplacementDistance = UsefulFeatures.CalculateDepthOfObjectEntryIntoNearestSurface(weaponStart.position, weaponEnd.position, layerMask);
        if (weaponDisplacementDistance > distanceFromWhichToPushWeapon)
        {
            transform.position += -transform.forward * weaponDisplacementDistance;
        }
    }
}