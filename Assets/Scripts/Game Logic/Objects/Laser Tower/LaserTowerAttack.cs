using UnityEngine;
using System.Collections;

/// <summary>
/// Описывает поведение лазерной башни при атаке цели
/// </summary>
public class LaserTowerAttack : MonoBehaviour
{
    [SerializeField] private Transform emitter;
    [SerializeField] private string targetName;
    // Название пула, где хранятся лазерные лучи
    [SerializeField] private string nameOfLaserPool;

    [SerializeField] private float reachRadius;
    // Интервал между двумя нанесениями урона цели
    [SerializeField] private float hitsInterval;
    [SerializeField] private float damagePerSecond;

    private Transform target;
    private Pool laserPool;
    // Лазерный луч для данной лазерной башни
    private GameObject laser;
    private bool onAttack = false;

    /// <summary>
    /// Досягаема ли цель для лазерной башни 
    /// </summary>
    public bool TargetInSight { get; set; }

    /// <summary>
    /// Готова ли к атаке башня в данный момент
    /// </summary>
    public bool TowerCharged { get; set; } = false;

    private void Awake()
    {
        var chargingComponent = GetComponent<LaserTowerCharging>();

        // Если на лазерной башне нет компонента LaserTowerCharging, то она всегда
        // будет заряжана и начнёт атаку, как только цель попадёт в её поле зрения
        if (chargingComponent == null || !chargingComponent.enabled)
        {
            TowerCharged = true;
        }
    }

    private void Start()
    {
        target = GameObject.Find(targetName).transform;
        laserPool = GameProperties.GeneralPool[nameOfLaserPool];
    }

    private void FixedUpdate()
    {
        if (IsTargetAvailableForAttack())
        {
            TargetInSight = true;
        }
        else
        {
            TargetInSight = false;
        }

        if (TowerCharged)
        {
            TryToAttackTarget();
        }
    }

    /// <summary>
    /// Проверить досягаемость цели для атаки
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
    /// Наносить урон игроку через промежутки времени
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
    /// Отрисовать лазерный луч между излучателем и целью
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
    /// Начать атаку цели, если это возможно
    /// </summary>
    private void TryToAttackTarget()
    {
        if (TargetInSight)
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