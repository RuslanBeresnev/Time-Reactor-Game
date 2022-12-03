using UnityEngine;
using System.Collections;

/// <summary>
/// ќписывает поведение лазерной башни
/// </summary>
public class LaserTowerController : MonoBehaviour
{
    [SerializeField] private Transform emitter;
    [SerializeField] private string targetName;
    // Ќазвание пула, где хран€тс€ лазерные лучи
    [SerializeField] private string nameOfLaserPool;

    [SerializeField] private Material laserSource;
    [SerializeField] private Material fieryLaserSource;

    [SerializeField] private float reachRadius;
    // »нтервал между двум€ нанесени€ми урона цели
    [SerializeField] private float hitsInterval;
    [SerializeField] private float damagePerSecond;

    private Transform target;
    private Pool laserPool;
    // Ћазерный луч дл€ данной лазерной башни
    private GameObject laser;
    private bool onAttack = false;

    private void Start()
    {
        target = GameObject.Find(targetName).transform;
        laserPool = GameProperties.GeneralPool[nameOfLaserPool];
    }

    private void FixedUpdate()
    {
        TryToAttackTarget();
    }

    /// <summary>
    /// ѕроверить дос€гаемость цели дл€ атаки
    /// </summary>
    private bool IsTargetAvailableForAttack()
    {
        var layerMask = LayerMask.GetMask("Default", "Player");
        var laserDirection = (target.position - emitter.position).normalized;

        if (Physics.Raycast(emitter.position, laserDirection, out RaycastHit hit, reachRadius, layerMask, QueryTriggerInteraction.Ignore)
            && hit.transform.name == targetName)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Ќаносить урон игроку через промежутки времени
    /// </summary>
    private IEnumerator DealDamageToTarget()
    {
        while (true)
        {
            var entityComponent = target.GetComponent<Entity>();
            if (entityComponent != null)
            {
                entityComponent.TakeDamage(damagePerSecond * hitsInterval);
                yield return new WaitForSeconds(hitsInterval);
            }
        }
    }

    /// <summary>
    /// ќтрисовать лазерный луч между излучателем и целью
    /// </summary>
    private void DrawLaserRay()
    {
        var laserLength = Vector3.Distance(target.position, emitter.position);
        var laserDirection = (target.position - emitter.position).normalized;
        var laserRotation = Quaternion.LookRotation(laserDirection);

        laser.transform.position = emitter.position;
        laser.transform.localScale = new Vector3(laser.transform.localScale.x, laser.transform.localScale.y, laserLength);
        laser.transform.rotation = laserRotation;
    }

    /// <summary>
    /// Ќачать атаку цели, если это возможно
    /// </summary>
    private void TryToAttackTarget()
    {
        if (IsTargetAvailableForAttack())
        {
            if (!onAttack)
            {
                laser = laserPool.GetObject();
                onAttack = true;
                StartCoroutine("DealDamageToTarget");
            }

            DrawLaserRay();
        }
        else
        {
            if (!onAttack)
            {
                return;
            }

            laserPool.ReturnObject(laser);
            onAttack = false;
            StopCoroutine("DealDamageToTarget");
        }
    }
}