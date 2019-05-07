using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    public int numLevels;
    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.CompareTag("Player"))
        {
            if ((LevelInfo.level + 1) == numLevels)
            {
                SceneManager.MoveGameObjectToScene(GameObject.Find("InventoryCanvas"), SceneManager.GetActiveScene());
                SceneManager.MoveGameObjectToScene(GameObject.Find("Player"), SceneManager.GetActiveScene());
                SceneManager.MoveGameObjectToScene(GameObject.Find("HUD"), SceneManager.GetActiveScene());
                Time.timeScale = 1;
                SceneManager.LoadScene("Victory");
                return;
            }
            LevelInfo.level++;
            LevelInfo.numRooms += 5;
            if ((LevelInfo.level + 1) % 2 == 0)
            {
                LevelInfo.gridSize++;
            }
            SceneManager.LoadScene("PlayNext");
            GameObject.Find("Player").transform.position = new Vector3(0, 0, 0);
            GameObject.Find("Level Generator").GetComponent<LevelGeneration>().Awake();
        }
    }
}
