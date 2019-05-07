using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    private bool shopOpen;
    private Canvas shopInventory;
    private GameObject[] Items;
    // Start is called before the first frame update
    void Start()
    {
        shopInventory = GameObject.Find("ShopInventoryCanvas").GetComponent<Canvas>();
        shopInventory.enabled = false;
        shopOpen = false;
        foreach (Transform slotTransform in shopInventory.transform.Find("ShopInventory").Find("InventorySlots"))
        {
            GameObject item = slotTransform.GetComponent<ItemSlot>().item;
            if (item)
            {
                item.GetComponent<Item>().shopActivate(slotTransform);
            }
        }
    }

    IEnumerator OpenShop()
    {
        yield return null;
        if (shopOpen)
        {
            shopInventory.enabled = false;
            shopOpen = false;
            GameObject.Find("InventoryCanvas").GetComponent<Canvas>().enabled = false;
            GameObject.Find("Game Manager").GetComponent<GameManager>().shopOpen = false;
            GameObject.Find("Game Manager").GetComponent<GameManager>().paused = false;
            Time.timeScale = 1;
            Enemy.setGamePause(false);
        }
        else
        {
            shopInventory.enabled = true;
            shopOpen = true;
            GameObject.Find("InventoryCanvas").GetComponent<Canvas>().enabled = true;
            GameObject.Find("Game Manager").GetComponent<GameManager>().shopOpen = true;
            GameObject.Find("Game Manager").GetComponent<GameManager>().paused = true;
            Time.timeScale = 0;
            Enemy.setGamePause(true);
        }
    }

    public void Interact()
    {
        StartCoroutine(OpenShop());
    }
}
