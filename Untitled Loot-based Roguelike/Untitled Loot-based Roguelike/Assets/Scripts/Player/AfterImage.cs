using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AfterImage : MonoBehaviour
{
    public float lifespan = 1;
    private float birthTime;

    public Animator anim;

    // Start is called before the first frame update
    void Awake()
    {
        birthTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - birthTime > lifespan)
        {
            Destroy(gameObject);
        }
    }

    public void PlayDashAnimation(string name)
    {
        anim.Play(name);
    }
}
