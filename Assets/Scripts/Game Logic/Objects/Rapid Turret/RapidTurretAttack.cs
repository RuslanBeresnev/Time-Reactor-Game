using UnityEngine;
using System.Collections;

/// <summary>
/// ���������� �������� ������ ���� � ������� ��������� ���� � �������� ����� ���� ���������� �������
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
    /// �������� �� ���� �� ������ ������
    /// </summary>
    public bool TargetDetected { get; private set; } = false;

    /// <summary>
    /// ���������� ��������� � �������
    /// </summary>
    public float ShotsPerSecond => shotsPerSecond;

    /// <summary>
    /// ���� �� ������ �������
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
    /// ������ ���� ��������� ��� �������� ��� ����������� ����
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
    /// ��-������� ������������� ���������� ��� ������ � ����������� �� ����, ���������� ���� ��� ���
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
    /// ��������� ��������� ������� ���������� � ����������� ��� Z (forward)
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
    /// ����������� �������� � ������������ ������������ ����� ���������� �� ��� ���, ���� �������� �� ����� �����������
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