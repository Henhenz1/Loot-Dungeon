using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    [SerializeField]
    private Skeleton skeleton;

    public bool dealtDamage()
    {
        return m_dealtDamage;
    }
    public bool m_dealtDamage { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        m_dealtDamage = false;
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion up = Quaternion.Euler(0, 0, 90),
                 left = Quaternion.Euler(0, 0, 180),
                 down = Quaternion.Euler(0, 0, -90),
                right = Quaternion.Euler(0, 0, 0);
        if (skeleton.Orientation == Vector2.up)
        {
            transform.rotation = up;
        }
        else if (skeleton.Orientation == Vector2.left)
        {
            transform.rotation = left;
        }
        else if(skeleton.Orientation == Vector2.down)
        {
            transform.rotation = down;
        } else
        if (skeleton.Orientation == Vector2.right)
        {
            transform.rotation = right;
        }
    }

    void OnEnable()
    {
        m_dealtDamage = false;
    }

    private void OnTriggerEnter2D(Collider2D c)
    {
        GameObject other = c.gameObject;
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            player.TakeDamage(skeleton.AttackDamage);
            m_dealtDamage = true;
        }
    }
}
