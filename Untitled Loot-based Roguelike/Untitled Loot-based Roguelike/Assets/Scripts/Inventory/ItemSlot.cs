using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    public string type;
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

    public void OnDrop (PointerEventData eventData)
    {
        if (DragHandler.itemBeingDragged == null)
        {
            return;
        }
        Transform t = DragHandler.itemBeingDragged.transform;
        if (t.GetComponent<DragHandler>().getStartParent().GetComponent<ItemSlot>().type.Equals("Shop"))
        {
            return;
        }
        // Only put item in slot if it's an inventory slot or its equipment slot type matches the item's type.
        if (type.Equals("Inventory") || type.Equals(t.GetComponent<Item>().type))
        {
            if (item)
            {
                // If there's already an item in this slot, put it in the slot the dropped item was from.
                Transform p = t.gameObject.GetComponent<DragHandler>().getStartParent();
                item.transform.SetParent(p);
            }
            t.SetParent(transform);
            RectTransform rt = t.GetComponent<RectTransform>();
            RectTransform prt = transform.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(prt.sizeDelta.x * 0.9f, prt.sizeDelta.y * 0.9f);

            ExecuteEvents.ExecuteHierarchy<IHasChanged>(gameObject, null, (x,y) => x.HasChanged());
        }
    }

    public void OnMouseEnter ()
    {
        if (!item)
        {
            // transform.GetComponentInParent<Inventory>().descriptionText.text = "Empty";
        } else
        {
            Inventory inv = inventory.GetComponent<Inventory>();
            Item it = item.GetComponent<Item>();
            inv.descriptionText.text = it.description;
            inv.itemName.text = it.itemName;
            inv.itemValue.text = "Value: " + it.value.ToString();
            inv.itemScaling.text = it.scaling;
        }
    }

    public void OnMouseExit ()
    {
        Inventory inv = inventory.GetComponent<Inventory>();
        inv.descriptionText.text = "";
        inv.itemName.text = "";
        inv.itemValue.text = "";
        inv.itemScaling.text = "";
    }
}
