using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// »гровые параметры
/// </summary>
public static class GameProperties
{
    /// <summary>
    /// Ётаж, на котором сейчас находитс€ игрок
    /// </summary>
    public static int FloorNumber { get; set; } = 0;

    /// <summary>
    /// —писок пройденных игроком этажей
    /// </summary>
    public static List<int> PassedFloors { get; } = new List<int>();

    /// <summary>
    /// ¬еро€тность по€влени€ врага
    /// </summary>
    public static float EnemyAppearanceChance { get; set; } = 0.5f;

    /// <summary>
    /// –азмер оружейного арсенала игрока
    /// </summary>
    public static int PlayerWeaponsArsenalSize { get; } = 3;

    /// <summary>
    /// ќбновить всю игровую статистику
    /// </summary>
    public static void ResetStatistics()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        FloorNumber = 0;
        PassedFloors.Clear();
    }
}