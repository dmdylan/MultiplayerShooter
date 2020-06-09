using Mirror;
using System.Collections;
using UnityEngine;

public class Pistol : Weapon
{
    [SerializeField] private GameObject firePoint = null;
    [SerializeField] private float weaponRange = 0;
    [SerializeField] private float weaponDamage = 0;
    [SerializeField] private int maxAmmo = 0;
    [SerializeField] private float attackRate = 0;
    private int currentAmmo = 0;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        transform.SetParent(Camera.main.transform);
    }

    public override IEnumerator Attack()
    {
        if (canAttack.Equals(false))
            yield break;

        canAttack = false;

        RaycastHit hit;

        if(Physics.Raycast(firePoint.transform.position, firePoint.transform.forward, out hit, WeaponRange))
        {
            Debug.Log(hit.collider.name);
            if(TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(WeaponDamage);
            }
        }

        currentAmmo--;

        yield return new WaitForSeconds(AttackRate);
        canAttack = true;
    }

    private void Start()
    {
        PistolSetup();
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.farClipPlane)));
        //Debug.DrawRay(firePoint.transform.position, firePoint.transform.forward * 100, Color.red);
    }

    private void PistolSetup()
    {
        MaxAmmo = maxAmmo;
        currentAmmo = MaxAmmo;
        WeaponDamage = weaponDamage;
        AttackRate = attackRate;
        WeaponRange = weaponRange;
    }
}
