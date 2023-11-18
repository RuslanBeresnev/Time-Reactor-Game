using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Assertions.Must;

/// <summary>
/// Вид оружия
/// </summary>
public enum Type
{
    Firearm,
    RPG,
    Laser,
    Annihilating,
    Wall
}

/// <summary>
/// Класс, реализующий каждое оружие в игре
/// </summary>
public class Weapon : ObjectWithInformation, ISerializationCallbackReceiver
{
    [HideInInspector][SerializeField] private Type type;

    [HideInInspector][SerializeField] private float laserDamage;
    [HideInInspector][SerializeField] private float laserWidth = 0.2f;
    [HideInInspector][SerializeField] private Color laserColor;
    [HideInInspector][SerializeField] private Material laserMaterial;
    private GameObject laserGO;
    [HideInInspector][SerializeField] private string annihilatingTag;

    [HideInInspector][SerializeField] private GameObject wallPrefab;

    [HideInInspector][SerializeField] private Transform positionInPlayerHand;
    [HideInInspector][SerializeField] private GameObject bulletPrefab;
    [HideInInspector][SerializeField] private Pool pool;
    [HideInInspector][SerializeField] private TextMeshProUGUI ammoScreen;
    [HideInInspector][SerializeField] private Transform weaponStart;
    [HideInInspector][SerializeField] private Transform weaponEnd;

    [HideInInspector][SerializeField] private AudioSource shotSound;
    [HideInInspector][SerializeField] private AudioSource reloadingSound;
    //@typo: hiting=>hitting
    [HideInInspector][SerializeField] private List<AudioSource> weaponHitingOnSurfaceSounds = new List<AudioSource>();

    private System.Random random = new System.Random();

    [HideInInspector][SerializeField] new private string name;
    [HideInInspector][SerializeField] private Sprite sprite;
    //@ typo: shoots=>shots
    [HideInInspector][SerializeField] private float intervalBetweenShoots;
    [HideInInspector][SerializeField] private bool semiAutoShooting;
    [HideInInspector][SerializeField] private float reloadingDuration;
    [HideInInspector][SerializeField] private int magazinCapacity;
    [HideInInspector][SerializeField] private int bulletsCountInMagazine;
    [HideInInspector][SerializeField] private int bulletsCountInReserve;
    [HideInInspector][SerializeField] private float rayDistance;

    #region Свойства
    /// <summary>
    /// Тэг, означающий, что может быть уничтожено аннигилирующим оружием
    /// </summary>
    public string AnnihilatingTag
    {
        get { return annihilatingTag; }
        set { annihilatingTag = value; }
    }

    /// <summary>
    /// Префаб стены для постройки
    /// </summary>
    public GameObject WallPrefab
    {
        get { return wallPrefab; }
        set { wallPrefab = value; }
    }

    /// <summary>
    /// Материал лазера
    /// </summary>
    public Material LaserMaterial
    {
        get { return laserMaterial; }
        set { laserMaterial = value; }
    }

    /// <summary>
    /// Цвет лазера
    /// </summary>
    public Color LaserColor
    {
        get { return laserColor; }
        set { laserColor = value; }
    }

    /// <summary>
    /// Ширина лазера
    /// </summary>
    public float LaserWidth
    {
        get { return laserWidth; }
        set { laserWidth = value; }
    }

    /// <summary>
    /// Урон от лазера
    /// </summary>
    public float LaserDamage
    {
        get { return laserDamage; }
        set { laserDamage = value; }
    }

    /// <summary>
    /// Вид (тип) оружия
    /// </summary>
    public Type Type
    {
        get { return type; }
        set { type = value; }
    }

    /// <summary>
    /// Положение оружия в руке игрока
    /// </summary>
    public Transform PositionInPlayerHand { get; set; }

    /// <summary>
    /// Префаб пули (снаряда)
    /// </summary>
    public GameObject BulletPrefab
    {
        get { return bulletPrefab; }
        set { bulletPrefab = value; }
    }

    /// <summary>
    /// Пул объектов (пуль)
    /// </summary>
    public Pool Pool
    {
        get { return pool; }
        set { pool = value; }
    }

    /// <summary>
    /// Экран с информацией о количестве патронов
    /// </summary>
    public TextMeshProUGUI AmmoScreen
    {
        get { return ammoScreen; }
        set { ammoScreen = value; }
    }

    /// <summary>
    /// Позиция начала оружия
    /// </summary>
    public Transform WeaponStart
    {
        get { return weaponStart; }
        set { weaponStart = value; }
    }

    /// <summary>
    /// Позиция конца оружия
    /// </summary>
    public Transform WeaponEnd
    {
        get { return weaponEnd; }
        set { weaponEnd = value; }
    }

    /// <summary>
    /// Звук выстрела
    /// </summary>
    public AudioSource ShotSound
    {
        get { return shotSound; }
        set { shotSound = value; }
    }

    /// <summary>
    /// Звук перезарядки
    /// </summary>
    public AudioSource ReloadingSound
    {
        get { return reloadingSound; }
        set { reloadingSound = value; }
    }

    /// <summary>
    /// Звуки удара оружия о поверхность
    /// </summary>
    public List<AudioSource> WeaponHitingOnSurfaceSounds
    {
        get { return weaponHitingOnSurfaceSounds; }
        set { weaponHitingOnSurfaceSounds = value; }
    }

    /// <summary>
    /// Название оружия
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Изображение оружия в арсенале игрока
    /// </summary>
    public Sprite Sprite { get; set; }

    /// <summary>
    /// Минимальный интервал между выстрелами
    /// </summary>
    public float IntervalBetweenShoots { get; set; }

    /// <summary>
    /// Длительность перезарядки оружия
    /// </summary>
    public float ReloadingDuration { get; set; }

    /// <summary>
    /// Если указано true, то оружие будет вести полуавтоматическую стрельбу (пистолет), иначе автоматическую (винтовка)
    /// </summary>
    public bool SemiAutoShooting { get; set; } = true;

    /// <summary>
    /// Звук подбора оружия
    /// </summary>
    public AudioSource PickUpSound { get; set; }

    /// <summary>
    /// Вместимость обоймы
    /// </summary>
    public int MagazinCapacity
    {
        get { return magazinCapacity; }
        set { magazinCapacity = value; }
    }

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

    /// <summary>
    /// Дальность стрельбы
    /// </summary>
    public float RayDistance
    {
        get { return rayDistance; }
        set { rayDistance = value; }
    }

    #endregion

    #region Other
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
        if (type == Type.Laser || type == Type.Annihilating)
        {
            SemiAutoShooting = false;
        }

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
        string damage = "";
        string velocity = "";
        if (type == Type.Laser)
        {
            damage = LaserDamage.ToString();
            velocity = "N/A";
        }
        else if (type == Type.Firearm || type == Type.RPG)
        {
            damage = bulletPrefab.GetComponent<Projectile>().Damage.ToString();
            velocity = bulletPrefab.GetComponent<Projectile>().Velocity.ToString();
        }
        ObjectInfoParameters = new string[5, 2] { { "Name:", Name },
                                                  { "Shooting type:", SemiAutoShooting ? "Semi-Automatic" : "Automatic" },
                                                  { "Firing Frequency:", Math.Round(1 / IntervalBetweenShoots).ToString() + " per sec." },
                                                  { "Bullet velocity:", velocity  + " m/s" },
                                                  { "Damage:", damage + " HP" } };

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

    #endregion

    /// <summary>
    /// Произвести выстрел из оружия
    /// </summary>
    public void Shoot()
    {
        if (type == Type.Firearm || type == Type.RPG)
        {
            if (BulletsCountInMagazine == 0)
            {
                return;
            }
            BulletsCountInMagazine--;
        }

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
            bulletDirection = (hit.point - weaponEnd.position).normalized;
        }
        else
        {
            bulletDirection = (rayToScreenCenter.origin + rayToScreenCenter.direction * rayDistance - weaponEnd.position).normalized;
        }

        if (type == Type.Firearm || type == Type.RPG)
        {
            FireProjectile(bulletDirection);
        }
        else if (type == Type.Laser)
        {
            FireLaser(hit);
        }
        else if (type == Type.Wall)
        {
            BuildWall(hit);
        }
        else if (type == Type.Annihilating)
        {
            FireAnnihilating(hit);
        }
    }

    /// <summary>
    /// Построить стену при выстреле
    /// </summary>
    private void BuildWall(RaycastHit hit)
    {
        //Поворот барьера к игроку основной стороной 
        float yRotation = gameObject.transform.rotation.eulerAngles.y;
        Quaternion rotation = Quaternion.Euler(0, yRotation, 0);

        //Отступ, чтобы барьер ставился точно на поверхность
        var renderer = wallPrefab.GetComponent<Renderer>();
        float yOffset = renderer.bounds.extents.y * hit.normal.y;

        Vector3 position = new Vector3(hit.point.x, hit.point.y + yOffset, hit.point.z);
        var wallGO = Instantiate(wallPrefab, position, rotation);
        wallGO.tag = "Annihil";
    } 

    /// <summary>
    /// Испускать лазер при выстреле
    /// </summary>
    private void FireLaser(RaycastHit hit)
    {
        MakeLaser(hit);

        var entity = hit.transform.GetComponent<Entity>();
        if (entity != null)
        {
            entity.TakeDamage(laserDamage);
        }
    }

    /// <summary>
    /// Испускать аннигилирующий лазер
    /// </summary>
    private void FireAnnihilating(RaycastHit hit)
    {
        MakeLaser(hit);

        var target = hit.transform.gameObject;
        if (target.CompareTag(annihilatingTag))
        {
            Destroy(target.gameObject);
        }
    }

    /// <summary>
    /// Создать лазер
    /// </summary>
    private void MakeLaser(RaycastHit hit)
    {
        if (laserGO == null)
        {
            laserGO = new GameObject("laserGO", typeof(LineRenderer));
            laserGO.transform.parent = transform;

            LineRenderer lineRendererComponent = laserGO.GetComponent<LineRenderer>();
            lineRendererComponent.material = laserMaterial;
            lineRendererComponent.material.SetColor("_Color", laserColor);
            lineRendererComponent.startWidth = laserWidth;
            lineRendererComponent.endWidth = laserWidth;
        }

        LineRenderer lineRenderer = laserGO.GetComponent<LineRenderer>();

        lineRenderer.SetPosition(0, weaponEnd.position);
        lineRenderer.SetPosition(1, hit.point);

        lineRenderer.enabled = true;
    }

    /// <summary>
    /// Прекратить испускание лазера
    /// </summary>
    public void StopLaser()
    {
        if (laserGO != null)
        {
            laserGO.GetComponent<LineRenderer>().enabled = false;
        }
    }

    /// <summary>
    /// Создать пулю при выстреле
    /// </summary>
    private void FireProjectile(Vector3 bulletDirection)
    {
        var projectile = pool.GetObject();
        if (projectile != null)
        {
            var rotation = Quaternion.FromToRotation(bulletPrefab.transform.forward, bulletDirection);
            projectile.transform.position = weaponEnd.position;
            projectile.transform.rotation = rotation;
        }

        var projectileComponent = projectile.GetComponent<Projectile>();
        projectileComponent.GiveKineticEnergy(bulletDirection);
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