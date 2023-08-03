using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Реализация шкалы выносливости
/// </summary>
public class StaminaBarController : MonoBehaviour
{
    [SerializeField] private StaminaController staminaController;
    [SerializeField] private Image staminaBar;
    [SerializeField] private Image staminaBarBackground;

    [SerializeField] private Sprite barSpriteForNormalState;
    [SerializeField] private Sprite barBackgroundSpriteForNormalState;
    [SerializeField] private Sprite barSpriteForTiredState;
    [SerializeField] private Sprite barBackgroundSpriteForTiredState;

    /// <summary>
    /// Перерисовать шкалу выносливости
    /// </summary>
    public void RedrawStaminaBar(float stamina)
    {
        staminaBar.fillAmount = staminaController.Stamina / staminaController.MaxStamina;
    }

    /// <summary>
    /// Показать шкалу выносливости
    /// </summary>
    public void ShowStaminaBar()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Скрыть шкалу выносливости
    /// </summary>
    public void HideStaminaBar()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Изменить стиль шкалы выносливости на обычный стиль
    /// </summary>
    public void SwapBarToNormalState()
    {
        staminaBar.sprite = barSpriteForNormalState;
        staminaBarBackground.sprite = barBackgroundSpriteForNormalState;
    }

    /// <summary>
    /// Изменить стиль шкалы выносливости на стиль во время истощения игрока
    /// </summary>
    public void SwapBarToTiredState()
    {
        staminaBar.sprite = barSpriteForTiredState;
        staminaBarBackground.sprite = barBackgroundSpriteForTiredState;
    }
}