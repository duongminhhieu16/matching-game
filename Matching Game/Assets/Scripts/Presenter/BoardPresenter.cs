﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class BoardPresenter : MonoBehaviour
{
    public BoardController boardController;
    public static BoardPresenter Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        boardController.SetUpBoard();
    }
    public void DestroyCombo(Vector2Int tile1Position, Vector2Int tile2Position)
    {
        bool changesOccurs = boardController.CheckIfChanged(tile1Position, tile2Position);
        if (changesOccurs)
        {
            boardController.SwapTiles(tile1Position, tile2Position);
            StartCoroutine(DoSequence(tile1Position, tile2Position));
            int numMoves = PlayerPrefs.GetInt("numMoves") - 1;

            PlayerPrefs.SetInt("numMoves", numMoves);
        }
        if (!boardController.CheckOnePossibleMatchAtleast())
        {
            Debug.Log("Shuffle");
            SceneManager.LoadScene(1);
        }

    }

    private IEnumerator Disappeared()
    {
        foreach (SpriteRenderer renderer in boardController.board.matchedTiles)
        {
            renderer.sprite = null;
        }
        PlayerPrefs.SetInt("score", PlayerPrefs.GetInt("score") + boardController.board.matchedTiles.Count);
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator DoExplode()
    {
        foreach (Vector2Int pos in boardController.board.matchedPos)
        {
            StartCoroutine(boardController.board.allTiles[pos.x, pos.y].GetComponent<TileController>().Explode());
            yield return new WaitForSeconds(0.1f);
        }
        foreach (Vector2Int pos in boardController.board.matchedPos)
        {
            StartCoroutine(boardController.board.allTiles[pos.x, pos.y].GetComponent<TileController>().Exploding());
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator DoSwap(Vector2Int tile1Position, Vector2Int tile2Position)
    {
        StartCoroutine(boardController.board.allTiles[tile1Position.x, tile1Position.y].GetComponent<TileController>().Move(boardController.board.pos[tile2Position.x, tile2Position.y], boardController.board.pos[tile1Position.x, tile1Position.y]));
        StartCoroutine(boardController.board.allTiles[tile2Position.x, tile2Position.y].GetComponent<TileController>().Move(boardController.board.pos[tile1Position.x, tile1Position.y], boardController.board.pos[tile2Position.x, tile2Position.y]));
        yield return new WaitForSeconds(0.1f);
    }
    public IEnumerator DoSequence(Vector2Int tile1Position, Vector2Int tile2Position)
    {
        yield return DoSwap(tile1Position, tile2Position);
        do
        {
            yield return DoExplode();
            yield return Disappeared();
            yield return FillHoles();
        } while (boardController.CheckMatch());
    }
    private IEnumerator FillHoles()
    {
        System.Random rand = new System.Random();
        for (int column = 0; column < boardController.board.dimension; column++)
        {
            for (int row = 0; row < boardController.board.dimension; row++)
            {
                while (boardController.board.GetSpriteRendererAt(column, row).sprite == null)
                {
                    SpriteRenderer current = boardController.board.GetSpriteRendererAt(column, row);
                    SpriteRenderer next = current;
                    for (int i = row; i < boardController.board.dimension - 1; i++)
                    {
                        int j = i + 1;
                        if (boardController.board.GetSpriteAt(column, i) != boardController.board.sprites[5])
                        {
                            while (boardController.board.GetSpriteAt(column, j) == boardController.board.sprites[5] && j < boardController.board.dimension)
                            {
                                j++;
                            }
                            if (j == boardController.board.dimension) j--;
                            if (boardController.board.GetSpriteAt(column, j) != boardController.board.sprites[5])
                            {
                                StartCoroutine(boardController.board.allTiles[column, i].GetComponent<TileController>().Move(boardController.board.pos[column, j], boardController.board.pos[column, i]));
                                next = boardController.board.GetSpriteRendererAt(column, j);
                                current.sprite = next.sprite;
                                current = next;
                            }
                        }
                    }
                    next.sprite = boardController.board.sprites[rand.Next(boardController.board.sprites.Count - 1)];
                }
            }
        }
        yield return null ;
    }
}
