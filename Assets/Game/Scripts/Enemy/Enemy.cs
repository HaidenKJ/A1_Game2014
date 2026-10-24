using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static float BaseDamage = 5f; // General enemy damage. Each enemy scales it with its own multiplier.
    [SerializeField] float maxHealth = 20f;
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float attackRange = 1f;
    [SerializeField] float attackInterval = 2f; // seconds between attacks
    [SerializeField] float firstAttackDelay = 0.3f; // wait after arriving before the first hit
    [SerializeField] float damageMultiplier = 1f;  // final damage = BaseDamage * this
    [SerializeField] int points = 1;
    [SerializeField] string displayName; // For displaying killer on end screen
    [SerializeField] ParticleSystem deathParticles; // prefab spawned when the beam kills this enemy

    float health;
    float attackTimer;
    bool hasArrived;

    public float Health => health;
    public float MaxHealth => maxHealth;
    public string DisplayName => string.IsNullOrEmpty(displayName) ? gameObject.name.Replace("(Clone)", "").Trim() : displayName;

    bool dead;
    PlayerHealth player;

    void Awake()
    {
        health = maxHealth;
    }

    void Start()
    {
        player = FindAnyObjectByType<PlayerHealth>();
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
            if (!hasArrived)
            {
                hasArrived = true;
                attackTimer = Mathf.Max(0f, attackInterval - firstAttackDelay);
            }

            attackTimer += Time.deltaTime;
            if (attackTimer >= attackInterval)
            {
                attackTimer = 0f;
                player.TakeDamage(BaseDamage * damageMultiplier, DisplayName);
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
        GameAudio.EnemyDeath();

        if (deathParticles != null)
            Instantiate(deathParticles, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}