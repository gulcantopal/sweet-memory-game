using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Card : MonoBehaviour
{
    public int cardId;
    public bool isMatched;
    public bool isFlipped;

    public Button button;
    public TMP_Text cardText;
    public Image cardImage;   // Kartın kendi arka plan Image'ı
    public Image iconImage;   // Yeni eklediğimiz IconImage

    private Sprite cardSprite;
    private GameManager gameManager;

    public void Setup(int id, Sprite newSprite, GameManager manager)
    {
        cardId = id;
        cardSprite = newSprite;
        gameManager = manager;

        isMatched = false;
        isFlipped = false;

        if (iconImage != null)
        {
            iconImage.sprite = cardSprite;
            iconImage.gameObject.SetActive(false);
        }

        HideCard();
    }

    public void OnCardClicked()
    {
        if (isMatched || isFlipped)
            return;

        gameManager.SelectCard(this);
    }

    public void ShowCard()
    {
        isFlipped = true;

        if (cardText != null)
            cardText.gameObject.SetActive(false);

        if (iconImage != null)
            iconImage.gameObject.SetActive(true);

        if (cardImage != null)
            cardImage.color = new Color(1f, 0.85f, 0.95f);
    }

    public void HideCard()
    {
        if (isMatched)
            return;

        isFlipped = false;

        if (cardText != null)
        {
            cardText.gameObject.SetActive(true);
            cardText.text = "?";
        }

        if (iconImage != null)
            iconImage.gameObject.SetActive(false);

        if (cardImage != null)
            cardImage.color = new Color(1f, 0.55f, 0.78f);
    }

    public void MatchCard()
    {
        isMatched = true;
        isFlipped = true;

        if (cardText != null)
            cardText.gameObject.SetActive(false);

        if (iconImage != null)
            iconImage.gameObject.SetActive(true);

        if (cardImage != null)
            cardImage.color = new Color(0.75f, 1f, 0.8f);
    }
}