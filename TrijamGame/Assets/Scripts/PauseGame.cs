 using UnityEngine;
 using UnityEngine.InputSystem;

public class PauseGame : MonoBehaviour
{
    public GameObject ScoreText;
    public GameObject BaseText;
    public GameObject BackButton;
    public GameObject PauseButton;

    bool isPaused = false;
    

    void Awake()
    {
        
    }

    void Update()
    {
         if (Input.GetKeyDown(KeyCode.Pause))
        {
            if (isPaused)
                OnPause();
            else
                OnBack();
        }
    }

    public void  OnPause()
    {
        Time.timeScale = 0f;
        PauseButton.SetActive(false);
        BackButton.SetActive(true);

        ScoreText.SetActive(false);
        BaseText.SetActive(false);

        isPaused = true;
    }

    public void OnBack()
{
    Time.timeScale = 1f;
    PauseButton.SetActive(true);
    BackButton.SetActive(false);

    ScoreText.SetActive(true);
    BaseText.SetActive(true);

    isPaused = false;
}



}


