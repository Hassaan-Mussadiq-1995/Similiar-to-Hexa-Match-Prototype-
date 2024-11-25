using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Card : MonoBehaviour
{
    public int cardId;
    public Sprite frontSprite; // Front of the card
    public Sprite backSprite; // Back of the card
    public Image imageComponent; // UI Image component
    private bool isFlipped = false;
    public Animator animator;


    private void Start()
    {
        //animator = GetComponent<Animator>();
        imageComponent = GetComponent<Image>();
        //ResetCard();
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnCardClicked);
        }
    }

    private void OnCardClicked()
    {
        FlipCard();
    }

    public void FlipCard()
    {
        if (isFlipped) return;

        isFlipped = true;
        animator.SetTrigger("Flip");
        imageComponent.sprite = frontSprite; // Show front sprite

        GameManager.Instance.AddScore(10); // Example score increment
        FindObjectOfType<GridManager>().CheckForMatch(this);
    }

    public void ResetCard()
    {
        isFlipped = false;
        animator.SetTrigger("Reset");
        imageComponent.sprite = backSprite;
    }
    public void FlipCardWithoutInteraction()
    {
        isFlipped = true;
        imageComponent.sprite = frontSprite;
    }
    public bool IsFlipped()
    {
        return isFlipped;
    }
}
