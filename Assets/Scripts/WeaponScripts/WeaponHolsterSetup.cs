using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolsterSetup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.SetParent(Camera.main.transform);
        transform.localPosition = new Vector3(1f, -.5f, .2f);
    }
}
