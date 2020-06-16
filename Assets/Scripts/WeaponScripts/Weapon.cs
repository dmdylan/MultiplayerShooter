using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponInfo weaponInfo;
    [SerializeField] protected ParticleSystem locationHitMark;
    [SerializeField] protected Sprite locationHitMarkSprite;
    public ParticleSystem muzzleFlashObject;
}