using UnityEngine;

/// <summary>
/// One script for Walker, Runner (and later Charger). Make a prefab per type and change the stats.
/// Needs: Collider2D + Rigidbody2D (Body Type: Kinematic) so the beam triggers detect it.
/// </summary>
public class Enemy : MonoBehaviour
{
    // General enemy damage. Each enemy scales it with its own multiplier.
    public static float BaseDamage = 5f;

    [Header("Stats")]
    [SerializeField] float maxHealth = 20f;
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float attackRange = 1f;
    [SerializeField] float attackInterval = 2f;     // seconds between attacks
    [SerializeField] float damageMultiplier = 1f;   // final damage = BaseDamage * this
    [SerializeField] int points = 1;

    float health;
    float attackTimer;
    bool dead;
    PlayerHealth player;

    void Start()
    {
        health = maxHealth;
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
        Debug.Log("Enemy taking damage" + health);
    }

    void Die()
    {
        dead = true;
        ScoreManager.Add(points);
        Destroy(gameObject);
    }
}