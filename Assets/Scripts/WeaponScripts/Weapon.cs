﻿using System.Collections;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected ParticleSystem locationHitMark;
    [SerializeField] protected Sprite locationHitMarkSprite;

    public float WeaponRange { get; protected set; }
    public float WeaponDamage { get; protected set; }
    public int MaxAmmo { get; protected set; }
    public float AttackRate { get; protected set; }

    protected bool canAttack = true;

    public abstract IEnumerator FireWeapon();

    public virtual void ReloadWeapon()
    {

    }
}