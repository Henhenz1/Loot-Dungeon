using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] // for health bar
    private HUD m_HUD;

    [SerializeField]
    private GameManager m_gameManager;

    [SerializeField]
    private Dash dash;

    public Animator m_HeadAnimator;
    public Animator m_TorsoAnimator;
    public Animator m_BottomAnimator;

    #region player_variables
    Rigidbody2D rb;                     // possibly unneccessary
    SpriteRenderer[] indicators;        // 0 is player, 1-4 are facing indicators, 5-8 are attack indicators
    PolygonCollider2D[] attackBoxes;
    Hitbox[] hitboxes;
    Coroutine currentDash;
    SpriteRenderer[] spriteRenderers;
    #endregion

    #region stat_variables
    private int prevPoints;
    private int statPoints;
    private int str;
    private int dex;
    private int intt;
    private int vit;
    private int agi;
    private int[] stats;
    private int[] statsToAdd;
    public Text statPointsNumber;
    public Text[] pointsText;
    private Transform statsUI;
    #endregion

    #region coin_variables
    private int coins;
    public Text coinsNumber;
    #endregion

    #region movement_variables
    public float moveSpeed;             // How fast the player moves
    public bool Moving => moving;
    private float xAxis;                // X-input for movement
    private float yAxis;                // Y-input for movement
    private Vector2 movementVector;     // tool to calculate movement
    private bool moving;                // true if player is moving
    Vector2 currDirection;              // for interaction
    #endregion

    #region attack_variables
    public float attackSpeed;           // How many seconds required between attacks; defined by weapon eventually
    private float attackTimer;          // resets to attackSpeed after attack; may only attack when timer < 0
    public float damage;                // power of player's attacks; defined by weapon eventually
    public int auraType;
    private Vector3 mousePosition;
    private float mouseX;               // possibly unneccessary
    private float mouseY;               // possibly unneccessary
    private int facing;                 // 0-3 representing R, U, L, D
    public int Facing => facing;
    private float angle;                // angle facing in radians w.r.t. right horizontal
    public int attackType;             // 0,1,2 representing melee, ranged, spear (can add other modes later, e.g. magic, summoning) -- only public for demo, todo make private again after implementing spear properly
    private GameObject projectile;
    private GameObject faceIndicator;
    #endregion

    #region health_variables
    public float health;                // current health of player
    public float maxhealth;
    private bool mercyInvuln;           // tells if player currently has mercy invulnerability
    public float mercyInvulnLength;     // length of mercy invuln 
    private float mercyInvulnTimer;     // resets to mercyInvulnLength whenever invuln ends
    private bool dying;                 // is the player currently dying?
    #endregion

    #region level_variables
    public GameObject levelGenerator;
    private int xPos, yPos;
    private Room[,] rooms;
    public bool isChangingRoom;
    public bool isCameraMoving;
    #endregion

    #region constants
    private const int faceMin = 0;
    private const int faceMax = 4;
    private const int faceIndMin = 1;
    private const int faceIndMax = 5;

    private const int faceRight = 0;
    private const int faceUp = 1;
    private const int faceLeft = 2;
    private const int faceDown = 3;

    private const int atkTypeMelee = 0;
    private const int atkTypeRanged = 1;
    private const int atkTypeSpear = 2;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        faceIndicator = transform.GetChild(0).gameObject;
        indicators = GetComponentsInChildren<SpriteRenderer>();
        for (int i = 2; i < indicators.Length - 3; i++) // - 3 because head/torso/bottom is a child too
        {
            indicators[i].enabled = false;
        }
        attackBoxes = GetComponentsInChildren<PolygonCollider2D>();
        for (int i = 0; i < attackBoxes.Length; i++)
        {
            attackBoxes[i].enabled = false;
        }
        hitboxes = GetComponentsInChildren<Hitbox>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        xAxis = 0;
        yAxis = 0;
        movementVector = new Vector2(0, 0);

        facing = 0;

        mercyInvuln = false;
        mercyInvulnTimer = 0;
        dying = false;

        maxhealth = health;

        m_HUD.UpdateText(1.0f * health, maxhealth);

        prevPoints = 10;
        statPoints = 10;
        statPointsNumber.text = statPoints.ToString();
        coins = 0;
        coinsNumber.text = coins.ToString();
        stats = new int[5];
        statsToAdd = new int[5];
        for (int i = 0; i < pointsText.Length; i++)
        {
            stats[i] = 5;
            pointsText[i].text = stats[i].ToString();
        }
        statsUI = GameObject.Find("InventoryCanvas").transform.Find(
                "Inventory").Find("Stat points");
        statsUI.Find("Control Buttons").gameObject.SetActive(false);
        updateStats();

        LevelGeneration levelGen = levelGenerator.GetComponent<LevelGeneration>();
        xPos = levelGen.gridSizeX;
        yPos = levelGen.gridSizeY;
        rooms = levelGen.getRooms();
    }

    // Update is called once per frame
    void Update()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");

        movementVector = new Vector2(xAxis, yAxis).normalized;      // if movementVector is not normalized, then movement is 1.4x faster when player is moving diagonally
        movementVector *= Time.deltaTime * moveSpeed;
        if (!dying && !isCameraMoving)
        {
            rb.MovePosition(rb.position + movementVector);
            if (movementVector.magnitude > 0)
            {
                m_HeadAnimator.SetBool("Moving", true);
                m_HeadAnimator.SetLayerWeight(1, 1);
                m_TorsoAnimator.SetBool("Moving", true);
                m_TorsoAnimator.SetLayerWeight(1, 1);
                m_BottomAnimator.SetBool("Moving", true);
                m_BottomAnimator.SetLayerWeight(1, 1);                    // when moving, changes animation layer to the "Moving" layer
            }
            else
            {
                m_HeadAnimator.SetBool("Moving", false);
                m_HeadAnimator.SetLayerWeight(1, 0);
                m_TorsoAnimator.SetBool("Moving", false);
                m_TorsoAnimator.SetLayerWeight(1, 0);
                m_BottomAnimator.SetBool("Moving", false);
                m_BottomAnimator.SetLayerWeight(1, 0);                    // when idle, changes animation layer to the "Idle" layer
            }
        }
        
        mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));

        DetermineFaceChange();
        HandleFaceChange();

        mercyInvulnTimer -= Time.deltaTime;
        if (mercyInvulnTimer < 0)
        {
            mercyInvuln = false;
        }

        attackTimer -= Time.deltaTime;
        if (Input.GetButton("Fire1") && attackTimer <= 0 && !dying)
        {
            Attack();
            m_TorsoAnimator.SetTrigger("MeleeAttack");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }

        dash.dashCDTimer += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space) && dash.dashCDTimer > dash.dashCooldown)
        {
            currentDash = StartCoroutine(Dash(movementVector));
        }
    }

    #region level_functions
    // Changes player position to a new room, returns this new room.
    public Room changeRoom(int xChange, int yChange)
    {
        xPos += xChange;
        yPos += yChange;
        return rooms[xPos, yPos];
    }
    #endregion

    #region stat_functions

    // Add a stat point to the player
    public void gainPoint()
    {
        if (statPoints < 1)
        {
            statsUI.Find("Stat Buttons").gameObject.SetActive(true);
        }
        statPoints++;
        statPointsNumber.text = statPoints.ToString();
        prevPoints++;
    }

    public void addPoint(int i)
    {
        if (statPoints < 1)
        {
            return;
        }
        
        statPoints -= 1;
        statPointsNumber.text = statPoints.ToString();
        statsToAdd[i]++;
        pointsText[i].text = (stats[i] + statsToAdd[i]).ToString();
        if (statPoints < 1)
        {
            statsUI.Find("Stat Buttons").gameObject.SetActive(false);
        }
        statsUI.Find("Control Buttons").gameObject.SetActive(true);
    }

    public void resetPoints()
    {
        if (statPoints != prevPoints)
        {
            statsUI.Find("Stat Buttons").gameObject.SetActive(true);
            statsUI.Find("Control Buttons").gameObject.SetActive(false);
        } else
        {
            return;
        }
        statPoints = prevPoints;
        statPointsNumber.text = statPoints.ToString();
        for (int i = 0; i < stats.Length; i++)
        {
            pointsText[i].text = stats[i].ToString();
            statsToAdd[i] = 0;
        }
    }

    public void acceptPoints()
    {
        prevPoints = statPoints;
        for (int i = 0; i < statsToAdd.Length; i++)
        {
            stats[i] += statsToAdd[i];
        }
        updateStats();
        statsUI.Find("Control Buttons").gameObject.SetActive(false);
    }

    private void updateStats()
    {
        str = stats[0];
        dex = stats[1];
        intt = stats[2];
        vit = stats[3];
        agi = stats[4];
        updateVit();
        updateAgi();
        statsToAdd = new int[5];
    }

    private void updateVit()
    {
        health += 5 * statsToAdd[3];
        maxhealth += 5 * statsToAdd[3];
        m_HUD.UpdateHealth(1.0f * health / maxhealth);
        m_HUD.UpdateText(1.0f * health, maxhealth);
    }

    private void updateAgi()
    {
        moveSpeed += statsToAdd[4] * 0.3f;
    }
    #endregion

    #region coin_functions
    public int getCoins()
    {
        return coins;
    }

    public void addCoin()
    {
        coins++;
        coinsNumber.text = coins.ToString();
    }

    public void addCoins(int num)
    {
        coins += num;
        coinsNumber.text = coins.ToString();
    }

    public void reduceCoins(int num)
    {
        coins -= num;
        coinsNumber.text = coins.ToString();
    }
    #endregion

    #region inventory_functions
    public void Equip(Item item)
    {
        item.equip(gameObject);

        Debug.Log("Equipped: " + item.name);

        m_HUD.UpdateHealth(1.0f * health / maxhealth);
        m_HUD.UpdateText(1.0f * health, maxhealth);

        if (item.name == "Helmet")
        {
            Transform head = this.gameObject.transform.GetChild(3); // get head child
            SpriteSwap[] headScripts = head.gameObject.GetComponents<SpriteSwap>();
            foreach (SpriteSwap headScript in headScripts)
            {
                headScript.SpriteSheetName += "A";
            }
        }
        if (item.name == "Armor") // affects both torso and leg for now because no item slot for grieves
        {
            Transform torso = this.gameObject.transform.GetChild(4); // get torso child
            SpriteSwap[] torsoScripts = torso.gameObject.GetComponents<SpriteSwap>();
            foreach (SpriteSwap torsoScript in torsoScripts)
            {
                torsoScript.SpriteSheetName += "A"; // A for armor
            }
            Transform leg = this.gameObject.transform.GetChild(5); // get leg child
            SpriteSwap[] legScripts = leg.gameObject.GetComponents<SpriteSwap>();
            foreach (SpriteSwap legScript in legScripts)
            {
                legScript.SpriteSheetName += "A";
            }
        }
        if (item.name == "Axe")
        {
            Transform torso = this.gameObject.transform.GetChild(4); // get torso child
            SpriteSwap[] torsoScripts = torso.gameObject.GetComponents<SpriteSwap>();
            for (int i = 0; i < 4; i++)
            {
                torsoScripts[i + 4].SpriteSheetName += "X"; // X for axe
            }
        }
        if (item.name == "Spear(Clone)(Clone)")
        {
            Transform torso = this.gameObject.transform.GetChild(4); // get torso child
            SpriteSwap[] torsoScripts = torso.gameObject.GetComponents<SpriteSwap>();
            for (int i = 0; i < 4; i++)
            {
                torsoScripts[i + 4].SpriteSheetName += "S"; // S for spear
            }
        }
        if (item.name == "Sword(Clone)(Clone)")
        {
            Transform torso = this.gameObject.transform.GetChild(4); // get torso child
            SpriteSwap[] torsoScripts = torso.gameObject.GetComponents<SpriteSwap>();
            for (int i = 0; i < 4; i++)
            {
                torsoScripts[i + 4].SpriteSheetName += "W"; // sets the melee animation to sword
            }
        }
        if (item.name == "Bow")
        {
            Transform torso = this.gameObject.transform.GetChild(4); // get torso child
            SpriteSwap[] torsoScripts = torso.gameObject.GetComponents<SpriteSwap>();
            for (int i = 0; i < 4; i++)
            {
                torsoScripts[i + 4].SpriteSheetName += "B";
            }
        }
    }

    public void Unequip(Item item)
    {
        item.unequip(gameObject);
        m_HUD.UpdateHealth(1.0f * health / maxhealth);
        m_HUD.UpdateText(1.0f * health, maxhealth);

        if (item.name == "Helmet")
        {
            Transform head = this.gameObject.transform.GetChild(3); // get head child
            SpriteSwap[] headScripts = head.gameObject.GetComponents<SpriteSwap>();
            foreach (SpriteSwap headScript in headScripts)
            {
                headScript.SpriteSheetName = headScript.SpriteSheetName.TrimEnd('A');
            }
        }
        if (item.name == "Armor") // affects both torso and leg for now because no item slot for grieves
        {
            Transform torso = this.gameObject.transform.GetChild(4); // get torso child
            SpriteSwap[] torsoScripts = torso.gameObject.GetComponents<SpriteSwap>();
            foreach (SpriteSwap torsoScript in torsoScripts)
            {
                if (torsoScript.SpriteSheetName[torsoScript.SpriteSheetName.Length - 1] == 'X')
                {
                    torsoScript.SpriteSheetName = torsoScript.SpriteSheetName.TrimEnd('X');
                    torsoScript.SpriteSheetName = torsoScript.SpriteSheetName.TrimEnd('A');
                    torsoScript.SpriteSheetName += "X";
                }
                torsoScript.SpriteSheetName = torsoScript.SpriteSheetName.TrimEnd('A');
            }
            foreach (SpriteSwap torsoScript in torsoScripts)
            {
                if (torsoScript.SpriteSheetName[torsoScript.SpriteSheetName.Length - 1] == 'B')
                {
                    torsoScript.SpriteSheetName = torsoScript.SpriteSheetName.TrimEnd('B');
                    torsoScript.SpriteSheetName = torsoScript.SpriteSheetName.TrimEnd('A');
                    torsoScript.SpriteSheetName += "B";
                }
                torsoScript.SpriteSheetName = torsoScript.SpriteSheetName.TrimEnd('A');
            }
            foreach (SpriteSwap torsoScript in torsoScripts)
            {
                if (torsoScript.SpriteSheetName[torsoScript.SpriteSheetName.Length - 1] == 'S')
                {
                    torsoScript.SpriteSheetName = torsoScript.SpriteSheetName.TrimEnd('S');
                    torsoScript.SpriteSheetName = torsoScript.SpriteSheetName.TrimEnd('A');
                    torsoScript.SpriteSheetName += "S";
                }
                torsoScript.SpriteSheetName = torsoScript.SpriteSheetName.TrimEnd('A');
            }
            foreach(SpriteSwap torsoScript in torsoScripts)
            {
                if (torsoScript.SpriteSheetName[torsoScript.SpriteSheetName.Length - 1] == 'W')
                {
                    torsoScript.SpriteSheetName = torsoScript.SpriteSheetName.TrimEnd('W');
                    torsoScript.SpriteSheetName = torsoScript.SpriteSheetName.TrimEnd('A');
                    torsoScript.SpriteSheetName += "W";
                }
                torsoScript.SpriteSheetName = torsoScript.SpriteSheetName.TrimEnd('A');
            }
            Transform leg = this.gameObject.transform.GetChild(5); // get leg child
            SpriteSwap[] legScripts = leg.gameObject.GetComponents<SpriteSwap>();
            foreach (SpriteSwap legScript in legScripts)
            {
                legScript.SpriteSheetName = legScript.SpriteSheetName.TrimEnd('A');
            }
        }
        if (item.name == "Axe")
        {
            Transform torso = this.gameObject.transform.GetChild(4); // get torso child
            SpriteSwap[] torsoScripts = torso.gameObject.GetComponents<SpriteSwap>();
            for (int i = 0; i < 4; i++)
            {
                if(torsoScripts[i + 4].SpriteSheetName[torsoScripts[i + 4].SpriteSheetName.Length - 1] == 'A')
                {
                    torsoScripts[i + 4].SpriteSheetName = torsoScripts[i + 4].SpriteSheetName.TrimEnd('A');
                    torsoScripts[i + 4].SpriteSheetName = torsoScripts[i + 4].SpriteSheetName.TrimEnd('X');
                    torsoScripts[i + 4].SpriteSheetName += "A";

                }
                torsoScripts[i + 4].SpriteSheetName = torsoScripts[i+4].SpriteSheetName.TrimEnd('X');
            }
        }
        if (item.name == "Spear(Clone)(Clone)")
        {
            Transform torso = this.gameObject.transform.GetChild(4); // get torso child
            SpriteSwap[] torsoScripts = torso.gameObject.GetComponents<SpriteSwap>();
            for (int i = 0; i < 4; i++)
            {
                if (torsoScripts[i + 4].SpriteSheetName[torsoScripts[i + 4].SpriteSheetName.Length - 1] == 'A')
                {
                    torsoScripts[i + 4].SpriteSheetName = torsoScripts[i + 4].SpriteSheetName.TrimEnd('A');
                    torsoScripts[i + 4].SpriteSheetName = torsoScripts[i + 4].SpriteSheetName.TrimEnd('S');
                    torsoScripts[i + 4].SpriteSheetName += "A";

                }
                torsoScripts[i + 4].SpriteSheetName = torsoScripts[i + 4].SpriteSheetName.TrimEnd('S');
            }
        }
        if (item.name == "Sword(Clone)(Clone)")
        {
            Transform torso = this.gameObject.transform.GetChild(4); // get torso child
            SpriteSwap[] torsoScripts = torso.gameObject.GetComponents<SpriteSwap>();
            for (int i = 0; i < 4; i++)
            {
                if (torsoScripts[i + 4].SpriteSheetName[torsoScripts[i + 4].SpriteSheetName.Length - 1] == 'A')
                {
                    torsoScripts[i + 4].SpriteSheetName = torsoScripts[i + 4].SpriteSheetName.TrimEnd('A');
                    torsoScripts[i + 4].SpriteSheetName = torsoScripts[i + 4].SpriteSheetName.TrimEnd('W');
                    torsoScripts[i + 4].SpriteSheetName += "A";

                }
                torsoScripts[i + 4].SpriteSheetName = torsoScripts[i + 4].SpriteSheetName.TrimEnd('W');
            }
        }
        if (item.name == "Bow")
        {
            Transform torso = this.gameObject.transform.GetChild(4); // get torso child
            SpriteSwap[] torsoScripts = torso.gameObject.GetComponents<SpriteSwap>();
            for (int i = 0; i < 4; i++)
            {
                if (torsoScripts[i + 4].SpriteSheetName[torsoScripts[i + 4].SpriteSheetName.Length - 1] == 'A')
                {
                    torsoScripts[i + 4].SpriteSheetName = torsoScripts[i + 4].SpriteSheetName.TrimEnd('A');
                    torsoScripts[i + 4].SpriteSheetName = torsoScripts[i + 4].SpriteSheetName.TrimEnd('B');
                    torsoScripts[i + 4].SpriteSheetName += "A";

                }
                torsoScripts[i + 4].SpriteSheetName = torsoScripts[i + 4].SpriteSheetName.TrimEnd('B');
            }
        }
    }

    public void ChangeDamage(float itemDamage)
    {
        damage += itemDamage;
    }

    public void RefactorSpeed(float itemSpeed)
    {
        attackSpeed *= itemSpeed;
    }

    public void ChangeCombatMode(int itemMode)
    {
        attackType = itemMode;
    }

    public void ChangeAura(int aura)
    {
        auraType = aura;
    }

    public void ChangeProjectile(GameObject itemProjectile)
    {
        projectile = itemProjectile;
    }
    #endregion

    private void DetermineFaceChange()
    {
        angle = Mathf.Rad2Deg * Mathf.Atan((mousePosition.y - transform.position.y) / (mousePosition.x - transform.position.x));
        if (mousePosition.x > transform.position.x)
        {
            if (angle <= 45 && angle > -45)
            {
                facing = faceRight;
                currDirection = Vector2.right;
            }
            else if (angle > 45)
            {
                facing = faceUp;
                currDirection = Vector2.up;
            }
            else
            {
                facing = faceDown;
                currDirection = Vector2.down;
            }
        }
        else
        {
            if (angle <= 45 && angle > -45)
            {
                facing = faceLeft;
                currDirection = Vector2.left;
            }
            else if (angle > 45)
            {
                facing = faceDown;
                currDirection = Vector2.down;
            }
            else
            {
                facing = faceUp;
                currDirection = Vector2.up;
            }
        }
    }

    private void HandleFaceChange()
    {
        angle *= Mathf.Deg2Rad;
        if (mousePosition.x < transform.position.x)
        {
            angle += Mathf.PI;
        }
        faceIndicator.transform.localPosition = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        faceIndicator.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Rad2Deg * angle));
        m_HeadAnimator.SetInteger("Facing", facing);
        m_TorsoAnimator.SetInteger("Facing", facing);
        m_BottomAnimator.SetInteger("Facing", facing);
    }

    #region combat_functions

    #region Dash
    IEnumerator Dash(Vector2 dir)
    {
        dash.dashCDTimer = 0;
        dir = dir.normalized;
        float dashDur = dash.dashDuration;
        if (dashDur > dash.dashCooldown) throw new System.Exception("dashCooldown is set too short");
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + (Vector3) dir * dash.dashDistance;
        Vector3 center = (startPos + endPos) * 0.5f;
        startPos = startPos - center;
        endPos = endPos - center;
        for (float timer = 0; timer < dashDur; timer += Time.deltaTime)
        {
            //Vector3.Slerp((Vector3)startPos, (Vector3)endPos, timer / dashDur) + center;
            Vector3 velocity = dir * Mathf.Sin(timer / dashDur * Mathf.PI);
            rb.MovePosition(velocity + transform.position);
            yield return null;
        }
        //transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
    #endregion

    private void Attack()
    {
        m_HUD.UpdateAttackSpeed(attackSpeed);
        m_HUD.ResetStamina();
        if (attackType == atkTypeMelee)
        {
            StartCoroutine(AttackMeleeRoutine());
        }
        else if (attackType == atkTypeRanged)
        {
            if (projectile != null)
            {
                AttackRanged();
            }
        }
        else if (attackType == atkTypeSpear)
        {
            StartCoroutine(AttackSpearRoutine());
        }
    }

    IEnumerator AttackMeleeRoutine()
    {
        hitboxes[0].damageValue = damage;
        indicators[2].transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Rad2Deg * angle));
        indicators[2].enabled = true;
        attackBoxes[0].enabled = true;
        attackTimer = attackSpeed;
        yield return new WaitForSeconds(0.1f);
        indicators[2].enabled = false;
        attackBoxes[0].enabled = false;
        hitboxes[0].damageApplied = false;
    }

    IEnumerator AttackSpearRoutine()
    {
        hitboxes[1].damageValue = damage;
        hitboxes[1].auraType = auraType;
        hitboxes[1].tickLength = 1f;
        hitboxes[1].tickDamage = 2;
        hitboxes[1].totalDuration = 5;
        indicators[3].transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Rad2Deg * angle));
        indicators[3].enabled = true;
        attackBoxes[1].enabled = true;
        attackTimer = attackSpeed;
        yield return new WaitForSeconds(0.1f);
        indicators[3].enabled = false;
        attackBoxes[1].enabled = false;
        hitboxes[1].damageApplied = false;
    }

    private void AttackRanged()
    {
        Instantiate(projectile, transform.position, transform.rotation);
        attackTimer = attackSpeed;
    }

    public void TakeDamage(float damage)
    {
        if (!mercyInvuln)
        {
            mercyInvulnTimer = mercyInvulnLength;
            mercyInvuln = true;
            health = Mathf.Round(health - damage);
            m_HUD.UpdateHealth(1.0f * health / maxhealth);
            m_HUD.UpdateText(1.0f * health, maxhealth);

            //m_TorsoAnimator.SetTrigger("TakeDamage");

            StartCoroutine(flashRedPlayer(spriteRenderers, 0.5f));

            //Debug.Log("Took " + damage + " damage! Health is now " + health);
            if (health <= 0)
            {
                dying = true;
                Die();
            }
        }
    }

    IEnumerator flashRedPlayer(SpriteRenderer[] sprites, float delay) 
    {
        Color headColor = sprites[4].color;
        Color torsoColor = sprites[5].color;
        Color bottomColor = sprites[6].color;

        sprites[13].color = new Color(255, 0, 0, 1f);
        sprites[14].color = new Color(255, 0, 0, 1f);
        sprites[15].color = new Color(255, 0, 0, 1f);

        yield return new WaitForSeconds(delay);

        sprites[13].color = headColor;
        sprites[14].color = torsoColor;
        sprites[15].color = bottomColor;

        yield return new WaitForSeconds(delay);
    }

    public void ReceiveHealing(float healingAmount)
    {
        if (health + healingAmount > maxhealth)
        {
            health = maxhealth;
        } else
        {
            health = Mathf.Round(health + healingAmount);
        }
        m_HUD.UpdateHealth(1.0f * health / maxhealth);
        m_HUD.UpdateText(1.0f * health, maxhealth);
    }

    private void Die()
    {
        Debug.Log("Ran out of health! Game over...");
        movementVector = Vector2.zero;
        Time.timeScale = 0;
        m_gameManager.backToMainMenu();
    }

    #endregion

    #region interact_functions
    void Interact()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(rb.position + currDirection, new Vector2(0.5f, 0.5f), 0f, Vector2.zero, 0);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.CompareTag("Chest"))
            {
                hit.transform.GetComponent<Chest>().Interact();
            } else if (hit.transform.CompareTag("Storage"))
            {
                hit.transform.GetComponent<Storage>().Interact();
            } else if (hit.transform.CompareTag("Shop"))
            {
                hit.transform.GetComponent<Shop>().Interact();
            }
        }
    }
    #endregion

    private void OnCollisionStay2D(Collision2D collision)
    {
        //if (collision.gameObject.CompareTag("Walls"))
        //{
        //    if (currentDash != null)
        //    {
        //        StopCoroutine(currentDash);
        //    }
        //}
    }

}
