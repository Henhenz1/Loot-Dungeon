using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * A generic potion object that is on the ground and can be picked up by the player
 */
public class WorldPotion : WorldObject
{
    /**
     * This generic potion will do nothing when the Player pick it up
     */
    protected virtual void OnTriggerEnter2D(Collider2D c)
    {
        GameObject other = c.gameObject;
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
