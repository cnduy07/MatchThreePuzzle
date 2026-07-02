using UnityEngine;
using UnityEngine.UI;

static class LevelMapSceneUiBuilder
{
    const int TotalMapNodes = 15;

    static readonly Vector2[] s_mapPath = {
        new Vector2(-210f, -660f),
        new Vector2(-4f, -590f),
        new Vector2(-62f, -472f),
        new Vector2(-244f, -350f),
        new Vector2(-122f, -228f),
        new Vector2(46f, -114f),
        new Vector2(-190f, 10f),
        new Vector2(-96f, 146f),
        new Vector2(132f, 254f),
        new Vector2(10f, 374f),
        new Vector2(232f, 456f),
        new Vector2(-52f, 560f),
        new Vector2(84f, 676f),
        new Vector2(268f, 612f),
        new Vector2(12f, 748f)
    };

    public static void Build()
    {
        Canvas canvas = RuntimeUiFactory.CreateOverlayCanvas("Level Select Canvas", 0);
        RectTransform root = canvas.GetComponent<RectTransform>();
        RectTransform background = RuntimeUiFactory.CreatePanel(root, "Map Background", RuntimePanelStyle.Background);
        Image backgroundImage = background.GetComponent<Image>();
        if (backgroundImage != null)
        {
            backgroundImage.color = new Color(0.08f, 0.33f, 0.42f, 1f);
        }

        RectTransform contentRoot = RuntimeUiFactory.CreateSafeAreaRoot(root, "Safe Area Root");
        CreateMapBackdrop(contentRoot);

        LevelDatabase database = SceneFlow.LevelDatabase;
        int earnedStars = GetEarnedStars(database);
        int possibleStars = Mathf.Max(TotalMapNodes * 3, GetLevelCount(database) * 3);

        CreateMapRegionLabel(contentRoot, "Forest World", "FOREST\nWORLD", "* " + earnedStars + "/" + possibleStars, new Vector2(-326f, 484f), new Vector2(292f, 176f), new Color(0.24f, 0.13f, 0.05f, 0.96f), new Color(0.54f, 0.94f, 0.20f, 1f));
        CreateMapRegionLabel(contentRoot, "Castle World", "CASTLE\nWORLD", "LOCKED", new Vector2(324f, 44f), new Vector2(254f, 152f), new Color(0.29f, 0.10f, 0.42f, 0.96f), new Color(0.82f, 0.47f, 1f, 1f));
        CreateMapRegionLabel(contentRoot, "Ice World", "ICE\nWORLD", "LOCKED", new Vector2(316f, -368f), new Vector2(238f, 144f), new Color(0.08f, 0.22f, 0.36f, 0.96f), new Color(0.44f, 0.83f, 1f, 1f));

        for (int i = 0; i < s_mapPath.Length - 1; i++)
        {
            CreatePathSegment(contentRoot, "Path Shadow " + i, s_mapPath[i] + new Vector2(0f, -8f), s_mapPath[i + 1] + new Vector2(0f, -8f), new Color(0.18f, 0.10f, 0.04f, 0.58f), 52f);
            CreatePathSegment(contentRoot, "Path " + i, s_mapPath[i], s_mapPath[i + 1], new Color(0.79f, 0.60f, 0.28f, 0.88f), 38f);
        }

        if (database == null || database.levels == null || database.levels.Length == 0)
        {
            Text empty = RuntimeUiFactory.CreateText(contentRoot, "Empty State", "No levels found", 38, new Color(0.85f, 0.9f, 0.95f, 1f), TextAnchor.MiddleCenter);
            RuntimeUiFactory.SetCenter(empty.GetComponent<RectTransform>(), Vector2.zero, new Vector2(700f, 100f));
        }
        else
        {
            int nodeCount = Mathf.Min(TotalMapNodes, s_mapPath.Length);
            for (int i = 0; i < nodeCount; i++)
            {
                LevelData level = GetLevelAt(database, i);
                bool hasLevel = level != null;
                bool isUnlocked = hasLevel && PlayerProgress.IsLevelUnlocked(level);
                bool isComplete = hasLevel && PlayerProgress.IsLevelComplete(level.levelId);

                Button levelButton = CreateLevelNode(contentRoot, level, i + 1, s_mapPath[i], isUnlocked, isComplete, hasLevel);

                if (hasLevel)
                {
                    int levelId = level.levelId;
                    levelButton.onClick.AddListener(() => SceneFlow.StartLevel(levelId));
                }
            }
        }

        CreateTopResourceBar(contentRoot);
        CreateBottomNav(contentRoot);
    }

    static void CreateMapBackdrop(Transform parent)
    {
        RectTransform sky = RuntimeUiFactory.CreatePanel(parent, "Sky Band", new Color(0.18f, 0.55f, 0.82f, 0.55f));
        sky.anchorMin = new Vector2(0f, 0.62f);
        sky.anchorMax = new Vector2(1f, 1f);
        sky.offsetMin = Vector2.zero;
        sky.offsetMax = Vector2.zero;

        RectTransform forest = RuntimeUiFactory.CreatePanel(parent, "Forest Land Mass", new Color(0.16f, 0.49f, 0.20f, 0.92f));
        RuntimeUiFactory.SetCenter(forest, new Vector2(-172f, 42f), new Vector2(780f, 1380f));
        forest.localRotation = Quaternion.Euler(0f, 0f, -13f);

        RectTransform river = RuntimeUiFactory.CreatePanel(parent, "River Channel", new Color(0.05f, 0.38f, 0.62f, 0.82f));
        RuntimeUiFactory.SetCenter(river, new Vector2(150f, -132f), new Vector2(214f, 1300f));
        river.localRotation = Quaternion.Euler(0f, 0f, -22f);

        RectTransform castle = RuntimeUiFactory.CreatePanel(parent, "Castle Plateau", new Color(0.37f, 0.25f, 0.31f, 0.76f));
        RuntimeUiFactory.SetCenter(castle, new Vector2(330f, -20f), new Vector2(360f, 330f));

        RectTransform ice = RuntimeUiFactory.CreatePanel(parent, "Ice Region", new Color(0.56f, 0.82f, 0.95f, 0.76f));
        RuntimeUiFactory.SetCenter(ice, new Vector2(336f, -552f), new Vector2(380f, 460f));

        CreateSceneryBlock(parent, "Forest Cottage", new Vector2(-338f, -522f), new Vector2(184f, 116f), new Color(0.18f, 0.10f, 0.05f, 0.86f), new Color(0.10f, 0.22f, 0.36f, 0.96f));
        CreateSceneryBlock(parent, "Castle Landmark", new Vector2(334f, -138f), new Vector2(190f, 112f), new Color(0.26f, 0.16f, 0.26f, 0.86f), new Color(0.65f, 0.13f, 0.13f, 0.96f));
        CreateSceneryBlock(parent, "Sky Balloon", new Vector2(-392f, -80f), new Vector2(136f, 94f), new Color(0.52f, 0.20f, 0.62f, 0.84f), new Color(0.92f, 0.66f, 0.16f, 0.9f));
    }

    static void CreateTopResourceBar(Transform parent)
    {
        RectTransform topRail = RuntimeUiFactory.CreatePanel(parent, "Top Rail", new Color(0.05f, 0.07f, 0.08f, 0.18f));
        topRail.anchorMin = new Vector2(0f, 1f);
        topRail.anchorMax = new Vector2(1f, 1f);
        topRail.pivot = new Vector2(0.5f, 1f);
        topRail.anchoredPosition = Vector2.zero;
        topRail.sizeDelta = new Vector2(0f, 108f);

        CreateResourceCounter(parent, "Lives", "HEART", "5 Full", new Vector2(142f, -54f), new Vector2(252f, 72f), TextAnchor.MiddleLeft, new Vector2(0f, 1f));
        CreateResourceCounter(parent, "Coins", "COIN", PlayerProgress.Data.coinCount.ToString(), new Vector2(0f, -54f), new Vector2(296f, 72f), TextAnchor.MiddleCenter, new Vector2(0.5f, 1f));
        CreateResourceCounter(parent, "Stars", "STAR", GetEarnedStars(SceneFlow.LevelDatabase).ToString(), new Vector2(-176f, -54f), new Vector2(188f, 72f), TextAnchor.MiddleRight, new Vector2(1f, 1f));
    }

    static void CreateResourceCounter(Transform parent, string name, string icon, string value, Vector2 anchoredPosition, Vector2 size, TextAnchor valueAnchor, Vector2 anchor)
    {
        RectTransform counter = RuntimeUiFactory.CreatePanel(parent, name + " Counter", RuntimePanelStyle.Surface);
        Image image = counter.GetComponent<Image>();
        if (image != null)
        {
            image.color = new Color(0.21f, 0.12f, 0.06f, 0.96f);
        }

        counter.anchorMin = anchor;
        counter.anchorMax = anchor;
        counter.pivot = new Vector2(0.5f, 0.5f);
        counter.anchoredPosition = anchoredPosition;
        counter.sizeDelta = size;

        Text iconText = RuntimeUiFactory.CreateText(counter, "Icon", icon, 20, new Color(1f, 0.82f, 0.18f, 1f), TextAnchor.MiddleCenter);
        RuntimeUiFactory.SetCenter(iconText.GetComponent<RectTransform>(), new Vector2(-size.x * 0.32f, 0f), new Vector2(74f, 48f));
        Text valueText = RuntimeUiFactory.CreateText(counter, "Value", value, 29, Color.white, valueAnchor);
        RuntimeUiFactory.SetCenter(valueText.GetComponent<RectTransform>(), new Vector2(size.x * 0.12f, 0f), new Vector2(size.x - 100f, 48f));
        RuntimeUiFactory.AddMotion(counter, RuntimeUiMotionType.SlideFromTop, 20f, 1f, 0.28f);
    }

    static void CreateBottomNav(Transform parent)
    {
        RectTransform dock = RuntimeUiFactory.CreatePanel(parent, "Bottom Dock", new Color(0.12f, 0.07f, 0.04f, 0.88f));
        dock.anchorMin = new Vector2(0f, 0f);
        dock.anchorMax = new Vector2(1f, 0f);
        dock.pivot = new Vector2(0.5f, 0f);
        dock.anchoredPosition = Vector2.zero;
        dock.sizeDelta = new Vector2(0f, 156f);

        Button back = CreateBottomButton(parent, "Back Button", "<", new Vector2(-422f, 70f), RuntimeButtonStyle.Secondary);
        back.onClick.AddListener(SceneFlow.LoadMenu);

        string[] labels = { "SHOP", "DAILY", "EVENTS", "QUESTS" };
        float startX = -210f;
        for (int i = 0; i < labels.Length; i++)
        {
            Button nav = CreateBottomButton(parent, "Bottom Nav " + labels[i], labels[i], new Vector2(startX + i * 210f, 70f), RuntimeButtonStyle.Secondary);
            nav.interactable = false;
        }
    }

    static Button CreateBottomButton(Transform parent, string name, string label, Vector2 position, RuntimeButtonStyle style)
    {
        Button button = RuntimeUiFactory.CreateButton(parent, name, label, style);
        RectTransform rect = button.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(160f, 106f);

        Text text = button.GetComponentInChildren<Text>();
        if (text != null)
        {
            text.fontSize = label.Length <= 2 ? 42 : 22;
        }

        RuntimeUiFactory.AddMotion(rect, RuntimeUiMotionType.SlideFromBottom, 32f + Mathf.Abs(position.x) * 0.03f, 1f, 0.42f);
        return button;
    }

    static void CreateMapRegionLabel(Transform parent, string name, string title, string progress, Vector2 position, Vector2 size, Color color, Color titleColor)
    {
        RectTransform panel = RuntimeUiFactory.CreatePanel(parent, name, RuntimePanelStyle.Surface);
        Image image = panel.GetComponent<Image>();
        if (image != null)
        {
            image.color = color;
        }

        RuntimeUiFactory.SetCenter(panel, position, size);
        Text titleText = RuntimeUiFactory.CreateText(panel, "Title", title, 34, titleColor, TextAnchor.MiddleCenter);
        RuntimeUiFactory.SetCenter(titleText.GetComponent<RectTransform>(), new Vector2(0f, 26f), new Vector2(size.x - 28f, 78f));
        Text progressText = RuntimeUiFactory.CreateText(panel, "Progress", progress, 28, Color.white, TextAnchor.MiddleCenter);
        RuntimeUiFactory.SetCenter(progressText.GetComponent<RectTransform>(), new Vector2(0f, -52f), new Vector2(size.x - 32f, 42f));
        RuntimeUiFactory.AddMotion(panel, RuntimeUiMotionType.Float, 3f, 0.18f);
    }

    static void CreatePathSegment(Transform parent, string name, Vector2 start, Vector2 end, Color color, float thickness)
    {
        RectTransform path = RuntimeUiFactory.CreatePanel(parent, name, color);
        Vector2 midpoint = (start + end) * 0.5f;
        Vector2 delta = end - start;
        RuntimeUiFactory.SetCenter(path, midpoint, new Vector2(delta.magnitude + thickness, thickness));
        path.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
    }

    static Button CreateLevelNode(Transform parent, LevelData level, int mapNumber, Vector2 position, bool isUnlocked, bool isComplete, bool hasLevel)
    {
        RectTransform shadow = RuntimeUiFactory.CreatePanel(parent, "Level " + mapNumber + " Shadow", new Color(0.07f, 0.06f, 0.05f, 0.64f));
        RuntimeUiFactory.SetCenter(shadow, position + new Vector2(0f, -18f), new Vector2(116f, 62f));

        string label = hasLevel ? level.levelId.ToString() : mapNumber.ToString();
        Button button = RuntimeUiFactory.CreateButton(parent, "Level " + mapNumber + " Node", label, isUnlocked ? RuntimeButtonStyle.Primary : RuntimeButtonStyle.Secondary);
        RuntimeUiFactory.SetCenter(button.GetComponent<RectTransform>(), position, new Vector2(104f, 104f));
        button.interactable = isUnlocked && hasLevel;

        Image image = button.GetComponent<Image>();
        if (image != null)
        {
            image.color = isUnlocked ? new Color(0.25f, 0.65f, 0.10f, 1f) : new Color(0.18f, 0.18f, 0.2f, 0.92f);
        }

        Text text = button.GetComponentInChildren<Text>();
        if (text != null)
        {
            text.fontSize = isUnlocked ? 42 : 32;
        }

        Text stars = RuntimeUiFactory.CreateText(parent, "Level " + mapNumber + " Stars", GetStarLabel(level, isComplete, hasLevel), 28, isUnlocked || isComplete ? new Color(1f, 0.78f, 0.12f, 1f) : new Color(0.54f, 0.54f, 0.58f, 0.9f), TextAnchor.MiddleCenter);
        RuntimeUiFactory.SetCenter(stars.GetComponent<RectTransform>(), position + new Vector2(0f, -66f), new Vector2(148f, 40f));

        if (isUnlocked)
        {
            RuntimeUiFactory.AddMotion(button.GetComponent<RectTransform>(), RuntimeUiMotionType.Pulse, 1.2f, 0.4f + mapNumber * 0.025f);
        }

        return button;
    }

    static string GetStarLabel(LevelData level, bool isComplete, bool hasLevel)
    {
        if (!hasLevel || level == null)
        {
            return "* * *";
        }

        int stars = PlayerProgress.GetBestStars(level.levelId);
        if (!isComplete || stars <= 0)
        {
            return "* * *";
        }

        switch (Mathf.Clamp(stars, 1, 3))
        {
            case 1:
                return "*";
            case 2:
                return "* *";
            default:
                return "* * *";
        }
    }

    static void CreateSceneryBlock(Transform parent, string name, Vector2 position, Vector2 size, Color bodyColor, Color accentColor)
    {
        RectTransform panel = RuntimeUiFactory.CreatePanel(parent, name, bodyColor);
        RuntimeUiFactory.SetCenter(panel, position, size);

        RectTransform roof = RuntimeUiFactory.CreatePanel(panel, "Accent", accentColor);
        RuntimeUiFactory.SetCenter(roof, new Vector2(0f, size.y * 0.24f), new Vector2(size.x * 0.78f, size.y * 0.28f));

        RectTransform light = RuntimeUiFactory.CreatePanel(panel, "Light", new Color(1f, 0.78f, 0.24f, 0.86f));
        RuntimeUiFactory.SetCenter(light, new Vector2(size.x * 0.22f, -size.y * 0.08f), new Vector2(size.x * 0.18f, size.y * 0.26f));
    }

    static LevelData GetLevelAt(LevelDatabase database, int index)
    {
        if (database == null || database.levels == null || index < 0 || index >= database.levels.Length)
        {
            return null;
        }

        return database.levels[index];
    }

    static int GetLevelCount(LevelDatabase database)
    {
        return database != null && database.levels != null ? database.levels.Length : 0;
    }

    static int GetEarnedStars(LevelDatabase database)
    {
        if (database == null || database.levels == null)
        {
            return 0;
        }

        int stars = 0;
        for (int i = 0; i < database.levels.Length; i++)
        {
            LevelData level = database.levels[i];
            if (level != null)
            {
                stars += Mathf.Clamp(PlayerProgress.GetBestStars(level.levelId), 0, 3);
            }
        }

        return stars;
    }
}
