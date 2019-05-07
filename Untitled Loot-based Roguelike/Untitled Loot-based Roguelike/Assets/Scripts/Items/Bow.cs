using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : Item
{
    public float additionalDamage;
    public float attackSpeedMultiplier;
    public float range;
    public GameObject projectile;

    public override void equip(GameObject g)
    {
        Player p = g.GetComponent<Player>();
        int scaleStat = p.getStat(weaponScaling);
        p.ChangeDamage(additionalDamage * (1f + scaleStat / 20f));
        p.RefactorSpeed(1 / attackSpeedMultiplier);
        p.ChangeCombatMode(1);
        p.ChangeProjectile(projectile);
    }

    public override void unequip(GameObject g)
    {
        Player p = g.GetComponent<Player>();
        int scaleStat = p.getStat(weaponScaling);
        p.ChangeDamage(-(additionalDamage * (1f + scaleStat / 20f)));
        p.RefactorSpeed(attackSpeedMultiplier);
        p.ChangeCombatMode(0);
        p.ChangeProjectile(null);
    }

    public override void shopActivate(Transform slotTransform)
    {
        return;
    }
}
