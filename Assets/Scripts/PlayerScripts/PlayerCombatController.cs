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

    [SyncVar]
    private int currentAmmo = 0;
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
        currentAmmo = Weapon.weaponInfo.MaxAmmo;
        muzzleFlash = Weapon.GetComponentInChildren<ParticleSystem>();
    }

    private void Shoot()
    {
        CmdReduceAmmo();
        Debug.Log("Shoot command sent");
        if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, Weapon.weaponInfo.WeaponRange))
        {
            Debug.Log("Raycast fired");
            var playerObject = hit.collider.gameObject.GetComponent<PlayerInfo>();
            if (playerObject)
            {
                var networkConnection = playerObject.netIdentity;
                //TODO: Working but only for host I think?
                //Need to make this part a command to call a targetrpc ?
                CmdShoot(networkConnection);
                Debug.Log(playerObject.gameObject.GetInstanceID());
            }
        }
    }

    [Command]
    private void CmdShoot(NetworkIdentity playerObject)
    {
        playerObject.GetComponent<PlayerInfo>().TargetTakeDamage(playerObject.connectionToClient, Weapon.weaponInfo.WeaponDamage);
        RpcWeaponEffects();    
    }

    [Command]
    private void CmdReduceAmmo()
    {
        TargetReduceAmmo();
    }

    [TargetRpc]
    void TargetReduceAmmo()
    {
        currentAmmo--;
        playerUI.UpdatePlayerAmmo(currentAmmo, Weapon.weaponInfo.MaxAmmo);
    }

    [ClientRpc]
    private void RpcWeaponEffects()
    {
        muzzleFlashObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(!isLocalPlayer) { return; }

        weaponHolster.transform.LookAt(playerCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, playerCamera.farClipPlane)));
        ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width/2,Screen.height/2,0));
        ray.origin = firePoint.position;

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
        
        if (Input.GetKey(KeyCode.Mouse0) && canAttack == true && currentAmmo > 0)
        {
            Shoot();
            timeBetweenAttacks = Weapon.weaponInfo.AttackRate;
        }

        if (Input.GetKeyDown(KeyCode.R) && isReloading.Equals(false))
        {
            StartCoroutine(TargetReloadWeapon());
        }

    }

    [Client]
    private IEnumerator TargetReloadWeapon()
    {
        isReloading = true;
        yield return new WaitForSeconds(Weapon.weaponInfo.ReloadTime);
        currentAmmo = Weapon.weaponInfo.MaxAmmo;
        playerUI.UpdatePlayerAmmo(currentAmmo, Weapon.weaponInfo.MaxAmmo);
        isReloading = false;
    }
}
