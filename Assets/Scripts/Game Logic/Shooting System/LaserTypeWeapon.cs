using UnityEngine;

public abstract class LaserTypeWeapon : Weapon
{
    protected GameObject laserGO;

    /// <summary>
    /// Префаб лазера
    /// </summary>
    [field: HideInInspector][field: SerializeField] public GameObject LaserPrefab { get; set; }

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
            laserGO = Instantiate(LaserPrefab);
        }

        var direction = (hit.point - WeaponEnd.position).normalized;
        var rotation = Quaternion.LookRotation(direction);
        var length = Vector3.Distance(hit.point, WeaponEnd.position);

        var laserTransf = laserGO.transform;
        laserTransf.localScale = new Vector3(laserTransf.localScale.x, 
            laserTransf.localScale.y, length);
        laserTransf.position = WeaponEnd.position;
        laserTransf.rotation = rotation;

        laserGO.SetActive(true);
    }

    /// <summary>
    /// Прекратить испускание лазера
    /// </summary>
    public void StopLaser()
    {
        if (laserGO != null)
        {
            laserGO.SetActive(false);
        }
    }
}
