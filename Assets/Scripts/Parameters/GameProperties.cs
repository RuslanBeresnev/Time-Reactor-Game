using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Игровые параметры и некоторые объекты
/// </summary>
public static class GameProperties
{
    /// <summary>
    /// Этаж, на котором сейчас находится игрок
    /// </summary>
    public static int FloorNumber { get; set; } = 0;

    /// <summary>
    /// Список пройденных игроком этажей
    /// </summary>
    public static List<int> PassedFloors { get; } = new List<int>();

    /// <summary>
    /// Вероятность появления врага
    /// </summary>
    public static float EnemyAppearanceChance { get; set; } = 0.5f;

    /// <summary>
    /// Размер оружейного арсенала игрока
    /// </summary>
    public static int PlayerWeaponsArsenalSize { get; } = 3;

    /// <summary>
    /// Обновить всю игровую статистику
    /// </summary>
    public static void ResetStatistics()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        FloorNumber = 0;
        PassedFloors.Clear();
    }
}