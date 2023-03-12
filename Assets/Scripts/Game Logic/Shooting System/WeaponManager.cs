using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

/// <summary>
/// Управление оружием игрока и его смена
/// </summary>
public class WeaponManager : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField] private List<Weapon> weaponsArsenal = new List<Weapon>(GameProperties.PlayerWeaponsArsenalSize);
    private int activeSlotNumber = 0;

    [SerializeField] private Camera weaponCamera;
    [SerializeField] private GameObject graphicAnalyzer;

    private bool canShoot = true;
    private bool stopShooting = false;
    private bool inProcessOfReloading = false;

    // Переменная нужна для того, чтобы процессы сериализации и десериализации не исполнялись до старта игры, так как
    // изначально WeaponsArsenal пуст
    private bool performSerializationAndDeserealization = false;

    /// <summary>
    /// Арсенал оружия игрока
    /// </summary>
    public ObservableCollection<Weapon> WeaponsArsenal { get; private set; } = new ObservableCollection<Weapon>();

    /// <summary>
    /// Индекс слота с активным оружием
    /// </summary>
    private int ActiveSlotNumber
    {
        get { return activeSlotNumber; }
        set
        {
            if (ActiveSlotChanged != null)
            {
                ActiveSlotChanged(ActiveSlotNumber, false);
            }
            activeSlotNumber = value;
            if (ActiveSlotChanged != null)
            {
                ActiveSlotChanged(ActiveSlotNumber, true);
            }
        }
    }

    /// <summary>
    /// Событие смены автивного оружия в руках игрока
    /// </summary>
    public Action<int, bool> ActiveSlotChanged { get; set; }

    public void OnBeforeSerialize()
    {
        if (!performSerializationAndDeserealization)
        {
            return;
        }

        for (int i = 0; i < GameProperties.PlayerWeaponsArsenalSize; i++)
        {
            weaponsArsenal[i] = WeaponsArsenal[i];
        }
    }

    public void OnAfterDeserialize()
    {
        if (!performSerializationAndDeserealization)
        {
            return;
        }

        for (int i = 0; i < GameProperties.PlayerWeaponsArsenalSize; i++)
        {
            WeaponsArsenal[i] = weaponsArsenal[i];
        }
    }

    private void Awake()
    {
        foreach (var weapon in weaponsArsenal)
        {
            WeaponsArsenal.Add(weapon);
        }

        performSerializationAndDeserealization = true;
    }

    private void FixedUpdate()
    {
        CheckClickForWeaponReloading();
        CheckClickForShooting();
        CheckClickForWeaponChanging();
        CheckClickForEjectionOrInteraction();
    }

    /// <summary>
    /// Проверить на пустоту текущий слот
    /// </summary>
    private bool IsActiveSlotEmpty()
    {
        return WeaponsArsenal[ActiveSlotNumber] == null;
    }

    /// <summary>
    /// Снова разрешить производить стрельбу по прошествии интервала между выстрелами
    /// </summary>
    private IEnumerator AllowShootAfterIntervalPassing()
    {
        // Задержка в реальных секундах
        yield return new WaitForSeconds(WeaponsArsenal[ActiveSlotNumber].IntervalBetweenShoots);
        canShoot = true;
    }

    /// <summary>
    /// Проверка на стрельбу в текущем кадре
    /// </summary>
    private void CheckClickForShooting()
    {
        if (Input.GetMouseButton(0) && !IsActiveSlotEmpty() && !inProcessOfReloading && canShoot && !stopShooting)
        {
            WeaponsArsenal[ActiveSlotNumber].Shoot();
            canShoot = false;
            StartCoroutine(AllowShootAfterIntervalPassing());

            if (WeaponsArsenal[ActiveSlotNumber].SemiAutoShooting)
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
    /// Снова разрешить перезарядку оружия после прошествия интервала времени
    /// </summary>
    private IEnumerator AllowReloadingAfterIntervalPassing()
    {
        // Задержка в реальных секундах
        yield return new WaitForSeconds(WeaponsArsenal[ActiveSlotNumber].ReloadingDuration);
        WeaponsArsenal[activeSlotNumber].ReloadWeapon();
        inProcessOfReloading = false;
    }

    /// <summary>
    /// Остановтить сопроцесс, который блокирует оружие во время перезарядки
    /// </summary>
    private void StopWeaponReloadingCoroutine()
    {
        StopCoroutine("AllowReloadingAfterIntervalPassing");
        inProcessOfReloading = false;
    }

    /// <summary>
    /// Проверка на перезарядку активного оружия в руке
    /// </summary>
    private void CheckClickForWeaponReloading()
    {
        if (Input.GetKey(KeyCode.R) && !Input.GetMouseButton(0) && !IsActiveSlotEmpty() && !inProcessOfReloading)
        {
            if (!WeaponsArsenal[activeSlotNumber].ReloadingCanBePerformed())
            {
                return;
            }

            inProcessOfReloading = true;
            WeaponsArsenal[activeSlotNumber].PlayReloadingSound();
            StartCoroutine("AllowReloadingAfterIntervalPassing");
        }
    }

    /// <summary>
    /// Проверка на смену активного оружия в текущем кадре
    /// </summary>
    private void CheckClickForWeaponChanging()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            ChangeActiveWeapon(0);
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            ChangeActiveWeapon(1);
        }
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            ChangeActiveWeapon(2);
        }
    }

    /// <summary>
    /// Проверка на выбрасывание предмета или взаимодействие с объектом в текущем кадре
    /// </summary>
    private void CheckClickForEjectionOrInteraction()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            EjectWeapon();
        }
        else if (Input.GetKey(KeyCode.E))
        {
            PickUpWeapon();
        }
    }

    /// <summary>
    /// Сменить активное оружие на другое
    /// </summary>
    private void ChangeActiveWeapon(int newActiveSlotNumber)
    {
        if (newActiveSlotNumber >= 0 && newActiveSlotNumber < GameProperties.PlayerWeaponsArsenalSize && newActiveSlotNumber != ActiveSlotNumber)
        {
            StopWeaponReloadingCoroutine();
            if (!IsActiveSlotEmpty())
            {
                WeaponsArsenal[ActiveSlotNumber].StopReloadingSound();
            }

            if (!IsActiveSlotEmpty())
            {
                WeaponsArsenal[ActiveSlotNumber].gameObject.SetActive(false);
            }

            ActiveSlotNumber = newActiveSlotNumber;

            if (!IsActiveSlotEmpty())
            {
                WeaponsArsenal[ActiveSlotNumber].gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Проверить, является ли оружием объект, на который смотрит игрок, и вернуть его в таком случае
    /// </summary>
    private GameObject CheckObjectAheadIsWeapon()
    {
        (var objectAhead, var _) = graphicAnalyzer.GetComponent<GraphicAnalyzerController>().GetObjectPlayerIsLookingAt();
        if (objectAhead != null && objectAhead.GetComponent<Weapon>() != null)
        {
            return objectAhead;
        }
        return null;
    }

    /// <summary>
    /// Установить оружие в слот, если он пуст
    /// </summary>
    private bool TryPutWeaponInSlot(GameObject weapon, int slotNumber)
    {
        if (WeaponsArsenal[slotNumber] != null)
        {
            return false;
        }

        WeaponsArsenal[slotNumber] = weapon.GetComponent<Weapon>();
        weapon.transform.SetParent(weaponCamera.transform);

        WeaponsArsenal[slotNumber].PickUpSound.Play();

        var positionInPlayerHand = weapon.GetComponent<Weapon>().PositionInPlayerHand;
        weapon.transform.position = positionInPlayerHand.position;
        weapon.transform.rotation = positionInPlayerHand.rotation;
        weapon.transform.localScale = positionInPlayerHand.localScale;
        weapon.GetComponent<Rigidbody>().isKinematic = true;

        weapon.GetComponent<Weapon>().SetUpWeaponPartsLayersAndColliders("Weapons", false);

        return true;
    }

    /// <summary>
    /// Выбросить активное оружие
    /// </summary>
    private void EjectWeapon()
    {
        if (IsActiveSlotEmpty())
        {
            return;
        }

        StopWeaponReloadingCoroutine();
        if (!IsActiveSlotEmpty())
        {
            WeaponsArsenal[ActiveSlotNumber].StopReloadingSound();
        }

        var ejectionForce = 200f;
        GameObject ejectedWeapon = WeaponsArsenal[ActiveSlotNumber].gameObject;
        ejectedWeapon.transform.SetParent(null);
        WeaponsArsenal[ActiveSlotNumber] = null;

        ejectedWeapon.GetComponent<Weapon>().PushOutWeaponFromWall(0f);

        ejectedWeapon.GetComponent<Rigidbody>().isKinematic = false;
        ejectedWeapon.GetComponent<Rigidbody>().AddForce(ejectedWeapon.transform.forward * ejectionForce * TimeScale.SharedInstance.Scale);
        ejectedWeapon.GetComponent<Weapon>().SetUpWeaponPartsLayersAndColliders("Default", true);

        // Здесь присваиваивается скорость, близкая к нулю, для того, чтобы условие в корутине не сработало раньше времени
        // (так как сила броска применяется к объекту только со следующего кадра)
        ejectedWeapon.GetComponent<Rigidbody>().velocity = new Vector3(0.01f, 0, 0);
        StartCoroutine(ejectedWeapon.GetComponent<Weapon>().PerformActionsAfterFallOfEjectedWeapon());
    }

    /// <summary>
    /// Подобрать оружие, на которое смотрит игрок
    /// </summary>
    private void PickUpWeapon()
    {
        GameObject weaponInFrontOfPlayer = CheckObjectAheadIsWeapon();
        if (weaponInFrontOfPlayer != null)
        {
            if (IsActiveSlotEmpty())
            {
                TryPutWeaponInSlot(weaponInFrontOfPlayer, ActiveSlotNumber);
            }
            else
            {
                // Установка оружия в первый свободный слот
                for (int slotNumber = 0; slotNumber < GameProperties.PlayerWeaponsArsenalSize; slotNumber++)
                {
                    bool weaponPickedUp = TryPutWeaponInSlot(weaponInFrontOfPlayer, slotNumber);
                    if (weaponPickedUp)
                    {
                        weaponInFrontOfPlayer.SetActive(false);
                        break;
                    }
                }
            }
        }
    }
}