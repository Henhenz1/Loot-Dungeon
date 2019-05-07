using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeVision : MonoBehaviour
{
    [HideInInspector]
    public bool stalking;
    [HideInInspector]
    public GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.name == "Player")
        {
            if (!stalking)
            {
                stalking = true;
                target = collider.gameObject;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.name == "Player")
        {
            stalking = false;
        }
    }

    
}
