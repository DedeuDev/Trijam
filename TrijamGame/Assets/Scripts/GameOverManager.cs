using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ScoreManager scoreManager;

    [Header("UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private TMP_Text bestScoreText;

    [Header("Settings")]
    [SerializeField] private float delayBeforeGameOver = 0.6f;
    [SerializeField] private bool pauseGameOnGameOver = true;

    [Header("Objects To Disable On Death")]
    [SerializeField] private Behaviour[] behavioursToDisable;

    private void Awake()
    {
        if (scoreManager == null)
        {
            scoreManager = FindAnyObjectByType<ScoreManager>();
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        Time.timeScale = 1f;
    }

    private void OnEnable()
    {
        PlayerDeath.OnPlayerDied += HandlePlayerDied;
    }

    private void OnDisable()
    {
        PlayerDeath.OnPlayerDied -= HandlePlayerDied;
    }

    private void HandlePlayerDied()
    {
        DisableGameplayBehaviours();
        StartCoroutine(ShowGameOverAfterDelay());
    }

    private IEnumerator ShowGameOverAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeGameOver);

        UpdateGameOverUI();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (pauseGameOnGameOver)
        {
            Time.timeScale = 0f;
        }
    }

    private void UpdateGameOverUI()
    {
        if (scoreManager == null) return;

        if (finalScoreText != null)
        {
            finalScoreText.text = "Score: " + scoreManager.CurrentScore.ToString();
        }

        if (bestScoreText != null)
        {
            bestScoreText.text = "Best: " + scoreManager.BestScore.ToString();
        }
    }

    private void DisableGameplayBehaviours()
    {
        for (int i = 0; i < behavioursToDisable.Length; i++)
        {
            if (behavioursToDisable[i] != null)
            {
                behavioursToDisable[i].enabled = false;
            }
        }
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;

        Application.Quit();
    }
}