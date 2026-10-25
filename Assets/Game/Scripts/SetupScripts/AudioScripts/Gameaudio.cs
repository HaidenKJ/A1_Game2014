using UnityEngine;
public class GameAudio : MonoBehaviour
{
    public static GameAudio Instance { get; private set; }
    [SerializeField] AudioClip music;
    [SerializeField, Range(0f, 1f)] float musicVolume = 0.4f; // baseline (minimum) music volume
    [SerializeField, Range(0f, 1f)] float maxMusicVolume = 0.9f; // volume when an enemy is right next to the player
    [SerializeField] float farDistance = 10f; // enemies this far away or more = baseline volume
    [SerializeField] float nearDistance = 2f;  // enemies this close or closer = max volume
    [SerializeField] float musicSmoothing = 3f; // higher = volume reacts faster
    [SerializeField, Range(0f, 1f)] float sfxVolume = 0.8f;
    [SerializeField] AudioClip[] playerHurtClips;
    [SerializeField] float enemyHurtInterval = 0.2f;   // beam damage happens every frame, so limiting  the sound rate
    [SerializeField] AudioClip enemyDeathClip;
    [SerializeField] AudioClip healClip;
    [SerializeField] AudioClip beamLoopClip;
    [SerializeField, Range(0f, 1f)] float beamVolume = 0.5f;
    [SerializeField] float beamFadeTime = 0.15f;
    [SerializeField] float beamHoldTime = 0.15f;

    AudioSource musicSource;
    AudioSource sfxSource;
    AudioSource beamSource;
    PlayerHealth player;
    float currentMusicVolume;
    float nearestEnemyDistance = float.MaxValue;
    float scanTimer;
    float lastBeamContact = -10f;
    bool beamActive;
    int lastPlayerHurtIndex = -1;

    void Awake()
    {
        Instance = this;

        musicSource = NewSource(true);
        musicSource.clip = music;
        currentMusicVolume = musicVolume;

        sfxSource = NewSource(false);

        beamSource = NewSource(true);
        beamSource.clip = beamLoopClip;
        beamSource.volume = 0f;
    }

    AudioSource NewSource(bool loop)
    {
        AudioSource s = gameObject.AddComponent<AudioSource>();
        s.loop = loop;
        s.playOnAwake = false;
        s.spatialBlend = 0f;
        return s;
    }

    void Start()
    {
        player = FindAnyObjectByType<PlayerHealth>();
        if (music != null) musicSource.Play();
    }

    void Update()
    {
        UpdateMusicVolume();
        UpdateBeamSound();
    }

    void OnDestroy()
    {
        if (Instance == this) 
        Instance = null;
    }

    void UpdateMusicVolume()
    {
        scanTimer -= Time.deltaTime; // Looking for the closest enemy 10 times a second instead of every frame
        if (scanTimer <= 0f)
        {
            scanTimer = 0.1f;
            nearestEnemyDistance = FindNearestEnemyDistance();
        }

        float proximity = Mathf.InverseLerp(farDistance, nearDistance, nearestEnemyDistance); // 0 = far away (baseline volume), 1 = right next to the player (max volume)
        float target = Mathf.Lerp(musicVolume, maxMusicVolume, proximity);

        currentMusicVolume = Mathf.Lerp(currentMusicVolume, target, musicSmoothing * Time.deltaTime); // Ease toward the target so the volume doesn't jump when an enemy dies
        musicSource.volume = currentMusicVolume;
    }

    float FindNearestEnemyDistance()
    {
        if (player == null) return float.MaxValue;

        float nearest = float.MaxValue;
        foreach (Enemy enemy in FindObjectsByType<Enemy>())
        {
            float d = Vector2.Distance(enemy.transform.position, player.transform.position);
            if (d < nearest) nearest = d;
        }
        return nearest;
    }

    void UpdateBeamSound()
    {
        if (beamLoopClip == null) return;
        bool inBeam = Time.time - lastBeamContact < beamHoldTime; // An enemy counts as being in the beam if a damage tier touched it very recently

        if (inBeam && !beamActive)
        {
            beamActive = true;
            if (!beamSource.isPlaying) beamSource.Play();
        }
        else if (!inBeam && beamActive)
        {
            beamActive = false;
        }

        float target = beamActive ? beamVolume : 0f;
        float step = beamVolume * Time.deltaTime / Mathf.Max(0.01f, beamFadeTime);
        beamSource.volume = Mathf.MoveTowards(beamSource.volume, target, step);

        if (!beamActive && beamSource.isPlaying && beamSource.volume <= 0.001f)
        {
            beamSource.Stop();
        }
    }

    void PlayRandom(AudioClip[] pool, ref int lastIndex)
    {
        if (pool == null || pool.Length == 0) return;

        int i = Random.Range(0, pool.Length);
        if (pool.Length > 1 && i == lastIndex)
        {
            i = (i + Random.Range(1, pool.Length)) % pool.Length; // never the same clip twice in a row
        }
        lastIndex = i;

        if (pool[i] != null) 
        {
            sfxSource.PlayOneShot(pool[i], sfxVolume);
        }
    }
    public static void PlayerHurt()
    {
        if (Instance != null) Instance.PlayRandom(Instance.playerHurtClips, ref Instance.lastPlayerHurtIndex);
    }

    public static void EnemyDeath()
    {
        if (Instance != null && Instance.enemyDeathClip != null)
            Instance.sfxSource.PlayOneShot(Instance.enemyDeathClip, Instance.sfxVolume);
    }

    public static void Heal()
    {
        if (Instance != null && Instance.healClip != null)
            Instance.sfxSource.PlayOneShot(Instance.healClip, Instance.sfxVolume);
    }
    public static void BeamContact()
    {
        if (Instance != null) Instance.lastBeamContact = Time.time;
    }
}