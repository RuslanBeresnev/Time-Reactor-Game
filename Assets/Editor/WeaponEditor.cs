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
    private bool showReferences = false;
    private bool showShooting = false;
    private bool showVisual = false;
    private bool showAudio = false;
    private bool showAudioList = false;

    public override void OnInspectorGUI()
    {
        Weapon weapon = (Weapon)target;
        Type type = weapon.Type;


        weapon.Type = (Type)EditorGUILayout.EnumPopup("Type", weapon.Type);

        showGeneral = EditorGUILayout.Foldout(showGeneral, "General");
        if (showGeneral)
        {
            weapon.Name = EditorGUILayout.TextField("Name", weapon.Name);
        }

        showReferences = EditorGUILayout.Foldout(showReferences, "References");
        if (showReferences)
        {
            weapon.PositionInPlayerHand = (Transform)EditorGUILayout.ObjectField("Position in player hand",
                weapon.PositionInPlayerHand, typeof(Transform), true);
            
            weapon.WeaponStart = (Transform)EditorGUILayout.ObjectField("Weapon start",
                weapon.WeaponStart, typeof(Transform), true);

            weapon.WeaponEnd = (Transform)EditorGUILayout.ObjectField("Weapon end",
                weapon.WeaponEnd, typeof(Transform), true);

            if (type == Type.Firearm || type == Type.RPG)
            {
                weapon.BulletPrefab = (GameObject)EditorGUILayout.ObjectField("Bullet prefab",
                weapon.BulletPrefab, typeof(GameObject), true);

                weapon.Pool = (Pool)EditorGUILayout.ObjectField("Pool",
                    weapon.Pool, typeof(Pool), true);
            }

            if (type == Type.Laser)
            {
            }
        }

        showShooting = EditorGUILayout.Foldout(showShooting, "Shooting");
        if (showShooting)
        {
            if (type == Type.Firearm)
            {
                weapon.SemiAutoShooting = EditorGUILayout.Toggle("Semi-shooting", weapon.SemiAutoShooting);
            }

            if (type == Type.Firearm || type == Type.RPG || type == Type.Laser)
            {
                weapon.IntervalBetweenShoots = EditorGUILayout.FloatField("Interval between shots",
                    weapon.IntervalBetweenShoots);

                //@@for different types
                weapon.MagazinCapacity = EditorGUILayout.IntField("Magazine capacity", weapon.MagazinCapacity);
                weapon.BulletsCountInMagazine = EditorGUILayout.IntField("Bullets in magazine", weapon.BulletsCountInMagazine);
                weapon.BulletsCountInReserve = EditorGUILayout.IntField("Bullets in reserve", weapon.BulletsCountInReserve);
            }

            if (type == Type.Laser || type == Type.Annihilating)
            {
                weapon.LaserWidth = EditorGUILayout.FloatField("Laser width",
                    weapon.LaserWidth);
                weapon.LaserMaterial = (Material)EditorGUILayout.ObjectField("Laser material", 
                    weapon.LaserMaterial, typeof(Material), true);
                weapon.LaserColor = EditorGUILayout.ColorField("Laser color", 
                    weapon.LaserColor);
            }

            if (type == Type.Laser)
            {
                weapon.LaserDamage = EditorGUILayout.FloatField("Laser damage",
                    weapon.LaserDamage);
            }

            if (type == Type.Annihilating)
            {
                weapon.AnnihilatingTag = EditorGUILayout.TagField("Tag: ", weapon.AnnihilatingTag);
            }

            if (type == Type.Wall)
            {
                weapon.WallPrefab = (GameObject)EditorGUILayout.ObjectField("Wall prefab", 
                    weapon.WallPrefab, typeof(GameObject), true);
            }

            weapon.ReloadingDuration = EditorGUILayout.FloatField("Reloading duration (s)", weapon.ReloadingDuration);
            
            weapon.RayDistance = EditorGUILayout.FloatField("Ray distance", weapon.RayDistance);
        }

        showVisual = EditorGUILayout.Foldout(showVisual, "Visual");
        if (showVisual)
        {
            weapon.AmmoScreen = (TextMeshProUGUI)EditorGUILayout.ObjectField("Ammo screen",
                weapon.AmmoScreen, typeof(TextMeshProUGUI), true);

            weapon.Sprite = (Sprite)EditorGUILayout.ObjectField("Sprite",
                weapon.Sprite, typeof(Sprite), true);
        }

        showAudio = EditorGUILayout.Foldout(showAudio, "Audio");
        if (showAudio)
        {
            weapon.ShotSound = (AudioSource)EditorGUILayout.ObjectField("Shot sound",
                weapon.ShotSound, typeof(AudioSource), true);

            weapon.ReloadingSound = (AudioSource)EditorGUILayout.ObjectField("Reloading sound",
                weapon.ReloadingSound, typeof(AudioSource), true);

            EditorGUI.indentLevel++;
            showAudioList = EditorGUILayout.Foldout(showAudioList, "WeaponHittingOnSurfaceSounds");
            if (showAudioList)
            {
                EditorGUI.indentLevel++;
                List<AudioSource> list = weapon.WeaponHitingOnSurfaceSounds;
                int size = Mathf.Max(0, EditorGUILayout.IntField("Size", list.Count));

                while (size > list.Count)
                    list.Add(null);
                while (size < list.Count)
                    list.RemoveAt(list.Count - 1);

                for (int i = 0; i < list.Count; i++)
                {
                    list[i] = (AudioSource)EditorGUILayout.ObjectField("Audio " + i,
                        list[i], typeof(AudioSource), true);
                }
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
        }


        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
