using System.Collections.Generic;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public GridManager gridManager; // Reference to GridManager

    public void SaveGame()
    {
        List<CardState> cardStates = gridManager.GetCardStates();
        GameManager.Instance.SaveGame(cardStates);
    }

    public void LoadGame()
    {
        SaveData saveData = GameManager.Instance.LoadGame();
        if (saveData != null)
        {
            gridManager.SetCardStates(saveData.cardStates);
        }
    }
}
