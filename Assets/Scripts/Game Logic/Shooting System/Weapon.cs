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
    Projectile,
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

    [HideInInspector][SerializeField] private Transform positionInPlayerHand;
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
    [HideInInspector][SerializeField] private float reloadingDuration;
    [HideInInspector][SerializeField] private int magazinCapacity;
    [HideInInspector][SerializeField] private int bulletsCountInMagazine;
    [HideInInspector][SerializeField] private int bulletsCountInReserve;
    [HideInInspector][SerializeField] private float rayDistance;

    #region Свойства
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
    public float IntervalBetweenShoots 
    {
        get => intervalBetweenShoots; 
        set => intervalBetweenShoots = value;
    }

    /// <summary>
    /// Длительность перезарядки оружия
    /// </summary>
    public float ReloadingDuration 
    { 
        get => reloadingDuration; 
        set => reloadingDuration = value; 
    }

    

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

    public virtual void OnBeforeSerialize()
    {
        positionInPlayerHand = PositionInPlayerHand;
        name = Name;
        sprite = Sprite;
        //intervalBetweenShoots = IntervalBetweenShoots;
        //reloadingDuration = ReloadingDuration;
        //semiAutoShooting = SemiAutoShooting;
        bulletsCountInMagazine = BulletsCountInMagazine;
        bulletsCountInReserve = BulletsCountInReserve;
    }

    public virtual void OnAfterDeserialize()
    {
        PositionInPlayerHand = positionInPlayerHand;
        Name = name;
        Sprite = sprite;
        //IntervalBetweenShoots = intervalBetweenShoots;
        //ReloadingDuration = reloadingDuration;
        //SemiAutoShooting = semiAutoShooting;
        BulletsCountInMagazine = bulletsCountInMagazine;
        BulletsCountInReserve = bulletsCountInReserve;
    }

    protected virtual void Awake()
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
    protected virtual void RedrawAmmoScreen()
    {
        //ammoScreen.text = BulletsCountInMagazine.ToString() + " / " + BulletsCountInReserve.ToString();
    }

    #endregion

    /// <summary>
    /// Произвести выстрел из оружия
    /// </summary>
    public virtual void Shoot()
    {
        if (type == Type.Projectile)
        {
            GetComponentInChildren<ProjectileWeapon>().Shoot();
        } 
        else if (type == Type.Laser)
        {
            GetComponentInChildren<LaserWeapon>().Shoot();

        }
        else if (type == Type.Annihilating)
        {
            GetComponentInChildren<AnnihilatingWeapon>().Shoot();

        }
        else if (type == Type.Wall)
        {
            GetComponentInChildren<WallBuilder>().Shoot();
        }
    }

    /// <summary>
    /// Получить RaycastHit при выстреле
    /// </summary>
    protected bool GetRaycastHit(ref RaycastHit hit)
    {
        Ray rayToScreenCenter = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        int defaultLayerMask = 1;

        return Physics.Raycast(rayToScreenCenter, out hit, RayDistance, defaultLayerMask, QueryTriggerInteraction.Ignore);
    }

    /// <summary>
    /// Определить направление выстрела
    /// </summary>
    protected Vector3 GetShootingDirection()
    {
        Ray rayToScreenCenter = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit = new RaycastHit();

        // Эта переменная отвечает за направление пули к центру экрана (прицелу);
        // Из центра камеры выпускается нормированный луч, пересекает какой-то объект, и задаётся направление от дула оружия до точки
        // соприкосновения луча с поверхностью в виде единичного вектора
        Vector3 bulletDirection;

        if (GetRaycastHit(ref hit))
        {
            bulletDirection = (hit.point - WeaponEnd.position).normalized;
        }
        else
        {
            bulletDirection = (rayToScreenCenter.origin + rayToScreenCenter.direction * RayDistance - WeaponEnd.position).normalized;
        }

        return bulletDirection;
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
    public virtual bool ReloadingCanBePerformed()
    {
        if (type == Type.Projectile)
        {
            return GetComponentInChildren<ProjectileWeapon>().ReloadingCanBePerformed();
        }
        //else if (type == Type.Laser)
        //{
        //    GetComponentInChildren<LaserWeapon>().Shoot();

        //}
        //else if (type == Type.Annihilating)
        //{
        //    GetComponentInChildren<AnnihilatingWeapon>().Shoot();

        //}
        //else if (type == Type.Wall)
        //{
        //    GetComponentInChildren<WallBuilder>().Shoot();

        //}
        return false;
    }

    /// <summary>
    /// Перезарядить оружие
    /// </summary>
    public virtual void ReloadWeapon()
    {
        if (type == Type.Projectile)
        {
            GetComponentInChildren<ProjectileWeapon>().ReloadWeapon();
        }
        //else if (type == Type.Laser)
        //{
        //    GetComponentInChildren<LaserWeapon>().Shoot();

        //}
        //else if (type == Type.Annihilating)
        //{
        //    GetComponentInChildren<AnnihilatingWeapon>().Shoot();

        //}
        //else if (type == Type.Wall)
        //{
        //    GetComponentInChildren<WallBuilder>().Shoot();

        //}

        
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