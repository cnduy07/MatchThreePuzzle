#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class Phase45UiPrefabBuilder
{
    const string SharedUiFolder = "Assets/Prefabs/UI/Shared";
    const string ClassicThemePath = "Assets/Data/Themes/ClassicTheme.asset";

    [MenuItem("Match Three/Build Shared UI Prefabs")]
    public static void CreateOrUpdateSharedUiPrefabs()
    {
        EnsureFolder("Assets/Prefabs/UI");
        EnsureFolder(SharedUiFolder);

        GameObject primaryButton = SavePrefab(CreateButtonPrefab("PrimaryButton", new Color(0.11f, 0.49f, 0.76f, 1f)), "PrimaryButton.prefab");
        GameObject secondaryButton = SavePrefab(CreateButtonPrefab("SecondaryButton", new Color(0.24f, 0.26f, 0.31f, 1f)), "SecondaryButton.prefab");
        GameObject iconButton = SavePrefab(CreateButtonPrefab("IconButton", new Color(0.12f, 0.12f, 0.16f, 0.82f), new Vector2(92f, 92f), "||", 34), "IconButton.prefab");
        GameObject panelFrame = SavePrefab(CreatePanelPrefab("PanelFrame", new Color(0.08f, 0.09f, 0.12f, 0.97f), new Vector2(720f, 620f)), "PanelFrame.prefab");
        GameObject modalWindow = SavePrefab(CreatePanelPrefab("ModalWindow", new Color(0.08f, 0.09f, 0.12f, 0.97f), new Vector2(760f, 620f)), "ModalWindow.prefab");
        GameObject topBar = SavePrefab(CreateLabeledPanelPrefab("TopBar", "Level", new Color(0.08f, 0.09f, 0.12f, 0.9f), new Vector2(760f, 104f)), "TopBar.prefab");
        GameObject currencyCounter = SavePrefab(CreateLabeledPanelPrefab("CurrencyCounter", "0", new Color(0.16f, 0.18f, 0.22f, 0.95f), new Vector2(220f, 72f)), "CurrencyCounter.prefab");
        GameObject levelCard = SavePrefab(CreateLabeledPanelPrefab("LevelCard", "Level", new Color(0.11f, 0.49f, 0.76f, 1f), new Vector2(220f, 130f)), "LevelCard.prefab");
        GameObject objectiveItem = SavePrefab(CreateLabeledPanelPrefab("ObjectiveItem", "Goal", new Color(0.16f, 0.18f, 0.22f, 0.95f), new Vector2(320f, 88f)), "ObjectiveItem.prefab");
        GameObject toggleRow = SavePrefab(CreateLabeledPanelPrefab("ToggleRow", "Toggle", new Color(0.24f, 0.26f, 0.31f, 1f), new Vector2(500f, 96f)), "ToggleRow.prefab");
        GameObject sliderRow = SavePrefab(CreateLabeledPanelPrefab("SliderRow", "Slider", new Color(0.24f, 0.26f, 0.31f, 1f), new Vector2(560f, 96f)), "SliderRow.prefab");
        GameObject loadingOverlay = SavePrefab(CreatePanelPrefab("LoadingOverlay", new Color(0f, 0f, 0f, 0.68f), new Vector2(1080f, 1920f)), "LoadingOverlay.prefab");
        GameObject loseThreatStage = SavePrefab(CreatePanelPrefab("LoseThreatStage", new Color(0f, 0f, 0f, 0f), new Vector2(900f, 300f)), "LoseThreatStage.prefab");

        ThemeData theme = AssetDatabase.LoadAssetAtPath<ThemeData>(ClassicThemePath);
        if (theme != null)
        {
            theme.primaryButtonPrefab = primaryButton;
            theme.secondaryButtonPrefab = secondaryButton;
            theme.iconButtonPrefab = iconButton;
            theme.panelFramePrefab = panelFrame;
            theme.modalWindowPrefab = modalWindow;
            theme.topBarPrefab = topBar;
            theme.currencyCounterPrefab = currencyCounter;
            theme.levelCardPrefab = levelCard;
            theme.objectiveItemPrefab = objectiveItem;
            theme.toggleRowPrefab = toggleRow;
            theme.sliderRowPrefab = sliderRow;
            theme.loadingOverlayPrefab = loadingOverlay;
            theme.loseThreatStagePrefab = loseThreatStage;
            EditorUtility.SetDirty(theme);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static GameObject CreateButtonPrefab(string name, Color color, Vector2? size = null, string label = "Button", int fontSize = 34)
    {
        GameObject root = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        RectTransform rectTransform = root.GetComponent<RectTransform>();
        rectTransform.sizeDelta = size ?? new Vector2(500f, 108f);

        Image image = root.GetComponent<Image>();
        image.color = color;

        Button button = root.GetComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = color;
        colors.highlightedColor = Color.Lerp(color, Color.white, 0.18f);
        colors.pressedColor = Color.Lerp(color, Color.black, 0.18f);
        colors.selectedColor = colors.highlightedColor;
        colors.disabledColor = new Color(0.45f, 0.45f, 0.45f, 0.55f);
        button.colors = colors;

        AddCenteredText(root.transform, "Label", label, fontSize, Color.white);
        return root;
    }

    static GameObject CreatePanelPrefab(string name, Color color, Vector2 size)
    {
        GameObject root = new GameObject(name, typeof(RectTransform), typeof(Image));
        RectTransform rectTransform = root.GetComponent<RectTransform>();
        rectTransform.sizeDelta = size;

        Image image = root.GetComponent<Image>();
        image.color = color;
        return root;
    }

    static GameObject CreateLabeledPanelPrefab(string name, string label, Color color, Vector2 size)
    {
        GameObject root = CreatePanelPrefab(name, color, size);
        AddCenteredText(root.transform, "Label", label, 30, Color.white);
        return root;
    }

    static Text AddCenteredText(Transform parent, string name, string value, int fontSize, Color color)
    {
        GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
        textObject.transform.SetParent(parent, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = new Vector2(18f, 6f);
        rectTransform.offsetMax = new Vector2(-18f, -6f);

        Text text = textObject.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (text.font == null)
        {
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        text.text = value;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        return text;
    }

    static GameObject SavePrefab(GameObject root, string fileName)
    {
        string path = Path.Combine(SharedUiFolder, fileName).Replace('\\', '/');
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        return prefab;
    }

    static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
        {
            return;
        }

        string parent = Path.GetDirectoryName(path).Replace('\\', '/');
        string folder = Path.GetFileName(path);
        AssetDatabase.CreateFolder(parent, folder);
    }
}
#endif
