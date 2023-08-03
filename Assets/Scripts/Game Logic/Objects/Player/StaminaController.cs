using UnityEngine;
using UnityEngine.Events;
using System;

/// <summary>
/// Реализация механики выносливости и утомления у игрока
/// </summary>
public class StaminaController : MonoBehaviour
{
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaRegeneratingPerSecond;
    private float stamina;

    /// <summary>
    /// Оставшееся на данный момент количество сил игрока (Если Stamina = 0, то появляется эффект Tired,
    /// и игрок не может бегать и прыгать до момента, когда данная шкала не восстановится до макс. значения)
    /// </summary>
    public float Stamina
    {
        get { return stamina; }
        set
        {
            if (Stamina == MaxStamina && value < MaxStamina)
            {
                StaminaBecameLessThanMax.Invoke();
            }

            stamina = value;
            if (stamina <= 0)
            {
                stamina = 0;
                Tired = true;
                StaminaExhausted.Invoke();
            }
            if (stamina >= MaxStamina)
            {
                stamina = MaxStamina;
                Tired = false;
                StaminaRestored.Invoke();
            }

            StaminaChanged.Invoke(Stamina);
        }
    }

    /// <summary>
    /// Событие изменения шкалы выносливости
    /// </summary>
    public UnityEvent<float> StaminaChanged = new UnityEvent<float>();

    /// <summary>
    /// Вызывается, когда значение выносливости становится меньше максимального
    /// </summary>
    public UnityEvent StaminaBecameLessThanMax = new UnityEvent();

    /// <summary>
    /// Вызывается, когда выносливость полностью исчерпалась
    /// </summary>
    public UnityEvent StaminaExhausted = new UnityEvent();

    /// <summary>
    /// Вызывается, когда выносливость полностью восстанавливается
    /// </summary>
    public UnityEvent StaminaRestored = new UnityEvent();

    /// <summary>
    /// Максимальное значение шкалы выносливости
    /// </summary>
    public float MaxStamina => maxStamina;

    /// <summary>
    /// Может ли восстанавливаться шкала выносливости в данный момент
    /// </summary>
    public bool StaminaCanRegenerating { get; set; } = true;

    /// <summary>
    /// Утомлён ли игрок в данный момент
    /// </summary>
    public bool Tired { get; private set; } = false;

    private void Awake()
    {
        Stamina = MaxStamina;
    }

    private void FixedUpdate()
    {
        TryToRegenerateStamina();
    }

    /// <summary>
    /// Восстанавливать шкалу выносливости, если это возможно
    /// </summary>
    private void TryToRegenerateStamina()
    {
        if (StaminaCanRegenerating)
        {
            Stamina += staminaRegeneratingPerSecond * Time.fixedDeltaTime;
        }
    }
}