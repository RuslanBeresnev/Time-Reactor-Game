using UnityEngine;

/// <summary>
/// Алгоритм генерации комнат случайного размера, которые с некоторым шансом появляются на каждом этаже
/// </summary>
public class RoomGeneration : MonoBehaviour
{
    // Центр двери, к которой должна "примкнуть" комната
    [SerializeField] private Transform centerOfDoor;
    // Толщина каждой стены
    [SerializeField] private float wallDepth;

    [SerializeField] private float wallMinLength;
    [SerializeField] private float wallMaxLength;
    [SerializeField] private float wallMinWidth;
    [SerializeField] private float wallMaxWidth;
    [SerializeField] private float wallMinHeight;
    [SerializeField] private float wallMaxHeight;
    [SerializeField] private float entranceWidth;
    [SerializeField] private float entranceHeight;

    private System.Random random = new System.Random();
    private int roomCounter = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            if (GameProperties.DoorOnFloor[-GameProperties.FloorNumber] && !GameProperties.GeneratedRooms.ContainsKey(GameProperties.FloorNumber))
            {
                (var room, var roomCenterOffsetRelativeToDoorCenter) = CreateRoom();
                MatchRoomEntranceWithDoor(room, roomCenterOffsetRelativeToDoorCenter);
                GameProperties.GeneratedRooms.Add(GameProperties.FloorNumber, room);
            }
        }
    }

    /// <summary>
    /// Сгенерировать случайные размеры комнаты (ширина: X, высота: Y, длина: Z)
    /// </summary>
    private Vector3 GenerateRoomDimensions()
    {
        var width = (float)(random.NextDouble() * (wallMaxWidth - wallMinWidth) + wallMinWidth);
        var height = (float)(random.NextDouble() * (wallMaxHeight - wallMinHeight) + wallMinHeight);
        var length = (float)(random.NextDouble() * (wallMaxLength - wallMinLength) + wallMinLength);

        return new Vector3(width, height, length);
    }

    /// <summary>
    /// Сгенерировать расстояние, на которое будет смещён центр входа по оси Z относительно центра стены со входом
    /// </summary>
    private float GenerateEntranceOffset(Vector3 roomDimensions)
    {
        var range = roomDimensions.z - entranceWidth - wallDepth * 2;
        var offset = (float)(random.NextDouble() * range - range * 0.5f);
        return offset;
    }

    /// <summary>
    /// Создать комнату случайного размера (с центром в начале координат и входом по положительному направлению оси X)
    /// </summary>
    /// <returns>Возврашает объект комнаты и смещение центра комнаты относительно центра входа в комнату</returns>
    private (GameObject room, Vector3 offset) CreateRoom()
    {
        var roomDimensions = GenerateRoomDimensions();
        var entranceOffset = GenerateEntranceOffset(roomDimensions);
        var centerOfEntrance = new Vector3(-roomDimensions.x * 0.5f, -(roomDimensions.y * 0.5f - wallDepth - entranceHeight * 0.5f), entranceOffset);

        roomCounter++;
        // Объект комнаты создался с центром в точке (0, 0, 0)
        var room = new GameObject($"Room {roomCounter}");

        var leftWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftWall.name = "Left Wall";
        leftWall.transform.SetParent(room.transform);
        leftWall.transform.localScale = new Vector3(roomDimensions.x, roomDimensions.y, wallDepth);
        leftWall.transform.position = new Vector3(0, 0, -(roomDimensions.z * 0.5f - wallDepth * 0.5f));

        var rightWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightWall.name = "Right Wall";
        rightWall.transform.SetParent(room.transform);
        rightWall.transform.localScale = new Vector3(roomDimensions.x, roomDimensions.y, wallDepth);
        rightWall.transform.position = new Vector3(0, 0, roomDimensions.z * 0.5f - wallDepth * 0.5f);

        var entranceOppositeWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        entranceOppositeWall.name = "Entrance Opposite Wall";
        entranceOppositeWall.transform.SetParent(room.transform);
        entranceOppositeWall.transform.localScale = new Vector3(wallDepth, roomDimensions.y, roomDimensions.z);
        entranceOppositeWall.transform.position = new Vector3(roomDimensions.x * 0.5f - wallDepth * 0.5f, 0, 0);

        var leftSideOfWallWithEntrance = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftSideOfWallWithEntrance.name = "Left Side Of Wall With Entrance";
        leftSideOfWallWithEntrance.transform.SetParent(room.transform);
        leftSideOfWallWithEntrance.transform.localScale = new Vector3(wallDepth, roomDimensions.y,
            roomDimensions.z * 0.5f - entranceOffset - entranceWidth * 0.5f);
        leftSideOfWallWithEntrance.transform.position = new Vector3(-(roomDimensions.x * 0.5f - wallDepth * 0.5f), 0,
            entranceOffset + entranceWidth * 0.5f + leftSideOfWallWithEntrance.transform.localScale.z * 0.5f);

        var rightSideOfWallWithEntrance = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightSideOfWallWithEntrance.name = "Right Side Of Wall With Entrance";
        rightSideOfWallWithEntrance.transform.SetParent(room.transform);
        rightSideOfWallWithEntrance.transform.localScale =
            new Vector3(wallDepth, roomDimensions.y, roomDimensions.z - leftSideOfWallWithEntrance.transform.localScale.z - entranceWidth);
        rightSideOfWallWithEntrance.transform.position = new Vector3(-(roomDimensions.x * 0.5f - wallDepth * 0.5f), 0,
            entranceOffset - entranceWidth * 0.5f - rightSideOfWallWithEntrance.transform.localScale.z * 0.5f);

        var topSideOfWallWithEntrance = GameObject.CreatePrimitive(PrimitiveType.Cube);
        topSideOfWallWithEntrance.name = "Top Side Of Wall With Entrance";
        topSideOfWallWithEntrance.transform.SetParent(room.transform);
        topSideOfWallWithEntrance.transform.localScale = new Vector3(wallDepth, roomDimensions.y - entranceHeight - wallDepth, entranceWidth);
        topSideOfWallWithEntrance.transform.position = new Vector3(-(roomDimensions.x * 0.5f - wallDepth * 0.5f),
            centerOfEntrance.y + entranceHeight * 0.5f + topSideOfWallWithEntrance.transform.localScale.y * 0.5f, entranceOffset);

        var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.transform.SetParent(room.transform);
        floor.transform.localScale = new Vector3(roomDimensions.x, wallDepth, roomDimensions.z);
        floor.transform.position = new Vector3(0, -(roomDimensions.y * 0.5f - wallDepth * 0.5f), 0);

        var ceiling = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ceiling.name = "Ceiling";
        ceiling.transform.SetParent(room.transform);
        ceiling.transform.localScale = new Vector3(roomDimensions.x, wallDepth, roomDimensions.z);
        ceiling.transform.position = new Vector3(0, roomDimensions.y * 0.5f - wallDepth * 0.5f, 0);

        var roomCenterOffsetRelativeToEntranceCenter = -centerOfEntrance;
        return (room, roomCenterOffsetRelativeToEntranceCenter);
    }

    /// <summary>
    /// Расположить комнату таким образом, чтобы вход в неё точно совместился с дверью на лестнице
    /// </summary>
    private void MatchRoomEntranceWithDoor(GameObject room, Vector3 roomCenterOffsetRelativeToDoorCenter)
    {
        room.transform.position = centerOfDoor.transform.position + roomCenterOffsetRelativeToDoorCenter;
    }
}