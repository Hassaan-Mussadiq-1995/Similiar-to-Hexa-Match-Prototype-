using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GridManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Sprite[] cardFronts;
    public Sprite cardBackSprite; // Back sprite for all cards
    public Transform cardGrid; // Parent with Grid Layout Group
    private int[] cardIds;
    private List<Card> cards = new List<Card>(); // List of instantiated cards
    private Card firstCard, secondCard; // To track selected cards

    public float revealDuration = 2.0f;

    public int rows = 4; // Number of rows in the grid
    public int columns = 5; // Number of columns in the grid

    void Start()
    {
        SetupGridLayout(); // Adjust the grid layout
        GenerateCardIds();
        Shuffle(cardIds);
        CreateCards();
        StartCoroutine(RevealAllCards());
       
    }



    // Adjust the Grid Layout based on rows, columns, and screen size
    void SetupGridLayout()
    {
        GridLayoutGroup gridLayout = cardGrid.GetComponent<GridLayoutGroup>();
        if (gridLayout == null)
        {
            Debug.LogError("GridLayoutGroup component is missing on cardGrid!");
            return;
        }

        RectTransform rectTransform = cardGrid.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("RectTransform component is missing on cardGrid!");
            return;
        }

        // Get the size of the container (the GameObject with GridLayoutGroup)
        float containerWidth = rectTransform.rect.width;
        float containerHeight = rectTransform.rect.height;

        // Calculate cell size dynamically
        float cellWidth = (containerWidth - (gridLayout.spacing.x * (columns - 1))) / columns;
        float cellHeight = (containerHeight - (gridLayout.spacing.y * (rows - 1))) / rows;

        gridLayout.cellSize = new Vector2(cellWidth, cellHeight);

        Debug.Log($"Grid Layout: Rows = {rows}, Columns = {columns}, Cell Size = {gridLayout.cellSize}");
    }


    void GenerateCardIds()
    {
        int totalCards = rows * columns;
        if (totalCards > cardFronts.Length * 2)
        {
            Debug.LogError("Not enough card front sprites to generate unique pairs for the grid size!");
            return;
        }
        cardIds = new int[totalCards];
        for (int i = 0; i < totalCards / 2; i++)
        {
            cardIds[i * 2] = i;
            cardIds[i * 2 + 1] = i;
        }

        //cardIds = new int[cardFronts.Length * 2];
        //for (int i = 0; i < cardFronts.Length; i++)
        //{
        //    cardIds[i * 2] = i;
        //    cardIds[i * 2 + 1] = i;
        //}
    }


    // Shuffle the card IDs for randomness
    void Shuffle(int[] array)
    {
       
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int temp = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = temp;
        }
    }

    // Create cards and assign properties
    void CreateCards()
    {
        for (int i = 0; i < cardIds.Length; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardGrid);
            Card card = cardObj.GetComponent<Card>();
            card.cardId = cardIds[i];
            card.frontSprite = cardFronts[cardIds[i]];
            card.backSprite = cardBackSprite;
            card.FlipCardWithoutInteraction(); // Initially show the front
            cards.Add(card);
        }
    }


    IEnumerator RevealAllCards()
    {
        yield return new WaitForSeconds(revealDuration); // Wait for the reveal duration
        foreach (Card card in cards)
        {
            card.ResetCard(); // Hide all cards
        }
    }

    public void CheckForMatch(Card selectedCard)
    {
        if (firstCard == null)
        {
            firstCard = selectedCard;
            return;
        }

        secondCard = selectedCard;

        // Check if IDs match
        if (firstCard.cardId == secondCard.cardId)
        {
            Debug.Log("Match!");
            firstCard = secondCard = null; // Clear the selection
        }
        else
        {
            Debug.Log("No Match!");
            Invoke("ResetCards", 1f); // Delay before flipping back
        }
    }
    private void ResetCards()
    {
        firstCard.ResetCard();
        secondCard.ResetCard();
        firstCard = secondCard = null;
    }

    public List<CardState> GetCardStates()
    {
        List<CardState> states = new List<CardState>();
        foreach (Card card in cards)
        {
            states.Add(new CardState
            {
                cardId = card.cardId,
                isFlipped = card.IsFlipped()
            });
        }
        return states;
    }

    // Set card states from loaded data
    public void SetCardStates(List<CardState> states)
    {
        for (int i = 0; i < states.Count; i++)
        {
            Card card = cards[i];
            if (states[i].isFlipped)
            {
                card.FlipCardWithoutInteraction(); // Implement a method to flip the card visually without triggering interaction
            }
            else
            {
                card.ResetCard();
            }
        }
    }
}
