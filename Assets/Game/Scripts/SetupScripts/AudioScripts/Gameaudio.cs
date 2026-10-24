using UnityEngine;
public class GameAudio : MonoBehaviour
{
    public static GameAudio Instance { get; private set; }
    [SerializeField] AudioClip music;
    [SerializeField, Range(0f, 1f)] float musicVolume = 0.4f;
    [SerializeField, Range(0f, 1f)] float sfxVolume = 0.8f;
    [SerializeField] AudioClip[] playerHurtClips;
    [SerializeField] float enemyHurtInterval = 0.2f;   // beam damage happens every frame, so limiting  the sound rate
    [SerializeField] AudioClip[] enemyDeathClips;
    [SerializeField] AudioClip[] healClips;
    [SerializeField] AudioClip beamLoopClip;
    [SerializeField, Range(0f, 1f)] float beamVolume = 0.5f;
    [SerializeField] float beamFadeTime = 0.15f;
    [SerializeField] float beamHoldTime = 0.15f;

    AudioSource musicSource;
    AudioSource sfxSource;
    AudioSource beamSource;
    float lastBeamContact = -10f;
    bool beamActive;
    int lastPlayerHurtIndex = -1;
    int lastDeathIndex = -1;
    int lastHealIndex = -1;

    void Awake()
    {
        Instance = this;

        musicSource = NewSource(true);
        musicSource.clip = music;

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
        if (music != null) musicSource.Play();
    }

    void Update()
    {
        musicSource.volume = musicVolume;
        UpdateBeamSound();
    }

    void OnDestroy()
    {
        if (Instance == this) 
        Instance = null;
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
            i = (i + Random.Range(1, pool.Length)) % pool.Length;   // never the same clip twice in a row
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
        if (Instance != null) Instance.PlayRandom(Instance.enemyDeathClips, ref Instance.lastDeathIndex);
    }

    public static void Heal()
    {
        if (Instance != null) Instance.PlayRandom(Instance.healClips, ref Instance.lastHealIndex);
    }
    public static void BeamContact()
    {
        if (Instance != null) Instance.lastBeamContact = Time.time;
    }
}