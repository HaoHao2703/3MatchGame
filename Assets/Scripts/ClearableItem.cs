using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearableItem : MonoBehaviour
{
    private bool isCleared;
    public bool IsCleared { get { return isCleared; } }

    protected GameItem fruit;
    // Start is called before the first frame update
    void Start()
    {
        fruit= GetComponent<GameItem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void Clear()
    {
        fruit.BoardManager.gameManager.OnPieceCleared(fruit);
        isCleared = true;
        StartCoroutine(ClearCoroutine());
    }
    private IEnumerator ClearCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }
}
