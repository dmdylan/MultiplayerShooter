using Mirror;
using UnityEngine;

public class PlayerInfo : NetworkBehaviour, IDamageable
{
    [SyncVar]
    [SerializeField] private float playerHealth;
    [SerializeField] private float maxHealth = 0;

    private PlayerEvents playerEvents;

    public float PlayerHealth => playerHealth;
    public float MaxHealth => maxHealth;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        playerHealth = maxHealth;   
    }

    private void Start()
    {
        playerEvents = GetComponent<PlayerEvents>();
        if (NetworkClient.active)
        {
            playerEvents.EventTakeDamage += TakeDamage;
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (playerHealth - damageAmount <= 0)
            PlayerDeath();
        else
            playerHealth -= damageAmount;
    }

    private void PlayerDeath()
    {
        Debug.Log("Player dead");
    }
}
