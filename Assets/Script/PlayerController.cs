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

    public GameObject aoeDamagePrefab;
    public GameObject singleTargetEffectPrefab;
    public GameObject dpsAuraEffectPrefab;

    private float aoeCooldown = 5f;
    private float singleCooldown = 2f;
    private float dpsCooldown = 20f;
    private float dpsDuration = 7f;

    private float aoeTimer = 0f;
    private float singleTimer = 0f;
    private float dpsTimer = 0f;

    private GameObject activeAuraEffect;
    public GameObject healEffectPrefab;

    public float aoeRadius = 5f;
    public float singlrRadius = 10f;
    public int aoeDamage = 5;

    private float playTime = 0f;
    public int leveltext = 1;

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

        aoeTimer += Time.deltaTime;
        singleTimer += Time.deltaTime;
        dpsTimer += Time.deltaTime;

        if (level >= 7 && aoeTimer >= aoeCooldown)
        {
            ActivateAOEDamage();
            aoeTimer = 0f;
        }

        if (level >= 5 && dpsTimer >= dpsCooldown)
        {
            StartCoroutine(ActivateDPSAura());
            dpsTimer = 0f;
        }

        if (level >= 3 && singleTimer >= singleCooldown)
        {
            ActivateSingleTargetDamage();
            singleTimer = 0f;
        }

        playTime += Time.deltaTime;
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
        Debug.Log("Player Dead!");
        PlayerPrefs.SetFloat("FinalTime", playTime);
        PlayerPrefs.SetInt("FinalLevel", level);
        SceneManager.LoadScene("EndGameScene");
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

    private void ActivateSingleTargetDamage()
    {
        if (level < 3) return;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, singlrRadius);

        if (enemies.Length > 0)
        {
            Collider2D selectedEnemy = null;

            foreach (var enemy in enemies)
            {
                if (enemy.CompareTag("Monster"))
                {
                    Vector2 directionToMonster = enemy.transform.position - transform.position;
                    float distanceToMonster = directionToMonster.magnitude;

                    if (distanceToMonster <= singlrRadius)
                    {
                        selectedEnemy = enemy;
                        break;
                    }
                }
            }

            if (selectedEnemy != null)
            {
                Monster monster = selectedEnemy.GetComponent<Monster>();
                if (monster != null)
                {
                    int randomDamage = Random.Range(20, 50);
                    monster.TakeDamage(randomDamage);
                    Instantiate(singleTargetEffectPrefab, selectedEnemy.transform.position, Quaternion.identity);
                }
            }
        }
    }

    private void ActivateAOEDamage()
    {
        if (level < 7) return;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, aoeRadius);

        foreach (Collider2D enemy in enemies)
        {
            if (enemy.CompareTag("Monster"))
            {
                Monster monster = enemy.GetComponent<Monster>();
                if (monster != null)
                {
                    int randomDamage = Random.Range(10, 30);
                    monster.TakeDamage(randomDamage);
                    Instantiate(aoeDamagePrefab, enemy.transform.position, Quaternion.identity);
                }
            }
        }
    }

    private IEnumerator ActivateDPSAura()
    {
        if (dpsAuraEffectPrefab != null)
        {
            activeAuraEffect = Instantiate(dpsAuraEffectPrefab, transform.position, Quaternion.identity, transform);
        }

        float elapsedTime = 0f;
        float damageInterval = 1f;
        float damageTimer = 0f;

        int baseDmg = 1 + ((level - 5) / 5) * 10;

        while (elapsedTime < dpsDuration)
        {
            damageTimer += Time.deltaTime;
            elapsedTime += Time.deltaTime;

            if (damageTimer >= damageInterval)
            {
                DamageNearbyMonsters(baseDmg);
                damageTimer = 0f;
            }

            yield return null;
        }

        if (activeAuraEffect != null)
        {
            Destroy(activeAuraEffect);
        }
    }

    private void DamageNearbyMonsters(int dmg)
    {
        float damageRadius = 3f;
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, damageRadius);

        foreach (Collider2D enemy in enemies)
        {
            if (enemy.CompareTag("Monster"))
            {
                Monster monster = enemy.GetComponent<Monster>();
                if (monster != null)
                {
                    monster.TakeDamage(dmg);
                    Debug.Log("Aura - Damaged monster for " + dmg + " at " + enemy.transform.position);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HealthItem"))
        {
            Heal(20);
            if (healEffectPrefab != null)
            {
                Instantiate(healEffectPrefab, transform.position, Quaternion.identity);
            }
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, singlrRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 3f);
    }
}
