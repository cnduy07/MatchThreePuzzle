using UnityEngine;
using UnityEngine.UI;

static class LevelMapSceneUiBuilder
{
    public static void Build()
    {
        Canvas canvas = RuntimeUiFactory.CreateOverlayCanvas("Level Select Canvas", 0);
        RectTransform root = canvas.GetComponent<RectTransform>();
        RectTransform background = RuntimeUiFactory.CreatePanel(root, "Map Background", RuntimePanelStyle.Background);
        Image backgroundImage = background.GetComponent<Image>();
        if (backgroundImage != null)
        {
            backgroundImage.color = new Color(0.11f, 0.42f, 0.32f, 1f);
        }

        RectTransform contentRoot = RuntimeUiFactory.CreateSafeAreaRoot(root, "Safe Area Root");
        CreateTopResourceBar(contentRoot);

        CreateMapRegionLabel(contentRoot, "Forest World", "FOREST\nWORLD\n* 15/30", new Vector2(-310f, 482f), new Color(0.22f, 0.13f, 0.06f, 0.96f));
        CreateMapRegionLabel(contentRoot, "Castle World", "CASTLE\nWORLD\nLOCKED", new Vector2(302f, 92f), new Color(0.26f, 0.12f, 0.42f, 0.96f));
        CreateMapRegionLabel(contentRoot, "Ice World", "ICE\nWORLD\nLOCKED", new Vector2(310f, -282f), new Color(0.10f, 0.25f, 0.38f, 0.96f));

        Vector2[] path = {
            new Vector2(-220f, -500f),
            new Vector2(-70f, -390f),
            new Vector2(36f, -272f),
            new Vector2(-96f, -142f),
            new Vector2(-248f, -8f),
            new Vector2(-52f, 124f),
            new Vector2(114f, 242f),
            new Vector2(-20f, 366f),
            new Vector2(186f, 486f)
        };

        for (int i = 0; i < path.Length - 1; i++)
        {
            CreatePathSegment(contentRoot, "Path " + i, path[i], path[i + 1]);
        }

        LevelDatabase database = SceneFlow.LevelDatabase;
        if (database == null || database.levels == null || database.levels.Length == 0)
        {
            Text empty = RuntimeUiFactory.CreateText(contentRoot, "Empty State", "No levels found", 38, new Color(0.85f, 0.9f, 0.95f, 1f), TextAnchor.MiddleCenter);
            RuntimeUiFactory.SetCenter(empty.GetComponent<RectTransform>(), Vector2.zero, new Vector2(700f, 100f));
        }
        else
        {
            int nodeCount = Mathf.Min(database.levels.Length, path.Length);
            for (int i = 0; i < nodeCount; i++)
            {
                LevelData level = database.levels[i];
                if (level == null)
                {
                    continue;
                }

                bool isUnlocked = PlayerProgress.IsLevelUnlocked(level);
                bool isComplete = PlayerProgress.IsLevelComplete(level.levelId);
                Button levelButton = CreateLevelNode(contentRoot, level, path[i], isUnlocked, isComplete);

                int levelId = level.levelId;
                levelButton.onClick.AddListener(() => SceneFlow.StartLevel(levelId));
            }
        }

        Button back = RuntimeUiFactory.CreateButton(contentRoot, "Back Button", "<", RuntimeButtonStyle.Secondary);
        RuntimeUiFactory.SetCenter(back.GetComponent<RectTransform>(), new Vector2(-410f, -754f), new Vector2(124f, 92f));
        back.onClick.AddListener(SceneFlow.LoadMenu);
        CreateBottomNav(contentRoot);
    }

    static void CreateTopResourceBar(Transform parent)
    {
        CreateResourceCounter(parent, "Lives", "HEART", "5 Full", new Vector2(-338f, -50f));
        CreateResourceCounter(parent, "Coins", "COIN", "12,450", new Vector2(0f, -50f));
        CreateResourceCounter(parent, "Stars", "STAR", "120", new Vector2(338f, -50f));
    }

    static void CreateResourceCounter(Transform parent, string name, string icon, string value, Vector2 anchoredPosition)
    {
        RectTransform counter = RuntimeUiFactory.CreatePanel(parent, name + " Counter", RuntimePanelStyle.Surface);
        Image image = counter.GetComponent<Image>();
        if (image != null)
        {
            image.color = new Color(0.22f, 0.13f, 0.06f, 0.95f);
        }

        counter.anchorMin = new Vector2(0.5f, 1f);
        counter.anchorMax = new Vector2(0.5f, 1f);
        counter.pivot = new Vector2(0.5f, 0.5f);
        counter.anchoredPosition = anchoredPosition;
        counter.sizeDelta = new Vector2(260f, 64f);

        Text iconText = RuntimeUiFactory.CreateText(counter, "Icon", icon, 22, new Color(1f, 0.85f, 0.25f, 1f), TextAnchor.MiddleCenter);
        RuntimeUiFactory.SetCenter(iconText.GetComponent<RectTransform>(), new Vector2(-88f, 0f), new Vector2(64f, 46f));
        Text valueText = RuntimeUiFactory.CreateText(counter, "Value", value, 28, Color.white, TextAnchor.MiddleLeft);
        RuntimeUiFactory.SetCenter(valueText.GetComponent<RectTransform>(), new Vector2(50f, 0f), new Vector2(154f, 46f));
    }

    static void CreateBottomNav(Transform parent)
    {
        string[] labels = { "SHOP", "DAILY", "EVENTS", "QUESTS" };
        float startX = -255f;
        for (int i = 0; i < labels.Length; i++)
        {
            Button nav = RuntimeUiFactory.CreateButton(parent, "Bottom Nav " + labels[i], labels[i], RuntimeButtonStyle.Secondary);
            RectTransform rect = nav.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(startX + i * 170f, 54f);
            rect.sizeDelta = new Vector2(138f, 88f);
            nav.interactable = false;
            Text text = nav.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.fontSize = 20;
            }
            RuntimeUiFactory.AddMotion(rect, RuntimeUiMotionType.SlideFromBottom, 32f + i * 6f, 1f, 0.42f);
        }
    }

    static void CreateMapRegionLabel(Transform parent, string name, string label, Vector2 position, Color color)
    {
        RectTransform panel = RuntimeUiFactory.CreatePanel(parent, name, RuntimePanelStyle.Surface);
        Image image = panel.GetComponent<Image>();
        if (image != null)
        {
            image.color = color;
        }

        RuntimeUiFactory.SetCenter(panel, position, new Vector2(230f, 150f));
        Text text = RuntimeUiFactory.CreateText(panel, "Label", label, 30, Color.white, TextAnchor.MiddleCenter);
        RuntimeUiFactory.Stretch(text.GetComponent<RectTransform>(), 10f, 10f, 10f, 10f);
    }

    static void CreatePathSegment(Transform parent, string name, Vector2 start, Vector2 end)
    {
        RectTransform path = RuntimeUiFactory.CreatePanel(parent, name, new Color(0.76f, 0.56f, 0.24f, 0.65f));
        Vector2 midpoint = (start + end) * 0.5f;
        Vector2 delta = end - start;
        RuntimeUiFactory.SetCenter(path, midpoint, new Vector2(delta.magnitude + 24f, 30f));
        path.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
    }

    static Button CreateLevelNode(Transform parent, LevelData level, Vector2 position, bool isUnlocked, bool isComplete)
    {
        Button button = RuntimeUiFactory.CreateButton(parent, "Level " + level.levelId + " Node", level.levelId + "\n" + GetStarLabel(level, isComplete), isUnlocked ? RuntimeButtonStyle.Primary : RuntimeButtonStyle.Secondary);
        RuntimeUiFactory.SetCenter(button.GetComponent<RectTransform>(), position, new Vector2(124f, 96f));
        button.interactable = isUnlocked;

        Image image = button.GetComponent<Image>();
        if (image != null && !isUnlocked)
        {
            image.color = new Color(0.18f, 0.18f, 0.2f, 0.9f);
        }

        Text text = button.GetComponentInChildren<Text>();
        if (text != null)
        {
            text.fontSize = 28;
        }

        if (isUnlocked)
        {
            RuntimeUiFactory.AddMotion(button.GetComponent<RectTransform>(), RuntimeUiMotionType.Pulse, 1.2f, 0.4f + level.levelId * 0.03f);
        }

        return button;
    }

    static string GetStarLabel(LevelData level, bool isComplete)
    {
        int stars = PlayerProgress.GetBestStars(level.levelId);
        if (!isComplete || stars <= 0)
        {
            return "* * *";
        }

        return new string('*', Mathf.Clamp(stars, 1, 3));
    }
}
