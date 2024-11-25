using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Sprite[] cardFronts;
    public Sprite cardBackSprite; // Back sprite for all cards
    public Transform cardGrid; // Parent with Grid Layout Group
    private int[] cardIds;
    private List<Card> cards = new List<Card>(); // List of instantiated cards
    private Card firstCard, secondCard; // To track selected cards

    void Start()
    { 
        GenerateCardIds();
        Shuffle(cardIds);
        CreateCards();

        //// Generate card IDs (pairs for matching)
        //cardIds = new int[cardFronts.Length * 2];
        //for (int i = 0; i < cardFronts.Length; i++)
        //{
        //    cardIds[i * 2] = i;
        //    cardIds[i * 2 + 1] = i;
        //}

        //// Shuffle IDs
        //Shuffle(cardIds);

        //// Create cards
        //for (int i = 0; i < cardIds.Length; i++)
        //{
        //    GameObject card = Instantiate(cardPrefab, transform);
        //    card.GetComponent<Card>().cardId = cardIds[i];
        //    card.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = cardFronts[cardIds[i]];
        //}
    }


    void GenerateCardIds()
    {
        cardIds = new int[cardFronts.Length * 2];
        for (int i = 0; i < cardFronts.Length; i++)
        {
            cardIds[i * 2] = i;
            cardIds[i * 2 + 1] = i;
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
            card.ResetCard(); // Initialize with the back sprite
            cards.Add(card);
        }
    }

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
}
