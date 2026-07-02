using UnityEngine;

[CreateAssetMenu(fileName = "GameResourceLibrary", menuName = "Match Three/Game Resource Library")]
public class GameResourceLibrary : ScriptableObject
{
    const string ResourcePath = "GameResourceLibrary";

    static GameResourceLibrary s_cachedLibrary;

    public ThemeData defaultTheme;
    public PieceSetData defaultPieceSet;
    public ThemeData[] themes;
    public PieceSetData[] pieceSets;

    public static GameResourceLibrary LoadDefault()
    {
        if (s_cachedLibrary == null)
        {
            s_cachedLibrary = Resources.Load<GameResourceLibrary>(ResourcePath);
        }

        return s_cachedLibrary;
    }

    public static ThemeData ResolveTheme(ThemeData levelTheme)
    {
        if (levelTheme != null)
        {
            return levelTheme;
        }

        GameResourceLibrary library = LoadDefault();
        return library != null ? library.defaultTheme : null;
    }

    public static PieceSetData ResolvePieceSet(ThemeData theme, PieceSetData levelPieceSet)
    {
        if (levelPieceSet != null)
        {
            return levelPieceSet;
        }

        if (theme != null && theme.defaultPieceSet != null)
        {
            return theme.defaultPieceSet;
        }

        GameResourceLibrary library = LoadDefault();
        return library != null ? library.defaultPieceSet : null;
    }

    public static PieceSetData ResolvePieceSet(LevelData levelData)
    {
        if (levelData == null)
        {
            return ResolvePieceSet(ResolveTheme(null), null);
        }

        ThemeData theme = ResolveTheme(levelData.themeData);
        return ResolvePieceSet(theme, levelData.pieceSetData);
    }
}
