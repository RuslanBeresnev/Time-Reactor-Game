using UnityEngine;

/// <summary>
/// Передвижение структур ступеней для создания бесконечной лестницы
/// </summary>
public class StairsMovement : MonoBehaviour
{
    public GameObject currentStairsStrucure;
    public GameObject otherStairsStructure;
    public GameObject centralWall;

    private const float STRUCTURE_OFFSET = 2.425f;

    private PlayerController.AxisDirection colliderEnteringDirection;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            colliderEnteringDirection = PlayerController.ZAxisDirection;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            if (PlayerController.ZAxisDirection == PlayerController.AxisDirection.Forward && colliderEnteringDirection == PlayerController.AxisDirection.Forward)
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
            else if (PlayerController.ZAxisDirection == PlayerController.AxisDirection.Back && colliderEnteringDirection == PlayerController.AxisDirection.Back)
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