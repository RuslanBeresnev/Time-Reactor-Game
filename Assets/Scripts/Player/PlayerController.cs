using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ���������� ������
/// </summary>
public class PlayerController : Entity
{
    private AudioSource playerMovingSource;
    private Rigidbody playerRigidbody;
    
    private Vector3 movementDirection = Vector3.zero;
    private Vector3 previousPosition;

    [SerializeField]
    private float movementSpeed = 3f;
    [SerializeField]
    private float distanceFromWhichToPushPlayer = 0.2f; // 40% �� ������� ������
    [SerializeField]
    private float minimumVelocityForMovingSound = 1f;

    /// <summary>
    /// �������� ������ �������� ������ (���������� �������� + �������� ��������������� �����������)
    /// </summary>
    public Vector3 PlayerVelocity { get; private set; } = Vector3.zero;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        previousPosition = transform.position;

        var audioSources = GetComponents<AudioSource>();
        foreach (var audioSource in audioSources)
        {
            if (audioSource.clip.name == "Player Steps Sound")
            {
                playerMovingSource = audioSource;
            }
        }

        Health = MaxHealth;
        // ������������� ���������� ����� �������� � ������ ���� � ������������ � �������� ���������
        if (HealthChanged != null)
        {
            HealthChanged(Health);
        }
    }

    private void FixedUpdate()
    {
        Move();
        PushOutPlayerFromWall();
        PlayerMovingSourcePlay();

        PlayerVelocity = playerRigidbody.velocity + (transform.position - previousPosition) / Time.fixedDeltaTime;
        previousPosition = transform.position;
    }

    /// <summary>
    /// �������� ������ � ����������� �� ������� ������ W, A, S, D
    /// </summary>
    private void Move()
    {
        movementDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            movementDirection += transform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movementDirection += -transform.forward;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movementDirection += transform.right;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movementDirection += -transform.right;
        }

        transform.position += movementDirection * movementSpeed * Time.fixedDeltaTime;
    }

    /// <summary>
    /// ���������� ������ �� �����, ���� �� � ��� �������
    /// </summary>
    private void PushOutPlayerFromWall()
    {
        var layerMask = 1;
        var edgeOfPlayer = transform.position + PlayerVelocity.normalized * GetComponent<CapsuleCollider>().radius;
        var playerDisplacementDistance = UsefulFeatures.CalculateDepthOfObjectEntryIntoNearestSurface(transform.position, edgeOfPlayer, layerMask);
        if (playerDisplacementDistance > distanceFromWhichToPushPlayer)
        {
            transform.position += -PlayerVelocity.normalized * playerDisplacementDistance;
        }
    }

    /// <summary>
    /// ���������, �������� �� ����� � ������� ���������, ��� ��������
    /// </summary>
    private bool IsPlayerVelocityHigher(float velocity)
    {
        return PlayerVelocity.magnitude > velocity;
    }

    /// <summary>
    /// ��������� ���� ������ ������
    /// </summary>
    private void PlayerMovingSourcePlay()
    {
        if (IsPlayerVelocityHigher(minimumVelocityForMovingSound) && !playerMovingSource.isPlaying)
        {
            playerMovingSource.Play();
        }
        else if (!IsPlayerVelocityHigher(minimumVelocityForMovingSound))
        {
            playerMovingSource.Stop();
        }
    }

    /// <summary>
    /// �������� ��� ������ ������
    /// </summary>
    public override void OnDeath()
    {
        SceneManager.LoadScene("Game Over Menu");
        Cursor.lockState = CursorLockMode.Confined;

        GameProperties.ResetStatistics();
    }
}