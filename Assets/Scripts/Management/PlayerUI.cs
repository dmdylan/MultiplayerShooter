using Mirror;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror.Examples;
using Mirror.Examples.Basic;

public class PlayerUI : NetworkBehaviour
{
    private PlayerEvents playerEvents = null;
    private PlayerInfo playerInfo = null;
    private PlayerCombatController playerCombatController = null;
    [SerializeField] private TMP_Text ammoCount = null;
    [SerializeField] private Image healthBar = null;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        playerEvents = GetComponent<PlayerEvents>();
        playerInfo = GetComponent<PlayerInfo>();
        playerCombatController = GetComponent<PlayerCombatController>();
        playerEvents.EventHealthChangedEvent += PlayerEvents_EventHealthChangedEvent;
        playerEvents.EventAmmoChangedEvent += PlayerEvents_EventAmmoChangedEvent;
    }

    private void PlayerEvents_EventAmmoChangedEvent(int value)
    {
        if(isLocalPlayer)
            ammoCount.text = $"{value}/{playerCombatController.Weapon.weaponInfo.MaxAmmo}";
    }

    private void OnDisable()
    {
        if(isLocalPlayer)
            playerEvents.EventHealthChangedEvent -= PlayerEvents_EventHealthChangedEvent;
    }

    private void PlayerEvents_EventHealthChangedEvent(float currentHealth)
    {
        if(isLocalPlayer)
            healthBar.fillAmount = currentHealth / playerInfo.MaxHealth;
    }
}
