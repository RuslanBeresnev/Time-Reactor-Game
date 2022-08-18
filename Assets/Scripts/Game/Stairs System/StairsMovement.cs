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

    private PlayerMovement.AxisDirection colliderEnteringDirection;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            colliderEnteringDirection = PlayerMovement.ZAxisDirection;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            if (PlayerMovement.ZAxisDirection == PlayerMovement.AxisDirection.Forward && colliderEnteringDirection == PlayerMovement.AxisDirection.Forward)
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
            else if (PlayerMovement.ZAxisDirection == PlayerMovement.AxisDirection.Back && colliderEnteringDirection == PlayerMovement.AxisDirection.Back)
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