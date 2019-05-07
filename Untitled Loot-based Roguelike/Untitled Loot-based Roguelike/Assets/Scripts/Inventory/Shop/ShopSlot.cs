using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ShopSlot : MonoBehaviour, IDropHandler
{
    private GameObject inventory;

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        inventory = GameObject.Find("InventoryCanvas").transform.Find("Inventory").gameObject;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        inventory = GameObject.Find("InventoryCanvas").transform.Find("Inventory").gameObject;
    }

    public GameObject item
    {
        get
        {
            if (transform.childCount > 0)
            {
                return transform.GetChild(0).gameObject;
            }
            return null;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Do nothing
    }

    public void OnMouseEnter()
    {
        if (!item)
        {
            // transform.GetComponentInParent<Inventory>().descriptionText.text = "Empty";
        }
        else
        {
            inventory.GetComponent<Inventory>().descriptionText.text = item.GetComponent<Item>().description;
            inventory.GetComponent<Inventory>().itemName.text = item.GetComponent<Item>().itemName;
        }
    }

    public void OnMouseExit()
    {
        inventory.GetComponent<Inventory>().descriptionText.text = "";
        inventory.GetComponent<Inventory>().itemName.text = "";
    }
}
