using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static AnimationManager;

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
    [SerializeField] private Dictionary<int, CardItem> _spawnedCards = new Dictionary<int, CardItem>();

    [Space(14)]
    [Header("Card Prefab & Transform")]
    [SerializeField] private GridLayoutGroup _cardGrid;

    [Header("Tween Effects & Events")]
    public UnityEvent GameWon;
    [SerializeField] private TweenEffect PointerEnterEffect;
    [SerializeField] private TweenEffect PointerExitEffect;
    [SerializeField] private TweenEffect ClickEffect;
    [SerializeField] private TweenEffect SpawnInEffect;
    [SerializeField] private TweenEffect MatchEffect;
    [SerializeField] private TweenEffect FlipEffect;
    public UnityEvent GameStarting;
    #endregion
    #region Debugging Items & Variables
    [Space(14)]
    [Header("Debugging List")]
    [SerializeField] private int _cardsClicked;
    [SerializeField] private int _totalCards;
    [SerializeField] private int _rows;
    [SerializeField] private int _columns;
    [SerializeField] private int _cardsMatched;

    private AnimationManager _animationManager;
    private AudioManager _audioManager;
    private ScoreManager _scoreManager;
    [SerializeField] private List<CardItem> _cards;
    public List<CardItem> Cards => _cards;
    [SerializeField] private List<CardItem> _selectedCards;
    public List<CardItem> SelectedCards => _selectedCards;
    [SerializeField] private List<CardItem> _matchedCards;
    public List<CardItem> MatchedCards => _matchedCards;
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

    #endregion
    #region Instance
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }

            return _instance;
        }
    }
    #endregion
    void Start()
    {
        _audioManager = AudioManager.Instance;
        _animationManager = AnimationManager.Instance;
        _scoreManager = ScoreManager.Instance;
        StartCoroutine(Initialize());
    }

    #region Initialization

    public void Restart()
    {
        StopAllCoroutines();
        StartCoroutine(Initialize());
    }
    public IEnumerator Initialize()
    {
        yield return SetupGrid(SelectedGameMode, SelectedDifficulty);
        AdjustGridLayoutGroup();
        StartCoroutine(StartGame());
    }

    public void Reset()
    {
        foreach (Transform child in _cardGrid.transform)
        {
            Destroy(child.gameObject);
        }
        _cards.Clear();
        _selectedCards.Clear();
        _matchedCards.Clear();

        // Check if _spawnedCards is null before clearing it
        if (_spawnedCards != null)
        {
            _spawnedCards.Clear();
        }
        else
        {
            // Log a message if _spawnedCards is null
            Debug.LogWarning("_spawnedCards is null in GameManager.Reset()");
        }
    }

#nullable enable
    private IEnumerator SetupGrid(GameModes gameMode, GameDifficulties gameDifficulty, List<CardItem>? cards = null)
    {
        _cardGrid.enabled = true;
        GridSize gridSize = GetGridSize(gameDifficulty);
        _rows = gridSize.Rows;
        _columns = gridSize.Columns;

        int allowedColors = (_rows * _columns) / 2;
        // use AvailableColors to get and set the colors, and there should only be two matching colors

        _totalCards = _rows * _columns;

        Reset();

        // Spawn cards
        _spawnedCards = new Dictionary<int, CardItem>();
        int cardIndex = 0;
        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _columns; col++)
            {

                GameObject cardGameObject = Instantiate(_cardPrefab, _cardGrid.transform);
                RectTransform cardRectTransform = cardGameObject.GetComponent<RectTransform>();
                cardRectTransform.localScale = Vector3.zero; // Set local scale to zero
                CardItem card = cardGameObject.GetComponent<CardItem>();
                card.Id = cardIndex;
                card.IsMine = false;
                card.Hidden = false;
                card.HasMatched = false;
                card.CardObject = cardGameObject;
                card.IsClickable = false;
                card.CardImage = cardGameObject.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>();
                card.CardRectTransform = cardRectTransform;
                card.EventTrigger = cardGameObject.GetComponent<EventTrigger>();
                card.GridPosition = new Vector2(row, col);
                card.AnimationManager = _animationManager;
                card.AudioManager = _audioManager;
                card.ScoreManager = _scoreManager;
                card.CardSelectable = cardGameObject.GetComponent<Selectable>();
                _spawnedCards.Add(card.Id, card);
                _cards.Add(card);
                AddEventTriggers(card);

                cardIndex++;
            }
        }

        // If cards are provided for loading, update the grid accordingly
        if (cards != null && cards.Count == _totalCards)
        {
            for (int i = 0; i < _totalCards; i++)
            {
                _spawnedCards[i] = cards[i];
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
        yield return new WaitForSeconds(0.4f);
    }
    private IEnumerator StartGame()
    {

        yield return new WaitForSeconds(SpawnInEffect.Delay);

        foreach (CardItem card in _spawnedCards.Values)
        {
            yield return new WaitForSeconds(0.1f);

            // Get the animation curve based on the selected effect
            AnimationCurve curve = _animationManager.GetAnimationCurve(SpawnInEffect.SelectedEffect);
            if (curve != null)
            {

                _audioManager.Play(card.FlippingSounds[0], AudioManager.Sources.Effects);
                StartCoroutine(card.ScaleWithCurve(card.CardObject.transform, SpawnInEffect.SetScale, curve, SpawnInEffect.Delay));
            }
            else
            {
                Debug.LogError("Animation curve not defined for selected effect: " + SpawnInEffect.SelectedEffect);
            }
        }
        _cardGrid.enabled = false;

        StartCoroutine(HideAllCards());
    }
    private IEnumerator HideAllCards()
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < _spawnedCards.Values.Count; i++)
        {
            yield return new WaitForSeconds(0.15f);

            _spawnedCards[i].Flip(MatchEffect);
        }
        for (int i = 0; i < _spawnedCards.Values.Count; i++)
        {
            _spawnedCards[i].IsClickable = true;
        }
        _cardGrid.enabled = false;
        GameStarting.Invoke();
    }
    private void AdjustGridLayoutGroup()
    {
        // Calculate cell size based on grid dimensions
        float cellSizeX = (_cardGrid.GetComponent<RectTransform>().rect.width / _columns) - _cellPadding;
        float cellSizeY = (_cardGrid.GetComponent<RectTransform>().rect.height / _rows) - _cellPadding;
        // Use the smaller dimension as the cell size
        float cellSize = Mathf.Min(cellSizeX, cellSizeY);
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
    #region Triggers & Scaling Curve
    private void AddEventTriggers(CardItem card)
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
    private void OnPointerEnter(CardItem card)
    {
        if (!card.HasClicked && card.IsClickable)
        {
            AnimationCurve curve = _animationManager.GetAnimationCurve(SpawnInEffect.SelectedEffect);
            StartCoroutine(card.ScaleWithCurve(card.CardObject.transform, PointerEnterEffect.SetScale, curve, PointerEnterEffect.Delay));
            card.CardObject.transform.SetSiblingIndex(card.CardObject.transform.parent.childCount - 1);
            CheckCards(card.IsClickable);
        }
    }
    private void OnPointerExit(CardItem card)
    {
        if (!card.HasClicked && card.IsClickable)
        {
            AnimationCurve curve = _animationManager.GetAnimationCurve(SpawnInEffect.SelectedEffect);
            StartCoroutine(card.ScaleWithCurve(card.CardObject.transform, PointerExitEffect.SetScale, curve, PointerExitEffect.Delay));
            card.CardObject.transform.SetSiblingIndex(card.CardObject.transform.parent.childCount - 1);
            CheckCards(card.IsClickable);
        }
    }
    private void OnPointerClick(CardItem card)
    {
        if (!card.HasClicked && card.IsClickable)
        {
            _scoreManager.Flips++;
            _scoreManager.UpdateScoreInformation();
        }
        if (!card.HasMatched && !card.HasClicked && card.IsClickable)
        {
            AnimationCurve curve = _animationManager.GetAnimationCurve(SpawnInEffect.SelectedEffect);
            StartCoroutine(card.ScaleWithCurve(card.CardObject.transform, ClickEffect.SetScale, curve, ClickEffect.Delay));
            card.CardObject.transform.SetSiblingIndex(card.CardObject.transform.parent.childCount - 1);
            card.HasClicked = !card.HasClicked;
            card.Flip(FlipEffect);
            _selectedCards.Add(card);
        }
        CheckCards(card.IsClickable, true);
    }
    private void CheckCards(bool IsClickable, bool? hasClicked = false)
    {
        if (!IsClickable)
        {
            return;
        }
        if (hasClicked == true)
        {
            _cardsClicked++;
        }

        if (_cardsClicked == 2)
        {
            CardItem[] cards = _selectedCards.ToArray();
            if (cards.Length == 2) // Ensure there are exactly two cards selected
            {
                if (cards[0].CardImage.color == cards[1].CardImage.color)
                {
                    for (int i = 0; i < cards.Length; i++)
                    {
                        cards[i].DisableCard();
                        AnimationCurve curve = _animationManager.GetAnimationCurve(MatchEffect.SelectedEffect);
                        StartCoroutine(cards[i].ScaleWithCurve(cards[i].CardObject.transform, MatchEffect.SetScale, curve, MatchEffect.Delay));

                    }
                    _cardsMatched++;
                    _scoreManager.Matches++;
                    _scoreManager.ScoreMultiplier++;
                    _scoreManager.AddScore();
                    CheckWinningState();
                }
                else
                {
                    // Cards don't match, flip them back
                    for (int i = 0; i < cards.Length; i++)
                    {
                        cards[i].HasClicked = false; // Reset the clicked state
                        cards[i].Flip(FlipEffect);
                        AnimationCurve exitCurve = _animationManager.GetAnimationCurve(PointerExitEffect.SelectedEffect);
                        StartCoroutine(cards[i].ScaleWithCurve(cards[i].CardObject.transform, PointerExitEffect.SetScale, exitCurve, PointerExitEffect.Delay));
                    }
                    Debug.Log("Didn't match, flipping back");
                    _scoreManager.ScoreMultiplier = 1;
                }
            }
            else
            {
                // If there are not exactly two cards selected, flip them back
                for (int i = 0; i < cards.Length; i++)
                {
                    cards[i].HasClicked = false; // Reset the clicked state
                    cards[i].Flip(FlipEffect);
                    AnimationCurve exitCurve = _animationManager.GetAnimationCurve(PointerExitEffect.SelectedEffect);
                    StartCoroutine(cards[i].ScaleWithCurve(cards[i].CardObject.transform, PointerExitEffect.SetScale, exitCurve, PointerExitEffect.Delay));
                }
                Debug.Log("more than two cards selected, flipping back");
            }
            // Reset the number of clicked cards          
            _cardsClicked = 0;
            _selectedCards.Clear();
            _scoreManager.UpdateScoreInformation();
        }
    }

    public void CheckWinningState()
    {
        // Setup specific game mode
        switch (SelectedGameMode)
        {
            case GameModes.Normal:
                if (_scoreManager.Matches == (_totalCards / 2))
                {
                    _audioManager.Cheer();
                }
                break;
            case GameModes.Endless:
                _audioManager.Cheer();
                break;
            case GameModes.TimeBased:
                _audioManager.Cheer();
                break;
            case GameModes.MineSweeper:
                _audioManager.Cheer();
                break;
            default:
                break;
        }
    }

    #endregion
}
