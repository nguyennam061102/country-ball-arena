using UnityEngine;

public abstract class Damagable : MonoBehaviour
{
    public abstract void CallTakeDamage(Vector2 damage, float damageAmount, Vector2 damagePosition, GameObject damagingWeapon = null, BaseCharacter damagingPlayer = null, bool lethal = true);

    public abstract void TakeDamage(Vector2 damage, float damageAmount, Vector2 damagePosition, GameObject damagingWeapon = null, BaseCharacter damagingPlayer = null, bool lethal = true, bool ignoreBlock = false);

    public abstract void TakeDamage(Vector2 damage, float damageAmount, Vector2 damagePosition, Color dmgColor, GameObject damagingWeapon = null, BaseCharacter damagingPlayer = null, bool lethal = true, bool ignoreBlock = false);
}