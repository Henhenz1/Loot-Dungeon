using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModularSpear : Spear
{
    public GameObject image;
    public bool inShop;
    public bool inInventory;
    private bool initiated;
    Inventory inv;
    Inventory shopInv;

    int tip;
    int sash;
    int handle;
    int aura;

    public int auraType; // 0 = no aura, 1 = fire, 2 = cold

    public Sprite[] tips;
    public Sprite[] sashes;
    public Sprite[] handles;
    public Sprite[] auras;

    public float[] tipVals;   // determines damage of weapon
    public float[] sashVals;   // determines damage reduction, or damage of weapon
    public float[] guardVals;    // determines speed of weapon

    // Cold effect values
    [HideInInspector]
    public float slowDuration;
    [HideInInspector]
    public float freezeDuration;

    // Start is called before the first frame update
    protected override void Start()
    {
        if (initiated)
        {
            return;
        }
        float toMult = 1 + (LevelInfo.level / 2f);
        for (int i = 0; i < tipVals.Length; i++)
        {
            tipVals[i] *= toMult;
            sashVals[i] *= toMult;
            guardVals[i] *= 1 + (LevelInfo.level / 8f);
        }
        base.Start();
        tip = Random.Range(0, tips.Length);
        sash = Random.Range(0, sashes.Length);
        handle = Random.Range(0, handles.Length);
        transform.Find("Tip").GetComponent<SpriteRenderer>().sprite = tips[tip];
        transform.Find("Sash").GetComponent<SpriteRenderer>().sprite = sashes[sash];
        transform.Find("Handle").GetComponent<SpriteRenderer>().sprite = handles[handle];

        if (!inInventory)
        {
            if (Random.Range(0, 1.0f) <= auraChance) // messy and balanced for first floor only. TODO scale up
            {
                aura = Random.Range(0, auras.Length);
                auraType = aura + 1;
                transform.Find("Aura").GetComponent<SpriteRenderer>().sprite = auras[aura];
                additionalDamage += 5;
            }
            else
            {
                transform.Find("Aura").GetComponent<SpriteRenderer>().enabled = false;
                auraType = 0;
            }
        }
        

        additionalDamage += tipVals[tip] + sashVals[sash];
        attackSpeedMultiplier = guardVals[handle];
        description = "* +" + additionalDamage + " damage\n* x" + attackSpeedMultiplier + " attack speed";
        if (auraType == 1)
        {
            description += "\n* deals damage over time";
        }
        else if (auraType == 2)
        {
            description += "\n* may slow enemy";
            slowDuration = 2;
            freezeDuration = 1;            
        }
        value = 10 * (tip+1) + 5 * (handle+1) + 2 * (sash+1) + (int) additionalDamage;
        if (auraType != 0)
        {
            value += 100;
        }
        value = (int) System.Math.Round(value * toMult);

        inv = GameObject.Find("InventoryCanvas").transform.Find("Inventory").gameObject.GetComponent<Inventory>();
        shopInv = GameObject.Find("ShopInventoryCanvas").transform.Find("ShopInventory").gameObject.GetComponent<Inventory>();
        initiated = true;

        if (weaponScaling == 0)
        {
            scaling = "";
        } else if (weaponScaling == 1)
        {
            scaling = "Strength";
        } else if (weaponScaling == 2)
        {
            scaling = "Dexterity";
        } else if (weaponScaling == 3)
        {
            scaling = "Intelligence";
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D c)
    {
        if (inShop)
        {
            return;
        }
        GameObject other = c.gameObject;
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            GameObject inventoryItem = Instantiate(image);
            inventoryItem.transform.Find("Tip").GetComponent<Image>().sprite = tips[tip];
            inventoryItem.transform.Find("Sash").GetComponent<Image>().sprite = sashes[sash];
            inventoryItem.transform.Find("Handle").GetComponent<Image>().sprite = handles[handle];

            if (auraType != 0)
            {
                inventoryItem.transform.Find("Aura").GetComponent<Image>().sprite = auras[aura];
            }
            else
            {
                inventoryItem.transform.Find("Aura").GetComponent<Image>().enabled = false;
            }

            ModularSpear invSpear = inventoryItem.GetComponent<ModularSpear>();
            invSpear.inInventory = true;
            invSpear.additionalDamage = additionalDamage;
            invSpear.attackSpeedMultiplier = attackSpeedMultiplier;
            invSpear.description = description;
            invSpear.itemName = "Spear";
            invSpear.value = value;
            invSpear.auraType = auraType;
            invSpear.slowDuration = slowDuration;
            invSpear.freezeDuration = freezeDuration;

            inv.AddItem(invSpear);
            Destroy(inventoryItem);
            Destroy(gameObject);
        }
    }

    public override void shopActivate(Transform slotTransform)
    {
        GameObject inventoryItem = Instantiate(image);
        inventoryItem.transform.Find("Tip").GetComponent<Image>().sprite = tips[tip];
        inventoryItem.transform.Find("Sash").GetComponent<Image>().sprite = sashes[sash];
        inventoryItem.transform.Find("Handle").GetComponent<Image>().sprite = handles[handle];

        //if (auraType != 0)
        //{
        //    //inventoryItem.transform.Find("Aura").GetComponent<Image>().sprite = auras[aura];
        //}
        //else
        //{
        //    inventoryItem.transform.Find("Aura").GetComponent<Image>().enabled = false;
        //}

        inventoryItem.transform.Find("Aura").GetComponent<Image>().enabled = false;
        ModularSpear invSpear = inventoryItem.GetComponent<ModularSpear>();
        invSpear.additionalDamage = additionalDamage;
        invSpear.attackSpeedMultiplier = attackSpeedMultiplier;
        invSpear.description = description;
        invSpear.itemName = "Spear";
        invSpear.value = value;
        invSpear.auraType = auraType;
        invSpear.slowDuration = slowDuration;
        invSpear.freezeDuration = freezeDuration;

        Instantiate(invSpear, slotTransform);
        Destroy(inventoryItem);
        Destroy(gameObject);
    }

    public override void equip(GameObject g)
    {
        base.equip(g);
        g.GetComponent<Player>().ChangeAura(auraType);
        g.GetComponent<Player>().ChangeColdValues(slowDuration, freezeDuration);
    }

    public override void unequip(GameObject g)
    {
        base.unequip(g);
        g.GetComponent<Player>().ChangeAura(0);
        g.GetComponent<Player>().ChangeColdValues(0, 0);

    }
}
