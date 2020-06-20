using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;

public class LocalPlayerAnnouncer : NetworkBehaviour
{
    public static event Action<NetworkIdentity> OnLocalPlayerUpdated;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        OnLocalPlayerUpdated?.Invoke(netIdentity);
    }

    private void OnEnable()
    {
        if (isLocalPlayer)
            OnLocalPlayerUpdated?.Invoke(netIdentity);
    }

    private void OnDisable()
    {
        if (isLocalPlayer)
            OnLocalPlayerUpdated?.Invoke(netIdentity);
    }
}
