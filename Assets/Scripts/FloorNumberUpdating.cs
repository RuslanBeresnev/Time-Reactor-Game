using UnityEngine;
using TMPro;

/// <summary>
/// Обновление счётчика этажей, когда игрок спускается ниже или поднимается выше текущего этажа
/// </summary>
public class FloorNumberUpdating : MonoBehaviour
{
    public TextMeshProUGUI floorNumberText;

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            if (PlayerController.ZAxisDirection == PlayerController.AxisDirection.Forward)
            {
                GameProperties.FloorNumber--;
            }
        }
        floorNumberText.text = GameProperties.FloorNumber.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            if (PlayerController.ZAxisDirection == PlayerController.AxisDirection.Back)
            {
                GameProperties.FloorNumber++;
            }
        }
        floorNumberText.text = GameProperties.FloorNumber.ToString();
    }
}