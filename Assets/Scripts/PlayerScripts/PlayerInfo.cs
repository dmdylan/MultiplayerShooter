using Mirror;
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

    [TargetRpc]
    public void TargetTakeDamage(float damageAmount)
    {
        if (playerHealth - damageAmount <= 0)
        {
            playerHealth = 0;
            PlayerDeath();
        }
        else
            playerHealth -= damageAmount;

        playerUI.UpdatePlayerHP(playerHealth, MaxHealth); 
    }

    private void PlayerDeath()
    {
        Debug.Log("Player dead");
    }
}
