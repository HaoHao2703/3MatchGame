using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoardManager : MonoBehaviour
{
    public enum ItemType
    {
        NORMAL,
        EMPTY,
        COUNT,
    };

    [System.Serializable]
    public struct ItemPrefab
    {
        public ItemType type;
        public GameObject prefab;
    }

    public ItemPrefab[] itemPrefabs;
    public GameObject bgPrefab;

    private GameItem[,] items;

    private float fillTime = 0.1f;

    private GameItem pressItem;
    private GameItem enteredItem;

    public GameManager gameManager;
    private bool gameOver = false;

    public int xDim;
    public int yDim;


    private Dictionary<ItemType, GameObject> itemPrefabDict;
    private Dictionary<ItemType, GameObject> refItemPrefabDict;

    private GameObject Apple;
    void Start()
    {
        SetUp();
    }

    private void SetUp()
    {
        itemPrefabDict = new Dictionary<ItemType, GameObject>();

        for (int i = 0; i < itemPrefabs.Length; i++)
        {
            if (!itemPrefabDict.ContainsKey(itemPrefabs[i].type))
            {
                itemPrefabDict.Add(itemPrefabs[i].type, itemPrefabs[i].prefab);
            }
        }

        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                GameObject bg = (GameObject)Instantiate(bgPrefab, GetWorldPosition(x, y), Quaternion.identity);
                bg.transform.parent = transform;
            }
        }

        items = new GameItem[xDim, yDim];
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                SpawnNewFruit(x, y, ItemType.EMPTY);
            }
        }
        StartCoroutine(Fill());
    }
    /*public void ReArrange()
    {

        for (int i = 0; i < itemPrefabs.Length; i++)
        {
            if (itemPrefabDict.ContainsValue(itemPrefabs[i].prefab))
            {
                itemPrefabDict.Remove(itemPrefabs[i].type);
                itemPrefabDict.Add(ItemType.EMPTY, bgPrefab);
                Debug.Log("Remove");
            }
        }

        refItemPrefabDict = new Dictionary<ItemType, GameObject>();

        for (int i = 0; i < itemPrefabs.Length; i++)
        {
            if (!refItemPrefabDict.ContainsKey(itemPrefabs[i].type))
            {
                refItemPrefabDict.Add(itemPrefabs[i].type, itemPrefabs[i].prefab);
                itemPrefabs[i].prefab.SetActive(true);
            }
        }

        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                GameObject bg = (GameObject)Instantiate(bgPrefab, GetWorldPosition(x, y), Quaternion.identity);
                bg.transform.parent = transform;
            }
        }

        items = new GameItem[xDim, yDim];
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                SpawnNewFruit(x, y, ItemType.NORMAL);
            }
        }
        StartCoroutine(Fill());
    }*/
    public IEnumerator Fill()
    {
        bool needRefill = true;

        while (needRefill)
        {
            yield return new WaitForSeconds(fillTime);
            while (FillStep())
            {
                yield return new WaitForSeconds(fillTime);
            }

            needRefill = ClearAllValidMatches();
        }
        
    }

    public bool FillStep()
    {
        bool movedPiece = false;
        for (int y = yDim - 2; y >= 0; y--)
        {
            for (int x = 0; x < xDim; x++)
            {
                GameItem item = items[x, y];

                if (item.IsMoveable())
                {
                    GameItem itemBelow = items[x, y + 1];

                    if (itemBelow.Type == ItemType.EMPTY)
                    {
                        Destroy(itemBelow.gameObject);
                        item.MovableComponent.Move(x, y + 1, fillTime);
                        items[x, y + 1] = item;
                        SpawnNewFruit(x, y, ItemType.EMPTY);
                        movedPiece = true;
                    }
                }
            }
        }

        for (int x = 0; x < xDim; x++)
        {
            GameItem itemBelow = items[x, 0];
            if (itemBelow.Type == ItemType.EMPTY)
            {
                Destroy(itemBelow.gameObject);
                GameObject newFruit = (GameObject)Instantiate(itemPrefabDict[ItemType.NORMAL], GetWorldPosition(x, -1), Quaternion.identity);
                newFruit.transform.parent = transform;

                items[x, 0] = newFruit.GetComponent<GameItem>();
                items[x, 0].InitItem(x, -1, this, ItemType.NORMAL);
                items[x, 0].MovableComponent.Move(x, 0, fillTime);
                items[x, 0].FruitComponent.SetFruitType((FruitItem.FruitType)Random.Range(0, items[x, 0].FruitComponent.NumFruits));
                movedPiece = true;
            }
        }
        return movedPiece;
    }
    public Vector2 GetWorldPosition(int x, int y)
    {
        return new Vector2(transform.position.x - xDim / 2.0f + x,
            transform.position.y + yDim / 2.0f - y);
    }

    public GameItem SpawnNewFruit(int x, int y, ItemType type)
    {
        GameObject newFruit = (GameObject)Instantiate(itemPrefabDict[type], GetWorldPosition(x, y), Quaternion.identity);
        newFruit.transform.parent = transform;

        items[x, y] = newFruit.GetComponent<GameItem>();
        items[x, y].InitItem(x, y, this, type);

        return items[x, y];
    }

    public bool IsAdjacent(GameItem item1, GameItem item2)
    {
        return (item1.X == item2.X && (int)Mathf.Abs(item1.Y - item2.Y) == 1)
            || (item1.Y == item2.Y && (int)Mathf.Abs(item1.X - item2.X) == 1);
    }

    public void SwapPieces(GameItem item1, GameItem item2)
    {
        if (gameOver)
        {
            return;
        }


        if (item1.IsMoveable() && item2.IsMoveable())
        {
            items[item1.X, item1.Y] = item2;
            items[item2.X, item2.Y] = item1;

            if (GetMatch(item1, item2.X, item2.Y) != null || GetMatch(item2, item1.X, item1.Y) != null)
            {
                int piece1X = item1.X;
                int piece1Y = item1.Y;

                item1.MovableComponent.Move(item2.X, item2.Y, fillTime);
                item2.MovableComponent.Move(piece1X, piece1Y, fillTime);
                ClearAllValidMatches();

                StartCoroutine(Fill());

                gameManager.OnMove();
            }
            else
            {
                items[item1.X, item1.Y] = item1;
                items[item2.X, item2.Y] = item2;
            }
        }

    }

    public void PressItem(GameItem item)
    {
        pressItem = item;
    }

    public void EnterdItem(GameItem item)
    {
        enteredItem = item;
    }

    public void ReleaseItem()
    {
        if (IsAdjacent(pressItem, enteredItem))
        {
            SwapPieces(pressItem, enteredItem);
        }
    }

    
    public List<GameItem> GetMatch(GameItem item, int newX, int newY)
    {
        if (item.IsFruit())
        {
            FruitItem.FruitType type = item.FruitComponent._FruitType;
            List<GameItem> hozItems = new List<GameItem>();
            List<GameItem> vertItem = new List<GameItem>();
            List<GameItem> matchItem = new List<GameItem>();

            hozItems.Add(item);
            //Horizontal check
            for(int dir = 0; dir <= 1; dir++)
            {
                for(int xOffset = 1; xOffset < xDim; xOffset++)
                {
                    int x;

                    if(dir == 0)
                    {
                        x = newX - xOffset;
                    }
                    else
                    {
                        x = newX  + xOffset;
                    }

                    if(x < 0 || x >= xDim)
                    {
                        break;
                    }

                    if (items[x, newY].IsFruit() && items[x, newY].FruitComponent._FruitType == type)
                    {
                        hozItems.Add(items[x, newY]);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if(hozItems.Count >= 3)
            {
                for(int i = 0; i < hozItems.Count; i++)
                {
                    matchItem.Add(hozItems[i]);
                }
            }

            if (matchItem.Count >= 3)
            {
                return matchItem;
            }

            hozItems.Clear();
            vertItem.Clear();

            vertItem.Add(item);

            //Vertical check
            for (int dir = 0; dir <= 1; dir++)
            {
                for (int yOffset = 1; yOffset < yDim; yOffset++)
                {
                    int y;

                    if (dir == 0) //Up
                    {
                        y = newY - yOffset;
                    }
                    else //Down
                    {
                        y = newY + yOffset;
                    }

                    if (y < 0 || y >= yDim)
                    {
                        break;
                    }

                    if (items[newX, y].IsFruit() && items[newX, y].FruitComponent._FruitType == type)
                    {
                        vertItem.Add(items[newX, y]);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (vertItem.Count >= 3)
            {
                for (int i = 0; i < vertItem.Count; i++)
                {
                    matchItem.Add(vertItem[i]);
                }
            
            }


            if (matchItem.Count >= 3)
            {
                return matchItem;
            }
        }
        return null;
    }

    public bool ClearItem(int x, int y)
    {
        if (items[x,y].IsClearable() && !items[x, y].ClearableComponent.IsCleared)
        {
            items[x,y].ClearableComponent.Clear();
            SpawnNewFruit(x, y, ItemType.EMPTY);
            return true;
        }
        return false;
    }

    public bool ClearAllValidMatches()
    {
        bool needRefild = false;
        for(int y = 0; y < yDim; y++)
        {
            for(int x = 0; x < xDim; x++)
            {
                if (items[x, y].IsClearable())
                {
                    List<GameItem> match = GetMatch(items[x, y], x, y);

                    if(match != null)
                    {
                        for(int i = 0; i < match.Count; i++)
                        {
                            if (ClearItem(match[i].X, match[i].Y))
                            {
                                needRefild = true;
                            }
                        }
                    }
                }
            }
        }
        return needRefild;
    }

    public void GameOver() 
    {
        gameOver = true;
    }



























}

