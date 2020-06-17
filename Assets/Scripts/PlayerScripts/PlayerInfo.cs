using Mirror;
using UnityEngine;

public class PlayerInfo : NetworkBehaviour, IDamageable
{
    [SyncVar]
    [SerializeField] private float playerHealth;

    [SerializeField] private float maxHealth = 0;

    private PlayerEvents playerEvents = null;

    public float PlayerHealth => playerHealth;
    public float MaxHealth => maxHealth;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        playerEvents = GetComponent<PlayerEvents>();
        playerHealth = maxHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        if (playerHealth - damageAmount <= 0)
            PlayerDeath();
        else
            playerHealth -= damageAmount;
        Debug.Log(playerHealth);

        playerEvents.CmdHealthChangedEvent(playerHealth);
    }

    private void PlayerDeath()
    {
        Debug.Log("Player dead");
    }
}
