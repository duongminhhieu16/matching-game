using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Board : MonoBehaviour
{
    public int dimension = 10;
    public List<Sprite> sprites = new List<Sprite>();
    public float distance = 1.0f;
    public GameObject tilePrefab;
    private bool oneMoveAtleast = false;
    private GameObject[,] allTiles;
    private HashSet<SpriteRenderer> matchedTiles;

    public int startingMoves;
    private int _numMoves;

    public TextMeshProUGUI movesText;
    public TextMeshProUGUI scoreText;
    private int _score;

    public int numMoves
    {
        get
        {
            return _numMoves;
        }
        set
        {
            _numMoves = value;
           // movesText.text = _numMoves.ToString();
        }
    }

    public int Score
    {
        get
        {
            return _score;
        }
        set
        {
            _score = value;
            //scoreText.text = _score.ToString();
        }
    }

    public static Board Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        Score = 0;
        numMoves = startingMoves;
    }
    // Start is called before the first frame update
    void Start()
    {
        allTiles = new GameObject[dimension, dimension];
        oneMoveAtleast = false;
        SetUp();
    }

    private void SetUp()
    {
        Vector3 offset = transform.position - new Vector3(dimension * distance / 2.0f, dimension * distance / 2.0f, 0);
        System.Random rand = new System.Random();
        do {
            for (int i = 0; i < dimension; i++)
            {
                for (int j = 0; j < dimension; j++)
                {
                    GameObject newTile = Instantiate(tilePrefab);
                    List<Sprite> possibleSprites = new List<Sprite>(sprites);

                    Sprite left1 = GetSpriteAt(j-1, i);
                    Sprite left2 = GetSpriteAt(j-2, i);
                    if (left1 != null && left1 == left2)
                    {
                        possibleSprites.Remove(left1);
                    }

                    Sprite down1 = GetSpriteAt(j, i-1);
                    Sprite down2 = GetSpriteAt(j, i-2);
                    if (down1 != null && down1 == down2)
                    {
                        possibleSprites.Remove(down1);
                    }

                    SpriteRenderer renderer = newTile.GetComponent<SpriteRenderer>();

                    int num = rand.Next(possibleSprites.Count);
                    renderer.sprite = possibleSprites[num];

                    Tile tile = newTile.AddComponent<Tile>();
                    tile.position = new Vector2Int(j, i);

                    newTile.transform.parent = transform;
                    newTile.transform.position = new Vector3(j * distance, i * distance, 0) + offset;
                    allTiles[j, i] = newTile;
                }
            }
        } while (!CheckOneMoveAtleast()) ;
    }

    private Sprite GetSpriteAt(int column, int row)
    {
        if(column < 0 || column >= dimension || row < 0 || row >= dimension)
        {
            return null;
        }
        GameObject tile = allTiles[column, row];
        SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();
        return renderer.sprite;
    }

    private SpriteRenderer GetSpriteRendererAt(int column, int row)
    {
        if (column < 0 || column >= dimension || row < 0 || row >= dimension)
        {
            return null;
        }
        GameObject tile = allTiles[column, row];
        SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();
        return renderer;
    }

    public void SwapTiles(Vector2Int tile1Position, Vector2Int tile2Position)
    {
        GameObject tile1 = allTiles[tile1Position.x, tile1Position.y];
        SpriteRenderer renderer1 = tile1.GetComponent<SpriteRenderer>();

        GameObject tile2 = allTiles[tile2Position.x, tile2Position.y];
        SpriteRenderer renderer2 = tile2.GetComponent<SpriteRenderer>();

        Sprite temp = renderer1.sprite;
        renderer1.sprite = renderer2.sprite;
        renderer2.sprite = temp;

        bool changesOccurs = CheckMatchesAllPosition();
        if (!changesOccurs)
        {
            temp = renderer1.sprite;
            renderer1.sprite = renderer2.sprite;
            renderer2.sprite = temp;
        }
        else
        {
            
            do
            {
                foreach (SpriteRenderer renderer in matchedTiles)
                {
                    renderer.sprite = null;
                }
                FillHoles();
            } while (CheckMatchesAllPosition());
            
        }
    }

    bool CheckMatchesAllPosition()
    {
        matchedTiles = new HashSet<SpriteRenderer>();
        for (int row = 0; row < dimension; row++)
        {
            for (int column = 0; column < dimension; column++)
            {
                SpriteRenderer current = GetSpriteRendererAt(column, row);

                List<SpriteRenderer> horizontalMatches = FindColumnMatchForTile(column, row, current.sprite);
                if (horizontalMatches.Count >= 2)
                {
                    matchedTiles.UnionWith(horizontalMatches);
                    matchedTiles.Add(current);
                }

                List<SpriteRenderer> verticalMatches = FindRowMatchForTile(column, row, current.sprite);
                if (verticalMatches.Count >= 2)
                {
                    matchedTiles.UnionWith(verticalMatches);
                    matchedTiles.Add(current);
                }
            }
        }
        Score += matchedTiles.Count;
        return matchedTiles.Count > 0;
    }

    List<SpriteRenderer> FindColumnMatchForTile(int col, int row, Sprite sprite)
    {
        List<SpriteRenderer> result = new List<SpriteRenderer>();
        for (int i = col + 1; i < dimension; i++)
        {
            SpriteRenderer nextColumn = GetSpriteRendererAt(i, row);
            if (nextColumn.sprite != sprite)
            {
                break;
            }
            result.Add(nextColumn);
        }
        return result;
    }

    List<SpriteRenderer> FindRowMatchForTile(int col, int row, Sprite sprite)
    {
        List<SpriteRenderer> result = new List<SpriteRenderer>();
        for (int i = row + 1; i < dimension; i++)
        {
            SpriteRenderer nextRow = GetSpriteRendererAt(col, i);
            if (nextRow.sprite != sprite)
            {
                break;
            }
            result.Add(nextRow);
        }
        return result;
    }

    private bool CheckOneMoveAtleast()
    {
        for (int i = 0; i < dimension; i++)
        {
            for (int j = 0; j < dimension; j++)
            {
                GameObject tile1 = allTiles[i, j];
                SpriteRenderer renderer1 = tile1.GetComponent<SpriteRenderer>();
                GameObject tileNextRow = allTiles[i + 1, j];
                SpriteRenderer rendererNextRow = tileNextRow.GetComponent<SpriteRenderer>();
                GameObject tileNextColumn = allTiles[i, j + 1];
                SpriteRenderer rendererNextColumn = tileNextColumn.GetComponent<SpriteRenderer>();

                Sprite temp = renderer1.sprite;
                renderer1.sprite = rendererNextRow.sprite;
                rendererNextRow.sprite = temp;

                bool match = CheckMatchesAllPosition();
                if (match) oneMoveAtleast = match;

                temp = renderer1.sprite;
                renderer1.sprite = rendererNextRow.sprite;
                rendererNextRow.sprite = temp;
                if (oneMoveAtleast) return oneMoveAtleast;

                temp = renderer1.sprite;
                renderer1.sprite = rendererNextColumn.sprite;
                rendererNextColumn.sprite = temp;

                match = CheckMatchesAllPosition();
                if (match) oneMoveAtleast = match;

                temp = renderer1.sprite;
                renderer1.sprite = rendererNextColumn.sprite;
                rendererNextColumn.sprite = temp;
                if (oneMoveAtleast) return oneMoveAtleast;
            }
        }
        return oneMoveAtleast; 
    }

    void FillHoles()
    {
        for (int column = 0; column < dimension; column++)
        {
            for (int row = 0; row < dimension; row++)
            {
                System.Random rand = new System.Random();
                while (GetSpriteRendererAt(column, row).sprite == null)
                {
                    SpriteRenderer current = GetSpriteRendererAt(column, row);
                    SpriteRenderer next = current;
                    for (int i = row; i < dimension - 1; i++)
                    {
                        next = GetSpriteRendererAt(column, i + 1);
                        current.sprite = next.sprite;
                        current = next;
                    }
                    next.sprite = sprites[rand.Next(sprites.Count)];
                }
            }
        }    
    }
}
