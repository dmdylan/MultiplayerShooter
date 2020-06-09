using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerCombatController : NetworkBehaviour
{
    [SerializeField] private GameObject[] weaponArray = null;
    private int selectedWeaponLocal = 0;

    [SyncVar(hook = nameof(OnWeaponChanged))]
    private int activeWeaponSynced;

    private Weapon weapon;

    private void Start()
    {
        weapon = weaponArray[selectedWeaponLocal].GetComponent<Weapon>();
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

    [Command]
    private void CmdFireWeapon()
    {
        StartCoroutine(weapon.Attack());
    }

    private void ChangeWeapons()
    {
        var scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput > 0f)
        {
            selectedWeaponLocal += 1;
            if (selectedWeaponLocal > weaponArray.Length-1) { selectedWeaponLocal = 0; }
            CmdChangeActiveWeapon(selectedWeaponLocal);
        }
        else if (scrollInput < 0f)
        {
            selectedWeaponLocal -= 1;
            if (selectedWeaponLocal < 0) { selectedWeaponLocal = 2; }
            CmdChangeActiveWeapon(selectedWeaponLocal);
        }

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
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedWeaponLocal = 2;
            CmdChangeActiveWeapon(selectedWeaponLocal);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //only our own player runs below here
        if (!isLocalPlayer) { return; }

        ChangeWeapons();

        if (Input.GetKey(KeyCode.Mouse0))
        {
            CmdFireWeapon();
        }
    }
}
