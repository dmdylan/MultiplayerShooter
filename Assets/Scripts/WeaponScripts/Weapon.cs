using Unity;
using Mirror;
using System.Collections;

public abstract class Weapon : NetworkBehaviour
{
    public float WeaponRange { get; protected set; }
    public float WeaponDamage { get; protected set; }
    public int MaxAmmo { get; protected set; }
    public float AttackRate { get; protected set; }
    public abstract IEnumerator Attack();
}