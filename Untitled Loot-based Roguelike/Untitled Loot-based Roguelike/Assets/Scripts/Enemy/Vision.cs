using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vision : MonoBehaviour
{
    public GameObject aggro { get; private set;}

    private CircleCollider2D cr_collider;

    [SerializeField]
    private Enemy whose;

    private void Start()
    {
        cr_collider = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.name == "Player")
        {
            aggro = collider.gameObject;
        }
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject == aggro)
        {
            aggro = null;
        }
    }

    public IEnumerator alertSearch()
    {
        float originRadius = cr_collider.radius;
        cr_collider.radius *= 10;
        yield return new WaitForSeconds(0.3f);
        cr_collider.radius = originRadius;
    }
}
