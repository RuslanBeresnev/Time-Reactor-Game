using UnityEngine;

/// <summary>
/// Управление движением игрока
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    private float moveSpeed = 3f;
    private Vector3 previousPosition;

    private AudioSource playerMovingSource;

    private void Awake()
    {
        var audioSources = GetComponents<AudioSource>();
        foreach (var audioSource in audioSources)
        {
            if (audioSource.clip.name == "Player Steps Sound")
            {
                playerMovingSource = audioSource;
            }
        }

        previousPosition = transform.position;
    }

    private void FixedUpdate()
    {
        Move();
        DetectDirection();
        previousPosition = transform.position;

        PlayerMovingPlay();
    }

    /// <summary>
    /// Текущее направление игрока по одной из осей
    /// </summary>
    public enum AxisDirection
    {
        Back = -1,
        Stay,
        Forward
    }

    /// <summary>
    /// Текущее направление по оси Z
    /// </summary>
    public static AxisDirection ZAxisDirection { get; private set; } = AxisDirection.Stay;

    /// <summary>
    /// Текущее направление по оси Y
    /// </summary>
    public static AxisDirection YAxisDirection { get; private set; } = AxisDirection.Stay;

    /// <summary>
    /// Текущее направление по оси X
    /// </summary>
    public static AxisDirection XAxisDirection { get; private set; } = AxisDirection.Stay;

    /// <summary>
    /// Движение игрока в зависимости от нажатых клавиш W, A, S, D
    /// </summary>
    private void Move()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * moveSpeed * Time.fixedDeltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * moveSpeed * Time.fixedDeltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * moveSpeed * Time.fixedDeltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= transform.right * moveSpeed * Time.fixedDeltaTime;
        }
    }

    /// <summary>
    /// Распознать, в какую сторону движется игрок по какой-либо оси
    /// </summary>
    private void DetectDirection()
    {
        // В функциях сравнения ниже берётся значение 0.01f, так как видимо даже когда игрок "стоит на месте", то
        // при поворотах камеры он сдвигается на тысячные или меньшие доли по осям

        if (transform.position.z - previousPosition.z > 0.0001f)
        {
            ZAxisDirection = AxisDirection.Forward;
        }
        else if (transform.position.z - previousPosition.z < -0.0001f)
        {
            ZAxisDirection = AxisDirection.Back;
        }
        else
        {
            ZAxisDirection = AxisDirection.Stay;
        }

        if (transform.position.y - previousPosition.y > 0.0001f)
        {
            YAxisDirection = AxisDirection.Forward;
        }
        else if (transform.position.y - previousPosition.y < -0.0001f)
        {
            YAxisDirection = AxisDirection.Back;
        }
        else
        {
            YAxisDirection = AxisDirection.Stay;
        }

        if (transform.position.x - previousPosition.x > 0.0001f)
        {
            XAxisDirection = AxisDirection.Forward;
        }
        else if (transform.position.x - previousPosition.x < -0.0001f)
        {
            XAxisDirection = AxisDirection.Back;
        }
        else
        {
            XAxisDirection = AxisDirection.Stay;
        }
    }

    /// <summary>
    /// Движется ли игрок в данный момент
    /// </summary>
    public bool IsPlayerMoving()
    {
        if (XAxisDirection != AxisDirection.Stay || YAxisDirection != AxisDirection.Stay || ZAxisDirection != AxisDirection.Stay)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Проиграть звук ходьбы игрока
    /// </summary>
    private void PlayerMovingPlay()
    {
        if (IsPlayerMoving() && !playerMovingSource.isPlaying)
        {
            playerMovingSource.Play();
        }
        else if (!IsPlayerMoving())
        {
            playerMovingSource.Stop();
        }
    }
}