using UnityEngine;

/// <summary>
/// Реализация объекта, имеющего информацию о себе, которую можно посмотреть через графический анализатор
/// </summary>
public abstract class ObjectWithInformation : MonoBehaviour
{
    protected GameObject infoPanelPrefab;
    protected bool panelWasCreated = false;
    protected GameObject createdPanel = null;

    /// <summary>
    /// Показать панель с информацией
    /// </summary>
    public abstract void ShowInfoPanel();

    /// <summary>
    /// Скрыть панель с информацией
    /// </summary>
    public abstract void HideInfoPanel();

    /// <summary>
    /// Получить панель с информацией об объекте
    /// </summary>
    public GameObject GetInfoPanel()
    {
        return createdPanel;
    }

    /// <summary>
    /// Настроить компонент Transform панели с информацией об объекте
    /// </summary>
    /// <returns>true, если панель успешно настроена, иначе false</returns>
    public bool SetInfoPanelTransform(Vector3 setterPosition, Quaternion setterRotation, Vector3 setterScale)
    {
        if (createdPanel == null)
        {
            return false;
        }

        createdPanel.transform.position = setterPosition;
        createdPanel.transform.rotation = setterRotation;
        createdPanel.transform.localScale = setterScale;
        return true;
    }
}