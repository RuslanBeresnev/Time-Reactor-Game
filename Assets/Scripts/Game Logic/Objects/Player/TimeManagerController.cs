using UnityEngine;
using System.Collections;

/// <summary>
/// –еализаци€ управлени€ временем (главна€ механика в игре)
/// </summary>
public class TimeManagerController : MonoBehaviour
{
    // ¬о сколько раз может быть замедлено врем€ (коэффициент будет расти от 1 до 20)
    [SerializeField] private float timeSlowdownFactor;
    // ƒлительность способности замедл€ть врем€
    [SerializeField] private float slowdownDuration;
    // ¬рем€ перезар€дки способности замедл€ть врем€ (в секундах)
    [SerializeField] private float abilityCooldown;

    private float standardFixedDeltaTime;
    private bool abilityCanBeUsed = true;

    private void Awake()
    {
        standardFixedDeltaTime = Time.fixedDeltaTime;
    }

    private void FixedUpdate()
    {
        CheckForAbilityUsing();
    }

    /// <summary>
    /// ќтслеживание нажати€ клавиши мыши дл€ распознани€ использовани€ способности
    /// </summary>
    private void CheckForAbilityUsing()
    {
        if (Input.GetMouseButtonDown(1) && abilityCanBeUsed)
        {
            SlowdownTime();
        }
    }

    /// <summary>
    /// «амедлить врем€ на несколько секунд
    /// </summary>
    private void SlowdownTime()
    {
        TimeScale.SharedInstance.SetTimeScale(1 / timeSlowdownFactor);
        abilityCanBeUsed = false;
        StartCoroutine(RevertToStandardTimescaleAfterAbilityPassing());
    }

    /// <summary>
    /// ѕо прошествии времени действи€ способности вернуть стандартное течение времени
    /// </summary>
    private IEnumerator RevertToStandardTimescaleAfterAbilityPassing()
    {
        yield return new WaitForSeconds(slowdownDuration);
        TimeScale.SharedInstance.SetTimeScale(1f);
        StartCoroutine(ImplementCooldownMechanic());
    }

    /// <summary>
    /// ѕроизвести кулдаун перед возможностью снова использовать способность
    /// </summary>
    /// <returns></returns>
    private IEnumerator ImplementCooldownMechanic()
    {
        yield return new WaitForSeconds(abilityCooldown);
        abilityCanBeUsed = true;
    }
}