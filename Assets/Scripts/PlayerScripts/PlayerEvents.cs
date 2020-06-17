using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerEvents : NetworkBehaviour
{
   //public delegate void OnTakeDamageDelegate(float value);
   //[SyncEvent]
   //public event OnTakeDamageDelegate EventOnTakeDamage;
   //[Command]
   //public void CmdTakeDamage(float value) => EventOnTakeDamage?.Invoke(value);

    [SyncEvent]
    public event Action EventUpdateUIElements;
    [Command]
    public void CmdUpdateUIElements() => EventUpdateUIElements?.Invoke();
}
