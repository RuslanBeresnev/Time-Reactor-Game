using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

/// <summary>
/// Набор тестов, проверяющих правильность поведения игрока
/// </summary>
public class PlayerFunctionsTests
{
    [UnityTest]
    public IEnumerator OnPlayerDeathGameOverMenuMustBeLoaded()
    {
        var player = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Assets For Tests/Player"),
            new Vector3(0, 0, 0), Quaternion.identity);
        var enemyHard = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Assets For Tests/Enemy Hard"),
            player.transform.position, Quaternion.identity);

        // Нужно какое-то время, чтобы сцена загрузилась, поэтому не yield return null
        yield return new WaitForSeconds(0.5f);

        var currentScene = SceneManager.GetActiveScene();
        Assert.AreEqual("Game Over Menu", currentScene.name);

        MonoBehaviour.Destroy(player);
        MonoBehaviour.Destroy(enemyHard);
    }

    [UnityTest]
    public IEnumerator OnPlayerDeathStatisticsMustBeReset()
    {
        var player = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Assets For Tests/Player"),
            new Vector3(0, 0, 0), Quaternion.identity);
        var enemyHard = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Assets For Tests/Enemy Hard"),
            player.transform.position, Quaternion.identity);

        yield return null;

        Assert.AreEqual(1f, TimeScale.SharedInstance.Scale);
        Assert.AreEqual(0, GameProperties.FloorNumber);
        Assert.AreEqual(0, GameProperties.PassedFloors.Count);
        Assert.AreEqual(0, GameProperties.DoorOnFloor.Count);
        Assert.AreEqual(0, GameProperties.GeneratedRooms.Count);
        Assert.AreEqual(0, GameProperties.GeneralPool.Count);
        Assert.False(GraphicAnalyzerController.AnalyzerIsActive);
        Assert.IsNull(GraphicAnalyzerController.StateChanged);

        MonoBehaviour.Destroy(player);
        MonoBehaviour.Destroy(enemyHard);
    }

    [UnityTest]
    public IEnumerator PlayerMustGetDamageFromEnemy()
    {
        var player = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Assets For Tests/Player"),
            new Vector3(0, 0, 0), Quaternion.identity);
        var enemyLite = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Assets For Tests/Enemy Lite"),
            player.transform.position, Quaternion.identity);
        var entityComponent = player.GetComponent<Entity>();
        Assert.AreEqual(100f, entityComponent.Health);

        yield return new WaitForSeconds(1f);

        Assert.That(entityComponent.Health < 90f);
        Assert.That(entityComponent.Health > 75f);

        MonoBehaviour.Destroy(player);
        MonoBehaviour.Destroy(enemyLite);
    }
}