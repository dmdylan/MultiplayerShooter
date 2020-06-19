using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;
using Mirror.Examples.Basic;

public class PlayerUI : NetworkBehaviour
{
    private PlayerCombatController playerCombatController;
    private PlayerInfo playerInfo;
    [SerializeField] private TMP_Text ammoCount = null;
    [SerializeField] private Image healthBar = null;

    private void Awake()
    {
        if(!isLocalPlayer) { return; }

        playerCombatController = GetComponent<PlayerCombatController>();
        playerInfo = GetComponent<PlayerInfo>();
    }

    public void UpdatePlayerHP(float current, float max)
    {
        healthBar.fillAmount = current / max;
    }

    public void UpdatePlayerAmmo(int current, int max)
    {
        ammoCount.text = $"{current}/{max}";
    }
}
