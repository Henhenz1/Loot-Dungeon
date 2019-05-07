using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    /*
     * this feels like a really crappy way of implementing hitboxes and attacks
     * TODO rework with raycasting (?) when not ded
     */

    [HideInInspector]
    public int auraType;
    [HideInInspector]
    public bool damageApplied;
    [HideInInspector]
    public float damageValue;
    private float knockbackValue;       // todo update based on weapon generating hitbox

    // Fire DoT values
    [HideInInspector]
    public float tickLength;
    [HideInInspector]
    public float tickDamage;
    [HideInInspector]
    public float totalDuration;

    // Cold effect values
    [HideInInspector]
    public float slowDuration;
    [HideInInspector]
    public float freezeDuration;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject other = collider.gameObject;
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            enemy.ReceiveDamage(damageValue);
            if (auraType == 1)
            {
                enemy.ReceiveFireDamage(tickLength, tickDamage, totalDuration);
            }
            if (auraType == 2)
            {
                enemy.HandleChillEffect(slowDuration, freezeDuration);
            }
            other.GetComponent<Rigidbody2D>().AddForce(
                new Vector2(
                        other.transform.position.x - transform.position.x
                        , other.transform.position.y - transform.position.y
                    ).normalized * other.GetComponent<Rigidbody2D>().mass * 100);    //Edit by Bill        
            Debug.Log("hit enemy, Damage = " + damageValue);
        }
    }
}


