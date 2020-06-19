using Mirror;
using Mirror.Examples.Basic;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerCombatController : NetworkBehaviour
{
    [SerializeField] private GameObject[] weaponArray = null;
    private int selectedWeaponLocal = 0;

    [SyncVar(hook = nameof(OnWeaponChanged))]
    private int activeWeaponSynced;

    private PlayerUI playerUI;
    private Camera playerCamera = null;
    private bool canAttack = true;
    private bool isReloading = false;
    private float timeBetweenAttacks = 0f;
    private Ray ray;
    [SerializeField] private Transform firePoint = null;
    [SerializeField] private GameObject weaponHolster = null;
    [SerializeField] private GameObject muzzleFlashObject = null;

    public int CurrentAmmo { get; private set; } = 0;
    public Weapon Weapon { get; private set; }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        playerCamera = Camera.main;
        weaponHolster.transform.SetParent(playerCamera.transform);
        weaponHolster.transform.localPosition = new Vector3(.35f, -.25f, .4f);
        Weapon = weaponArray[selectedWeaponLocal].GetComponent<Weapon>();
        CurrentAmmo = Weapon.weaponInfo.MaxAmmo;
        playerUI = GetComponent<PlayerUI>();
    }

    void OnWeaponChanged(int _old, int _new)
    {
        activeWeaponSynced = _new;
        foreach (var item in weaponArray)
        {
            if (item != null) { item.SetActive(false); }
        }
        if (_new < weaponArray.Length && weaponArray[_new] != null)
        {
            weaponArray[_new].SetActive(true);
            Weapon = weaponArray[_new].GetComponent<Weapon>();
        }
    }

    [Command]
    public void CmdChangeActiveWeapon(int currentLocalWeapon)
    {
        activeWeaponSynced = currentLocalWeapon;
    }

    private void Shoot()
    {
        if(canAttack == true && CurrentAmmo > 0)
        {
            CmdShoot();
            timeBetweenAttacks = Weapon.weaponInfo.AttackRate;
            RpcWeaponEffects();
        }
    }

    [Command]
    private void CmdShoot()
    {
        ReduceAmmo();
        //RpcWeaponEffects();

        if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, Weapon.weaponInfo.WeaponRange))
        {
            var playerObject = hit.collider.gameObject.GetComponent<PlayerInfo>();
            if (playerObject)
            {
                playerObject.TargetTakeDamage(Weapon.weaponInfo.WeaponDamage);
            }
        }

        //RpcWeaponEffects();
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

    private void ChangeWeapons()
    {
        var scrollInput = Input.GetAxis("Mouse ScrollWheel");

        //if (scrollInput > 0f)
        //{
        //    selectedWeaponLocal += 1;
        //    if (selectedWeaponLocal > weaponArray.Length-1) { selectedWeaponLocal = 0; }
        //    CmdChangeActiveWeapon(selectedWeaponLocal);
        //}
        //else if (scrollInput < 0f)
        //{
        //    selectedWeaponLocal -= 1;
        //    if (selectedWeaponLocal < 0) { selectedWeaponLocal = weaponArray.Length-1; }
        //    CmdChangeActiveWeapon(selectedWeaponLocal);
        //}

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedWeaponLocal = 0;
            CmdChangeActiveWeapon(selectedWeaponLocal);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedWeaponLocal = 1;
            CmdChangeActiveWeapon(selectedWeaponLocal);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            weaponHolster.transform.LookAt(playerCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, playerCamera.farClipPlane)));
            ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width/2,Screen.height/2,0));
            ray.origin = firePoint.position;

            ChangeWeapons();

            if (Input.GetKey(KeyCode.Mouse0))
            {
                Shoot();
            }

            if (Input.GetKeyDown(KeyCode.R) && isReloading.Equals(false))
            {
                StartCoroutine(TargetReloadWeapon());
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
