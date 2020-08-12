using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public BoardData board;
    public static BoardController Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    
    public void SetUpBoard()
    {
        bool possibleMove = true;
        board.allTiles = new GameObject[board.dimension, board.dimension];
        board.matchedTiles = new HashSet<SpriteRenderer>();
        board.matchedPos = new HashSet<Vector2Int>();
        board.BLOCK += ScoreData.level;
        do
        {
            if (!possibleMove)
            {
                for (int col = 0; col < board.dimension; col++)
                {
                    for (int row = 0; row < board.dimension; row++)
                    {
                        Destroy(board.allTiles[col, row]);
                    }
                }
            }
            CreateBoard();
            possibleMove = CheckOnePossibleMatchAtleast();
        } while (!possibleMove);
    }
    private void CreateBoard()
    {
        Vector3 offset = transform.position - new Vector3(board.dimension * board.distance / 2.0f - 1f, board.dimension * board.distance / 2.0f + 5f, 0);
        System.Random rand = new System.Random();
        for (int i = 0; i < board.dimension; i++)
        {
            for (int j = 0; j < board.dimension; j++)
            {
                GameObject newTile = Instantiate(board.tilePrefab);
                List<Sprite> possibleSprites = new List<Sprite>(board.sprites);
                if (board.BLOCK <= 0) possibleSprites.Remove(board.sprites[5]);
                Sprite left1 = board.GetSpriteAt(j - 1, i);
                Sprite left2 = board.GetSpriteAt(j - 2, i);
                if (left1 != null && left1 == left2)
                {
                    possibleSprites.Remove(left1);
                }

                Sprite down1 = board.GetSpriteAt(j, i - 1);
                Sprite down2 = board.GetSpriteAt(j, i - 2);
                if (down1 != null && down1 == down2)
                {
                    possibleSprites.Remove(down1);
                }

                SpriteRenderer renderer = newTile.GetComponent<SpriteRenderer>();

                int num = rand.Next(possibleSprites.Count);
                renderer.sprite = possibleSprites[num];
                if (renderer.sprite == board.sprites[5]) board.BLOCK--;

                TileController tile = newTile.AddComponent<TileController>();
                tile.SetPosition(new Vector2Int(j, i));

                newTile.transform.SetParent(transform);
                newTile.transform.position = new Vector3(j * board.distance, i * board.distance, 0) + offset;
                board.pos[j, i] = newTile.transform.position;
                board.allTiles[j, i] = newTile;
            }
        }
    }
    public bool CheckOnePossibleMatchAtleast()
    {
        bool check = false;
        for (int column = 0; column < board.dimension; column++)
        {
            for (int row = 0; row < board.dimension; row++)
            {
                if (board.GetSpriteAt(column, row) != board.sprites[5])
                {
                    if (column < board.dimension - 1) check = CheckIfChanged(new Vector2Int(column, row), new Vector2Int(column + 1, row));
                    if (check) return true;
                    if (row < board.dimension - 1) check = CheckIfChanged(new Vector2Int(column, row), new Vector2Int(column, row + 1));
                    if (check) return true;
                }
            }
        }
        return false;
    }
    public bool CheckIfChanged(Vector2Int tile1Position, Vector2Int tile2Position)
    {
        if (board.GetSpriteAt(tile1Position.x, tile1Position.y) != board.sprites[5] && board.GetSpriteAt(tile2Position.x, tile2Position.y) != board.sprites[5])
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
    public void SwapTiles(Vector2Int tile1Position, Vector2Int tile2Position)
    {
        GameObject tile1 = board.allTiles[tile1Position.x, tile1Position.y];
        SpriteRenderer renderer1 = tile1.GetComponent<SpriteRenderer>();

        GameObject tile2 = board.allTiles[tile2Position.x, tile2Position.y];
        SpriteRenderer renderer2 = tile2.GetComponent<SpriteRenderer>();

        Sprite temp = renderer1.sprite;
        renderer1.sprite = renderer2.sprite;
        renderer2.sprite = temp;


        SoundManager.Instance.PlaySound(SoundType.TypeMove);
    }
    public bool CheckMatch()
    {
        bool check = false;
        board.matchedTiles = new HashSet<SpriteRenderer>();
        board.matchedPos = new HashSet<Vector2Int>();
        for (int column = 0; column < board.dimension; column++)
        {
            for (int row = 0; row < board.dimension; row++)
            {
                if (board.GetSpriteAt(column, row) != board.sprites[5])
                {
                    SpriteRenderer current = board.GetSpriteRendererAt(column, row);
                    List<SpriteRenderer> horizontalMatches = FindColumnMatchForTile(column, row, current.sprite);
                    if (horizontalMatches.Count >= 2)
                    {
                        int count = 0;
                        check = true;
                        board.matchedTiles.UnionWith(horizontalMatches);
                        board.matchedTiles.Add(current);
                        while (count <= horizontalMatches.Count)
                        {
                            board.matchedPos.Add(new Vector2Int(column + count, row));
                            count++;
                        }
                    }
                    List<SpriteRenderer> verticalMatches = FindRowMatchForTile(column, row, current.sprite);
                    if (verticalMatches.Count >= 2)
                    {
                        int count = 0;
                        check = true;
                        board.matchedTiles.UnionWith(verticalMatches);
                        board.matchedTiles.Add(current);
                        while (count <= verticalMatches.Count)
                        {
                            board.matchedPos.Add(new Vector2Int(column, row + count));
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
        for (int i = col + 1; i < board.dimension; i++)
        {
            SpriteRenderer nextColumn = board.GetSpriteRendererAt(i, row);
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
        for (int i = row + 1; i < board.dimension; i++)
        {
            SpriteRenderer nextRow = board.GetSpriteRendererAt(col, i);
            if (nextRow.sprite != sprite)
            {
                break;
            }
            result.Add(nextRow);
        }
        return result;
    }

}
