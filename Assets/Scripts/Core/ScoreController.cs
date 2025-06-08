using TMPro;
using UnityEngine;
using System;

public class ScoreController : MonoBehaviour
{
    public static ScoreController Instance { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;

    private int score;
    private int combo;

    public static event Action<int> OnComboChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        ResetScore();
    }

    public void AddScore(string grade)
    {
        int baseScore = 0;
        bool resetCombo = false;

        switch (grade)
        {
            case "Perfect":
                baseScore = 100;
                combo++;
                break;
            case "Good":
                baseScore = 50;
                combo++;
                break;
            case "Bad":
                baseScore = 10;
                resetCombo = true;
                break;
            case "Miss":
                resetCombo = true;
                break;
        }

        if (resetCombo)
        {
            combo = 0;
        }

        float comboMultiplier = GetComboMultiplier(combo);
        int totalScore = Mathf.RoundToInt(baseScore * (1f + comboMultiplier));
        score += totalScore;

        OnComboChanged?.Invoke(combo);
        UpdateUI();
    }

    private float GetComboMultiplier(int combo)
    {
        if (combo >= 50)
            return 1.0f;
        else if (combo >= 20)
            return 0.7f;
        else if (combo >= 10)
            return 0.5f;
        else if (combo >= 3)
            return 0.2f;
        else
            return 0f;
    }

    public void ResetScore()
    {
        score = 0;
        combo = 0;
        OnComboChanged?.Invoke(combo);
        UpdateUI();
    }

    private void UpdateUI()
    {
        scoreText.text = $"Score: {score}";
        comboText.text = combo > 1 ? $"X{combo}" : "";
    }
}
