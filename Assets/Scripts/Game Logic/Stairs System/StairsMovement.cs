using UnityEngine;

/// <summary>
/// Передвижение структур ступеней для создания бесконечной лестницы
/// </summary>
public class StairsMovement : MonoBehaviour
{
    [SerializeField] private GameObject currentStairsStrucure;
    [SerializeField] private GameObject otherStairsStructure;
    [SerializeField] private GameObject centralWall;

    private const float STRUCTURE_OFFSET = 2.425f;
    
    // Направление игрока по оси Z (вверх или вниз по лестнице), когда он входит в триггер передвижения структур ступеней
    private float onColliderEnterZAxisValue;

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
                if (currentStairsStrucure.name == "Lower Structure")
                {
                    otherStairsStructure.transform.position = new Vector3(otherStairsStructure.transform.position.x,
                        currentStairsStrucure.transform.position.y - STRUCTURE_OFFSET, otherStairsStructure.transform.position.z);
                    centralWall.transform.position = new Vector3(centralWall.transform.position.x,
                        centralWall.transform.position.y - STRUCTURE_OFFSET, centralWall.transform.position.z);
                }
                else if (currentStairsStrucure.name == "Upper Structure")
                {
                    otherStairsStructure.transform.position = new Vector3(otherStairsStructure.transform.position.x,
                        currentStairsStrucure.transform.position.y + STRUCTURE_OFFSET, otherStairsStructure.transform.position.z);
                    centralWall.transform.position = new Vector3(centralWall.transform.position.x,
                        centralWall.transform.position.y + STRUCTURE_OFFSET, centralWall.transform.position.z);
                }
            }
            else if (playerController.PlayerVelocity.z < 0f && onColliderEnterZAxisValue < 0f)
            {
                if (currentStairsStrucure.name == "Lower Structure")
                {
                    otherStairsStructure.transform.position = new Vector3(otherStairsStructure.transform.position.x,
                        currentStairsStrucure.transform.position.y + STRUCTURE_OFFSET, otherStairsStructure.transform.position.z);
                    centralWall.transform.position = new Vector3(centralWall.transform.position.x,
                        centralWall.transform.position.y + STRUCTURE_OFFSET, centralWall.transform.position.z);
                }
                else if (currentStairsStrucure.name == "Upper Structure")
                {
                    otherStairsStructure.transform.position = new Vector3(otherStairsStructure.transform.position.x,
                        currentStairsStrucure.transform.position.y - STRUCTURE_OFFSET, otherStairsStructure.transform.position.z);
                    centralWall.transform.position = new Vector3(centralWall.transform.position.x,
                        centralWall.transform.position.y - STRUCTURE_OFFSET, centralWall.transform.position.z);
                }
            }
        }
    }
}