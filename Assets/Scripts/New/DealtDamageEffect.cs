﻿using UnityEngine;

public abstract class DealtDamageEffect : MonoBehaviour
{
    public abstract void DealtDamage(Vector2 damage, bool selfDamage, BaseCharacter damagedPlayer = null);
}