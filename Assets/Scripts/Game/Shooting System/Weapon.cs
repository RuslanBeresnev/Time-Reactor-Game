using UnityEngine;
using System;
using System.Collections;
using TMPro;

/// <summary>
/// Класс, реализующий каждое оружие в игре
/// </summary>
public class Weapon : ObjectWithInformation, ISerializationCallbackReceiver
{
    [SerializeField] private Transform positionInPlayerHand;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private TextMeshProUGUI ammoScreen;
    [SerializeField] private Transform weaponStart;
    [SerializeField] private Transform weaponEnd;

    [SerializeField] new private string name;
    [SerializeField] private Sprite sprite;
    [SerializeField] private float intervalBetweenShoots;
    [SerializeField] private bool semiAutoShooting = true;
    [SerializeField] private int magazinCapacity;
    [SerializeField] private int bulletsCountInMagazine;
    [SerializeField] private int bulletsCountInReserve;
    [SerializeField] private float rayDistance;

    /// <summary>
    /// Положение оружия в руке игрока
    /// </summary>
    public Transform PositionInPlayerHand { get; private set; }

    /// <summary>
    /// Название оружия
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Изображение оружия в арсенале игрока
    /// </summary>
    public Sprite Sprite { get; private set; }
    
    /// <summary>
    /// Минимальный интервал между выстрелами
    /// </summary>
    public float IntervalBetweenShoots { get; private set; } = 0.1f;

    /// <summary>
    /// Если указано true, то оружие будет вести полуавтоматическую стрельбу (пистолет), иначе автоматическую (винтовка)
    /// </summary>
    public bool SemiAutoShooting { get; private set; } = true;

    /// <summary>
    /// Текущее количество патронов в обойме
    /// </summary>
    public int BulletsCountInMagazine
    {
        get { return bulletsCountInMagazine; }
        set
        {
            if (value > magazinCapacity)
            {
                return;
            }

            var oldBulletsCountInMagazine = bulletsCountInMagazine;
            bulletsCountInMagazine = value;
            if (oldBulletsCountInMagazine != bulletsCountInMagazine)
            {
                RedrawAmmoScreen();
            }
        }
    }

    /// <summary>
    /// Количество патронов в запасе
    /// </summary>
    public int BulletsCountInReserve
    {
        get { return bulletsCountInReserve; }
        set
        {
            var oldBulletsCountInReserve = bulletsCountInReserve;
            bulletsCountInReserve = value;
            if (oldBulletsCountInReserve != bulletsCountInReserve)
            {
                RedrawAmmoScreen();
            }
        }
    }

    public void OnBeforeSerialize()
    {
        positionInPlayerHand = PositionInPlayerHand;
        name = Name;
        sprite = Sprite;
        intervalBetweenShoots = IntervalBetweenShoots;
        semiAutoShooting = SemiAutoShooting;
        bulletsCountInMagazine = BulletsCountInMagazine;
        bulletsCountInReserve = BulletsCountInReserve;
    }

    public void OnAfterDeserialize()
    {
        PositionInPlayerHand = positionInPlayerHand;
        Name = name;
        Sprite = sprite;
        IntervalBetweenShoots = intervalBetweenShoots;
        SemiAutoShooting = semiAutoShooting;
        BulletsCountInMagazine = bulletsCountInMagazine;
        BulletsCountInReserve = bulletsCountInReserve;
    }

    private void Awake()
    {
        infoPanelPrefab = Resources.Load<GameObject>("Prefabs/Object Information Panel");

        if (BulletsCountInMagazine > magazinCapacity)
        {
            BulletsCountInMagazine = magazinCapacity;
        }
    }

    public override void ShowInfoPanel()
    {
        if (!panelWasCreated)
        {
            createdPanel = Instantiate(infoPanelPrefab);
            panelWasCreated = true;

            createdPanel.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            createdPanel.GetComponent<Canvas>().worldCamera = Camera.main;

            var header = createdPanel.transform.Find("Header").Find("Type Of Object").GetComponent<TextMeshProUGUI>();
            header.text = "Weapon";
            header.color = Color.red;

            var firstParameter = createdPanel.transform.Find("First Parameter");
            firstParameter.Find("Parameter Text").GetComponent<TextMeshProUGUI>().text = "Name:";
            firstParameter.Find("Parameter Value").GetComponent<TextMeshProUGUI>().text = Name;

            var secondParameter = createdPanel.transform.Find("Second Parameter");
            secondParameter.Find("Parameter Text").GetComponent<TextMeshProUGUI>().text = "Shooting type:";
            secondParameter.Find("Parameter Value").GetComponent<TextMeshProUGUI>().text = semiAutoShooting ? "Semi-Automatic" : "Automatic";

            var thirdParameter = createdPanel.transform.Find("Third Parameter");
            thirdParameter.Find("Parameter Text").GetComponent<TextMeshProUGUI>().text = "Firing frequency:";
            thirdParameter.Find("Parameter Value").GetComponent<TextMeshProUGUI>().text = Math.Round(1 / IntervalBetweenShoots).ToString() + " per sec.";

            var fourthParameter = createdPanel.transform.Find("Fourth Parameter");
            fourthParameter.Find("Parameter Text").GetComponent<TextMeshProUGUI>().text = "Bullet velocity:";
            fourthParameter.Find("Parameter Value").GetComponent<TextMeshProUGUI>().text = bulletPrefab.GetComponent<Bullet>().Velocity.ToString() + " m/s";

            var fifthParameter = createdPanel.transform.Find("Fifth Parameter");
            fifthParameter.Find("Parameter Text").GetComponent<TextMeshProUGUI>().text = "Damage:";
            fifthParameter.Find("Parameter Value").GetComponent<TextMeshProUGUI>().text = bulletPrefab.GetComponent<Bullet>().Damage.ToString() + " HP";
        }

        createdPanel.SetActive(true);
    }

    public override void HideInfoPanel()
    {
        if (!panelWasCreated)
        {
            return;
        }

        createdPanel.SetActive(false);
    }

    /// <summary>
    /// Перерисовать экран с информацией о количестве патронов
    /// </summary>
    private void RedrawAmmoScreen()
    {
        ammoScreen.text = BulletsCountInMagazine.ToString() + " / " + BulletsCountInReserve.ToString();
    }

    /// <summary>
    /// Произвести выстрел из оружия
    /// </summary>
    public void Shoot()
    {
        if (BulletsCountInMagazine == 0)
        {
            return;
        }
        BulletsCountInMagazine--;

        Ray rayToScreenCenter = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        int defaultLayerMask = 1;

        // Эта переменная отвечает за направление пули к центру экрана (прицелу);
        // Из центра камеры выпускается нормированный луч, пересекает какой-то объект, и задаётся направление от дула оружия до точки
        // соприкосновения луча с поверхностью в виде единичного вектора
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
    /// Создать пулю при выстреле
    /// </summary>
    private void FireABullet(Vector3 bulletDirection)
    {
        var bulletRotation = Quaternion.FromToRotation(bulletPrefab.transform.forward, bulletDirection);
        var bullet = Instantiate(bulletPrefab, weaponEnd.position, bulletRotation);

        var bulletComponent = bullet.GetComponent<Bullet>();
        bulletComponent.GiveBulletKineticEnergy(bulletDirection);
    }

    /// <summary>
    /// Смещение оружия назад, если оно застряло в стене
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
    /// Установить всем частям оружия определённый слой и включить/выключить коллайдеры (используется при выбрасывании/подбирании оружия)
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
    /// Совершить действия с выброшенным оружием после того, как оно остановится после падения
    /// </summary>
    public IEnumerator PerformActionsAfterFallOfEjectedWeapon()
    {
        yield return new WaitUntil(() => GetComponent<Rigidbody>().velocity == Vector3.zero);
        GetComponent<Rigidbody>().isKinematic = true;
    }

    /// <summary>
    /// Перезарядить оружие
    /// </summary>
    public void ReloadWeapon()
    {
        if (BulletsCountInMagazine == magazinCapacity)
        {
            return;
        }

        var bulletsCountToFillMagazine = magazinCapacity - BulletsCountInMagazine;
        if (BulletsCountInReserve < bulletsCountToFillMagazine)
        {
            BulletsCountInMagazine += BulletsCountInReserve;
            BulletsCountInReserve = 0;
        }
        else
        {
            BulletsCountInReserve -= bulletsCountToFillMagazine;
            BulletsCountInMagazine = magazinCapacity;
        }
    }
}