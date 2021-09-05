using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour
{
    public enum TileType
    {
        Normal,
        Obstacle,
        Breakable
    }

    public int xIndex;
    public int yIndex;
    public TileType tileType = TileType.Normal;
    public int breakableValue;
    public Sprite[] breakableSprites;
    public Color normalColor;

    Board m_Board;
    SpriteRenderer m_spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(int x, int y, Board board)
    {
        xIndex = x;
        yIndex = y;
        m_Board = board;

        if (tileType == TileType.Breakable)
        {
            if (breakableSprites[breakableValue] != null)
            {
                m_spriteRenderer.sprite = breakableSprites[breakableValue];
            }
        }
    }

    private void OnMouseEnter()
    {
        if (m_Board != null)
        {
            m_Board.DragToTile(this);
        }
    }

    private void OnMouseDown()
    {
        if (m_Board != null)
        {
            m_Board.ClickedTile(this);
        }
    }

    private void OnMouseUp()
    {
        if (m_Board != null)
        {
            m_Board.ReleaseTile();
        }
        
    }

    public void BreakTile()
    {
        if (tileType != TileType.Breakable)
        {
            return;
        }

        StartCoroutine(BreakTileRoutine());
    }

    IEnumerator BreakTileRoutine()
    {
        breakableValue = Mathf.Clamp(--breakableValue, 0, breakableValue);

        yield return new WaitForSeconds(0.25f);

        if (breakableSprites[breakableValue] != null)
        {
            m_spriteRenderer.sprite = breakableSprites[breakableValue];
        }

        if (breakableValue <= 0)
        {
            tileType = TileType.Normal;
            //m_spriteRenderer.color = normalColor;
        }
    }
}
