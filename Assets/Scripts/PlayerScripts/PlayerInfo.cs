using Mirror;
using Mirror.Examples.Basic;
using UnityEngine;

public class PlayerInfo : NetworkBehaviour, IDamageable
{
    [SyncVar]
    [SerializeField] private float playerHealth;

    [SerializeField] private float maxHealth = 0;

    private PlayerUI playerUI;

    public float PlayerHealth => playerHealth;
    public float MaxHealth => maxHealth;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        playerUI = GetComponent<PlayerUI>();
        playerHealth = maxHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        if (playerHealth - damageAmount <= 0)
        {
            playerHealth = 0;
            PlayerDeath();
        }
        else
            playerHealth -= damageAmount;
        playerUI.CmdUpdatePlayerHP(playerHealth, MaxHealth); 
    }

    [TargetRpc]
    public void TargetTakeDamage(NetworkConnection targetPlayer, float damageAmount)
    {
        TakeDamage(damageAmount);
    }

    private void PlayerDeath()
    {
        Debug.Log("Player dead");
    }
}
