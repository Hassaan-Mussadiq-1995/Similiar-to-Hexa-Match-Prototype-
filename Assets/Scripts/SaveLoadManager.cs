using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SaveLoadManager : MonoBehaviour
{
    public void SaveGame()
    {
        GameManager.Instance.SaveGame();
    }

    public void LoadGame()
    {
        GameManager.Instance.LoadGame();
    }
}
