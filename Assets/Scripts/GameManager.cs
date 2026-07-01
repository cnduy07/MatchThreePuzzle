using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    protected override bool ShouldPersistAcrossScenes
    {
        get
        {
            return false;
        }
    }

    public int movesLeft = 30;
    public int scoreGoal = 10000;
    public ScreenFader screenFader;
    public Text levelNameText;
    public Text movesLeftText;

    bool m_isReadyToBegin = false;
    bool m_isGameOver = false;
    public bool IsGameOver
    {
        get
        {
            return m_isGameOver;
        }
    }

    bool m_isWinner = false;
    bool m_isReadyToReplay = false;
    bool m_isPaused = false;
    string m_levelDisplayName;
    LevelData m_activeLevelData;
    int m_startingMoveLimit;
    RuntimeUiShell m_uiShell;

    public Board m_board;

    public MessageWindow messageWindow;
    public Sprite goalIcon;
    public Sprite winIcon;
    public Sprite loseIcon;

    // Start is called before the first frame update
    void Start()
    {
        m_board = FindAnyObjectByType<Board>();
        m_uiShell = RuntimeUiShell.CreateOrFind();
        if (m_uiShell != null)
        {
            HideLegacyMessageWindow();
        }

        if (levelNameText != null)
        {
            Scene scene = SceneManager.GetActiveScene();
            levelNameText.text = string.IsNullOrEmpty(m_levelDisplayName) ? scene.name : m_levelDisplayName;
        }

        if (m_startingMoveLimit <= 0)
        {
            m_startingMoveLimit = movesLeft;
        }

        UpdateMoves();

        StartCoroutine("ExecuteGameLoop");
    }

    public void ApplyLevelData(LevelData levelData)
    {
        if (levelData == null)
        {
            return;
        }

        movesLeft = Mathf.Max(0, levelData.moveLimit);
        m_startingMoveLimit = movesLeft;
        scoreGoal = Mathf.Max(0, levelData.scoreGoal);
        m_levelDisplayName = string.IsNullOrEmpty(levelData.displayName) ? levelData.name : levelData.displayName;
        m_activeLevelData = levelData;
    }

    public void UpdateMoves()
    {
        if (movesLeftText != null)
        {
            movesLeftText.text = movesLeft.ToString();
        }
    }

    IEnumerator ExecuteGameLoop()
    {
        yield return StartCoroutine("StartGameRoutine");
        yield return StartCoroutine("PlayGameRoutine");
        yield return StartCoroutine("EndGameRoutine");
    }

    public void BeginStart()
    {
        m_isReadyToBegin = true;
    }

    IEnumerator StartGameRoutine()
    {
        if (m_uiShell != null)
        {
            yield return StartCoroutine(WaitForRuntimeModal(goalIcon, "Your goal", scoreGoal.ToString(), "Start"));
        }
        else if (messageWindow != null)
        {
            messageWindow.gameObject.SetActive(true);
            messageWindow.ShowMessage(goalIcon, "Your goal \n" + scoreGoal.ToString(), "start");
            messageWindow.GetComponent<RectXformMove>().MoveOn();

            while (!m_isReadyToBegin)
            {
                yield return null;
            }
        }

        if (screenFader != null)
        {
            screenFader.FadeOff();
        }

        yield return new WaitForSeconds(0.5f);

        if (m_board != null)
        {
            m_board.SetupBoard();
            if (m_uiShell != null)
            {
                m_uiShell.CreatePauseButton(PauseGame);
            }
        }
    }

    IEnumerator PlayGameRoutine()
    {
        while (!m_isGameOver)
        {
            if (m_isPaused)
            {
                yield return null;
                continue;
            }

            if (HasReachedScoreGoal())
            {
                m_isGameOver = true;
                m_isWinner = true;
            }
            else if (movesLeft <= 0)
            {
                if (m_board != null && m_board.isRefilling)
                {
                    yield return null;
                    continue;
                }

                m_isGameOver = true;
                m_isWinner = HasReachedScoreGoal();
            }

            yield return null;
        }
    }

    IEnumerator EndGameRoutine()
    {
        m_isReadyToReplay = false;

        if (m_board != null)
        {
            m_board.SetPlayerInputEnabled(false);

            while (m_board.isRefilling)
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);
        }

        if (m_isWinner)
        {
            int finalScore = ScoreManager.Instance != null ? ScoreManager.Instance.CurrentScore : 0;
            int earnedStars = PlayerProgress.RecordLevelWin(m_activeLevelData, finalScore, movesLeft, m_startingMoveLimit, SceneFlow.LevelDatabase);

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayWinSound();
            }

            if (m_uiShell != null)
            {
                bool nextLevelSelected = false;
                bool levelSelectSelected = false;
                m_uiShell.ShowModal(winIcon, "You win", BuildWinMessage(finalScore, earnedStars), "Next", () => nextLevelSelected = true, "Levels", () => levelSelectSelected = true);
                while (!nextLevelSelected && !levelSelectSelected)
                {
                    yield return null;
                }

                m_uiShell.HideModal();
                if (levelSelectSelected || !SceneFlow.TrySelectNextLevel())
                {
                    SceneFlow.LoadLevelSelect();
                }
                else
                {
                    SceneFlow.LoadGame();
                }

                yield break;
            }
            else if (messageWindow != null)
            {
                messageWindow.gameObject.SetActive(true);
                messageWindow.ShowMessage(winIcon, "You win", "OK");
                messageWindow.GetComponent<RectXformMove>().MoveOn();
            }
        } else
        {
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayLoseSound();
            }

            if (m_uiShell != null)
            {
                bool retrySelected = false;
                bool levelSelectSelected = false;
                m_uiShell.ShowModal(loseIcon, "You lose", "No moves left", "Retry", () => retrySelected = true, "Levels", () => levelSelectSelected = true);
                while (!retrySelected && !levelSelectSelected)
                {
                    yield return null;
                }

                m_uiShell.HideModal();
                if (levelSelectSelected)
                {
                    SceneFlow.LoadLevelSelect();
                }
                else
                {
                    SceneFlow.RetryLevel();
                }

                yield break;
            }
            else if (messageWindow != null)
            {
                messageWindow.gameObject.SetActive(true);
                messageWindow.ShowMessage(loseIcon, "You Lose", "OK");
                messageWindow.GetComponent<RectXformMove>().MoveOn();
            }
        }

        yield return new WaitForSeconds(1f);

        if (screenFader != null)
        {
            screenFader.FadeOn();
        }

        while (!m_isReadyToReplay)
        {
            yield return null;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReadyToReplay()
    {
        m_isReadyToReplay = true;
    }

    bool HasReachedScoreGoal()
    {
        return ScoreManager.Instance != null && ScoreManager.Instance.CurrentScore >= scoreGoal;
    }

    string BuildWinMessage(int finalScore, int earnedStars)
    {
        string stars = earnedStars > 0 ? earnedStars + "/3 stars" : "Complete";
        return "Score: " + finalScore + "\n" + stars;
    }

    void HideLegacyMessageWindow()
    {
        if (messageWindow != null)
        {
            messageWindow.gameObject.SetActive(false);
        }
    }

    IEnumerator WaitForRuntimeModal(Sprite icon, string title, string body, string primaryLabel)
    {
        bool isReady = false;
        m_uiShell.ShowModal(icon, title, body, primaryLabel, () => isReady = true);

        while (!isReady)
        {
            yield return null;
        }

        m_uiShell.HideModal();
        m_isReadyToBegin = true;
    }

    public void PauseGame()
    {
        if (m_uiShell == null || m_isGameOver || m_isPaused)
        {
            return;
        }

        m_isPaused = true;
        Time.timeScale = 0f;
        if (m_board != null)
        {
            m_board.SetPlayerInputEnabled(false);
        }
        m_uiShell.ShowPauseMenu(ResumeGame, RetryLevel, OpenSettingsFromPause, ReturnToLevelSelect);
    }

    public void ResumeGame()
    {
        if (m_uiShell != null)
        {
            m_uiShell.HidePauseMenu();
        }

        Time.timeScale = 1f;
        m_isPaused = false;
        if (m_board != null)
        {
            m_board.SetPlayerInputEnabled(true);
        }
    }

    public void RetryLevel()
    {
        Time.timeScale = 1f;
        SceneFlow.RetryLevel();
    }

    public void ReturnToLevelSelect()
    {
        Time.timeScale = 1f;
        SceneFlow.LoadLevelSelect();
    }

    void OpenSettingsFromPause()
    {
        if (m_uiShell != null)
        {
            m_uiShell.ShowSettingsOverlay(null);
        }
    }
}
