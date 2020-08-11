using DG.Tweening.Core;
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
    private BoxCollider2D boxCollider;
    SoundManager sm;
    // Start is called before the first frame update

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        sm = SoundManager.Instance;
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
            yield return new WaitForSeconds(0.005f);
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
            yield return new WaitForSeconds(0.005f);
        }
    }

    private void CheckTile()
    {
        if (_renderer.sprite == BoardPresenter.Instance.boardController.board.sprites[5]) return;
        else
        {
            if (selected != null)
            {
                if (selected == this)
                {
                    sm.PlaySound(SoundType.TypeSelect);
                    selected.Unselect();
                    selected = null;
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
    }
    private void Update()
    {
        
        if (Input.touchCount > 0)
        {
            Touch t1 = Input.GetTouch(0);
            Vector3 wp = Camera.main.ScreenToWorldPoint(t1.position); 
            if (t1.phase == TouchPhase.Began && boxCollider.OverlapPoint(wp))
            {
                CheckTile();
            }
            else if(t1.phase == TouchPhase.Moved)
            {
                Vector2 deltaPos = t1.deltaPosition;
                Debug.Log(t1.position);
                if(deltaPos.y > 0)
                {
                    if (deltaPos.x > deltaPos.y) BoardPresenter.Instance.DestroyCombo(selected.position, selected.position + new Vector2Int(1, 0));
                    else
                    {
                        if(deltaPos.x > 0 || deltaPos.x < 0 && -deltaPos.x < deltaPos.y) BoardPresenter.Instance.DestroyCombo(selected.position, selected.position + new Vector2Int(0, 1));
                        else if(deltaPos.x < 0 && -deltaPos.x > deltaPos.y) BoardPresenter.Instance.DestroyCombo(selected.position, selected.position + new Vector2Int(-1, 0));

                    }
                }
                else
                {
                    if (deltaPos.x > -deltaPos.y && deltaPos.x > 0) BoardPresenter.Instance.DestroyCombo(selected.position, selected.position + new Vector2Int(1, 0));
                    else if(deltaPos.x < 0)
                    {
                        if(deltaPos.x < deltaPos.y) BoardPresenter.Instance.DestroyCombo(selected.position, selected.position + new Vector2Int(-1, 0));
                        else BoardPresenter.Instance.DestroyCombo(selected.position, selected.position + new Vector2Int(0, -1));
                    }
                }
                selected = null;
                selected.Unselect();
                sm.PlaySound(SoundType.TypeSelect);
            }
            if (t1.phase == TouchPhase.Stationary) return;
        }
    }
}   
    