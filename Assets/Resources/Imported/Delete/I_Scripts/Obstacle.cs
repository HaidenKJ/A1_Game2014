using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float minSize = 0.5f;
    public float maxSize = 2.0f;

    public float minSpeed = 50f;
    public float maxSpeed = 150f;

    public float maxSpinSpeed = 10f;
    [SerializeField] public GameObject ExplosionEffect;
    Rigidbody2D rb;

    private static float lastObstacleSound = -1f; // shared by all obstacles

    void Start()
    {
        lastObstacleSound = -1f; // reset in case domain reload is disabled

        float randomSize = Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(randomSize, randomSize, 1);

        float randomSpeed = Random.Range(minSpeed, maxSpeed) / randomSize;
        Vector2 randomDirection = Random.insideUnitCircle;

        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(randomDirection * randomSpeed);

        float randomTorque = Random.Range(-maxSpinSpeed, maxSpinSpeed);
        rb.AddTorque(randomTorque);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject ExplosionInstance = Instantiate(ExplosionEffect, collision.GetContact(0).point, Quaternion.identity);
            Destroy(ExplosionInstance, 2f);
        }
        else if (collision.gameObject.GetComponent<Obstacle>() != null)
        {
            if (collision.relativeVelocity.magnitude < 0.05f) return; // ignore only tiny touches
            if (Time.time - lastObstacleSound < 0.05f) return;

            lastObstacleSound = Time.time;
            AudioManager.Instance.PlayObstacleHit();
        }
    }
}