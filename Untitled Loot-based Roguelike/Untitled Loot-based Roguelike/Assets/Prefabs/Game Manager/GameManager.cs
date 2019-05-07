using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public bool paused;
    private bool inventoryOpen;
    private Canvas inventory;
    private Player player;
    public bool storageOpen;
    public bool shopOpen;
    public Text levelText;
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        // DontDestroyOnLoad(gameObject);
        Debug.Log("testtest");
        Debug.Log(GameObject.Find("InventoryCanvas"));
        Debug.Log(GameObject.Find("InventoryCanvas").GetComponent<Canvas>());
        inventory = GameObject.Find("InventoryCanvas").GetComponent<Canvas>();
        player = GameObject.Find("Player").GetComponent<Player>();
        inventoryOpen = false;
        storageOpen = false;
        DontDestroyOnLoad(GameObject.Find("InventoryCanvas"));
        DontDestroyOnLoad(GameObject.Find("StorageInventoryCanvas"));
        DontDestroyOnLoad(GameObject.Find("Player"));
        DontDestroyOnLoad(GameObject.Find("HUD"));
        levelText.text = "Level " + (LevelInfo.level + 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !storageOpen && !shopOpen)
        {
            if (inventoryOpen)
            {
                inventory.enabled = false;
                inventoryOpen = false;
                Time.timeScale = 1;
                Enemy.setGamePause(false);
                paused = false;
            } else
            {
                // player.coinsNumber.text = player.getCoins().ToString();
                inventory.enabled = true;
                inventoryOpen = true;
                Time.timeScale = 0;
                Enemy.setGamePause(true);
                paused = true;
            }
        }
    }

    public void gameOverScreen()
    {
        SceneManager.MoveGameObjectToScene(GameObject.Find("InventoryCanvas"), SceneManager.GetActiveScene());
        SceneManager.MoveGameObjectToScene(GameObject.Find("Player"), SceneManager.GetActiveScene());
        SceneManager.MoveGameObjectToScene(GameObject.Find("HUD"), SceneManager.GetActiveScene());
        Debug.Log("Game Over Screen");
        Time.timeScale = 1;
        SceneManager.LoadScene("GameOver");
    }

    public void victoryScreen()
    {
        Debug.Log("Victory Screen");
        Time.timeScale = 1;
        SceneManager.LoadScene("Victory");
    }
}
