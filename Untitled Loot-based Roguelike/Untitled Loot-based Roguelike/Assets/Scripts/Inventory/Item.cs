using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Item : MonoBehaviour
{
    public string type;

    public string itemName;

    public int value;

    public int minValue;

    public int maxValue;

    // 0 = None, 1 = Strength, 2 = Dexterity, 3 = Intelligence
    public int weaponScaling;
    public string scaling;

    [TextArea]
    public string description;

    public abstract void equip(GameObject g);

    public abstract void unequip(GameObject g);

    public abstract void shopActivate(Transform slotTransform);
}
