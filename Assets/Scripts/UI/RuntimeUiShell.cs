using System;
using UnityEngine;
using UnityEngine.UI;

public class RuntimeUiShell : MonoBehaviour
{
    const int OverlaySortingOrder = 100;

    Canvas m_canvas;
    RectTransform m_root;
    RectTransform m_safeAreaRoot;
    RectTransform m_modalLayer;
    RectTransform m_pauseButtonLayer;
    RectTransform m_pauseLayer;
    RectTransform m_settingsLayer;
    GameObject m_pauseButtonObject;

    public static RuntimeUiShell CreateOrFind()
    {
        RuntimeUiShell shell = FindAnyObjectByType<RuntimeUiShell>();
        if (shell != null)
        {
            shell.EnsureCanvas();
            return shell;
        }

        GameObject shellObject = new GameObject("Runtime UI Shell");
        shell = shellObject.AddComponent<RuntimeUiShell>();
        shell.EnsureCanvas();
        return shell;
    }

    void Awake()
    {
        EnsureCanvas();
    }

    public void EnsureCanvas()
    {
        if (m_canvas != null)
        {
            return;
        }

        m_canvas = RuntimeUiFactory.CreateOverlayCanvas("Runtime UI Canvas", OverlaySortingOrder);
        m_canvas.transform.SetParent(transform, false);
        m_root = m_canvas.GetComponent<RectTransform>();
        m_safeAreaRoot = RuntimeUiFactory.CreateSafeAreaRoot(m_root, "Safe Area Root");

        m_pauseButtonLayer = CreateLayer("Pause Button Layer");
        m_modalLayer = CreateLayer("Modal Layer");
        m_pauseLayer = CreateLayer("Pause Layer");
        m_settingsLayer = CreateLayer("Settings Layer");
        m_modalLayer.gameObject.SetActive(false);
        m_pauseLayer.gameObject.SetActive(false);
        m_settingsLayer.gameObject.SetActive(false);
    }

    public void CreatePauseButton(Action onClick)
    {
        EnsureCanvas();

        if (m_pauseButtonObject != null)
        {
            Destroy(m_pauseButtonObject);
        }

        Button button = RuntimeUiFactory.CreateButton(m_pauseButtonLayer, "Pause Button", "||", new Color(0.12f, 0.12f, 0.16f, 0.82f), Color.white);
        m_pauseButtonObject = button.gameObject;
        RectTransform buttonRect = button.GetComponent<RectTransform>();
        RuntimeUiFactory.SetTopLeft(buttonRect, new Vector2(72f, -72f), new Vector2(92f, 92f));
        button.onClick.AddListener(() => onClick?.Invoke());
    }

    public void ShowModal(Sprite icon, string title, string body, string primaryLabel, Action primaryAction, string secondaryLabel = null, Action secondaryAction = null)
    {
        EnsureCanvas();
        ClearChildren(m_modalLayer);
        m_modalLayer.gameObject.SetActive(true);

        RuntimeUiFactory.CreatePanel(m_modalLayer, "Dimmer", new Color(0f, 0f, 0f, 0.58f));

        RectTransform panel = RuntimeUiFactory.CreatePanel(m_modalLayer, "Dialog Panel", new Color(0.08f, 0.09f, 0.12f, 0.96f));
        RuntimeUiFactory.SetCenter(panel, Vector2.zero, new Vector2(760f, 620f));

        if (icon != null)
        {
            GameObject iconObject = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            iconObject.transform.SetParent(panel, false);
            Image image = iconObject.GetComponent<Image>();
            image.sprite = icon;
            image.preserveAspect = true;
            RuntimeUiFactory.SetCenter(image.GetComponent<RectTransform>(), new Vector2(0f, 190f), new Vector2(140f, 140f));
        }

        Text titleText = RuntimeUiFactory.CreateText(panel, "Title", title, 58, Color.white, TextAnchor.MiddleCenter);
        RuntimeUiFactory.SetCenter(titleText.GetComponent<RectTransform>(), new Vector2(0f, 80f), new Vector2(640f, 100f));

        Text bodyText = RuntimeUiFactory.CreateText(panel, "Body", body, 34, new Color(0.86f, 0.9f, 0.95f, 1f), TextAnchor.MiddleCenter);
        RuntimeUiFactory.SetCenter(bodyText.GetComponent<RectTransform>(), new Vector2(0f, -40f), new Vector2(620f, 150f));

        if (!string.IsNullOrEmpty(secondaryLabel))
        {
            Button secondary = RuntimeUiFactory.CreateButton(panel, "Secondary Button", secondaryLabel, new Color(0.24f, 0.25f, 0.3f, 1f), Color.white);
            RuntimeUiFactory.SetCenter(secondary.GetComponent<RectTransform>(), new Vector2(-170f, -215f), new Vector2(260f, 92f));
            secondary.onClick.AddListener(() => secondaryAction?.Invoke());

            Button primary = RuntimeUiFactory.CreateButton(panel, "Primary Button", primaryLabel, new Color(0.11f, 0.49f, 0.76f, 1f), Color.white);
            RuntimeUiFactory.SetCenter(primary.GetComponent<RectTransform>(), new Vector2(170f, -215f), new Vector2(260f, 92f));
            primary.onClick.AddListener(() => primaryAction?.Invoke());
        }
        else
        {
            Button primary = RuntimeUiFactory.CreateButton(panel, "Primary Button", primaryLabel, new Color(0.11f, 0.49f, 0.76f, 1f), Color.white);
            RuntimeUiFactory.SetCenter(primary.GetComponent<RectTransform>(), new Vector2(0f, -215f), new Vector2(330f, 92f));
            primary.onClick.AddListener(() => primaryAction?.Invoke());
        }
    }

    public void HideModal()
    {
        if (m_modalLayer == null)
        {
            return;
        }

        ClearChildren(m_modalLayer);
        m_modalLayer.gameObject.SetActive(false);
    }

    public void ShowPauseMenu(Action resumeAction, Action retryAction, Action settingsAction, Action levelSelectAction)
    {
        EnsureCanvas();
        ClearChildren(m_pauseLayer);
        m_pauseLayer.gameObject.SetActive(true);

        RuntimeUiFactory.CreatePanel(m_pauseLayer, "Dimmer", new Color(0f, 0f, 0f, 0.62f));
        RectTransform panel = RuntimeUiFactory.CreatePanel(m_pauseLayer, "Pause Panel", new Color(0.08f, 0.09f, 0.12f, 0.97f));
        RuntimeUiFactory.SetCenter(panel, Vector2.zero, new Vector2(700f, 650f));

        Text titleText = RuntimeUiFactory.CreateText(panel, "Title", "Paused", 64, Color.white, TextAnchor.MiddleCenter);
        RuntimeUiFactory.SetCenter(titleText.GetComponent<RectTransform>(), new Vector2(0f, 210f), new Vector2(560f, 100f));

        Button resume = RuntimeUiFactory.CreateButton(panel, "Resume Button", "Resume", new Color(0.11f, 0.49f, 0.76f, 1f), Color.white);
        RuntimeUiFactory.SetCenter(resume.GetComponent<RectTransform>(), new Vector2(0f, 70f), new Vector2(440f, 96f));
        resume.onClick.AddListener(() => resumeAction?.Invoke());

        Button retry = RuntimeUiFactory.CreateButton(panel, "Retry Button", "Retry", new Color(0.25f, 0.27f, 0.32f, 1f), Color.white);
        RuntimeUiFactory.SetCenter(retry.GetComponent<RectTransform>(), new Vector2(0f, -35f), new Vector2(440f, 96f));
        retry.onClick.AddListener(() => retryAction?.Invoke());

        Button settings = RuntimeUiFactory.CreateButton(panel, "Settings Button", "Settings", new Color(0.25f, 0.27f, 0.32f, 1f), Color.white);
        RuntimeUiFactory.SetCenter(settings.GetComponent<RectTransform>(), new Vector2(0f, -145f), new Vector2(440f, 96f));
        settings.onClick.AddListener(() => settingsAction?.Invoke());

        Button levels = RuntimeUiFactory.CreateButton(panel, "Level Select Button", "Level Select", new Color(0.25f, 0.27f, 0.32f, 1f), Color.white);
        RuntimeUiFactory.SetCenter(levels.GetComponent<RectTransform>(), new Vector2(0f, -255f), new Vector2(440f, 96f));
        levels.onClick.AddListener(() => levelSelectAction?.Invoke());
    }

    public void HidePauseMenu()
    {
        if (m_pauseLayer == null)
        {
            return;
        }

        ClearChildren(m_pauseLayer);
        m_pauseLayer.gameObject.SetActive(false);
    }

    public void ShowSettingsOverlay(Action closeAction)
    {
        EnsureCanvas();
        ClearChildren(m_settingsLayer);
        m_settingsLayer.gameObject.SetActive(true);

        RuntimeUiFactory.CreatePanel(m_settingsLayer, "Dimmer", new Color(0f, 0f, 0f, 0.66f));
        RectTransform panel = RuntimeUiFactory.CreatePanel(m_settingsLayer, "Settings Panel", new Color(0.08f, 0.09f, 0.12f, 0.98f));
        RuntimeUiFactory.SetCenter(panel, Vector2.zero, new Vector2(720f, 620f));

        Text titleText = RuntimeUiFactory.CreateText(panel, "Title", "Settings", 62, Color.white, TextAnchor.MiddleCenter);
        RuntimeUiFactory.SetCenter(titleText.GetComponent<RectTransform>(), new Vector2(0f, 205f), new Vector2(560f, 100f));

        Button audioButton = RuntimeUiFactory.CreateButton(panel, "Audio Toggle", GetAudioLabel(), new Color(0.11f, 0.49f, 0.76f, 1f), Color.white);
        RuntimeUiFactory.SetCenter(audioButton.GetComponent<RectTransform>(), new Vector2(0f, 55f), new Vector2(500f, 96f));

        Button hapticsButton = RuntimeUiFactory.CreateButton(panel, "Haptics Toggle", GetHapticsLabel(), new Color(0.25f, 0.27f, 0.32f, 1f), Color.white);
        RuntimeUiFactory.SetCenter(hapticsButton.GetComponent<RectTransform>(), new Vector2(0f, -65f), new Vector2(500f, 96f));

        audioButton.onClick.AddListener(() =>
        {
            RuntimeSettingsState.ToggleAudio();
            SetButtonLabel(audioButton, GetAudioLabel());
        });

        hapticsButton.onClick.AddListener(() =>
        {
            RuntimeSettingsState.ToggleHaptics();
            SetButtonLabel(hapticsButton, GetHapticsLabel());
        });

        Button close = RuntimeUiFactory.CreateButton(panel, "Close Button", "Close", new Color(0.24f, 0.26f, 0.31f, 1f), Color.white);
        RuntimeUiFactory.SetCenter(close.GetComponent<RectTransform>(), new Vector2(0f, -220f), new Vector2(360f, 96f));
        close.onClick.AddListener(() =>
        {
            HideSettingsOverlay();
            closeAction?.Invoke();
        });
    }

    public void HideSettingsOverlay()
    {
        if (m_settingsLayer == null)
        {
            return;
        }

        ClearChildren(m_settingsLayer);
        m_settingsLayer.gameObject.SetActive(false);
    }

    RectTransform CreateLayer(string name)
    {
        GameObject layerObject = new GameObject(name, typeof(RectTransform));
        layerObject.transform.SetParent(m_safeAreaRoot, false);
        RectTransform rectTransform = layerObject.GetComponent<RectTransform>();
        RuntimeUiFactory.Stretch(rectTransform);
        return rectTransform;
    }

    void ClearChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }

    string GetAudioLabel()
    {
        return RuntimeSettingsState.AudioEnabled ? "Audio: On" : "Audio: Off";
    }

    string GetHapticsLabel()
    {
        return RuntimeSettingsState.HapticsEnabled ? "Haptics: On (TODO)" : "Haptics: Off (TODO)";
    }

    void SetButtonLabel(Button button, string label)
    {
        Text labelText = button.GetComponentInChildren<Text>();
        if (labelText != null)
        {
            labelText.text = label;
        }
    }
}
