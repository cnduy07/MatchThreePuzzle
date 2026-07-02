using UnityEngine;
using UnityEngine.UI;

static class MenuSceneUiBuilder
{
    public static void Build()
    {
        Canvas canvas = RuntimeUiFactory.CreateOverlayCanvas("Menu Canvas", 0);
        RectTransform root = canvas.GetComponent<RectTransform>();
        RectTransform background = RuntimeUiFactory.CreatePanel(root, "Concept Background", RuntimePanelStyle.Background);
        Image backgroundImage = background.GetComponent<Image>();
        if (backgroundImage != null)
        {
            backgroundImage.color = new Color(0.14f, 0.38f, 0.34f, 1f);
        }

        RectTransform contentRoot = RuntimeUiFactory.CreateSafeAreaRoot(root, "Safe Area Root");
        CreateTopResourceBar(contentRoot);

        RectTransform skyBand = RuntimeUiFactory.CreatePanel(contentRoot, "Sunset Band", new Color(0.9f, 0.48f, 0.22f, 0.38f));
        RuntimeUiFactory.SetCenter(skyBand, new Vector2(0f, 330f), new Vector2(1020f, 360f));

        Text logo = RuntimeUiFactory.CreateText(contentRoot, "Logo", "PIXEL\nMATCH", 92, new Color(1f, 0.86f, 0.24f, 1f), TextAnchor.MiddleCenter);
        RuntimeUiFactory.SetCenter(logo.GetComponent<RectTransform>(), new Vector2(0f, 318f), new Vector2(760f, 210f));
        RuntimeUiFactory.AddMotion(logo.GetComponent<RectTransform>(), RuntimeUiMotionType.Float, 5f, 0.28f);

        Text subtitle = RuntimeUiFactory.CreateText(contentRoot, "Subtitle", "ADVENTURE", 42, Color.white, TextAnchor.MiddleCenter);
        RuntimeUiFactory.SetCenter(subtitle.GetComponent<RectTransform>(), new Vector2(0f, 172f), new Vector2(420f, 64f));

        int highestUnlockedLevel = PlayerProgress.Data.highestUnlockedLevelId;
        string playLabel = highestUnlockedLevel > 1 ? "Continue Level " + highestUnlockedLevel : "Play";
        Button play = CreateMenuAction(contentRoot, "Play Button", ">  " + playLabel.ToUpperInvariant(), new Vector2(0f, 12f), RuntimeButtonStyle.Primary);
        RuntimeUiFactory.AddMotion(play.GetComponent<RectTransform>(), RuntimeUiMotionType.Pulse, 2f, 0.55f);
        play.onClick.AddListener(SceneFlow.ContinueFromHighestUnlockedLevel);

        Button levels = CreateMenuAction(contentRoot, "Levels Button", "MAP  LEVELS", new Vector2(0f, -116f), RuntimeButtonStyle.Primary);
        levels.onClick.AddListener(SceneFlow.LoadLevelSelect);

        CreateMenuAction(contentRoot, "Daily Button", "CHEST  DAILY REWARD", new Vector2(0f, -244f), RuntimeButtonStyle.Secondary).interactable = false;
        CreateMenuAction(contentRoot, "Events Button", "TROPHY  EVENTS", new Vector2(0f, -372f), RuntimeButtonStyle.Secondary).interactable = false;
        CreateMenuAction(contentRoot, "Shop Button", "SHOP", new Vector2(0f, -500f), RuntimeButtonStyle.Secondary).interactable = false;

        RectTransform progressSign = RuntimeUiFactory.CreatePanel(contentRoot, "Progress Sign", RuntimePanelStyle.Surface);
        Image progressImage = progressSign.GetComponent<Image>();
        if (progressImage != null)
        {
            progressImage.color = new Color(0.22f, 0.13f, 0.06f, 0.95f);
        }
        RuntimeUiFactory.SetCenter(progressSign, new Vector2(344f, -466f), new Vector2(178f, 132f));
        Text progressText = RuntimeUiFactory.CreateText(progressSign, "Progress Text", "Level\n" + highestUnlockedLevel + "\n* 15/30", 28, new Color(0.9f, 1f, 0.35f, 1f), TextAnchor.MiddleCenter);
        RuntimeUiFactory.Stretch(progressText.GetComponent<RectTransform>(), 8f, 8f, 8f, 8f);

        Button settings = RuntimeUiFactory.CreateButton(contentRoot, "Settings Button", "GEAR", RuntimeButtonStyle.Icon);
        RectTransform settingsRect = settings.GetComponent<RectTransform>();
        settingsRect.anchorMin = new Vector2(1f, 1f);
        settingsRect.anchorMax = new Vector2(1f, 1f);
        settingsRect.pivot = new Vector2(0.5f, 0.5f);
        settingsRect.anchoredPosition = new Vector2(-58f, -54f);
        settingsRect.sizeDelta = new Vector2(76f, 76f);
        settings.onClick.AddListener(OpenSettings);

        CreateBottomNav(contentRoot);
    }

    static void OpenSettings()
    {
        RuntimeUiShell.CreateOrFind().ShowSettingsOverlay(null);
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

    static Button CreateMenuAction(Transform parent, string name, string label, Vector2 position, RuntimeButtonStyle style)
    {
        Button button = RuntimeUiFactory.CreateButton(parent, name, label, style);
        RuntimeUiFactory.SetCenter(button.GetComponent<RectTransform>(), position, new Vector2(560f, 96f));
        Text text = button.GetComponentInChildren<Text>();
        if (text != null)
        {
            text.fontSize = 32;
        }

        return button;
    }

    static void CreateBottomNav(Transform parent)
    {
        string[] labels = { "RANK", "BADGE", "MAIL", "SOCIAL" };
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
}
