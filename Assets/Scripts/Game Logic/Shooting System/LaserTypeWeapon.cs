﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LaserTypeWeapon : Weapon
{
    protected GameObject laserGO;

    /// <summary>
    /// Материал лазера
    /// </summary>
    [field: HideInInspector][field: SerializeField] public Material LaserMaterial { get; set; }

    /// <summary>
    /// Цвет лазера
    /// </summary>
    [field: HideInInspector][field: SerializeField] public Color LaserColor {  get; set; }

    /// <summary>
    /// Ширина лазера
    /// </summary>
    [field: HideInInspector][field: SerializeField] public float LaserWidth { get; set; }

    /// <summary>
    /// Урон от лазера
    /// </summary>
    [field: HideInInspector][field: SerializeField] public float LaserDamage { get; set; }

    protected override void RedrawAmmoScreen()
    {
        AmmoScreen.text =  "inf / inf";
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
            lineRendererComponent.material = LaserMaterial;
            lineRendererComponent.material.SetColor("_Color", LaserColor);
            lineRendererComponent.startWidth = LaserWidth;
            lineRendererComponent.endWidth = LaserWidth;
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
}
