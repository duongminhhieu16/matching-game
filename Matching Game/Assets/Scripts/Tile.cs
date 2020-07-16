using System;
using System.Collections;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private static Tile selected;
    private SpriteRenderer _renderer;
    public Vector2Int position;
    private float speed = 0.2f;
    private bool isRunning;
    // Start is called before the first frame update

    public static Tile Instance{ get; private set; }

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
                SoundManager.Instance.PlaySound(SoundType.TypeSelect);
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
                SoundManager.Instance.PlaySound(SoundType.TypeSelect);
                selected = this;
                Select();
            }
        }
        else
        {
            SoundManager.Instance.PlaySound(SoundType.TypeSelect);
            selected = this;
            Select();
        }
    }

    public IEnumerator FallDown(Vector3 prepos, Vector3 pos)
    {
        float fraction = 0;
        isRunning = true;
        while(isRunning)
        {
            fraction += speed;
            transform.position = Vector3.Lerp(prepos, pos, fraction);
            if (fraction == 1) isRunning = false;
            yield return new WaitForSeconds(0.2f);
        }
        
    }
}   
