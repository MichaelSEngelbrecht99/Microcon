using System;
using TMPro;
using UnityEngine;

public class TimeTrackerManager : MonoBehaviour
{
    public TextMeshProUGUI TimeText;
    private int _time; // Time in seconds
    private GameManager _gameManager;

    private static TimeTrackerManager _instance;
    public static TimeTrackerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<TimeTrackerManager>();
            }

            return _instance;
        }
    }
    private void Start()
    {
        _gameManager = GameManager.Instance;
        InvokeRepeating(nameof(IncreaseTime), 1, 1);
    }

    private void IncreaseTime()
    {
        if (_gameManager.GameIsRunning)
        {
            _time++;
            UpdateTimeText();
        }
    }

    private void UpdateTimeText()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(_time);
        string formattedTime = $"{timeSpan.Days:00}:{timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
        TimeText.text = formattedTime;
    }

    public void ResetTimer()
    {
        _time = 0;
        UpdateTimeText();
    }
}
