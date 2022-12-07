using UnityEngine;

/// <summary>
/// ќписывает поведение лазерной башни в процессе зар€дки и разр€дки
/// ≈сли данный скрипт прикреплЄн к лазерной башне (и активен) перед запускаом игры, то она будет зар€жатьс€ в течение
/// некоторого времени с анимацией раскалени€ лазерного источника, а затем атаковать цель; иначе же атака будет
/// происходить сразу при попадении цели в поле зрени€ башни
/// </summary>
public class LaserTowerCharging : MonoBehaviour
{
    [SerializeField] private GameObject laserSource;
    [SerializeField] private Material laserSourceChargedState;
    [SerializeField] private Material laserSourceDischargedState;
    [SerializeField] private float chargingDuration;

    // —тепень зар€женности лазерного источника (от 0 до 1)
    private float chargeDegree = 0f;
    private Renderer render;
    private LaserTowerAttack behaviourOnAttack;

    private void Awake()
    {
        render = laserSource.GetComponent<Renderer>();
        render.material = laserSourceDischargedState;
        behaviourOnAttack = GetComponent<LaserTowerAttack>();
    }

    private void FixedUpdate()
    {
        if (behaviourOnAttack.TargetInSight)
        {
            LaserSourceCharging();
        }
        else
        {
            LaserSourceDischarging();
        }

        if (IsTowerCharged())
        {
            behaviourOnAttack.TowerCharged = true;
        }
        else
        {
            behaviourOnAttack.TowerCharged = false;
        }
    }

    /// <summary>
    /// ѕроцесс зар€дки лазерного источника
    /// </summary>
    private void LaserSourceCharging()
    {
        chargeDegree += (1f / chargingDuration) * Time.fixedDeltaTime;
        chargeDegree = Mathf.Clamp(chargeDegree, 0, 1);

        render.material.Lerp(laserSourceDischargedState, laserSourceChargedState, chargeDegree);
    }

    /// <summary>
    /// ѕроцесс разр€дки лазерного источника
    /// </summary>
    private void LaserSourceDischarging()
    {
        chargeDegree -= (1f / chargingDuration) * Time.fixedDeltaTime;
        chargeDegree = Mathf.Clamp(chargeDegree, 0, 1);

        render.material.Lerp(laserSourceChargedState, laserSourceDischargedState, 1f - chargeDegree);
    }

    /// <summary>
    /// ѕроверить, зар€жена ли лазерна€ башн€ в данный момент
    /// </summary>
    private bool IsTowerCharged()
    {
        return Mathf.Abs(chargeDegree - 1) < 0.0001f;
    }
}