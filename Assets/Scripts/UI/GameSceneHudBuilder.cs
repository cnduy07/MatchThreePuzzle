using System;
using UnityEngine;
using UnityEngine.UI;

struct GameSceneHudState
{
    public Text MovesText;
    public Text ScoreText;
    public GameObject PauseButtonObject;
}

static class GameSceneHudBuilder
{
    static readonly Color WoodDark = new Color(0.19f, 0.09f, 0.035f, 0.96f);
    static readonly Color WoodMid = new Color(0.42f, 0.2f, 0.075f, 0.96f);
    static readonly Color Parchment = new Color(0.86f, 0.62f, 0.34f, 0.96f);
    static readonly Color Gold = new Color(1f, 0.76f, 0.22f, 1f);
    static readonly Color Cream = new Color(1f, 0.92f, 0.72f, 1f);
    static readonly Color RedBanner = new Color(0.57f, 0.06f, 0.055f, 0.98f);

    public static GameSceneHudState Build(Transform parent, LevelData levelData, int movesLeft, int scoreGoal, Action pauseAction)
    {
        RectTransform levelPanel = CreateLevelPanel(parent, levelData);
        RectTransform goalPanel = CreateGoalPanel(parent, levelData, scoreGoal);
        RectTransform movesPanel = CreateMovesPanel(parent, movesLeft);
        Text scoreText = CreateGameplayThreatPreview(parent, scoreGoal);
        CreateBoosterBar(parent);

        Button pause = CreatePauseButton(parent, pauseAction);
        RuntimeUiFactory.AddMotion(goalPanel, RuntimeUiMotionType.SlideFromTop, 26f, 1f, 0.3f);
        RuntimeUiFactory.AddMotion(levelPanel, RuntimeUiMotionType.SlideFromTop, 22f, 1f, 0.28f);
        RuntimeUiFactory.AddMotion(movesPanel, RuntimeUiMotionType.SlideFromTop, 24f, 1f, 0.32f);
        RuntimeUiFactory.AddMotion(pause.GetComponent<RectTransform>(), RuntimeUiMotionType.SlideFromBottom, 22f, 1f, 0.36f);

        return new GameSceneHudState
        {
            MovesText = movesPanel.Find("Value").GetComponent<Text>(),
            ScoreText = scoreText,
            PauseButtonObject = pause.gameObject
        };
    }

    static RectTransform CreateLevelPanel(Transform parent, LevelData levelData)
    {
        RectTransform panel = CreateCounterPanel(parent, "Level Counter", "LEVEL", levelData != null ? levelData.levelId.ToString() : "1", WoodDark, WoodMid);
        RuntimeUiFactory.SetTopLeft(panel, new Vector2(88f, -70f), new Vector2(150f, 126f));
        return panel;
    }

    static RectTransform CreateGoalPanel(Transform parent, LevelData levelData, int scoreGoal)
    {
        RectTransform panel = RuntimeUiFactory.CreatePanel(parent, "Goal Panel", RuntimePanelStyle.Dialog);
        SetPanelImage(panel, Parchment, true);
        panel.anchorMin = new Vector2(0.5f, 1f);
        panel.anchorMax = new Vector2(0.5f, 1f);
        panel.pivot = new Vector2(0.5f, 0.5f);
        panel.anchoredPosition = new Vector2(-12f, -76f);
        panel.sizeDelta = new Vector2(520f, 122f);

        RectTransform titleCap = CreateSolidImage(panel, "Goal Title Cap", WoodDark);
        RuntimeUiFactory.SetCenter(titleCap, new Vector2(0f, 57f), new Vector2(196f, 50f));
        AddText(titleCap, "Title", "GOAL", 31, Color.white, TextAnchor.MiddleCenter, Vector2.zero, new Vector2(176f, 42f));

        RectTransform leftRivet = CreateSolidImage(panel, "Left Rivet", WoodMid);
        RuntimeUiFactory.SetCenter(leftRivet, new Vector2(-238f, 0f), new Vector2(20f, 84f));
        RectTransform rightRivet = CreateSolidImage(panel, "Right Rivet", WoodMid);
        RuntimeUiFactory.SetCenter(rightRivet, new Vector2(238f, 0f), new Vector2(20f, 84f));

        CreateObjectiveChip(panel, new Vector2(-142f, -7f), GetObjectiveLabel(levelData), scoreGoal.ToString(), Gold);
        CreateObjectiveChip(panel, new Vector2(108f, -7f), "TARGET", "Score", new Color(0.76f, 0.28f, 0.14f, 1f));

        return panel;
    }

    static RectTransform CreateMovesPanel(Transform parent, int movesLeft)
    {
        RectTransform panel = CreateCounterPanel(parent, "Moves Counter", "MOVES", movesLeft.ToString(), RedBanner, new Color(0.78f, 0.12f, 0.08f, 0.98f));
        panel.anchorMin = new Vector2(1f, 1f);
        panel.anchorMax = new Vector2(1f, 1f);
        panel.pivot = new Vector2(0.5f, 0.5f);
        panel.anchoredPosition = new Vector2(-178f, -78f);
        panel.sizeDelta = new Vector2(148f, 138f);

        RectTransform tail = CreateSolidImage(panel, "Ribbon Tail", new Color(0.36f, 0.025f, 0.03f, 0.96f));
        RuntimeUiFactory.SetCenter(tail, new Vector2(0f, -72f), new Vector2(96f, 34f));
        tail.SetAsFirstSibling();
        return panel;
    }

    static RectTransform CreateCounterPanel(Transform parent, string name, string title, string value, Color color, Color insetColor)
    {
        RectTransform panel = RuntimeUiFactory.CreatePanel(parent, name, RuntimePanelStyle.Surface);
        SetPanelImage(panel, color, true);

        RectTransform inset = CreateSolidImage(panel, "Inset", insetColor);
        RuntimeUiFactory.Stretch(inset, 10f, 10f, 10f, 10f);

        Text titleText = RuntimeUiFactory.CreateText(panel, "Title", title, 24, Cream, TextAnchor.MiddleCenter);
        titleText.raycastTarget = false;
        RuntimeUiFactory.SetCenter(titleText.GetComponent<RectTransform>(), new Vector2(0f, 27f), new Vector2(124f, 30f));

        Text valueText = RuntimeUiFactory.CreateText(panel, "Value", value, 52, Color.white, TextAnchor.MiddleCenter);
        valueText.raycastTarget = false;
        valueText.fontStyle = FontStyle.Bold;
        RuntimeUiFactory.SetCenter(valueText.GetComponent<RectTransform>(), new Vector2(0f, -19f), new Vector2(124f, 64f));
        return panel;
    }

    static Text CreateGameplayThreatPreview(Transform parent, int scoreGoal)
    {
        ThemeData theme = RuntimeUiFactory.Theme;
        RectTransform stage = CreateLoseThreatStage(parent, theme);
        stage.name = "Threat Preview";
        stage.anchoredPosition = new Vector2(0f, -206f);
        stage.sizeDelta = new Vector2(820f, 300f);
        RuntimeUiFactory.AddMotion(stage, RuntimeUiMotionType.SlideFromTop, 30f, 1f, 0.35f);

        RectTransform backWall = CreateSolidImage(stage, "Threat Back Wall", new Color(0.065f, 0.055f, 0.06f, 0.52f));
        RuntimeUiFactory.SetCenter(backWall, new Vector2(0f, -14f), new Vector2(780f, 208f));

        RectTransform floorShadow = CreateSolidImage(stage, "Threat Floor Shadow", new Color(0.015f, 0.012f, 0.01f, 0.42f));
        RuntimeUiFactory.SetCenter(floorShadow, new Vector2(10f, -118f), new Vector2(780f, 58f));

        CreateDecorativeWindow(stage);

        RectTransform monster = CreateThreatActor(stage, "Monster Preview", theme != null ? theme.loseThreatMonsterSprite : null, theme != null ? theme.loseThreatMonsterColor : new Color(0.45f, 0.16f, 0.72f, 1f), "MONSTER");
        RuntimeUiFactory.SetCenter(monster, new Vector2(-105f, -42f), new Vector2(220f, 152f));
        RuntimeUiFactory.AddMotion(monster, RuntimeUiMotionType.Float, 4f, 0.4f);

        RectTransform door = CreateThreatActor(stage, "Door Preview", theme != null ? theme.loseThreatDoorSprite : null, theme != null ? theme.loseThreatDoorColor : new Color(0.47f, 0.25f, 0.09f, 1f), "DOOR");
        RuntimeUiFactory.SetCenter(door, new Vector2(252f, -28f), new Vector2(132f, 212f));

        RectTransform scorePlaque = CreateSolidImage(stage, "Score Plaque", new Color(0.16f, 0.075f, 0.035f, 0.92f));
        RuntimeUiFactory.SetCenter(scorePlaque, new Vector2(-304f, 92f), new Vector2(218f, 58f));
        Text score = RuntimeUiFactory.CreateText(scorePlaque, "Score", "Score 0", 25, Gold, TextAnchor.MiddleCenter);
        score.fontStyle = FontStyle.Bold;
        score.raycastTarget = false;
        RuntimeUiFactory.Stretch(score.GetComponent<RectTransform>(), 10f, 4f, 10f, 4f);

        Text scoreGoalText = RuntimeUiFactory.CreateText(stage, "Score Goal Hint", "Goal " + scoreGoal, 19, Cream, TextAnchor.MiddleCenter);
        scoreGoalText.raycastTarget = false;
        RuntimeUiFactory.SetCenter(scoreGoalText.GetComponent<RectTransform>(), new Vector2(-304f, 54f), new Vector2(178f, 28f));
        return score;
    }

    static void CreateBoosterBar(Transform parent)
    {
        string[] names = { "HAMMER", "BOMB", "COLOR", "HAND" };
        string[] icons = { "M", "B", "C", "H" };
        Color[] iconColors =
        {
            new Color(0.77f, 0.77f, 0.72f, 1f),
            new Color(0.18f, 0.2f, 0.24f, 1f),
            new Color(0.65f, 0.24f, 0.9f, 1f),
            new Color(1f, 0.72f, 0.15f, 1f)
        };

        float startX = -294f;
        for (int i = 0; i < names.Length; i++)
        {
            RectTransform rectTransform = RuntimeUiFactory.CreatePanel(parent, "Booster " + (i + 1), RuntimePanelStyle.Surface);
            SetPanelImage(rectTransform, WoodMid, true);
            rectTransform.anchorMin = new Vector2(0.5f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = new Vector2(startX + i * 188f, 84f);
            rectTransform.sizeDelta = new Vector2(128f, 112f);

            RectTransform inset = CreateSolidImage(rectTransform, "Inset", new Color(0.16f, 0.085f, 0.045f, 0.82f));
            RuntimeUiFactory.Stretch(inset, 10f, 10f, 10f, 10f);

            Text icon = AddText(rectTransform, "Icon", icons[i], 45, iconColors[i], TextAnchor.MiddleCenter, new Vector2(0f, 15f), new Vector2(82f, 58f));
            icon.fontStyle = FontStyle.Bold;
            AddText(rectTransform, "Name", names[i], 14, Cream, TextAnchor.MiddleCenter, new Vector2(0f, -32f), new Vector2(100f, 22f));
            CreateBadge(rectTransform, "3");
            RuntimeUiFactory.AddMotion(rectTransform, RuntimeUiMotionType.SlideFromBottom, 28f + i * 8f, 1f, 0.38f);
        }
    }

    static Button CreatePauseButton(Transform parent, Action pauseAction)
    {
        Button pause = RuntimeUiFactory.CreateButton(parent, "Pause Button", "II", RuntimeButtonStyle.Icon);
        RectTransform pauseRect = pause.GetComponent<RectTransform>();
        pauseRect.anchorMin = new Vector2(1f, 0f);
        pauseRect.anchorMax = new Vector2(1f, 0f);
        pauseRect.pivot = new Vector2(0.5f, 0.5f);
        pauseRect.anchoredPosition = new Vector2(-94f, 84f);
        pauseRect.sizeDelta = new Vector2(112f, 112f);
        SetPanelImage(pauseRect, new Color(0.28f, 0.13f, 0.055f, 0.98f), false);

        Text label = pause.GetComponentInChildren<Text>();
        if (label != null)
        {
            label.fontSize = 44;
            label.fontStyle = FontStyle.Bold;
            label.color = Cream;
            label.raycastTarget = false;
        }

        pause.onClick.AddListener(() => pauseAction?.Invoke());
        return pause;
    }

    static RectTransform CreateLoseThreatStage(Transform parent, ThemeData theme)
    {
        GameObject stagePrefab = theme != null ? theme.loseThreatStagePrefab : null;
        GameObject stageObject = null;
        if (stagePrefab != null)
        {
            stageObject = UnityEngine.Object.Instantiate(stagePrefab, parent, false);
            stageObject.name = "Lose Threat Stage";
            if (stageObject.GetComponent<RectTransform>() == null)
            {
                UnityEngine.Object.Destroy(stageObject);
                stageObject = null;
            }
        }

        if (stageObject == null)
        {
            stageObject = new GameObject("Lose Threat Stage", typeof(RectTransform), typeof(CanvasGroup));
            stageObject.transform.SetParent(parent, false);
        }

        RectTransform stage = stageObject.GetComponent<RectTransform>();
        stage.anchorMin = new Vector2(0.5f, 1f);
        stage.anchorMax = new Vector2(0.5f, 1f);
        stage.pivot = new Vector2(0.5f, 1f);
        stage.anchoredPosition = new Vector2(0f, -210f);
        stage.sizeDelta = new Vector2(900f, 300f);
        SetGraphicRaycastTargets(stage, false);
        return stage;
    }

    static RectTransform CreateThreatActor(Transform parent, string name, Sprite sprite, Color color, string fallbackLabel)
    {
        GameObject actorObject = new GameObject(name, typeof(RectTransform), typeof(Image));
        actorObject.transform.SetParent(parent, false);

        Image image = actorObject.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        if (sprite != null)
        {
            image.sprite = sprite;
            image.preserveAspect = true;
            image.color = Color.white;
        }

        if (sprite == null)
        {
            Text label = RuntimeUiFactory.CreateText(actorObject.transform, "Label", fallbackLabel, 26, Color.white, TextAnchor.MiddleCenter);
            label.raycastTarget = false;
            label.fontStyle = FontStyle.Bold;
            RuntimeUiFactory.Stretch(label.GetComponent<RectTransform>(), 8f, 4f, 8f, 4f);
        }

        return actorObject.GetComponent<RectTransform>();
    }

    static void CreateObjectiveChip(Transform parent, Vector2 position, string iconLabel, string value, Color iconColor)
    {
        RectTransform chip = CreateSolidImage(parent, "Goal Chip", new Color(0.25f, 0.12f, 0.045f, 0.34f));
        RuntimeUiFactory.SetCenter(chip, position, new Vector2(220f, 62f));

        RectTransform icon = CreateSolidImage(chip, "Icon", iconColor);
        RuntimeUiFactory.SetCenter(icon, new Vector2(-68f, 0f), new Vector2(54f, 46f));
        AddText(icon, "Icon Label", iconLabel, iconLabel.Length > 3 ? 15 : 21, Color.white, TextAnchor.MiddleCenter, Vector2.zero, new Vector2(48f, 40f));

        Text valueText = AddText(chip, "Value", value, 26, Color.white, TextAnchor.MiddleLeft, new Vector2(30f, 0f), new Vector2(128f, 42f));
        valueText.fontStyle = FontStyle.Bold;
    }

    static void CreateDecorativeWindow(Transform parent)
    {
        RectTransform window = CreateSolidImage(parent, "Window", new Color(0.035f, 0.075f, 0.12f, 0.92f));
        RuntimeUiFactory.SetCenter(window, new Vector2(-314f, -34f), new Vector2(86f, 94f));

        RectTransform crossVertical = CreateSolidImage(window, "Window Vertical", new Color(0.12f, 0.075f, 0.045f, 1f));
        RuntimeUiFactory.SetCenter(crossVertical, Vector2.zero, new Vector2(10f, 84f));
        RectTransform crossHorizontal = CreateSolidImage(window, "Window Horizontal", new Color(0.12f, 0.075f, 0.045f, 1f));
        RuntimeUiFactory.SetCenter(crossHorizontal, Vector2.zero, new Vector2(76f, 10f));
    }

    static void CreateBadge(Transform parent, string value)
    {
        RectTransform badge = CreateSolidImage(parent, "Count Badge", RedBanner);
        RuntimeUiFactory.SetCenter(badge, new Vector2(46f, -38f), new Vector2(42f, 42f));
        Text text = AddText(badge, "Value", value, 26, Color.white, TextAnchor.MiddleCenter, Vector2.zero, new Vector2(36f, 34f));
        text.fontStyle = FontStyle.Bold;
    }

    static RectTransform CreateSolidImage(Transform parent, string name, Color color)
    {
        GameObject imageObject = new GameObject(name, typeof(RectTransform), typeof(Image));
        imageObject.transform.SetParent(parent, false);

        Image image = imageObject.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        return imageObject.GetComponent<RectTransform>();
    }

    static Text AddText(Transform parent, string name, string value, int size, Color color, TextAnchor alignment, Vector2 position, Vector2 rectSize)
    {
        Text text = RuntimeUiFactory.CreateText(parent, name, value, size, color, alignment);
        text.raycastTarget = false;
        RuntimeUiFactory.SetCenter(text.GetComponent<RectTransform>(), position, rectSize);
        return text;
    }

    static void SetPanelImage(RectTransform rectTransform, Color color, bool disableRaycast)
    {
        Image image = rectTransform.GetComponent<Image>();
        if (image == null)
        {
            return;
        }

        image.color = color;
        if (disableRaycast)
        {
            image.raycastTarget = false;
        }
    }

    static void SetGraphicRaycastTargets(Transform root, bool value)
    {
        Graphic[] graphics = root.GetComponentsInChildren<Graphic>(true);
        for (int i = 0; i < graphics.Length; i++)
        {
            graphics[i].raycastTarget = value;
        }
    }

    static string GetObjectiveLabel(LevelData levelData)
    {
        if (levelData == null)
        {
            return "PTS";
        }

        switch (levelData.objectiveType)
        {
            case LevelObjectiveType.CollectiblesToBottom:
                return "KEY";
            case LevelObjectiveType.BreakTiles:
                return "TILE";
            case LevelObjectiveType.ClearBlockers:
                return "BOX";
            case LevelObjectiveType.Mixed:
                return "MIX";
            default:
                return "PTS";
        }
    }
}
