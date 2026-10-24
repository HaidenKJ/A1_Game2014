using UnityEngine;
using TMPro;
public class EndScreen : MonoBehaviour
{
    [SerializeField] TMP_Text killedByText;
    [SerializeField] TMP_Text scoreText;

    void Start()
    {
        if (killedByText != null) killedByText.text = "Killed by: " + GameResult.KilledBy;
        if (scoreText != null) scoreText.text = "Score: " + GameResult.FinalScore;
    }
}