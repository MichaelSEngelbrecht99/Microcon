using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Game Configuration Config/UI/Effects
    [Header("Game Configuration")]
    public GameModes SelectedGameMode;
    public GameDifficulties SelectedDifficulty;
    public Color[] AvailableColors;
    [Space(14)]

    [Header("Card Prefab & Transform")]
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private float _cellPadding;
    private Dictionary<string, Card> _spawnedCards;
    [Space(14)]
    [Header("Card Prefab & Transform")]
    [SerializeField] private GridLayoutGroup _cardGrid;

    [Header("LeanTween Effects & Events")]
    public UnityEvent CardSpawnIn;
    public UnityEvent CardSpawnOut;
    [SerializeField] private LeanTweenEffect PointerEnterEffect;
    [SerializeField] private LeanTweenEffect PointerExitEffect;
    [SerializeField] private LeanTweenEffect ClickEffect;
    [SerializeField] private LeanTweenEffect SpawnInEffect;
    #endregion
    #region Debugging Items & Variables
    [Space(14)]
    [Header("Debugging List")]
    [SerializeField] private int _totalCards;
    [SerializeField] private int _rows;
    [SerializeField] private int _columns;
    [SerializeField] private float _cellSizeRatio = 0.3f; // Adjust this value as needed

    [SerializeField] private List<Card> _Cards;
    public List<Card> Cards => _Cards;
    [SerializeField] private List<Card> _selectedCards;
    public List<Card> SelectedCards => _selectedCards;
    [SerializeField] private List<Card> _matchedCards;
    public List<Card> MatchedCards => _matchedCards;
    #endregion
    #region Game Enums
    public enum GameModes
    {
        None = 0,
        Normal = 1,
        Endless = 2,
        TimeBased = 3,
        MineSweeper = 4,
    }

    public enum GameDifficulties
    {
        None = 0,
        Easy = 1,
        Normal = 2,
        Medium = 3,
        Hard = 4,
        Extreme = 5,
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
        public Vector2 GridPosition;
        public bool IsMine;
        public bool Matched;
        public Image CardImage;
        public RectTransform CardRectTransform;
        public EventTrigger EventTrigger;
        public GameObject CardObject;
    }

    [Serializable]
    public struct LeanTweenEffect
    {
        public LeanTweenType SelectedEffect;
        public float Delay;
        public Vector3 SetScale;
        public AudioClip EffectClip;
    }
    #endregion
    void Start()
    {
        StartCoroutine(Initialize());
    }

    #region Initialization
    public IEnumerator Initialize()
    {
        yield return SetupGrid(SelectedGameMode, SelectedDifficulty);
        AdjustGridLayoutGroup();
        StartCoroutine(StartGame());
    }

    private IEnumerator SetupGrid(GameModes gameMode, GameDifficulties gameDifficulty, List<Card>? cards = null)
    {
        GridSize gridSize = GetGridSize(gameDifficulty);
        _rows = gridSize.Rows;
        _columns = gridSize.Columns;

        int allowedColors = (_rows * _columns) / 2;

        _totalCards = _rows * _columns;

        // Clear previous cards if any
        foreach (Transform child in _cardGrid.transform)
        {
            Destroy(child.gameObject);
        }

        // Spawn cards
        _spawnedCards = new Dictionary<string, Card>();
        int cardIndex = 0;
        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _columns; col++)
            {
                yield return new WaitForSeconds(0.03f);
                GameObject cardGameObject = Instantiate(_cardPrefab, _cardGrid.transform);
                RectTransform cardRectTransform = cardGameObject.GetComponent<RectTransform>();
                cardRectTransform.localScale = Vector3.zero; // Set local scale to zero
                Card card = new Card
                {
                    Id = cardIndex.ToString(),
                    IsMine = false,
                    Matched = false,
                    CardObject = cardGameObject,
                    CardImage = cardGameObject.transform.GetChild(0).GetChild(0).GetComponent<Image>(),
                    CardRectTransform = cardRectTransform,
                    EventTrigger = cardGameObject.GetComponent<EventTrigger>(),
                    GridPosition = new Vector2(row, col)
                };
                _spawnedCards.Add(card.Id, card);
                AddEventTriggers(card);

                cardIndex++;
            }
        }

        // If cards are provided for loading, update the grid accordingly
        if (cards != null && cards.Count == _totalCards)
        {
            for (int i = 0; i < _totalCards; i++)
            {
                _spawnedCards[i.ToString()] = cards[i];
            }
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

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(SpawnInEffect.Delay);

        foreach (Card card in _spawnedCards.Values)
        {
            card.CardRectTransform.localScale = Vector3.one;
            LeanTween.scale(card.CardObject, SpawnInEffect.SetScale, SpawnInEffect.Delay).setEase(SpawnInEffect.SelectedEffect);
        }
    }

    private void AdjustGridLayoutGroup()
    {
        // Calculate cell size based on grid dimensions
        float cellSizeX = (_cardGrid.GetComponent<RectTransform>().rect.width / _columns) + _cellPadding;
        float cellSizeY = (_cardGrid.GetComponent<RectTransform>().rect.height / _rows) + _cellPadding;
        Debug.Log(cellSizeX);
        Debug.Log(cellSizeY);
        // Use the smaller dimension as the cell size
        float cellSize = Mathf.Min(cellSizeX, cellSizeY);
        Debug.Log(cellSize);
        // Ensure cell size is valid
        if (cellSize <= 0f)
        {
            Debug.LogWarning("Invalid cell size. Adjusting to default value.");
            cellSize = 100f; // Default cell size value
        }

        _cardGrid.constraintCount = _columns;
        _cardGrid.cellSize = new Vector2(cellSize, cellSize);
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
    #endregion
    #region Triggers
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
    #endregion
}
