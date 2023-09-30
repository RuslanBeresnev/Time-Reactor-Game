using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

/// <summary>
/// Реализация шкалы здоровья у каждой сущности
/// </summary>
public class HealthBarController : MonoBehaviour
{
    [SerializeField] private Entity entity;
    [SerializeField] private Image healthBar;
    [SerializeField] private TextMeshProUGUI healthCount;

    private void Awake()
    {
        entity.HealthChanged += RedrawHealthInfo;
        GraphicAnalyzerController.StateChanged += ShowOrHideHealthBar;
        ShowOrHideHealthBar(GraphicAnalyzerController.AnalyzerIsActive);
    }

    private void FixedUpdate()
    {
        SetRotationOfHealthBar();
    }

    private void OnDestroy()
    {
        GraphicAnalyzerController.StateChanged -= ShowOrHideHealthBar;
    }

    /// <summary>
    /// Перерисовать полоску и изменить выводимое значение здоровья сущности
    /// </summary>
    private void RedrawHealthInfo(float health)
    {
        healthBar.fillAmount = health / entity.MaxHealth;
        healthCount.text = Math.Ceiling(health).ToString() + " HP";
    }

    /// <summary>
    /// Повернуть полосу здоровья сущности так, чтобы она была параллельна плоскости камеры игрока
    /// </summary>
    private void SetRotationOfHealthBar()
    {
        transform.rotation = Camera.main.transform.rotation;
    }

    /// <summary>
    /// Показать или скрыть шкалу здоровья сущности в зависимости от режима графического анализатора игрока
    /// </summary>
    private void ShowOrHideHealthBar(bool graphicAnalyzerIsActive)
    {
        if (entity.name != "Player")
        {
            gameObject.SetActive(graphicAnalyzerIsActive);
        }
    }
}