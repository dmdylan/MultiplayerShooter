using Mirror;
using Mirror.Examples.Basic;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking.Types;
using UnityEngine.UIElements;

public class PlayerCombatController : NetworkBehaviour
{
    private PlayerUI playerUI;
    private Camera playerCamera = null;
    private bool canAttack = true;
    private bool isReloading = false;
    private float timeBetweenAttacks = 0f;
    private Ray ray;
    [SerializeField] private Transform firePoint = null;
    [SerializeField] private GameObject weaponHolster = null;
    [SerializeField] private GameObject muzzleFlashObject = null;
    private ParticleSystem muzzleFlash;

    public int CurrentAmmo { get; private set; } = 0;
    public Weapon Weapon { get; private set; }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        playerCamera = Camera.main;
        weaponHolster.transform.SetParent(playerCamera.transform);
        weaponHolster.transform.localPosition = new Vector3(.35f, -.25f, .4f);
        playerUI = GetComponent<PlayerUI>();
    }

    private void Start()
    {
        Weapon = GetComponentInChildren<Weapon>();
        CurrentAmmo = Weapon.weaponInfo.MaxAmmo;
        muzzleFlash = Weapon.GetComponentInChildren<ParticleSystem>();
    }

    private void Shoot()
    {
        if(canAttack == true && CurrentAmmo > 0 && hasAuthority)
        {
            CmdShoot();
            timeBetweenAttacks = Weapon.weaponInfo.AttackRate;
            ReduceAmmo();
        }
    }

    [Command]
    private void CmdShoot()
    {
        if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, Weapon.weaponInfo.WeaponRange))
        {
            var playerObject = hit.collider.gameObject.GetComponent<PlayerInfo>();
            if (playerObject)
            {
                //TODO: Working but only for host I think?
                playerObject.TargetTakeDamage(playerObject.netIdentity.connectionToClient, Weapon.weaponInfo.WeaponDamage);
            }
        }

        RpcWeaponEffects();    
    }

    //TODO: Only host has muzzle flash effect and can fire? ---Appears as only host can shoot 
    //private IEnumerator FireWeapon()
    //{
    //    if (canAttack.Equals(false) || CurrentAmmo <= 0)
    //        yield break;
    //
    //    canAttack = false;
    //    RpcWeaponEffects();
    //
    //    //TODO: Need to get the network id and apply damage at some point?
    //    if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, Weapon.weaponInfo.WeaponRange))
    //    {
    //        var playerObject = hit.collider.gameObject.GetComponent<PlayerInfo>();
    //        if (playerObject)
    //        {
    //           playerObject.TargetTakeDamage(Weapon.weaponInfo.WeaponDamage);
    //        }
    //    }
    //
    //    ReduceAmmo();
    //
    //    yield return new WaitForSeconds(Weapon.weaponInfo.AttackRate);
    //    RpcWeaponEffects();
    //    canAttack = true;
    //}

    void ReduceAmmo()
    {
        CurrentAmmo--;
        playerUI.UpdatePlayerAmmo(CurrentAmmo, Weapon.weaponInfo.MaxAmmo);
    }

    [ClientRpc]
    private void RpcWeaponEffects()
    {
        muzzleFlashObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            weaponHolster.transform.LookAt(playerCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, playerCamera.farClipPlane)));
            ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width/2,Screen.height/2,0));
            ray.origin = firePoint.position;

            if (Input.GetKey(KeyCode.Mouse0) && hasAuthority)
            {
                Shoot();
            }

            if (Input.GetKeyDown(KeyCode.R) && isReloading.Equals(false))
            {
                StartCoroutine(TargetReloadWeapon());
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                GetComponent<PlayerInfo>().CmdTakeDamage(10);
            }

            if(timeBetweenAttacks >= 0)
            {
                canAttack = false;
                timeBetweenAttacks -= Time.deltaTime;
            }
            else
            {
                canAttack = true;
                muzzleFlashObject.SetActive(false);
            }
        }
    }

    [Client]
    private IEnumerator TargetReloadWeapon()
    {
        isReloading = true;
        yield return new WaitForSeconds(Weapon.weaponInfo.ReloadTime);
        CurrentAmmo = Weapon.weaponInfo.MaxAmmo;
        playerUI.UpdatePlayerAmmo(CurrentAmmo, Weapon.weaponInfo.MaxAmmo);
        isReloading = false;
    }
}
