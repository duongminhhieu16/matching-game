using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardData : MonoBehaviour
{
    public int dimension = 10;
    public List<Sprite> sprites = new List<Sprite>();
    public float distance = 1.0f;
    public GameObject tilePrefab;
    public GameObject[,] allTiles;
    public HashSet<SpriteRenderer> matchedTiles;
    public HashSet<Vector2Int> matchedPos;
    public Vector3[,] pos = new Vector3[10, 10];
    public int BLOCK = 10;
    private BoardData savedBoard;

    public Sprite GetSpriteAt(int column, int row)
    {
        if (column < 0 || column >= dimension || row < 0 || row >= dimension)
        {
            return null;
        }
        GameObject tile = allTiles[column, row];
        SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();
        return renderer.sprite;
    }

    public SpriteRenderer GetSpriteRendererAt(int column, int row)
    {
        if (column < 0 || column >= dimension || row < 0 || row >= dimension)
        {
            return null;
        }
        GameObject tile = allTiles[column, row];
        SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();
        return renderer;
    }
    public void SaveBoard(BoardData board)
    {
        savedBoard = board;
    }
}
