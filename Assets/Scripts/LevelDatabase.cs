using UnityEngine;

[CreateAssetMenu(fileName = "LevelDatabase", menuName = "Match Three/Level Database")]
public class LevelDatabase : ScriptableObject
{
    public LevelData[] levels;

    public LevelData GetLevelById(int levelId)
    {
        if (levels == null)
        {
            return null;
        }

        foreach (LevelData level in levels)
        {
            if (level != null && level.levelId == levelId)
            {
                return level;
            }
        }

        return null;
    }

    public LevelData GetLevelAtIndex(int index)
    {
        if (levels == null || index < 0 || index >= levels.Length)
        {
            return null;
        }

        return levels[index];
    }
}
