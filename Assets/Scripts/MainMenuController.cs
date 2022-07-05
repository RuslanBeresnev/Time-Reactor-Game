using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Управление событиями из стартового меню
/// </summary>
public class MainMenuController : MonoBehaviour
{
    public TMP_InputField chanceInputField;

    /// <summary>
    /// Запустить игровую сцену
    /// </summary>
    public void StartGame()
    {
        if (chanceInputField.text != "")
        {
            GameProperties.AppearanceChance = float.Parse(chanceInputField.text) / 100f;
        }
        SceneManager.LoadScene("Game");
    }

    /// <summary>
    /// Выйти из игры
    /// </summary>
    public void QuitFromGame()
    {
        Application.Quit();
    }
}