using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTypeWeapon : Weapon
{
    [HideInInspector][SerializeField] private float laserDamage;
    [HideInInspector][SerializeField] private float laserWidth = 0.2f;
    [HideInInspector][SerializeField] private Color laserColor;
    [HideInInspector][SerializeField] private Material laserMaterial;
    protected GameObject laserGO;

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

    public override void Shoot()
    {
        ShotSound.Play();

        RaycastHit direction = new RaycastHit();
        if (GetRaycastHit(ref direction))
        {
            Fire(direction);
        }
    }

    protected virtual void Fire(RaycastHit hit)
    {
        MakeLaser(hit);
    }

    /// <summary>
    /// Создать лазер
    /// </summary>
    protected void MakeLaser(RaycastHit hit)
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

        lineRenderer.SetPosition(0, WeaponEnd.position);
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

    protected override void Awake()
    {
        base.Awake();

        SemiAutoShooting = false;

        ObjectInfoParameters = new string[5, 2] { { "Name:", Name },
                                                  { "Shooting type:", SemiAutoShooting ? "Semi-Automatic" : "Automatic" },
                                                  { "Firing Frequency:", "N/A" },
                                                  { "Bullet velocity:", "N/A" },
                                                  { "Damage:", "Full HP" } };
    }
}
