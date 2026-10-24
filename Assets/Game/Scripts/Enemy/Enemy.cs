using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static float BaseDamage = 5f; // General enemy damage. Each enemy scales it with its own multiplier.
    [SerializeField] float maxHealth = 20f;
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float attackRange = 1f;
    [SerializeField] float attackInterval = 2f; // seconds between attacks
    [SerializeField] float firstAttackDelay = 0.1f; // wait after arriving before the first hit
    [SerializeField] float damageMultiplier = 1f;  // final damage = BaseDamage * this
    [SerializeField] int points = 1;

    float health;
    float attackTimer;
    bool hasArrived;

    public float Health => health;
    public float MaxHealth => maxHealth;
    bool dead;
    PlayerHealth player;

    void Awake()
    {
        health = maxHealth;
    }

    void Start()
    {
        player = FindFirstObjectByType<PlayerHealth>();
    }

    void Update()
    {
        if (dead || player == null) return;

        Vector2 toPlayer = (Vector2)player.transform.position - (Vector2)transform.position;

        if (toPlayer.magnitude > attackRange)
        {
            transform.position += (Vector3)(toPlayer.normalized * moveSpeed * Time.deltaTime);
        }
        else
        {
            if (!hasArrived) // skip most of the wait so there's no awkward pause on arrival, only once tho
            {
                hasArrived = true;
                attackTimer = Mathf.Max(0f, attackInterval - firstAttackDelay);
            }

            attackTimer += Time.deltaTime;
            if (attackTimer >= attackInterval)
            {
                attackTimer = 0f;
                player.TakeDamage(BaseDamage * damageMultiplier);
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (dead) return;
        health -= amount;
        if (health <= 0f) Die();
    }

    void Die()
    {
        dead = true;
        ScoreManager.Add(points);
        Destroy(gameObject);
    }
}