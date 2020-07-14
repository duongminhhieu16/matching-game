﻿using UnityEngine;

public class Tile : MonoBehaviour
{
    private static Tile selected;
    private SpriteRenderer _renderer;
    public Vector2Int position;

    // Start is called before the first frame update
    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void Select()
    {
        _renderer.color = Color.grey;
    }

    public void Unselect()
    {
        _renderer.color = Color.white;
    }

    private void OnMouseDown()
    {
        if (selected != null)
        {
            if (selected == this)
            {
                selected.Unselect();
                selected = null;
                return;
            }
            
            if (Vector2Int.Distance(selected.position, position) == 1)
            {
                Board.Instance.DestroyIfCombo(position, selected.position);
                selected.Unselect();
                selected = null;
            }
            else
            {
                selected.Unselect();
                selected = this;
                Select();
            }
        }
        else
        {
            selected = this;
            Select();
        }
    }
}   