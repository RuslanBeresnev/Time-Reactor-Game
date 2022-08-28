using UnityEngine;

/// <summary>
/// Управление основной камерой
/// </summary>
public class MainCameraController : MonoBehaviour
{
    private Quaternion cameraOriginRotation;
    private Quaternion playerOriginalRotation;

    private float horizontalAngle = 0f;
    private float verticalAngle = 0f;

    [SerializeField]
    private Transform player;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        AudioListener.volume = SettingsOptions.GeneralVolume;

        cameraOriginRotation = transform.rotation;
        playerOriginalRotation = player.rotation;
    }

    private void FixedUpdate()
    {
        horizontalAngle += Input.GetAxis("Mouse X") * SettingsOptions.MouseSensitivity * Time.fixedDeltaTime;
        verticalAngle += Input.GetAxis("Mouse Y") * SettingsOptions.MouseSensitivity * Time.fixedDeltaTime;
        verticalAngle = Mathf.Clamp(verticalAngle, -70, 70);

        var yAxisRotation = Quaternion.AngleAxis(horizontalAngle, Vector3.up);
        var xAxisRotation = Quaternion.AngleAxis(-verticalAngle, Vector3.right);

        player.rotation = playerOriginalRotation * yAxisRotation;
        transform.localRotation = cameraOriginRotation * xAxisRotation;
    }
}