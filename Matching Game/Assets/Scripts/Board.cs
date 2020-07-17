using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Text.RegularExpressions;

public class Board : MonoBehaviour
{
    private static int dimension = 10;
    public List<Sprite> sprites = new List<Sprite>();
    public float distance = 1.0f;
    public GameObject tilePrefab;
    private GameObject[,] allTiles;
    private HashSet<SpriteRenderer> matchedTiles;
    private Vector3[,] pos = new Vector3[dimension, dimension];
    private List<Vector2Int> matchedPos = new List<Vector2Int>();

    public bool win;

    [SerializeField] private int startingMoves = 15;
    private int remainingMoves;

    public TextMeshProUGUI movesText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI goalText;
    public TextMeshProUGUI highscoreText;
    private int _score;
    private int _goal;
    private int _highscore;
    private int block;
    private bool possibleToMove = true;


    public int NumMoves
    {
        get
        {
            return remainingMoves;
        }
        set
        {
            remainingMoves = value;
            movesText.text = remainingMoves.ToString();
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
            scoreText.text = _score.ToString();
        }
    }

    public int Goal
    {
        get
        {
            return _goal;
        }
        set
        {
            _goal = value;
            goalText.text = _goal.ToString();
        }
    }

    public int HighScore
    {
        get
        {
            return _highscore;
        }
        set
        {
            _highscore = value;
            highscoreText.text = _highscore.ToString();
        }
    }

    public static Board Instance { get; private set; }


    void Awake()
    {
        Instance = this;
        int level = PlayerPrefs.GetInt("win");
        Score = PlayerPrefs.GetInt("score");
        NumMoves = startingMoves;
        Goal = level * 200;
        HighScore = PlayerPrefs.GetInt("highScore");
    }
    // Start is called before the first frame update
    void Start()
    {
        allTiles = new GameObject[dimension, dimension];
        matchedTiles = new HashSet<SpriteRenderer>();
        matchedPos = new List<Vector2Int>();
        block = 10;
        do
        {
            if (!possibleToMove)
            {
                for (int i = 0; i < dimension; i++)
                {
                    for (int j = 0; j < dimension; j++)
                    {
                        Destroy(allTiles[i, j]);
                    }
                }
            }
            SetUp();
            possibleToMove = CheckOnePossibleMatchAtleast();
            
            Debug.Log(possibleToMove);
        } while (!possibleToMove);
    }

    private void SetUp()
    {
        Vector3 offset = transform.position - new Vector3(dimension * distance / 2.0f, dimension * distance / 2.0f, 0);
        System.Random rand = new System.Random();
        for (int i = 0; i < dimension; i++)
        {
            for (int j = 0; j < dimension; j++)
            {
                GameObject newTile = Instantiate(tilePrefab);
                List<Sprite> possibleSprites = new List<Sprite>(sprites);
                if (block <= 0) possibleSprites.Remove(sprites[5]);
                Sprite left1 = GetSpriteAt(j - 1, i);
                Sprite left2 = GetSpriteAt(j - 2, i);
                if (left1 != null && left1 == left2)
                {
                    possibleSprites.Remove(left1);
                }

                Sprite down1 = GetSpriteAt(j, i - 1);
                Sprite down2 = GetSpriteAt(j, i - 2);
                if (down1 != null && down1 == down2)
                {
                    possibleSprites.Remove(down1);
                }

                SpriteRenderer renderer = newTile.GetComponent<SpriteRenderer>();

                int num = rand.Next(possibleSprites.Count);
                renderer.sprite = possibleSprites[num];
                if (renderer.sprite == sprites[5]) block--;

                Tile tile = newTile.AddComponent<Tile>();
                tile.position = new Vector2Int(j, i);

                newTile.transform.SetParent(transform);
                newTile.transform.position = new Vector3(j * distance, i * distance, 0) + offset;
                pos[j, i] = newTile.transform.position;
                allTiles[j, i] = newTile;
                
            }
        }
    }

    private Sprite GetSpriteAt(int column, int row)
    {
        if (column < 0 || column >= dimension || row < 0 || row >= dimension)
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

    private void SwapTiles(Vector2Int tile1Position, Vector2Int tile2Position)
    {
        GameObject tile1 = allTiles[tile1Position.x, tile1Position.y];
        SpriteRenderer renderer1 = tile1.GetComponent<SpriteRenderer>();

        GameObject tile2 = allTiles[tile2Position.x, tile2Position.y];
        SpriteRenderer renderer2 = tile2.GetComponent<SpriteRenderer>();

        Sprite temp = renderer1.sprite;
        renderer1.sprite = renderer2.sprite;
        renderer2.sprite = temp;
        

        SoundManager.Instance.PlaySound(SoundType.TypeMove);
    }

    private bool SwapAndCheck(Vector2Int tile1Position, Vector2Int tile2Position)
    {
        if (GetSpriteAt(tile1Position.x, tile1Position.y) != sprites[5] && GetSpriteAt(tile2Position.x, tile2Position.y) != sprites[5])
        {
            SwapTiles(tile1Position, tile2Position);
            if (CheckMatch())
            {
                SwapTiles(tile1Position, tile2Position);
                return true;
            }
            SwapTiles(tile1Position, tile2Position);
        }
        return false;
    }

    public void DestroyIfCombo(Vector2Int tile1Position, Vector2Int tile2Position)
    {
        bool changesOccurs = SwapAndCheck(tile1Position, tile2Position);
        if (changesOccurs)
        {
            SwapTiles(tile1Position, tile2Position);
            StartCoroutine(DoSequence(tile1Position, tile2Position));
            NumMoves--;
            
            if (Score >= Goal)
            {
                win = true;
                GameEnding();
            }
            else
            {
                if (NumMoves <= 0)
                {
                    NumMoves = 0;
                    win = false;
                    GameEnding();
                }
            }
        }
        if (!CheckOnePossibleMatchAtleast())
        {
            Debug.Log("Shuffle");
            SceneManager.LoadScene(1);
        }

    }

    public IEnumerator Exploded()
    {
        do
        {
            foreach (SpriteRenderer renderer in matchedTiles)
            {
                renderer.sprite = null;
            }
            Score += matchedTiles.Count;
            FillHoles();
        } while (CheckMatch());
        yield return new WaitForSeconds(0.5f);
    }

    public IEnumerator DoExplode()
    {
        foreach (Vector2Int pos in matchedPos)
        {
            StartCoroutine(allTiles[pos.x, pos.y].GetComponent<Tile>().Explode());
            yield return new WaitForSeconds(0.3f);
        }
    }
    
    public IEnumerator DoSwap(Vector2Int tile1Position, Vector2Int tile2Position)
    {
        StartCoroutine(allTiles[tile1Position.x, tile1Position.y].GetComponent<Tile>().Move(pos[tile2Position.x, tile2Position.y], pos[tile1Position.x, tile1Position.y]));
        StartCoroutine(allTiles[tile2Position.x, tile2Position.y].GetComponent<Tile>().Move(pos[tile1Position.x, tile1Position.y], pos[tile2Position.x, tile2Position.y]));
        yield return null;
    }
    public IEnumerator DoSequence(Vector2Int tile1Position, Vector2Int tile2Position)
    {
        yield return DoSwap(tile1Position, tile2Position);
        yield return DoExplode();
        yield return Exploded();
    }

    bool CheckMatch()
    {
        bool check = false;
        matchedTiles = new HashSet<SpriteRenderer>();
        matchedPos = new List<Vector2Int>();
        for (int column = 0; column < dimension; column++)
        {
            for (int row = 0; row < dimension; row++)
            {
                if (GetSpriteAt(column, row) != sprites[5])
                {
                    SpriteRenderer current = GetSpriteRendererAt(column, row);

                    List<SpriteRenderer> horizontalMatches = FindColumnMatchForTile(column, row, current.sprite);
                    if (horizontalMatches.Count >= 2)
                    {
                        int count = 0;
                        check = true;
                        matchedTiles.UnionWith(horizontalMatches);
                        matchedTiles.Add(current);
                        while(count < horizontalMatches.Count)
                        {
                            matchedPos.Add(new Vector2Int(column+count, row));
                            count++;
                        }
                    }

                    List<SpriteRenderer> verticalMatches = FindRowMatchForTile(column, row, current.sprite);
                    if (verticalMatches.Count >= 2)
                    {
                        int count = 0;
                        check = true;
                        matchedTiles.UnionWith(verticalMatches);
                        matchedTiles.Add(current);
                        while (count < verticalMatches.Count)
                        {
                            matchedPos.Add(new Vector2Int(column, row + count));
                            count++;
                        }
                    }
                }
            }
        }

        return check;
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

    public bool CheckOnePossibleMatchAtleast()
    {
        bool check = false;
        for (int column = 0; column < dimension; column++)
        {
            for (int row = 0; row < dimension; row++)
            {
                if (GetSpriteAt(column, row) != sprites[5])
                {
                    if (column < dimension - 1) check = SwapAndCheck(new Vector2Int(column, row), new Vector2Int(column + 1, row));
                    if (check) return true;
                    if (row < dimension - 1) check = SwapAndCheck(new Vector2Int(column, row), new Vector2Int(column, row + 1));
                    if (check) return true;
                }
            }
        }
        return false;
    }


    private void FillHoles()
    {
        System.Random rand = new System.Random();
        for (int column = 0; column < dimension; column++)
        {
            for (int row = 0; row < dimension; row++)
            {
                while (GetSpriteRendererAt(column, row).sprite == null)
                {
                    SpriteRenderer current = GetSpriteRendererAt(column, row);
                    SpriteRenderer next = current;
                    for (int i = row; i < dimension-1; i++)
                    {
                        int j = i + 1;
                        if (GetSpriteAt(column, i) != sprites[5])
                        {
                            while (GetSpriteAt(column, j) == sprites[5] && j < dimension)
                            {
                                j++;
                            }
                            if (j == dimension) j--;
                            if(GetSpriteAt(column, j) != sprites[5])
                            {
                                StartCoroutine(allTiles[column, i].GetComponent<Tile>().Move(pos[column, j], pos[column, i]));
                                next = GetSpriteRendererAt(column, j);
                                current.sprite = next.sprite;
                                current = next;
                            }
                        }
                    }
                    next.sprite = sprites[rand.Next(sprites.Count - 1)];
                }
            }
        }
    }

    private void Update()
    {
        if (Score > HighScore)
        {
            PlayerPrefs.SetInt("highScore", Score);
            HighScore = Score;
        }
        PlayerPrefs.SetInt("score", Score);
        PlayerPrefs.SetInt("win", PlayerPrefs.GetInt("win"));
        highscoreText.text = HighScore.ToString();
    }
    private void GameEnding()
    {
        if (win)
        {
            Debug.Log("YOU WIN!!!");
            PlayerPrefs.SetInt("win", PlayerPrefs.GetInt("win") + 1);
            PlayerPrefs.SetInt("score", Score);
            SceneManager.LoadScene(2);
        }
        else
        {
            Debug.Log("YOU LOSE!!!!!!!!!!!!!!!!!!!!!!");
            PlayerPrefs.SetInt("win", 1);
            PlayerPrefs.SetInt("score", 0);
            SceneManager.LoadScene(3);
        }

    }
}
