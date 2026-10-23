using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float maxHealth = 100f;

    [Header("UI")]
    [SerializeField] TMP_Text healthText; // show te number
    [SerializeField] Transform barSquare; // the square that shrinks on X only

    [SerializeField] string gameOverScene = "End";

    public float Current { get; private set; }

    Vector3 barStartScale;

    void Awake()
    {
        Current = maxHealth;
        if (barSquare != null) barStartScale = barSquare.localScale;
        UpdateBar();
    }

    public void TakeDamage(float amount)
    {
        if (Current <= 0f) return;

        Current = Mathf.Max(0f, Current - amount);
        UpdateBar();

        if (Current <= 0f)
            SceneManager.LoadScene(gameOverScene);
    }

    public void Heal(float amount)
    {
        Current = Mathf.Min(maxHealth, Current + amount);
        UpdateBar();
    }

    void UpdateBar()
    {
        float fraction = Mathf.Clamp01(Current / maxHealth);

        if (barSquare != null)
        {
            Vector3 s = barStartScale;
            s.x = barStartScale.x * fraction; 
            barSquare.localScale = s;
        }

        if (healthText != null)
            healthText.text = Mathf.CeilToInt(Current).ToString();
    }
}