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
    private ParticleSystem muzzleFlash;
    NetworkIdentity network;

    //Might need to be a command to work on the server, but causes network identity error issue otherwise
    //[Command]
    public override void CmdAttack()
    {
        RaycastHit hit;

        if(Physics.Raycast(firePoint.transform.position, firePoint.transform.forward, out hit, WeaponRange))
        {
            Debug.Log(hit.collider.name);
            if(TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(WeaponDamage);
            }
        }

        muzzleFlash.Play();

        currentAmmo--;
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        muzzleFlash = GetComponent<ParticleSystem>();
        PistolSetup();
    }

    private void Start()
    {
        network = GetComponentInParent<NetworkIdentity>();
        muzzleFlash = GetComponent<ParticleSystem>();
        //transform.SetParent(Camera.main.transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (!network.isLocalPlayer)
            return;

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
