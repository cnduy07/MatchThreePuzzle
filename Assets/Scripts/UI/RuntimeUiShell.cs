using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RuntimeUiShell : MonoBehaviour
{
    const int OverlaySortingOrder = 100;

    public static RuntimeUiShell Active { get; private set; }

    Canvas m_canvas;
    RectTransform m_root;
    RectTransform m_safeAreaRoot;
    RectTransform m_hudLayer;
    RectTransform m_modalLayer;
    RectTransform m_pauseButtonLayer;
    RectTransform m_gameplayPresentationLayer;
    RectTransform m_pauseLayer;
    RectTransform m_settingsLayer;
    GameObject m_pauseButtonObject;
    Text m_runtimeMovesText;
    Text m_runtimeScoreText;

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
        Active = this;
        EnsureCanvas();
    }

    void OnDestroy()
    {
        if (Active == this)
        {
            Active = null;
        }
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

        m_hudLayer = CreateLayer("HUD Layer");
        m_pauseButtonLayer = CreateLayer("Pause Button Layer");
        m_gameplayPresentationLayer = CreateLayer("Gameplay Presentation Layer");
        m_modalLayer = CreateLayer("Modal Layer");
        m_pauseLayer = CreateLayer("Pause Layer");
        m_settingsLayer = CreateLayer("Settings Layer");
        m_modalLayer.gameObject.SetActive(false);
        m_gameplayPresentationLayer.gameObject.SetActive(false);
        m_pauseLayer.gameObject.SetActive(false);
        m_settingsLayer.gameObject.SetActive(false);
    }

    public void CreateGameplayHud(LevelData levelData, int movesLeft, int scoreGoal, Action pauseAction)
    {
        EnsureCanvas();
        ClearChildren(m_hudLayer);
        m_hudLayer.gameObject.SetActive(true);

        GameSceneHudState hudState = GameSceneHudBuilder.Build(m_hudLayer, levelData, movesLeft, scoreGoal, pauseAction);
        m_runtimeMovesText = hudState.MovesText;
        m_runtimeScoreText = hudState.ScoreText;
        m_pauseButtonObject = hudState.PauseButtonObject;
        UpdateGameplayMoves(movesLeft);
        UpdateGameplayScore(ScoreManager.Instance != null ? ScoreManager.Instance.CurrentScore : 0);
    }

    public void UpdateGameplayMoves(int movesLeft)
    {
        if (m_runtimeMovesText != null)
        {
            m_runtimeMovesText.text = movesLeft.ToString();
        }
    }

    public void UpdateGameplayScore(int score)
    {
        if (m_runtimeScoreText != null)
        {
            m_runtimeScoreText.text = "Score " + score;
        }
    }

    public void CreatePauseButton(Action onClick)
    {
        EnsureCanvas();

        if (m_pauseButtonObject != null)
        {
            Destroy(m_pauseButtonObject);
        }

        Button button = RuntimeUiFactory.CreateButton(m_pauseButtonLayer, "Pause Button", "||", RuntimeButtonStyle.Icon);
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

        RuntimeUiFactory.CreatePanel(m_modalLayer, "Dimmer", RuntimePanelStyle.Dimmer);

        RectTransform panel = RuntimeUiFactory.CreatePanel(m_modalLayer, "Dialog Panel", RuntimePanelStyle.Dialog);
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
            Button secondary = RuntimeUiFactory.CreateButton(panel, "Secondary Button", secondaryLabel, RuntimeButtonStyle.Secondary);
            RuntimeUiFactory.SetCenter(secondary.GetComponent<RectTransform>(), new Vector2(-170f, -215f), new Vector2(260f, 92f));
            secondary.onClick.AddListener(() => secondaryAction?.Invoke());

            Button primary = RuntimeUiFactory.CreateButton(panel, "Primary Button", primaryLabel, RuntimeButtonStyle.Primary);
            RuntimeUiFactory.SetCenter(primary.GetComponent<RectTransform>(), new Vector2(170f, -215f), new Vector2(260f, 92f));
            primary.onClick.AddListener(() => primaryAction?.Invoke());
        }
        else
        {
            Button primary = RuntimeUiFactory.CreateButton(panel, "Primary Button", primaryLabel, RuntimeButtonStyle.Primary);
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

    public IEnumerator PlayLoseThreatAnimation()
    {
        EnsureCanvas();
        ClearChildren(m_gameplayPresentationLayer);
        m_gameplayPresentationLayer.gameObject.SetActive(true);

        ThemeData theme = RuntimeUiFactory.Theme;
        RectTransform stage = CreateLoseThreatStage(theme);
        RectTransform monster = CreateThreatActor(stage, "Monster", theme != null ? theme.loseThreatMonsterSprite : null, theme != null ? theme.loseThreatMonsterColor : new Color(0.45f, 0.16f, 0.72f, 1f), "MONSTER");
        RectTransform door = CreateThreatActor(stage, "Door", theme != null ? theme.loseThreatDoorSprite : null, theme != null ? theme.loseThreatDoorColor : new Color(0.47f, 0.25f, 0.09f, 1f), "DOOR");
        RectTransform alert = RuntimeUiFactory.CreatePanel(stage, "Attack Flash", theme != null ? theme.loseThreatAlertColor : new Color(0.72f, 0.08f, 0.05f, 0.38f));
        RuntimeUiFactory.Stretch(alert);
        alert.gameObject.SetActive(false);

        RuntimeUiFactory.SetCenter(door, new Vector2(250f, 6f), new Vector2(148f, 220f));
        RuntimeUiFactory.SetCenter(monster, new Vector2(-420f, -10f), new Vector2(220f, 152f));

        yield return MoveThreatActor(monster, new Vector2(-420f, -10f), new Vector2(110f, -10f), 1.05f);
        yield return ShakeDoorAndFlash(door, alert);
        yield return new WaitForSecondsRealtime(0.2f);

        ClearChildren(m_gameplayPresentationLayer);
        m_gameplayPresentationLayer.gameObject.SetActive(false);
    }

    public void ShowPauseMenu(Action resumeAction, Action retryAction, Action settingsAction, Action levelSelectAction)
    {
        EnsureCanvas();
        ClearChildren(m_pauseLayer);
        m_pauseLayer.gameObject.SetActive(true);

        RuntimeUiFactory.CreatePanel(m_pauseLayer, "Dimmer", RuntimePanelStyle.Dimmer);
        RectTransform panel = RuntimeUiFactory.CreatePanel(m_pauseLayer, "Pause Panel", RuntimePanelStyle.Dialog);
        RuntimeUiFactory.SetCenter(panel, Vector2.zero, new Vector2(700f, 650f));

        Text titleText = RuntimeUiFactory.CreateText(panel, "Title", "Paused", 64, Color.white, TextAnchor.MiddleCenter);
        RuntimeUiFactory.SetCenter(titleText.GetComponent<RectTransform>(), new Vector2(0f, 210f), new Vector2(560f, 100f));

        Button resume = RuntimeUiFactory.CreateButton(panel, "Resume Button", "Resume", RuntimeButtonStyle.Primary);
        RuntimeUiFactory.SetCenter(resume.GetComponent<RectTransform>(), new Vector2(0f, 70f), new Vector2(440f, 96f));
        resume.onClick.AddListener(() => resumeAction?.Invoke());

        Button retry = RuntimeUiFactory.CreateButton(panel, "Retry Button", "Retry", RuntimeButtonStyle.Secondary);
        RuntimeUiFactory.SetCenter(retry.GetComponent<RectTransform>(), new Vector2(0f, -35f), new Vector2(440f, 96f));
        retry.onClick.AddListener(() => retryAction?.Invoke());

        Button settings = RuntimeUiFactory.CreateButton(panel, "Settings Button", "Settings", RuntimeButtonStyle.Secondary);
        RuntimeUiFactory.SetCenter(settings.GetComponent<RectTransform>(), new Vector2(0f, -145f), new Vector2(440f, 96f));
        settings.onClick.AddListener(() => settingsAction?.Invoke());

        Button levels = RuntimeUiFactory.CreateButton(panel, "Level Select Button", "Level Select", RuntimeButtonStyle.Secondary);
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

        RuntimeUiFactory.CreatePanel(m_settingsLayer, "Dimmer", RuntimePanelStyle.Dimmer);
        RectTransform panel = RuntimeUiFactory.CreatePanel(m_settingsLayer, "Settings Panel", RuntimePanelStyle.Dialog);
        RuntimeUiFactory.SetCenter(panel, Vector2.zero, new Vector2(720f, 620f));

        Text titleText = RuntimeUiFactory.CreateText(panel, "Title", "Settings", 62, Color.white, TextAnchor.MiddleCenter);
        RuntimeUiFactory.SetCenter(titleText.GetComponent<RectTransform>(), new Vector2(0f, 205f), new Vector2(560f, 100f));

        Button audioButton = RuntimeUiFactory.CreateButton(panel, "Audio Toggle", GetAudioLabel(), RuntimeButtonStyle.Primary);
        RuntimeUiFactory.SetCenter(audioButton.GetComponent<RectTransform>(), new Vector2(0f, 55f), new Vector2(500f, 96f));

        Button hapticsButton = RuntimeUiFactory.CreateButton(panel, "Haptics Toggle", GetHapticsLabel(), RuntimeButtonStyle.Secondary);
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

        Button close = RuntimeUiFactory.CreateButton(panel, "Close Button", "Close", RuntimeButtonStyle.Secondary);
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

    RectTransform CreateLoseThreatStage(ThemeData theme)
    {
        GameObject stagePrefab = theme != null ? theme.loseThreatStagePrefab : null;
        GameObject stageObject = null;
        if (stagePrefab != null)
        {
            stageObject = Instantiate(stagePrefab, m_gameplayPresentationLayer, false);
            stageObject.name = "Lose Threat Stage";
            if (stageObject.GetComponent<RectTransform>() == null)
            {
                Destroy(stageObject);
                stageObject = null;
            }
        }

        if (stageObject == null)
        {
            stageObject = new GameObject("Lose Threat Stage", typeof(RectTransform), typeof(CanvasGroup));
            stageObject.transform.SetParent(m_gameplayPresentationLayer, false);
        }

        RectTransform stage = stageObject.GetComponent<RectTransform>();
        stage.anchorMin = new Vector2(0.5f, 1f);
        stage.anchorMax = new Vector2(0.5f, 1f);
        stage.pivot = new Vector2(0.5f, 1f);
        stage.anchoredPosition = new Vector2(0f, -210f);
        stage.sizeDelta = new Vector2(900f, 300f);
        return stage;
    }

    RectTransform CreateThreatActor(Transform parent, string name, Sprite sprite, Color color, string fallbackLabel)
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

    IEnumerator MoveThreatActor(RectTransform actor, Vector2 startPosition, Vector2 endPosition, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = t * t * (3f - 2f * t);
            actor.anchoredPosition = Vector2.LerpUnclamped(startPosition, endPosition, eased);
            actor.localScale = Vector3.one * (1f + Mathf.Sin(t * Mathf.PI * 10f) * 0.035f);
            yield return null;
        }

        actor.anchoredPosition = endPosition;
        actor.localScale = Vector3.one;
    }

    IEnumerator ShakeDoorAndFlash(RectTransform door, RectTransform alert)
    {
        Vector2 basePosition = door.anchoredPosition;
        alert.gameObject.SetActive(true);

        const float duration = 0.58f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float shake = Mathf.Sin(t * Mathf.PI * 18f) * (1f - t) * 18f;
            door.anchoredPosition = basePosition + new Vector2(shake, 0f);
            door.localScale = Vector3.one * (1f + Mathf.Sin(t * Mathf.PI * 6f) * 0.05f);

            CanvasGroup alertGroup = alert.GetComponent<CanvasGroup>();
            if (alertGroup == null)
            {
                alertGroup = alert.gameObject.AddComponent<CanvasGroup>();
            }

            alertGroup.alpha = Mathf.PingPong(t * 5f, 1f);
            yield return null;
        }

        door.anchoredPosition = basePosition;
        door.localScale = Vector3.one;
        alert.gameObject.SetActive(false);
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
