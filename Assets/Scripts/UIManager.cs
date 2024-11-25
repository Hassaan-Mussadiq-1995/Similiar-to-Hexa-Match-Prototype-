using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text scoreText;

    private void Update()
    {
        scoreText.text = "Score: " + GameManager.Instance.score;
    }
    public void RefreshScore()
    {
        scoreText.text = "Score: " + GameManager.Instance.score;
    }

}
