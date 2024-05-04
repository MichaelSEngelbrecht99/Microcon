using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    [Header("Game Configuration")]
    public GameModes SelectedGameMode;
    public GameDifficulties SelectedDifficulty;
    public Color[] AvailableColors;
    [Space(24)]

    [Header("Card Prefab & Transform")]
    [SerializeField] private GameObject _cardPrefab;
    private Dictionary<string, Card> _spawnedCards;
    [Space(24)]
    [Header("Card Prefab & Transform")]
    [SerializeField] private Grid _cardGrid;
    [Space(24)]
    [Header("Debugging List")]
    [SerializeField] private List<Card> _selectedCards;
    public List<Card> SelectedCards => _selectedCards;
    [SerializeField] private List<Card> _matchedCards;
    public List<Card> MatchedCards => _matchedCards;

    private int _totalCards;
    private int _rows;
    private int _columns;

    public enum GameModes
    {
        Normal,
        Endless,
        TimeBased,
        MineSweeper
    }

    public enum GameDifficulties
    {
        Easy,
        Normal,
        Medium,
        Hard,
        Extreme
    }

    [Serializable]
    public struct GridSize
    {
        public int Rows;
        public int Columns;
    }

    [Serializable]
    public struct Card
    {
        public string Id;
        public bool IsMine;
        public bool Matched;
        public EventTrigger EventTrigger;
        public GameObject GameObject;
    }

    public void Initialize()
    {
        SetupGrid(SelectedGameMode, SelectedDifficulty);
    }

    public void SetupGrid(GameModes gameMode, GameDifficulties gameDifficulty)
    {
        GridSize gridSize = GetGridSize(gameDifficulty);
        _rows = gridSize.Rows;
        _columns = gridSize.Columns;

        _totalCards = _rows * _columns;

        // Spawn cards
        _spawnedCards = new Dictionary<string, Card>();
        for (int i = 0; i < _totalCards; i++)
        {
            GameObject cardGO = Instantiate(_cardPrefab, _cardGrid.transform);
            Card card = new Card
            {
                Id = i.ToString(),
                IsMine = false,
                Matched = false,
                GameObject = cardGO,
                EventTrigger = cardGO.GetComponent<EventTrigger>()
            };
            _spawnedCards.Add(card.Id, card);
            AddEventTriggers(card);
        }

        // Setup specific game mode
        switch (gameMode)
        {
            case GameModes.Normal:
                // Implement setup for normal mode
                break;
            case GameModes.Endless:
                // Implement setup for endless mode
                break;
            case GameModes.TimeBased:
                // Implement setup for time-based mode
                break;
            case GameModes.MineSweeper:
                // Implement setup for minesweeper mode
                break;
            default:
                break;
        }
    }

    private GridSize GetGridSize(GameDifficulties difficulty)
    {
        switch (difficulty)
        {
            case GameDifficulties.Easy:
                return new GridSize { Rows = 2, Columns = 2 };
            case GameDifficulties.Normal:
                return new GridSize { Rows = 2, Columns = 3 };
            case GameDifficulties.Medium:
                return new GridSize { Rows = 4, Columns = 4 };
            case GameDifficulties.Hard:
                return new GridSize { Rows = 5, Columns = 6 };
            case GameDifficulties.Extreme:
                return new GridSize { Rows = 8, Columns = 8 };
            default:
                return new GridSize { Rows = 2, Columns = 2 };
        }
    }

    private void AddEventTriggers(Card card)
    {
        EventTrigger trigger = card.EventTrigger;
        if (trigger == null)
            return;

        trigger.triggers.Clear();

        // Pointer Enter
        EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((eventData) => { OnPointerEnter(card); });
        trigger.triggers.Add(pointerEnter);

        // Pointer Exit
        EventTrigger.Entry pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((eventData) => { OnPointerExit(card); });
        trigger.triggers.Add(pointerExit);

        // Pointer Click
        EventTrigger.Entry pointerClick = new EventTrigger.Entry();
        pointerClick.eventID = EventTriggerType.PointerClick;
        pointerClick.callback.AddListener((eventData) => { OnPointerClick(card); });
        trigger.triggers.Add(pointerClick);
    }

    private void OnPointerEnter(Card card)
    {
        // Implement pointer enter logic
    }

    private void OnPointerExit(Card card)
    {
        // Implement pointer exit logic
    }

    private void OnPointerClick(Card card)
    {
        // Implement pointer click logic
    }
}
