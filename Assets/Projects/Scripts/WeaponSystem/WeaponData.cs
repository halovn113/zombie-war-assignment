using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [SerializeField]
    public WeaponInformation[] listWeaponInforation;
}
