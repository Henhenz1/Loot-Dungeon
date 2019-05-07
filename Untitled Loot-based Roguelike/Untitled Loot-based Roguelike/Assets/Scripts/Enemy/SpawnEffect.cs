using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEffect : MonoBehaviour
{
    [SerializeField]
    private float lifespan;

    private float countdown;

    // Start is called before the first frame update
    void Start()
    {
        countdown = lifespan;
    }

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown < 0)
        {
            Destroy(gameObject);
        }
    }
}
