using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolsterSetup : NetworkBehaviour
{
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        transform.SetParent(Camera.main.transform);
        transform.localPosition = new Vector3(1f, -.5f, .2f);
    }
}
