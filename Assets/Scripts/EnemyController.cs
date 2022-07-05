using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Поведение врага
/// </summary>
public class EnemyController : MonoBehaviour
{
    private GameObject target;

    private float moveSpeed = 4f;

    /// <summary>
    /// Имя для поиска цели
    /// </summary>
    public string TargetName { get; set; } = "Player";

    /// <summary>
    /// Будет ли враг следовать за целью
    /// </summary>
    public bool FollowsTheTarget { get; private set; } = true;

    private void Start()
    {
        target = GameObject.Find(TargetName);
        target.GetComponent<AudioSource>().Stop();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Player")
        {
            SceneManager.LoadScene("Game Over Menu");
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    private void FixedUpdate()
    {
        if (FollowsTheTarget)
        {
            FollowTheTarget();
        }

        transform.Rotate(new Vector3(0f, -2f, 0f));
    }

    /// <summary>
    /// Следовать за целью
    /// </summary>
    private void FollowTheTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.fixedDeltaTime);
    }
}