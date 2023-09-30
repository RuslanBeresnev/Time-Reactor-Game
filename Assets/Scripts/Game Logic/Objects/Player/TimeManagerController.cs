using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;

/// <summary>
/// Реализация управления временем (главная механика в игре)
/// </summary>
public class TimeManagerController : MonoBehaviour
{
    // Коэффициент замедления времени на экране
    [SerializeField] private TextMeshProUGUI TSFCounter;
    [SerializeField] private Image timeSlowdownTimer;

    private float timeSlowdownFactor = 1f;
    // Длительность способности замедлять время
    [SerializeField] private float slowdownDuration;
    // Время перезарядки способности замедлять время (в секундах)
    [SerializeField] private float abilityCooldown;

    private bool abilityCanBeUsed = true;

    /// <summary>
    /// Во сколько раз может быть замедлено время (коэффициент будет расти от 1 до 20)
    /// </summary>
    public float TimeSlowdownFactor
    {
        get { return timeSlowdownFactor; }
        set
        {
            timeSlowdownFactor = (float)Math.Round(value, 2);
            TSFCounter.text = value.ToString();
        }
    }

    /// <summary>
    /// Общий экземпляр класса для других классов
    /// </summary>
    public static TimeManagerController SharedInstance { get; private set; }

    private void Awake()
    {
        SharedInstance = this;
    }

    private void FixedUpdate()
    {
        CheckForAbilityUsing();
    }

    /// <summary>
    /// Отслеживание нажатия клавиши мыши для распознания использования способности
    /// </summary>
    private void CheckForAbilityUsing()
    {
        if (Input.GetMouseButtonDown(1) && abilityCanBeUsed)
        {
            SlowdownTime();
        }
    }

    /// <summary>
    /// Замедлить время на несколько секунд
    /// </summary>
    private void SlowdownTime()
    {
        if (TimeSlowdownFactor == 1f)
        {
            return;
        }

        TimeScale.SharedInstance.SetTimeScale(1 / TimeSlowdownFactor);
        abilityCanBeUsed = false;
        StartCoroutine(RevertToStandardTimescaleAfterAbilityPassing());
        StartCoroutine(DisplayTimeSlowdownTimer());
    }

    /// <summary>
    /// По прошествии времени действия способности вернуть стандартное течение времени
    /// </summary>
    private IEnumerator RevertToStandardTimescaleAfterAbilityPassing()
    {
        yield return new WaitForSeconds(slowdownDuration);
        TimeScale.SharedInstance.SetTimeScale(1f);
        StartCoroutine(ImplementCooldownMechanic());
    }

    /// <summary>
    /// Произвести кулдаун перед возможностью снова использовать способность
    /// </summary>
    /// <returns></returns>
    private IEnumerator ImplementCooldownMechanic()
    {
        yield return new WaitForSeconds(abilityCooldown);
        abilityCanBeUsed = true;
    }

    /// <summary>
    /// Отобразить истекающий круговой таймер во время замедления времени
    /// </summary>
    private IEnumerator DisplayTimeSlowdownTimer()
    {
        float timeLeft = slowdownDuration;

        while (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;
            timeSlowdownTimer.fillAmount = timeLeft / slowdownDuration;
            yield return null;
        }
    }
}