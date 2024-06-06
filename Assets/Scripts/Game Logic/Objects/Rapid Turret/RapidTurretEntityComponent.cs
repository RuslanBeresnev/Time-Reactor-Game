using UnityEngine;

/// <summary>
/// Компонента, отвечающая за здоровье турели, за возможность её уничтожения и
/// за отображение информации о ней в графическом анализаторе
/// </summary>
public class RapidTurretEntityComponent : Entity
{
    [SerializeField] private RapidTurretAttack attackComponent;

    public override string[,] ObjectInfoParameters { get; set; } = null;

    public override string ObjectInfoHeader { get; set; } = "Rapid Turret";

    public override Color ObjectInfoHeaderColor { get; set; } = Color.red;

    private void Awake()
    {
        ObjectInfoParameters = new string[4, 2] { { "Max health:", MaxHealth.ToString() + " HP" },
                                                  { "Shots Per Second:", attackComponent.ShotsPerSecond.ToString() },
                                                  { "Damage per shot:", attackComponent.DamagePerShot.ToString() + " HP" },
                                                  { "Feature:", "Pursuit fire upon target acquisition" } };
        InitializeInfoPanelPrefab();
    }
}