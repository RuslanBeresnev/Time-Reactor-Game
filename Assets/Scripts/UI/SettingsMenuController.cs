using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Управление событиями из меню настроек
/// </summary>
public class SettingsMenuController : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TextMeshProUGUI volumeValue;

    [SerializeField] private Slider mouseSensitivitySlider;
    [SerializeField] private TextMeshProUGUI mouseSensitivityValue;

    [SerializeField] private Toggle improvedGraphicsToggle;

    private void Start()
    {

        volumeSlider.value = SettingsOptions.GeneralVolume * 100;
        volumeValue.text = (SettingsOptions.GeneralVolume * 100).ToString();

        mouseSensitivitySlider.value = SettingsOptions.MouseSensitivity;
        mouseSensitivityValue.text = SettingsOptions.MouseSensitivity.ToString();

        improvedGraphicsToggle.isOn = SettingsOptions.ImprovedGraphicsIsOn;
    }

    /// <summary>
    /// Изменить значение громкости (при изменении слайдера)
    /// </summary>
    public void ChangeVolumeValue()
    {
        SettingsOptions.GeneralVolume = volumeSlider.value / 100f;
        volumeValue.text = volumeSlider.value.ToString();
    }

    /// <summary>
    /// Изменить значение чувствительности мыши (при изменении слайдера)
    /// </summary>
    public void ChangeMouseSensitivityValue()
    {
        SettingsOptions.MouseSensitivity = mouseSensitivitySlider.value;
        mouseSensitivityValue.text = mouseSensitivitySlider.value.ToString();
    }

    /// <summary>
    /// Изменить тип графики (при изменении состояния флажка)
    /// </summary>
    public void ChangeGraphics()
    {
        SettingsOptions.ImprovedGraphicsIsOn = improvedGraphicsToggle.isOn;
    }

    /// <summary>
    /// Вернуться в главное меню
    /// </summary>
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}