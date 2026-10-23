using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float maxHealth = 100f;
    [SerializeField] Slider healthBar;
    [SerializeField] string gameOverScene = "GameOver";

    public float Current { get; private set; }

    void Awake()
    {
        Current = maxHealth;
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
        if (healthBar == null) return;
        healthBar.minValue = 0f;
        healthBar.maxValue = maxHealth;
        healthBar.value = Current;
    }
}