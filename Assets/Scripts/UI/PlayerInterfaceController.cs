using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Collections.Specialized;

/// <summary>
/// Отрисовка основного интерфейса игрока
/// </summary>
public class PlayerInterfaceController : MonoBehaviour
{
    [SerializeField]
    private WeaponManager weaponManager;
    [SerializeField]
    private Entity player;

    [SerializeField]
    private List<Image> weaponsInArsenalImages = new List<Image>(GameProperties.PlayerWeaponsArsenalSize);
    [SerializeField]
    private List<TextMeshProUGUI> weaponsInArsenalNames = new List<TextMeshProUGUI>(GameProperties.PlayerWeaponsArsenalSize);
    [SerializeField]
    private List<Image> weaponsInArsenalBackgrounds = new List<Image>(GameProperties.PlayerWeaponsArsenalSize);

    [SerializeField]
    private Sprite activeSlotSprite;
    [SerializeField]
    private Sprite inactiveSlotSprite;

    [SerializeField]
    private Image healthBar;
    [SerializeField]
    private TextMeshProUGUI healthCount;

    private void Awake()
    {
        weaponManager.WeaponsArsenal.CollectionChanged += RedrawWeaponSlotContent;
        weaponManager.ActiveSlotChanged += RedrawWeaponSlotBackground;
        player.HealthChanged += RedrawHealthInfo;
    }

    /// <summary>
    /// Поместить в слот изображение оружия и его название
    /// </summary>
    private void RedrawWeaponSlotContent(object sender, NotifyCollectionChangedEventArgs e)
    {
        // Индекс в арсенале, где изменилось оружие
        var slotNumber = e.NewStartingIndex;
        var weapon = weaponManager.WeaponsArsenal[slotNumber];

        if (weapon == null)
        {
            weaponsInArsenalImages[slotNumber].sprite = null;
            weaponsInArsenalNames[slotNumber].text = "Empty";
            weaponsInArsenalImages[slotNumber].enabled = false;
            return;
        }

        weaponsInArsenalImages[slotNumber].enabled = true;
        weaponsInArsenalImages[slotNumber].sprite = weapon.Sprite;
        weaponsInArsenalNames[slotNumber].text = weapon.Name;
    }

    /// <summary>
    /// Сделать фон слота более прозрачным, если он неактивен, или менее прозрачным, если он активен
    /// </summary>
    private void RedrawWeaponSlotBackground(int slotNumber, bool isSlotActive)
    {
        if (isSlotActive)
        {
            weaponsInArsenalBackgrounds[slotNumber].sprite = activeSlotSprite;
            return;
        }

        weaponsInArsenalBackgrounds[slotNumber].sprite = inactiveSlotSprite;
    }

    /// <summary>
    /// Перерисовать полоску и изменить выводимое значение здоровья игрока
    /// </summary>
    private void RedrawHealthInfo(float health)
    {
        healthBar.fillAmount = health / player.MaxHealth;
        healthCount.text = health.ToString();
    }
}