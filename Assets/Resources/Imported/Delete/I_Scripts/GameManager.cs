using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    private float elpsedTime = 0f;
    public float ScoreMultiplier = 10.0f;
    private float score = 0f;
    private float highscore = 0f;

    [SerializeField] private PanelRenderer panel;
    [SerializeField] private VisualElementReference<Label> ScorelabelRefrence;
    [SerializeField] private VisualElementReference<Label> HighScorelabelRefrence;
    [SerializeField] private VisualElementReference<Button> RestartButtonRefrence;
    [SerializeField] private VisualElementReference<Button> HSResetButtonRefrence;
    private Label ScoreText;
    private Label HighScoreText;
    private Button RestartButton;
    private Button HSResetButton;
    private bool isGameOver = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Update()
    {
        if (isGameOver) return;

        if (Input.GetMouseButtonDown(0))
        {
            elpsedTime += Time.deltaTime;
            score = Mathf.FloorToInt(elpsedTime * ScoreMultiplier); // "Mathf.FloorToInt" just rounds
            
            if(ScoreText != null)
            {
                ScoreText.text = "Score: " + score.ToString();
            }
        }
    }
    
    private void Awake()
    {
        panel.RegisterUIReloadCallback(OnUIReload);
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        panel.UnregisterUIReloadCallback(OnUIReload);
    }

    private void OnUIReload(PanelRenderer panel, VisualElement root, int version)
    {
        ScorelabelRefrence.RegisterReferenceResolvedCallback(HandleLabelRefrenceCallBack);
        HighScorelabelRefrence.RegisterReferenceResolvedCallback(HandleHighScoreLabelRefrenceCallBack);
        RestartButtonRefrence.RegisterReferenceResolvedCallback(HandleRestartButtonRefrenceCallBack);
        HSResetButtonRefrence.RegisterReferenceResolvedCallback(HandleHSResetButtonRefrenceCallBack);
    }

    private void HandleLabelRefrenceCallBack(Label label) // When using VisualElementReference, you need to make an idvidiual callback for each label/button or it won't work as intended.
    {
        ScoreText = label;
    }
    private void HandleHighScoreLabelRefrenceCallBack(Label Hlabel) 
    {
        HighScoreText = Hlabel;
        HighScoreText.text = "Highscore: " + PlayerPrefs.GetInt("Highscore", 0).ToString();   
    }
    private void HandleRestartButtonRefrenceCallBack(Button button)
    {
        RestartButton = button;
        if (RestartButton != null)
        {
            RestartButton.clicked += OnRestart;
            RestartButton.style.display = isGameOver ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
    private void HandleHSResetButtonRefrenceCallBack(Button HSbutton)
    {
        HSResetButton = HSbutton;
        if (HSResetButton != null)
        {
            HSResetButton.clicked += OnHSReset;
        }
    }
    private void OnHSReset()
    {
        PlayerPrefs.DeleteKey("Highscore");
        PlayerPrefs.Save();
        if (HighScoreText != null)
        {
            HighScoreText.text = "Highscore: 0";
        }
    }
    private void OnRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GameOver()
    {
        isGameOver = true;
        if (RestartButton != null)
        {
            RestartButton.style.display = DisplayStyle.Flex;
        }

        highscore = PlayerPrefs.GetInt("Highscore", 0);
        if (score > highscore)
        {
            PlayerPrefs.SetInt("Highscore", (int)score);
            PlayerPrefs.Save();
        }
    }
}
