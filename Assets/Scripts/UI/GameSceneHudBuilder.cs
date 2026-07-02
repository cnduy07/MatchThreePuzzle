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
    public static GameSceneHudState Build(Transform parent, LevelData levelData, int movesLeft, int scoreGoal, Action pauseAction)
    {
        RectTransform levelPanel = CreateCounterPanel(parent, "Level Counter", "LEVEL", levelData != null ? levelData.levelId.ToString() : "1", new Color(0.25f, 0.13f, 0.06f, 0.94f));
        RuntimeUiFactory.SetTopLeft(levelPanel, new Vector2(88f, -64f), new Vector2(144f, 104f));

        RectTransform goalPanel = RuntimeUiFactory.CreatePanel(parent, "Goal Panel", RuntimePanelStyle.Dialog);
        goalPanel.anchorMin = new Vector2(0.5f, 1f);
        goalPanel.anchorMax = new Vector2(0.5f, 1f);
        goalPanel.pivot = new Vector2(0.5f, 0.5f);
        goalPanel.anchoredPosition = new Vector2(0f, -66f);
        goalPanel.sizeDelta = new Vector2(460f, 104f);
        RuntimeUiFactory.CreateText(goalPanel, "Goal Title", "GOAL", 34, Color.white, TextAnchor.MiddleCenter);
        RuntimeUiFactory.SetCenter(goalPanel.Find("Goal Title").GetComponent<RectTransform>(), new Vector2(0f, 28f), new Vector2(220f, 36f));
        Text goalText = RuntimeUiFactory.CreateText(goalPanel, "Goal Text", "Score " + scoreGoal, 28, new Color(1f, 0.9f, 0.52f, 1f), TextAnchor.MiddleCenter);
        RuntimeUiFactory.SetCenter(goalText.GetComponent<RectTransform>(), new Vector2(0f, -16f), new Vector2(390f, 48f));

        RectTransform movesPanel = CreateCounterPanel(parent, "Moves Counter", "MOVES", movesLeft.ToString(), new Color(0.55f, 0.08f, 0.07f, 0.96f));
        movesPanel.anchorMin = new Vector2(1f, 1f);
        movesPanel.anchorMax = new Vector2(1f, 1f);
        movesPanel.pivot = new Vector2(0.5f, 0.5f);
        movesPanel.anchoredPosition = new Vector2(-178f, -64f);
        movesPanel.sizeDelta = new Vector2(144f, 104f);

        Button pause = RuntimeUiFactory.CreateButton(parent, "Pause Button", "||", RuntimeButtonStyle.Icon);
        RectTransform pauseRect = pause.GetComponent<RectTransform>();
        pauseRect.anchorMin = new Vector2(1f, 1f);
        pauseRect.anchorMax = new Vector2(1f, 1f);
        pauseRect.pivot = new Vector2(0.5f, 0.5f);
        pauseRect.anchoredPosition = new Vector2(-58f, -64f);
        pauseRect.sizeDelta = new Vector2(76f, 76f);
        pause.onClick.AddListener(() => pauseAction?.Invoke());

        Text scoreText = CreateGameplayThreatPreview(parent);
        CreateBoosterBar(parent);
        RuntimeUiFactory.AddMotion(goalPanel, RuntimeUiMotionType.SlideFromTop, 26f, 1f, 0.3f);
        RuntimeUiFactory.AddMotion(levelPanel, RuntimeUiMotionType.SlideFromTop, 22f, 1f, 0.28f);
        RuntimeUiFactory.AddMotion(movesPanel, RuntimeUiMotionType.SlideFromTop, 24f, 1f, 0.32f);

        return new GameSceneHudState
        {
            MovesText = movesPanel.Find("Value").GetComponent<Text>(),
            ScoreText = scoreText,
            PauseButtonObject = pause.gameObject
        };
    }

    static RectTransform CreateCounterPanel(Transform parent, string name, string title, string value, Color color)
    {
        RectTransform panel = RuntimeUiFactory.CreatePanel(parent, name, RuntimePanelStyle.Surface);
        Image image = panel.GetComponent<Image>();
        if (image != null)
        {
            image.color = color;
        }

        Text titleText = RuntimeUiFactory.CreateText(panel, "Title", title, 25, Color.white, TextAnchor.MiddleCenter);
        RuntimeUiFactory.SetCenter(titleText.GetComponent<RectTransform>(), new Vector2(0f, 24f), new Vector2(124f, 32f));

        Text valueText = RuntimeUiFactory.CreateText(panel, "Value", value, 46, Color.white, TextAnchor.MiddleCenter);
        RuntimeUiFactory.SetCenter(valueText.GetComponent<RectTransform>(), new Vector2(0f, -16f), new Vector2(124f, 58f));
        return panel;
    }

    static Text CreateGameplayThreatPreview(Transform parent)
    {
        ThemeData theme = RuntimeUiFactory.Theme;
        RectTransform stage = CreateLoseThreatStage(parent, theme);
        stage.name = "Threat Preview";
        stage.anchoredPosition = new Vector2(0f, -204f);
        stage.sizeDelta = new Vector2(760f, 230f);
        RuntimeUiFactory.AddMotion(stage, RuntimeUiMotionType.SlideFromTop, 30f, 1f, 0.35f);

        RectTransform monster = CreateThreatActor(stage, "Monster Preview", theme != null ? theme.loseThreatMonsterSprite : null, theme != null ? theme.loseThreatMonsterColor : new Color(0.45f, 0.16f, 0.72f, 1f), "MONSTER");
        RuntimeUiFactory.SetCenter(monster, new Vector2(-130f, -8f), new Vector2(180f, 126f));
        RuntimeUiFactory.AddMotion(monster, RuntimeUiMotionType.Float, 4f, 0.4f);

        RectTransform door = CreateThreatActor(stage, "Door Preview", theme != null ? theme.loseThreatDoorSprite : null, theme != null ? theme.loseThreatDoorColor : new Color(0.47f, 0.25f, 0.09f, 1f), "DOOR");
        RuntimeUiFactory.SetCenter(door, new Vector2(240f, 0f), new Vector2(124f, 184f));

        Text score = RuntimeUiFactory.CreateText(stage, "Score", "Score 0", 28, new Color(1f, 0.9f, 0.52f, 1f), TextAnchor.MiddleLeft);
        RuntimeUiFactory.SetCenter(score.GetComponent<RectTransform>(), new Vector2(-326f, 78f), new Vector2(180f, 44f));
        return score;
    }

    static void CreateBoosterBar(Transform parent)
    {
        string[] labels = { "HAMMER\n3", "BOMB\n3", "COLOR\n3", "HAND\n3" };
        float startX = -300f;
        for (int i = 0; i < labels.Length; i++)
        {
            Button booster = RuntimeUiFactory.CreateButton(parent, "Booster " + (i + 1), labels[i], RuntimeButtonStyle.Secondary);
            RectTransform rectTransform = booster.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = new Vector2(startX + i * 200f, 76f);
            rectTransform.sizeDelta = new Vector2(150f, 108f);
            booster.interactable = false;
            Text label = booster.GetComponentInChildren<Text>();
            if (label != null)
            {
                label.fontSize = 21;
            }
            RuntimeUiFactory.AddMotion(rectTransform, RuntimeUiMotionType.SlideFromBottom, 28f + i * 8f, 1f, 0.38f);
        }
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
        return stage;
    }

    static RectTransform CreateThreatActor(Transform parent, string name, Sprite sprite, Color color, string fallbackLabel)
    {
        GameObject actorObject = new GameObject(name, typeof(RectTransform), typeof(Image));
        actorObject.transform.SetParent(parent, false);

        Image image = actorObject.GetComponent<Image>();
        image.color = color;
        if (sprite != null)
        {
            image.sprite = sprite;
            image.preserveAspect = true;
            image.color = Color.white;
        }

        if (sprite == null)
        {
            Text label = RuntimeUiFactory.CreateText(actorObject.transform, "Label", fallbackLabel, 26, Color.white, TextAnchor.MiddleCenter);
            RuntimeUiFactory.Stretch(label.GetComponent<RectTransform>(), 8f, 4f, 8f, 4f);
        }

        return actorObject.GetComponent<RectTransform>();
    }
}
