using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelProgressRecord
{
    public int levelId;
    public bool completed;
    public int bestScore;
    public int bestStars;
}

[Serializable]
public class PlayerProgressData
{
    public int highestUnlockedLevelId = 1;
    public int coinCount;
    public bool audioEnabled = true;
    public bool hapticsEnabled = true;
    public List<LevelProgressRecord> levelProgress = new();
}

public static class PlayerProgress
{
    const string SaveKey = "MatchThreePuzzle.PlayerProgress.v1";

    static PlayerProgressData s_data;

    public static PlayerProgressData Data
    {
        get
        {
            EnsureLoaded();
            return s_data;
        }
    }

    public static bool AudioEnabled
    {
        get { return Data.audioEnabled; }
    }

    public static bool HapticsEnabled
    {
        get { return Data.hapticsEnabled; }
    }

    public static void EnsureLoaded()
    {
        if (s_data != null)
        {
            return;
        }

        string json = PlayerPrefs.GetString(SaveKey, string.Empty);
        if (!string.IsNullOrEmpty(json))
        {
            try
            {
                s_data = JsonUtility.FromJson<PlayerProgressData>(json);
            }
            catch (ArgumentException)
            {
                s_data = null;
            }
        }

        if (s_data == null)
        {
            s_data = new PlayerProgressData();
        }

        if (s_data.highestUnlockedLevelId < 1)
        {
            s_data.highestUnlockedLevelId = 1;
        }

        if (s_data.levelProgress == null)
        {
            s_data.levelProgress = new List<LevelProgressRecord>();
        }
    }

    public static void SetAudioEnabled(bool isEnabled)
    {
        Data.audioEnabled = isEnabled;
        Save();
    }

    public static void SetHapticsEnabled(bool isEnabled)
    {
        Data.hapticsEnabled = isEnabled;
        Save();
    }

    public static bool IsLevelUnlocked(LevelData level)
    {
        if (level == null)
        {
            return false;
        }

        return level.levelId <= Data.highestUnlockedLevelId;
    }

    public static LevelProgressRecord GetLevelProgress(int levelId)
    {
        EnsureLoaded();
        for (int i = 0; i < s_data.levelProgress.Count; i++)
        {
            LevelProgressRecord record = s_data.levelProgress[i];
            if (record != null && record.levelId == levelId)
            {
                return record;
            }
        }

        return null;
    }

    public static int GetBestScore(int levelId)
    {
        LevelProgressRecord record = GetLevelProgress(levelId);
        return record != null ? record.bestScore : 0;
    }

    public static int GetBestStars(int levelId)
    {
        LevelProgressRecord record = GetLevelProgress(levelId);
        return record != null ? record.bestStars : 0;
    }

    public static bool IsLevelComplete(int levelId)
    {
        LevelProgressRecord record = GetLevelProgress(levelId);
        return record != null && record.completed;
    }

    public static int RecordLevelWin(LevelData level, int score, int movesLeft, int moveLimit, LevelDatabase database)
    {
        if (level == null)
        {
            return 0;
        }

        int stars = CalculateStars(score, level.scoreGoal, movesLeft, moveLimit);
        LevelProgressRecord record = GetOrCreateLevelProgress(level.levelId);
        record.completed = true;
        record.bestScore = Mathf.Max(record.bestScore, score);
        record.bestStars = Mathf.Max(record.bestStars, stars);

        LevelData nextLevel = database != null ? database.GetNextLevel(level.levelId) : null;
        if (nextLevel != null)
        {
            s_data.highestUnlockedLevelId = Mathf.Max(s_data.highestUnlockedLevelId, nextLevel.levelId);
        }
        else
        {
            s_data.highestUnlockedLevelId = Mathf.Max(s_data.highestUnlockedLevelId, level.levelId);
        }

        Save();
        return stars;
    }

    public static int CalculateStars(int score, int scoreGoal, int movesLeft, int moveLimit)
    {
        if (scoreGoal <= 0 || score < scoreGoal)
        {
            return 0;
        }

        int stars = 1;
        float scoreRatio = (float)score / scoreGoal;
        float movesRatio = moveLimit > 0 ? (float)movesLeft / moveLimit : 0f;

        if (scoreRatio >= 1.25f || movesRatio >= 0.25f)
        {
            stars++;
        }

        if (scoreRatio >= 1.5f || movesRatio >= 0.4f)
        {
            stars++;
        }

        return Mathf.Clamp(stars, 1, 3);
    }

    static LevelProgressRecord GetOrCreateLevelProgress(int levelId)
    {
        LevelProgressRecord record = GetLevelProgress(levelId);
        if (record != null)
        {
            return record;
        }

        record = new LevelProgressRecord { levelId = levelId };
        s_data.levelProgress.Add(record);
        return record;
    }

    static void Save()
    {
        EnsureLoaded();
        string json = JsonUtility.ToJson(s_data);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }
}
