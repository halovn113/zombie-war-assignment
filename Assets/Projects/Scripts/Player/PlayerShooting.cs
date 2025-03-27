using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    public Weapon[] weapons;
    private int currentWeaponIndex = 0;
    private Weapon currentWeapon;

    private bool isFiring = false;

    void Start()
    {
        EquipWeapon(0);
    }

    public void OnFire(InputAction.CallbackContext ctx)
    {
        isFiring = ctx.ReadValue<float>() > 0.1f;
    }

    public void OnSwitchWeapon(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Length;
        EquipWeapon(currentWeaponIndex);
    }

    void EquipWeapon(int index)
    {
        for (int i = 0; i < weapons.Length; i++)
            weapons[i].gameObject.SetActive(i == index);

        currentWeapon = weapons[index];
        ServiceLocator.Get<UIManager>().GetUI(UIEnum.GAMEPLAY).GetComponent<UIGameplay>().UpdateWeaponInfo(weapons[index].weaponData);
    }

    public Weapon GetCurrentWeapon()
    {
        return currentWeapon;
    }
}
