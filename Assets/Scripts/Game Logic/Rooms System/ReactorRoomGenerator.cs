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

    // Границы для генерации номера самого нижнего этажа лестницы (отрицательные)
    [SerializeField] private int minBoundOfLastFloorNumber;
    [SerializeField] private int maxBoundOfLastFloorNumber;

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
        if (GameProperties.FloorNumber == GameProperties.LastFloorNumber + 1 && !finalRoomWasGenerated)
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
        int generatedFloorCount = random.Next(minBoundOfLastFloorNumber, maxBoundOfLastFloorNumber + 1);
        GameProperties.LastFloorNumber = generatedFloorCount;
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