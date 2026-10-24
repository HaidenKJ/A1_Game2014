using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] public float maxHealth = 100f;
    [SerializeField] TMP_Text healthText;
    [SerializeField] Transform barSquare;
    [SerializeField] string gameOverScene = "GameOver";
    [SerializeField] float gameOverDelay = 0.8f;  // lets the hurt sound and flash play before the scene changes

    public float Current { get; private set; }
    public float MaxHealth => maxHealth;

    Vector3 barStartScale;
    bool dying;

    void Awake()
    {
        Current = maxHealth;
        if (barSquare != null) barStartScale = barSquare.localScale;
        UpdateBar();
    }

    public void TakeDamage(float amount, string source = "Unknown")
    {
        if (dying || Current <= 0f || amount <= 0f) return;

        Current = Mathf.Max(0f, Current - amount);
        UpdateBar();
        GameAudio.PlayerHurt();

        if (Current <= 0f)
        {
            dying = true;
            GameResult.KilledBy = source;
            GameResult.FinalScore = ScoreManager.Score;
            StartCoroutine(LoadGameOver());
        }
    }

    public void Heal(float amount)
    {
        if (dying) return;
        Current = Mathf.Min(maxHealth, Current + amount);
        UpdateBar();
    }

    IEnumerator LoadGameOver()
    {
        yield return new WaitForSeconds(gameOverDelay);
        SceneManager.LoadScene(gameOverScene);
    }

    void UpdateBar()
    {
        float fraction = Mathf.Clamp01(Current / maxHealth);

        if (barSquare != null)
        {
            Vector3 s = barStartScale;
            s.x = barStartScale.x * fraction; // only X changes, Y and Z stay the same
            barSquare.localScale = s;
        }

        if (healthText != null)
            healthText.text = Mathf.CeilToInt(Current).ToString();
    }
}