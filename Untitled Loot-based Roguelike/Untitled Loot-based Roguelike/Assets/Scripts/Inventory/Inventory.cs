using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory : MonoBehaviour, IHasChanged
{
    public Item[] inventory;
    public Item[] equipment;
    public GameObject player;
    [SerializeField] Transform equipmentSlots;
    [SerializeField] Transform inventorySlots;
    public Text descriptionText;
    public Text itemName;
    public Text itemValue;
    public Text itemScaling;

    void Start ()
    {
        inventory = new Item[inventorySlots.childCount];
        equipment = new Item[equipmentSlots.childCount];
        HasChanged();
    }

    // A slot's item has changed.
    public void HasChanged ()
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        int i = 0;
        foreach (Transform slotTransform in equipmentSlots)
        {
            GameObject item = slotTransform.GetComponent<ItemSlot>().item;
            if (item)
            {
                Item eq = item.GetComponent<Item>();
                if (!equipment[i])
                {
                    // Equipping this item, calling relevant methods in Player.
                    equipment[i] = eq;
                    player.GetComponent<Player>().Equip(eq);
                } else if (equipment[i] != eq)
                {
                    player.GetComponent<Player>().Unequip(equipment[i]);
                    equipment[i] = eq;
                    player.GetComponent<Player>().Equip(eq);
                }
                builder.Append(item.name);
                builder.Append(" - ");
            } else
            {
                if (equipment[i])
                {
                    player.GetComponent<Player>().Unequip(equipment[i]);
                }
                equipment[i] = null;
            }
            i++;
        }
        if (builder.Length > 0)
        {
            builder.Remove(builder.Length - 3, 3);
        }
        // equipmentText.text = builder.ToString();
        i = 0;
        foreach (Transform slotTransform in inventorySlots)
        {
            GameObject item = slotTransform.GetComponent<ItemSlot>().item;
            if (item)
            {
                Item inv = item.GetComponent<Item>();
                inventory[i] = inv;
            } else
            {
                inventory[i] = null;
            }
            i++;
        }
    }

    // Add an item to inventory slots.
    public void AddItem(Item itemToAdd)
    {
        int i = 0;
        foreach (Transform slotTransform in inventorySlots)
        {
            GameObject item = slotTransform.GetComponent<ItemSlot>().item;
            if (!item)
            {
                Instantiate(itemToAdd, slotTransform);
                //GameObject newItem = slotTransform.GetComponent<ItemSlot>().item;
                //Image newImage = newItem.GetComponent<Image>();
                //newImage.sprite = itemToAdd.GetComponent<Image>().sprite;
                //newImage.color = itemToAdd.GetComponent<Image>().color;
                return;
            }
            i++;
        }
    }

    public Transform getNextEmpty()
    {
        foreach (Transform slotTransform in inventorySlots)
        {
            GameObject item = slotTransform.GetComponent<ItemSlot>().item;
            if (!item)
            {
                return slotTransform;
            }
        }
        return null;
    }

    // Remove an item from inventory slots.
    public void RemoveItem(Item itemToRemove)
    {
        int i = 0;
        foreach (Transform slotTransform in inventorySlots)
        {
            if (inventory[i] == itemToRemove)
            {
                inventory[i] = null;
                Destroy(slotTransform.GetComponent<ItemSlot>().item);
            }
        }
    }
}

namespace UnityEngine.EventSystems
{
    public interface IHasChanged : IEventSystemHandler
    {
        void HasChanged();
    }
}
