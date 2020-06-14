using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations;

public class Pistol : Weapon
{
    [SerializeField] private GameObject firePoint = null;
    [SerializeField] private float weaponRange = 0;
    [SerializeField] private float weaponDamage = 0;
    [SerializeField] private int maxAmmo = 0;
    [SerializeField] private float attackRate = 0;
    private int currentAmmo = 0;
    [SerializeField] private ParticleSystem muzzleFlash;

    public override IEnumerator FireWeapon()
    {
        if (canAttack.Equals(false))
            yield break;

        canAttack = false;

        RaycastHit hit;

        if (Physics.Raycast(firePoint.transform.position, firePoint.transform.forward, out hit, WeaponRange))
        {
            Debug.Log(hit.collider.name);
            if (TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(WeaponDamage);
            }
        }

        muzzleFlash.Play();
        currentAmmo--;

        Debug.Log(AttackRate);
        Debug.Log("Weapon fired");
        yield return new WaitForSeconds(AttackRate);
        canAttack = true;
    }

    private void Start()
    {
        muzzleFlash = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        PistolSetup();
    }

    private void PistolSetup()
    {
        AttackRate = attackRate;
        MaxAmmo = maxAmmo;
        currentAmmo = MaxAmmo;
        WeaponDamage = weaponDamage;
        WeaponRange = weaponRange;
    }
}
