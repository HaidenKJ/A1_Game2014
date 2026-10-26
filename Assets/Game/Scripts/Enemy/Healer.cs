using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class Healer : MonoBehaviour // This fella is my gimmick version of a pickup
{
    [SerializeField] float healPercent = 0.25f; // 25% of the player's max health (Was considering having a static number, but this just feels less rigid)
    float touchDistance = 1f;

    Enemy enemy;
    PlayerHealth player;

    void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    void Start()
    {
        player = FindAnyObjectByType<PlayerHealth>(); 
    }

    void Update()
    {
        if (player == null) return;
        if (enemy.Health <= 0f) return; // Already killed by the beam this frame, so no heal 

        float dist = Vector2.Distance(transform.position, player.transform.position);
        if (dist <= touchDistance)
        {
            player.Heal(player.maxHealth * healPercent);
            Destroy(gameObject);
        }
    }
}

