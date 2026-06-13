using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Score Settings")]
    [SerializeField] private int pointsPerInterval = 10;
    [SerializeField] private float scoreInterval = 5f;

    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text bestScoreText;

    [Header("Best Score")]
    [SerializeField] private string bestScoreKey = "BestScore";

    private float survivalTime;
    private int currentScore;
    private int bestScore;

    private bool isCountingScore = true;

    public int CurrentScore => currentScore;
    public int BestScore => bestScore;
    public float SurvivalTime => survivalTime;

    private void OnEnable()
    {
        PlayerDeath.OnPlayerDied += HandlePlayerDied;
    }

    private void OnDisable()
    {
        PlayerDeath.OnPlayerDied -= HandlePlayerDied;
    }

    private void Start()
    {
        bestScore = PlayerPrefs.GetInt(bestScoreKey, 0);

        UpdateScoreUI();
        UpdateBestScoreUI();
    }

    private void Update()
    {
        if (!isCountingScore) return;

        survivalTime += Time.deltaTime;

        int completedIntervals = Mathf.FloorToInt(survivalTime / scoreInterval);
        currentScore = completedIntervals * pointsPerInterval;

        UpdateScoreUI();
    }

    private void HandlePlayerDied()
    {
        isCountingScore = false;

        if (currentScore > bestScore)
        {
            bestScore = currentScore;
            PlayerPrefs.SetInt(bestScoreKey, bestScore);
            PlayerPrefs.Save();
        }

        UpdateScoreUI();
        UpdateBestScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText == null) return;

        scoreText.text = "Score: " + currentScore.ToString();
    }

    private void UpdateBestScoreUI()
    {
        if (bestScoreText == null) return;

        bestScoreText.text = "Best: " + bestScore.ToString();
    }
}