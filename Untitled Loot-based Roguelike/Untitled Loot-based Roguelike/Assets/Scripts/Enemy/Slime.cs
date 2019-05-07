using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Slime : MoveableEnemy
{
    #region Editor Variables
    [SerializeField]
    [Tooltip("Self bleeding health per second")]
    private float m_bleed = 5;
    #endregion

    #region Private Variables
    private SlimeVision sv;
    private bool m_isAttacking;
    private Animator anim;
    #endregion

    #region Public API
    public override bool IsMoving => sv.stalking;
    public override bool IsAttacking => m_isAttacking;
    public AudioClip slimeDamagedAudio;
    public AudioSource source;
    #endregion

    protected override void move(Vector2 destination, float tolerance)
    {
        base.move(destination, tolerance);

        //rb.velocity = dir * m_speed;
        // Reduces self health along the way
        ReceiveDamage(m_bleed * Time.deltaTime);
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        sv = GetComponentInChildren<SlimeVision>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (sv.stalking)
        {
            move(sv.target.transform.position, 0.5f);
        } else
        {
            rb.velocity = Vector2.zero;
        }
    }

    public override void ReceiveDamage(float damage)
    {
        if (damage > 5) 
        {
            source.PlayOneShot(slimeDamagedAudio, 1.0f);
            anim.Play("SlimeDamaged");
        }
        base.ReceiveDamage(damage);
    }

    private void OnCollisionStay2D(Collision2D c)
    {
        GameObject other = c.collider.gameObject;
        if (other.CompareTag("Player"))
        {
            m_isAttacking = true;
            Player player = other.GetComponent<Player>();
            player.TakeDamage(AttackDamage * Time.deltaTime);
        }
        m_isAttacking = false;
    }
}
