using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// Класс, реализующий каждое оружие в игре
/// </summary>
public class Weapon : ObjectWithInformation, ISerializationCallbackReceiver
{
    [SerializeField] private Transform positionInPlayerHand;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Pool pool;
    [SerializeField] private TextMeshProUGUI ammoScreen;
    [SerializeField] private Transform weaponStart;
    [SerializeField] private Transform weaponEnd;

    private AudioSource shotSound;
    private AudioSource reloadingSound;
    private List<AudioSource> weaponHitingOnSurfaceSounds = new List<AudioSource>();

    private System.Random random = new System.Random();

    [SerializeField] new private string name;
    [SerializeField] private Sprite sprite;
    [SerializeField] private float intervalBetweenShoots;
    [SerializeField] private bool semiAutoShooting;
    [SerializeField] private float reloadingDuration;
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
    public float IntervalBetweenShoots { get; private set; }

    /// <summary>
    /// Длительность перезарядки оружия
    /// </summary>
    public float ReloadingDuration { get; private set; }

    /// <summary>
    /// Если указано true, то оружие будет вести полуавтоматическую стрельбу (пистолет), иначе автоматическую (винтовка)
    /// </summary>
    public bool SemiAutoShooting { get; private set; } = true;

    /// <summary>
    /// Звук подбирания оружия
    /// </summary>
    public AudioSource PickUpSound { get; private set; }

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

    public override string[,] ObjectInfoParameters { get; set; }

    public override string ObjectInfoHeader { get; set; } = "Weapon";

    public override Color ObjectInfoHeaderColor { get; set; } = Color.yellow;

    public void OnBeforeSerialize()
    {
        positionInPlayerHand = PositionInPlayerHand;
        name = Name;
        sprite = Sprite;
        intervalBetweenShoots = IntervalBetweenShoots;
        reloadingDuration = ReloadingDuration;
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
        ReloadingDuration = reloadingDuration;
        SemiAutoShooting = semiAutoShooting;
        BulletsCountInMagazine = bulletsCountInMagazine;
        BulletsCountInReserve = bulletsCountInReserve;
    }

    private void Awake()
    {
        foreach (var audioSource in GetComponents<AudioSource>())
        {
            var clipName = audioSource.clip.name;
            if (clipName.EndsWith("Shot"))
            {
                shotSound = audioSource;
            }
            else if (clipName.EndsWith("Reloading"))
            {
                reloadingSound = audioSource;
            }
            else if (clipName.StartsWith("Weapon Hitting On Surface"))
            {
                weaponHitingOnSurfaceSounds.Add(audioSource);
            }
            else if (clipName == "Weapon Picking Up")
            {
                PickUpSound = audioSource;
            }
        }

        InitializeInfoPanelPrefab();
        ObjectInfoParameters = new string[5, 2] { { "Name:", Name },
                                                  { "Shooting type:", SemiAutoShooting ? "Semi-Automatic" : "Automatic" },
                                                  { "Firing Frequency:", Math.Round(1 / IntervalBetweenShoots).ToString() + " per sec." },
                                                  { "Bullet velocity:", bulletPrefab.GetComponent<Bullet>().Velocity.ToString() + " m/s" },
                                                  { "Damage:", bulletPrefab.GetComponent<Bullet>().Damage.ToString() + " HP" } };

        if (BulletsCountInMagazine > magazinCapacity)
        {
            BulletsCountInMagazine = magazinCapacity;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Оружие издаёт случайный звук удара с поверхностью, только когда касается слоя Default
        if (collision.gameObject.layer == 0)
        {
            // Если какой-то звук уже проигрывается, то он должен доиграться до конца
            foreach (var sound in weaponHitingOnSurfaceSounds)
            {
                if (sound.isPlaying)
                {
                    return;
                }
            }

            var randomIndex = random.Next(weaponHitingOnSurfaceSounds.Count);
            weaponHitingOnSurfaceSounds[randomIndex].Play();
        }
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

        shotSound.Play();

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
        var bullet = pool.GetObject();
        if (bullet != null)
        {
            var bulletRotation = Quaternion.FromToRotation(bulletPrefab.transform.forward, bulletDirection);
            bullet.transform.position = weaponEnd.position;
            bullet.transform.rotation = bulletRotation;
        }

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
        yield return new WaitUntil(() => GetComponent<Rigidbody>().velocity.magnitude <= 0.001f);
        GetComponent<Rigidbody>().isKinematic = true;
    }

    /// <summary>
    /// Может ли быть выполнена перезарядка оружия
    /// </summary>
    public bool ReloadingCanBePerformed()
    {
        return BulletsCountInMagazine != magazinCapacity && BulletsCountInReserve != 0;
    }

    /// <summary>
    /// Перезарядить оружие
    /// </summary>
    public void ReloadWeapon()
    {
        if (!ReloadingCanBePerformed())
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

    /// <summary>
    /// Проиграть звук перезарядки оружия
    /// </summary>
    public void PlayReloadingSound()
    {
        if (reloadingSound != null)
        {
            reloadingSound.Play();
        }
    }

    /// <summary>
    /// Прервать звук перезарядки оружия
    /// </summary>
    public void StopReloadingSound()
    {
        if (reloadingSound != null)
        {
            reloadingSound.Stop();
        }
    }
}