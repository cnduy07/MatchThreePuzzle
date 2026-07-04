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
            ApplyBreakableSprite();
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
        breakableValue = Mathf.Max(0, breakableValue - 1);

        yield return new WaitForSeconds(0.25f);

        ApplyBreakableSprite();

        if (breakableValue <= 0)
        {
            tileType = TileType.Normal;
            m_spriteRenderer.color = normalColor;
        }
    }

    void ApplyBreakableSprite()
    {
        if (m_spriteRenderer == null || breakableSprites == null || breakableValue < 0 || breakableValue >= breakableSprites.Length)
        {
            return;
        }

        Sprite sprite = breakableSprites[breakableValue];
        if (sprite != null)
        {
            m_spriteRenderer.sprite = sprite;
        }
    }
}
