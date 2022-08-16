using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

/// <summary>
/// ”правление камерой
/// </summary>
public class CameraController : MonoBehaviour
{
    private Quaternion cameraOriginRotation;
    private Quaternion playerOriginalRotation;

    private float horizontalAngle = 0f;
    private float verticalAngle = 0f;

    public Transform player;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cameraOriginRotation = transform.rotation;
        playerOriginalRotation = player.rotation;

        AudioListener.volume = SettingsOptions.GeneralVolume;
        GetComponent<PostProcessLayer>().enabled = SettingsOptions.ImprovedGraphicsIsOn;
        GetComponent<PostProcessVolume>().enabled = SettingsOptions.ImprovedGraphicsIsOn;
    }

    void FixedUpdate()
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