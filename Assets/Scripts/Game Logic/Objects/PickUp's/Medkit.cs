using UnityEngine;

/// <summary>
/// Реализация подбираемой аптечки, которая восстанавливает несколько очков здоровья
/// </summary>
public class Medkit : ObjectWithInformation
{
    [SerializeField] private int recoveryPoints = 15;

    /// <summary>
    /// Количество восстанавливаемых единиц здоровья игрока
    /// </summary>
    public int RecoveryPoints => recoveryPoints;

    public override string[,] ObjectInfoParameters { get; set; }

    public override string ObjectInfoHeader { get; set; } = "Med Kit";

    public override Color ObjectInfoHeaderColor { get; set; } = Color.green;

    private void Awake()
    {
        InitializeInfoPanelPrefab();
        ObjectInfoParameters = new string[1, 2] { { "Recovery Points:", RecoveryPoints.ToString() } };
    }

    /// <summary>
    /// Действия аптечки при её подбирании игроком
    /// </summary>
    public void PickUp()
    {
        Destroy(gameObject);
    }
}