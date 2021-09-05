using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public int borderSize;
    
    public GameObject normalTilePrefab;
    public GameObject obstacleTilePrefab;
    public GameObject[] gamePiecePrefabs;
    public StartingTile[] startingTiles;

    float moveTime = 0.5f;
    bool canSwitchGamePiece = true;

    Tile m_clickedTile;
    Tile m_targetTile;

    Tile[,] m_allTiles;
    GamePiece[,] m_allGamePiece;

    [System.Serializable]
    public class StartingTile
    {
        public GameObject prefab;
        public int x;
        public int y;
        public int z;
    }

    void Start()
    {
        m_allTiles = new Tile[width, height];
        m_allGamePiece = new GamePiece[width, height];

        SettupTile();
        SettupCamera();
        FillBoard(10, 0.5f);
    }

    void SettupTile()
    {
        Debug.Log("startingTiles: "+startingTiles.Length);
        foreach(StartingTile startingTile in startingTiles)
        {
            //if (startingTile != null)
            //{
                MakeNewTile(startingTile.prefab, startingTile.x, startingTile.y);
            //}
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                MakeNewTile(normalTilePrefab, i, j);
            }
        }
    }

    private void MakeNewTile(GameObject prefab, int x, int y)
    {
        if (prefab != null)
        {
            if (m_allTiles[x, y] == null)
            {
                GameObject tile = Instantiate(prefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;

                tile.name = "Tile (" + x + " , " + y + ")";

                m_allTiles[x, y] = tile.GetComponent<Tile>();
                m_allTiles[x, y].Init(x, y, this);

                tile.transform.parent = transform;
            }
        }
    }

    /*
     * The orthographicSize is half the size of the vertical viewing volume. 
     * The horizontal size of the viewing volume depends on the aspect ratio.
     */

    void SettupCamera()
    {
        Camera.main.transform.position = new Vector3( ((float)width - 1) / 2f, ((float)height -1) / 2f, -10);

        float aspectRatio = (float)Screen.width / (float)Screen.height;
        float verticalSize = (float)height / 2 + (float)borderSize;
        float horizontalSize = ((float)width / 2 + (float)borderSize) / aspectRatio;

        Camera.main.orthographicSize = (horizontalSize > verticalSize) ? horizontalSize : verticalSize;
    }

    GameObject GetRandomGamePiece()
    {
        int randomIdx = Random.Range(0, gamePiecePrefabs.Length);
        if (gamePiecePrefabs[randomIdx] == null)
        {
            Debug.LogWarning("BOARD: Invalid gamepiece" + randomIdx + "does not contain valid GamePirce prefab!");
        }

        return gamePiecePrefabs[randomIdx];
    }

    public void PlaceGamePiece(GamePiece gamePiece, int x, int y)
    {
        if (gamePiece == null)
        {
            Debug.LogWarning("BOARD: GamePiece Invalid");
            return;
        }

        gamePiece.transform.position = new Vector3(x, y, 0);
        gamePiece.transform.rotation = Quaternion.identity;
        
        if (IsWithinBounds(x, y)) {
            m_allGamePiece[x, y] = gamePiece;
        }

        gamePiece.SetCoord(x, y);
    }

    void FillBoard(int yOffset, float moveTime)
    {
        int maxIterations = 100;
        int iteration = 0;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (m_allGamePiece[i, j] == null && m_allTiles[i, j].tileType != Tile.TileType.Obstacle)
                {
                    GamePiece gamePiece = FillRandomPieceAt(i, j, yOffset, moveTime);
                    iteration = 0;

                    while (HasMatchesOnFill(i, j))
                    {
                        ClearPieceAt(i, j);
                        gamePiece = FillRandomPieceAt(i, j, yOffset, moveTime);
                        iteration++;

                        if (iteration >= maxIterations)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }

    bool HasMatchesOnFill(int x, int y)
    {
        List<GamePiece> leftMatches = FindMatchePieces(x, y, new Vector2(-1, 0));
        List<GamePiece> downMatches = FindMatchePieces(x, y, new Vector2(0, -1));

        if (leftMatches == null)
        {
            leftMatches = new List<GamePiece>();
        }

        if (downMatches == null)
        {
            downMatches = new List<GamePiece>();
        }

        return (leftMatches.Count > 0 || downMatches.Count > 0);
    }

    private GamePiece FillRandomPieceAt(int i, int j,int yOffset, float moveTime)
    {
        GameObject randomPiece = Instantiate(GetRandomGamePiece(), Vector3.zero, Quaternion.identity) as GameObject;

        if (randomPiece != null)
        {
            randomPiece.GetComponent<GamePiece>().Init(this);
            randomPiece.transform.parent = transform;

            PlaceGamePiece(randomPiece.GetComponent<GamePiece>(), i, j);

            if (yOffset != 0)
            {
                randomPiece.transform.position = new Vector3(i, j + yOffset, 0);
                randomPiece.GetComponent<GamePiece>().Move(i, j, moveTime);
            }

            //randomPiece.transform.parent = transform;
            return randomPiece.GetComponent<GamePiece>();
        }

        return null;
    }

    bool IsWithinBounds(int x, int y)
    {
        return (x >= 0 && x < width && y >= 0 && y < height);
    }

    bool IsNextTo(Tile start, Tile end)
    {
        if (Mathf.Abs(start.xIndex - end.xIndex) == 1 && start.yIndex == end.yIndex)
        {
            return true;
        }

        if (Mathf.Abs(start.yIndex - end.yIndex) == 1 && start.xIndex == end.xIndex)
        {
            return true;
        }

        return false;
    }

    public void ClickedTile(Tile tile)
    {
        if (m_clickedTile == null) {
            m_clickedTile = tile;
        }
    }

    public void DragToTile(Tile tile)
    {
        if (m_clickedTile != null && IsNextTo(tile, m_clickedTile))
        {
            m_targetTile = tile;
        }
    }

    public void ReleaseTile()
    {
        if (m_clickedTile != null && m_targetTile != null)
        {
            SwitchTiles(m_clickedTile, m_targetTile);
        }

        m_clickedTile = null;
        m_targetTile = null;
    }

    void SwitchTiles(Tile clickedTile, Tile targetTile)
    {
        if (canSwitchGamePiece)
        {
            StartCoroutine(SwitchTilesRoutine(clickedTile, targetTile));
        }
    }

    IEnumerator SwitchTilesRoutine(Tile clickedTile, Tile targetTile)
    {
        GamePiece clickedPiece = m_allGamePiece[clickedTile.xIndex, clickedTile.yIndex];
        GamePiece targetPiece = m_allGamePiece[targetTile.xIndex, targetTile.yIndex];

        if (clickedPiece != null && targetPiece != null)
        {
            clickedPiece.Move(targetTile.xIndex, targetTile.yIndex, moveTime);
            targetPiece.Move(clickedTile.xIndex, clickedTile.yIndex, moveTime);

            yield return new WaitForSeconds(moveTime);

            List<GamePiece> clickedPieceMatches = FindMatchesAt(clickedTile.xIndex, clickedTile.yIndex);
            List<GamePiece> targetPieceMatches = FindMatchesAt(targetTile.xIndex, targetTile.yIndex);

            if (clickedPieceMatches.Count <= 0 && targetPieceMatches.Count <= 0)
            {
                clickedPiece.Move(clickedTile.xIndex, clickedTile.yIndex, moveTime);
                targetPiece.Move(targetTile.xIndex, targetTile.yIndex, moveTime);
            } else
            {
                yield return new WaitForSeconds(moveTime);
                ClearAndRefillBoard(clickedPieceMatches.Union(targetPieceMatches).ToList());
            }
        }
    }

    /*
     * Vector2 searchDirection: 
     * -> Right: (x,y) = (1,0)
     * -> Left: (x,y) = (-1,0)
     * -> Up: (x,y) = (0,1)
     * -> Down: (x,y) = (0,-1)
     */
    List<GamePiece> FindMatchePieces(int startX, int startY, Vector2 searchDirection, int minLenght = 3)
    {
        List<GamePiece> matches = new List<GamePiece>();
        GamePiece startPiece = null;
        GamePiece nextPiece = null;

        if (IsWithinBounds(startX, startY))
        {
            startPiece = m_allGamePiece[startX, startY];
        }

        if (startPiece != null)
        {
            matches.Add(startPiece);
        } else
        {
            return null;
        }

        int nextX;
        int nextY;

        int maxValue = (width > height) ? width : height;

        for (int i = 1; i < maxValue-1; i++)
        {
            nextX = (int)(startX + Mathf.Clamp(searchDirection.x, -1, 1) * i);
            nextY = (int)(startY + Mathf.Clamp(searchDirection.y, -1, 1) * i);

            if (!IsWithinBounds(nextX, nextY))
            {
                break;
            }

            nextPiece = m_allGamePiece[nextX, nextY];

            if (nextPiece != null && nextPiece.matchValue == startPiece.matchValue && !matches.Contains(nextPiece))
            {
                matches.Add(nextPiece);
            }
            else
            {
                break;
            }
        }

        if (matches.Count >= minLenght)
        {
            return matches;
        }

        return null;
    }

    List<GamePiece> FindVerticalMatchPiece(int startX, int startY, int minLengt = 3 )
    {
        List<GamePiece> upMatches = FindMatchePieces(startX, startY, new Vector2(0, 1), 2);
        List<GamePiece> downMatches = FindMatchePieces(startX, startY, new Vector2(0, -1), 2);

        if (upMatches == null)
        {
            upMatches = new List<GamePiece>();
        }

        if (downMatches == null)
        {
            downMatches = new List<GamePiece>();
        }

        var combinedMatches = upMatches.Union(downMatches).ToList();

        return (combinedMatches.Count >= minLengt) ? combinedMatches : null;
    }

    List<GamePiece> FindHorizontalMatchPiece(int startX, int startY, int minLengt = 3)
    {
        List<GamePiece> leftMatches = FindMatchePieces(startX, startY, new Vector2(-1, 0), 2);
        List<GamePiece> rightMatches = FindMatchePieces(startX, startY, new Vector2(1, 0), 2);

        if (leftMatches == null)
        {
            leftMatches = new List<GamePiece>();
        }

        if (rightMatches == null)
        {
            rightMatches = new List<GamePiece>();
        }

        var combined = leftMatches.Union(rightMatches).ToList();

        return (combined.Count >= minLengt) ? combined : null;
    }

    void HighlightTileOff(int x, int y)
    {
        if (m_allTiles[x, y].tileType != Tile.TileType.Breakable)
        {
            SpriteRenderer spriteRenderer = m_allTiles[x, y].GetComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);
        }
    }

    void HighlightTileOn(int x, int y, Color color)
    {
        if (m_allTiles[x, y].tileType != Tile.TileType.Breakable)
        {
            SpriteRenderer spriteRenderer = m_allTiles[x, y].GetComponent<SpriteRenderer>();
            spriteRenderer.color = color;
        }
    }

    void HighlightMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j< height; j++)
            {
                HighlightMatchesAt(i, j);
            }
        }
    }

    void BreakTileAt(int x, int y)
    {
        Tile tile = m_allTiles[x, y];
        if (tile != null)
        {
            tile.BreakTile();
        }
    }

    void BreakTileAt(List<GamePiece> gamePieces)
    {
        foreach(GamePiece piece in gamePieces) 
        {
            if (piece != null)
            {
                BreakTileAt(piece.xIndex, piece.yIndex);
            }
        }
    }

    void ClearPieceAt(int x, int y)
    {
        GamePiece gamePiece = m_allGamePiece[x, y];

        if (gamePiece != null)
        {
            Destroy(gamePiece.gameObject);
            m_allGamePiece[x, y] = null;
        }

        HighlightTileOff(x, y);
    }

    void ClearPieceAt(List<GamePiece> gamePieces)
    {
        if (gamePieces.Count > 0)
        {
            foreach (GamePiece piece in gamePieces)
            {
                ClearPieceAt(piece.xIndex, piece.yIndex);
            }
        }
    }

    void ClearPieces(List<GamePiece> gamePieces)
    {
        if (gamePieces.Count > 0)
        {
            foreach (GamePiece piece in gamePieces)
            {
                ClearPieceAt(piece.xIndex, piece.yIndex);
            }
        }
    }

    private void HighlightMatchesAt(int x, int y)
    {
        HighlightTileOff(x, y);

        List<GamePiece> combinedMatches = FindMatchesAt(x, y);

        if (combinedMatches.Count > 0)
        {
            foreach (GamePiece piece in combinedMatches)
            {
                HighlightTileOn(piece.xIndex, piece.yIndex, piece.GetComponent<SpriteRenderer>().color);
            }
        }
    }

    void HighlightMatches(List<GamePiece> gamePieces)
    {
        if (gamePieces.Count > 0)
        {
            foreach(GamePiece piece in gamePieces)
            {
                if (piece != null)
                {
                    HighlightTileOn(piece.xIndex, piece.yIndex, piece.GetComponent<SpriteRenderer>().color);
                }
            }
        }
    }

    private List<GamePiece> FindMatchesAt(int x, int y , int minLenght = 3)
    {
        List<GamePiece> horizMatches = FindHorizontalMatchPiece(x, y, minLenght);
        List<GamePiece> vertiMatches = FindVerticalMatchPiece(x, y, minLenght);

        if (horizMatches == null)
        {
            horizMatches = new List<GamePiece>();
        }

        if (vertiMatches == null)
        {
            vertiMatches = new List<GamePiece>();
        }

        var combinedMatches = horizMatches.Union(vertiMatches).ToList();

        return combinedMatches;
    }

    List<GamePiece> FindMatchesAt(List<GamePiece> gamePieces)
    {
        List<GamePiece> matchPieces = new List<GamePiece>();

        if (gamePieces.Count > 0)
        {
            foreach (GamePiece piece in gamePieces)
            {
                matchPieces = matchPieces.Union(FindMatchesAt(piece.xIndex, piece.yIndex)).ToList();
            }
        }

        return matchPieces;
    }

    List<GamePiece> FindAllMatches()
    {
        List<GamePiece> allMatches = new List<GamePiece>();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                allMatches = allMatches.Union(FindMatchesAt(i, j)).ToList();
            }
        }

        return allMatches;
    }

    List<GamePiece> CollapseColumn(int column, float collapseTime = 0.1f)
    {
        List<GamePiece> movingPieces = new List<GamePiece>();

        for (int i = 0; i < height; i++)
        {
            if (m_allGamePiece[column, i] == null && m_allTiles[column, i].tileType != Tile.TileType.Obstacle)
            {
                for (int j=i+1; j < height; j++)
                {
                    if (m_allGamePiece[column, j] != null)
                    {
                        m_allGamePiece[column, j].Move(column, i, collapseTime*(j-i));
                        m_allGamePiece[column, i] = m_allGamePiece[column, j];
                        m_allGamePiece[column, i].SetCoord(column, i);

                        if (!movingPieces.Contains(m_allGamePiece[column, i]))
                        {
                            movingPieces.Add(m_allGamePiece[column, i]);
                        }

                        m_allGamePiece[column, j] = null;

                        break;
                    }
                }
            }
        }

        return movingPieces;
    }

    List<GamePiece> CollapseColumn(List<GamePiece> gamePieces)
    {
        List<int> columns = GetColumns(gamePieces);
        List<GamePiece> movingPieces = new List<GamePiece>();

        if (columns.Count > 0)
        {
            foreach (int column in columns) {
                movingPieces = movingPieces.Union(CollapseColumn(column)).ToList();
            }
        }

        return movingPieces;
    }

    List<int> GetColumns(List<GamePiece> gamePieces)
    {
        List<int> columns = new List<int>();

        foreach (GamePiece piece in gamePieces)
        {
            if (!columns.Contains(piece.xIndex))
            {
                columns.Add(piece.xIndex);
            }
        }

        return columns;
    }

    // Collapseandrefill

    void ClearAndRefillBoard(List<GamePiece> gamePieces)
    {
        StartCoroutine(ClearAndRefillBoardRoutine(gamePieces));
    }

    IEnumerator ClearAndRefillBoardRoutine(List<GamePiece> gamePieces)
    {
        canSwitchGamePiece = false;
        List<GamePiece> allMatches = gamePieces;

        do
        {
            // clear and collapse
            yield return StartCoroutine(ClearAndCollapseRoutine(gamePieces));
            yield return null;

            //refill board
            yield return StartCoroutine(RefillBoardRoutine());
            yield return new WaitForSeconds(0.5f);
            allMatches = FindAllMatches();
        } while (allMatches.Count != 0);


        //
        canSwitchGamePiece = true;
    }

    IEnumerator RefillBoardRoutine()
    {
        FillBoard(10, 0.5f);
        yield return null;
    }

    IEnumerator ClearAndCollapseRoutine(List<GamePiece> gamePieces)
    {
        List<GamePiece> movingPieces = new List<GamePiece>();
        List<GamePiece> matches = new List<GamePiece>();

        bool isFinished = false;

        HighlightMatches(gamePieces);
        yield return new WaitForSeconds(0.25f);

        while (!isFinished)
        {
            ClearPieceAt(gamePieces);
            BreakTileAt(gamePieces);

            yield return new WaitForSeconds(0.25f);

            movingPieces = CollapseColumn(gamePieces);
            while(!Collapsed(gamePieces))
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.25f);

            matches = FindMatchesAt(gamePieces);

            if (matches.Count < 1)
            {
                isFinished = true;
                break;
            }
            else
            {
                yield return StartCoroutine(ClearAndCollapseRoutine(matches));
            }
        }

        yield return null;
    }

    bool Collapsed(List<GamePiece> gamePieces)
    {
        if (gamePieces.Count > 0)
        {
            foreach(GamePiece piece in gamePieces)
            {
                if (piece != null)
                {
                    if (piece.transform.position.y - piece.yIndex > 0.01f)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }


}
