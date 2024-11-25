using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameManager : MonoBehaviour
{
    public int score = 0;
    public string saveFileName = "saveData.json";

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int points)
    {
        score += points;
    }

    public void SaveGame(List<CardState> cardStates)
    {
        SaveData saveData = new SaveData
        {
            score = score,
            cardStates = cardStates
        };

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(Application.persistentDataPath + "/" + saveFileName, json);
        Debug.Log("Game Saved!");
    }

    public SaveData LoadGame()
    {
        string path = Application.persistentDataPath + "/" + saveFileName;
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);
            score = saveData.score;
            Debug.Log("Game Loaded!");
            return saveData;
        }

        Debug.LogWarning("No Save File Found!");
        return null;
    }
}

[System.Serializable]
public class SaveData
{
    public int score;
    public List<CardState> cardStates;
}

[System.Serializable]
public class CardState
{
    public int cardId;
    public bool isFlipped;
}
