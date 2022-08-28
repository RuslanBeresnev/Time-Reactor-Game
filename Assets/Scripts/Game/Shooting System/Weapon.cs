using UnityEngine;
using System.Collections;

/// <summary>
///  ласс, реализующий каждое оружие в игре
/// </summary>
public class Weapon : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField]
    private Transform positionInPlayerHand;
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private Transform weaponStart;
    [SerializeField]
    private Transform weaponEnd;

    [SerializeField]
    private string weaponName;
    [SerializeField]
    private float intervalBetweenShoots = 0.1f;
    [SerializeField]
    private bool semiAutoShooting = true;
    [SerializeField]
    private float rayDistance = 100f;

    /// <summary>
    /// ѕоложение оружи€ в руке игрока
    /// </summary>
    public Transform PositionInPlayerHand { get; private set; }

    /// <summary>
    /// Ќазвание оружи€
    /// </summary>
    public string WeaponName { get; private set; }
    
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
        positionInPlayerHand = PositionInPlayerHand;
        weaponName = WeaponName;
        intervalBetweenShoots = IntervalBetweenShoots;
        semiAutoShooting = SemiAutoShooting;
    }

    public void OnAfterDeserialize()
    {
        PositionInPlayerHand = positionInPlayerHand;
        WeaponName = weaponName;
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

    /// <summary>
    /// ”становить всем част€м оружи€ определЄнный слой и включить/выключить коллайдеры (используетс€ при выбрасывании/подбирании оружи€)
    /// </summary>
    public void SetUpWeaponPartsLayersAndColliders(string layerName, bool collidersEnabled)
    {
        Transform[] weaponParts = GetComponentsInChildren<Transform>();
        foreach (Transform part in weaponParts)
        {
            var partCollider = part.GetComponent<Collider>();
            if (partCollider != null)
            {
                partCollider.enabled = collidersEnabled;
            }
            part.gameObject.layer = LayerMask.NameToLayer(layerName);
        }
    }

    /// <summary>
    /// —овершить действи€ с выброшенным оружием после того, как оно остановитс€ после падени€
    /// </summary>
    public IEnumerator PerformActionsAfterFallOfEjectedWeapon()
    {
        yield return new WaitUntil(() => GetComponent<Rigidbody>().velocity == Vector3.zero);
        GetComponent<Rigidbody>().isKinematic = true;
    }
}