using UnityEngine;

/// <summary>
/// Передвижение структур ступеней для создания бесконечной лестницы
/// </summary>
public class StairsMovement : MonoBehaviour
{
    public GameObject currentStairsStrucure;
    public GameObject otherStairsStructure;

    private const float STRUCTURE_OFFSET = 2.425f;

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            if (PlayerController.ZAxisDirection == PlayerController.AxisDirection.Forward)
            {
                if (currentStairsStrucure.name == "Lower Structure")
                {
                    otherStairsStructure.transform.position = new Vector3(otherStairsStructure.transform.position.x,
                        currentStairsStrucure.transform.position.y - STRUCTURE_OFFSET, otherStairsStructure.transform.position.z);
                }
                else if (currentStairsStrucure.name == "Upper Structure")
                {
                    otherStairsStructure.transform.position = new Vector3(otherStairsStructure.transform.position.x,
                        currentStairsStrucure.transform.position.y + STRUCTURE_OFFSET, otherStairsStructure.transform.position.z);
                }
            }
            else if (PlayerController.ZAxisDirection == PlayerController.AxisDirection.Back)
            {
                if (currentStairsStrucure.name == "Lower Structure")
                {
                    otherStairsStructure.transform.position = new Vector3(otherStairsStructure.transform.position.x,
                        currentStairsStrucure.transform.position.y + STRUCTURE_OFFSET, otherStairsStructure.transform.position.z);
                }
                else if (currentStairsStrucure.name == "Upper Structure")
                {
                    otherStairsStructure.transform.position = new Vector3(otherStairsStructure.transform.position.x,
                        currentStairsStrucure.transform.position.y - STRUCTURE_OFFSET, otherStairsStructure.transform.position.z);
                }
            }
        }
    }
}