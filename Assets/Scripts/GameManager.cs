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
    public Sprite[] AvailableIcons;
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
    public bool GameIsRunning;
    private AnimationManager _animationManager;
    private AudioManager _audioManager;
    private ScoreManager _scoreManager;
    private TimeTrackerManager _timeTrackerManager;
    private SaveManager _saveManager;
    public List<CardItem> Cards;
    public List<CardItem> SelectedCards;
    public List<CardItem> MatchedCards;
    public List<CardItemData> CardsData;
    public List<CardItemData> MatchedCardsData;
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
        _timeTrackerManager = TimeTrackerManager.Instance;
        _saveManager = SaveManager.Instance;
    }

    public void NewGame()
    {
        StartCoroutine(Initialize());
    }

    public void LoadGame()
    {
        StartCoroutine(FromSaveInitialize());
    }

    public void SetGameMode(int gameMode)
    {
        switch (gameMode)
        {
            case 0:
                SelectedGameMode = GameModes.Normal;
                break;
            case 1:
                SelectedGameMode = GameModes.Endless;
                break;
            case 2:
                SelectedGameMode = GameModes.TimeBased;
                break;
            case 3:
                SelectedGameMode = GameModes.MineSweeper;
                break;
        }
    }

    public void SetDifficulty(int gameDifficulty)
    {
        switch (gameDifficulty)
        {
            case 0:
                SelectedDifficulty = GameDifficulties.Easy;
                break;
            case 1:
                SelectedDifficulty = GameDifficulties.Normal;
                break;
            case 2:
                SelectedDifficulty = GameDifficulties.Medium;
                break;
            case 3:
                SelectedDifficulty = GameDifficulties.Hard;
                break;
            case 4:
                SelectedDifficulty = GameDifficulties.Extreme;
                break;
        }
    }

    #region Initialization

    public void Restart()
    {
        _timeTrackerManager.ResetTimer();
        StopAllCoroutines();
        StartCoroutine(Initialize());
    }
    public IEnumerator FromSaveInitialize()
    {
        yield return _saveManager.LoadGame();
        yield return SetupGrid(SelectedGameMode, SelectedDifficulty, true);
    }
    public IEnumerator Initialize()
    {
        yield return SetupGrid(SelectedGameMode, SelectedDifficulty, false);
    }

    public void ResetGame(bool fromSave)
    {
        GameIsRunning = false;
        foreach (Transform child in _cardGrid.transform)
        {
            Destroy(child.gameObject);
        }
        if (!fromSave)
        {
            Cards.Clear();
            SelectedCards.Clear();
            MatchedCards.Clear();

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
    }

#nullable enable
    private IEnumerator SetupGrid(GameModes gameMode, GameDifficulties gameDifficulty, bool fromSave)
    {
        _cardGrid.enabled = true;
        GridSize gridSize = GetGridSize(gameDifficulty);
        _rows = gridSize.Rows;
        _columns = gridSize.Columns;

        _totalCards = _rows * _columns;

        ResetGame(fromSave);

        // Spawn cards
        _spawnedCards = new Dictionary<int, CardItem>();
        int cardIndex = 0;
        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _columns; col++)
            {
                if (!fromSave)
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
                    card.CardImage.sprite = AvailableIcons[card.CardSpriteIndex];
                    card.GridPosition = new Vector2(row, col);
                    card.AnimationManager = _animationManager;
                    card.AudioManager = _audioManager;
                    card.ScoreManager = _scoreManager;
                    card.CardSelectable = cardGameObject.GetComponent<Selectable>();
                    _spawnedCards.Add(card.Id, card);
                    Cards.Add(card);
                    AddEventTriggers(card);
                    cardIndex++;
                }
                else
                {
                    foreach (CardItem card in Cards)
                    {
                        GameObject cardGameObject = Instantiate(_cardPrefab, _cardGrid.transform);
                        RectTransform cardRectTransform = cardGameObject.GetComponent<RectTransform>();
                        cardRectTransform.localScale = Vector3.one;
                        card.CardImage = cardGameObject.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>();
                        card.CardRectTransform = cardRectTransform;
                        card.EventTrigger = cardGameObject.GetComponent<EventTrigger>();
                        card.AnimationManager = _animationManager;
                        card.AudioManager = _audioManager;
                        card.ScoreManager = _scoreManager;
                        card.GridPosition = new Vector2(row, col);
                        card.CardSelectable = cardGameObject.GetComponent<Selectable>();
                        card.CardImage.sprite = AvailableIcons[card.CardSpriteIndex];
                        if (card.Hidden)
                        {
                            card.CardImage.gameObject.SetActive(false);
                        }
                        _spawnedCards.Add(card.Id, card);
                        AddEventTriggers(card);
                        cardIndex++;
                    }
                }

            }
        }

        yield return new WaitForSeconds(0.4f);
        AdjustGridLayoutGroup();
        StartCoroutine(StartGame(fromSave));
    }
    private IEnumerator StartGame(bool fromSave)
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

        if (!fromSave)
        {
            StartCoroutine(HideAllCards());
        }
    }
    private IEnumerator HideAllCards()
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < _spawnedCards.Values.Count; i++)
        {
            yield return new WaitForSeconds(0.08f);

            _spawnedCards[i].Flip(MatchEffect);
        }
        for (int i = 0; i < _spawnedCards.Values.Count; i++)
        {
            _spawnedCards[i].IsClickable = true;
        }
        _cardGrid.enabled = false;
        GameIsRunning = true;
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
            SelectedCards.Add(card);
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
            CardItem[] cards = SelectedCards.ToArray();
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
            SelectedCards.Clear();
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
                    GameIsRunning = false;
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
