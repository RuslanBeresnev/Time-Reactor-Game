/// <summary>
/// Параметры игровых настроек
/// </summary>
public static class SettingsOptions
{
    /// <summary>
    /// Общая громкость звуков и музыки в игре
    /// </summary>
    public static float GeneralVolume { get; set; } = 1f;

    /// <summary>
    /// Чувствительность мыши
    /// </summary>
    public static float MouseSensitivity { get; set; } = 200;

    /// <summary>
    /// Включена ли улучшенная графика
    /// </summary>
    public static bool ImprovedGraphicsIsOn { get; set; } = true;
}