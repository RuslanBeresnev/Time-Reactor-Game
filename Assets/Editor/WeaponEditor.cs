using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

/// <summary>
/// Скрипт, создающий кастомный редактор для Weapon.cs
/// </summary>
[CustomEditor(typeof(Weapon))]
public class WeaponEditor : Editor
{
    private bool showGeneral = false;
    private bool showShooting = false;
    private bool showVisual = false;
    private bool showAudio = false;
    private bool showAudioList = false;

    public override void OnInspectorGUI()
    {
        Weapon weapon = (Weapon)target;
        Type type = weapon.Type;

        weapon.Type = (Type)EditorGUILayout.EnumPopup("Type", weapon.Type);

        if (type == Type.Projectile)
        {
            var projectileWeapon = weapon.GetComponentInChildren<ProjectileWeapon>();
            DrawEditorProjectile(projectileWeapon);
        } 
        else if (type == Type.Laser)
        {
            var laserWeapon = weapon.GetComponentInChildren<LaserWeapon>();
            DrawEditorLaser(laserWeapon);
        }
        else if (type == Type.Annihilating)
        {
            var annihilatingWeapon = weapon.GetComponentInChildren<AnnihilatingWeapon>();
            DrawEditorAnnihilating(annihilatingWeapon);
        }
        else if (type == Type.Wall)
        {
            var wall = weapon.GetComponentInChildren<WallBuilder>();
            DrawEditorWall(wall);
        }

        //showVisual = EditorGUILayout.Foldout(showVisual, "Visual");
        //if (showVisual)
        //{
        //    weapon.AmmoScreen = (TextMeshProUGUI)EditorGUILayout.ObjectField("Ammo screen",
        //        weapon.AmmoScreen, typeof(TextMeshProUGUI), true);

        //    weapon.Sprite = (Sprite)EditorGUILayout.ObjectField("Sprite",
        //        weapon.Sprite, typeof(Sprite), true);
        //}

        //// Ненужно? (добавляются компоненты)
        //showAudio = EditorGUILayout.Foldout(showAudio, "Audio");
        //if (showAudio)
        //{
        //    weapon.ShotSound = (AudioSource)EditorGUILayout.ObjectField("Shot sound",
        //        weapon.ShotSound, typeof(AudioSource), true);

        //    weapon.ReloadingSound = (AudioSource)EditorGUILayout.ObjectField("Reloading sound",
        //        weapon.ReloadingSound, typeof(AudioSource), true);

        //    EditorGUI.indentLevel++;
        //    showAudioList = EditorGUILayout.Foldout(showAudioList, "WeaponHittingOnSurfaceSounds");
        //    if (showAudioList)
        //    {
        //        EditorGUI.indentLevel++;
        //        List<AudioSource> list = weapon.WeaponHitingOnSurfaceSounds;
        //        int size = Mathf.Max(0, EditorGUILayout.IntField("Size", list.Count));

        //        while (size > list.Count)
        //            list.Add(null);
        //        while (size < list.Count)
        //            list.RemoveAt(list.Count - 1);

        //        for (int i = 0; i < list.Count; i++)
        //        {
        //            list[i] = (AudioSource)EditorGUILayout.ObjectField("Audio " + i,
        //                list[i], typeof(AudioSource), true);
        //        }
        //        EditorGUI.indentLevel--;
        //    }
        //    EditorGUI.indentLevel--;
        //}


        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }

    /// <summary>
    /// Отрисовать инспектор для типа ProjectileWeapon
    /// </summary>
    private void DrawEditorProjectile(ProjectileWeapon weapon)
    {
        showGeneral = EditorGUILayout.Foldout(showGeneral, "General");
        if (showGeneral)
        {
            weapon.Name = EditorGUILayout.TextField("Name", weapon.Name);

            weapon.PositionInPlayerHand = (Transform)EditorGUILayout.ObjectField("Position in player hand",
                weapon.PositionInPlayerHand, typeof(Transform), true);

            weapon.WeaponStart = (Transform)EditorGUILayout.ObjectField("Weapon start",
                weapon.WeaponStart, typeof(Transform), true);

            weapon.WeaponEnd = (Transform)EditorGUILayout.ObjectField("Weapon end",
                weapon.WeaponEnd, typeof(Transform), true);

            weapon.RayDistance = EditorGUILayout.FloatField("Ray distance", weapon.RayDistance);
        }

        showShooting = EditorGUILayout.Foldout(showShooting, "Shooting");
        if (showShooting)
        {
            weapon.ProjectilePrefab = (GameObject)EditorGUILayout.ObjectField("Projectile prefab",
            weapon.ProjectilePrefab, typeof(GameObject), true);

            weapon.Pool = (Pool)EditorGUILayout.ObjectField("Pool",
                weapon.Pool, typeof(Pool), true);

            weapon.SemiAutoShooting = EditorGUILayout.Toggle("Semi-shooting", weapon.SemiAutoShooting);

            weapon.IntervalBetweenShoots = EditorGUILayout.FloatField("Interval between shots",
                weapon.IntervalBetweenShoots);

            weapon.MagazinCapacity = EditorGUILayout.IntField("Magazine capacity", weapon.MagazinCapacity);
            weapon.BulletsCountInMagazine = EditorGUILayout.IntField("Bullets in magazine", weapon.BulletsCountInMagazine);
            weapon.BulletsCountInReserve = EditorGUILayout.IntField("Bullets in reserve", weapon.BulletsCountInReserve);

            weapon.ReloadingDuration = EditorGUILayout.FloatField("Reloading duration (s)", weapon.ReloadingDuration);
        }
    }

    /// <summary>
    /// Отрисовать инспектор для типа LaserWeapon
    /// </summary>
    private void DrawEditorLaser(LaserWeapon weapon)
    {
        showGeneral = EditorGUILayout.Foldout(showGeneral, "General");
        if (showGeneral)
        {
            weapon.Name = EditorGUILayout.TextField("Name", weapon.Name);

            weapon.PositionInPlayerHand = (Transform)EditorGUILayout.ObjectField("Position in player hand",
                weapon.PositionInPlayerHand, typeof(Transform), true);

            weapon.WeaponStart = (Transform)EditorGUILayout.ObjectField("Weapon start",
                weapon.WeaponStart, typeof(Transform), true);

            weapon.WeaponEnd = (Transform)EditorGUILayout.ObjectField("Weapon end",
                weapon.WeaponEnd, typeof(Transform), true);

            weapon.RayDistance = EditorGUILayout.FloatField("Ray distance", weapon.RayDistance);
        }

        showShooting = EditorGUILayout.Foldout(showShooting, "Shooting");
        if (showShooting)
        {
            weapon.LaserWidth = EditorGUILayout.FloatField("Laser width",
                    weapon.LaserWidth);
            weapon.LaserMaterial = (Material)EditorGUILayout.ObjectField("Laser material",
                weapon.LaserMaterial, typeof(Material), true);
            weapon.LaserColor = EditorGUILayout.ColorField("Laser color",
                weapon.LaserColor);

            weapon.LaserDamage = EditorGUILayout.FloatField("Laser damage",
                weapon.LaserDamage);
        }
    }

    /// <summary>
    /// Отрисовать инспектор для типа AnnihilatingWeapon
    /// </summary>
    private void DrawEditorAnnihilating(AnnihilatingWeapon weapon)
    {
        showGeneral = EditorGUILayout.Foldout(showGeneral, "General");
        if (showGeneral)
        {
            weapon.Name = EditorGUILayout.TextField("Name", weapon.Name);

            weapon.PositionInPlayerHand = (Transform)EditorGUILayout.ObjectField("Position in player hand",
                weapon.PositionInPlayerHand, typeof(Transform), true);

            weapon.WeaponStart = (Transform)EditorGUILayout.ObjectField("Weapon start",
                weapon.WeaponStart, typeof(Transform), true);

            weapon.WeaponEnd = (Transform)EditorGUILayout.ObjectField("Weapon end",
                weapon.WeaponEnd, typeof(Transform), true);

            weapon.RayDistance = EditorGUILayout.FloatField("Ray distance", weapon.RayDistance);
        }

        showShooting = EditorGUILayout.Foldout(showShooting, "Shooting");
        if (showShooting)
        {
            weapon.LaserWidth = EditorGUILayout.FloatField("Laser width",
                    weapon.LaserWidth);
            weapon.LaserMaterial = (Material)EditorGUILayout.ObjectField("Laser material",
                weapon.LaserMaterial, typeof(Material), true);
            weapon.LaserColor = EditorGUILayout.ColorField("Laser color",
                weapon.LaserColor);

            weapon.AnnihilatingTag = EditorGUILayout.TagField("Annihilating tag", 
                weapon.AnnihilatingTag);
        }
    }

    /// <summary>
    /// Отрисовать инспектор для типа WallBuilder
    /// </summary>
    private void DrawEditorWall(WallBuilder weapon)
    {
        showGeneral = EditorGUILayout.Foldout(showGeneral, "General");
        if (showGeneral)
        {
            weapon.Name = EditorGUILayout.TextField("Name", weapon.Name);

            weapon.PositionInPlayerHand = (Transform)EditorGUILayout.ObjectField("Position in player hand",
                weapon.PositionInPlayerHand, typeof(Transform), true);

            weapon.WeaponStart = (Transform)EditorGUILayout.ObjectField("Weapon start",
                weapon.WeaponStart, typeof(Transform), true);

            weapon.WeaponEnd = (Transform)EditorGUILayout.ObjectField("Weapon end",
                weapon.WeaponEnd, typeof(Transform), true);

            weapon.RayDistance = EditorGUILayout.FloatField("Ray distance", weapon.RayDistance);
        }

        showShooting = EditorGUILayout.Foldout(showShooting, "Shooting");
        if (showShooting)
        {
            weapon.SemiAutoShooting = EditorGUILayout.Toggle("SemiAuto shooting",
                weapon.SemiAutoShooting);

            weapon.WallPrefab = (GameObject)EditorGUILayout.ObjectField("Wall prefab",
                weapon.WallPrefab, typeof(GameObject), true);
        }
    }
}
