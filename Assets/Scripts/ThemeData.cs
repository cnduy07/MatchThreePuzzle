using UnityEngine;

public enum RuntimeButtonStyle
{
    Primary,
    Secondary,
    Icon
}

public enum RuntimePanelStyle
{
    Background,
    Dimmer,
    Dialog,
    Surface,
    LevelCard
}

[CreateAssetMenu(fileName = "ThemeData", menuName = "Match Three/Theme Data")]
public class ThemeData : ScriptableObject
{
    public string themeId = "classic";
    public string displayName = "Classic Theme";

    [Header("Gameplay")]
    public Sprite gameplayBackground;
    public Sprite loseThreatMonsterSprite;
    public Sprite loseThreatDoorSprite;
    public PieceSetData defaultPieceSet;
    public GameObject normalTilePrefab;
    public GameObject[] obstacleTilePrefabs;
    public AudioClip musicClip;

    [Header("UI Sprites")]
    public Sprite panelSprite;
    public Sprite primaryButtonSprite;
    public Sprite secondaryButtonSprite;
    public Sprite iconButtonSprite;
    public Sprite levelCardSprite;
    public Sprite objectiveBadgeSprite;
    public Sprite currencyCounterSprite;
    public Sprite toggleBackgroundSprite;

    [Header("UI Prefabs")]
    public GameObject primaryButtonPrefab;
    public GameObject secondaryButtonPrefab;
    public GameObject iconButtonPrefab;
    public GameObject panelFramePrefab;
    public GameObject modalWindowPrefab;
    public GameObject topBarPrefab;
    public GameObject currencyCounterPrefab;
    public GameObject levelCardPrefab;
    public GameObject objectiveItemPrefab;
    public GameObject toggleRowPrefab;
    public GameObject sliderRowPrefab;
    public GameObject loadingOverlayPrefab;
    public GameObject loseThreatStagePrefab;

    [Header("UI Colors")]
    public Color backgroundColor = new Color(0.06f, 0.08f, 0.1f, 1f);
    public Color dimmerColor = new Color(0f, 0f, 0f, 0.62f);
    public Color dialogColor = new Color(0.08f, 0.09f, 0.12f, 0.97f);
    public Color surfaceColor = new Color(0.24f, 0.26f, 0.31f, 1f);
    public Color primaryButtonColor = new Color(0.11f, 0.49f, 0.76f, 1f);
    public Color secondaryButtonColor = new Color(0.24f, 0.26f, 0.31f, 1f);
    public Color iconButtonColor = new Color(0.12f, 0.12f, 0.16f, 0.82f);
    public Color loseThreatMonsterColor = new Color(0.45f, 0.16f, 0.72f, 1f);
    public Color loseThreatDoorColor = new Color(0.47f, 0.25f, 0.09f, 1f);
    public Color loseThreatAlertColor = new Color(0.72f, 0.08f, 0.05f, 0.38f);
    public Color primaryTextColor = Color.white;
    public Color secondaryTextColor = new Color(0.86f, 0.9f, 0.95f, 1f);

    [TextArea(2, 8)]
    public string paletteNotes;

    public Color GetButtonColor(RuntimeButtonStyle style)
    {
        switch (style)
        {
            case RuntimeButtonStyle.Primary:
                return primaryButtonColor;
            case RuntimeButtonStyle.Icon:
                return iconButtonColor;
            default:
                return secondaryButtonColor;
        }
    }

    public Sprite GetButtonSprite(RuntimeButtonStyle style)
    {
        switch (style)
        {
            case RuntimeButtonStyle.Primary:
                return primaryButtonSprite;
            case RuntimeButtonStyle.Icon:
                return iconButtonSprite;
            default:
                return secondaryButtonSprite;
        }
    }

    public GameObject GetButtonPrefab(RuntimeButtonStyle style)
    {
        switch (style)
        {
            case RuntimeButtonStyle.Primary:
                return primaryButtonPrefab;
            case RuntimeButtonStyle.Icon:
                return iconButtonPrefab;
            default:
                return secondaryButtonPrefab;
        }
    }

    public Color GetPanelColor(RuntimePanelStyle style)
    {
        switch (style)
        {
            case RuntimePanelStyle.Background:
                return backgroundColor;
            case RuntimePanelStyle.Dimmer:
                return dimmerColor;
            case RuntimePanelStyle.Dialog:
                return dialogColor;
            default:
                return surfaceColor;
        }
    }

    public Sprite GetPanelSprite(RuntimePanelStyle style)
    {
        switch (style)
        {
            case RuntimePanelStyle.LevelCard:
                return levelCardSprite;
            default:
                return panelSprite;
        }
    }

    public GameObject GetPanelPrefab(RuntimePanelStyle style)
    {
        switch (style)
        {
            case RuntimePanelStyle.Dialog:
                return modalWindowPrefab != null ? modalWindowPrefab : panelFramePrefab;
            case RuntimePanelStyle.LevelCard:
                return levelCardPrefab;
            default:
                return panelFramePrefab;
        }
    }

    public GameObject[] ResolveObstacleTiles(GameObject[] fallback)
    {
        return PieceSetData.HasConfiguredPrefab(obstacleTilePrefabs) ? obstacleTilePrefabs : fallback;
    }
}
