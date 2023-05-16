using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Управление событиями из главного меню
/// </summary>
public class MainMenuController : MonoBehaviour
{
    /// <summary>
    /// Запустить игровую сцену
    /// </summary>
    public void StartGame()
    {
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