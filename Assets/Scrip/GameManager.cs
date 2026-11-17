using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public Transform cardContainer;
    public GameObject cardPrefabs;
    public Sprite[] cardIcons;

    [SerializeField] private Button newGameButton;

    private GameTimer gameTimer;
    private CardController firstSelectedCard;
    private CardController secondSelectedCard;
    private bool isResolvingPair;
    private bool isGameOver;

    private void Start()
    {
        gameTimer = FindFirstObjectByType<GameTimer>();

        if (gameTimer == null)
        {
            Debug.LogError("GameTimer was not found in the scene.", this);
        }

        if (newGameButton == null)
        {
            Debug.LogError("Assign the NewGame button in the GameManager Inspector.", this);
        }
        else
        {
            newGameButton.onClick.AddListener(StartNewGame);
            newGameButton.gameObject.SetActive(false);
        }

        StartNewGame();
    }

    private void Update()
    {
        if (isGameOver == false && gameTimer != null && gameTimer.HasTimeRunOut())
        {
            EndGame();
        }
    }

    private void OnDestroy()
    {
        if (newGameButton != null)
        {
            newGameButton.onClick.RemoveListener(StartNewGame);
        }
    }

    private void StartNewGame()
    {
        StopAllCoroutines();
        isGameOver = true;

        if (newGameButton != null)
        {
            newGameButton.gameObject.SetActive(false);
        }

        if (cardContainer == null || cardPrefabs == null || cardIcons == null)
        {
            Debug.LogError("Assign the card container, card prefab, and card icons in the Inspector.", this);
            return;
        }

        StartCoroutine(CreateNewGame());
    }

    private IEnumerator CreateNewGame()
    {
        for (int i = cardContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(cardContainer.GetChild(i).gameObject);
        }

        firstSelectedCard = null;
        secondSelectedCard = null;
        isResolvingPair = false;

        // Wait until Unity removes the old cards before shuffling the new ones.
        yield return null;

        for (int i = 0; i < cardIcons.Length; i++)
        {
            CreateCardPair(i);
        }

        ShuffleCards();
        isGameOver = false;

        if (gameTimer != null)
        {
            gameTimer.ResetTimer();
            gameTimer.StartTimer();
        }
    }

    private void CreateCardPair(int cardId)
    {
        GameObject firstCardObject = Instantiate(cardPrefabs, cardContainer);
        GameObject secondCardObject = Instantiate(cardPrefabs, cardContainer);

        CardController firstCard = firstCardObject.GetComponent<CardController>();
        CardController secondCard = secondCardObject.GetComponent<CardController>();

        firstCard.InitializeCard(cardId, cardIcons[cardId], this);
        secondCard.InitializeCard(cardId, cardIcons[cardId], this);
    }

    private void ShuffleCards()
    {
        int numberOfCards = cardContainer.childCount;

        for (int i = 0; i < numberOfCards; i++)
        {
            int randomIndex = Random.Range(0, numberOfCards);
            cardContainer.GetChild(randomIndex).SetAsFirstSibling();
        }
    }

    private void EndGame()
    {
        isGameOver = true;
        StopAllCoroutines();

        firstSelectedCard = null;
        secondSelectedCard = null;
        isResolvingPair = false;

        if (newGameButton != null)
        {
            newGameButton.gameObject.SetActive(true);
        }
    }

    public void SelectCard(CardController card)
    {
        if (isGameOver || isResolvingPair || card.IsFaceUp || card.IsMatched)
        {
            return;
        }

        card.SetFaceUp(true);

        if (firstSelectedCard == null)
        {
            firstSelectedCard = card;
            return;
        }

        secondSelectedCard = card;
        isResolvingPair = true;
        StartCoroutine(CheckSelectedCards());
    }

    private IEnumerator CheckSelectedCards()
    {
        yield return new WaitForSeconds(1f);

        if (firstSelectedCard.id == secondSelectedCard.id)
        {
            firstSelectedCard.MarkMatched();
            secondSelectedCard.MarkMatched();

            Destroy(firstSelectedCard.gameObject);
            Destroy(secondSelectedCard.gameObject);
        }
        else
        {
            firstSelectedCard.SetFaceUp(false);
            secondSelectedCard.SetFaceUp(false);
        }

        firstSelectedCard = null;
        secondSelectedCard = null;
        isResolvingPair = false;
    }
}
