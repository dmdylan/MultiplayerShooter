using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerEvents : NetworkBehaviour
{
    public delegate void FloatChangeDelegate(float value);
    [SyncEvent]
    public event FloatChangeDelegate EventHealthChangedEvent;
    [Command]
    public void CmdHealthChangedEvent(float value) => EventHealthChangedEvent?.Invoke(value);


    public delegate void IntChangeDelegate(int value);
    [SyncEvent]
    public event IntChangeDelegate EventAmmoChangedEvent;
    [Command]
    public void CmdAmmoChangedEvent(int value) => EventAmmoChangedEvent?.Invoke(value);
}
