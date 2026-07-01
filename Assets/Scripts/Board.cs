using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public int borderSize;

    public GameObject normalTilePrefab;
    public GameObject[] obstacleTilePrefabs;
    public GameObject[] rowBombPrefabs;
    public GameObject[] columnBombPrefabs;
    public GameObject[] adjacentBombPrefabs;
    public GameObject colorBombPrefab;

    public GameObject[] gamePiecePrefabs;
    public StartingGameObject[] startingTiles;
    public StartingGameObject[] startingPieces;

    float swapTime = 0.5f;
    int fillYOffset = 10;
    float fillMoveTime = 0.5f;
    bool m_playerInputEnable = true;

    Tile m_clickedTile;
    Tile m_targetTile;

    ParticleManager m_particleManager;
    GameObject m_clickedTileBomb;
    GameObject m_targetTileBomb;

    Tile[,] m_allTiles;
    GamePiece[,] m_allGamePieces;
    Camera m_mainCamera;
    bool m_isBoardSetup = false;
    bool m_isPointerDown = false;

    [System.Serializable]
    public class StartingGameObject
    {
        public GameObject prefab;
        public int x;
        public int y;
        public int z;
    }

    public int collectibleCount = 0;
    public int collectibleMax = 3;

    [Range(0, 1)]
    public float changeForCollectible = 0.1f;
    public GameObject[] collectiblePrefabs;

    int m_scoreMultiplier;

    public bool isRefilling = false;

    void Awake()
    {
        m_allTiles = new Tile[width, height];
        m_allGamePieces = new GamePiece[width, height];
        m_mainCamera = Camera.main;
    }

    void Start()
    {
        GameObject particleManagerObject = GameObject.FindGameObjectWithTag("ParticleManager");
        if (particleManagerObject != null)
        {
            m_particleManager = particleManagerObject.GetComponent<ParticleManager>();
        }
    }

    void Update()
    {
        HandleBoardInput();
    }

    public void SetPlayerInputEnabled(bool isEnabled)
    {
        m_playerInputEnable = isEnabled;
        if (!m_playerInputEnable)
        {
            m_isPointerDown = false;
            m_clickedTile = null;
            m_targetTile = null;
        }
    }

    public void SetupBoard()
    {
        if (m_isBoardSetup)
        {
            return;
        }

        List<GamePiece> foundCollectible = FindAllCollectibles();
        collectibleCount = foundCollectible.Count;

        SettupTile();
        SettupStartingPiece();
        SettupCamera();
        FillBoard(fillYOffset, fillMoveTime);
        m_isBoardSetup = true;
    }

    public void ApplyLevelData(LevelData levelData)
    {
        if (levelData == null)
        {
            return;
        }

        if (m_isBoardSetup)
        {
            Debug.LogWarning("BOARD: LevelData cannot be applied after the board has been set up");
            return;
        }

        width = Mathf.Max(1, levelData.boardWidth);
        height = Mathf.Max(1, levelData.boardHeight);

        if (levelData.normalTilePrefab != null)
        {
            normalTilePrefab = levelData.normalTilePrefab;
        }

        obstacleTilePrefabs = levelData.obstacleTilePrefabs ?? new GameObject[0];
        rowBombPrefabs = levelData.rowBombPrefabs ?? new GameObject[0];
        columnBombPrefabs = levelData.columnBombPrefabs ?? new GameObject[0];
        adjacentBombPrefabs = levelData.adjacentBombPrefabs ?? new GameObject[0];

        if (levelData.colorBombPrefab != null)
        {
            colorBombPrefab = levelData.colorBombPrefab;
        }

        gamePiecePrefabs = levelData.gamePiecePrefabs ?? new GameObject[0];
        startingTiles = ConvertStartingObjects(levelData.startingTiles);
        startingPieces = ConvertStartingObjects(levelData.startingPieces);
        collectibleMax = Mathf.Max(0, levelData.collectibleMax);
        changeForCollectible = Mathf.Clamp01(levelData.chanceForCollectible);
        collectiblePrefabs = levelData.collectiblePrefabs ?? new GameObject[0];
        collectibleCount = 0;

        m_allTiles = new Tile[width, height];
        m_allGamePieces = new GamePiece[width, height];
    }

    StartingGameObject[] ConvertStartingObjects(LevelStartingObject[] levelObjects)
    {
        if (levelObjects == null || levelObjects.Length == 0)
        {
            return new StartingGameObject[0];
        }

        StartingGameObject[] convertedObjects = new StartingGameObject[levelObjects.Length];
        for (int i = 0; i < levelObjects.Length; i++)
        {
            LevelStartingObject levelObject = levelObjects[i];
            convertedObjects[i] = new StartingGameObject();

            if (levelObject == null)
            {
                continue;
            }

            convertedObjects[i].prefab = levelObject.prefab;
            convertedObjects[i].x = levelObject.x;
            convertedObjects[i].y = levelObject.y;
            convertedObjects[i].z = levelObject.z;
        }

        return convertedObjects;
    }

    void HandleBoardInput()
    {
        if (!m_playerInputEnable || isRefilling)
        {
            return;
        }

        if (Input.touchCount > 0)
        {
            HandleTouchInput(Input.GetTouch(0));
            return;
        }

        HandleMouseInput();
    }

    void HandleTouchInput(Touch touch)
    {
        if (touch.phase == TouchPhase.Began && IsPointerOverBlockingUi(touch.position, touch.fingerId))
        {
            return;
        }

        switch (touch.phase)
        {
            case TouchPhase.Began:
                BeginBoardPointer(touch.position);
                break;
            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                DragBoardPointer(touch.position);
                break;
            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                EndBoardPointer();
                break;
        }
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverBlockingUi(Input.mousePosition))
            {
                return;
            }

            BeginBoardPointer(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            DragBoardPointer(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndBoardPointer();
        }
    }

    bool IsPointerOverBlockingUi(Vector2 screenPosition, int pointerId = -1)
    {
        if (EventSystem.current == null)
        {
            return false;
        }

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = screenPosition,
            pointerId = pointerId
        };

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        foreach (RaycastResult result in raycastResults)
        {
            Selectable selectable = result.gameObject.GetComponentInParent<Selectable>();
            if (selectable != null && selectable.IsInteractable())
            {
                return true;
            }
        }

        return false;
    }

    void BeginBoardPointer(Vector2 screenPosition)
    {
        Tile tile = GetTileAtScreenPosition(screenPosition);
        if (tile != null)
        {
            m_isPointerDown = true;
            ClickedTile(tile);
        }
    }

    void DragBoardPointer(Vector2 screenPosition)
    {
        if (!m_isPointerDown)
        {
            return;
        }

        Tile tile = GetTileAtScreenPosition(screenPosition);
        if (tile != null)
        {
            DragToTile(tile);
        }
    }

    void EndBoardPointer()
    {
        if (!m_isPointerDown)
        {
            return;
        }

        ReleaseTile();
        m_isPointerDown = false;
    }

    Tile GetTileAtScreenPosition(Vector2 screenPosition)
    {
        if (m_mainCamera == null)
        {
            m_mainCamera = Camera.main;
        }

        if (m_mainCamera == null)
        {
            return null;
        }

        Ray ray = m_mainCamera.ScreenPointToRay(screenPosition);
        if (Mathf.Approximately(ray.direction.z, 0f))
        {
            return null;
        }

        float distanceToBoardPlane = -ray.origin.z / ray.direction.z;
        Vector3 worldPosition = ray.GetPoint(distanceToBoardPlane);
        int x = Mathf.FloorToInt(worldPosition.x + 0.5f);
        int y = Mathf.FloorToInt(worldPosition.y + 0.5f);

        if (!IsWithinBounds(x, y))
        {
            return null;
        }

        return m_allTiles[x, y];
    }

    void SettupTile()
    {
        foreach(StartingGameObject startingTile in startingTiles)
        {
            if (startingTile != null)
            {
                MakeNewTile(startingTile.prefab, startingTile.x, startingTile.y);
            }
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
        if (prefab == null || !IsWithinBounds(x, y) || m_allTiles[x, y] != null)
        {
            return;
        }

        GameObject tile = Instantiate(prefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
        Tile tileComponent = tile.GetComponent<Tile>();
        if (tileComponent == null)
        {
            Debug.LogWarning("BOARD: Tile prefab does not contain a Tile component");
            Destroy(tile);
            return;
        }

        tile.name = "Tile (" + x + " , " + y + ")";

        m_allTiles[x, y] = tileComponent;
        m_allTiles[x, y].Init(x, y, this);

        tile.transform.parent = transform;
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

    GameObject GetRandomGamePiecePrefab()
    {
        return GetRandomObjectPrefab(gamePiecePrefabs);
    }

    GameObject GetRandomCollectiblePrefab()
    {
        return GetRandomObjectPrefab(collectiblePrefabs);
    }

    GameObject GetRandomObjectPrefab(GameObject[] objectPrefabs)
    {
        if (objectPrefabs == null || objectPrefabs.Length == 0)
        {
            Debug.LogWarning("BOARD: No object prefabs configured");
            return null;
        }

        int randomIdx = Random.Range(0, objectPrefabs.Length);
        if (objectPrefabs[randomIdx] == null)
        {
            Debug.LogWarning("BOARD: Invalid object" + randomIdx + "does not contain valid object prefab");
        }

        return objectPrefabs[randomIdx];
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
            m_allGamePieces[x, y] = gamePiece;
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
                Tile tile = m_allTiles[i, j];
                if (tile != null && m_allGamePieces[i, j] == null && tile.tileType != Tile.TileType.Obstacle)
                {
                    if (j == height - 1 && CanAddCollectible())
                    {
                        FillRandomCollectibleAt(i, j, yOffset, moveTime);
                        collectibleCount += 1;
                    } else
                    {
                        FillRandomPieceAt(i, j, yOffset, moveTime);
                        iteration = 0;

                        while (HasMatchesOnFill(i, j))
                        {
                            ClearPieceAt(i, j);
                            FillRandomPieceAt(i, j, yOffset, moveTime);
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

    private GamePiece FillRandomPieceAt(int i, int j, int yOffset, float moveTime)
    {
        GameObject piecePrefab = GetRandomGamePiecePrefab();
        if (piecePrefab == null)
        {
            return null;
        }

        GameObject randomPiecePrefab = Instantiate(piecePrefab, Vector3.zero, Quaternion.identity) as GameObject;

        if (randomPiecePrefab != null && IsWithinBounds(i, j))
        {
            MakeNewPiece(randomPiecePrefab, i, j, yOffset, moveTime);
            return randomPiecePrefab.GetComponent<GamePiece>();
        }

        return null;
    }

    private GamePiece FillRandomCollectibleAt(int i, int j, int yOffset, float moveTime)
    {
        GameObject collectiblePrefab = GetRandomCollectiblePrefab();
        if (collectiblePrefab == null)
        {
            return null;
        }

        GameObject randomCollectible = Instantiate(collectiblePrefab, Vector3.zero, Quaternion.identity) as GameObject;

        if (randomCollectible != null && IsWithinBounds(i, j))
        {
            MakeNewPiece(randomCollectible, i, j, yOffset, moveTime);
            return randomCollectible.GetComponent<GamePiece>();
        }

        return null;
    }

    private void SettupStartingPiece()
    {
        foreach(StartingGameObject startingPiece in startingPieces)
        {
            if (startingPiece != null)
            {
                GameObject gamePiecePrefab = Instantiate(startingPiece.prefab, new Vector3(startingPiece.x, startingPiece.y, 0), Quaternion.identity) as GameObject;
                MakeNewPiece(gamePiecePrefab, startingPiece.x, startingPiece.y, fillYOffset, fillMoveTime);
            }
        }
    }

    private void MakeNewPiece(GameObject gamePiecePrefab, int x, int y, int yOffset, float moveTime)
    {
        if (gamePiecePrefab != null && IsWithinBounds(x, y))
        {
            gamePiecePrefab.GetComponent<GamePiece>().Init(this);
            gamePiecePrefab.transform.parent = transform;

            PlaceGamePiece(gamePiecePrefab.GetComponent<GamePiece>(), x, y);

            if (yOffset != 0)
            {
                gamePiecePrefab.transform.position = new Vector3(x, y + yOffset, 0);
                gamePiecePrefab.GetComponent<GamePiece>().Move(x, y, moveTime);
            }
        }
    }

    GameObject MakeNewBomb(GameObject bombPrefab, int x, int y)
    {
        if (bombPrefab != null && IsWithinBounds(x, y))
        {
            GameObject bomb = Instantiate(bombPrefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
            bomb.GetComponent<Bomb>().Init(this);
            bomb.GetComponent<Bomb>().SetCoord(x, y);
            bomb.transform.parent = transform;

            return bomb;
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
        if (GameManager.Instance != null)
        {
            if (!GameManager.Instance.IsGameOver && m_playerInputEnable)
            {
                StartCoroutine(SwitchTilesRoutine(clickedTile, targetTile));
            }
        }
    }

    IEnumerator SwitchTilesRoutine(Tile clickedTile, Tile targetTile)
    {
        GamePiece clickedPiece = m_allGamePieces[clickedTile.xIndex, clickedTile.yIndex];
        GamePiece targetPiece = m_allGamePieces[targetTile.xIndex, targetTile.yIndex];

        if (clickedPiece != null && targetPiece != null)
        {
            clickedPiece.Move(targetTile.xIndex, targetTile.yIndex, swapTime);
            targetPiece.Move(clickedTile.xIndex, clickedTile.yIndex, swapTime);

            yield return new WaitForSeconds(swapTime);

            List<GamePiece> clickedPieceMatches = FindMatchesAt(clickedTile.xIndex, clickedTile.yIndex);
            List<GamePiece> targetPieceMatches = FindMatchesAt(targetTile.xIndex, targetTile.yIndex);
            List<GamePiece> colorMatches = new List<GamePiece>();

            if (IsColorBomb(clickedPiece) && !IsColorBomb(targetPiece))
            {
                clickedPiece.matchValue = targetPiece.matchValue;
                colorMatches = FindAllMatchValue(targetPiece.matchValue);
            }

            if (!IsColorBomb(clickedPiece) && IsColorBomb(targetPiece))
            {
                targetPiece.matchValue = clickedPiece.matchValue;
                colorMatches = FindAllMatchValue(clickedPiece.matchValue);
            }

            if (IsColorBomb(clickedPiece) && IsColorBomb(targetPiece))
            {
                foreach (GamePiece gamePiece in m_allGamePieces)
                {
                    if (!colorMatches.Contains(gamePiece))
                    {
                        colorMatches.Add(gamePiece);
                    }
                }
            }

            if (clickedPieceMatches.Count == 0 && targetPieceMatches.Count == 0 && colorMatches.Count == 0)
            {
                clickedPiece.Move(clickedTile.xIndex, clickedTile.yIndex, swapTime);
                targetPiece.Move(targetTile.xIndex, targetTile.yIndex, swapTime);
            } else
            {
                yield return new WaitForSeconds(swapTime);

                GameManager.Instance.movesLeft--;
                GameManager.Instance.UpdateMoves();

                Vector2 direction = new Vector2(targetTile.xIndex - clickedTile.xIndex, targetTile.yIndex - clickedTile.yIndex);

                m_clickedTileBomb = DropBomb(clickedPieceMatches, clickedTile.xIndex, clickedTile.yIndex, direction);
                m_targetTileBomb = DropBomb(targetPieceMatches, targetTile.xIndex, targetTile.yIndex, direction);

                if (m_clickedTileBomb != null && targetPiece != null)
                {
                    m_clickedTileBomb.GetComponent<GamePiece>().ChangeColor(targetPiece);
                }

                if (m_targetTileBomb != null && clickedPiece != null)
                {
                    m_targetTileBomb.GetComponent<GamePiece>().ChangeColor(clickedPiece);
                }

                ClearAndRefillBoard(clickedPieceMatches.Union(targetPieceMatches).ToList().Union(colorMatches).ToList());
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
            startPiece = m_allGamePieces[startX, startY];
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

            nextPiece = m_allGamePieces[nextX, nextY];

            if (nextPiece != null && nextPiece.matchValue == startPiece.matchValue && !matches.Contains(nextPiece) && nextPiece.matchValue != MatchValue.None)
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

    void BreakTileAt(int x, int y)
    {
        if (!IsWithinBounds(x, y))
        {
            return;
        }

        Tile tile = m_allTiles[x, y];
        if (tile != null)
        {
            if (m_particleManager != null)
            {
                m_particleManager.BreakTileFXAt(tile.breakableValue, x, y);
            }

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
        if (!IsWithinBounds(x, y))
        {
            return;
        }

        GamePiece gamePiece = m_allGamePieces[x, y];

        if (gamePiece != null)
        {
            m_allGamePieces[x, y] = null;
            Destroy(gamePiece.gameObject);
        }
    }

    void ClearPieceAt(List<GamePiece> gamePieces, List<GamePiece> bombPieces)
    {
        if (gamePieces.Count > 0)
        {
            foreach (GamePiece piece in gamePieces)
            {
                if (piece != null)
                {
                    if (m_particleManager != null)
                    {
                        if (bombPieces.Contains(piece))
                        {
                            m_particleManager.BombFXAt(piece.xIndex, piece.yIndex);
                        } else
                        {
                            m_particleManager.ClearTileFXAt(piece.xIndex, piece.yIndex);
                        }
                    }

                    ClearPieceAt(piece.xIndex, piece.yIndex);

                    int bonus = 0;
                    if (gamePieces.Count > 3)
                    {
                        bonus = 20;
                    }

                    piece.AddScore(m_scoreMultiplier, bonus);
                }
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
        List<GamePiece> movingPieces = new();

        for (int i = 0; i < height; i++)
        {
            Tile tile = m_allTiles[column, i];
            if (tile != null && m_allGamePieces[column, i] == null && tile.tileType != Tile.TileType.Obstacle)
            {
                for (int j=i+1; j < height; j++)
                {
                    if (m_allGamePieces[column, j] != null)
                    {
                        m_allGamePieces[column, j].Move(column, i, collapseTime*(j-i));
                        m_allGamePieces[column, i] = m_allGamePieces[column, j];
                        m_allGamePieces[column, i].SetCoord(column, i);

                        if (!movingPieces.Contains(m_allGamePieces[column, i]))
                        {
                            movingPieces.Add(m_allGamePieces[column, i]);
                        }

                        m_allGamePieces[column, j] = null;

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
        List<GamePiece> movingPieces = new();

        if (columns.Count > 0)
        {
            foreach (int column in columns) {
                movingPieces = movingPieces.Union(CollapseColumn(column)).ToList();
            }
        }

        return movingPieces;
    }

    List<GamePiece> CollapseColumn(List<int> columns)
    {
        List<GamePiece> movingPieces = new();
        if (columns.Count > 0)
        {
            foreach (int column in columns)
            {
                movingPieces = movingPieces.Union(CollapseColumn(column)).ToList();
            }
        }

        return movingPieces;
    }

    List<int> GetColumns(List<GamePiece> gamePieces)
    {
        List<int> columns = new();

        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                if (!columns.Contains(piece.xIndex))
                {
                    columns.Add(piece.xIndex);
                }
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
        isRefilling = true;
        m_playerInputEnable = false;
        List<GamePiece> matches = gamePieces;

        m_scoreMultiplier = 0;

        do
        {
            m_scoreMultiplier++;
            // clear and collapse
            yield return StartCoroutine(ClearAndCollapseRoutine(matches));
            yield return null;

            //refill board
            yield return StartCoroutine(RefillBoardRoutine());

            matches = FindAllMatches();

            yield return new WaitForSeconds(0.2f);

        } while (matches.Count > 0);

        m_playerInputEnable = true;
        isRefilling = false;
    }

    IEnumerator RefillBoardRoutine()
    {
        FillBoard(fillYOffset, fillMoveTime);
        yield return null;
    }

    IEnumerator ClearAndCollapseRoutine(List<GamePiece> gamePieces)
    {
        List<GamePiece> movingPieces = new();
        List<GamePiece> matches = new();

        bool isFinished = false;

        yield return new WaitForSeconds(0.25f);

        while (!isFinished)
        {
            List<GamePiece> bombedPieces = GetBombedPieces(gamePieces);
            gamePieces = gamePieces.Union(bombedPieces).ToList();

            bombedPieces = GetBombedPieces(gamePieces);
            gamePieces = gamePieces.Union(bombedPieces).ToList();

            List<GamePiece> collectiblePieces = FindCollectibleAtRow(0, true);
            List<GamePiece> allCollectiblePieces = FindAllCollectibles();
            List<GamePiece> blockPieces = gamePieces.Intersect(allCollectiblePieces).ToList();
            collectiblePieces = collectiblePieces.Union(blockPieces).ToList();

            collectibleCount -= collectiblePieces.Count;
            gamePieces = gamePieces.Union(collectiblePieces).ToList();

            List<int> columns = GetColumns(gamePieces);

            ClearPieceAt(gamePieces, bombedPieces);
            BreakTileAt(gamePieces);

            if (m_clickedTileBomb != null)
            {
                ActivateBomb(m_clickedTileBomb);
                m_clickedTileBomb = null;
            }

            if (m_targetTileBomb != null)
            {
                ActivateBomb(m_targetTileBomb);
                m_targetTileBomb = null;
            }

            yield return new WaitForSeconds(0.25f);

            movingPieces = CollapseColumn(columns);

            while(!Collapsed(movingPieces))
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.25f);

            matches = FindMatchesAt(gamePieces);

            collectiblePieces = FindCollectibleAtRow(0, true);
            matches = matches.Union(collectiblePieces).ToList();

            if (matches.Count < 1)
            {
                break;
            }
            else
            {
                m_scoreMultiplier++;
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayBonusSound();
                }

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

    List<GamePiece> GetColumnPieces(int column)
    {
        List<GamePiece> gamePieces = new();

        for (int i = 0; i < height; i++)
        {
            if (m_allGamePieces[column, i] != null)
            {
                gamePieces.Add(m_allGamePieces[column, i]);
            }
        }

        return gamePieces;
    }

    List<GamePiece> GetRowPieces(int row)
    {
        List<GamePiece> gamePieces = new List<GamePiece>();

        for (int i = 0; i < width; i++)
        {
            if (m_allGamePieces[i, row] != null)
            {
                gamePieces.Add(m_allGamePieces[i, row]);
            }
        }

        return gamePieces;
    }

    List<GamePiece> GetAdjacentPieces(int x, int y, int offset)
    {
        List<GamePiece> gamePieces = new List<GamePiece>();

        for (int i = x - offset; i <= x + offset; i++)
        {
            for (int j = y - offset; j <= y + offset; j++)
            {
                if (IsWithinBounds(i, j))
                {
                    if (m_allGamePieces[i, j] != null)
                    {
                        gamePieces.Add(m_allGamePieces[i, j]);
                    }
                }
            }
        }

        return gamePieces;
    }

    List<GamePiece> GetBombedPieces(List<GamePiece> gamePieces)
    {
        List<GamePiece> allBombedPieces = new List<GamePiece>();

        foreach (GamePiece gamePiece in gamePieces) {
            if (gamePiece != null)
            {
                if (gamePiece.GetComponent<Bomb>() != null)
                {
                    List<GamePiece> bombedPieces = new List<GamePiece>();
                    Bomb bomb = gamePiece.GetComponent<Bomb>();

                    switch (bomb.bombType)
                    {
                        case BombType.Column:
                            bombedPieces = bombedPieces.Union(GetColumnPieces(gamePiece.xIndex)).ToList();
                            break;
                        case BombType.Row:
                            bombedPieces = bombedPieces.Union(GetRowPieces(gamePiece.yIndex)).ToList();
                            break;
                        case BombType.Adjacent:
                            bombedPieces = bombedPieces.Union(GetAdjacentPieces(gamePiece.xIndex, gamePiece.yIndex, 1)).ToList();
                            break;
                        case BombType.Color:
                            break;
                    }

                    allBombedPieces = allBombedPieces.Union(bombedPieces).ToList();
                    allBombedPieces = RemoveCollectible(allBombedPieces);
                }
            }
        }

        return allBombedPieces;
    }

    GameObject DropBomb(List<GamePiece> gamePieces, int x, int y, Vector2 direction)
    {
        GameObject bomb = null;

        MatchValue matchValue = FindMatchValue(gamePieces);

        if (gamePieces.Count >= 5 && matchValue != MatchValue.None)
        {
            if (IsCornerMatches(gamePieces, x, y))
            {
                GameObject adjacentBombPrefab = FindGamePieceByMatchValue(adjacentBombPrefabs, matchValue);
                if (adjacentBombPrefab != null)
                {
                    bomb = MakeNewBomb(adjacentBombPrefab, x, y);
                    Debug.Log("DROP BOMB: Adjacent Bomb");
                }
            }
            else
            {
                bomb = MakeNewBomb(colorBombPrefab, x, y);
            }
        }
        else if (gamePieces.Count == 4 && matchValue != MatchValue.None)
        {
            if (direction.x != 0)
            {
                GameObject rowBombPrefab = FindGamePieceByMatchValue(rowBombPrefabs, matchValue);
                if (rowBombPrefab != null)
                {
                    bomb = MakeNewBomb(rowBombPrefab, x, y);
                }
            }
            else
            {
                GameObject columnBombPrefab = FindGamePieceByMatchValue(columnBombPrefabs, matchValue);
                if (columnBombPrefab != null)
                {
                    bomb = MakeNewBomb(columnBombPrefab, x, y);
                }
            }
        }

        return bomb;
    }

    bool IsCornerMatches(List<GamePiece> gamePieces, int x, int y)
    {
        bool vertical = false;
        bool horizontal = false;
        int xStart = -1;
        int yStart = -1;

        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                if (xStart == -1 || yStart == -1)
                {
                    xStart = piece.xIndex;
                    yStart = piece.yIndex;
                    continue;
                }

                if (piece.xIndex != xStart && piece.yIndex == yStart)
                {
                    horizontal = true;
                }

                if (piece.xIndex == xStart && piece.yIndex != yStart)
                {
                    vertical = true;
                }
            }
        }

        return (horizontal && vertical);
    }

    void ActivateBomb(GameObject bomb)
    {
        int x = (int)bomb.transform.position.x;
        int y = (int)bomb.transform.position.y;

        if (IsWithinBounds(x, y))
        {
            m_allGamePieces[x, y] = bomb.GetComponent<GamePiece>();
        }
    }

    List<GamePiece> FindAllMatchValue(MatchValue mValue)
    {
        List<GamePiece> foundPieces = new List<GamePiece>();

        foreach (GamePiece gamePiece in m_allGamePieces)
        {
            if (gamePiece != null && gamePiece.matchValue == mValue)
            {
                foundPieces.Add(gamePiece);
            }
        }

        return foundPieces;
    }

    bool IsColorBomb(GamePiece gamePiece)
    {
        if (gamePiece == null)
        {
            return false;
        }

        Bomb bomb = gamePiece.GetComponent<Bomb>();
        if (bomb != null)
        {
            if (bomb.bombType == BombType.Color)
            {
                return true;
            }
        }

        return false;
    }

    List<GamePiece> FindCollectibleAtRow(int rowId, bool isClearedAtBottom = false)
    {
        List<GamePiece> foundCollectibles = new();
        if (rowId < 0 || rowId >= height)
        {
            return foundCollectibles;
        }

        for (int i = 0; i < width; i++)
        {
            if (m_allGamePieces[i, rowId] != null)
            {
                Collectibles collectibleComponent = m_allGamePieces[i, rowId].GetComponent<Collectibles>();

                if (collectibleComponent != null)
                {
                    if (m_allGamePieces[i, rowId].matchValue == MatchValue.None)
                    {
                        if ((collectibleComponent.clearedAtBottom && isClearedAtBottom) || !isClearedAtBottom)
                        {
                            foundCollectibles.Add(m_allGamePieces[i, rowId]);
                        }
                    }
                }
            }
        }

        return foundCollectibles;
    }

    List<GamePiece> FindAllCollectibles()
    {
        List<GamePiece> foundCollectibles = new List<GamePiece>();

        for (int i = 0; i < height; i++)
        {
            List<GamePiece> foundCollectibleAtRow = FindCollectibleAtRow(i);
            foundCollectibles = foundCollectibles.Union(foundCollectibleAtRow).ToList();
        }

        return foundCollectibles;
    }

    bool CanAddCollectible()
    {
        return (collectibleCount < collectibleMax && collectiblePrefabs != null && collectiblePrefabs.Length > 0 && Random.Range(0f, 1f) <= changeForCollectible);
    }

    List<GamePiece> RemoveCollectible(List<GamePiece> bombPieces)
    {
        List<GamePiece> allCollectiblePieces = FindAllCollectibles();
        List<GamePiece> piecesToRemove = new();

        foreach(GamePiece piece in allCollectiblePieces)
        {
            Collectibles collectibleComponent = piece.GetComponent<Collectibles>();
            if (collectibleComponent != null)
            {
                if (!collectibleComponent.clearedByBomb)
                {
                    piecesToRemove.Add(collectibleComponent);
                }
            }
        }

        return bombPieces.Except(piecesToRemove).ToList();
    }

    MatchValue FindMatchValue(List<GamePiece> gamePieces)
    {
        if (gamePieces != null)
        {
            foreach(GamePiece piece in gamePieces)
            {
                if (piece != null)
                {
                    return piece.matchValue;
                }
            }
        }

        return MatchValue.None;
    }

    GameObject FindGamePieceByMatchValue(GameObject[] gamePiecePrefabs, MatchValue matchValue)
    {
        if (gamePiecePrefabs == null || matchValue == MatchValue.None)
        {
            return null;
        }

        foreach(GameObject go in gamePiecePrefabs)
        {
            if (go == null)
            {
                continue;
            }

            GamePiece piece = go.GetComponent<GamePiece>();

            if (piece != null)
            {
                if (piece.matchValue == matchValue)
                {
                    return go;
                }
            }
        }

        return null;
    }
}
