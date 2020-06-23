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

    private void Update()
    {
        Debug.Log(playerHealth);
    }

    //TODO: Move take damage logic to normal method and then call that method through a command and
    //send info back through TargetRPC
    [Command]
    public void CmdTakeDamage(float damageAmount)
    {
        TakeDamage(damageAmount);
    }

    private void TakeDamage(float damageAmount)
    {
        if (playerHealth - damageAmount <= 0)
        {
            playerHealth = 0;
            PlayerDeath();
        }
        else
            playerHealth -= damageAmount;
    }

    [TargetRpc]
    private void TargetTakeDamage(float damageAmount)
    {

        playerUI.UpdatePlayerHP(playerHealth, MaxHealth); 
    }

    private void PlayerDeath()
    {
        Debug.Log("Player dead");
    }
}
