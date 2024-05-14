using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Реализация генерации комнаты случайного типа на каждом этаже
/// </summary>
public class RoomGeneration : MonoBehaviour
{
    // Префабы комнат разного типа (лаборатория, склад и т.д.)
    [SerializeField] private List<GameObject> differentTypeRooms = new List<GameObject>();
    // Центр двери, к которой должна "примкнуть" комната
    [SerializeField] private Transform centerOfDoor;

    private System.Random random = new System.Random();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name != "Player" || GameProperties.FloorNumber == GameProperties.LastFloorNumber)
        {
            return;
        }

        // Комната создаётся, если для данного этажа сгенерировался дверной проём и на данном этаже ранее
        // не была сгенерирована комната
        if (GameProperties.DoorOnFloor[-GameProperties.FloorNumber] &&
            !GameProperties.GeneratedRooms.ContainsKey(GameProperties.FloorNumber))
        {
            var randomRoomPrefab = ChooseRandomRoom();
            CreateAndPlaceRoom(randomRoomPrefab);
        }
    }

    /// <summary>
    /// Выбрать комнату случайного типа из списка префабов
    /// </summary>
    /// <returns>Возврщает префаб выбранной комнаты</returns>
    private GameObject ChooseRandomRoom()
    {
        var randomRoomNumber = random.Next(differentTypeRooms.Count);
        return differentTypeRooms[randomRoomNumber];
    }

    /// <summary>
    /// Создать из префаба комнату и разместить её входом к двери
    /// </summary>
    private void CreateAndPlaceRoom(GameObject roomPrefab)
    {
        var room = Instantiate(roomPrefab);
        GameProperties.GeneratedRooms.Add(GameProperties.FloorNumber, room);

        var roomEntranceCenter = room.transform.Find("Center Of Entrance");
        // Центр двери в стене лестницы совмещается с центром входа в комнату
        room.transform.position = centerOfDoor.position + (roomPrefab.transform.position - roomEntranceCenter.position);
    }
}