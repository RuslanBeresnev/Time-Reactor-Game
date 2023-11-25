using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEditor.Experimental.GraphView;

/// <summary>
/// РЈРїСЂР°РІР»РµРЅРёРµ РѕСЂСѓР¶РёРµРј РёРіСЂРѕРєР° Рё РµРіРѕ СЃРјРµРЅР°
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

    // РџРµСЂРµРјРµРЅРЅР°СЏ РЅСѓР¶РЅР° РґР»СЏ С‚РѕРіРѕ, С‡С‚РѕР±С‹ РїСЂРѕС†РµСЃСЃС‹ СЃРµСЂРёР°Р»РёР·Р°С†РёРё Рё РґРµСЃРµСЂРёР°Р»РёР·Р°С†РёРё РЅРµ РёСЃРїРѕР»РЅСЏР»РёСЃСЊ РґРѕ СЃС‚Р°СЂС‚Р° РёРіСЂС‹, С‚Р°Рє РєР°Рє
    // РёР·РЅР°С‡Р°Р»СЊРЅРѕ WeaponsArsenal РїСѓСЃС‚
    private bool performSerializationAndDeserealization = false;

    /// <summary>
    /// РђСЂСЃРµРЅР°Р» РѕСЂСѓР¶РёСЏ РёРіСЂРѕРєР°
    /// </summary>
    public ObservableCollection<Weapon> WeaponsArsenal { get; private set; } = new ObservableCollection<Weapon>();

    /// <summary>
    /// РРЅРґРµРєСЃ СЃР»РѕС‚Р° СЃ Р°РєС‚РёРІРЅС‹Рј РѕСЂСѓР¶РёРµРј
    /// </summary>
    public int ActiveSlotNumber
    {
        get { return activeSlotNumber; }
        private set
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
    /// РЎРѕР±С‹С‚РёРµ СЃРјРµРЅС‹ Р°РІС‚РёРІРЅРѕРіРѕ РѕСЂСѓР¶РёСЏ РІ СЂСѓРєР°С… РёРіСЂРѕРєР°
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
    /// РџСЂРѕРІРµСЂРёС‚СЊ РЅР° РїСѓСЃС‚РѕС‚Сѓ С‚РµРєСѓС‰РёР№ СЃР»РѕС‚
    /// </summary>
    private bool IsActiveSlotEmpty()
    {
        return WeaponsArsenal[ActiveSlotNumber] == null;
    }

    /// <summary>
    /// РЎРЅРѕРІР° СЂР°Р·СЂРµС€РёС‚СЊ РїСЂРѕРёР·РІРѕРґРёС‚СЊ СЃС‚СЂРµР»СЊР±Сѓ РїРѕ РїСЂРѕС€РµСЃС‚РІРёРё РёРЅС‚РµСЂРІР°Р»Р° РјРµР¶РґСѓ РІС‹СЃС‚СЂРµР»Р°РјРё
    /// </summary>
    private IEnumerator AllowShootAfterIntervalPassing()
    {
        // Р—Р°РґРµСЂР¶РєР° РІ СЂРµР°Р»СЊРЅС‹С… СЃРµРєСѓРЅРґР°С…
        yield return new WaitForSeconds(WeaponsArsenal[ActiveSlotNumber].IntervalBetweenShoots);
        canShoot = true;
    }

    /// <summary>
    /// РџСЂРѕРІРµСЂРєР° РЅР° СЃС‚СЂРµР»СЊР±Сѓ РІ С‚РµРєСѓС‰РµРј РєР°РґСЂРµ
    /// </summary>
    private void CheckClickForShooting()
    {
        if (Input.GetMouseButton(0) && !IsActiveSlotEmpty() && !inProcessOfReloading && canShoot && !stopShooting)
        {
            WeaponsArsenal[ActiveSlotNumber].Shoot();
            canShoot = false;
            StartCoroutine(AllowShootAfterIntervalPassing());

            var weapon = WeaponsArsenal[ActiveSlotNumber];
            if (weapon.SemiAutoShooting)
            {
                stopShooting = true;
            }
        }
        else if (!Input.GetMouseButton(0))
        {
            if (WeaponsArsenal[ActiveSlotNumber] != null)
            {
                stopShooting = false;
                var weapon = WeaponsArsenal[ActiveSlotNumber];
                if (weapon.Type == Type.Laser || weapon.Type == Type.Annihilating)
                {
                    ((LaserTypeWeapon)weapon).StopLaser();
                }
            }
        }
    }

    /// <summary>
    /// РЎРЅРѕРІР° СЂР°Р·СЂРµС€РёС‚СЊ РїРµСЂРµР·Р°СЂСЏРґРєСѓ РѕСЂСѓР¶РёСЏ РїРѕСЃР»Рµ РїСЂРѕС€РµСЃС‚РІРёСЏ РёРЅС‚РµСЂРІР°Р»Р° РІСЂРµРјРµРЅРё
    /// </summary>
    private IEnumerator AllowReloadingAfterIntervalPassing()
    {
        // Р—Р°РґРµСЂР¶РєР° РІ СЂРµР°Р»СЊРЅС‹С… СЃРµРєСѓРЅРґР°С…
        yield return new WaitForSeconds(WeaponsArsenal[ActiveSlotNumber].ReloadingDuration);
        WeaponsArsenal[activeSlotNumber].ReloadWeapon();
        inProcessOfReloading = false;
    }

    /// <summary>
    /// РћСЃС‚Р°РЅРѕРІС‚РёС‚СЊ СЃРѕРїСЂРѕС†РµСЃСЃ, РєРѕС‚РѕСЂС‹Р№ Р±Р»РѕРєРёСЂСѓРµС‚ РѕСЂСѓР¶РёРµ РІРѕ РІСЂРµРјСЏ РїРµСЂРµР·Р°СЂСЏРґРєРё
    /// </summary>
    private void StopWeaponReloadingCoroutine()
    {
        StopCoroutine("AllowReloadingAfterIntervalPassing");
        inProcessOfReloading = false;
    }

    /// <summary>
    /// РџСЂРѕРІРµСЂРєР° РЅР° РїРµСЂРµР·Р°СЂСЏРґРєСѓ Р°РєС‚РёРІРЅРѕРіРѕ РѕСЂСѓР¶РёСЏ РІ СЂСѓРєРµ
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
    /// РџСЂРѕРІРµСЂРєР° РЅР° СЃРјРµРЅСѓ Р°РєС‚РёРІРЅРѕРіРѕ РѕСЂСѓР¶РёСЏ РІ С‚РµРєСѓС‰РµРј РєР°РґСЂРµ
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
    /// РџСЂРѕРІРµСЂРєР° РЅР° РІС‹Р±СЂР°СЃС‹РІР°РЅРёРµ РїСЂРµРґРјРµС‚Р° РёР»Рё РІР·Р°РёРјРѕРґРµР№СЃС‚РІРёРµ СЃ РѕР±СЉРµРєС‚РѕРј РІ С‚РµРєСѓС‰РµРј РєР°РґСЂРµ
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
    /// РЎРјРµРЅРёС‚СЊ Р°РєС‚РёРІРЅРѕРµ РѕСЂСѓР¶РёРµ РЅР° РґСЂСѓРіРѕРµ
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
                WeaponsArsenal[ActiveSlotNumber].transform.parent.parent.gameObject.SetActive(false);
            }

            ActiveSlotNumber = newActiveSlotNumber;

            if (!IsActiveSlotEmpty())
            {
                WeaponsArsenal[ActiveSlotNumber].transform.parent.parent.gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// РџСЂРѕРІРµСЂРёС‚СЊ, СЏРІР»СЏРµС‚СЃСЏ Р»Рё РѕСЂСѓР¶РёРµРј РѕР±СЉРµРєС‚, РЅР° РєРѕС‚РѕСЂС‹Р№ СЃРјРѕС‚СЂРёС‚ РёРіСЂРѕРє, Рё РІРµСЂРЅСѓС‚СЊ РµРіРѕ РІ С‚Р°РєРѕРј СЃР»СѓС‡Р°Рµ
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
    /// РЈСЃС‚Р°РЅРѕРІРёС‚СЊ РѕСЂСѓР¶РёРµ РІ СЃР»РѕС‚, РµСЃР»Рё РѕРЅ РїСѓСЃС‚
    /// </summary>
    private bool TryPutWeaponInSlot(GameObject weapon, int slotNumber)
    {
        if (WeaponsArsenal[slotNumber] != null)
        {
            return false;
        }

        var weaponComp = weapon.GetComponent<Weapon>();
        var typesObj = weapon.transform.Find("WeaponTypes");
        var weaponTypeObj = typesObj.transform.Find(weaponComp.Type.ToString() + "Component");
        WeaponsArsenal[slotNumber] = weaponTypeObj.GetComponent<Weapon>();
        weapon.transform.SetParent(weaponCamera.transform);

        //Надо добавить аудио
        //WeaponsArsenal[slotNumber].PickUpSound.Play();

        var weaponTypeComp = weaponTypeObj.GetComponent<Weapon>();
        var positionInPlayerHand = weaponTypeComp.PositionInPlayerHand;
        weapon.transform.position = positionInPlayerHand.position;
        weapon.transform.rotation = positionInPlayerHand.rotation;
        weapon.transform.localScale = positionInPlayerHand.localScale;
        weapon.GetComponent<Rigidbody>().isKinematic = true;

        weapon.GetComponent<Weapon>().SetUpWeaponPartsLayersAndColliders("Weapons", false);

        return true;
    }

    /// <summary>
    /// Р’С‹Р±СЂРѕСЃРёС‚СЊ Р°РєС‚РёРІРЅРѕРµ РѕСЂСѓР¶РёРµ
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
        GameObject ejectedWeapon = WeaponsArsenal[ActiveSlotNumber].transform.parent.parent.gameObject;
        ejectedWeapon.transform.SetParent(null);
        WeaponsArsenal[ActiveSlotNumber] = null;

        ejectedWeapon.GetComponent<Weapon>().PushOutWeaponFromWall(0f);

        ejectedWeapon.GetComponent<Rigidbody>().isKinematic = false;
        ejectedWeapon.GetComponent<Rigidbody>().AddForce(ejectedWeapon.transform.forward * ejectionForce * TimeScale.SharedInstance.Scale);
        ejectedWeapon.GetComponent<Weapon>().SetUpWeaponPartsLayersAndColliders("Default", true);

        // Р—РґРµСЃСЊ РїСЂРёСЃРІР°РёРІР°РёРІР°РµС‚СЃСЏ СЃРєРѕСЂРѕСЃС‚СЊ, Р±Р»РёР·РєР°СЏ Рє РЅСѓР»СЋ, РґР»СЏ С‚РѕРіРѕ, С‡С‚РѕР±С‹ СѓСЃР»РѕРІРёРµ РІ РєРѕСЂСѓС‚РёРЅРµ РЅРµ СЃСЂР°Р±РѕС‚Р°Р»Рѕ СЂР°РЅСЊС€Рµ РІСЂРµРјРµРЅРё
        // (С‚Р°Рє РєР°Рє СЃРёР»Р° Р±СЂРѕСЃРєР° РїСЂРёРјРµРЅСЏРµС‚СЃСЏ Рє РѕР±СЉРµРєС‚Сѓ С‚РѕР»СЊРєРѕ СЃРѕ СЃР»РµРґСѓСЋС‰РµРіРѕ РєР°РґСЂР°)
        ejectedWeapon.GetComponent<Rigidbody>().velocity = new Vector3(0.01f, 0, 0);
        StartCoroutine(ejectedWeapon.GetComponent<Weapon>().PerformActionsAfterFallOfEjectedWeapon());
    }

    /// <summary>
    /// РџРѕРґРѕР±СЂР°С‚СЊ РѕСЂСѓР¶РёРµ, РЅР° РєРѕС‚РѕСЂРѕРµ СЃРјРѕС‚СЂРёС‚ РёРіСЂРѕРє
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
                // РЈСЃС‚Р°РЅРѕРІРєР° РѕСЂСѓР¶РёСЏ РІ РїРµСЂРІС‹Р№ СЃРІРѕР±РѕРґРЅС‹Р№ СЃР»РѕС‚
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