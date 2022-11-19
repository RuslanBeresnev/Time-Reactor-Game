using UnityEngine;

/// <summary>
/// Алгоритм генерации комнат случайного размера, которые с некоторым шансом появляются на каждом этаже
/// </summary>
public class RoomGeneration : MonoBehaviour
{
    // Толщина каждой стены
    [SerializeField] private float wallDepth;
    // Точка, находящаяся по центру стены со входом в комнату
    private Vector3 centerOfWallWithEntrance = new Vector3(10, 10, 10);

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

    private void Awake()
    {
        CreateRoom();
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
    /// Сгенерировать расстояние, на которое будет смещён центр входа относительно центра стены
    /// </summary>
    private float GenerateEntranceOffset(Vector3 roomDimensions)
    {
        var range = roomDimensions.x - entranceWidth - wallDepth * 2;
        var offset = (float)(random.NextDouble() * range - range * 0.5f);
        return offset;
    }

    /// <summary>
    /// Создать комнату случайного размера
    /// </summary>
    private void CreateRoom()
    {
        var roomDimensions = GenerateRoomDimensions();
        var entranceOffset = GenerateEntranceOffset(roomDimensions);
        var centerOfEntranceByXAxis = centerOfWallWithEntrance + new Vector3(-entranceOffset, 0, 0);
        roomCounter++;    
        var room = new GameObject($"Room {roomCounter}");

        var leftWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftWall.name = "Left Wall";
        leftWall.transform.SetParent(room.transform);
        leftWall.transform.localScale = new Vector3(wallDepth, roomDimensions.y, roomDimensions.z);
        leftWall.transform.position = centerOfWallWithEntrance + new Vector3(-roomDimensions.x * 0.5f + wallDepth * 0.5f, 0,
            roomDimensions.z * 0.5f);

        var rightWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightWall.name = "Right Wall";
        rightWall.transform.SetParent(room.transform);
        rightWall.transform.localScale = new Vector3(wallDepth, roomDimensions.y, roomDimensions.z);
        rightWall.transform.position = centerOfWallWithEntrance + new Vector3(roomDimensions.x * 0.5f - wallDepth * 0.5f, 0,
            roomDimensions.z * 0.5f);

        var entranceOppositeWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        entranceOppositeWall.name = "Entrance Opposite Wall";
        entranceOppositeWall.transform.SetParent(room.transform);
        entranceOppositeWall.transform.localScale = new Vector3(roomDimensions.x, roomDimensions.y, wallDepth);
        entranceOppositeWall.transform.position = centerOfWallWithEntrance + new Vector3(0, 0, roomDimensions.z - wallDepth * 0.5f);

        var leftSideOfWallWithEntrance = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftSideOfWallWithEntrance.name = "Left Side Of Wall With Entrance";
        leftSideOfWallWithEntrance.transform.SetParent(room.transform);
        leftSideOfWallWithEntrance.transform.localScale =
            new Vector3((centerOfEntranceByXAxis.x - entranceWidth * 0.5f) - (centerOfWallWithEntrance.x - roomDimensions.x * 0.5f),
            roomDimensions.y, wallDepth);
        leftSideOfWallWithEntrance.transform.position =
           new Vector3(centerOfEntranceByXAxis.x - entranceWidth * 0.5f - leftSideOfWallWithEntrance.transform.localScale.x * 0.5f,
           centerOfWallWithEntrance.y, centerOfWallWithEntrance.z + wallDepth * 0.5f);

        var rightSideOfWallWithEntrance = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightSideOfWallWithEntrance.name = "Right Side Of Wall With Entrance";
        rightSideOfWallWithEntrance.transform.SetParent(room.transform);
        rightSideOfWallWithEntrance.transform.localScale =
            new Vector3(roomDimensions.x - leftSideOfWallWithEntrance.transform.localScale.x - entranceWidth, roomDimensions.y, wallDepth);
        rightSideOfWallWithEntrance.transform.position =
          new Vector3(centerOfEntranceByXAxis.x + entranceWidth * 0.5f + rightSideOfWallWithEntrance.transform.localScale.x * 0.5f,
          centerOfWallWithEntrance.y, centerOfWallWithEntrance.z + wallDepth * 0.5f);

        var topSideOfWallWithEntrance = GameObject.CreatePrimitive(PrimitiveType.Cube);
        topSideOfWallWithEntrance.name = "Top Side Of Wall With Entrance";
        topSideOfWallWithEntrance.transform.SetParent(room.transform);
        topSideOfWallWithEntrance.transform.localScale = new Vector3(entranceWidth, roomDimensions.y - entranceHeight, wallDepth);
        topSideOfWallWithEntrance.transform.position = new Vector3(centerOfEntranceByXAxis.x,
           centerOfWallWithEntrance.y + roomDimensions.y * 0.5f - topSideOfWallWithEntrance.transform.localScale.y * 0.5f,
           centerOfWallWithEntrance.z + wallDepth * 0.5f);

        var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.transform.SetParent(room.transform);
        floor.transform.localScale = new Vector3(roomDimensions.x, wallDepth, roomDimensions.z);
        floor.transform.position = centerOfWallWithEntrance + new Vector3(0, -roomDimensions.y * 0.5f + wallDepth * 0.5f, 
            roomDimensions.z * 0.5f);

        var ceiling = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ceiling.name = "Ceiling";
        ceiling.transform.SetParent(room.transform);
        ceiling.transform.localScale = new Vector3(roomDimensions.x, wallDepth, roomDimensions.z);
        ceiling.transform.position = centerOfWallWithEntrance + new Vector3(0, roomDimensions.y * 0.5f - wallDepth * 0.5f,
            roomDimensions.z * 0.5f);
    }
}