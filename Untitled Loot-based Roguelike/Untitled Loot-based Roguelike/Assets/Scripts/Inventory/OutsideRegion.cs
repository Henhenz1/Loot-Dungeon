using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OutsideRegion : MonoBehaviour, IDropHandler
{
    public void OnDrop (PointerEventData eventData)
    {
        Destroy(DragHandler.itemBeingDragged);
    }
}
