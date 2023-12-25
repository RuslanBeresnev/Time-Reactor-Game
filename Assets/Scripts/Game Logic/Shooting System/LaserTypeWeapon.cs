using UnityEngine;

/// <summary>
/// Оружие лазерного типа
/// </summary>
public abstract class LaserTypeWeapon : Weapon
{
    private GameObject laserGO;

    [SerializeField, HideInInspector] private Material laserMaterial;
    [SerializeField, HideInInspector] private float laserWidth;

    /// <summary>
    /// Материал лазера
    /// </summary>
    public Material LaserMaterial 
    { 
        get => laserMaterial; 
        set => laserMaterial = value; 
    }

    /// <summary>
    /// Ширина лазера
    /// </summary>
    public float LaserWidth
    { 
        get => laserWidth; 
        set => laserWidth = value; 
    }

    protected override void RedrawAmmoScreen()
    {
        AmmoScreen.text =  "inf / inf";
    }

    /// <summary>
    /// Создать лазер
    /// </summary>
    protected void MakeLaser(RaycastHit hit)
    {
        if (!ShotSound.isPlaying)
        {
            ShotSound.Play();
        }

        if (laserGO == null)
        {
            laserGO = new GameObject("laserGO", typeof(LineRenderer));
            laserGO.transform.parent = transform;

            LineRenderer lineRendererComponent = laserGO.GetComponent<LineRenderer>();
            lineRendererComponent.material = LaserMaterial;
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
        ShotSound.Stop();
    }
}
