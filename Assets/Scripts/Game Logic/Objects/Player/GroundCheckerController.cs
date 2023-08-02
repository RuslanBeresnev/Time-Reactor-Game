using UnityEngine;

/// <summary>
/// Реализация механики прыжка игрока и проверка соприкосновения с поверхностями под игроком
/// </summary>
public class GroundCheckerController : MonoBehaviour
{
    [SerializeField] private Rigidbody player;
    [SerializeField] private float jumpHeight;

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
        if (Input.GetKey(KeyCode.Space) && IsGrounded)
        {
            Jump();
        }
    }

    /// <summary>
    /// Совершить прыжок
    /// </summary>
    private void Jump()
    {
        IsGrounded = false;
        // Формула скорости тела, брошенного с поверхности вертикально вверх
        player.velocity = new Vector3(player.velocity.x, Mathf.Sqrt(2f * -GravitationController.GlobalGravity * jumpHeight), player.velocity.z);
    }
}