using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public TMP_InputField nameInput;

    public GameObject settingsPanel;
    public GameObject howToPlayPanel;

    public TMP_Text startButtonText;
    public TMP_Text howToPlayButtonText;
    public TMP_Text settingsButtonText;
    public TMP_Text exitButtonText;

    public TMP_Text settingsTitleText;
    public TMP_Text languageText;
    public TMP_Text closeSettingsButtonText;

    public TMP_Text howToPlayText;
    public TMP_Text closeHowToPlayButtonText;

    private string language;

    void Start()
    {
        language = PlayerPrefs.GetString("Language", "EN");

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (howToPlayPanel != null)
            howToPlayPanel.SetActive(false);

        UpdateLanguageTexts();
    }

    public void StartGame()
    {
        string playerName = "Player";

        if (nameInput != null && !string.IsNullOrWhiteSpace(nameInput.text))
        {
            playerName = nameInput.text;
        }

        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.SetInt("CurrentLevel", 1);
        PlayerPrefs.SetInt("Score", 0);

        SceneManager.LoadScene("GameScene");
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void OpenHowToPlay()
    {
        if (howToPlayPanel != null)
            howToPlayPanel.SetActive(true);
    }

    public void CloseHowToPlay()
    {
        if (howToPlayPanel != null)
            howToPlayPanel.SetActive(false);
    }

    public void SetEnglish()
    {
        language = "EN";
        PlayerPrefs.SetString("Language", language);
        PlayerPrefs.Save();

        UpdateLanguageTexts();
    }

    public void SetTurkish()
    {
        language = "TR";
        PlayerPrefs.SetString("Language", language);
        PlayerPrefs.Save();

        UpdateLanguageTexts();
    }

    void UpdateLanguageTexts()
    {
        if (language == "TR")
        {
            if (nameInput != null && nameInput.placeholder is TMP_Text placeholder)
                placeholder.text = "İsminizi girin";

            if (startButtonText != null)
                startButtonText.text = "Oyuna Başla";

            if (howToPlayButtonText != null)
                howToPlayButtonText.text = "Nasıl Oynanır";

            if (settingsButtonText != null)
                settingsButtonText.text = "Ayarlar";

            if (exitButtonText != null)
                exitButtonText.text = "Çıkış";

            if (settingsTitleText != null)
                settingsTitleText.text = "Ayarlar";

            if (languageText != null)
                languageText.text = "Dil";

            if (closeSettingsButtonText != null)
                closeSettingsButtonText.text = "Kapat";

            if (howToPlayText != null)
            {
                howToPlayText.text =
                    "Nasıl Oynanır\n\n" +
                    "Süre bitmeden eşleşen kartları bul.\n" +
                    "Her seviyede hamle sınırı vardır.\n" +
                    "Süre veya hamle hakkı biterse 1 can kaybedersin.\n" +
                    "Her seviyede 3 can hakkın vardır.\n" +
                    "İpucu kartları kısa süreliğine gösterir.\n" +
                    "10 seviyeyi tamamlayarak oyunu kazan.";
            }

            if (closeHowToPlayButtonText != null)
                closeHowToPlayButtonText.text = "Kapat";
        }
        else
        {
            if (nameInput != null && nameInput.placeholder is TMP_Text placeholder)
                placeholder.text = "Enter your name";

            if (startButtonText != null)
                startButtonText.text = "Start Game";

            if (howToPlayButtonText != null)
                howToPlayButtonText.text = "How to Play";

            if (settingsButtonText != null)
                settingsButtonText.text = "Settings";

            if (exitButtonText != null)
                exitButtonText.text = "Exit";

            if (settingsTitleText != null)
                settingsTitleText.text = "Settings";

            if (languageText != null)
                languageText.text = "Language";

            if (closeSettingsButtonText != null)
                closeSettingsButtonText.text = "Close";

            if (howToPlayText != null)
            {
                howToPlayText.text =
                    "How to Play\n\n" +
                    "Find all matching cards before time runs out.\n" +
                    "Each level has a move limit.\n" +
                    "If time or moves run out, you lose one life.\n" +
                    "You have 3 lives for each level.\n" +
                    "Hint reveals cards for a short time.\n" +
                    "Complete all 10 levels to win.";
            }

            if (closeHowToPlayButtonText != null)
                closeHowToPlayButtonText.text = "Close";
        }
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}