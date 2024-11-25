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

    public float revealDuration = 2.0f;

    void Start()
    { 
        GenerateCardIds();
        Shuffle(cardIds);
        CreateCards();
        StartCoroutine(RevealAllCards());
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

    IEnumerator RevealAllCards()
    {
        yield return new WaitForSeconds(revealDuration); // Wait for the reveal duration
        foreach (Card card in cards)
        {
            card.ResetCard(); // Hide all cards
        }
    }

    void GenerateCardIds()
    {
        int totalCards = cardFronts.Length * 2;
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
