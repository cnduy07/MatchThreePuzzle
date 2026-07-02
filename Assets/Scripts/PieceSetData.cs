using UnityEngine;

[CreateAssetMenu(fileName = "PieceSetData", menuName = "Match Three/Piece Set Data")]
public class PieceSetData : ScriptableObject
{
    public string pieceSetId = "classic";
    public string displayName = "Classic Pieces";

    [Header("Normal Pieces")]
    public GameObject[] normalPiecePrefabs;

    [Header("Special Pieces")]
    public GameObject[] rowBombPrefabs;
    public GameObject[] columnBombPrefabs;
    public GameObject[] adjacentBombPrefabs;
    public GameObject colorBombPrefab;

    [Header("Collectibles")]
    public GameObject[] collectiblePrefabs;
    public GameObject[] blockerPrefabs;

    public GameObject[] ResolveNormalPieces(GameObject[] fallback)
    {
        return HasConfiguredPrefab(normalPiecePrefabs) ? normalPiecePrefabs : fallback;
    }

    public GameObject[] ResolveRowBombs(GameObject[] fallback)
    {
        return HasConfiguredPrefab(rowBombPrefabs) ? rowBombPrefabs : fallback;
    }

    public GameObject[] ResolveColumnBombs(GameObject[] fallback)
    {
        return HasConfiguredPrefab(columnBombPrefabs) ? columnBombPrefabs : fallback;
    }

    public GameObject[] ResolveAdjacentBombs(GameObject[] fallback)
    {
        return HasConfiguredPrefab(adjacentBombPrefabs) ? adjacentBombPrefabs : fallback;
    }

    public GameObject ResolveColorBomb(GameObject fallback)
    {
        return colorBombPrefab != null ? colorBombPrefab : fallback;
    }

    public GameObject[] ResolveCollectibles(GameObject[] fallback)
    {
        return HasConfiguredPrefab(collectiblePrefabs) ? collectiblePrefabs : fallback;
    }

    public static bool HasConfiguredPrefab(GameObject[] prefabs)
    {
        if (prefabs == null)
        {
            return false;
        }

        for (int i = 0; i < prefabs.Length; i++)
        {
            if (prefabs[i] != null)
            {
                return true;
            }
        }

        return false;
    }
}
