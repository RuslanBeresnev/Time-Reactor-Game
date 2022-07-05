using UnityEngine;

/// <summary>
/// ”правление камерой
/// </summary>
public class CameraController : MonoBehaviour
{
    private Quaternion cameraOriginRotation;
    private Quaternion playerOriginalRotation;

    private float horizontalAngle = 0f;
    private float verticalAngle = 0f;

    private float mouseSensitivity = 200f;

    public Transform player;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cameraOriginRotation = transform.rotation;
        playerOriginalRotation = player.rotation;
    }

    void FixedUpdate()
    {
        horizontalAngle += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        verticalAngle += Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        verticalAngle = Mathf.Clamp(verticalAngle, -60, 60);

        var yAxisRotation = Quaternion.AngleAxis(horizontalAngle, Vector3.up);
        var xAxisRotation = Quaternion.AngleAxis(-verticalAngle, Vector3.right);

        player.rotation = playerOriginalRotation * yAxisRotation;
        transform.localRotation = cameraOriginRotation * xAxisRotation;
    }
}