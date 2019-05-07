using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Item
{
    public float additionalDamage;
    public float attackSpeedMultiplier;

    protected virtual void Start()
    {
        type = "Weapon";
    }

    public override void equip(GameObject g)
    {
        Player p = g.GetComponent<Player>();
        int scaleStat = p.getStat(weaponScaling);
        p.ChangeDamage(additionalDamage * (1f + scaleStat/20f));
        p.RefactorSpeed(1 / attackSpeedMultiplier);
        p.ChangeCombatMode(0);
    }

    public override void unequip(GameObject g)
    {
        Player p = g.GetComponent<Player>();
        int scaleStat = p.getStat(weaponScaling);
        g.GetComponent<Player>().ChangeDamage(-(additionalDamage * (1f + scaleStat / 20f)));
        g.GetComponent<Player>().RefactorSpeed(attackSpeedMultiplier);
        g.GetComponent<Player>().ChangeCombatMode(0);
    }

    public override void shopActivate(Transform slotTransform)
    {
        return;
    }
}
