using UnityEngine;
using TMPro;
public class ScoreManager : MonoBehaviour
{
    public static int Score { get; private set; }

    [SerializeField] TMP_Text scoreText;
    [SerializeField] float secondsPerPoint = 2f;

    static ScoreManager instance;
    float timer;

    void Awake()
    {
        instance = this;
        Score = 0;
        Refresh();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= secondsPerPoint)
        {
            timer -= secondsPerPoint;
            Add(1);
        }
    }

    public static void Add(int amount)
    {
        Score += amount;
        if (instance != null) instance.Refresh();
    }

    void Refresh()
    {
        if (scoreText != null) scoreText.text = "Score: " + Score;
    }
}