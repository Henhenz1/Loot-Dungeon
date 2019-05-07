using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningEffect : MonoBehaviour
{
    [SerializeField]
    private int burnStack;
    public int BurnStack => burnStack;

    [SerializeField]
    private int maxBurnStack;
    public int MaxBurnStack => maxBurnStack;

    [SerializeField]
    private float damagePerTick;
    [SerializeField]
    private float[] durationsOfFires;
    [SerializeField]
    private float timePerTick;
    private float timePerTick_origin;

    private GameObject attachTo;
    private Enemy enemy;
    private bool initialied;

    /**
     * The user has to call initialize(float, float, float) right after instanciate(), otherwise the gameObject will be destroyed. 
     */
    private void Awake()
    {
        attachTo = transform.parent.gameObject;
        if (!attachTo.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
        enemy = attachTo.GetComponent<Enemy>();
        timePerTick_origin = timePerTick;
        durationsOfFires = new float[maxBurnStack];
    }

    public void initialize(float tickLength, float damageAmount, float totalDuration)
    {
        if (!initialied)
        {
            initialied = true;
            damagePerTick = damageAmount;
            timePerTick = tickLength;
            addBurning(totalDuration);
        }
    }

    /**
     * Add a new fire on the effect
     */
    public void addBurning(float dur, int n = 1)
    {
        for (int i = 0; i < n; i++)
        {
            for (int index = 0; index < durationsOfFires.Length; index++)
            {
                if (durationsOfFires[index] <= 0)
                {
                    durationsOfFires[index] = dur;
                    break;
                }
                else if (durationsOfFires[index] <= dur)
                {
                    durationsOfFires[index] = dur;
                }
            }
        }
    }

    public void DestroyBurn()
    {
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        timePerTick -= Time.deltaTime;
        if (timePerTick <= 0)
        {
            enemy.ReceiveDamage(damagePerTick * burnStack);
            timePerTick = timePerTick_origin;
        }
        int currActiveFires = 0;
        for (int i = 0; i < durationsOfFires.Length; i++)
        {
            if (durationsOfFires[i] > 0)
            {
                durationsOfFires[i] -= Time.deltaTime;
                currActiveFires++;
            }
        }
        burnStack = currActiveFires;
        if (burnStack <= 0)
        {
            Debug.Log("Fire dying");
            Destroy(gameObject);
        }
    }
}
