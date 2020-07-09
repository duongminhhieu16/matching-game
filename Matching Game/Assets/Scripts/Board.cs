using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int dimension = 10;
    public List<Sprite> sprites = new List<Sprite>();
    public float distance = 1.0f;
    public GameObject tilePrefab;
    private GameObject[,] allTiles;
 
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
        for (int i = 0; i < dimension; i++)
        {
            for (int j = 0; j < dimension; j++)
            {
                GameObject newTile = Instantiate(tilePrefab);
                SpriteRenderer renderer = newTile.GetComponent<SpriteRenderer>();
                int num = rand.Next(5);
                renderer.sprite = sprites[num];
                newTile.transform.parent = transform;
                newTile.transform.position = new Vector3(i * distance, j * distance, 0) + offset;
                allTiles[i, j] = newTile;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
