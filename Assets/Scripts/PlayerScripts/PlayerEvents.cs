using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvents : NetworkBehaviour
{
    public delegate void TakeDamage(float amount);

    [SyncEvent]
    public event TakeDamage EventTakeDamage;

    [Command]
    public void CmdTakeDamage(float value)
    {
        EventTakeDamage(value);
    }
}
