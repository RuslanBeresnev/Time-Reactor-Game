using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Реализация механики течения времени (замена для Time.timeScale)
/// </summary>
public class TimeScale : MonoBehaviour
{
    private AudioSource timeSlowdownSound;
    private AudioSource timeAccelerationSound;

    // Имена источников звука, на которые не действует эффект замедления/ускорения времени
    private List<string> sourcesWithoutPitchChange = new()
    {
        "Player Steps Sound", "Gun Shot", "Gun Reloading", "Weapon Picking Up",
        "Assault Riffle Shot", "Assault Riffle Reloading", "Activation", "Deactivation",
        "Time Slowdown", "Time Acceleration"
    };

    /// <summary>
    /// Скорость течения времени. Все величины, которые меняются в течение некоторого времени
    /// (например, скорость при кинематическом движении объекта), а также приложенные силы
    /// (например, толчок при выбрасывании предмета) должны домножаться на TimeScale.Scale,
    /// чтобы иметь зависимость от течения времени
    /// </summary>
    public float Scale { get; private set; } = 1f;

    /// <summary>
    /// Общий экземпляр класса для других классов
    /// </summary>
    public static TimeScale SharedInstance { get; private set; }

    private void Awake()
    {
        SharedInstance = this;

        foreach (var audioSource in GetComponents<AudioSource>())
        {
            var clipName = audioSource.clip.name;
            if (clipName == "Time Slowdown")
            {
                timeSlowdownSound = audioSource;
            }
            else if (clipName == "Time Acceleration")
            {
                timeAccelerationSound = audioSource;
            }
        }
    }

    /// <summary>
    /// Применить эффекты замедления/ускорения к физике объектов
    /// </summary>
    private void ApplyTimeEffectsToObjectPhysics(float newScale)
    {
        foreach (var rigidBody in GameObject.FindObjectsOfType<Rigidbody>(true))
        {
            if (rigidBody.gameObject.name == "Player")
            {
                continue;
            }
            rigidBody.velocity *= newScale / Scale;
            rigidBody.angularVelocity *= newScale / Scale;
        }
        foreach (var gravitationComponent in GameObject.FindObjectsOfType<GravitationController>(true))
        {
            if (gravitationComponent.gameObject.name == "Player")
            {
                continue;
            }
            // Так как изменение скорости падения квадратично зависит от изменения ускорения свободного падения
            gravitationComponent.GravityScale = newScale * newScale;
        }
    }

    /// <summary>
    /// Понизить/повысить высоту звука и музыки
    /// </summary>
    private void ApplyTimeEffectsToAudioSources(float newScale)
    {
        foreach (var audioSource in GameObject.FindObjectsOfType<AudioSource>(true))
        {
            if (!sourcesWithoutPitchChange.Contains(audioSource.clip.name))
            {
                audioSource.pitch *= newScale / Scale;
            }
        }
    }

    /// <summary>
    /// Изменить течение времени и применить эффекты на игровые объекты и звуки
    /// </summary>
    public void SetTimeScale(float newScale)
    {
        if (newScale <= 0)
        {
            return;
        }

        ApplyTimeEffectsToObjectPhysics(newScale);
        ApplyTimeEffectsToAudioSources(newScale);

        if (newScale <= Scale)
        {
            timeSlowdownSound.Play();
        }
        else
        {
            timeAccelerationSound.Play();
        }

        Scale = newScale;
    }

    /// <summary>
    /// Реалиация стандартного WaitForSeconds() для корректной работы при изменении течения времени
    /// с помощью данного класса (Так как Time.timeScale не изменятеся, стандартный WaitForSeconds()
    /// всегда будет отсчитывать некоторое количество секунд РЕАЛЬНОГО времени, что не подходит для корректной работы)
    /// </summary>
    public IEnumerator WaitForSeconds(float seconds)
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