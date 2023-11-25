using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBuilder : Weapon
{
    /// <summary>
    /// Префаб стены для постройки
    /// </summary>
    [field: HideInInspector][field:SerializeField] public GameObject WallPrefab { get; set; }

    protected override void RedrawAmmoScreen()
    {
        AmmoScreen.text = "inf / inf";
    }

    public override void Shoot()
    {
        //ShotSound.Play();

        RaycastHit hit = GetRaycastHit();
        if (hit.collider != null)
        {
            BuildWall(hit);
        }
    }

    /// <summary>
    /// Построить стену при выстреле
    /// </summary>
    private void BuildWall(RaycastHit hit)
    {
        //Поворот барьера к игроку основной стороной 
        float yRotation = gameObject.transform.rotation.eulerAngles.y;
        Quaternion rotation = Quaternion.Euler(0, yRotation, 0);

        //Отступ, чтобы барьер ставился точно на поверхность
        var renderer = WallPrefab.GetComponent<Renderer>();
        float yOffset = renderer.bounds.extents.y * hit.normal.y;

        Vector3 position = new Vector3(hit.point.x, hit.point.y + yOffset, hit.point.z);
        var wallGO = Instantiate(WallPrefab, position, rotation);

        //Навесить на стену тэг, чтобы она разрушалась
        //при стрельбе из аннигилирующего оружия
        wallGO.tag = "Annihil";
    }

    private void Awake()
    {
        Type = Type.WallBuilder;

        var weapon = transform.parent.parent.GetComponent<Weapon>();
        if (Type != weapon.Type)
        {
            return;
        }

        RedrawAmmoScreen();

        foreach (var audioSource in GetComponents<AudioSource>())
        {
            var clipName = audioSource.clip.name;
            if (clipName.EndsWith("Shot"))
            {
                ShotSound = audioSource;
            }
            else if (clipName.EndsWith("Reloading"))
            {
                ReloadingSound = audioSource;
            }
            else if (clipName.StartsWith("Weapon Hitting On Surface"))
            {
                WeaponHitingOnSurfaceSounds.Add(audioSource);
            }
            else if (clipName == "Weapon Picking Up")
            {
                PickUpSound = audioSource;
            }
        }

        InitializeInfoPanelPrefab();

        ObjectInfoParameters = new string[5, 2] { { "Name:", Name },
                                              { "Shooting type:", "Semi-Automatic"},
                                              { "Firing Frequency:", "N/A" },
                                              { "Bullet velocity:", "N/A" },
                                              { "Damage:", "N/A" } };
    }
}
