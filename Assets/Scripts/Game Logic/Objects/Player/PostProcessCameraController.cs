using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

/// <summary>
/// Управление камерой с эффектами пост-обработки
/// </summary>
public class PostProcessCameraController : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<PostProcessLayer>().enabled = SettingsOptions.ImprovedGraphicsIsOn;
        GetComponent<PostProcessVolume>().enabled = SettingsOptions.ImprovedGraphicsIsOn;
    }
}