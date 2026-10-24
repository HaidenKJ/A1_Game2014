using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButtonBehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEndButtonPressed()
    {
    Debug.Log("End Button Pressed");
    SceneManager.LoadScene("Start");
    }
}
