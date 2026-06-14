using TMPro;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static int DifficultyLevel { get; private set; } = 1;
    public static float DifficultyMultiplier { get; private set; } = 1f;

    [Header("Difficulty Settings")]
    [SerializeField] private float increaseInterval = 5f;
    [SerializeField] private float multiplierIncreasePerLevel = 0.25f;

    [Header("UI Optional")]
    [SerializeField] private TMP_Text difficultyText;

    private float timer;
    private bool isGameOver;

    private void Awake()
    {
        DifficultyLevel = 1;
        DifficultyMultiplier = 1f;
        timer = 0f;
        isGameOver = false;

        UpdateUI();
    }

    private void OnEnable()
    {
        PlayerDeath.OnPlayerDied += HandlePlayerDied;
    }

    private void OnDisable()
    {
        PlayerDeath.OnPlayerDied -= HandlePlayerDied;
    }

    private void Update()
    {
        if (isGameOver) return;

        timer += Time.deltaTime;

        if (timer >= increaseInterval)
        {
            timer -= increaseInterval;
            IncreaseDifficulty();
        }
    }

    private void IncreaseDifficulty()
    {
        DifficultyLevel++;

        DifficultyMultiplier = 1f + ((DifficultyLevel - 1) * multiplierIncreasePerLevel);

        UpdateUI();
    }

    private void HandlePlayerDied()
    {
        isGameOver = true;
    }

    private void UpdateUI()
    {
        if (difficultyText == null) return;

        difficultyText.text = "Difficulty: " + DifficultyLevel.ToString();
    }

    public static float ApplySpeed(float baseValue)
    {
        return baseValue * DifficultyMultiplier;
    }

    public static float ApplyInterval(float baseValue)
    {
        return baseValue / DifficultyMultiplier;
    }
}