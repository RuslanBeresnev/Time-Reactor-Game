using UnityEngine;
using TMPro;

/// <summary>
/// Обновление счётчика этажей, когда игрок спускается ниже или поднимается выше текущего этажа
/// </summary>
public class FloorNumberUpdating : MonoBehaviour
{
    public TextMeshProUGUI floorNumberText;

    /// <summary>
    /// Этаж, на котором сейчас находится игрок
    /// </summary>
    public static int FloorNumber { get; private set; } = 0;

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            if (PlayerController.ZAxisDirection == PlayerController.AxisDirection.Forward)
            {
                FloorNumber--;
            }
        }
        floorNumberText.text = FloorNumber.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            if (PlayerController.ZAxisDirection == PlayerController.AxisDirection.Back)
            {
                FloorNumber++;
            }
        }
        floorNumberText.text = FloorNumber.ToString();
    }
}