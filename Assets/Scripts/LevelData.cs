using UnityEngine;

public enum LevelObjectiveType
{
    Score,
    CollectiblesToBottom,
    BreakTiles,
    ClearBlockers,
    Mixed
}

[System.Serializable]
public class LevelStartingObject
{
    public GameObject prefab;
    public int x;
    public int y;
    public int z;
}

[CreateAssetMenu(fileName = "LevelData", menuName = "Match Three/Level Data")]
public class LevelData : ScriptableObject
{
    public int levelId = 1;
    public string displayName = "Level 1";
    public ThemeData themeData;
    public PieceSetData pieceSetData;

    [Min(1)]
    public int boardWidth = 7;

    [Min(1)]
    public int boardHeight = 9;

    [Min(0)]
    public int moveLimit = 30;

    [Min(0)]
    public int scoreGoal = 10000;

    public LevelObjectiveType objectiveType = LevelObjectiveType.Score;

    [Min(0)]
    public int objectiveTargetCount;

    public GameObject normalTilePrefab;
    public GameObject[] obstacleTilePrefabs;
    public GameObject[] rowBombPrefabs;
    public GameObject[] columnBombPrefabs;
    public GameObject[] adjacentBombPrefabs;
    public GameObject colorBombPrefab;
    public GameObject[] gamePiecePrefabs;
    public LevelStartingObject[] startingTiles;
    public LevelStartingObject[] startingPieces;

    [Min(0)]
    public int collectibleMax = 3;

    [Range(0, 1)]
    public float chanceForCollectible = 0.1f;

    public GameObject[] collectiblePrefabs;
}
