using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour
{
    private bool storageOpen;
    private Canvas storageInventory;

    private Canvas inventoryCanvas;
  

    // Start is called before the first frame update
    void Start()
    {
        inventoryCanvas = GameObject.Find("InventoryCanvas").GetComponent<Canvas>();
        storageInventory = GameObject.Find("StorageInventoryCanvas").GetComponent<Canvas>();
        storageInventory.enabled = false;
        storageOpen = false;

    }

    #region helper_functions
    IEnumerator OpenStorage()
    {
        yield return null;
        if (storageOpen)
        {
            storageInventory.enabled = false;
            storageOpen = false;
            inventoryCanvas.enabled = false;
            GameObject.Find("Game Manager").GetComponent<GameManager>().storageOpen = false;
            GameObject.Find("Game Manager").GetComponent<GameManager>().paused = false;
            Time.timeScale = 1;
            Enemy.setGamePause(false);
        }
        else
        {
            storageInventory.enabled = true;
            storageOpen = true;
            inventoryCanvas.enabled = true;
            GameObject.Find("Game Manager").GetComponent<GameManager>().storageOpen = true;
            GameObject.Find("Game Manager").GetComponent<GameManager>().paused = true;
            Time.timeScale = 0;
            Enemy.setGamePause(true);
        }
    }

    public void Interact()
    {
        StartCoroutine(OpenStorage());
    }
    #endregion
}
