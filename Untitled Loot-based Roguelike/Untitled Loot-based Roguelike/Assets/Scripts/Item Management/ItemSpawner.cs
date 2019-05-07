using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    static Inventory inv;

    [SerializeField]
    GameObject newItem;

    public Sprite[] blades;
    public Sprite[] guards;
    public Sprite[] hilts;

    public float[] bladeVals;   // determines damage of weapon
    public float[] guardVals;   // determines damage reduction, or damage of weapon
    public float[] hiltVals;    // determines speed of weapon

    // Start is called before the first frame update
    void Start()
    {
        //inv = GameObject.FindWithTag("Inventory").GetComponent<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("l"))
        {
            //GenerateSword();
        }
    }

    public void GenerateSword(Transform position)
    {
        GameObject newObj = Instantiate(newItem, position);
        int blade = Random.Range(0, blades.Length);
        int guard = Random.Range(0, guards.Length);
        int hilt = Random.Range(0, hilts.Length);
        newObj.transform.Find("Blade").GetComponent<SpriteRenderer>().sprite = blades[blade];
        newObj.transform.Find("Guard").GetComponent<SpriteRenderer>().sprite = guards[guard];
        newObj.transform.Find("Hilt").GetComponent<SpriteRenderer>().sprite = hilts[hilt];

        Sword s = newObj.GetComponent<Sword>();
        s.type = "Weapon";
        s.additionalDamage = bladeVals[blade];
        s.attackSpeedMultiplier = hiltVals[hilt];
        s.description = "* +" + s.additionalDamage + " damage\n* x" + s.attackSpeedMultiplier + " attack speed"; 
    }

    public static void AddToInventory(Item i)
    {
        inv.AddItem(i);
    }
}
