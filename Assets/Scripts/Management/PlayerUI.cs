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

    // Start is called before the first frame update
    void Start()
    {
        playerEvents = GetComponent<PlayerEvents>();
        playerInfo = GetComponent<PlayerInfo>();
        playerCombatController = GetComponent<PlayerCombatController>();
        playerEvents.EventHealthChangedEvent += PlayerEvents_EventHealthChangedEvent;
        playerEvents.EventAmmoChangedEvent += PlayerEvents_EventAmmoChangedEvent;
    }

    private void PlayerEvents_EventAmmoChangedEvent(int value)
    {
        ammoCount.text = $"{value}/{playerCombatController.Weapon.weaponInfo.MaxAmmo}";
    }

    private void OnDisable()
    {
        playerEvents.EventHealthChangedEvent -= PlayerEvents_EventHealthChangedEvent;
    }

    private void PlayerEvents_EventHealthChangedEvent(float currentHealth)
    {
        healthBar.fillAmount = currentHealth / playerInfo.MaxHealth;
    }
}
