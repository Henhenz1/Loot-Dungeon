using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModularSword : Sword
{
    Blade o_blade;
    Handguard o_handguard;
    Hilt o_hilt;

    public GameObject image;
    public bool inShop;
    private bool initiated;
    Inventory inv;
    Inventory shopInv;

    int blade;
    int guard;
    int hilt;

    public Sprite[] blades;
    public Sprite[] guards;
    public Sprite[] hilts;

    public float[] bladeVals;   // determines damage of weapon
    public float[] guardVals;   // determines damage reduction, or damage of weapon
    public float[] hiltVals;    // determines speed of weapon

    // Start is called before the first frame update
    protected override void Start()
    {
        if (initiated)
        {
            return;
        }
        float toMult = 1 + (LevelInfo.level / 2f);
        for (int i = 0; i < bladeVals.Length; i++)
        {
            bladeVals[i] *= toMult;
            guardVals[i] *= toMult;
            hiltVals[i] *= 1 + (LevelInfo.level / 6f);
        }
        blade = Random.Range(0, blades.Length);
        guard = Random.Range(0, guards.Length);
        hilt = Random.Range(0, hilts.Length);
        transform.Find("Blade").GetComponent<SpriteRenderer>().sprite = blades[blade];
        transform.Find("Guard").GetComponent<SpriteRenderer>().sprite = guards[guard];
        transform.Find("Hilt").GetComponent<SpriteRenderer>().sprite = hilts[hilt];

        additionalDamage = bladeVals[blade] + guardVals[guard];
        attackSpeedMultiplier = hiltVals[hilt];
        description = "* +" + additionalDamage + " damage\n* x" + attackSpeedMultiplier + " attack speed";
        value = 17 * (blade+1) + 11 * (guard+1)+ 7 * (hilt+1) + (int) additionalDamage;
        value = (int) System.Math.Round(value * toMult);

        inv = GameObject.Find("InventoryCanvas").transform.Find("Inventory").gameObject.GetComponent<Inventory>();
        shopInv = GameObject.Find("ShopInventoryCanvas").transform.Find("ShopInventory").gameObject.GetComponent<Inventory>();
        initiated = true;
        if (weaponScaling == 0)
        {
            scaling = "";
        }
        else if (weaponScaling == 1)
        {
            scaling = "Strength";
        }
        else if (weaponScaling == 2)
        {
            scaling = "Dexterity";
        }
        else if (weaponScaling == 3)
        {
            scaling = "Intelligence";
        }
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        if (inShop)
        {
            Debug.Log("hello");
            return;
        }
        GameObject other = c.gameObject;
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            GameObject inventoryItem = Instantiate(image);
            inventoryItem.transform.Find("Blade").GetComponent<Image>().sprite = blades[blade];
            inventoryItem.transform.Find("Guard").GetComponent<Image>().sprite = guards[guard];
            inventoryItem.transform.Find("Hilt").GetComponent<Image>().sprite = hilts[hilt];

            Sword invSword = inventoryItem.GetComponent<Sword>();
            invSword.additionalDamage = additionalDamage;
            invSword.attackSpeedMultiplier = attackSpeedMultiplier;
            invSword.description = description;
            invSword.itemName = "Sword";
            invSword.value = value;

            inv.AddItem(invSword);
            Destroy(inventoryItem);
            Destroy(gameObject);
        }
    }

    public override void shopActivate(Transform slotTransform)
    {
        GameObject inventoryItem = Instantiate(image);
        inventoryItem.transform.Find("Blade").GetComponent<Image>().sprite = blades[blade];
        inventoryItem.transform.Find("Guard").GetComponent<Image>().sprite = guards[guard];
        inventoryItem.transform.Find("Hilt").GetComponent<Image>().sprite = hilts[hilt];

        Sword invSword = inventoryItem.GetComponent<Sword>();
        invSword.additionalDamage = additionalDamage;
        invSword.attackSpeedMultiplier = attackSpeedMultiplier;
        invSword.description = description;
        invSword.itemName = "Sword";
        invSword.value = Random.Range(minValue, invSword.value);

        Instantiate(invSword, slotTransform);
        Destroy(inventoryItem);
        Destroy(gameObject);
    }
}
