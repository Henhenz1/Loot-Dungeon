using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SellSlot : MonoBehaviour, IDropHandler
{
    private Inventory inv;
    void Start()
    {
        inv = GameObject.Find("InventoryCanvas").transform.Find("Inventory").gameObject.GetComponent<Inventory>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        Transform t = DragHandler.itemBeingDragged.transform;
        if (t.GetComponent<DragHandler>().getStartParent().GetComponent<ItemSlot>().type.Equals("Shop"))
        {
            return;
        }
        GameObject droppedItem = DragHandler.itemBeingDragged;
        int value = droppedItem.GetComponent<Item>().value;
        inv.player.GetComponent<Player>().addCoins(value);
        Destroy(droppedItem);
    }
}
