using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class MoveableEnemy : Enemy
{
    #region Editor Variables
    [SerializeField]
    [Tooltip("Movement speed of the enemey, in unit 1 unit length per second")]
    protected float m_speed = 5;
    #endregion

    #region Cached Components
    protected Rigidbody2D rb;
    #endregion

    #region Public Read-only Properties
    public bool IsIdle => !IsMoving;
    abstract public bool IsMoving { get; }
    #endregion

    #region Protected Methods
    protected virtual void move(Vector2 destination, float tolerance = 0.5f)
    {
        if (gamePaused)
        {
            return;
        }
        Vector2 dir = destination - (Vector2)transform.position;
        if (dir.magnitude < tolerance)
        {
            dir = Vector2.zero;
        }
        else
        {
            dir = dir.normalized;
        }

        if (Vector2.Dot(rb.velocity, dir * m_speed) < (dir * m_speed).magnitude * (dir * m_speed).magnitude)
        {
            rb.AddForce(-rb.velocity * rb.mass);
            rb.AddForce(dir * m_speed * rb.mass * chilled);
        }
    }
    #endregion

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
    }
}
