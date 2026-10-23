using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Clips")]
    public AudioClip backgroundMusic;
    public List<AudioClip> wallHits = new List<AudioClip>(); // pool of wall hit sounds
    public AudioClip obstacleHit;
    public AudioClip deathSound;

    [Header("Volume")]
    [Range(0f, 1f)] public float musicVolume = 0.25f;
    [Range(0f, 2f)] public float sfxVolume = 1f;
    [Range(0f, 2f)] public float wallHitVolume = 1.5f;
    [Range(0f, 2f)] public float obstacleHitVolume = 0.5f;   // quieter

    [Header("Pitch")]
    [Range(0.5f, 3f)] public float obstacleHitPitch = 1.4f;  // faster playback (also higher pitch)

    private AudioSource musicSource;
    private AudioSource sfxSource;
    private AudioSource obstacleSource;
    private int lastWallIndex = -1;

    void Awake()
    {
        Instance = this;

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.playOnAwake = false;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        // Dedicated source so changing pitch doesn't affect other sounds
        obstacleSource = gameObject.AddComponent<AudioSource>();
        obstacleSource.playOnAwake = false;
        obstacleSource.pitch = obstacleHitPitch;
    }

    void Start() { musicSource.Play(); }

    void Update() { musicSource.volume = musicVolume; } // live tweaking

    // Random clip from the pool, never the same one twice in a row
    public void PlayWallHit()
    {
        if (wallHits == null || wallHits.Count == 0) return;

        int index = Random.Range(0, wallHits.Count);
        if (wallHits.Count > 1 && index == lastWallIndex)
        {
            index = (index + 1 + Random.Range(0, wallHits.Count - 1)) % wallHits.Count;
        }
        lastWallIndex = index;

        sfxSource.PlayOneShot(wallHits[index], wallHitVolume);
    }

    public void PlayObstacleHit()
    {
        if (obstacleHit == null) return;
        obstacleSource.pitch = obstacleHitPitch; // live tweaking
        obstacleSource.PlayOneShot(obstacleHit, obstacleHitVolume);
    }

    public void StopMusic() { musicSource.Stop(); }

    // PlayClipAtPoint keeps playing after the player is destroyed
    public void PlayDeath(Vector3 pos)
    {
        if (deathSound != null) AudioSource.PlayClipAtPoint(deathSound, pos, sfxVolume);
    }
}