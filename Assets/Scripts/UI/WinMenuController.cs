using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Управление событиями из меню победы
/// </summary>
public class WinMenuController : MonoBehaviour
{
    /// <summary>
    /// Запустить главное меню
    /// </summary>
    public void ToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    /// <summary>
    /// Выйти из игры
    /// </summary>
    public void QuitFromGame()
    {
        Application.Quit();
    }
}