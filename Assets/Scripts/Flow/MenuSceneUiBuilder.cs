using UnityEngine;
using UnityEngine.UI;

static class MenuSceneUiBuilder
{
    const float PhoneButtonWidth = 424f;
    const float PhoneButtonHeight = 115f;
    const float TabletButtonWidth = 500f;
    const float TabletButtonHeight = 136f;

    struct MenuLayout
    {
        public bool IsTablet;
        public float LogoY;
        public float SubtitleY;
        public float ButtonStartY;
        public float ButtonGap;
        public float ButtonWidth;
        public float ButtonHeight;
        public float ProgressX;
        public float ProgressY;
        public float NavStartX;
        public float NavGap;
    }

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
        MenuLayout layout = GetLayout();
        CreateScenery(contentRoot, layout);
        CreateTopResourceBar(contentRoot);

        Text logo = RuntimeUiFactory.CreateText(contentRoot, "Logo", "PIXEL\nMATCH", 92, new Color(1f, 0.86f, 0.24f, 1f), TextAnchor.MiddleCenter);
        logo.fontStyle = FontStyle.Bold;
        logo.lineSpacing = 0.78f;
        RuntimeUiFactory.SetCenter(logo.GetComponent<RectTransform>(), new Vector2(0f, layout.LogoY), new Vector2(layout.IsTablet ? 820f : 760f, 218f));
        RuntimeUiFactory.AddMotion(logo.GetComponent<RectTransform>(), RuntimeUiMotionType.Float, 5f, 0.28f);

        Text subtitle = RuntimeUiFactory.CreateText(contentRoot, "Subtitle", "ADVENTURE", 42, Color.white, TextAnchor.MiddleCenter);
        subtitle.fontStyle = FontStyle.Bold;
        RuntimeUiFactory.SetCenter(subtitle.GetComponent<RectTransform>(), new Vector2(0f, layout.SubtitleY), new Vector2(448f, 64f));
        RectTransform subtitlePlank = RuntimeUiFactory.CreatePanel(contentRoot, "Subtitle Plank", new Color(0.34f, 0.16f, 0.06f, 0.96f));
        subtitlePlank.SetSiblingIndex(subtitle.transform.GetSiblingIndex());
        RuntimeUiFactory.SetCenter(subtitlePlank, new Vector2(0f, layout.SubtitleY - 2f), new Vector2(510f, 74f));

        int highestUnlockedLevel = PlayerProgress.Data.highestUnlockedLevelId;
        string playLabel = highestUnlockedLevel > 1 ? "PLAY  CONTINUE " + highestUnlockedLevel : "PLAY";
        ThemeData theme = RuntimeUiFactory.Theme;
        Button play = CreateMenuAction(contentRoot, "Play Button", playLabel, new Vector2(0f, layout.ButtonStartY), RuntimeButtonStyle.Primary, layout, theme != null ? theme.menuPlayButtonSprite : null);
        RuntimeUiFactory.AddMotion(play.GetComponent<RectTransform>(), RuntimeUiMotionType.Pulse, 2f, 0.55f);
        play.onClick.AddListener(SceneFlow.ContinueFromHighestUnlockedLevel);

        Button levels = CreateMenuAction(contentRoot, "Levels Button", "MAP  LEVELS", new Vector2(0f, layout.ButtonStartY - layout.ButtonGap), RuntimeButtonStyle.Primary, layout, theme != null ? theme.menuLevelsButtonSprite : null);
        levels.onClick.AddListener(SceneFlow.LoadLevelSelect);

        CreateMenuAction(contentRoot, "Daily Button", "CHEST  DAILY REWARD", new Vector2(0f, layout.ButtonStartY - layout.ButtonGap * 2f), RuntimeButtonStyle.Secondary, layout, theme != null ? theme.menuDailyRewardButtonSprite : null).interactable = false;
        CreateMenuAction(contentRoot, "Events Button", "TROPHY  EVENTS", new Vector2(0f, layout.ButtonStartY - layout.ButtonGap * 3f), RuntimeButtonStyle.Secondary, layout, theme != null ? theme.menuEventsButtonSprite : null).interactable = false;
        CreateMenuAction(contentRoot, "Shop Button", "SHOP", new Vector2(0f, layout.ButtonStartY - layout.ButtonGap * 4f), RuntimeButtonStyle.Secondary, layout, theme != null ? theme.menuShopButtonSprite : null).interactable = false;

        RectTransform progressSign = RuntimeUiFactory.CreatePanel(contentRoot, "Progress Sign", RuntimePanelStyle.Surface);
        Image progressImage = progressSign.GetComponent<Image>();
        if (progressImage != null)
        {
            progressImage.color = new Color(0.22f, 0.13f, 0.06f, 0.95f);
        }
        RuntimeUiFactory.SetCenter(progressSign, new Vector2(layout.ProgressX, layout.ProgressY), new Vector2(188f, 146f));
        progressSign.localRotation = Quaternion.Euler(0f, 0f, -6f);
        Text progressText = RuntimeUiFactory.CreateText(progressSign, "Progress Text", "Level\n" + highestUnlockedLevel + "\n* 15/30", 28, new Color(0.9f, 1f, 0.35f, 1f), TextAnchor.MiddleCenter);
        progressText.fontStyle = FontStyle.Bold;
        RuntimeUiFactory.Stretch(progressText.GetComponent<RectTransform>(), 8f, 8f, 8f, 8f);

        Button settings = RuntimeUiFactory.CreateButton(contentRoot, "Settings Button", "GEAR", RuntimeButtonStyle.Icon);
        RectTransform settingsRect = settings.GetComponent<RectTransform>();
        settingsRect.anchorMin = new Vector2(1f, 1f);
        settingsRect.anchorMax = new Vector2(1f, 1f);
        settingsRect.pivot = new Vector2(0.5f, 0.5f);
        settingsRect.anchoredPosition = new Vector2(-58f, -56f);
        settingsRect.sizeDelta = new Vector2(76f, 76f);
        settings.onClick.AddListener(OpenSettings);

        CreateBottomNav(contentRoot, layout);
    }

    static void OpenSettings()
    {
        RuntimeUiShell.CreateOrFind().ShowSettingsOverlay(null);
    }

    static MenuLayout GetLayout()
    {
        float aspect = Screen.height > 0 ? (float)Screen.width / Screen.height : 9f / 16f;
        bool isTablet = aspect > 0.64f;

        return new MenuLayout
        {
            IsTablet = isTablet,
            LogoY = isTablet ? 326f : 318f,
            SubtitleY = isTablet ? 168f : 166f,
            ButtonStartY = isTablet ? -18f : 0f,
            ButtonGap = isTablet ? 148f : 130f,
            ButtonWidth = isTablet ? TabletButtonWidth : PhoneButtonWidth,
            ButtonHeight = isTablet ? TabletButtonHeight : PhoneButtonHeight,
            ProgressX = isTablet ? 386f : 356f,
            ProgressY = isTablet ? -458f : -470f,
            NavStartX = isTablet ? -300f : -255f,
            NavGap = isTablet ? 200f : 170f
        };
    }

    static void CreateScenery(Transform parent, MenuLayout layout)
    {
        RectTransform sunset = RuntimeUiFactory.CreatePanel(parent, "Sunset Sky", new Color(0.88f, 0.42f, 0.22f, 0.54f));
        RuntimeUiFactory.SetCenter(sunset, new Vector2(0f, 318f), new Vector2(1180f, 520f));
        DisableRaycast(sunset);

        RectTransform horizon = RuntimeUiFactory.CreatePanel(parent, "Distant Horizon", new Color(0.16f, 0.23f, 0.42f, 0.68f));
        RuntimeUiFactory.SetCenter(horizon, new Vector2(218f, 120f), new Vector2(760f, 172f));
        DisableRaycast(horizon);

        RectTransform sun = RuntimeUiFactory.CreatePanel(parent, "Sun Placeholder", new Color(1f, 0.88f, 0.35f, 0.88f));
        RuntimeUiFactory.SetCenter(sun, new Vector2(330f, 230f), new Vector2(142f, 142f));
        DisableRaycast(sun);

        RectTransform treeCanopy = RuntimeUiFactory.CreatePanel(parent, "Tree Canopy", new Color(0.06f, 0.22f, 0.12f, 0.94f));
        RuntimeUiFactory.SetCenter(treeCanopy, new Vector2(-384f, 500f), new Vector2(424f, 260f));
        DisableRaycast(treeCanopy);

        RectTransform cottage = RuntimeUiFactory.CreatePanel(parent, "Cottage Placeholder", new Color(0.40f, 0.20f, 0.08f, 0.92f));
        RuntimeUiFactory.SetCenter(cottage, new Vector2(-390f, -122f), new Vector2(246f, 420f));
        DisableRaycast(cottage);
        Text cottageText = RuntimeUiFactory.CreateText(cottage, "Window", "[]\nHOME", 26, new Color(1f, 0.78f, 0.30f, 1f), TextAnchor.MiddleCenter);
        cottageText.raycastTarget = false;
        RuntimeUiFactory.Stretch(cottageText.GetComponent<RectTransform>(), 12f, 12f, 12f, 12f);

        RectTransform foreground = RuntimeUiFactory.CreatePanel(parent, "Foreground Garden", new Color(0.08f, 0.32f, 0.14f, 0.82f));
        foreground.anchorMin = new Vector2(0f, 0f);
        foreground.anchorMax = new Vector2(1f, 0f);
        foreground.pivot = new Vector2(0.5f, 0f);
        foreground.anchoredPosition = Vector2.zero;
        foreground.sizeDelta = new Vector2(0f, 530f);
        DisableRaycast(foreground);

        RectTransform path = RuntimeUiFactory.CreatePanel(parent, "Stone Path", new Color(0.54f, 0.40f, 0.24f, 0.7f));
        RuntimeUiFactory.SetCenter(path, new Vector2(0f, -606f), new Vector2(layout.IsTablet ? 520f : 430f, 310f));
        DisableRaycast(path);

        RectTransform crate = RuntimeUiFactory.CreatePanel(parent, "Gem Crate", new Color(0.28f, 0.13f, 0.05f, 0.96f));
        RuntimeUiFactory.SetCenter(crate, new Vector2(-378f, -532f), new Vector2(236f, 160f));
        DisableRaycast(crate);
        Text crateText = RuntimeUiFactory.CreateText(crate, "Crate Label", "GEMS", 28, new Color(0.75f, 0.95f, 1f, 1f), TextAnchor.MiddleCenter);
        crateText.raycastTarget = false;
        RuntimeUiFactory.Stretch(crateText.GetComponent<RectTransform>(), 10f, 10f, 10f, 10f);

        RectTransform mascot = RuntimeUiFactory.CreatePanel(parent, "Mascot Placeholder", new Color(1f, 0.64f, 0.12f, 0.96f));
        RuntimeUiFactory.SetCenter(mascot, new Vector2(-370f, -382f), new Vector2(158f, 132f));
        DisableRaycast(mascot);
        RuntimeUiFactory.AddMotion(mascot, RuntimeUiMotionType.Float, 4f, 0.36f);
        Text mascotText = RuntimeUiFactory.CreateText(mascot, "Mascot Face", ":)", 40, new Color(0.17f, 0.08f, 0.03f, 1f), TextAnchor.MiddleCenter);
        mascotText.raycastTarget = false;
        RuntimeUiFactory.Stretch(mascotText.GetComponent<RectTransform>(), 8f, 8f, 8f, 8f);
    }

    static void CreateTopResourceBar(Transform parent)
    {
        CreateResourceCounter(parent, "Lives", "5", "Full", new Vector2(-352f, -50f), new Color(0.9f, 0.08f, 0.08f, 1f));
        CreateResourceCounter(parent, "Coins", "C", "12,450", new Vector2(-40f, -50f), new Color(1f, 0.75f, 0.08f, 1f));
        CreateResourceCounter(parent, "Stars", "*", "120", new Vector2(274f, -50f), new Color(1f, 0.82f, 0.08f, 1f));
    }

    static void CreateResourceCounter(Transform parent, string name, string icon, string value, Vector2 anchoredPosition, Color iconColor)
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
        counter.sizeDelta = new Vector2(246f, 64f);

        RectTransform iconBacking = RuntimeUiFactory.CreatePanel(counter, "Icon Backing", iconColor);
        RuntimeUiFactory.SetCenter(iconBacking, new Vector2(-94f, 0f), new Vector2(64f, 64f));
        Text iconText = RuntimeUiFactory.CreateText(iconBacking, "Icon", icon, 30, Color.white, TextAnchor.MiddleCenter);
        iconText.fontStyle = FontStyle.Bold;
        RuntimeUiFactory.Stretch(iconText.GetComponent<RectTransform>(), 4f, 4f, 4f, 4f);

        Text valueText = RuntimeUiFactory.CreateText(counter, "Value", value, 28, Color.white, TextAnchor.MiddleLeft);
        valueText.fontStyle = FontStyle.Bold;
        RuntimeUiFactory.SetCenter(valueText.GetComponent<RectTransform>(), new Vector2(26f, 0f), new Vector2(132f, 46f));

        RectTransform plusBacking = RuntimeUiFactory.CreatePanel(counter, "Plus Backing", new Color(0.24f, 0.68f, 0.10f, 1f));
        RuntimeUiFactory.SetCenter(plusBacking, new Vector2(100f, 0f), new Vector2(52f, 52f));
        Text plus = RuntimeUiFactory.CreateText(plusBacking, "Plus", "+", 36, Color.white, TextAnchor.MiddleCenter);
        plus.fontStyle = FontStyle.Bold;
        RuntimeUiFactory.Stretch(plus.GetComponent<RectTransform>(), 0f, 0f, 0f, 4f);
    }

    static Button CreateMenuAction(Transform parent, string name, string label, Vector2 position, RuntimeButtonStyle style, MenuLayout layout, Sprite menuSprite)
    {
        Button button = RuntimeUiFactory.CreateButton(parent, name, label, style);
        RuntimeUiFactory.SetCenter(button.GetComponent<RectTransform>(), position, new Vector2(layout.ButtonWidth, layout.ButtonHeight));
        if (menuSprite != null)
        {
            ApplyMenuActionSprite(button, menuSprite);
        }
        else
        {
            Text text = button.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.fontSize = 32;
                text.fontStyle = FontStyle.Bold;
            }
        }

        RuntimeUiFactory.AddMotion(button.GetComponent<RectTransform>(), RuntimeUiMotionType.SlideFromBottom, 20f, 1f, 0.34f);
        return button;
    }

    static void ApplyMenuActionSprite(Button button, Sprite menuSprite)
    {
        Image image = button.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = menuSprite;
            image.type = Image.Type.Simple;
            image.preserveAspect = false;
            image.color = Color.white;
        }

        Text text = button.GetComponentInChildren<Text>();
        if (text != null)
        {
            text.gameObject.SetActive(false);
        }

        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, 1f, 1f, 0.94f);
        colors.pressedColor = new Color(0.82f, 0.82f, 0.82f, 1f);
        colors.selectedColor = colors.highlightedColor;
        colors.disabledColor = new Color(0.58f, 0.58f, 0.58f, 0.82f);
        button.colors = colors;
    }

    static void CreateBottomNav(Transform parent, MenuLayout layout)
    {
        string[] labels = { "RANK\n*", "BADGE\n@", "MAIL\n[]", "SOCIAL\n++" };
        for (int i = 0; i < labels.Length; i++)
        {
            Button nav = RuntimeUiFactory.CreateButton(parent, "Bottom Nav " + i, labels[i], RuntimeButtonStyle.Secondary);
            RectTransform rect = nav.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(layout.NavStartX + i * layout.NavGap, 54f);
            rect.sizeDelta = new Vector2(layout.IsTablet ? 154f : 138f, 88f);
            nav.interactable = false;
            Text text = nav.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.fontSize = 19;
                text.fontStyle = FontStyle.Bold;
            }
            RuntimeUiFactory.AddMotion(rect, RuntimeUiMotionType.SlideFromBottom, 32f + i * 6f, 1f, 0.42f);
        }
    }

    static void DisableRaycast(RectTransform rectTransform)
    {
        Graphic graphic = rectTransform.GetComponent<Graphic>();
        if (graphic != null)
        {
            graphic.raycastTarget = false;
        }
    }
}
