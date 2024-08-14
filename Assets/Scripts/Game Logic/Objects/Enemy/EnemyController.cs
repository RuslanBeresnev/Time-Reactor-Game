using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Поведение врага
/// </summary>
public class EnemyController : Entity
{
    [SerializeField] private string enemyType;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private string targetName;
    [SerializeField] private float damagePerSecond;
    [SerializeField] private bool followsTheTarget;
    [SerializeField] private bool killTargetOnTouch;

    private AudioSource backgroundMusic;
    private GameObject target;
    private Queue<Vector3> targetTrajectory = new Queue<Vector3>();

    /// <summary>
    /// Имя для поиска цели
    /// </summary>
    public string TargetName
    {
        get => targetName;
        set => targetName = value;
    }

    /// <summary>
    /// Будет ли враг следовать за целью
    /// </summary>
    public bool FollowsTheTarget
    {
        get => followsTheTarget;
        private set => followsTheTarget = value;
    }

    public override string[,] ObjectInfoParameters { get; set; } = null;

    public override string ObjectInfoHeader { get; set; } = null;

    public override Color ObjectInfoHeaderColor { get; set; } = Color.red;

    private void Awake()
    {
        ObjectInfoHeader = enemyType;
        string infoAboutDamage;
        if (killTargetOnTouch)
        {
            infoAboutDamage = "One punch to death";
        }
        else
        {
            infoAboutDamage = damagePerSecond.ToString() + " HP/s";
        }
        ObjectInfoParameters = new string[3, 2] { { "Max health:", MaxHealth.ToString() + " HP" },
                                                  { "Damage:", infoAboutDamage },
                                                  { "Movement speed:", moveSpeed.ToString() + " m/s" } };
        InitializeInfoPanelPrefab();

        foreach (var audioSource in GetComponents<AudioSource>())
        {
            audioSource.pitch = TimeScale.SharedInstance.Scale;
        }

        target = GameObject.Find(TargetName);
        StartCoroutine(TrackPlayerTrajectory());
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.name == TargetName)
        {
            if (killTargetOnTouch)
            {
                collision.gameObject.GetComponent<Entity>().OnDeath();
            }
            else
            {
                var damagePerTouch = damagePerSecond * Time.fixedDeltaTime * TimeScale.SharedInstance.Scale;
                collision.gameObject.GetComponent<Entity>().TakeDamage(damagePerTouch);
            }
        }
    }

    private void FixedUpdate()
    {
        if (FollowsTheTarget)
        {
            FollowTheTarget();
        }

        transform.Rotate(new Vector3(0f, rotationSpeed * Time.fixedDeltaTime * TimeScale.SharedInstance.Scale, 0f));
    }

    /// <summary>
    /// Отслеживать траекторию игрока и добавлять метки положения раз в несколько миллисекунд
    /// </summary>
    private IEnumerator TrackPlayerTrajectory()
    {
        var lastAddedPoint = Vector3.zero;
        while (true)
        {
            if (targetTrajectory.Count == 0 || targetTrajectory.Count > 0 && Vector3.Distance(target.transform.position, lastAddedPoint) >= 0.25f)
            {
                // + new Vector3(...) - костыль, так как центр врага находится под ним, поэтому враг должен
                // стремиться к точке, которая ниже центра игрока
                targetTrajectory.Enqueue(target.transform.position + new Vector3(0, -1.5f, 0));
                lastAddedPoint = target.transform.position;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <summary>
    /// Следовать за целью
    /// </summary>
    private void FollowTheTarget()
    {
        if (targetTrajectory.Count > 0)
        {
            // Расстояние для достижения каждой точки траектории игрока не нулевое, так как иначе враг упирается в стену и не может достать до точки
            if (Vector3.Distance(transform.position, targetTrajectory.Peek()) <= 1f)
            {
                targetTrajectory.Dequeue();
            }
            if (targetTrajectory.Count > 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetTrajectory.Peek(), moveSpeed * Time.fixedDeltaTime * TimeScale.SharedInstance.Scale);
            }
        }
    }
}