using UnityEngine;

public class ProximityBeeper : MonoBehaviour
{
    public AudioClip beepClip;
    public float maxDistance = 6f;      // beyond this: slowest, lowest beep
    public float slowInterval = 1.0f;   // seconds between beeps when far
    public float fastInterval = 0.08f;  // seconds between beeps when very close
    public float lowPitch = 1f;
    public float highPitch = 2.5f;
    [Range(0f, 1f)] public float volume = 0.6f;

    private AudioSource src;
    private Obstacle[] enemies;
    private float nextBeep;

    void Awake()
    {
        src = gameObject.AddComponent<AudioSource>();
        src.playOnAwake = false;
    }

    void Start()
    {
        enemies = FindObjectsByType<Obstacle>();
    }

    void Update()
    {
        if (Time.time < nextBeep || beepClip == null) return;

        float nearest = float.MaxValue;
        foreach (Obstacle e in enemies)
        {
            if (e == null) continue;
            float d = Vector2.Distance(transform.position, e.transform.position);
            if (d < nearest) nearest = d;
        }
        if (nearest == float.MaxValue) return;

        float t = Mathf.Clamp01(nearest / maxDistance); // 0 = touching, 1 = far
        src.pitch = Mathf.Lerp(highPitch, lowPitch, t);
        src.PlayOneShot(beepClip, volume);
        nextBeep = Time.time + Mathf.Lerp(fastInterval, slowInterval, t);
    }
}