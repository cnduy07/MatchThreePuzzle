using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneBootstrapper : MonoBehaviour
{
    Canvas m_canvas;

    void Start()
    {
        RuntimeSettingsState.EnsureInitialized();

        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == SceneNames.Boot)
        {
            SceneFlow.LoadMenu();
            return;
        }

        if (sceneName == SceneNames.Menu)
        {
            BuildMenu();
            return;
        }

        if (sceneName == SceneNames.LevelSelect)
        {
            BuildLevelSelect();
        }
    }

    void BuildMenu()
    {
        CreateCanvas("Menu Canvas");
        RectTransform root = m_canvas.GetComponent<RectTransform>();
        RuntimeUiFactory.CreatePanel(root, "Background", new Color(0.06f, 0.08f, 0.1f, 1f));
        RectTransform contentRoot = RuntimeUiFactory.CreateSafeAreaRoot(root, "Safe Area Root");

        Text title = RuntimeUiFactory.CreateText(contentRoot, "Title", "Match Three Puzzle", 70, Color.white, TextAnchor.MiddleCenter);
        RuntimeUiFactory.SetCenter(title.GetComponent<RectTransform>(), new Vector2(0f, 330f), new Vector2(840f, 130f));

        Text subtitle = RuntimeUiFactory.CreateText(contentRoot, "Subtitle", "Level goals, bombs, cascades, and collectible drops", 32, new Color(0.78f, 0.84f, 0.9f, 1f), TextAnchor.MiddleCenter);
        RuntimeUiFactory.SetCenter(subtitle.GetComponent<RectTransform>(), new Vector2(0f, 225f), new Vector2(820f, 90f));

        int highestUnlockedLevel = PlayerProgress.Data.highestUnlockedLevelId;
        string playLabel = highestUnlockedLevel > 1 ? "Continue Level " + highestUnlockedLevel : "Play";
        Button play = RuntimeUiFactory.CreateButton(contentRoot, "Play Button", playLabel, new Color(0.11f, 0.49f, 0.76f, 1f), Color.white);
        RuntimeUiFactory.SetCenter(play.GetComponent<RectTransform>(), new Vector2(0f, 35f), new Vector2(500f, 108f));
        play.onClick.AddListener(SceneFlow.ContinueFromHighestUnlockedLevel);

        Button levels = RuntimeUiFactory.CreateButton(contentRoot, "Levels Button", "Level Select", new Color(0.24f, 0.26f, 0.31f, 1f), Color.white);
        RuntimeUiFactory.SetCenter(levels.GetComponent<RectTransform>(), new Vector2(0f, -105f), new Vector2(500f, 108f));
        levels.onClick.AddListener(SceneFlow.LoadLevelSelect);

        Button settings = RuntimeUiFactory.CreateButton(contentRoot, "Settings Button", "Settings", new Color(0.24f, 0.26f, 0.31f, 1f), Color.white);
        RuntimeUiFactory.SetCenter(settings.GetComponent<RectTransform>(), new Vector2(0f, -245f), new Vector2(500f, 108f));
        settings.onClick.AddListener(OpenSettings);
    }

    void BuildLevelSelect()
    {
        CreateCanvas("Level Select Canvas");
        RectTransform root = m_canvas.GetComponent<RectTransform>();
        RuntimeUiFactory.CreatePanel(root, "Background", new Color(0.06f, 0.08f, 0.1f, 1f));
        RectTransform contentRoot = RuntimeUiFactory.CreateSafeAreaRoot(root, "Safe Area Root");

        Text title = RuntimeUiFactory.CreateText(contentRoot, "Title", "Select Level", 62, Color.white, TextAnchor.MiddleCenter);
        RuntimeUiFactory.SetCenter(title.GetComponent<RectTransform>(), new Vector2(0f, 375f), new Vector2(760f, 100f));

        LevelDatabase database = SceneFlow.LevelDatabase;
        if (database == null || database.levels == null || database.levels.Length == 0)
        {
            Text empty = RuntimeUiFactory.CreateText(contentRoot, "Empty State", "No levels found", 38, new Color(0.85f, 0.9f, 0.95f, 1f), TextAnchor.MiddleCenter);
            RuntimeUiFactory.SetCenter(empty.GetComponent<RectTransform>(), Vector2.zero, new Vector2(700f, 100f));
        }
        else
        {
            for (int i = 0; i < database.levels.Length; i++)
            {
                LevelData level = database.levels[i];
                if (level == null)
                {
                    continue;
                }

                int row = i / 3;
                int column = i % 3;
                float x = (column - 1) * 245f;
                float y = 185f - row * 145f;
                bool isUnlocked = PlayerProgress.IsLevelUnlocked(level);
                bool isComplete = PlayerProgress.IsLevelComplete(level.levelId);
                Color buttonColor = isUnlocked ? new Color(0.11f, 0.49f, 0.76f, 1f) : new Color(0.2f, 0.21f, 0.24f, 1f);
                Button levelButton = RuntimeUiFactory.CreateButton(contentRoot, "Level " + level.levelId + " Button", GetLevelButtonLabel(level, isUnlocked, isComplete), buttonColor, Color.white);
                RuntimeUiFactory.SetCenter(levelButton.GetComponent<RectTransform>(), new Vector2(x, y), new Vector2(220f, 130f));
                levelButton.interactable = isUnlocked;
                Text buttonText = levelButton.GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    buttonText.fontSize = 25;
                }

                int levelId = level.levelId;
                levelButton.onClick.AddListener(() => SceneFlow.StartLevel(levelId));
            }
        }

        Button back = RuntimeUiFactory.CreateButton(contentRoot, "Back Button", "Back", new Color(0.24f, 0.26f, 0.31f, 1f), Color.white);
        RuntimeUiFactory.SetCenter(back.GetComponent<RectTransform>(), new Vector2(0f, -430f), new Vector2(360f, 96f));
        back.onClick.AddListener(SceneFlow.LoadMenu);
    }

    void CreateCanvas(string name)
    {
        m_canvas = RuntimeUiFactory.CreateOverlayCanvas(name, 0);
    }

    void OpenSettings()
    {
        RuntimeUiShell.CreateOrFind().ShowSettingsOverlay(null);
    }

    string GetLevelButtonLabel(LevelData level, bool isUnlocked, bool isComplete)
    {
        if (!isUnlocked)
        {
            return level.displayName + "\nLocked";
        }

        int bestScore = PlayerProgress.GetBestScore(level.levelId);
        int bestStars = PlayerProgress.GetBestStars(level.levelId);
        string status = isComplete ? "Stars: " + bestStars + "/3" : "New";
        string score = bestScore > 0 ? "Best: " + bestScore : "Goal: " + level.scoreGoal;
        return level.displayName + "\n" + status + "\n" + score;
    }
}
