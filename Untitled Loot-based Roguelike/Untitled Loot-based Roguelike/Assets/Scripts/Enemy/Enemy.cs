using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Basic Enemy Super Class
 */
public abstract class Enemy : MonoBehaviour
{
    #region Editor Variables
    [SerializeField]
    [Tooltip("The hp of the object at its initialization")]
    private float m_maxHealth;

    [SerializeField]
    [Tooltip("The attack damage of the object at its initialization")]
    private float m_attackDamage;

    [SerializeField]
    private GameObject[] drop;      // temporary

    [SerializeField]
    private float[] dropRates;

    [SerializeField]
    private GameObject coin;

    [SerializeField]
    private int coinMin;

    [SerializeField]
    private int coinMax;

    [SerializeField]
    private GameObject SpawnEffect;

    [SerializeField]
    private BurningEffect burningeffectPrefab;
    private BurningEffect currBurn;

    private bool onFire = false;        // avoid stacking DoTs
    #endregion

    #region Protected Properties
    protected float CurrHealth { get; set; }
    SpriteRenderer spriteRenderer;
    Color originalColor;
    #endregion

    #region Static Properties
    protected static bool gamePaused;
    #endregion

    float backupColdTimer;      // needed because sometiles Skeleton calls StopAllCoroutines()
    bool coldActive;

    protected virtual void Start()
    {
        float toMult = 1 + (LevelInfo.level / 2f);
        chilled = 1;
        m_maxHealth *= toMult;
        m_attackDamage *= toMult;
        CurrHealth = MaxHealth;
        Instantiate(SpawnEffect, gameObject.transform.position, Quaternion.identity);
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        originalColor = spriteRenderer.color;
    }

    protected virtual void Update()
    {
        backupColdTimer -= Time.deltaTime;
        if (backupColdTimer <= 0 && coldActive)
        {
            coldActive = false;
            chilled = 1;
            spriteRenderer.color = originalColor;
            transform.Find("Frozen").GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    #region Public API

    #region Public Read-only Properties
    public Vector2 Position => transform.position;
    public float MaxHealth => m_maxHealth;
    public float AttackDamage => m_attackDamage;
    abstract public bool IsAttacking { get; }
    public float lastTimeBeHit { get; protected set; }
    public float chilled { get; protected set; }         // 1 = normal, 0.5 = slowed, 0 = frozen
    #endregion

    #region Public Methods
    /**
     * Coroutine that lets the enemy sprite to flash in color in dur of time in seconds
     */
     protected IEnumerator flashColor(Color color, float dur)
    {
        spriteRenderer.color = color;
        yield return new WaitForSeconds(dur);
        spriteRenderer.color = originalColor;
    }

    IEnumerator TakeFireDamageOverTime(float tickLength, float damageAmount, float totalDuration)
    {
        onFire = true;
        float burntTime = 0;
        while (burntTime < totalDuration)
        {
            ReceiveDamage(damageAmount);
            yield return new WaitForSeconds(tickLength);
            burntTime += tickLength;
        }
        onFire = false;     // TODO can't stack DoTs, but might want to be able to extend them
    }

    virtual public void ReceiveFireDamage(float tickLength, float damageAmount, float totalDuration)
    {
        Debug.Log("Starting burn for " + damageAmount + " damage per tick");
        Debug.Log("Duration is " + totalDuration);
        //StartCoroutine(TakeFireDamageOverTime(tickLength, damageAmount, totalDuration));
        if (currBurn == null) { 
            currBurn = Instantiate(burningeffectPrefab, transform);
            currBurn.initialize(tickLength, damageAmount, totalDuration);
        } else
        {
            currBurn.addBurning(totalDuration);
        }
    }

    public void HandleChillEffect(float slowDuration, float freezeDuration)
    {
        if (chilled == 1)
        {
            StartCoroutine(SlowEnemyRoutine(slowDuration));
        }
        else if (chilled == 0.5)
        {
            StopCoroutine(SlowEnemyRoutine(slowDuration));
            StopCoroutine(flashColor(Color.cyan, slowDuration));
            StartCoroutine(FreezeEnemyRoutine(slowDuration));
        }
    }

    IEnumerator SlowEnemyRoutine(float slowDuration)
    {
        BurningEffect burn = GetComponentInChildren<BurningEffect>();
        if (burn != null)
        {
            burn.DestroyBurn();
        }
        coldActive = true;
        chilled = 0.5f;
        StartCoroutine(flashColor(Color.cyan, slowDuration));
        backupColdTimer = slowDuration;
        yield return new WaitForSeconds(slowDuration);
        Debug.Log("ending slow");
        coldActive = false;
        chilled = 1f;
    }

    IEnumerator FreezeEnemyRoutine(float freezeDuration)
    {
        Debug.Log("freezing enemy");
        coldActive = true;
        chilled = 0f;
        gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        StartCoroutine(flashColor(Color.blue, freezeDuration));
        backupColdTimer = freezeDuration;
        transform.Find("Frozen").GetComponent<SpriteRenderer>().enabled = true;
        yield return new WaitForSeconds(freezeDuration);
        Debug.Log("ending freeze");
        transform.Find("Frozen").GetComponent<SpriteRenderer>().enabled = false;
        chilled = 1f;
        coldActive = false;
    }

    /**
     * Let Enemy object take damage 
     */
    virtual public void ReceiveDamage(float damage)
    {
        CurrHealth -= damage;
        lastTimeBeHit = Time.time;
        if (damage > 0)
        {
            Debug.Log("Enemy: Got Hit, Health: " + (CurrHealth + damage) + " -> " + CurrHealth);
        }
        if (CurrHealth <= 0)
        {
            Kill();
        }
    }

    /**
     * Bring Enemy object to death
     */
    virtual public void Kill()
    {
        if (drop.Length > 0)
        {
            for (int itemToDrop = 0; itemToDrop < drop.Length; itemToDrop++)
            {
                if (Random.value < dropRates[itemToDrop])
                {
                    Instantiate(drop[itemToDrop], gameObject.transform.position + (Vector3) Random.insideUnitCircle,
                        Quaternion.identity);
                    break;
                }
            }
        }
        int coinsToSpawn = Random.Range(coinMin, coinMax);
        for (int i = 0; i < coinsToSpawn; i++)
        {
            Instantiate(coin, gameObject.transform.position + (Vector3) Random.insideUnitCircle, Quaternion.identity);
        }
        Destroy(gameObject);
    }
    #endregion

    #endregion

    #region Message
    public static void setGamePause(bool pause)
    {
        if (pause)
        {
            Debug.Log("Enemy: game paused");
        } else
        {
            Debug.Log("Enemy: game continued");
        }
        gamePaused = pause;
    }
    #endregion


}
