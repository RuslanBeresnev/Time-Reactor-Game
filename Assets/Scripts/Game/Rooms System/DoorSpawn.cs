using UnityEngine;

/// <summary>
/// Спавн двери с некоторым шансом на каждом этаже
/// </summary>
public class DoorSpawn : MonoBehaviour
{
    [SerializeField] private float doorSpawnChance;
    [SerializeField] private GameObject wallWithDoor;
    [SerializeField] private GameObject wallWithoutDoor;

    // Направление игрока по оси Z (вверх или вниз по лестнице), когда он входит в триггер передвижения структур ступеней
    private float onColliderEnterZAxisValue;
    private System.Random random = new();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            var playerController = other.gameObject.GetComponent<PlayerController>();
            onColliderEnterZAxisValue = playerController.PlayerVelocity.z;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            var playerController = other.gameObject.GetComponent<PlayerController>();

            // Проверка на соответсвие одному направлению входа и выхода (игрок полностью прошёл триггер)
            if (playerController.PlayerVelocity.z > 0f && onColliderEnterZAxisValue > 0f)
            {
                CreateOrDestroyDoorDependingFloorNumber(-GameProperties.FloorNumber - 1);
            }
            else if (playerController.PlayerVelocity.z < 0f && onColliderEnterZAxisValue < 0f)
            {
                DetermineIfDoorWillOnFloor();
                CreateOrDestroyDoorDependingFloorNumber(-GameProperties.FloorNumber);
            }
        }
    }

    /// <summary>
    /// Определить наличие двери на этаже, где находится игрок
    /// </summary>
    private void DetermineIfDoorWillOnFloor()
    {
        // Если игрок впервые проходит триггер на текущем этаже, то для данного этажа
        // с некоторым шансом определяется наличие двери (иначе обычная стена)
        if (GameProperties.DoorOnFloor.Count == -GameProperties.FloorNumber)
        {
            float generatedFloat = (float)random.NextDouble();
            if (generatedFloat < doorSpawnChance)
            {
                GameProperties.DoorOnFloor.Add(true);
            }
            else
            {
                GameProperties.DoorOnFloor.Add(false);
            }
        }
    }

    /// <summary>
    /// Создать или убрать дверь в зависимости от номера этажа
    /// </summary>
    private void CreateOrDestroyDoorDependingFloorNumber(int floorNumber)
    {
        if (floorNumber < 0 || floorNumber >= GameProperties.DoorOnFloor.Count)
        {
            return;
        }

        if (GameProperties.DoorOnFloor[floorNumber])
        {
            wallWithDoor.SetActive(true);
            wallWithoutDoor.SetActive(false);
        }
        else
        {
            wallWithDoor.SetActive(false);
            wallWithoutDoor.SetActive(true);
        }
    }
}