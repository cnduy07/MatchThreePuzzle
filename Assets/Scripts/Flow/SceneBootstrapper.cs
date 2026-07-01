using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneBootstrapper : MonoBehaviour
{
    Canvas m_canvas;

    void Start()
    {
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

        Text title = RuntimeUiFactory.CreateText(root, "Title", "Match Three Puzzle", 70, Color.white, TextAnchor.MiddleCenter);
        RuntimeUiFactory.SetCenter(title.GetComponent<RectTransform>(), new Vector2(0f, 330f), new Vector2(840f, 130f));

        Text subtitle = RuntimeUiFactory.CreateText(root, "Subtitle", "Level goals, bombs, cascades, and collectible drops", 32, new Color(0.78f, 0.84f, 0.9f, 1f), TextAnchor.MiddleCenter);
        RuntimeUiFactory.SetCenter(subtitle.GetComponent<RectTransform>(), new Vector2(0f, 225f), new Vector2(820f, 90f));

        Button play = RuntimeUiFactory.CreateButton(root, "Play Button", "Play", new Color(0.11f, 0.49f, 0.76f, 1f), Color.white);
        RuntimeUiFactory.SetCenter(play.GetComponent<RectTransform>(), new Vector2(0f, 35f), new Vector2(500f, 108f));
        play.onClick.AddListener(() => SceneFlow.StartLevel(1));

        Button levels = RuntimeUiFactory.CreateButton(root, "Levels Button", "Level Select", new Color(0.24f, 0.26f, 0.31f, 1f), Color.white);
        RuntimeUiFactory.SetCenter(levels.GetComponent<RectTransform>(), new Vector2(0f, -105f), new Vector2(500f, 108f));
        levels.onClick.AddListener(SceneFlow.LoadLevelSelect);
    }

    void BuildLevelSelect()
    {
        CreateCanvas("Level Select Canvas");
        RectTransform root = m_canvas.GetComponent<RectTransform>();
        RuntimeUiFactory.CreatePanel(root, "Background", new Color(0.06f, 0.08f, 0.1f, 1f));

        Text title = RuntimeUiFactory.CreateText(root, "Title", "Select Level", 62, Color.white, TextAnchor.MiddleCenter);
        RuntimeUiFactory.SetCenter(title.GetComponent<RectTransform>(), new Vector2(0f, 375f), new Vector2(760f, 100f));

        LevelDatabase database = SceneFlow.LevelDatabase;
        if (database == null || database.levels == null || database.levels.Length == 0)
        {
            Text empty = RuntimeUiFactory.CreateText(root, "Empty State", "No levels found", 38, new Color(0.85f, 0.9f, 0.95f, 1f), TextAnchor.MiddleCenter);
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
                Button levelButton = RuntimeUiFactory.CreateButton(root, "Level " + level.levelId + " Button", level.displayName, new Color(0.11f, 0.49f, 0.76f, 1f), Color.white);
                RuntimeUiFactory.SetCenter(levelButton.GetComponent<RectTransform>(), new Vector2(x, y), new Vector2(220f, 110f));
                int levelId = level.levelId;
                levelButton.onClick.AddListener(() => SceneFlow.StartLevel(levelId));
            }
        }

        Button back = RuntimeUiFactory.CreateButton(root, "Back Button", "Back", new Color(0.24f, 0.26f, 0.31f, 1f), Color.white);
        RuntimeUiFactory.SetCenter(back.GetComponent<RectTransform>(), new Vector2(0f, -430f), new Vector2(360f, 96f));
        back.onClick.AddListener(SceneFlow.LoadMenu);
    }

    void CreateCanvas(string name)
    {
        m_canvas = RuntimeUiFactory.CreateOverlayCanvas(name, 0);
    }
}
