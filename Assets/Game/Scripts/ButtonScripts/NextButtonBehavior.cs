using UnityEngine;
using UnityEngine.SceneManagement;

public class NextButtonBehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnNextButtonPressed()
    {
    Debug.Log("Next Button Pressed");
    SceneManager.LoadScene("End");
    }
}
