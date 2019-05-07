using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldHealthPotion : WorldPotion
{
    #region Editor Variables
    [SerializeField]
    private float healingAmount;
    #endregion

    protected override void OnTriggerEnter2D(Collider2D c)
    {
        base.OnTriggerEnter2D(c);
        GameObject other = c.gameObject;
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            player.ReceiveHealing(healingAmount);
        }
    }
}
