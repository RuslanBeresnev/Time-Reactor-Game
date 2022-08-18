using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Управление оружием игрока и его смена
/// </summary>
public class WeaponManager : MonoBehaviour
{
    public List<Weapon> weaponsArsenal = new List<Weapon>(3);
    private Weapon activeWeapon;
    private int activeSlotNumber = 1;
    private int arsenalMaxSize = 3;

    private bool stopShooting = false;

    private void Awake()
    {
        activeWeapon = weaponsArsenal[0];
    }

    private void FixedUpdate()
    {
        CheckClickForShooting();
        CheckClickForChangeWeapon();
    }

    /// <summary>
    /// Проверка на стрельбу в текущем кадре
    /// </summary>
    private void CheckClickForShooting()
    {
        if (Input.GetMouseButton(0) && !stopShooting)
        {
            activeWeapon.Shoot();

            if (activeWeapon.semiAutoShooting)
            {
                stopShooting = true;
            }
        }
        else if (!Input.GetMouseButton(0))
        {
            stopShooting = false;
        }

        // Для тестов (замедление времени)
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
            ChangeActiveWeapon(1);
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            ChangeActiveWeapon(2);
        }
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            ChangeActiveWeapon(3);
        }
    }

    /// <summary>
    /// Сменить активное оружие на другое
    /// </summary>
    private void ChangeActiveWeapon(int newActiveSlotNumber)
    {
        if (newActiveSlotNumber >= 1 && newActiveSlotNumber <= arsenalMaxSize && newActiveSlotNumber != activeSlotNumber)
        {
            activeSlotNumber = newActiveSlotNumber;
            activeWeapon.gameObject.SetActive(false);
            activeWeapon = weaponsArsenal[newActiveSlotNumber - 1];
            activeWeapon.gameObject.SetActive(true);
        }
    }
}