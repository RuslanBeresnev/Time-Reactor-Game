using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Управление событиями из меню паузы в игре
/// </summary>
public class PauseMenuController : MonoBehaviour
{
    [SerializeField]
    private GameObject elements;

    private bool gameOnPause = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!gameOnPause)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }
    }

    /// <summary>
    /// Действия при остановке игры на паузу
    /// </summary>
    public void Pause()
    {
        elements.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.Confined;
        AudioListener.pause = true;
        gameOnPause = true;
    }

    /// <summary>
    /// Действия при продолжении игры после паузы
    /// </summary>
    public void Resume()
    {
        elements.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        AudioListener.pause = false;
        gameOnPause = false;
    }

    /// <summary>
    /// Запустить главное меню
    /// </summary>
    public void ToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
        GameProperties.ResetStatistics();
    }

    /// <summary>
    /// Выйти из игры
    /// </summary>
    public void QuitFromGame()
    {
        Application.Quit();
    }
}