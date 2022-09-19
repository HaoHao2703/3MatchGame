using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FruitItem : MonoBehaviour
{
    public enum FruitType
    {
        ORANGE, 
        STRAWBERRY,
        BANANA, 
        APPLE,
        BLUEBERRY,
        GRAPE, 
        PEAR,
        ANY,
        COUNT
    };

    private FruitType fruitType;
    public FruitType _FruitType
    {
        get { return fruitType; }
        set { SetFruitType(value); }
    }

    public int NumFruits { get { return fruitSprites.Length; } }

    private SpriteRenderer sprite;

    [System.Serializable]
    public struct FruitSprite
    {
        public FruitType type;
        public Sprite sprite;
    }

    public FruitSprite[] fruitSprites;

    private Dictionary<FruitType, Sprite> fruitSpriteDict;

    // Start is called before the first frame update
    void Awake()
    {
        sprite = transform.Find("Apple").GetComponent<SpriteRenderer>();
        fruitSpriteDict = new Dictionary<FruitType, Sprite>();

        for(int i = 0; i < fruitSprites.Length; i++)
        {
            if (!fruitSpriteDict.ContainsKey(fruitSprites[i].type))
            {
                fruitSpriteDict.Add(fruitSprites[i].type, fruitSprites[i].sprite);
            }
        }
    }

    public void SetFruitType(FruitType newFruit)
    {
        fruitType = newFruit;

        if (fruitSpriteDict.ContainsKey(newFruit))
        {
            sprite.sprite = fruitSpriteDict[newFruit];
        }
    }
}
