using UnityEngine;

/// <summary>
/// Описывает поведение генератора энергии для лазерной башни
/// </summary>
public class LaserTowerGeneratorController : Entity
{
    private LaserTowerAttack attackComponent;

    public override string[,] ObjectInfoParameters { get; set; } = null;

    public override string ObjectInfoHeader { get; set; } = "Laser Tower";

    public override Color ObjectInfoHeaderColor { get; set; } = Color.red;

    private void Awake()
    {
        attackComponent = transform.parent.gameObject.GetComponent<LaserTowerAttack>();
        ObjectInfoParameters = new string[4, 2] { { "Max health:", MaxHealth.ToString() + " HP" },
                                                  { "Damage per second:",  attackComponent.DamagePerSecond.ToString() + " HP" },
                                                  { "Reach radius:", attackComponent.ReachRadius.ToString() + " m." },
                                                  { "Feature:", "Defenseless power generator" } };
        InitializeInfoPanelPrefab();
    }

    public override void OnDeath()
    {
        base.OnDeath();
        Destroy(transform.parent.gameObject);
        attackComponent.ReturnLaserToPool();
    }
}