using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum RuntimeUiMotionType
{
    None,
    Float,
    Pulse,
    SlideFromBottom,
    SlideFromTop
}

public class RuntimeUiMotion : MonoBehaviour
{
    public RuntimeUiMotionType motionType = RuntimeUiMotionType.None;
    public float amplitude = 8f;
    public float frequency = 1f;
    public float introDuration = 0.35f;

    RectTransform m_rectTransform;
    Vector2 m_basePosition;
    Vector3 m_baseScale;
    float m_startTime;

    void Awake()
    {
        CaptureBaseState();
    }

    public void Configure(RuntimeUiMotionType type, float motionAmplitude, float motionFrequency, float motionIntroDuration)
    {
        motionType = type;
        amplitude = motionAmplitude;
        frequency = motionFrequency;
        introDuration = motionIntroDuration;
        CaptureBaseState();
    }

    void CaptureBaseState()
    {
        m_rectTransform = GetComponent<RectTransform>();
        if (m_rectTransform == null)
        {
            enabled = false;
            return;
        }

        m_basePosition = m_rectTransform.anchoredPosition;
        m_baseScale = transform.localScale;
        m_startTime = Time.unscaledTime;

        if (motionType == RuntimeUiMotionType.SlideFromBottom)
        {
            m_rectTransform.anchoredPosition = m_basePosition + new Vector2(0f, -amplitude);
        }
        else if (motionType == RuntimeUiMotionType.SlideFromTop)
        {
            m_rectTransform.anchoredPosition = m_basePosition + new Vector2(0f, amplitude);
        }
    }

    void Update()
    {
        float elapsed = Time.unscaledTime - m_startTime;
        float introT = introDuration > 0f ? Mathf.Clamp01(elapsed / introDuration) : 1f;
        float easedIntro = introT * introT * (3f - 2f * introT);

        switch (motionType)
        {
            case RuntimeUiMotionType.Float:
                m_rectTransform.anchoredPosition = m_basePosition + new Vector2(0f, Mathf.Sin(elapsed * frequency * Mathf.PI * 2f) * amplitude);
                break;
            case RuntimeUiMotionType.Pulse:
                float pulse = 1f + Mathf.Sin(elapsed * frequency * Mathf.PI * 2f) * amplitude * 0.01f;
                transform.localScale = m_baseScale * pulse;
                break;
            case RuntimeUiMotionType.SlideFromBottom:
                m_rectTransform.anchoredPosition = Vector2.LerpUnclamped(m_basePosition + new Vector2(0f, -amplitude), m_basePosition, easedIntro);
                break;
            case RuntimeUiMotionType.SlideFromTop:
                m_rectTransform.anchoredPosition = Vector2.LerpUnclamped(m_basePosition + new Vector2(0f, amplitude), m_basePosition, easedIntro);
                break;
        }
    }
}

public static class RuntimeUiFactory
{
    static Font s_defaultFont;
    static ThemeData s_themeData;

    public static Font DefaultFont
    {
        get
        {
            if (s_defaultFont == null)
            {
                s_defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }

            if (s_defaultFont == null)
            {
                s_defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }

            return s_defaultFont;
        }
    }

    public static ThemeData Theme
    {
        get
        {
            if (s_themeData == null)
            {
                s_themeData = GameResourceLibrary.ResolveTheme(null);
            }

            return s_themeData;
        }
        set
        {
            s_themeData = value;
        }
    }

    public static Canvas CreateOverlayCanvas(string name, int sortingOrder)
    {
        EnsureEventSystem();

        GameObject canvasObject = new GameObject(name, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortingOrder;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        return canvas;
    }

    public static void EnsureEventSystem()
    {
        EventSystem[] eventSystems = Object.FindObjectsByType<EventSystem>(FindObjectsInactive.Exclude);
        if (eventSystems.Length == 0)
        {
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            return;
        }

        EventSystem activeEventSystem = GetPreferredEventSystem(eventSystems);
        for (int i = 0; i < eventSystems.Length; i++)
        {
            if (eventSystems[i] != activeEventSystem)
            {
                Object.Destroy(eventSystems[i].gameObject);
            }
        }

        if (activeEventSystem.GetComponent<BaseInputModule>() == null)
        {
            activeEventSystem.gameObject.AddComponent<StandaloneInputModule>();
        }

        EventSystem.current = activeEventSystem;
    }

    static EventSystem GetPreferredEventSystem(EventSystem[] eventSystems)
    {
        Scene activeScene = SceneManager.GetActiveScene();
        for (int i = 0; i < eventSystems.Length; i++)
        {
            if (eventSystems[i].gameObject.scene == activeScene)
            {
                return eventSystems[i];
            }
        }

        return EventSystem.current != null ? EventSystem.current : eventSystems[0];
    }

    public static RectTransform CreatePanel(Transform parent, string name, Color color)
    {
        GameObject panelObject = new GameObject(name, typeof(RectTransform), typeof(Image));
        panelObject.transform.SetParent(parent, false);

        RectTransform rectTransform = panelObject.GetComponent<RectTransform>();
        Stretch(rectTransform);

        Image image = panelObject.GetComponent<Image>();
        image.color = color;

        return rectTransform;
    }

    public static RectTransform CreatePanel(Transform parent, string name, RuntimePanelStyle style)
    {
        ThemeData theme = Theme;
        GameObject panelPrefab = theme != null ? theme.GetPanelPrefab(style) : null;
        Color color = theme != null ? theme.GetPanelColor(style) : GetFallbackPanelColor(style);
        Sprite sprite = theme != null ? theme.GetPanelSprite(style) : null;

        GameObject panelObject = null;
        if (panelPrefab != null)
        {
            panelObject = Object.Instantiate(panelPrefab, parent, false);
            panelObject.name = name;
            if (panelObject.GetComponent<RectTransform>() == null)
            {
                Object.Destroy(panelObject);
                panelObject = null;
            }
        }

        if (panelObject == null)
        {
            panelObject = new GameObject(name, typeof(RectTransform), typeof(Image));
            panelObject.transform.SetParent(parent, false);
        }

        RectTransform rectTransform = panelObject.GetComponent<RectTransform>();
        Stretch(rectTransform);

        Image image = panelObject.GetComponent<Image>();
        if (image == null)
        {
            image = panelObject.AddComponent<Image>();
        }

        image.color = color;
        if (sprite != null)
        {
            image.sprite = sprite;
            image.type = Image.Type.Sliced;
        }

        return rectTransform;
    }

    public static RectTransform CreateSafeAreaRoot(Transform parent, string name)
    {
        GameObject safeAreaObject = new GameObject(name, typeof(RectTransform), typeof(SafeAreaRoot));
        safeAreaObject.transform.SetParent(parent, false);

        RectTransform rectTransform = safeAreaObject.GetComponent<RectTransform>();
        Stretch(rectTransform);
        safeAreaObject.GetComponent<SafeAreaRoot>().ApplySafeArea();

        return rectTransform;
    }

    public static Text CreateText(Transform parent, string name, string value, int fontSize, Color color, TextAnchor alignment)
    {
        GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
        textObject.transform.SetParent(parent, false);

        Text text = textObject.GetComponent<Text>();
        text.font = DefaultFont;
        text.text = value;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = alignment;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;

        return text;
    }

    public static Button CreateButton(Transform parent, string name, string label, Color normalColor, Color textColor)
    {
        GameObject buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);

        Image image = buttonObject.GetComponent<Image>();
        image.color = normalColor;

        Button button = buttonObject.GetComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = normalColor;
        colors.highlightedColor = Color.Lerp(normalColor, Color.white, 0.18f);
        colors.pressedColor = Color.Lerp(normalColor, Color.black, 0.18f);
        colors.selectedColor = colors.highlightedColor;
        colors.disabledColor = new Color(0.45f, 0.45f, 0.45f, 0.55f);
        button.colors = colors;

        Text text = CreateText(buttonObject.transform, "Label", label, 34, textColor, TextAnchor.MiddleCenter);
        RectTransform textRect = text.GetComponent<RectTransform>();
        Stretch(textRect, 18f, 6f, 18f, 6f);

        return button;
    }

    public static Button CreateButton(Transform parent, string name, string label, RuntimeButtonStyle style)
    {
        ThemeData theme = Theme;
        Color normalColor = theme != null ? theme.GetButtonColor(style) : GetFallbackButtonColor(style);
        Color textColor = theme != null ? theme.primaryTextColor : Color.white;
        Sprite sprite = theme != null ? theme.GetButtonSprite(style) : null;
        GameObject buttonPrefab = theme != null ? theme.GetButtonPrefab(style) : null;

        GameObject buttonObject = null;
        if (buttonPrefab != null)
        {
            buttonObject = Object.Instantiate(buttonPrefab, parent, false);
            buttonObject.name = name;
            if (buttonObject.GetComponent<RectTransform>() == null)
            {
                Object.Destroy(buttonObject);
                buttonObject = null;
            }
        }

        if (buttonObject == null)
        {
            buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
        }

        Image image = buttonObject.GetComponent<Image>();
        if (image == null)
        {
            image = buttonObject.AddComponent<Image>();
        }

        image.color = normalColor;
        if (sprite != null)
        {
            image.sprite = sprite;
            image.type = Image.Type.Sliced;
        }

        Button button = buttonObject.GetComponent<Button>();
        if (button == null)
        {
            button = buttonObject.AddComponent<Button>();
        }

        ApplyButtonColors(button, normalColor);
        SetButtonLabel(buttonObject.transform, label, textColor);

        return button;
    }

    static void ApplyButtonColors(Button button, Color normalColor)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = normalColor;
        colors.highlightedColor = Color.Lerp(normalColor, Color.white, 0.18f);
        colors.pressedColor = Color.Lerp(normalColor, Color.black, 0.18f);
        colors.selectedColor = colors.highlightedColor;
        colors.disabledColor = new Color(0.45f, 0.45f, 0.45f, 0.55f);
        button.colors = colors;
    }

    static void SetButtonLabel(Transform buttonTransform, string label, Color textColor)
    {
        Text text = buttonTransform.GetComponentInChildren<Text>();
        if (text == null)
        {
            text = CreateText(buttonTransform, "Label", label, 34, textColor, TextAnchor.MiddleCenter);
            Stretch(text.GetComponent<RectTransform>(), 18f, 6f, 18f, 6f);
            return;
        }

        text.font = DefaultFont;
        text.text = label;
        text.color = textColor;
        text.alignment = TextAnchor.MiddleCenter;
    }

    static Color GetFallbackButtonColor(RuntimeButtonStyle style)
    {
        switch (style)
        {
            case RuntimeButtonStyle.Primary:
                return new Color(0.11f, 0.49f, 0.76f, 1f);
            case RuntimeButtonStyle.Icon:
                return new Color(0.12f, 0.12f, 0.16f, 0.82f);
            default:
                return new Color(0.24f, 0.26f, 0.31f, 1f);
        }
    }

    static Color GetFallbackPanelColor(RuntimePanelStyle style)
    {
        switch (style)
        {
            case RuntimePanelStyle.Background:
                return new Color(0.06f, 0.08f, 0.1f, 1f);
            case RuntimePanelStyle.Dimmer:
                return new Color(0f, 0f, 0f, 0.62f);
            case RuntimePanelStyle.Dialog:
                return new Color(0.08f, 0.09f, 0.12f, 0.97f);
            default:
                return new Color(0.24f, 0.26f, 0.31f, 1f);
        }
    }

    public static void Stretch(RectTransform rectTransform, float left = 0f, float bottom = 0f, float right = 0f, float top = 0f)
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = new Vector2(left, bottom);
        rectTransform.offsetMax = new Vector2(-right, -top);
    }

    public static void SetTopLeft(RectTransform rectTransform, Vector2 anchoredPosition, Vector2 size)
    {
        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchorMax = new Vector2(0f, 1f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = size;
    }

    public static void SetCenter(RectTransform rectTransform, Vector2 anchoredPosition, Vector2 size)
    {
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = size;
    }

    public static RuntimeUiMotion AddMotion(RectTransform rectTransform, RuntimeUiMotionType motionType, float amplitude, float frequency = 1f, float introDuration = 0.35f)
    {
        RuntimeUiMotion motion = rectTransform.GetComponent<RuntimeUiMotion>();
        if (motion == null)
        {
            motion = rectTransform.gameObject.AddComponent<RuntimeUiMotion>();
        }

        motion.Configure(motionType, amplitude, frequency, introDuration);
        return motion;
    }
}
