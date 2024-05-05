using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Score Variables")]
    public int Score;
    public int ScoreMultiplier;
    public int Matches;
    public int Flips;
    public int BaseScore;
    [Header("Text Elements")]
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI ScoreMultiplierText;
    public TextMeshProUGUI MatchesText;
    public TextMeshProUGUI FlipsText;

    private static ScoreManager _instance;
    public static ScoreManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ScoreManager>();
            }

            return _instance;
        }
    }
    public void AddScore()
    {
        Score = Score + (BaseScore * ScoreMultiplier);
    }

    public void ResetScore()
    {
        Score = Matches = Flips = 0;
        ScoreMultiplier = 1;
    }
    public void UpdateScoreInformation()
    {
        ScoreText.text = Score.ToString();
        ScoreMultiplierText.text = ScoreMultiplier.ToString() + "x";
        MatchesText.text = Matches.ToString();
        FlipsText.text = Flips.ToString();
    }
}
