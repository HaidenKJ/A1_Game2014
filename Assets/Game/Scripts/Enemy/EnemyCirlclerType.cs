using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyCirlclerType : MonoBehaviour
{
    [SerializeField] float evadeSpeed = 5f;
    [SerializeField] float safeDistance = 1.5f;   // starts dodging when this close to the beam shape
    [SerializeField] float outwardBias = 0.3f;
    [SerializeField] float steerSmoothing = 8f;

    PlayerController player;
    Collider2D beam; // the biggest damage tier collider
    Collider2D myCollider;
    Vector2 evadeVelocity;
    int evadeSide = 1;   // +1 = counter-clockwise, -1 = clockwise

    void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        myCollider = GetComponent<Collider2D>();

        if (player != null)
        {
            float biggest = 0f;
            foreach (DamageTier tier in player.GetComponentsInChildren<DamageTier>())
            {
                Collider2D c = tier.GetComponent<Collider2D>();
                if (c == null) continue;

                float size = c.bounds.size.sqrMagnitude;
                if (size > biggest)
                {
                    biggest = size;
                    beam = c;
                }
            }
        }

        if (player == null || beam == null || myCollider == null)
            Debug.LogWarning(name + ": Circler can't find the player, a DamageTier collider, or its own Collider2D.");
    }

    void Update()
    {
        if (player == null || beam == null || myCollider == null) return;

        Vector2 toPlayer = (Vector2)player.transform.position - (Vector2)transform.position;
        float dist = toPlayer.magnitude;
        if (dist < 0.0001f) return;
        bool inDanger = beam.Distance(myCollider).distance < safeDistance; // Distance from the beam shape to this enemy's collider (negative = inside the beam)

        Vector2 desired = Vector2.zero;   // not evading = no extra movement
        if (inDanger)
        {
            Vector2 outward = -toPlayer / dist;                             
            float angle = Vector2.SignedAngle(player.AimDirection, outward);    
            if (Mathf.Abs(angle) > 1f) evadeSide = angle > 0f ? 1 : -1;

            Vector2 tangent = new Vector2(-outward.y, outward.x) * evadeSide;
            desired = (tangent + outward * outwardBias).normalized * evadeSpeed;
        }

        evadeVelocity = Vector2.Lerp(evadeVelocity, desired, steerSmoothing * Time.deltaTime);  // ease the dodge in and out
        transform.position += (Vector3)(evadeVelocity * Time.deltaTime);
    }
}