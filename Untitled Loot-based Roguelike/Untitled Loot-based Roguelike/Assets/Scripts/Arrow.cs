using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// This class defines a player-shot arrow.

public class Arrow : MonoBehaviour
{
    float angle;
    Vector2 direction;
    Player player;
    float damage;

    [SerializeField]
    [Tooltip("Speed of projectile")]
    public float bulletSpeed;
    [SerializeField]
    [Tooltip("Lifetime of projectile")]
    [Range(1, 3)]
    public float bulletLifetime;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        angle = player.angle;
        direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        GetComponent<Rigidbody2D>().AddForce(direction * bulletSpeed * 100);
        damage = player.damage;
    }

    // Update is called once per frame
    void Update()
    {
        bulletLifetime -= Time.deltaTime;
        if (bulletLifetime <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        try
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                collision.gameObject.GetComponent<Enemy>().ReceiveDamage(damage);
                Destroy(gameObject);
            }
            else if (collision.gameObject.CompareTag("Walls"))
            {
                Destroy(gameObject);
            }
        } 
        catch (NullReferenceException e)
        {
            
        }
    }

}
