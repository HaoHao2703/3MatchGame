using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum LevelType
    {
        TIMER,
        MOVES,
    }

    public BoardManager boardManager;

    private int currentScore;
    private int targetScore = 7000;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI targetScoreTxt;
    public GameObject gameOverUI;
    

    protected LevelType type;
    public LevelType Type { get { return type; } }

    public virtual void GameWin()
    {
        boardManager.GameOver();
    }

    public virtual void OnMove()
    {
        Debug.Log("Move");
    }

    public virtual void OnPieceCleared(GameItem item)
    {
        //Update score
        currentScore += item.score;
        scoreText.text = "Score: " + currentScore.ToString();

        //Check level Complete
        LevelComplete();


    }

    private void LevelComplete()
    {
        targetScoreTxt.text = "Target: " + targetScore.ToString();

        if (currentScore == targetScore)
        {
            boardManager.GameOver();
            gameOverUI.gameObject.SetActive(true);
        }
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
