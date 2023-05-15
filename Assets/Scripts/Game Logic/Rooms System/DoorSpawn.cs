using UnityEngine;

/// <summary>
/// —павн двери с некоторым шансом на каждом этаже
/// </summary>
public class DoorSpawn : MonoBehaviour
{
    [SerializeField] private float doorSpawnChance;
    [SerializeField] private GameObject wallWithDoor;
    [SerializeField] private GameObject wallWithoutDoor;

    // Ќаправление игрока по оси Z (вверх или вниз по лестнице), когда он входит в триггер передвижени€ структур ступеней
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

            // ѕроверка на соответсвие одному направлению входа и выхода (игрок полностью прошЄл триггер)
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
    /// ќпределить наличие двери на этаже, где находитс€ игрок
    /// </summary>
    private void DetermineIfDoorWillOnFloor()
    {
        // ≈сли игрок впервые проходит триггер на данном этаже
        if (GameProperties.DoorOnFloor.Count == -GameProperties.FloorNumber)
        {
            // ≈сли на предыдущем этаже нет двери с комнатой, то дл€ данного этажа с некоторым шансом
            // определ€етс€ наличие двери (иначе обычна€ стена). “о есть комнаты генерируютс€ минимум
            // каждый второй этаж, чтобы не было пересечени€ высоких комнат.
            if (GameProperties.FloorNumber != 0 && GameProperties.DoorOnFloor[-GameProperties.FloorNumber - 1])
            {
                GameProperties.DoorOnFloor.Add(false);
                return;
            }

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
    /// —оздать или убрать дверь в зависимости от номера этажа
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