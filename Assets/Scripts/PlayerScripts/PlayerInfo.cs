using Mirror;
using UnityEngine;

public class PlayerInfo : NetworkBehaviour, IDamageable
{
    [SyncVar]
    [SerializeField] private float playerHealth;

    [SerializeField] private float maxHealth = 0;

    public float PlayerHealth => playerHealth;
    public float MaxHealth => maxHealth;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerHealth = maxHealth;   
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
