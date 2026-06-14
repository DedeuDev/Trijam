using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public GameObject CreditsText;
    public GameObject PlayButton;
    public GameObject CreditsButton;
    public GameObject BackButton;


public void PlayGame()
    {
        SceneManager.LoadScene("Game");
    }

  public void ShowText()
{
    CreditsText.SetActive(true);
    BackButton.SetActive(true);
    PlayButton.SetActive(false);
    CreditsButton.SetActive(false);

}

    public void HideText()
{
    CreditsText.SetActive(false);
    BackButton.SetActive(false);
    PlayButton.SetActive(true);
    CreditsButton.SetActive(true);
}
}



