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

    private void Awake()
    {
        Instance = this;
        Score = 0;
        numMoves = startingMoves;
    }
    // Start is called before the first frame update
    void Start()
    {
        allTiles = new GameObject[dimension, dimension];
        SetUp();
    }

    private void SetUp()
    {
        
        Vector3 offset = transform.position - new Vector3(dimension * distance / 2.0f, 0);
        System.Random rand = new System.Random();
        oneMoveAtleast = false;
            for (int i = 0; i < dimension; i++)
            {
                for (int j = 0; j < dimension; j++)
                {
                    GameObject newTile = Instantiate(tilePrefab);
                    List<Sprite> possibleSprites = new List<Sprite>(sprites);
                    
                    Sprite left1 = GetSpriteAt(i - 1, j);
                    Sprite left2 = GetSpriteAt(i - 2, j);
                    if (left1 != null && left1 == left2)
                    {
                        possibleSprites.Remove(left1);
                    }

                    Sprite down1 = GetSpriteAt(i, j - 1);
                    Sprite down2 = GetSpriteAt(i, j - 2);
                    if (down1 != null && down1 == down2)
                    {
                        possibleSprites.Remove(down1);
                    }

                    SpriteRenderer renderer = newTile.GetComponent<SpriteRenderer>();
                    
                    int num = rand.Next(possibleSprites.Count);
                    renderer.sprite = possibleSprites[num];

                    Tile tile = newTile.AddComponent<Tile>();
                    tile.position = new Vector2Int(i, j);

                    newTile.transform.parent = transform;
                    newTile.transform.position = new Vector3(i * distance, j * distance, 0) + offset;
                    allTiles[i, j] = newTile;
                }
            }
        
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

    private List <SpriteRenderer> FindVerticalMatchesForTile(int column, int row, Sprite sprite)
    {
        List<SpriteRenderer> matches = new List<SpriteRenderer>();
        for(int i = row + 1; i < dimension; i++)
        {
            SpriteRenderer nextRow = GetSpriteRendererAt(column, i);
            if(nextRow.sprite != sprite)
            {
                break;
            }
            matches.Add(nextRow);
        }
        return matches;
    }

    private List<SpriteRenderer> FindHorizontalMatchesForTile(int column, int row, Sprite sprite)
    {
        List<SpriteRenderer> matches = new List<SpriteRenderer>();
        for (int i = column + 1; i < dimension; i++)
        {
            SpriteRenderer nextColumn = GetSpriteRendererAt(i, row);
            if (nextColumn.sprite != sprite)
            {
                break;
            }
            matches.Add(nextColumn);
        }
        return matches; 
    }

    private bool CheckMatches()
    {
        HashSet<SpriteRenderer> matchedTiles = new HashSet<SpriteRenderer>();
        for (int i = 0; i < dimension; i++)
        {
            for (int j = 0; j < dimension; j++)
            {
                SpriteRenderer current = GetSpriteRendererAt(i, j);
                List<SpriteRenderer> rowMatches = FindHorizontalMatchesForTile(i, j, current.sprite);
                List<SpriteRenderer> columnMatches = FindVerticalMatchesForTile(i, j, current.sprite);
                if (rowMatches.Count >= 2)
                {
                    matchedTiles.UnionWith(rowMatches);
                    matchedTiles.Add(current);
                }
                if (columnMatches.Count >= 2)
                {
                    matchedTiles.UnionWith(columnMatches);
                    matchedTiles.Add(current);
                }
            }
        }
        foreach (SpriteRenderer renderer in matchedTiles)
        {
            renderer.sprite = null;
        }
        return matchedTiles.Count > 0;
    }

    public void SwapTiles(Vector2Int tiles1Position, Vector2Int tiles2Position)
    {
        GameObject tile1 = allTiles[tiles1Position.x, tiles1Position.y];
        SpriteRenderer renderer1 = tile1.GetComponent<SpriteRenderer>();
        GameObject tile2 = allTiles[tiles2Position.x, tiles2Position.y];
        SpriteRenderer renderer2 = tile2.GetComponent<SpriteRenderer>();
        
        Sprite temp = renderer1.sprite;
        renderer1.sprite = renderer2.sprite;
        renderer2.sprite = temp;

        oneMoveAtleast = CheckMatches();
        Debug.Log(oneMoveAtleast);
        if(!oneMoveAtleast)
        {
            temp = renderer1.sprite;
            renderer1.sprite = renderer2.sprite;
            renderer2.sprite = temp;
        }
        else
        {
            numMoves--;
            do
            {
                FillHoles();
            } while (CheckMatches());
            if(numMoves <= 0)
            {
                numMoves = 0;
                SceneManager.LoadScene(3);
            }
        }
    }

    private bool CheckOneMoveAtleast()
    {
        for (int i = 0; i < dimension; i++)
        {
            for (int j = 0; j < dimension; j++)
            {
                GameObject tile = allTiles[i, j];
                SpriteRenderer tileRenderer = tile.GetComponent<SpriteRenderer>();
                Vector2Int tilePosition = new Vector2Int(i, j);
                if (j < dimension - 1)
                {
                    GameObject nextRowTile = allTiles[i, j + 1];
                    SpriteRenderer nextRowTileRenderer = nextRowTile.GetComponent<SpriteRenderer>();
                    Vector2Int nextRowTilePosition = new Vector2Int(i, j + 1);
                    SwapTiles(tilePosition, nextRowTilePosition);
                    if (oneMoveAtleast)
                    {
                        Sprite temp = tileRenderer.sprite;
                        tileRenderer.sprite = nextRowTileRenderer.sprite;
                        nextRowTileRenderer.sprite = temp;
                        return oneMoveAtleast;
                    }
                }
                if(i < dimension - 1)
                {
                    GameObject nextColumnTile = allTiles[i + 1, j];
                    SpriteRenderer nextColumnTileRenderer = nextColumnTile.GetComponent<SpriteRenderer>();
                    Vector2Int nextColumnTilePosition = new Vector2Int(i + 1, j);
                    SwapTiles(tilePosition, nextColumnTilePosition);
                    if (oneMoveAtleast)
                    {
                        Sprite temp = tileRenderer.sprite;
                        tileRenderer.sprite = nextColumnTileRenderer.sprite;
                        nextColumnTileRenderer.sprite = temp;
                        return oneMoveAtleast;
                    }
                }
            }
        }
        return oneMoveAtleast; 
    }

    void FillHoles()
    {
        for (int i = 0; i < dimension; i++)
        {
            for (int j = 0; j < dimension; j++)
            {
                while (GetSpriteRendererAt(i, j).sprite == null)
                {
                    System.Random rand = new System.Random();
                    for (int k = j; k < dimension - 1; k++)
                    {
                        SpriteRenderer current = GetSpriteRendererAt(i, k);
                        SpriteRenderer next = GetSpriteRendererAt(i, k + 1);
                        current.sprite = next.sprite;
                    }
                    SpriteRenderer last = GetSpriteRendererAt(i, dimension - 1);
                    last.sprite = sprites[rand.Next(sprites.Count)];
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
