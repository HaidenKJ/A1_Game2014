using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButtonBehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnStartButtonPressed()
    {
    Debug.Log("Start Button Pressed");
    SceneManager.LoadScene("PlayScene");
    }
}
