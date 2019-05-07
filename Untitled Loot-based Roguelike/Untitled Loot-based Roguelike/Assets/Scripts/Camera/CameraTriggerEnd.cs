using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTriggerEnd : MonoBehaviour
{
    public bool isStartingRoom;
    public bool isSafeRoom;
    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.CompareTag("Player"))
        {
            Player player = coll.gameObject.GetComponent<Player>();
            player.isChangingRoom = false;
            if (!isStartingRoom && !isSafeRoom)
            {
                EnemySpawner enemySpawner = transform.parent.Find("Enemy Spawner").GetComponent<EnemySpawner>();
                Transform enemySpawnerTransform = enemySpawner.transform;
                Debug.Log(transform.parent.name);
                transform.parent.Find("Door Blocks").gameObject.SetActive(true);
                enemySpawner.spawnEnemies();
                isSafeRoom = true;
            }
        }
    }
}
