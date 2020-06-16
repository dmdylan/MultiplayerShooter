using Mirror;
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

    private Camera playerCamera = null;

    private Weapon weapon;
    private bool canAttack = true;
    private int currentAmmo = 0;
    private Ray ray;
    [SerializeField] private Transform firePoint = null;
    [SerializeField] private GameObject muzzleFlashObject = null;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        playerCamera = GetComponentInChildren<Camera>();
        weapon = weaponArray[selectedWeaponLocal].GetComponent<Weapon>();
        currentAmmo = weapon.weaponInfo.MaxAmmo;
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
            weapon = weaponArray[_new].GetComponent<Weapon>();
        }
    }

    [Command]
    public void CmdChangeActiveWeapon(int currentLocalWeapon)
    {
        activeWeaponSynced = currentLocalWeapon;
    }

    //TODO: Only host has muzzle flash effect and can fire?
    private IEnumerator FireWeapon()
    {
        if (canAttack.Equals(false) || currentAmmo <= 0)
            yield break;

        canAttack = false;
        RpcWeaponEffects();

        Debug.Log("Shot fired");
        //TODO: Need to get the network id and apply damage at some point
        //Network game even
        if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, weapon.weaponInfo.WeaponRange))
        {
            Debug.Log($"Hit: {hit.collider.name}");
            if (TryGetComponent(out PlayerEvents player) && !isLocalPlayer)
            {
                //damageable.TakeDamage(weapon.weaponInfo.WeaponDamage);
                player.CmdTakeDamage(weapon.weaponInfo.WeaponDamage);
            }
        }

        currentAmmo--;
    
        yield return new WaitForSeconds(weapon.weaponInfo.AttackRate);
        RpcWeaponEffects();
        canAttack = true;
    }

    [Command]
    private void CmdFireWeapon()
    {
        StartCoroutine(FireWeapon());
    }

    [ClientRpc]
    private void RpcWeaponEffects()
    {
        if (muzzleFlashObject.activeSelf.Equals(false))
            muzzleFlashObject.SetActive(true);
        else
            muzzleFlashObject.SetActive(false);
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
        //only our own player runs below here
        if (!isLocalPlayer) { return; }
        weapon.transform.LookAt(playerCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, playerCamera.farClipPlane)));
        ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width/2,Screen.height/2,0));
        ray.origin = firePoint.position;
        Debug.DrawRay(ray.origin, ray.direction * weapon.weaponInfo.WeaponRange, Color.white);
        //Debug.DrawRay(playerCamera.transform.position, playerCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, playerCamera.farClipPlane)) , Color.red);
        ChangeWeapons();

        if (Input.GetKey(KeyCode.Mouse0))
        {
            CmdFireWeapon();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(ReloadWeapon());
        }
    }

    private IEnumerator ReloadWeapon()
    {
        canAttack = false;
        yield return new WaitForSeconds(weapon.weaponInfo.ReloadTime);
        currentAmmo = weapon.weaponInfo.MaxAmmo;
        canAttack = true;
    }
}
