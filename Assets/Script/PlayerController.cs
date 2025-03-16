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

        Debug.Log("Healed: " + amount);
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
        expToNextLevel += 50;
        maxHealth += 20;
        currentHealth = maxHealth;
        damage += 5;
        Debug.Log("Level Up! New Level: " + level);
        UpdateLevelUI();
    }

    private void UpdateLevelUI()
    {
        if (levelText != null)
        {
            levelText.text = "Level: " + level;
        }
        if (expText != null)
        {
            expText.text = "EXP: " + exp + " / " + expToNextLevel;
        }
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
            Debug.Log("Coins: " + coins);
            Destroy(collision.gameObject);
            UpdateCoinUI();
            UpdateCoinDatabase();
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

    private void DisconnectDatabase()
    {
        if (sqliteAdapter != null)
        {
            sqliteAdapter.DisconnectDatabase();
        }
    }

    private void OnApplicationQuit()
    {
        DisconnectDatabase();
    }
}
