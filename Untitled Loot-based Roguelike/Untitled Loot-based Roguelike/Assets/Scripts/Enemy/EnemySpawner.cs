using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    #region Editor Variables
    /*
     * precondition: Enemy is a Monobehavior child class
     */
    [SerializeField]
    [Tooltip("The Prefabs of enemies being spawned")]
    private Enemy[] enemies;

    [SerializeField]
    [Tooltip("the radius vector indicating the range of spawning")]
    private Vector2 spawnRange;

    [SerializeField]
    [Tooltip("time interval between enemies spawn")]
    private float[] spawnCD;

    [SerializeField]
    [Tooltip("minimum number of enemies of each type that can spawn in one room")]
    private int minEnemiesEach;

    [SerializeField]
    [Tooltip("max number of enemies of each type that can spawn in one room")]
    private int maxEnemiesEach;

    [SerializeField]
    private int minEnemies;

    [SerializeField]
    private int maxEnemies;

    private int enemyCount;

    private bool spawned;

    public bool isStartingRoom;

    public bool isSafeRoom;

    #endregion
    // Start is called before the first frame update
    void Start()
    {
        // Rooms only spawn enemies once.
        spawned = false;
        enemyCount = 0;
        Debug.Log("level: " + LevelInfo.level);
        float toMult = 1 + (LevelInfo.level / 3f);
        minEnemiesEach = (int) System.Math.Round(minEnemiesEach * toMult);
        maxEnemiesEach = (int) System.Math.Round(maxEnemiesEach * toMult);
        minEnemies = (int) System.Math.Round(minEnemies * toMult);
        maxEnemies = (int) System.Math.Round(maxEnemies * toMult);
    }

    public void spawnEnemies()
    {
        if (!spawned)
        {
            StartCoroutine(spawn());
        }
        spawned = true;
    }

    /**
     * Spawn the i-th enemy at a random location in range spawnRange
     */
    private IEnumerator spawn()
    {
        yield return new WaitForSeconds(1f);
        int numEnemiesToSpawn = Random.Range(minEnemies, maxEnemies);
        while (enemyCount < numEnemiesToSpawn)
        {
            int enemyToSpawn = Random.Range(0, enemies.Length);
            int numEnemies = Random.Range(minEnemiesEach, maxEnemiesEach);
            for (int i = 0; i < numEnemies; i++)
            {
                if (enemyCount >= numEnemiesToSpawn)
                {
                    break;
                }
                float degree = Random.Range(0f, 360f);
                float length = Random.Range(0f, 1f);
                Enemy enemy = Instantiate(enemies[enemyToSpawn],
                    gameObject.transform.position + Quaternion.Euler(0, 0, degree) * spawnRange * length,
                    Quaternion.identity);
                enemy.transform.SetParent(transform);
                enemyCount++;
                yield return new WaitForSeconds(0.5f);
            }
            // yield return new WaitForSeconds(spawnCD[i]);
        }
        StartCoroutine(removeDoors());
    }

    private IEnumerator removeDoors()
    {
        while (transform.childCount > 0)
        {
            yield return new WaitForSeconds(0.5f);
        }
        transform.parent.Find("Door Blocks").gameObject.SetActive(false);
    }
}
