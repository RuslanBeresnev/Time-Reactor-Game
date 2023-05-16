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
    public static List<int> PassedFloors { get; } = new();

    /// <summary>
    /// Список, который определяет наличие на каждом этаже двери со сгенерированной комнатой
    /// </summary>
    public static List<bool> DoorOnFloor { get; } = new();

    /// <summary>
    /// Словарь со всеми сгенерированными комнатами, где ключ - номер этажа, значение - комната на этом этаже
    /// </summary>
    public static Dictionary<int, GameObject> GeneratedRooms { get; } = new();

    /// <summary>
    /// Размер оружейного арсенала игрока
    /// </summary>
    public static int PlayerWeaponsArsenalSize { get; } = 3;

    /// <summary>
    /// Общий пул в виде словаря, где ключ - это название пула объектов, а значение - сам пул объектов
    /// </summary>
    public static Dictionary<string, Pool> GeneralPool { get; } = new();

    /// <summary>
    /// Обновить всю игровую статистику
    /// </summary>
    public static void ResetStatistics()
    {
        TimeScale.SharedInstance.SetTimeScale(1f);
        Time.timeScale = 1f;
        AudioListener.pause = false;

        FloorNumber = 0;
        PassedFloors.Clear();
        DoorOnFloor.Clear();
        GeneratedRooms.Clear();
        GeneralPool.Clear();

        GraphicAnalyzerController.AnalyzerIsActive = false;
        GraphicAnalyzerController.StateChanged = null;
    }
}