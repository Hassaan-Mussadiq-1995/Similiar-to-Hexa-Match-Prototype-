using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GridManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Sprite[] cardFronts;
    public Sprite cardBackSprite; // Back sprite for all cards
    public Transform cardGrid; // Parent with Grid Layout Group
    public int rows = 4; // Number of rows in the grid
    public int columns = 5; // Number of columns in the grid
    public float revealDuration = 2.0f;
    public Text cardCounterText; // UI element to display 50/0 format
    public int totalCards = 50; // Total number of cards
    public int moveLimit; // Maximum allowed moves
    public Text movesCounterText; // UI element to display remaining moves
    private int remainingMoves; // Moves remaining for the player
    public Text LevelCompleteText, LevelFailText;


    private int[] cardIds;
    private List<Card> cards = new List<Card>(); // List of instantiated cards
    private int matchedCards = 0; // Number of matched cards
    private int remainingCards = 0; // Cards currently in the grid
    //private int maxGridCards = 20; // Maximum cards in the grid at a time
    private int replenishThreshold = 4; // Threshold to add more cards

    private Card firstCard, secondCard; // To track selected cards


    private bool isResolving = true; // Flag to prevent interactions during resolution
    private int remainingMatches; // Number of remaining card pairs to match
    


    public bool IsResolving()
    {
        return isResolving;
    }

    private void Update()
    {
        HandleTouchInput();
    }

    private void HandleTouchInput()
    {
        // Check if there are any touches
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);

                if (touch.phase == TouchPhase.Began)
                {
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        Card touchedCard = hit.collider.GetComponent<Card>();
                        if (touchedCard != null && !touchedCard.IsFlipped())
                        {
                            touchedCard.FlipCard();
                        }
                    }
                }
            }
        }
    }

    void Start()
    {
        moveLimit = totalCards + Mathf.CeilToInt(totalCards * 0.5f); // Add 50% buffer
        remainingMoves = moveLimit;

        SetupGrid();
        UpdateCardCounter();
        UpdateMovesCounter();

        //SetupLevel();
    }
    void UpdateMovesCounter()
    {
        movesCounterText.text = $"Moves: {remainingMoves}/{moveLimit}";
    }

    // Setup the initial grid with cards
    void SetupGrid()
    {
        remainingCards = Mathf.Min((rows * columns), totalCards - matchedCards); // Limit cards to maxGridCards
        SetupGridLayout();
        CreateCards(remainingCards);
        //StartCoroutine(RevealAllCards());

    }

    void CreateCards(int count)
    {
        List<int> newCardIds = GenerateCardIds(count);

        foreach (int cardId in newCardIds)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardGrid);
            Card card = cardObj.GetComponent<Card>();
            card.cardId = cardId;
            card.frontSprite = cardFronts[cardId];
            card.backSprite = cardBackSprite;
            card.GetComponent<Image>().sprite = cardFronts[cardId];
            cards.Add(card);
        }
        StartCoroutine(RevealAllCards());

    }

    // Generate card IDs for new cards
    List<int> GenerateCardIds(int count)
    {
        List<int> cardIds = new List<int>();

        // Create a list of available card indices
        List<int> availableIds = new List<int>();
        for (int i = 0; i < cardFronts.Length; i++) // Assuming cardFronts.Length is the total number of unique cards
        {
            availableIds.Add(i);
        }

        // Randomly select IDs for pairs
        List<int> selectedIds = new List<int>();
        for (int i = 0; i < count / 2; i++)
        {
            int randomIndex = Random.Range(0, availableIds.Count);
            selectedIds.Add(availableIds[randomIndex]); // Select a random ID
            availableIds.RemoveAt(randomIndex); // Remove it to avoid duplicates
        }

        // Add pairs to the card IDs list
        foreach (int id in selectedIds)
        {
            cardIds.Add(id);
            cardIds.Add(id);
        }

        // Shuffle the list to randomize card positions
        Shuffle(cardIds);

        return cardIds;


        //List<int> cardIds = new List<int>();

        //for (int i = 0; i < count / 2; i++)
        //{
        //    cardIds.Add(i);
        //    cardIds.Add(i); // Add pairs
        //}

        //Shuffle(cardIds);
        //return cardIds;
    }


    void Shuffle(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int temp = list[randomIndex];
            list[randomIndex] = list[i];
            list[i] = temp;
        }
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


    #region For levels Base normally
    //// Setup the initial or next level
    //void SetupLevel()
    //{
    //    remainingMatches = (rows * columns) / 2; // Calculate the number of pairs
    //    SetupGridLayout();
    //    GenerateCardIds();
    //    Shuffle(cardIds);
    //    CreateCards();
    //    StartCoroutine(RevealAllCards());
    //}

    //// Adjust the Grid Layout based on rows, columns, and screen size
    //void SetupGridLayout()
    //{
    //    GridLayoutGroup gridLayout = cardGrid.GetComponent<GridLayoutGroup>();
    //    if (gridLayout == null)
    //    {
    //        Debug.LogError("GridLayoutGroup component is missing on cardGrid!");
    //        return;
    //    }

    //    RectTransform rectTransform = cardGrid.GetComponent<RectTransform>();
    //    if (rectTransform == null)
    //    {
    //        Debug.LogError("RectTransform component is missing on cardGrid!");
    //        return;
    //    }

    //    // Get the size of the container (the GameObject with GridLayoutGroup)
    //    float containerWidth = rectTransform.rect.width;
    //    float containerHeight = rectTransform.rect.height;

    //    // Calculate cell size dynamically
    //    float cellWidth = (containerWidth - (gridLayout.spacing.x * (columns - 1))) / columns;
    //    float cellHeight = (containerHeight - (gridLayout.spacing.y * (rows - 1))) / rows;

    //    gridLayout.cellSize = new Vector2(cellWidth, cellHeight);

    //    Debug.Log($"Grid Layout: Rows = {rows}, Columns = {columns}, Cell Size = {gridLayout.cellSize}");
    //}


    //void GenerateCardIds()
    //{
    //    int totalCards = rows * columns;
    //    if (totalCards > cardFronts.Length * 2)
    //    {
    //        Debug.LogError("Not enough card front sprites to generate unique pairs for the grid size!");
    //        return;
    //    }
    //    cardIds = new int[totalCards];
    //    for (int i = 0; i < totalCards / 2; i++)
    //    {
    //        cardIds[i * 2] = i;
    //        cardIds[i * 2 + 1] = i;
    //    }
    //}


    //// Shuffle the card IDs for randomness
    //void Shuffle(int[] array)
    //{

    //    for (int i = array.Length - 1; i > 0; i--)
    //    {
    //        int randomIndex = Random.Range(0, i + 1);
    //        int temp = array[randomIndex];
    //        array[randomIndex] = array[i];
    //        array[i] = temp;
    //    }
    //}

    //// Create cards and assign properties
    //void CreateCards()
    //{
    //    cards.Clear(); // Clear the card list

    //    foreach (int cardId in cardIds)
    //    {
    //        GameObject cardObj = Instantiate(cardPrefab, cardGrid);
    //        Card card = cardObj.GetComponent<Card>();
    //        card.cardId = cardId;
    //        card.frontSprite = cardFronts[cardId];
    //        card.backSprite = cardBackSprite;
    //        card.GetComponent<Image>().sprite = cardFronts[cardId];

    //        cards.Add(card);
    //    }

    //}
    //IEnumerator NextLevel()
    //{
    //    yield return new WaitForSeconds(1f);
    //    foreach (Transform child in cardGrid)
    //    {
    //        Destroy(child.gameObject); // Clear current grid
    //    }

    //    rows = Random.Range(3, 6); // Random rows between 3 and 5
    //    columns = Random.Range(4, 7); // Random columns between 4 and 6
    //    SetupLevel(); // Start the next level
    //}
    #endregion

    IEnumerator RevealAllCards()
    {
        yield return new WaitForSeconds(revealDuration); // Wait for the reveal duration
        foreach (Card card in cards)
        {
            card.ResetCard(); // Hide all cards
        }
        isResolving = false;
    }

    public void CheckForMatch(Card selectedCard)
    {
        //if (isResolving) return; // Prevent interactions if resolving a match
        if (isResolving || remainingMoves <= 0) return;

        remainingMoves--;
        UpdateMovesCounter();

        if (remainingMoves <= 0)
        {
            GameOver();
            return;
        }

        if (firstCard == null)
        {
            firstCard = selectedCard;
            return;
        }

        secondCard = selectedCard;
        isResolving = true;
        // Check if IDs match
        if (firstCard.cardId == secondCard.cardId)
        {
            Debug.Log("Match!");
            StartCoroutine(HandleMatch(firstCard, secondCard));
        }
        else
        {
            Debug.Log("No Match!");
            Invoke("ResetCards", 1f); // Delay before flipping back
        }
    }

    IEnumerator HandleMatch(Card FirstCard,Card SecondCard)
    {
       
        yield return new WaitForSecondsRealtime(1f);
        FirstCard.GetComponent<Image>().enabled = SecondCard.GetComponent<Image>().enabled = false;
        //FirstCard.gameObject.SetActive(false);
        //SecondCard.gameObject.SetActive(false);

        matchedCards += 2;
        remainingCards -= 2;

        UpdateCardCounter();


        ClearCards();
        isResolving = false;

        if (matchedCards >= totalCards)
        {
            isResolving = true;
            StartCoroutine(NextLevel());
        }
        else if (remainingCards < replenishThreshold)
        {
            RemoveCards();
            ReplenishGrid();
        }

        //remainingMatches--;

        //if (remainingMatches <= 0)
        //{
        //    Debug.Log("All matches complete! Loading next level...");
        //    StartCoroutine(NextLevel());
        //}

    }
    private void ResetCards()
    {
        firstCard.ResetCard();
        secondCard.ResetCard();
        ClearCards();
        isResolving = false; // Allow interactions again
    }

    private void ClearCards()
    {
        firstCard = null;
        secondCard = null;
    }

    void ReplenishGrid()
    {
       
        int cardsToAdd = Mathf.Min((rows * columns) - remainingCards, totalCards - matchedCards);
        CreateCards(cardsToAdd);
        remainingCards += cardsToAdd;
    }

    // Update the card counter display
    void UpdateCardCounter()
    {
        cardCounterText.text = $"{matchedCards}/{totalCards}";
    }

    IEnumerator NextLevel()
    {
        LevelCompleteText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        LevelCompleteText.gameObject.SetActive(false); 

        Debug.Log("Level Complete! Starting next level...");
        // Clear current grid
        foreach (Transform child in cardGrid)
        {
            cards.Remove(child.GetComponent<Card>());
            Destroy(child.gameObject);
        }

        // Increase target cards for the next level
        totalCards += 10; // Increase total cards by 10 for each level
        matchedCards = 0;
        remainingCards = 0;

        // Setup the new level
        SetupGrid();
        UpdateCardCounter();
        moveLimit = totalCards + Mathf.CeilToInt(totalCards * 0.5f); // Add 50% buffer
        remainingMoves = moveLimit;
        UpdateMovesCounter();
    }


    void GameOver()
    {
        LevelFailText.gameObject.SetActive(true);
        Debug.Log("Game Over! No moves left.");
        // Show Game Over UI or restart the level
        StartCoroutine(RestartLevel());
    }
    IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(2f); // Optional delay before restart
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
    }

    public void RemoveCards()
    {
        List<Card> cardsToRemove = new List<Card>(); // Temporary list to store cards to remove

        // Identify cards to remove
        foreach (Card card in cards)
        {
            if (!card.GetComponent<Image>().enabled) // Check if the card is disabled
            {
                cardsToRemove.Add(card); // Add to temporary list
            }
        }

        // Remove cards from the main list and destroy them
        foreach (Card card in cardsToRemove)
        {
            cards.Remove(card);
            Destroy(card.gameObject);
        }

        
        
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
