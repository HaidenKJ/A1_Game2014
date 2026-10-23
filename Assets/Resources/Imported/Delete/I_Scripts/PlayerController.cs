using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    public float thrustForce = 1f;
    public float maxSpeed = 5f;

    public GameObject boosterFlame;
    public GameObject trailPrefab; //
    private GameObject currentTrail; // Reference to the current trail instance
    public Transform trailPoint; // Point where the trail should be instantiated

    Rigidbody2D rb;
    private float lastWallSound = -1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePos - transform.position).normalized;
            transform.up = direction;

            rb.AddForce(direction * thrustForce);
        }

        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        if (Input.GetMouseButtonDown(0))
        {
            boosterFlame.SetActive(true);
            currentTrail = Instantiate(trailPrefab, trailPoint.transform.position, Quaternion.identity, transform);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            boosterFlame.SetActive(false);
            DetachTrail();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            if (Time.time - lastWallSound > 0.2f)
            {
                lastWallSound = Time.time;
                AudioManager.Instance.PlayWallHit();
            }
        }
        else if (collision.gameObject.GetComponent<Obstacle>() != null)
        {
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlayDeath(transform.position);

            if (gameManager != null) gameManager.GameOver();
            Destroy(gameObject);
        }
    }

    void DetachTrail() 
    {
        if (currentTrail == null) return;

        currentTrail.transform.SetParent(null); // stop following the ship
        var tr = currentTrail.GetComponent<TrailRenderer>();
        tr.emitting = false; // no new points
        Destroy(currentTrail, tr.time); // will remove once fully faded
        currentTrail = null;
    }
}

