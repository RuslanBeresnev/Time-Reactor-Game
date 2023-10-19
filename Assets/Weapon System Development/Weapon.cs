using System.Collections;
using UnityEngine;
using TMPro;

public class Weapon : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public float hitForce = 10f;
    public float fireRate = 5f;

    public int maxAmmo = 10;
    public int ammoInReserve = 100;
    private int currentAmmo;
    public float reloadTime = 1f;
    private bool isReloading = false;
    private float nextTimeToFire = 0f;

    public Camera weaponCamera;
    public TextMeshProUGUI textAmmo;

    void Start()
    {
        currentAmmo = maxAmmo;
        RedrawAmmoText();
    }

    void OnEnable()
    {
        isReloading = false;
        RedrawAmmoText();
    }

    void Update()
    {
        if (isReloading)
            return;

        if (currentAmmo <= 0 || Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            Shoot();
        }
    }

    IEnumerator Reload()
    {
        if (!CanReload())
            yield break; //test this

        isReloading = true;

        yield return new WaitForSeconds(reloadTime);

        int difference = maxAmmo - currentAmmo;

        if (ammoInReserve >= difference)
        {
            currentAmmo += difference;
            ammoInReserve -= difference;
        }
        else if (ammoInReserve > 0)
        {
            currentAmmo = ammoInReserve;
            ammoInReserve = 0;
        }

        RedrawAmmoText();
        isReloading = false;
    }

    private void RedrawAmmoText()
    {
        textAmmo.text = currentAmmo + "/" + ammoInReserve;
    }

    private bool CanShoot()
    {
        return currentAmmo > 0 && Time.time >= nextTimeToFire;
    }

    private bool CanReload()
    {
        return ammoInReserve > 0 && currentAmmo != maxAmmo;
    }


    public void Shoot()
    {
        if (!CanShoot())
            return;
        
        nextTimeToFire = Time.time + 1f / fireRate;

        currentAmmo--;
        RedrawAmmoText();


        Debug.Log("Shoot");
        RaycastHit hit;
        if (Physics.Raycast(weaponCamera.transform.position,
            weaponCamera.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            Entity target = hit.transform.GetComponent<Entity>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }

            Debug.Log(hit.rigidbody.ToString());
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * hitForce);
            }

            Debug.Log(hit.point.ToString());
            //GameObject hitGameObj = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            //Destroy(hitGameObj, 2f);
        }
    }
}
