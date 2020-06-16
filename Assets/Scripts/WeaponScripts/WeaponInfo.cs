using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Info", menuName = "Weapon/Weapon Info")]
public class WeaponInfo : ScriptableObject
{
    [SerializeField] private float weaponRange = 0;
    [SerializeField] private float weaponDamage = 0;
    [SerializeField] private float attackRate = 0;
    [SerializeField] private float reloadTime = 0;
    [SerializeField] private int maxAmmo = 0;

    public float WeaponRange => weaponRange;
    public float WeaponDamage => weaponDamage;
    public float AttackRate => attackRate;
    public float ReloadTime => reloadTime;
    public int MaxAmmo => maxAmmo;
}
