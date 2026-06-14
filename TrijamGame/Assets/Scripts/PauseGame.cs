using UnityEngine;

public class PauseGame : MonoBehaviour
{
    public GameObject ScoreText;
    public GameObject BaseText;
    public GameObject BackButton;
    public GameObject PauseButton;

    public void  OnPause()
    {
        Time.timeScale = 0f;
        PauseButton.SetActive(false);
        BackButton.SetActive(true);

        ScoreText.SetActive(false);
        BaseText.SetActive(false);
    }

    public void OnBack()
{
    Time.timeScale = 1f;
    PauseButton.SetActive(true);
    BackButton.SetActive(false);

    ScoreText.SetActive(true);
    BaseText.SetActive(true);
}
}


