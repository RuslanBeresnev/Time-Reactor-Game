using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ���������� ������
/// </summary>
public class PlayerController : Entity
{
    private AudioSource playerMovingSource;
    private Rigidbody playerRigidbody;

    // ��������, � ������� ����� ������ ��������� � ������ ������ �������
    private float currentSpeed;
    private Vector3 movementDirection;
    private Vector3 previousPosition;

    // �������� ������ ������
    [SerializeField] private float walkingSpeed;
    // �������� ���� ������
    [SerializeField] private float runningSpeed;
    [SerializeField] private float staminaCostPerSecondOfRunning;
    [SerializeField] private float minimumVelocityForMovingSound;
    [SerializeField] private float distanceFromWhichToPushPlayer; // 40% �� ������� ������

    [SerializeField] private GroundCheckerController groundChecker;
    [SerializeField] private StaminaController staminaController;

    [SerializeField] private AudioClip playerWalkingSound;
    [SerializeField] private AudioClip playerRunningSound;

    /// <summary>
    /// �������� ������ �������� ������ (���������� �������� + �������� ��������������� �����������)
    /// </summary>
    public Vector3 PlayerVelocity { get; private set; }

    // ��������� 3 �������� ��� ������ �� ������������
    public override string[,] ObjectInfoParameters { get; set; }

    public override string ObjectInfoHeader { get; set; }

    public override Color ObjectInfoHeaderColor { get; set; }

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        playerMovingSource = GetComponent<AudioSource>();
        previousPosition = transform.position;
        currentSpeed = walkingSpeed;
    }

    private void FixedUpdate()
    {
        CheckMoving();
        CheckRunning();
        PushOutPlayerFromWall();
        PlayerMovingSourcePlay();

        PlayerVelocity = playerRigidbody.velocity + (transform.position - previousPosition) / Time.fixedDeltaTime;
        previousPosition = transform.position;
    }

    /// <summary>
    /// �������� ������ � ����������� �� ������� ������ W, A, S, D
    /// </summary>
    private void CheckMoving()
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

        transform.Translate(movementDirection.normalized * currentSpeed * Time.fixedDeltaTime, Space.World);
    }

    /// <summary>
    /// ���������� �������� ����
    /// </summary>
    private void CheckRunning()
    {
        if (Input.GetKey(KeyCode.LeftShift) && !staminaController.Tired &&
            (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)))
        {
            staminaController.StaminaCanRegenerating = false;
            currentSpeed = runningSpeed;
            staminaController.Stamina -= staminaCostPerSecondOfRunning * Time.fixedDeltaTime;

            if (playerMovingSource.clip == playerWalkingSound)
            {
                SwapMovingSoundToRunning();
            }
        }
        else
        {
            currentSpeed = walkingSpeed;
            // ����� ������������ �����������������, ������ ���� ����� �� �����
            staminaController.StaminaCanRegenerating = true;

            if (playerMovingSource.clip == playerRunningSound)
            {
                SwapMovingSoundToWalking();
            }
        }
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
        if (!groundChecker.IsGrounded)
        {
            playerMovingSource.Stop();
            return;
        }

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
    /// �������� �������� ���� ����� ������ �� ����� ������
    /// </summary>
    private void SwapMovingSoundToWalking()
    {
        playerMovingSource.clip = playerWalkingSound;
    }

    /// <summary>
    /// �������� �������� ���� ����� ������ �� ����� ����
    /// </summary>
    private void SwapMovingSoundToRunning()
    {
        playerMovingSource.clip = playerRunningSound;
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