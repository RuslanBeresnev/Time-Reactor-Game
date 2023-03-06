using UnityEngine;
using System.Collections;

/// <summary>
/// Реализация механики течения времени (замена для Time.timeScale)
/// </summary>
public class TimeScale : MonoBehaviour
{
    /// <summary>
    /// Скорость течения времени. Все величины, которые меняются в течение некоторого времени
    /// (например, движение объекта), должны домножаться на TimeScale.Scale, чтобы иметь зависимость
    /// от течения времени
    /// </summary>
    public static float Scale { get; private set; } = 1f;

    /// <summary>
    /// Изменить течение времени и замедлить/ускорить физику объектов
    /// </summary>
    public static void SetTimeScale(float scale)
    {
        Scale = scale;
        // pass
    }

    /// <summary>
    /// Реалиация стандартного WaitForSeconds() для корректной работы при изменении течения времени
    /// с помощью данного класса (Так как Time.timeScale не изменятеся, стандартный WaitForSeconds()
    /// всегда будет отсчитывать некоторое количество секунд РЕАЛЬНОГО времени, что не подходит для корректной работы)
    /// </summary>
    public static IEnumerator WaitForSeconds(float seconds)
    {
        float numberOfSecondsElapsed = 0f;

        while (numberOfSecondsElapsed < seconds)
        {
            // Если, например, Scale = 0.1f (время идёт в 10 раз медленнее), то реального времени должно пройти
            // в 10 раз больше, прежде чем numberOfSecondsElapsed станет >= seconds
            numberOfSecondsElapsed += Time.deltaTime * Scale; // Здесь использовать только Time.deltaTime, а не Time.fixedDeltaTime
            yield return null;
        }
    }
}