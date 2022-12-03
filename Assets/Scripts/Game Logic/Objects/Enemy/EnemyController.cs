using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ѕоведение врага
/// </summary>
public class EnemyController : Entity, ISerializationCallbackReceiver
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private string targetName;
    [SerializeField] private bool followsTheTarget;

    private AudioSource backgroundMusic;
    private GameObject target;
    private Queue<Vector3> targetTrajectory = new Queue<Vector3>();

    /// <summary>
    /// »м€ дл€ поиска цели
    /// </summary>
    public string TargetName { get; set; }

    /// <summary>
    /// Ѕудет ли враг следовать за целью
    /// </summary>
    public bool FollowsTheTarget { get; private set; }

    public override string[,] ObjectInfoParameters { get; set; } = null;

    public override string ObjectInfoHeader { get; set; } = "Enemy";

    public override Color ObjectInfoHeaderColor { get; set; } = Color.red;

    public override void OnBeforeSerialize()
    {
        health = Health;
        maxHealth = MaxHealth;

        targetName = TargetName;
        followsTheTarget = FollowsTheTarget;
    }

    public override void OnAfterDeserialize()
    {
        Health = health;
        MaxHealth = maxHealth;

        TargetName = targetName;
        FollowsTheTarget = followsTheTarget;
    }

    private void Awake()
    {
        ObjectInfoParameters = new string[3, 2] { { "Max health:", MaxHealth.ToString() + " HP" },
                                                  { "Damage:", "One punch to death" },
                                                  { "Movement speed:", moveSpeed.ToString() + " m/s" } };
        InitializeInfoPanelPrefab();

        var backgroundMusicObject = GameObject.Find("Background Music");
        backgroundMusic = backgroundMusicObject.GetComponent<AudioSource>();
        backgroundMusic.Stop();

        target = GameObject.Find(TargetName);
        StartCoroutine(TrackPlayerTrajectory());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == TargetName)
        {
            collision.gameObject.GetComponent<Entity>().OnDeath();
        }
    }

    private void FixedUpdate()
    {
        if (FollowsTheTarget)
        {
            FollowTheTarget();
        }

        transform.Rotate(new Vector3(0f, rotationSpeed * Time.fixedDeltaTime, 0f));
    }

    /// <summary>
    /// ќтслеживать траекторию игрока и добавл€ть метки положени€ раз в несколько миллисекунд
    /// </summary>
    private IEnumerator TrackPlayerTrajectory()
    {
        var lastAddedPoint = Vector3.zero;
        while (true)
        {
            if (targetTrajectory.Count == 0 || targetTrajectory.Count > 0 && Vector3.Distance(target.transform.position, lastAddedPoint) >= 0.25f)
            {
                targetTrajectory.Enqueue(target.transform.position);
                lastAddedPoint = target.transform.position;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <summary>
    /// —ледовать за целью
    /// </summary>
    private void FollowTheTarget()
    {
        if (targetTrajectory.Count > 0)
        {
            // –ассто€ние дл€ достижени€ каждой точки траектории игрока не нулевое, так как иначе враг упираетс€ в стену и не может достать до точки
            if (Vector3.Distance(transform.position, targetTrajectory.Peek()) <= 0.25f)
            {
                targetTrajectory.Dequeue();
            }
            if (targetTrajectory.Count > 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetTrajectory.Peek(), moveSpeed * Time.fixedDeltaTime);
            }
        }
    }

    /// <summary>
    /// ƒействи€ при смерти врага
    /// </summary>
    public override void OnDeath()
    {
        Destroy(createdPanel);
        Destroy(gameObject);
        backgroundMusic.Play();
    }
}