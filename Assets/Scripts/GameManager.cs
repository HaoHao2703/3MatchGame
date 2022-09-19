using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum LevelType
    {
        TIMER,
        MOVES,
    }

    public BoardManager boardManager;

    protected int currentScore;

    public TextMeshProUGUI scoreText;
    

    protected LevelType type;
    public LevelType Type { get { return type; } }

    public virtual void GameOver()
    {
        Debug.Log("Loose");
    }

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
    }

}
