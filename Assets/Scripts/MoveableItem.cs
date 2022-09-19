using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableItem : MonoBehaviour
{
    private GameItem item;
    private IEnumerator moveCoroutine;

    void Awake()
    {
        item = GetComponent<GameItem>();   
    }
 

    public void Move(int newX, int newY, float time) 
    {
        if(moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine = MoveCoroutine(newX, newY, time);
        StartCoroutine(moveCoroutine);
    }

    private IEnumerator MoveCoroutine(int newX, int newY, float time)
    {
        item.X = newX;
        item.Y = newY;

        Vector3 startPos = transform.position;
        Vector3 endPos = item.BoardManager.GetWorldPosition(newX, newY);

        for(float t = 0; t <= 1 * time; t+= Time.deltaTime)
        {
            item.transform.position = Vector3.Lerp(startPos, endPos, t/time);
            yield return 0;
        }

        item.transform.position = endPos;
    }
}
