using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;
using Mirror.Examples.Basic;

public class PlayerUI : NetworkBehaviour
{
    [SerializeField] private TMP_Text ammoCount = null;
    [SerializeField] private Image healthBar = null;

    [TargetRpc]
    public void TargetUpdatePlayerHP(float current, float max)
    {
        healthBar.fillAmount = current / max;
    }

    public void UpdatePlayerAmmo(int current, int max)
    {
        ammoCount.text = $"{current}/{max}";
    }

    [Command]
    public void CmdUpdatePlayerHP(float current, float max)
    {
        TargetUpdatePlayerHP(current, max);
    }
}
