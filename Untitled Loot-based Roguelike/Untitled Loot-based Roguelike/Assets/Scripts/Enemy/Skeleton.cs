using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Skeleton : MoveableEnemy
{
    #region Public Members
    public Vector2 Orientation => orientation;
    #endregion

    #region Private Variables
    /**
     * Current facing, can only be one of the four unit Vector2. 
     */
    private Vector2 orientation;
    private GameObject cachedAggro;

    private bool m_isMovingtoTarget;
    private bool m_isMovingtoLoc;
    private bool m_isAttacking;
    private bool m_isIdle;
    private bool m_playerInSight => vision.aggro != null;

    [SerializeField]
    private float m_chargeTime = 0.5f;
    [SerializeField]
    private float m_attackCooldownTime = 1f;
    #endregion

    #region Cached Reference
    [SerializeField]
    private HitBox hitBox;
    [SerializeField]
    private Vision vision;

    private Animator cr_anim;
    #endregion

    public override bool IsAttacking => m_isAttacking;

    public override bool IsMoving => m_isMovingtoTarget || m_isMovingtoLoc;

    public override void Kill()
    {
        base.Kill();
    }

    public override void ReceiveDamage(float damage)
    {
        vision.StartCoroutine(vision.alertSearch());
        if (IsIdle)
        {
            cr_anim.Play("Hit");
        } else
        {
            if (orientation == Vector2.right)
            {
                cr_anim.Play("MoveRightHit");
            } else if (orientation == Vector2.up)
            {
                cr_anim.Play("MoveUpHit");
            }
            else if (orientation == Vector2.left)
            {
                cr_anim.Play("MoveLeftHit");
            }
            else if (orientation == Vector2.down)
            {
                cr_anim.Play("MoveDownHit");
            }
        }

        
        base.ReceiveDamage(damage);
    }

    protected override void move(Vector2 destination, float tolerance)
    {
        base.move(destination, tolerance);
    }

    protected override void Start()
    {
        base.Start();
        cr_anim = gameObject.GetComponentInChildren<Animator>();
        hitBox.gameObject.SetActive(false);
        StartCoroutine(Idle());
    }

    protected override void Update()
    {
        base.Update();
        if (m_isIdle)
        {
            //Debug.Log("Idle");
            if (vision.aggro != null)
            {
                StopAllCoroutines();
                cachedAggro = vision.aggro;
                StartCoroutine(MovetoTarget(vision.aggro));
            }
        }
        else if (m_isMovingtoTarget)
        {
            //Debug.Log("Moving to Target");
            if (vision.aggro == null)
            {
                //Debug.Log("Target Lost...");
                StopAllCoroutines();
                StartCoroutine(MovetoLoc(cachedAggro.transform.position));
            } else if (Utility.Utility.near(transform.position, vision.aggro.transform.position, 2f))
            {
                //Debug.Log("Near Player!");
                StopAllCoroutines();
                StartCoroutine(Attack(Utility.Utility.findOrientation(transform.position, vision.aggro.transform.position), m_chargeTime));
            }

        }
        else if (m_isMovingtoLoc)
        {
            //Debug.Log("Moving to Loc");
            if (vision.aggro != null)
            {
                StopAllCoroutines();
                StartCoroutine(MovetoTarget(vision.aggro));
            }
        }
        else if (m_isAttacking) { }
    }

    IEnumerator Attack(Vector2 orientation, float chargeTime)
    {
        Debug.Log("Attacking");
        m_isIdle = false;
        m_isMovingtoTarget = false;
        m_isAttacking = true;
        hitBox.gameObject.SetActive(false);
        this.orientation = orientation;
        rb.velocity = Vector2.zero;

        if (orientation == Vector2.right)
        {
            cr_anim.Play("StabRight");
        }
        else if (orientation == Vector2.up)
        {
            cr_anim.Play("StabUp");
        }
        else if (orientation == Vector2.left)
        {
            cr_anim.Play("StabLeft");
        }
        else if (orientation == Vector2.down)
        {
            cr_anim.Play("StabDown");
        }

        Vector2 initialPos = transform.position, holdbackPos = (Vector2) transform.position - orientation * 0.5f;
        Debug.Log("Charging");
        rb.bodyType = RigidbodyType2D.Static;
        for(float timer = 0f; timer < chargeTime; timer += Time.deltaTime)
        {
            transform.position = Vector2.Lerp(initialPos, holdbackPos, timer/chargeTime);
            yield return null;
        }
        Debug.Log("Charged");
        transform.position = initialPos;
        hitBox.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        hitBox.gameObject.SetActive(false);
        Debug.Log("CDing");
        rb.bodyType = RigidbodyType2D.Dynamic;
        yield return new WaitForSeconds(m_attackCooldownTime);
        Debug.Log("CDed");
        StartCoroutine(Idle());
        yield return null;
    }

    IEnumerator Idle()
    {
        m_isMovingtoTarget = false;
        m_isMovingtoLoc = false;
        m_isAttacking = false;
        m_isIdle = true;
        cr_anim.Play("Idle");
        while (true)
        {
            if (rb.velocity.magnitude > 0) rb.AddForce(-rb.velocity / rb.velocity.magnitude);
            yield return null;
        }
    }

    IEnumerator MovetoLoc(Vector2 loc)
    {
        m_isIdle = false;
        m_isMovingtoTarget = false;
        m_isMovingtoLoc = true;
        bool nearTarget = false;
        cr_anim.Play("Move");
        while (!nearTarget)
        {
            orientation = Utility.Utility.findOrientation(transform.position, loc);
            move(loc, 0.5f);
            if (((Vector2) transform.position - loc).magnitude < 0.5f)
            {
                nearTarget = true;
            }
            yield return null;
        }
        StartCoroutine(Idle());
        yield return null;
    }

    IEnumerator MovetoTarget(GameObject other)
    {
        m_isIdle = false;
        m_isMovingtoLoc = false;
        m_isMovingtoTarget = true;
        cr_anim.Play("Move");
        Vector2 targetLoc = other.transform.position;
        while (vision.aggro != null)
        {
            orientation = Utility.Utility.findOrientation(transform.position, targetLoc);
            setAnimMoveOrientation();
            move(targetLoc, 0.5f);
            targetLoc = other.transform.position;
            yield return null;
        }
    }

    void setAnimMoveOrientation()
    {
        if (orientation == Vector2.right)
        {
            cr_anim.SetBool("OrientRight", true);
            cr_anim.SetBool("OrientUp", false);
            cr_anim.SetBool("OrientLeft", false);
            cr_anim.SetBool("OrientDown", false);
        } else if (orientation == Vector2.up)
        {
            cr_anim.SetBool("OrientRight", false);
            cr_anim.SetBool("OrientUp", true);
            cr_anim.SetBool("OrientLeft", false);
            cr_anim.SetBool("OrientDown", false);
        }
        else if(orientation == Vector2.left)
        {
            cr_anim.SetBool("OrientRight", false);
            cr_anim.SetBool("OrientUp", false);
            cr_anim.SetBool("OrientLeft", true);
            cr_anim.SetBool("OrientDown", false);
        } else if (orientation == Vector2.down)
        {
            cr_anim.SetBool("OrientRight", false);
            cr_anim.SetBool("OrientUp", false);
            cr_anim.SetBool("OrientLeft", false);
            cr_anim.SetBool("OrientDown", true);
        }
    }
}
