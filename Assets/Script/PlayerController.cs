using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mono.Data.Sqlite;
using System.Data;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float dashSpeed = 10f;
    public float dashDuration = 0.2f;
    private float dashTime;
    private bool isDashing;
    private Rigidbody2D rb;
    private Vector2 movement;
    private SQLiteAdapter sqliteAdapter;

    public int maxHealth = 100;
    public int currentHealth;
    public int damage = 10;
    public int level = 1;
    public int exp = 0;
    public int expToNextLevel = 100;
    public int coins = 0;

    private SpriteRenderer spriteRenderer;
    public HealthBar healthBar;
    public Text coinText;
    public Text levelText;
    public Text expText;

    public GameObject shieldPrefab;
    public GameObject aoeDamagePrefab;
    public GameObject singleTargetEffectPrefab;

    private bool hasShield = false;

    private float shieldCooldown = 10f;
    private float shieldDuration = 7f;
    private float aoeCooldown = 5f;
    private float singleCooldown = 2f;
    private float shieldTimer = 0f;
    private float aoeTimer = 0f;
    private float singleTimer = 0f;

    public float aoeRadius = 5f;
    public float singlrRadius = 10f;
    public int aoeDamage = 5;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(currentHealth);
        }

        sqliteAdapter = FindObjectOfType<SQLiteAdapter>();
        if (sqliteAdapter != null)
        {
            sqliteAdapter.ConnectDatabase();
            coins = sqliteAdapter.GetCoinsFromDatabase();
            UpdateCoinUI();
        }
        else
        {
            Debug.LogError("SQLiteAdapter is not assigned in the scene.");
        }

        UpdateLevelUI();
    }

    void Update()
    {
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");
        FlipPlayer();

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
        {
            StartCoroutine(Dash());
        }

        shieldTimer += Time.deltaTime;
        aoeTimer += Time.deltaTime;
        singleTimer += Time.deltaTime;

        // ตรวจสอบ AOE
        if (level >= 7 && aoeTimer >= aoeCooldown)
        {
            ActivateAOEDamage(); // ใช้ AOE เมื่อเงื่อนไขครบ
            aoeTimer = 0f;
        }

        // ตรวจสอบ Shield
        if (level >= 5 && shieldTimer >= shieldCooldown)
        {
            StartCoroutine(ActivateShield());
            shieldTimer = 0f;
        }

        if (level >= 3 && singleTimer >= singleCooldown)
        {
            ActivateSingleTargetDamage();
            singleTimer = 0f;
        }
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            rb.MovePosition(rb.position + movement * dashSpeed * Time.fixedDeltaTime);
        }
        else
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        dashTime = dashDuration;
        yield return new WaitForSeconds(dashTime);
        isDashing = false;
    }

    public void TakeDamage(int damage)
    {
        if (hasShield)
        {
            return;
        }
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Debug.Log("Player Dead!");
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }
    }

    private void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GainEXP(int amount)
    {
        exp += amount;
        while (exp >= expToNextLevel)
        {
            LevelUp();
        }
        UpdateLevelUI();
    }

    void LevelUp()
    {
        level++;
        exp -= expToNextLevel;
        maxHealth += 20;
        currentHealth = maxHealth;
        damage += 5;

        UpdateLevelUI();
    }

    // AOE Damage Function (Level 7)
    // Single Target Damage Function (Level 3)
    private void ActivateSingleTargetDamage()
    {
        if (level < 3)  // ใช้เงื่อนไขที่ Level 3
        {
            Debug.Log("Level 3 not reached, skipping Single Target Damage.");
            return;
        }

        Debug.Log("Activating Single Target Damage Effect...");

        // ตรวจสอบมอนสเตอร์ในระยะที่กำหนด
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, singlrRadius);

        if (enemies.Length > 0)
        {
            // เลือกมอนสเตอร์ที่อยู่ในวง
            Collider2D selectedEnemy = null;

            foreach (var enemy in enemies)
            {
                if (enemy.CompareTag("Monster"))
                {
                    // เช็คว่า monster อยู่ในวงจริง ๆ
                    Vector2 directionToMonster = enemy.transform.position - transform.position;
                    float distanceToMonster = directionToMonster.magnitude;

                    if (distanceToMonster <= singlrRadius)
                    {
                        selectedEnemy = enemy;
                        break;  // เลือกมอนสเตอร์ที่อยู่ในระยะ
                    }
                }
            }

            if (selectedEnemy != null)
            {
                Monster monster = selectedEnemy.GetComponent<Monster>();
                if (monster != null)
                {
                    // สุ่มดาเมจ
                    int randomDamage = Random.Range(20, 50); // ปรับช่วงของดาเมจ
                    monster.TakeDamage(randomDamage);

                    // สร้าง Single Target Effect
                    Instantiate(singleTargetEffectPrefab, selectedEnemy.transform.position, Quaternion.identity);
                    Debug.Log("Single Target - Monster damaged: " + randomDamage + " at position: " + selectedEnemy.transform.position);
                }
            }
            else
            {
                Debug.Log("No valid monster found in range.");
            }
        }
        else
        {
            Debug.Log("No monsters detected in range.");
        }
    }


    // AOE Damage Function (Level 7)
    private void ActivateAOEDamage()
{
    if (level < 7)  // ใช้เงื่อนไขที่ Level 7
    {
        Debug.Log("Level 7 not reached, skipping AOE Damage.");
        return;
    }

    Debug.Log("Activating AOE Damage Effect...");

    // ตรวจสอบมอนสเตอร์ในระยะที่กำหนด
    Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, aoeRadius);

    if (enemies.Length > 0)
    {
        foreach (Collider2D enemy in enemies)
        {
            if (enemy.CompareTag("Monster"))
            {
                Monster monster = enemy.GetComponent<Monster>();
                if (monster != null)
                {
                    // สุ่มดาเมจ
                    int randomDamage = Random.Range(10, 30); // ปรับดาเมจให้เหมาะสมกับ AOE
                    monster.TakeDamage(randomDamage);

                    // สร้าง AOE Effect
                    Instantiate(aoeDamagePrefab, enemy.transform.position, Quaternion.identity);
                    Debug.Log("AOE - Monster damaged: " + randomDamage + " at position: " + enemy.transform.position);
                }
            }
        }
    }
    else
    {
        Debug.Log("No monsters detected in AOE range.");
    }
}



    private IEnumerator ActivateShield()
    {
        hasShield = true;
        GameObject shield = Instantiate(shieldPrefab, transform.position, Quaternion.identity, transform);
        yield return new WaitForSeconds(shieldDuration);
        hasShield = false;
        Destroy(shield);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HealthItem"))
        {
            Heal(20);
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("Coin"))
        {
            coins++;
            UpdateCoinUI();
            UpdateCoinDatabase();
            Destroy(collision.gameObject);
        }
    }

    private void FlipPlayer()
    {
        if (movement.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (movement.x < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = "Coins: " + coins.ToString();
        }
    }

    private void UpdateCoinDatabase()
    {
        if (sqliteAdapter != null)
        {
            sqliteAdapter.UpdateCoinsInDatabase(coins);
        }
    }

    private void UpdateLevelUI()
    {
        if (levelText != null)
        {
            levelText.text = "Level: " + level.ToString();
        }
        if (expText != null)
        {
            expText.text = "EXP: " + exp.ToString() + "/" + expToNextLevel.ToString();
        }
    }

    void OnDrawGizmosSelected()
    {
        // แสดง Gizmo สำหรับ AOE Radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, singlrRadius);
        
    }
}
