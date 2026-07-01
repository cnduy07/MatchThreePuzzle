using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class RuntimeUiFactory
{
    static Font s_defaultFont;

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
}
