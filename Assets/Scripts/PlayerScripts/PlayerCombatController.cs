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

    private PlayerEvents playerEvents = null;
    private Camera playerCamera = null;
    private bool canAttack = true;
    private Ray ray;
    [SerializeField] private Transform firePoint = null;
    [SerializeField] private GameObject muzzleFlashObject = null;

    public int CurrentAmmo { get; private set; } = 0;
    public Weapon Weapon { get; private set; }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        playerCamera = GetComponentInChildren<Camera>();
        Weapon = weaponArray[selectedWeaponLocal].GetComponent<Weapon>();
        CurrentAmmo = Weapon.weaponInfo.MaxAmmo;
    }

    private void Start()
    {
        playerEvents = GetComponent<PlayerEvents>();    
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

    //TODO: Only host has muzzle flash effect and can fire? ---Appears as only host can shoot 
    private IEnumerator FireWeapon()
    {
        if (canAttack.Equals(false) || CurrentAmmo <= 0)
            yield break;

        canAttack = false;
        RpcWeaponEffects();

        Debug.Log("Shot fired");
        //TODO: Need to get the network id and apply damage at some point?
        //Network game event?
        if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, Weapon.weaponInfo.WeaponRange))
        {
            Debug.Log($"Hit: {hit.collider.name}");
            var playerObject = hit.collider.gameObject.GetComponent<PlayerInfo>();
            if (playerObject)
            {
               Debug.Log("damaging pipe");
               playerObject.TakeDamage(Weapon.weaponInfo.WeaponDamage);
               //player.CmdTakeDamage(weapon.weaponInfo.WeaponDamage);
               //player.CmdUpdateUIElements();
            }
        }

        ReduceAmmo();

        yield return new WaitForSeconds(Weapon.weaponInfo.AttackRate);
        RpcWeaponEffects();
        canAttack = true;
    }

    void ReduceAmmo()
    {
        CurrentAmmo--;
        playerEvents.CmdAmmoChangedEvent(CurrentAmmo);
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
        if (isLocalPlayer)
        {
            Weapon.transform.LookAt(playerCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, playerCamera.farClipPlane)));
            ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width/2,Screen.height/2,0));
            ray.origin = firePoint.position;

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
    }

    private IEnumerator ReloadWeapon()
    {
        canAttack = false;
        yield return new WaitForSeconds(Weapon.weaponInfo.ReloadTime);
        CurrentAmmo = Weapon.weaponInfo.MaxAmmo;
        playerEvents.CmdAmmoChangedEvent(CurrentAmmo);
        canAttack = true;
    }
}
