using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    public GameObject healEffectPrefab;

    public float aoeCooldown = 5f;
    public float singleCooldown = 2f;
    public float dpsCooldown = 20f;
    public float dpsDuration = 7f;

    public float aoeRadius = 5f;
    public float singlrRadius = 10f;

    private float aoeTimer = 0f;
    private float singleTimer = 0f;
    private float dpsTimer = 0f;

    private GameObject activeAuraEffect;
    private float playTime = 0f;

    // Skill unlock flags
    public bool aoeUnlocked = false;
    public bool dpsUnlocked = false;
    public bool singleUnlocked = false;

    private SkillUpgradeUI skillUpgradeUI;

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

        skillUpgradeUI = FindObjectOfType<SkillUpgradeUI>();

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

        if (aoeUnlocked && aoeTimer >= aoeCooldown)
        {
            ActivateAOEDamage();
            aoeTimer = 0f;
        }

        if (dpsUnlocked && dpsTimer >= dpsCooldown)
        {
            StartCoroutine(ActivateDPSAura());
            dpsTimer = 0f;
        }

        if (singleUnlocked && singleTimer >= singleCooldown)
        {
            ActivateSingleTargetDamage();
            singleTimer = 0f;
        }

        playTime += Time.deltaTime;
        aoeTimer += Time.deltaTime;

        if (aoeUnlocked && aoeTimer >= aoeCooldown)
        {
            Debug.Log("Activating AOE");
            ActivateAOEDamage();
            aoeTimer = 0f;
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
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBar?.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBar?.SetHealth(currentHealth);
    }

    private void Die()
    {
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
        // damage จะไม่เพิ่มโดยอัตโนมัติอีกต่อไป
        UpdateLevelUI();
    }

    private void ActivateSingleTargetDamage()
    {
        if (!singleUnlocked || skillUpgradeUI == null) return;

        int damage = skillUpgradeUI.GetSingleDamage();
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, singlrRadius);

        foreach (var enemy in enemies)
        {
            if (enemy.CompareTag("Monster"))
            {
                Monster monster = enemy.GetComponent<Monster>();
                if (monster != null)
                {
                    monster.TakeDamage(damage);
                    Instantiate(singleTargetEffectPrefab, enemy.transform.position, Quaternion.identity);
                    break;
                }
            }
        }
    }

    private void ActivateAOEDamage()
    {
        Debug.Log("AOE Activated!");

        if (!aoeUnlocked || skillUpgradeUI == null)
        {
            Debug.LogWarning("AOE not unlocked or skillUpgradeUI missing");
            return;
        }

        int damage = skillUpgradeUI.GetAOEDamage();
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, aoeRadius);

        foreach (var enemy in enemies)
        {
            if (enemy.CompareTag("Monster"))
            {
                Monster monster = enemy.GetComponent<Monster>();
                if (monster != null)
                {
                    monster.TakeDamage(damage);
                    Instantiate(aoeDamagePrefab, enemy.transform.position, Quaternion.identity);
                }
            }
        }
    }


    private IEnumerator ActivateDPSAura()
    {
        if (!dpsUnlocked || skillUpgradeUI == null) yield break;

        int damage = skillUpgradeUI.GetDPSDamage();

        if (dpsAuraEffectPrefab != null)
        {
            activeAuraEffect = Instantiate(dpsAuraEffectPrefab, transform.position, Quaternion.identity, transform);
        }

        float elapsedTime = 0f;
        float damageInterval = 1f;
        float damageTimer = 0f;

        while (elapsedTime < dpsDuration)
        {
            damageTimer += Time.deltaTime;
            elapsedTime += Time.deltaTime;

            if (damageTimer >= damageInterval)
            {
                DamageNearbyMonsters(damage);
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
            spriteRenderer.flipX = false;
        else if (movement.x < 0)
            spriteRenderer.flipX = true;
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

    public void ResetAOETimer()
    {
        aoeTimer = aoeCooldown; // ให้มันพร้อมใช้ทันทีใน frame ถัดไป
    }

    public void ResetDPSTimer()
    {
        dpsTimer = dpsCooldown;
    }

    public void ResetSingleTimer()
    {
        singleTimer = singleCooldown;
    }

}
