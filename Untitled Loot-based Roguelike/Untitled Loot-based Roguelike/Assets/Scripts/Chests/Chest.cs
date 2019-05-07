using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    #region gameObject_variables
    [SerializeField]
    [Tooltip("Enemy spawn")]
    private GameObject mimic;

    [SerializeField]
    [Tooltip("Chance to spawn mimic")]
    private float mimicChance;

    [SerializeField]
    [Tooltip("Item")]
    private GameObject[] drop;

    [SerializeField]
    [Tooltip("Item chances")]
    private float[] dropRates;

    [SerializeField]
    [Tooltip("Coin chance")]
    private float coinChance;

    public GameObject coin;
    public int coinMin;
    public int coinMax;
    #endregion

    #region helper_functions
    IEnumerator DeleteChest()
    {
        yield return new WaitForSeconds(0.3f);
        if (Random.Range(0f,1f) > mimicChance) {
            bool dropped = false;
            while (!dropped)
            {
                for (int itemToDrop = 0; itemToDrop < drop.Length; itemToDrop++)
                {
                    if (Random.value < dropRates[itemToDrop])
                    {
                        dropped = true;
                        Instantiate(drop[itemToDrop], transform.position + (Vector3)Random.insideUnitCircle, transform.rotation);
                    }
                }
                if (Random.value < coinChance)
                {
                    dropped = true;
                    int coinsToSpawn = Random.Range(coinMin, coinMax);
                    for (int i = 0; i < coinsToSpawn; i++)
                    {
                        Instantiate(coin, gameObject.transform.position + (Vector3)Random.insideUnitCircle, Quaternion.identity);
                    }
                }
            }
        }
        else
        {
            Instantiate(mimic, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }

    public void Interact()
    {
        StartCoroutine(DeleteChest());
    }
    #endregion
}
