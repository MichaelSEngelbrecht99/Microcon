using System;
using TMPro;
using UnityEngine;

public class TimeTrackerManager : MonoBehaviour
{
    public TextMeshProUGUI TimeText;
    private int _time; // Time in seconds

    private void Start()
    {
        InvokeRepeating(nameof(IncreaseTime), 1, 1);
    }

    private void IncreaseTime()
    {
        _time++;
        UpdateTimeText();
    }

    private void UpdateTimeText()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(_time);
        string formattedTime = $"{timeSpan.Days:00}:{timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
        TimeText.text = formattedTime;
    }

    private void ResetTimer()
    {
        _time = 0;
        UpdateTimeText();
    }
}
