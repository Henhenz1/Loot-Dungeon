using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuySlot : MonoBehaviour, IDropHandler
{
    private Inventory inv;
    void Start()
    {
        inv = GameObject.Find("InventoryCanvas").transform.Find("Inventory").gameObject.GetComponent<Inventory>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedItem = DragHandler.itemBeingDragged;
        if (inv.player.GetComponent<Player>().getCoins() < droppedItem.GetComponent<Item>().value)
        {
            return;
        } else
        {
            inv.player.GetComponent<Player>().reduceCoins(droppedItem.GetComponent<Item>().value);
            Transform slotTransform = inv.getNextEmpty();
            if (slotTransform)
            {
                droppedItem.transform.SetParent(slotTransform);
            }

            // ExecuteEvents.ExecuteHierarchy<IHasChanged>(gameObject, null, (x, y) => x.HasChanged());
        }
    }
}
