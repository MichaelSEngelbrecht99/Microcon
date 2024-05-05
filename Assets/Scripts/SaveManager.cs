using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System;

public class SaveManager : MonoBehaviour
{
        private bool isLoading = false;
    private GameManager _gameManager;
    private static SaveManager _instance;
    public static SaveManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SaveManager>();
            }
            return _instance;
        }
    }

    private string savePath;

    private void Awake()
    {
        savePath = Application.persistentDataPath + "/savegame.dat";
        _gameManager = GameManager.Instance;
    }



    public IEnumerator LoadGame()
    {
        if (isLoading) yield break; // Exit if loading is already in progress

        isLoading = true; // Set loading flag to true

        if (File.Exists(savePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(savePath, FileMode.Open);

            SaveData saveData = formatter.Deserialize(fileStream) as SaveData;
            fileStream.Close();

            Debug.Log("Game loaded successfully!");

            // Restore game state
            _gameManager.SelectedGameMode = saveData.SelectedGameMode;
            _gameManager.SelectedDifficulty = saveData.SelectedDifficulty;
            _gameManager.GameIsRunning = saveData.GameIsRunning;

            // Set cards and matched cards data
            _gameManager.CardsData = saveData.Cards;
            _gameManager.MatchedCardsData = saveData.MatchedCards;

            StartCoroutine(_gameManager.FromSaveInitialize());
        }
        else
        {
            Debug.LogWarning("No save file found.");
        }

        isLoading = false; // Reset loading flag
    }


    public void SaveGame()
    {
        SaveData saveData = new SaveData();
        saveData.SelectedGameMode = _gameManager.SelectedGameMode;
        saveData.SelectedDifficulty = _gameManager.SelectedDifficulty;
        saveData.GameIsRunning = _gameManager.GameIsRunning;

        // Set cards and matched cards data
        saveData.Cards = _gameManager.CardsData;
        saveData.MatchedCards = _gameManager.MatchedCardsData;

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(savePath, FileMode.Create);

        formatter.Serialize(fileStream, saveData);
        fileStream.Close();

        Debug.Log("Game saved successfully!");
    }
}

[Serializable]
public class SaveData
{
    public List<CardItemData> Cards;
    public List<CardItemData> MatchedCards;
    public GameManager.GameModes SelectedGameMode;
    public GameManager.GameDifficulties SelectedDifficulty;
    public bool GameIsRunning;
}

[Serializable]
public class CardItemData
{
    public int Id;
    public bool IsMine;
    public bool IsClickable;
    public bool HasClicked;
    public bool HasMatched;
    public bool Hidden;
    public int CardSpriteIndex;

    public CardItemData(CardItem card)
    {
        Id = card.Id;
        IsMine = card.IsMine;
        IsClickable = card.IsClickable;
        HasClicked = card.HasClicked;
        HasMatched = card.HasMatched;
        Hidden = card.Hidden;
        CardSpriteIndex = card.CardSpriteIndex;
    }
}