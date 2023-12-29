using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// ����� ������, ����������� ������������ ������ ���������� �������� ������
/// </summary>
public class WeaponArsenalTests
{
    [UnityTest]
    public IEnumerator CurrentSlotShouldBeFreeAfterWeaponEjecting()
    {
        // �������� ������ � ��������� ��������� � ����
        var player = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Assets For Tests/Player"),
            new Vector3(0, 0, 0), Quaternion.identity);
        var weaponManager = player.GetComponent<WeaponManager>();

        var weaponToEject = weaponManager.WeaponsArsenal[weaponManager.ActiveSlotNumber];

        // ��������� ���������� ������ EjectWeapon() � ��� �����
        var weaponManagerClassType = typeof(WeaponManager);
        MethodInfo ejectWeaponMethod = weaponManagerClassType.GetMethod("EjectWeapon", BindingFlags.NonPublic | BindingFlags.Instance);
        ejectWeaponMethod.Invoke(weaponManager, new object[0]);

        yield return null;

        Assert.IsNull(weaponManager.WeaponsArsenal[weaponManager.ActiveSlotNumber]);
        Assert.False(weaponToEject.transform.parent.parent.gameObject.GetComponent<Rigidbody>().isKinematic);

        MonoBehaviour.Destroy(weaponToEject);
        MonoBehaviour.Destroy(player);
    }
}