using UnityEngine;

/// <summary>
/// Реализация платформы под лестницами, чтобы в случае проваливания под них или за них игрок не падал вечно
/// </summary>
public class DeathPlatformController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            other.gameObject.GetComponent<Entity>().OnDeath();
        }
    }
}