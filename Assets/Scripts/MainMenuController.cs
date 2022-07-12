using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Управление событиями из главного меню
/// </summary>
public class MainMenuController : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI probabilityText;

    /// <summary>
    /// Отобразить текущую вероятность появления врага со слайдера
    /// </summary>
    public void DisplayProbabilityFromSlider()
    {
        probabilityText.text = slider.value.ToString() + " %";
    }

    /// <summary>
    /// Запустить игровую сцену
    /// </summary>
    public void StartGame()
    {
        GameProperties.EnemyAppearanceChance = slider.value / 100f;
        SceneManager.LoadScene("Game");
    }

    /// <summary>
    /// Открыть меню настроек
    /// </summary>
    public void OpenSettingsMenu()
    {
        SceneManager.LoadScene("Settings Menu");
    }

    /// <summary>
    /// Выйти из игры
    /// </summary>
    public void QuitFromGame()
    {
        Application.Quit();
    }
}