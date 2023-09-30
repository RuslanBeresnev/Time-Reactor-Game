using UnityEngine;

/// <summary>
/// Описывает поведение лазерной башни в процессе зарядки и разрядки
/// Если данный скрипт прикреплён к лазерной башне (и активен) перед запускаом игры, то она будет заряжаться в течение
/// некоторого времени с анимацией раскаления лазерного источника, а затем атаковать цель; иначе же атака будет
/// происходить сразу при попадении цели в поле зрения башни
/// </summary>
public class LaserTowerCharging : MonoBehaviour
{
    [SerializeField] private GameObject laserSource;
    [SerializeField] private Material laserSourceChargedState;
    [SerializeField] private Material laserSourceDischargedState;
    [SerializeField] private float chargingDuration;

    // Степень заряженности лазерного источника (от 0 до 1)
    private float chargeDegree = 0f;
    private Renderer render;
    private LaserTowerAttack behaviourOnAttack;

    private AudioSource chargingSound;

    private void Awake()
    {
        render = laserSource.GetComponent<Renderer>();
        render.material = laserSourceDischargedState;
        behaviourOnAttack = GetComponent<LaserTowerAttack>();

        foreach (var audioSource in GetComponents<AudioSource>())
        {
            var clipName = audioSource.clip.name;
            if (clipName == "Laser Tower Charging")
            {
                chargingSound = audioSource;
                chargingSound.pitch = TimeScale.SharedInstance.Scale;
            }
        }
    }

    private void FixedUpdate()
    {
        if (behaviourOnAttack.TargetInSight)
        {
            LaserSourceCharging();
            PlayCharhingSound();
        }
        else
        {
            LaserSourceDischarging();
            PlayCharhingSound();
        }

        if (IsTowerCharged())
        {
            behaviourOnAttack.TowerCharged = true;
            chargingSound.Stop();
        }
        else
        {
            behaviourOnAttack.TowerCharged = false;
        }

        if (IsTowerDischarged())
        {
            chargingSound.Stop();
        }
    }

    /// <summary>
    /// Процесс зарядки лазерного источника
    /// </summary>
    private void LaserSourceCharging()
    {
        chargeDegree += (1f / chargingDuration) * Time.fixedDeltaTime * TimeScale.SharedInstance.Scale;
        chargeDegree = Mathf.Clamp(chargeDegree, 0, 1);

        render.material.Lerp(laserSourceDischargedState, laserSourceChargedState, chargeDegree);
    }

    /// <summary>
    /// Процесс разрядки лазерного источника
    /// </summary>
    private void LaserSourceDischarging()
    {
        chargeDegree -= (1f / chargingDuration) * Time.fixedDeltaTime * TimeScale.SharedInstance.Scale;
        chargeDegree = Mathf.Clamp(chargeDegree, 0, 1);

        render.material.Lerp(laserSourceChargedState, laserSourceDischargedState, 1f - chargeDegree);
    }

    /// <summary>
    /// Проверить, полностью ли заряжена лазерная башня в данный момент
    /// </summary>
    private bool IsTowerCharged()
    {
        return Mathf.Abs(chargeDegree - 1) < 0.0001f;
    }

    /// <summary>
    /// Проверить, полностью ли разряжена лазерная башня в данный момент
    /// </summary>
    private bool IsTowerDischarged()
    {
        return chargeDegree < 0.0001f;
    }

    /// <summary>
    /// Проиграть звук зарядки/разрядки лазерной башни
    /// </summary>
    private void PlayCharhingSound()
    {
        if (!chargingSound.isPlaying)
        {
            chargingSound.Play();
        }
    }
}