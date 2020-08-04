 using System.Collections;
using UnityEngine;

public class TileController : MonoBehaviour
{
    private static TileController selected;
    private SpriteRenderer _renderer;
    private Vector2Int position;
    private float speed = 0.2f;
    private bool isRunning;
    private bool isExploding;
    private Vector3 originalPos = new Vector3();
    private Vector3 changedPos = new Vector3();
    // Start is called before the first frame update

    private void Start()
    {
        originalPos = transform.localScale;
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
    public void SetPosition(Vector2Int position)
    {
        this.position = position;
    }
    public Vector2Int GetPosition()
    {
        return position;
    }
    private void OnMouseDown()
    {
        
        SoundManager sm = SoundManager.Instance;
        if (_renderer.sprite == BoardPresenter.Instance.boardController.board.sprites[5]) return;
        if (selected != null)
        {

            if (selected == this)
            {
                sm.PlaySound(SoundType.TypeSelect);
                selected.Unselect();
                selected = null; Debug.Log("position " + position);
                return;
            }
            
            if (Vector2Int.Distance(selected.position, position) == 1)
            {
                BoardPresenter.Instance.DestroyCombo(position, selected.position);
                selected.Unselect();
                selected = null;
                
            }
            else
            {
                selected.Unselect();
                sm.PlaySound(SoundType.TypeSelect);
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

    public IEnumerator Move(Vector3 prepos, Vector3 pos)
    {
        float fraction = 0;
        isRunning = true;
        while(isRunning)
        {
            fraction += speed;
            transform.position = Vector3.Lerp(prepos, pos, fraction);
            if (fraction == 1) isRunning = false;
            yield return new WaitForSeconds(0.01f);
        }
    }
    public IEnumerator Explode()
    {
        float fraction = 0;
        changedPos = originalPos * 2;
        isExploding = true;
        while (isExploding)
        {
            fraction += speed;
            transform.localScale = Vector3.Lerp(originalPos, changedPos, fraction);
            if (fraction >= 1)
            {
                isExploding = false;
            }
            yield return new WaitForSeconds(0.01f);
        }
    }
    public IEnumerator Exploding()
    {
        float fraction = 0;
        isExploding = true;
        while (isExploding)
        {
            fraction += speed;
            transform.localScale = Vector3.Lerp(changedPos, originalPos, fraction);
            if (fraction >= 1)
            {
                isExploding = false;
            }
            yield return new WaitForSeconds(0.01f);
        }
    }
    /*private void Update()
    {
        if(Input.touchCount > 0)
        {
            Touch t1 = Input.GetTouch(0);
        }
    }*/
}   
