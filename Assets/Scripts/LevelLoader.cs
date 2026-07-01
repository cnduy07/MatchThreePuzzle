using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public LevelData levelData;
    public Board board;
    public GameManager gameManager;

    void Awake()
    {
        if (levelData == null)
        {
            return;
        }

        if (board == null)
        {
            board = FindAnyObjectByType<Board>();
        }

        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }

        if (board != null)
        {
            board.ApplyLevelData(levelData);
        }

        if (gameManager != null)
        {
            gameManager.ApplyLevelData(levelData);
        }
    }
}
