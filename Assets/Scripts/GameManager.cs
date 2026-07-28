using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Card System")]
    public Card cardPrefab;
    public Transform cardParent;
    public GridLayoutGroup gridLayout;

    [Header("Old Info Text")]
    public TMP_Text infoText;

    [Header("Value Texts")]
    public TMP_Text playerValueText;
    public TMP_Text levelValueText;
    public TMP_Text timeValueText;
    public TMP_Text livesValueText;
    public TMP_Text scoreValueText;
    public TMP_Text movesText;

    [Header("End Panel")]
    public GameObject endPanel;
    public TMP_Text endTitleText;
    public TMP_Text endMessageText;
    public TMP_Text restartButtonText;
    public TMP_Text endMenuButtonText;

    [Header("Language Backgrounds")]
    public GameObject gameBackgroundEN;
    public GameObject gameBackgroundTR;

    [Header("Buttons Text")]
    public TMP_Text pauseButtonText;
    public TMP_Text hintButtonText;
    public TMP_Text menuButtonText;
    public TMP_Text soundButtonText;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip buttonClickSound;
    public AudioClip cardFlipSound;
    public AudioClip correctMatchSound;
    public AudioClip wrongMatchSound;
    public AudioClip winSound;
    public AudioClip loseSound;
    public AudioClip hintSound;

    private Card firstCard;
    private Card secondCard;
    private List<Card> spawnedCards = new List<Card>();

    private int currentLevel;
    private int score;
    private int scoreAtLevelStart;

    private int lives;
    private int movesUsed;
    private int moveLimit;

    private int matchedPairs;
    private int totalPairs;
    private int hintsLeft;

    private float timeLeft;

    private bool canClick = true;
    private bool levelActive = true;
    private bool isPaused = false;
    private bool isHintShowing = false;
    private bool soundEnabled = true;

    private string playerName;
    private string language;

    private string[] symbols =
    {
        "A", "B", "C", "D", "E", "F",
        "G", "H", "I", "J", "K", "L"
    };
    
    [Header("Card Sprites")]
    public Sprite[] cardSprites;

    private int[] cardCounts = { 4, 6, 8, 10, 12, 14, 16, 18, 20, 24 };
    private int[] levelTimes = { 20, 30, 40, 50, 60, 75, 90, 105, 120, 140 };
    private int[] moveLimits = { 3, 5, 7, 9, 11, 13, 15, 18, 21, 25 };

    void Start()
    {
        playerName = PlayerPrefs.GetString("PlayerName", "Player");
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        score = PlayerPrefs.GetInt("Score", 0);
        language = PlayerPrefs.GetString("Language", "EN");

        soundEnabled = PlayerPrefs.GetInt("SoundEnabled", 1) == 1;

        if (currentLevel < 1)
            currentLevel = 1;

        if (currentLevel > 10)
            currentLevel = 10;

        lives = 3;
        hintsLeft = 3;
        scoreAtLevelStart = score;

        if (endPanel != null)
            endPanel.SetActive(false);

        SetLanguageBackground();
        StartLevel(true);
    }

    void Update()
    {
        if (!levelActive || isPaused)
            return;

        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;

            if (timeLeft <= 0)
            {
                timeLeft = 0;
                UpdateInfo();
                LoseLifeAndRetryLevel();
                return;
            }

            UpdateInfo();
        }
    }

    void SetLanguageBackground()
    {
        if (gameBackgroundEN != null)
            gameBackgroundEN.SetActive(language == "EN");

        if (gameBackgroundTR != null)
            gameBackgroundTR.SetActive(language == "TR");
    }

    void StartLevel(bool newLevel)
    {
        if (endPanel != null)
            endPanel.SetActive(false);

        foreach (Transform child in cardParent)
        {
            Destroy(child.gameObject);
        }

        spawnedCards.Clear();

        firstCard = null;
        secondCard = null;

        matchedPairs = 0;
        movesUsed = 0;

        canClick = true;
        levelActive = true;
        isPaused = false;
        isHintShowing = false;

        if (newLevel)
        {
            lives = 3;
            scoreAtLevelStart = score;
        }

        int cardCount = cardCounts[currentLevel - 1];
        moveLimit = moveLimits[currentLevel - 1];

        SetGridByCardCount(cardCount);

        totalPairs = cardCount / 2;
        timeLeft = levelTimes[currentLevel - 1];

        List<int> ids = new List<int>();

        for (int i = 0; i < totalPairs; i++)
        {
            ids.Add(i);
            ids.Add(i);
        }

        Shuffle(ids);

        for (int i = 0; i < ids.Count; i++)
        {
            Card newCard = Instantiate(cardPrefab, cardParent);
            int id = ids[i];

            newCard.Setup(id, cardSprites[id], this);
            spawnedCards.Add(newCard);
        }

        UpdateButtonTexts();
        UpdateInfo();
    }

    public void SelectCard(Card card)
    {
        if (!canClick || !levelActive || isPaused || isHintShowing)
            return;

        if (card == firstCard)
            return;

        PlaySound(cardFlipSound);

        card.ShowCard();

        if (firstCard == null)
        {
            firstCard = card;
        }
        else
        {
            secondCard = card;

            movesUsed++;
            UpdateInfo();

            StartCoroutine(CheckMatch());
        }
    }

    IEnumerator CheckMatch()
    {
        canClick = false;

        yield return new WaitForSeconds(0.7f);

        bool levelEnded = false;

        if (firstCard.cardId == secondCard.cardId)
        {
            PlaySound(correctMatchSound);

            firstCard.MatchCard();
            secondCard.MatchCard();

            score += 10;
            matchedPairs++;

            if (matchedPairs >= totalPairs)
            {
                LevelComplete();
                levelEnded = true;
            }
        }
        else
        {
            PlaySound(wrongMatchSound);

            firstCard.HideCard();
            secondCard.HideCard();
        }

        if (!levelEnded && movesUsed >= moveLimit)
        {
            LoseLifeAndRetryLevel();
            levelEnded = true;
        }

        if (!levelEnded)
        {
            firstCard = null;
            secondCard = null;
            canClick = true;
            UpdateInfo();
        }
    }

    void LoseLifeAndRetryLevel()
    {
        levelActive = false;
        canClick = false;

        lives--;
        score = scoreAtLevelStart;

        if (lives <= 0)
        {
            LoseGame();
        }
        else
        {
            StartLevel(false);
        }
    }

    void LevelComplete()
    {
        levelActive = false;
        canClick = false;

        score += Mathf.RoundToInt(timeLeft);
        PlayerPrefs.SetInt("Score", score);

        if (currentLevel >= 10)
        {
            WinGame();
        }
        else
        {
            currentLevel++;
            PlayerPrefs.SetInt("CurrentLevel", currentLevel);
            StartLevel(true);
        }
    }

    void LoseGame()
    {
        levelActive = false;
        canClick = false;
        timeLeft = 0;

        PlaySound(loseSound);

        if (endPanel != null)
            endPanel.SetActive(true);

        if (language == "TR")
        {
            if (endTitleText != null)
                endTitleText.text = "Kaybettin!";

            if (endMessageText != null)
                endMessageText.text = "Tekrar dene!";

            if (restartButtonText != null)
                restartButtonText.text = "Restart";

            if (endMenuButtonText != null)
                endMenuButtonText.text = "Menü";
        }
        else
        {
            if (endTitleText != null)
                endTitleText.text = "You Lose!";

            if (endMessageText != null)
                endMessageText.text = "Try again!";

            if (restartButtonText != null)
                restartButtonText.text = "Restart";

            if (endMenuButtonText != null)
                endMenuButtonText.text = "Menu";
        }

        UpdateInfo();
    }

    void WinGame()
    {
        levelActive = false;
        canClick = false;
        timeLeft = 0;

        PlaySound(winSound);

        if (endPanel != null)
            endPanel.SetActive(true);

        if (language == "TR")
        {
            if (endTitleText != null)
                endTitleText.text = "Kazandın!";

            if (endMessageText != null)
                endMessageText.text = "Harika iş!";

            if (restartButtonText != null)
                restartButtonText.text = "Restart";

            if (endMenuButtonText != null)
                endMenuButtonText.text = "Menü";
        }
        else
        {
            if (endTitleText != null)
                endTitleText.text = "You Win!";

            if (endMessageText != null)
                endMessageText.text = "Sweet job!";

            if (restartButtonText != null)
                restartButtonText.text = "Restart";

            if (endMenuButtonText != null)
                endMenuButtonText.text = "Menu";
        }

        UpdateInfo();
    }

    void UpdateInfo()
    {
        if (playerValueText != null)
            playerValueText.text = playerName;

        if (levelValueText != null)
            levelValueText.text = currentLevel.ToString();

        if (timeValueText != null)
            timeValueText.text = Mathf.CeilToInt(timeLeft).ToString();

        if (livesValueText != null)
            livesValueText.text = lives.ToString();

        if (scoreValueText != null)
            scoreValueText.text = score.ToString();

        if (movesText != null)
        {
            if (language == "TR")
                movesText.text = "Hamle: " + movesUsed + " / " + moveLimit + "   İpucu: " + hintsLeft;
            else
                movesText.text = "Moves: " + movesUsed + " / " + moveLimit + "   Hint: " + hintsLeft;
        }

        if (infoText != null)
            infoText.text = "";
    }

    void UpdateButtonTexts()
    {
        if (language == "TR")
        {
            if (pauseButtonText != null)
                pauseButtonText.text = isPaused ? "Devam Et" : "Duraklat";

            if (hintButtonText != null)
                hintButtonText.text = "İpucu";

            if (menuButtonText != null)
                menuButtonText.text = "Menü";

            if (soundButtonText != null)
                soundButtonText.text = soundEnabled ? "Ses" : "Sessiz";
        }
        else
        {
            if (pauseButtonText != null)
                pauseButtonText.text = isPaused ? "Play" : "Pause";

            if (hintButtonText != null)
                hintButtonText.text = "Hint";

            if (menuButtonText != null)
                menuButtonText.text = "Menu";

            if (soundButtonText != null)
                soundButtonText.text = soundEnabled ? "Sound" : "Muted";
        }
    }

    public void TogglePause()
    {
        if (!levelActive)
            return;

        PlaySound(buttonClickSound);

        isPaused = !isPaused;
        canClick = !isPaused;

        UpdateButtonTexts();
    }

    public void UseHint()
    {
        if (!levelActive || isPaused || isHintShowing)
            return;

        if (hintsLeft <= 0)
            return;

        PlaySound(hintSound != null ? hintSound : buttonClickSound);

        hintsLeft--;
        StartCoroutine(ShowHintCards());
        UpdateInfo();
    }

    public void ToggleSound()
    {
        soundEnabled = !soundEnabled;

        PlayerPrefs.SetInt("SoundEnabled", soundEnabled ? 1 : 0);
        PlayerPrefs.Save();

        if (soundEnabled)
            PlaySound(buttonClickSound);

        UpdateButtonTexts();
    }

    IEnumerator ShowHintCards()
    {
        isHintShowing = true;
        canClick = false;

        foreach (Card card in spawnedCards)
        {
            if (!card.isMatched)
            {
                card.ShowCard();
            }
        }

        yield return new WaitForSeconds(0.45f);

        foreach (Card card in spawnedCards)
        {
            if (!card.isMatched)
            {
                card.HideCard();
            }
        }

        firstCard = null;
        secondCard = null;

        isHintShowing = false;
        canClick = true;
    }

    void SetGridByCardCount(int cardCount)
    {
        if (gridLayout == null)
            return;

        int columns = 4;

        if (cardCount == 4)
            columns = 2;
        else if (cardCount == 6)
            columns = 3;
        else if (cardCount == 8)
            columns = 4;
        else if (cardCount == 10)
            columns = 5;
        else if (cardCount == 12)
            columns = 4;
        else if (cardCount == 14)
            columns = 4;
        else if (cardCount == 16)
            columns = 4;
        else if (cardCount == 18)
            columns = 6;
        else if (cardCount == 20)
            columns = 5;
        else if (cardCount == 24)
            columns = 6;

        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columns;
    }

    void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);

            int temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (!soundEnabled)
            return;

        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }

    public void RestartGame()
    {
        PlaySound(buttonClickSound);

        PlayerPrefs.SetInt("CurrentLevel", 1);
        PlayerPrefs.SetInt("Score", 0);
        SceneManager.LoadScene("GameScene");
    }

    public void GoToMenu()
    {
        PlaySound(buttonClickSound);

        SceneManager.LoadScene("MainMenu");
    }
}