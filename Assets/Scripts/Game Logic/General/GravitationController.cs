using UnityEngine;

/// <summary>
/// Реализация механики гравитации для объектов с Rigidbody
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class GravitationController : MonoBehaviour
{
    [SerializeField] private float gravityScale = 1f;

    private Rigidbody rigidBody;

    /// <summary>
    /// Мировое ускорение свободного падения
    /// </summary>
    public static float GlobalGravity => -9.81f;

    /// <summary>
    /// Множитель гравитации для каждого объекта
    /// </summary>
    public float GravityScale
    {
        get { return gravityScale; }
        set { gravityScale = value; }
    }

    private void OnEnable()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.useGravity = false;
    }

    private void FixedUpdate()
    {
        Vector3 gravityVector = GlobalGravity * GravityScale * Vector3.up;
        rigidBody.AddForce(gravityVector, ForceMode.Acceleration);
    }
}