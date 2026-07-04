using UnityEditor;
using UnityEngine;

public static class GeneratedSpritePrefabWireUp
{
    const string GeneratedRoot = "Assets/Sprites/PixelArt/Generated/";
    const string ClassicThemePath = "Assets/Data/Themes/ClassicTheme.asset";
    const string FullCellTileSpritePath = "Assets/Sprites/squareFilled.png";
    const string FallbackGameplayBackgroundPath = "Assets/Sprites/sky_night.png";
    const int TileSortingOrder = 0;
    const int PieceSortingOrder = 10;
    const int BombIconSortingOrder = 12;
    static readonly Color NormalTileColor = new Color(0.08f, 0.13f, 0.18f, 0.88f);

    static readonly string[] PieceSprites =
    {
        "piece_red_ruby.png",
        "piece_blue_drop.png",
        "piece_yellow_star.png",
        "piece_green_leaf.png",
        "piece_purple_moon.png",
        "piece_orange_shell.png"
    };

    [MenuItem("Tools/Match Three/Wire Generated Gameplay Sprites")]
    public static void Apply()
    {
        NormalizeGeneratedImports();

        Sprite[] pieces = LoadSprites(PieceSprites);
        Sprite rowBomb = LoadSprite("special_row_bomb_arrow.png");
        Sprite columnBomb = LoadSprite("special_column_bomb_rocket.png");
        Sprite adjacentBomb = LoadSprite("special_adjacent_bomb.png");
        Sprite colorBomb = LoadSprite("piece_color_bomb_prism.png");
        Sprite collectible = LoadSprite("collectible_golden_key.png");
        Sprite blocker = LoadSprite("blocker_dark_stone.png");
        Sprite normalTile = LoadProjectSprite(FullCellTileSpritePath);
        Sprite breakableTile = LoadSprite("tile_breakable_cracked.png");
        Sprite obstacleTile = LoadSprite("tile_obstacle_dark_stone.png");

        for (int i = 0; i < pieces.Length; i++)
        {
            int pieceNumber = i + 1;
            SetRootSprite($"Assets/Prefabs/Dots/Dot {pieceNumber}.prefab", pieces[i]);
            SetBombSprites($"Assets/Prefabs/Bombs/RowBombs/RowBomb {pieceNumber}.prefab", pieces[i], rowBomb);
            SetBombSprites($"Assets/Prefabs/Bombs/ColumnBombs/ColumnBomb {pieceNumber}.prefab", pieces[i], columnBomb);
            SetBombSprites($"Assets/Prefabs/Bombs/AdjacentBombs/AdjacentBomb {pieceNumber}.prefab", pieces[i], adjacentBomb);
        }

        SetRootSprite("Assets/Prefabs/Bombs/ColorBomb.prefab", colorBomb);
        SetRootSprite("Assets/Prefabs/Collectibles/Collectible.prefab", collectible);
        SetRootSprite("Assets/Prefabs/Collectibles/Blocker.prefab", blocker);
        SetTileSprite("Assets/Prefabs/Tiles/TileNormal.prefab", normalTile, NormalTileColor, null, null);
        SetTileSprite("Assets/Prefabs/Tiles/TileBreakable.prefab", breakableTile, Color.white, normalTile, breakableTile);
        SetTileSprite("Assets/Prefabs/Tiles/TileDoubleBreakable.prefab", breakableTile, Color.white, normalTile, breakableTile);
        SetTileSprite("Assets/Prefabs/Tiles/TileObstacle.prefab", obstacleTile, Color.white, null, null);
        AssignClassicThemeSprites();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Generated gameplay sprites wired into prefabs.");
    }

    static void NormalizeGeneratedImports()
    {
        string[] spriteGuids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Sprites/PixelArt/Generated" });
        for (int i = 0; i < spriteGuids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(spriteGuids[i]);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                continue;
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 128f;
            importer.spritePivot = new Vector2(0.5f, 0.5f);
            importer.mipmapEnabled = false;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.alphaIsTransparency = true;
            importer.SaveAndReimport();
        }
    }

    static Sprite[] LoadSprites(string[] names)
    {
        Sprite[] sprites = new Sprite[names.Length];
        for (int i = 0; i < names.Length; i++)
        {
            sprites[i] = LoadSprite(names[i]);
        }

        return sprites;
    }

    static Sprite LoadSprite(string fileName)
    {
        return LoadProjectSprite(GeneratedRoot + fileName);
    }

    static Sprite LoadOptionalSprite(string fileName)
    {
        return AssetDatabase.LoadAssetAtPath<Sprite>(GeneratedRoot + fileName);
    }

    static Sprite LoadProjectSprite(string path)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (sprite == null)
        {
            throw new System.InvalidOperationException("Missing sprite: " + path);
        }

        return sprite;
    }

    static void SetRootSprite(string prefabPath, Sprite sprite)
    {
        EditPrefab(prefabPath, root =>
        {
            SpriteRenderer renderer = root.GetComponent<SpriteRenderer>();
            if (renderer == null)
            {
                throw new System.InvalidOperationException("Missing root SpriteRenderer: " + prefabPath);
            }

            renderer.sprite = sprite;
            renderer.color = Color.white;
            renderer.sortingOrder = PieceSortingOrder;
        });
    }

    static void SetBombSprites(string prefabPath, Sprite pieceSprite, Sprite iconSprite)
    {
        EditPrefab(prefabPath, root =>
        {
            SpriteRenderer rootRenderer = root.GetComponent<SpriteRenderer>();
            if (rootRenderer == null)
            {
                throw new System.InvalidOperationException("Missing bomb root SpriteRenderer: " + prefabPath);
            }

            rootRenderer.sprite = pieceSprite;
            rootRenderer.color = Color.white;
            rootRenderer.sortingOrder = PieceSortingOrder;

            Transform icon = root.transform.Find("Icon");
            if (icon == null)
            {
                throw new System.InvalidOperationException("Missing bomb Icon child: " + prefabPath);
            }

            SpriteRenderer iconRenderer = icon.GetComponent<SpriteRenderer>();
            if (iconRenderer == null)
            {
                throw new System.InvalidOperationException("Missing bomb Icon SpriteRenderer: " + prefabPath);
            }

            iconRenderer.sprite = iconSprite;
            iconRenderer.color = Color.white;
            iconRenderer.sortingOrder = BombIconSortingOrder;
            icon.localScale = Vector3.one;
        });
    }

    static void SetTileSprite(string prefabPath, Sprite sprite, Color rendererColor, Sprite normalSprite, Sprite breakableSprite)
    {
        EditPrefab(prefabPath, root =>
        {
            SpriteRenderer renderer = root.GetComponent<SpriteRenderer>();
            if (renderer == null)
            {
                throw new System.InvalidOperationException("Missing tile SpriteRenderer: " + prefabPath);
            }

            renderer.sprite = sprite;
            renderer.color = rendererColor;
            renderer.sortingOrder = TileSortingOrder;

            Tile tile = root.GetComponent<Tile>();
            if (tile != null)
            {
                tile.normalColor = NormalTileColor;
            }

            if (tile != null && normalSprite != null && breakableSprite != null)
            {
                if (tile.breakableSprites == null || tile.breakableSprites.Length == 0)
                {
                    tile.breakableSprites = new Sprite[] { normalSprite, breakableSprite };
                }
                else
                {
                    for (int i = 0; i < tile.breakableSprites.Length; i++)
                    {
                        tile.breakableSprites[i] = i == 0 ? normalSprite : breakableSprite;
                    }
                }
            }
        });
    }

    static void AssignClassicThemeSprites()
    {
        ThemeData theme = AssetDatabase.LoadAssetAtPath<ThemeData>(ClassicThemePath);
        if (theme == null)
        {
            throw new System.InvalidOperationException("Missing theme: " + ClassicThemePath);
        }

        Sprite generatedBackground = LoadOptionalSprite("ui_gameplay_background.png");
        theme.gameplayBackground = generatedBackground != null ? generatedBackground : LoadProjectSprite(FallbackGameplayBackgroundPath);
        theme.boosterButtonSprite = null;
        theme.boosterHammerIconSprite = LoadOptionalSprite("ui_booster_hammer.png");
        theme.boosterBombIconSprite = LoadOptionalSprite("ui_booster_bomb.png");
        theme.boosterColorIconSprite = LoadOptionalSprite("ui_booster_color.png");
        theme.boosterHandIconSprite = LoadOptionalSprite("ui_booster_hand.png");
        theme.pauseIconSprite = LoadOptionalSprite("ui_pause_icon.png");
        EditorUtility.SetDirty(theme);
    }

    static void EditPrefab(string prefabPath, System.Action<GameObject> edit)
    {
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            edit(root);
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }
}
