using UnityEngine;
using System.Collections;

/// <summary>
/// –еализаци€ механики поиска цели с помощью лазерного луча и механики атаки цели скоростной турелью
/// </summary>
public class RapidTurretAttack : MonoBehaviour
{
    [SerializeField] private string targetName;
    [SerializeField] private string projectilesPoolName;
    [SerializeField] private Transform muzzleEnd;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private AudioSource shotSound;

    [SerializeField] private Transform emitter;
    [SerializeField] private float rayDistance = 300f;
    [SerializeField] private Material targetSearchingStateMaterial;
    [SerializeField] private Material targetDetectedStateMaterial;

    [SerializeField] private int shotsPerSecond = 30;

    private LineRenderer searchingLaser;
    private Transform target;
    private Pool projectilesPool;

    private Coroutine firing;

    /// <summary>
    /// «амечена ли цель на данный момент
    /// </summary>
    public bool TargetDetected { get; private set; } = false;

    /// <summary>
    ///  оличество выстрелов в секунду
    /// </summary>
    public float ShotsPerSecond => shotsPerSecond;

    /// <summary>
    /// ”рон от одного снар€да
    /// </summary>
    public float DamagePerShot => projectilePrefab.GetComponent<Bullet>().Damage;

    private void Awake()
    {
        searchingLaser = GetComponent<LineRenderer>();
        target = GameObject.Find(targetName).transform;
        projectilesPool = GameObject.Find(projectilesPoolName).GetComponent<Pool>();
    }

    private void Update()
    {
        SearchTarget();
    }

    /// <summary>
    ///  аждый кадр испускать луч рейкаста дл€ обнаружени€ цели
    /// </summary>
    private void SearchTarget()
    {
        if (Physics.Raycast(emitter.position, emitter.forward, out RaycastHit hit, rayDistance,
            LayerMask.GetMask("Default", "Player"), QueryTriggerInteraction.Ignore))
        {
            DrawSearchingRay(hit);

            if (hit.transform.gameObject.name == targetName && !TargetDetected)
            {
                TargetDetected = true;
                searchingLaser.material = targetDetectedStateMaterial;
                firing = StartCoroutine(PerformFiring());
            }
            else if (hit.transform.gameObject.name != targetName && TargetDetected)
            {
                TargetDetected = false;
                searchingLaser.material = targetSearchingStateMaterial;
                StopCoroutine(firing);
            }
        }
    }

    /// <summary>
    /// ѕо-разному прорисовывать прицельный луч турели в зависимости от того, обнаружена цель или нет
    /// </summary>
    private void DrawSearchingRay(RaycastHit hit)
    {
        searchingLaser.SetPosition(0, emitter.position);
        if (!TargetDetected)
        {
            searchingLaser.SetPosition(1, hit.point);
        }
        else
        {
            searchingLaser.SetPosition(1, target.position);
        }
    }

    /// <summary>
    /// ¬ыполнить одиночный выстрел боеприпаса в направлении оси Z (forward)
    /// </summary>
    private void Shoot()
    {
        var directionToTarget = (target.position - muzzleEnd.position).normalized;
        var projectile = projectilesPool.GetObject();
        if (projectile != null)
        {
            projectile.transform.rotation = Quaternion.FromToRotation(projectile.transform.forward, directionToTarget);
            projectile.transform.position = muzzleEnd.position;
        }
        projectile.GetComponent<Projectile>().GiveKineticEnergy(directionToTarget);
        shotSound.Play();
    }

    /// <summary>
    /// ѕроизводить стрельбу с определЄнными промежутками между выстрелами до тех пор, пока корутина не будет остановлена
    /// </summary>
    private IEnumerator PerformFiring()
    {
        while (true)
        {
            Shoot();
            yield return StartCoroutine(TimeScale.SharedInstance.WaitForSeconds(1f / shotsPerSecond));
        }
    }
}