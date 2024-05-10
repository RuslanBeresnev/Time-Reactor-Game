using UnityEngine;

/// <summary>
/// Реализация универсального боеприпаса, который при подбирании может добавить определённое количество
/// патронов в любой магазин
/// </summary>
public class Ammo : ObjectWithInformation
{
    [SerializeField] private int ammoCount = 5;
    [SerializeField] private AudioClip pickUpSound;

    /// <summary>
    /// Количество патронов, которое добавляет боеприпас в запас
    /// </summary>
    public int AmmoCount => ammoCount;

    public override string[,] ObjectInfoParameters { get; set; }

    public override string ObjectInfoHeader { get; set; } = "Universal Ammo";

    public override Color ObjectInfoHeaderColor { get; set; } = Color.green;

    private void Awake()
    {
        InitializeInfoPanelPrefab();
        ObjectInfoParameters = new string[1, 2] { { "Bullets:", AmmoCount.ToString() } };
    }

    /// <summary>
    /// Действия боеприпаса при его подбирании игроком
    /// </summary>
    public void PickUp()
    {
        AudioSource.PlayClipAtPoint(pickUpSound, transform.position);
        Destroy(gameObject);
    }
}