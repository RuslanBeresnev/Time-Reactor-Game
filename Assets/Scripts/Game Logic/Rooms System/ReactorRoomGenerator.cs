using UnityEngine;

/// <summary>
/// Генерация комнаты с временным реактором, которая будет находиться в самом низу лестницы
/// </summary>
public class ReactorRoomGenerator : MonoBehaviour
{
    [SerializeField] private GameObject roomPrefab;
    // Позиция создания финальной комнаты (совмещённая с центром комнаты) находится в виде пустого GameObject
    // в нижней лестничной структуре ("Lower Structure")
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private int minFloorCount;
    [SerializeField] private int maxFloorCount;

    private System.Random random = new System.Random();
    private bool finalRoomWasGenerated = false;

    private void Awake()
    {
        GenerateGeneralFloorCount();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Финальная комната создаётся на последнем этаже лестницы, когда игрок находится
        // на предпоследнем этаже и проходит триггер генератора
        if (GameProperties.FloorNumber == GameProperties.GeneralFloorCount - 1 && !finalRoomWasGenerated)
        {
            GenerateFinalRoom();
            finalRoomWasGenerated = true;
        }
    }

    /// <summary>
    /// Сгенерировать количество этажей на лестнице
    /// </summary>
    private void GenerateGeneralFloorCount()
    {
        int generatedFloorCount = random.Next(minFloorCount, maxFloorCount + 1);
        GameProperties.GeneralFloorCount = generatedFloorCount;
    }

    /// <summary>
    /// Создать и спозиционировать финальную комнату с реактором внизу лестницы
    /// </summary>
    private void GenerateFinalRoom()
    {
        var room = Instantiate(roomPrefab);
        room.transform.position = spawnPoint.position;
    }
}