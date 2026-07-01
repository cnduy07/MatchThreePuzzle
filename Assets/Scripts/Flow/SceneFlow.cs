using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneFlow
{
    const string LevelDatabaseResourcePath = "LevelDatabase";

    static int s_selectedLevelId = 1;

    public static int SelectedLevelId
    {
        get { return s_selectedLevelId; }
    }

    public static LevelDatabase LevelDatabase
    {
        get { return Resources.Load<LevelDatabase>(LevelDatabaseResourcePath); }
    }

    public static LevelData GetSelectedLevelData()
    {
        LevelDatabase database = LevelDatabase;
        if (database == null)
        {
            return null;
        }

        LevelData level = database.GetLevelById(s_selectedLevelId);
        return level != null ? level : database.GetLevelAtIndex(0);
    }

    public static void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneNames.Menu);
    }

    public static void LoadLevelSelect()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneNames.LevelSelect);
    }

    public static void StartLevel(int levelId)
    {
        s_selectedLevelId = Mathf.Max(1, levelId);
        LoadGame();
    }

    public static void RetryLevel()
    {
        LoadGame();
    }

    public static void LoadGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneNames.Game);
    }

    public static bool TrySelectNextLevel()
    {
        LevelDatabase database = LevelDatabase;
        if (database == null || database.levels == null || database.levels.Length == 0)
        {
            return false;
        }

        for (int i = 0; i < database.levels.Length; i++)
        {
            LevelData level = database.levels[i];
            if (level != null && level.levelId == s_selectedLevelId)
            {
                int nextIndex = i + 1;
                if (nextIndex >= database.levels.Length || database.levels[nextIndex] == null)
                {
                    return false;
                }

                s_selectedLevelId = database.levels[nextIndex].levelId;
                return true;
            }
        }

        return false;
    }
}
