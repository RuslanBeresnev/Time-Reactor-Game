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
    [SerializeField] private float interactionDistance = 2.5f;

    private bool canShoot = true;
    private bool stopShooting = false;

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
        CheckClickForShooting();
        CheckClickForWeaponReloading();
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
        yield return new WaitForSeconds(WeaponsArsenal[ActiveSlotNumber].IntervalBetweenShoots);
        canShoot = true;
    }

    /// <summary>
    /// Проверка на стрельбу в текущем кадре
    /// </summary>
    private void CheckClickForShooting()
    {
        if (Input.GetMouseButton(0) && !IsActiveSlotEmpty() && canShoot && !stopShooting)
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

        // Для тестов (замедление времени), потом убрать
        if (Input.GetMouseButton(1))
        {
            Time.timeScale = 0.02f;
            Time.fixedDeltaTime = 0.02f * 0.02f;
        }
        else
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
        }
    }

    /// <summary>
    /// Проверка на перезарядку активного оружия в руке
    /// </summary>
    private void CheckClickForWeaponReloading()
    {
        if (Input.GetKey(KeyCode.R))
        {
            WeaponsArsenal[activeSlotNumber].ReloadWeapon();
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
        Ray rayToScreenCenter = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        int defaultLayerMask = 1;

        if (Physics.Raycast(rayToScreenCenter, out hit, interactionDistance, defaultLayerMask, QueryTriggerInteraction.Ignore))
        {
            GameObject hitObject = hit.transform.gameObject;
            if (hitObject.GetComponent<Weapon>() != null)
            {
                return hitObject;
            }
        }

        return null;
    }

    /// <summary>
    /// Установить оружие в слот, если он пуст
    /// </summary>
    private bool TryPutWeaponInSlot(GameObject weapon, int slotNumber)
    {
        if (WeaponsArsenal[slotNumber] == null)
        {
            WeaponsArsenal[slotNumber] = weapon.GetComponent<Weapon>();
            weapon.transform.SetParent(weaponCamera.transform);
            // playerInterface.RedrawWeaponSlotContent(slotNumber, weapon.GetComponent<Weapon>());

            var positionInPlayerHand = weapon.GetComponent<Weapon>().PositionInPlayerHand;
            weapon.transform.position = positionInPlayerHand.position;
            weapon.transform.rotation = positionInPlayerHand.rotation;
            weapon.transform.localScale = positionInPlayerHand.localScale;
            weapon.GetComponent<Rigidbody>().isKinematic = true;

            weapon.GetComponent<Weapon>().SetUpWeaponPartsLayersAndColliders("Weapons", false);

            return true;
        }

        return false;
    }

    /// <summary>
    /// Выбросить активное оружие
    /// </summary>
    private void EjectWeapon()
    {
        var ejectionForce = 200f;
         
        if (!IsActiveSlotEmpty())
        {
            GameObject ejectedWeapon = WeaponsArsenal[ActiveSlotNumber].gameObject;
            ejectedWeapon.transform.SetParent(null);
            WeaponsArsenal[ActiveSlotNumber] = null;
            // playerInterface.RedrawWeaponSlotContent(activeSlotNumber, null);

            ejectedWeapon.GetComponent<Weapon>().PushOutWeaponFromWall(0f);

            ejectedWeapon.GetComponent<Rigidbody>().isKinematic = false;
            ejectedWeapon.GetComponent<Rigidbody>().AddForce(ejectedWeapon.transform.forward * ejectionForce);
            ejectedWeapon.GetComponent<Weapon>().SetUpWeaponPartsLayersAndColliders("Default", true);

            // Здесь присваиваивается скорость, близкая к нулю, для того, чтобы условие в корутине не сработало раньше времени
            // (так как сила броска применяется к объекту только со следующего кадра)
            ejectedWeapon.GetComponent<Rigidbody>().velocity = new Vector3(0.01f, 0, 0);
            StartCoroutine(ejectedWeapon.GetComponent<Weapon>().PerformActionsAfterFallOfEjectedWeapon());
        }
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