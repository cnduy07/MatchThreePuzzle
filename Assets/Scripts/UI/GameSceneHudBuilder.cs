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
    static readonly Color NightPanel = new Color(0.035f, 0.025f, 0.10f, 0.62f);
    static readonly Color BoosterInset = new Color(0.12f, 0.065f, 0.04f, 0.94f);

    public static GameSceneHudState Build(Transform parent, LevelData levelData, int movesLeft, int scoreGoal, Action pauseAction)
    {
        ThemeData theme = RuntimeUiFactory.Theme;
        RectTransform levelPanel = CreateLevelPanel(parent, levelData);
        RectTransform goalPanel = CreateGoalPanel(parent, levelData, scoreGoal);
        RectTransform movesPanel = CreateMovesPanel(parent, movesLeft);
        Text scoreText = CreateGameplayThreatPreview(parent, scoreGoal);
        CreateBoosterBar(parent, theme);

        Button pause = CreatePauseButton(parent, theme, pauseAction);
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
        RuntimeUiFactory.SetTopLeft(panel, new Vector2(88f, -64f), new Vector2(136f, 104f));
        return panel;
    }

    static RectTransform CreateGoalPanel(Transform parent, LevelData levelData, int scoreGoal)
    {
        RectTransform panel = RuntimeUiFactory.CreatePanel(parent, "Goal Panel", RuntimePanelStyle.Dialog);
        SetPanelImage(panel, Parchment, true);
        panel.anchorMin = new Vector2(0.5f, 1f);
        panel.anchorMax = new Vector2(0.5f, 1f);
        panel.pivot = new Vector2(0.5f, 0.5f);
        panel.anchoredPosition = new Vector2(-12f, -70f);
        panel.sizeDelta = new Vector2(500f, 98f);

        RectTransform titleCap = CreateSolidImage(panel, "Goal Title Cap", WoodDark);
        RuntimeUiFactory.SetCenter(titleCap, new Vector2(0f, 47f), new Vector2(176f, 40f));
        AddText(titleCap, "Title", "GOAL", 27, Color.white, TextAnchor.MiddleCenter, Vector2.zero, new Vector2(156f, 34f));

        RectTransform leftRivet = CreateSolidImage(panel, "Left Rivet", WoodMid);
        RuntimeUiFactory.SetCenter(leftRivet, new Vector2(-226f, 0f), new Vector2(18f, 66f));
        RectTransform rightRivet = CreateSolidImage(panel, "Right Rivet", WoodMid);
        RuntimeUiFactory.SetCenter(rightRivet, new Vector2(226f, 0f), new Vector2(18f, 66f));

        CreateObjectiveChip(panel, new Vector2(-132f, -5f), GetObjectiveLabel(levelData), scoreGoal.ToString(), Gold);
        CreateObjectiveChip(panel, new Vector2(112f, -5f), "TARGET", "Score", new Color(0.76f, 0.28f, 0.14f, 1f));

        return panel;
    }

    static RectTransform CreateMovesPanel(Transform parent, int movesLeft)
    {
        RectTransform panel = CreateCounterPanel(parent, "Moves Counter", "MOVES", movesLeft.ToString(), RedBanner, new Color(0.78f, 0.12f, 0.08f, 0.98f));
        panel.anchorMin = new Vector2(1f, 1f);
        panel.anchorMax = new Vector2(1f, 1f);
        panel.pivot = new Vector2(0.5f, 0.5f);
        panel.anchoredPosition = new Vector2(-166f, -70f);
        panel.sizeDelta = new Vector2(130f, 112f);

        RectTransform tail = CreateSolidImage(panel, "Ribbon Tail", new Color(0.36f, 0.025f, 0.03f, 0.96f));
        RuntimeUiFactory.SetCenter(tail, new Vector2(0f, -58f), new Vector2(84f, 26f));
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
        RuntimeUiFactory.SetCenter(titleText.GetComponent<RectTransform>(), new Vector2(0f, 23f), new Vector2(116f, 28f));

        Text valueText = RuntimeUiFactory.CreateText(panel, "Value", value, 44, Color.white, TextAnchor.MiddleCenter);
        valueText.raycastTarget = false;
        valueText.fontStyle = FontStyle.Bold;
        RuntimeUiFactory.SetCenter(valueText.GetComponent<RectTransform>(), new Vector2(0f, -15f), new Vector2(116f, 56f));
        return panel;
    }

    static Text CreateGameplayThreatPreview(Transform parent, int scoreGoal)
    {
        ThemeData theme = RuntimeUiFactory.Theme;
        RectTransform stage = CreateLoseThreatStage(parent, theme);
        stage.name = "Threat Preview";
        stage.anchoredPosition = new Vector2(0f, -182f);
        stage.sizeDelta = new Vector2(760f, 210f);
        RuntimeUiFactory.AddMotion(stage, RuntimeUiMotionType.SlideFromTop, 30f, 1f, 0.35f);

        RectTransform backWall = CreateSolidImage(stage, "Threat Back Wall", NightPanel);
        RuntimeUiFactory.SetCenter(backWall, new Vector2(0f, -4f), new Vector2(720f, 154f));

        RectTransform floorShadow = CreateSolidImage(stage, "Threat Floor Shadow", new Color(0.015f, 0.012f, 0.04f, 0.55f));
        RuntimeUiFactory.SetCenter(floorShadow, new Vector2(10f, -86f), new Vector2(706f, 34f));

        CreateDecorativeWindow(stage);

        RectTransform monster = CreateThreatActor(stage, "Monster Preview", theme != null ? theme.loseThreatMonsterSprite : null, theme != null ? theme.loseThreatMonsterColor : new Color(0.45f, 0.16f, 0.72f, 1f), "MONSTER");
        RuntimeUiFactory.SetCenter(monster, new Vector2(-86f, -34f), new Vector2(150f, 108f));
        RuntimeUiFactory.AddMotion(monster, RuntimeUiMotionType.Float, 4f, 0.4f);

        RectTransform door = CreateThreatActor(stage, "Door Preview", theme != null ? theme.loseThreatDoorSprite : null, theme != null ? theme.loseThreatDoorColor : new Color(0.47f, 0.25f, 0.09f, 1f), "DOOR");
        RuntimeUiFactory.SetCenter(door, new Vector2(226f, -28f), new Vector2(92f, 142f));

        RectTransform scorePlaque = CreateSolidImage(stage, "Score Plaque", new Color(0.16f, 0.075f, 0.035f, 0.92f));
        RuntimeUiFactory.SetCenter(scorePlaque, new Vector2(-278f, 56f), new Vector2(178f, 42f));
        Text score = RuntimeUiFactory.CreateText(scorePlaque, "Score", "Score 0", 21, Gold, TextAnchor.MiddleCenter);
        score.fontStyle = FontStyle.Bold;
        score.raycastTarget = false;
        RuntimeUiFactory.Stretch(score.GetComponent<RectTransform>(), 10f, 4f, 10f, 4f);

        Text scoreGoalText = RuntimeUiFactory.CreateText(stage, "Score Goal Hint", "Goal " + scoreGoal, 16, Cream, TextAnchor.MiddleCenter);
        scoreGoalText.raycastTarget = false;
        RuntimeUiFactory.SetCenter(scoreGoalText.GetComponent<RectTransform>(), new Vector2(-278f, 24f), new Vector2(150f, 24f));
        return score;
    }

    static void CreateBoosterBar(Transform parent, ThemeData theme)
    {
        string[] names = { "HAMMER", "BOMB", "COLOR", "HAND" };
        string[] icons = { "M", "B", "C", "H" };
        Sprite[] iconSprites =
        {
            theme != null ? theme.boosterHammerIconSprite : null,
            theme != null ? theme.boosterBombIconSprite : null,
            theme != null ? theme.boosterColorIconSprite : null,
            theme != null ? theme.boosterHandIconSprite : null
        };

        Color[] iconColors =
        {
            new Color(0.77f, 0.77f, 0.72f, 1f),
            new Color(0.18f, 0.2f, 0.24f, 1f),
            new Color(0.65f, 0.24f, 0.9f, 1f),
            new Color(1f, 0.72f, 0.15f, 1f)
        };

        float startX = -330f;
        for (int i = 0; i < names.Length; i++)
        {
            RectTransform rectTransform = RuntimeUiFactory.CreatePanel(parent, "Booster " + (i + 1), RuntimePanelStyle.Surface);
            SetPanelImage(rectTransform, WoodMid, true);
            rectTransform.anchorMin = new Vector2(0.5f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = new Vector2(startX + i * 190f, 96f);
            rectTransform.sizeDelta = new Vector2(138f, 122f);

            RectTransform bevel = CreateSolidImage(rectTransform, "Top Bevel", new Color(0.62f, 0.32f, 0.12f, 0.88f));
            RuntimeUiFactory.SetCenter(bevel, new Vector2(0f, 45f), new Vector2(104f, 12f));

            RectTransform inset = CreateSolidImage(rectTransform, "Inset", BoosterInset);
            RuntimeUiFactory.Stretch(inset, 12f, 12f, 12f, 12f);

            if (iconSprites[i] != null)
            {
                RectTransform icon = CreateSpriteImage(rectTransform, "Icon Sprite", iconSprites[i]);
                RuntimeUiFactory.SetCenter(icon, new Vector2(0f, 10f), new Vector2(78f, 78f));
            }
            else
            {
                Text icon = AddText(rectTransform, "Icon", icons[i], 52, iconColors[i], TextAnchor.MiddleCenter, new Vector2(0f, 10f), new Vector2(92f, 72f));
                icon.fontStyle = FontStyle.Bold;
            }

            CreateBadge(rectTransform, "3");
            RuntimeUiFactory.AddMotion(rectTransform, RuntimeUiMotionType.SlideFromBottom, 28f + i * 8f, 1f, 0.38f);
        }
    }

    static Button CreatePauseButton(Transform parent, ThemeData theme, Action pauseAction)
    {
        Button pause = RuntimeUiFactory.CreateButton(parent, "Pause Button", "II", RuntimeButtonStyle.Icon);
        RectTransform pauseRect = pause.GetComponent<RectTransform>();
        pauseRect.anchorMin = new Vector2(1f, 0f);
        pauseRect.anchorMax = new Vector2(1f, 0f);
        pauseRect.pivot = new Vector2(0.5f, 0.5f);
        pauseRect.anchoredPosition = new Vector2(-110f, 96f);
        pauseRect.sizeDelta = new Vector2(118f, 118f);
        SetPanelImage(pauseRect, new Color(0.18f, 0.09f, 0.04f, 0.98f), false);

        RectTransform pauseInset = CreateSolidImage(pauseRect, "Inset", new Color(0.055f, 0.045f, 0.055f, 0.96f));
        RuntimeUiFactory.Stretch(pauseInset, 18f, 18f, 18f, 18f);

        Text label = pause.GetComponentInChildren<Text>();
        if (label != null)
        {
            label.fontSize = 36;
            label.fontStyle = FontStyle.Bold;
            label.color = Cream;
            label.raycastTarget = false;
            label.gameObject.SetActive(theme == null || theme.pauseIconSprite == null);
        }

        if (theme != null && theme.pauseIconSprite != null)
        {
            RectTransform icon = CreateSpriteImage(pauseRect, "Pause Icon Sprite", theme.pauseIconSprite);
            RuntimeUiFactory.SetCenter(icon, Vector2.zero, new Vector2(56f, 56f));
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
        stage.anchoredPosition = new Vector2(0f, -184f);
        stage.sizeDelta = new Vector2(760f, 210f);
        SetGraphicRaycastTargets(stage, false);
        return stage;
    }

    static RectTransform CreateThreatActor(Transform parent, string name, Sprite sprite, Color color, string fallbackLabel)
    {
        GameObject actorObject = new GameObject(name, typeof(RectTransform), typeof(Image));
        actorObject.transform.SetParent(parent, false);

        Image image = actorObject.GetComponent<Image>();
        image.color = new Color(color.r, color.g, color.b, sprite != null ? 1f : 0.72f);
        image.raycastTarget = false;
        if (sprite != null)
        {
            image.sprite = sprite;
            image.preserveAspect = true;
            image.color = Color.white;
        }

        if (sprite == null)
        {
            Text label = RuntimeUiFactory.CreateText(actorObject.transform, "Label", fallbackLabel, 24, Color.white, TextAnchor.MiddleCenter);
            label.raycastTarget = false;
            label.fontStyle = FontStyle.Bold;
            label.horizontalOverflow = HorizontalWrapMode.Overflow;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            RuntimeUiFactory.Stretch(label.GetComponent<RectTransform>(), 8f, 4f, 8f, 4f);
        }

        return actorObject.GetComponent<RectTransform>();
    }

    static void CreateObjectiveChip(Transform parent, Vector2 position, string iconLabel, string value, Color iconColor)
    {
        RectTransform chip = CreateSolidImage(parent, "Goal Chip", new Color(0.25f, 0.12f, 0.045f, 0.34f));
        RuntimeUiFactory.SetCenter(chip, position, new Vector2(210f, 50f));

        RectTransform icon = CreateSolidImage(chip, "Icon", iconColor);
        RuntimeUiFactory.SetCenter(icon, new Vector2(-66f, 0f), new Vector2(48f, 38f));
        AddText(icon, "Icon Label", iconLabel, iconLabel.Length > 3 ? 12 : 18, Color.white, TextAnchor.MiddleCenter, Vector2.zero, new Vector2(42f, 34f));

        Text valueText = AddText(chip, "Value", value, 22, Color.white, TextAnchor.MiddleLeft, new Vector2(28f, 0f), new Vector2(120f, 36f));
        valueText.fontStyle = FontStyle.Bold;
    }

    static void CreateDecorativeWindow(Transform parent)
    {
        RectTransform window = CreateSolidImage(parent, "Window", new Color(0.035f, 0.075f, 0.12f, 0.92f));
        RuntimeUiFactory.SetCenter(window, new Vector2(-280f, -28f), new Vector2(62f, 66f));

        RectTransform crossVertical = CreateSolidImage(window, "Window Vertical", new Color(0.12f, 0.075f, 0.045f, 1f));
        RuntimeUiFactory.SetCenter(crossVertical, Vector2.zero, new Vector2(7f, 58f));
        RectTransform crossHorizontal = CreateSolidImage(window, "Window Horizontal", new Color(0.12f, 0.075f, 0.045f, 1f));
        RuntimeUiFactory.SetCenter(crossHorizontal, Vector2.zero, new Vector2(54f, 7f));
    }

    static void CreateBadge(Transform parent, string value)
    {
        RectTransform badge = CreateSolidImage(parent, "Count Badge", RedBanner);
        RuntimeUiFactory.SetCenter(badge, new Vector2(50f, -44f), new Vector2(42f, 42f));
        Text text = AddText(badge, "Value", value, 25, Color.white, TextAnchor.MiddleCenter, Vector2.zero, new Vector2(36f, 34f));
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

    static RectTransform CreateSpriteImage(Transform parent, string name, Sprite sprite)
    {
        RectTransform rectTransform = CreateSolidImage(parent, name, Color.white);
        Image image = rectTransform.GetComponent<Image>();
        image.sprite = sprite;
        image.type = Image.Type.Simple;
        image.preserveAspect = true;
        image.raycastTarget = false;
        return rectTransform;
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

    static void SetOptionalSprite(RectTransform rectTransform, Sprite sprite, Color color, bool preserveAspect)
    {
        if (sprite == null)
        {
            return;
        }

        Image image = rectTransform.GetComponent<Image>();
        if (image == null)
        {
            return;
        }

        image.sprite = sprite;
        image.type = Image.Type.Simple;
        image.preserveAspect = preserveAspect;
        image.color = color;
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
