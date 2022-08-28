using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Управление оружием игрока и его смена
/// </summary>
public class WeaponManager : MonoBehaviour
{
    [SerializeField]
    private int arsenalMaxSize = 3;
    [SerializeField]
    private List<Weapon> weaponsArsenal = new List<Weapon>(3);
    private int activeSlotNumber = 0;

    [SerializeField]
    private Camera weaponCamera;
    [SerializeField]
    private float interactionDistance = 2.5f;

    private bool canShoot = true;
    private bool stopShooting = false;

    private void FixedUpdate()
    {
        CheckClickForShooting();
        CheckClickForChangeWeapon();
        CheckClickForEjectionOrInteraction();
    }

    /// <summary>
    /// Проверить на пустоту текущий слот
    /// </summary>
    private bool IsActiveSlotEmpty()
    {
        return weaponsArsenal[activeSlotNumber] == null;
    }

    /// <summary>
    /// Снова разрешить производить стрельбу по прошествии интервала между выстрелами
    /// </summary>
    private IEnumerator AllowShootAfterIntervalPassing()
    {
        yield return new WaitForSeconds(weaponsArsenal[activeSlotNumber].IntervalBetweenShoots);
        canShoot = true;
    }

    /// <summary>
    /// Проверка на стрельбу в текущем кадре
    /// </summary>
    private void CheckClickForShooting()
    {
        if (Input.GetMouseButton(0) && !IsActiveSlotEmpty() && canShoot && !stopShooting)
        {
            weaponsArsenal[activeSlotNumber].Shoot();
            canShoot = false;
            StartCoroutine(AllowShootAfterIntervalPassing());

            if (weaponsArsenal[activeSlotNumber].SemiAutoShooting)
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
    /// Проверка на смену активного оружия в текущем кадре
    /// </summary>
    private void CheckClickForChangeWeapon()
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
        if (newActiveSlotNumber >= 0 && newActiveSlotNumber < arsenalMaxSize && newActiveSlotNumber != activeSlotNumber)
        {
            if (!IsActiveSlotEmpty())
            {
                weaponsArsenal[activeSlotNumber].gameObject.SetActive(false);
            }
            activeSlotNumber = newActiveSlotNumber;
            if (!IsActiveSlotEmpty())
            {
                weaponsArsenal[activeSlotNumber].gameObject.SetActive(true);
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
        if (weaponsArsenal[slotNumber] == null)
        {
            weaponsArsenal[slotNumber] = weapon.GetComponent<Weapon>();
            weapon.transform.SetParent(weaponCamera.transform);

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
            GameObject ejectedWeapon = weaponsArsenal[activeSlotNumber].gameObject;
            ejectedWeapon.transform.SetParent(null);
            weaponsArsenal[activeSlotNumber] = null;

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
                TryPutWeaponInSlot(weaponInFrontOfPlayer, activeSlotNumber);
            }
            else
            {
                // Установка оружия в первый свободный слот
                for (int slotNumber = 0; slotNumber < arsenalMaxSize; slotNumber++)
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