using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class MenuAudio : MonoBehaviour
{
    public static MenuAudio Instance { get; private set; }

    [SerializeField] AudioClip music;
    [SerializeField, Range(0f, 1f)] float musicVolume = 0.5f;
    [SerializeField] AudioClip hoverSfx;
    [SerializeField] AudioClip clickSfx;
    [SerializeField, Range(0f, 1f)] float sfxVolume = 0.8f;

    AudioSource musicSource;

    void Awake()
    {
        Instance = this;

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.clip = music;
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.spatialBlend = 0f;
    }

    void Start()
    {
        if (music != null) musicSource.Play();
        foreach (Button b in FindObjectsByType<Button>(FindObjectsInactive.Include)) // Give every button (even inactive ones) hover and click sounds
        {
            if (b.GetComponent<ButtonSfxHook>() == null)
                b.gameObject.AddComponent<ButtonSfxHook>();
        }
    }

    void Update()
    {
        musicSource.volume = musicVolume; // might remove later, for testing for now
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);
    }

    public void PlayHover() { PlayPersistent(hoverSfx); }
    public void PlayClick() { PlayPersistent(clickSfx); }
    void PlayPersistent(AudioClip clip) // Won't cut on transiton
    {
        if (clip == null) return;

        GameObject go = new GameObject("OneShotSfx");
        DontDestroyOnLoad(go); // might remove

        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = sfxVolume;
        source.spatialBlend = 0f;
        source.Play();

        Destroy(go, clip.length + 0.1f);
    }
}
public class ButtonSfxHook : MonoBehaviour, IPointerEnterHandler
{
    Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable && MenuAudio.Instance != null)
            MenuAudio.Instance.PlayHover();
    }

    void OnClick()
    {
        if (MenuAudio.Instance != null)
            MenuAudio.Instance.PlayClick();
    }
}