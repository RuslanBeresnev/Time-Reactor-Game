using UnityEngine;

/// <summary>
/// Реализация механики прыжка игрока и проверка соприкосновения с поверхностями под игроком
/// </summary>
public class GroundCheckerController : MonoBehaviour
{
    [SerializeField] private Rigidbody player;
    [SerializeField] private StaminaController staminaController;

    [SerializeField] private float jumpHeight;
    [SerializeField] private float staminaCostPerJump;

    /// <summary>
    /// Находится ли игрок в данный момент на какой-либо поверхности
    /// </summary>
    public bool IsGrounded { get; private set; } = true;

    private void OnTriggerStay(Collider other)
    {
        if (other.name != "Player")
        {
            IsGrounded = true;
        }
    }

    private void FixedUpdate()
    {
        CheckJump();
    }

    /// <summary>
    /// Реализация механики прыжка и проверка на его возможность
    /// </summary>
    private void CheckJump()
    {
        if (staminaController.Tired)
        {
            return;
        }

        if (Input.GetKey(KeyCode.Space) && IsGrounded)
        {
            IsGrounded = false;
            // Формула скорости тела, брошенного с поверхности вертикально вверх
            player.velocity = new Vector3(player.velocity.x, Mathf.Sqrt(2f * -GravitationController.GlobalGravity * jumpHeight), player.velocity.z);
            staminaController.Stamina -= staminaCostPerJump;
        }
    }
}