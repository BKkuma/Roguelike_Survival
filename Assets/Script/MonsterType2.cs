using UnityEngine;

public class MonsterType2 : MonoBehaviour, IDamageable
{
    public int health = 50;
    public GameObject coinPrefab;
    public GameObject[] itemPrefabs;
    public float itemDropChance = 0.3f;
    public float speed = 2f;
    public int damage = 20;
    public float damageInterval = 1f;
    private float damageTimer = 0f;
    private Transform target;

    private bool isCollidingWithPlayer = false;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (target != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            transform.position += (Vector3)(direction * speed * Time.deltaTime);
        }

        if (isCollidingWithPlayer)
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageInterval)
            {
                PlayerController playerController = target.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.TakeDamage(damage);
                    damageTimer = 0f;
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isCollidingWithPlayer = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isCollidingWithPlayer = false;
        }
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("MonsterType2 โดนตีแล้ว: -" + damage);
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.GainEXP(50);
        }

        Instantiate(coinPrefab, transform.position, Quaternion.identity);

        if (Random.value < itemDropChance)
        {
            int randomIndex = Random.Range(0, itemPrefabs.Length);
            Instantiate(itemPrefabs[randomIndex], transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
