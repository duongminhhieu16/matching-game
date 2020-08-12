using System.Collections;
using UnityEngine;
public class TileController : MonoBehaviour
{
    
    public static TileController selected;
    private SpriteRenderer _renderer;
    private Vector2Int position;
    private float speed = 0.2f;
    private bool isRunning;
    private bool isExploding;
    private Vector3 originalPos = new Vector3();
    private Vector3 changedPos = new Vector3();
    private BoxCollider2D boxCollider;
    Vector2 deltaTouchPos = new Vector2(0, 0);
    Vector2 currentTouchPos = new Vector2();
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

    /*private void OnMouseDown()
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
                if (Vector2.Distance(position, selected.position) == 1)
                {
                    StartCoroutine(BoardPresenter.Instance.DestroyCombo(selected.position, position));
                    selected.Unselect();
                    selected = null;
                    sm.PlaySound(SoundType.TypeSelect);
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
    }*/
    private void Update()
    {
        
        if (Input.touchCount > 0)
        {
            Touch t1 = Input.GetTouch(0);
            
            Vector3 wp = Camera.main.ScreenToWorldPoint(t1.position); 
            if (t1.phase == TouchPhase.Began && boxCollider.OverlapPoint(wp))
            {
                if (_renderer.sprite == BoardController.Instance.board.sprites[5]) return;
                Select();
                selected = this;
                currentTouchPos = t1.position;
            }
            else if(t1.phase == TouchPhase.Moved)
            {
                deltaTouchPos = t1.position - currentTouchPos;
                currentTouchPos = t1.position;
            }
            else if(t1.phase == TouchPhase.Ended)
            {
                if (deltaTouchPos.y > 0)
                {
                    if (deltaTouchPos.x > deltaTouchPos.y) StartCoroutine(BoardPresenter.Instance.DestroyCombo(selected.position, selected.position + new Vector2Int(1, 0)));
                    else
                    {
                        if (deltaTouchPos.x > 0 || deltaTouchPos.x <= 0 && -deltaTouchPos.x < deltaTouchPos.y) StartCoroutine(BoardPresenter.Instance.DestroyCombo(selected.position, selected.position + new Vector2Int(0, 1)));
                        else if (deltaTouchPos.x <= 0 && -deltaTouchPos.x > deltaTouchPos.y) StartCoroutine(BoardPresenter.Instance.DestroyCombo(selected.position, selected.position + new Vector2Int(-1, 0)));
                    }
                }
                else
                {
                    if (deltaTouchPos.x > -deltaTouchPos.y && deltaTouchPos.x >= 0) StartCoroutine(BoardPresenter.Instance.DestroyCombo(selected.position, selected.position + new Vector2Int(1, 0)));
                    else if (deltaTouchPos.x < 0)
                    {
                        if (deltaTouchPos.x < deltaTouchPos.y) StartCoroutine(BoardPresenter.Instance.DestroyCombo(selected.position, selected.position + new Vector2Int(-1, 0)));
                        else StartCoroutine(BoardPresenter.Instance.DestroyCombo(selected.position, selected.position + new Vector2Int(0, -1)));
                    }
                    else if(deltaTouchPos.x >= 0 && deltaTouchPos.x < - deltaTouchPos.y) StartCoroutine(BoardPresenter.Instance.DestroyCombo(selected.position, selected.position + new Vector2Int(0, -1)));
                }
                
                selected.Unselect();
                selected = null;
                sm.PlaySound(SoundType.TypeSelect);
            }
            else if (t1.phase == TouchPhase.Stationary) return;
        }
    }
}   
    