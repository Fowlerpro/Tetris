using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    public TetrisManager tetrisManager;

    public GameObject endGamePanel;

    public void UpdateScore()
    {
        scoreText.text = $"SCORE: {tetrisManager.score:n0}";
    }

    public void UpdateGameOver()
    {    
        endGamePanel.SetActive(tetrisManager.gameOver);
    }

    public void PlayAgain()
    {   
        tetrisManager.SetGameOver(false);
    }
}