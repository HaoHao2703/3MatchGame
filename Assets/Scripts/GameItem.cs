using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameItem : MonoBehaviour
{
    public int score;
    //GameObject position
    private int x;
    private int y;

    public int X
    {
        get { return x; }
        set
        {
            if (IsMoveable())
            {
                x = value;
            }
        }
    }
    public int Y 
    {
        get { return y; }
        set
        {
            if (IsMoveable())
            {
                y = value;
            }
        }
    }

    private BoardManager.ItemType type;
    public BoardManager.ItemType Type { get { return type; } }

    private BoardManager boardManager;
    public BoardManager BoardManager { get { return boardManager; } }

    private MoveableItem movableComponent;
    public MoveableItem MovableComponent { get { return movableComponent; } }

    private FruitItem fruitComponent;
    public FruitItem FruitComponent { get { return fruitComponent; } }

    private ClearableItem clearableComponent;
    public ClearableItem ClearableComponent { get { return clearableComponent; } }

    private void Awake()
    {
        movableComponent = GetComponent<MoveableItem>();
        fruitComponent = GetComponent<FruitItem>();
        clearableComponent = GetComponent<ClearableItem>();
    }


    public void InitItem(int _x, int _y, BoardManager _board, BoardManager.ItemType _type)
    {
        x = _x;
        y = _y;
        boardManager = _board;
        type = _type;
    }

    public bool IsMoveable()
    {
        return movableComponent != null;
    }

    public bool IsFruit()
    {
        return fruitComponent != null;
    }

    void OnMouseEnter()
    {
        boardManager.EnterdItem(this);
    }

    void OnMouseDown()
    {
        boardManager.PressItem(this);
    }

    void OnMouseUp()
    {
        boardManager.ReleaseItem();
    }

    public bool IsClearable()
    {
        return clearableComponent != null;
    }

}
