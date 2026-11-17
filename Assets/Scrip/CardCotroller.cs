using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    public Image icon;
    public int id;

    private Button cardButton;
    private Animator cardAnimator;
    private GameManager gameManager;
    private bool isFaceUp;
    private bool isMatched;

    private const string FlipParameter = "isFlip";

    public bool IsFaceUp
    {
        get { return isFaceUp; }
    }

    public bool IsMatched
    {
        get { return isMatched; }
    }

    private void Awake()
    {
        cardAnimator = GetComponent<Animator>();
        if (cardAnimator == null)
        {
            Debug.LogError("Card prefab needs an Animator component.", this);
        }
        else
        {
            cardAnimator.SetBool(FlipParameter, false);
        }

        cardButton = GetComponent<Button>();
        if (cardButton != null)
        {
            cardButton.onClick.AddListener(OnCardClicked);
        }
    }

    private void OnDestroy()
    {
        if (cardButton != null)
        {
            cardButton.onClick.RemoveListener(OnCardClicked);
        }
    }

    public void InitializeCard(int cardId, Sprite cardIcon, GameManager manager)
    {
        id = cardId;
        gameManager = manager;

        if (icon != null)
        {
            icon.sprite = cardIcon;
        }
    }

    public void SetFaceUp(bool showFace)
    {
        if (isFaceUp == showFace)
        {
            return;
        }

        isFaceUp = showFace;

        if (cardAnimator != null)
        {
            cardAnimator.SetBool(FlipParameter, showFace);
        }
    }

    public void MarkMatched()
    {
        isMatched = true;

        if (cardButton != null)
        {
            cardButton.interactable = false;
        }
    }

    private void OnCardClicked()
    {
        if (gameManager != null)
        {
            gameManager.SelectCard(this);
        }
    }
}
