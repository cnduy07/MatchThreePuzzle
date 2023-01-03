using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public int movesLeft = 30;
    public int scoreGoal = 10000;
    public ScreenFader screenFader;
    public Text levelNameText;
    public Text movesLeftText;

    bool m_isReadyToBegin = false;
    bool m_isGameOver = false;
    bool m_isWinner = false;
    bool m_isReadyToReplay = false;

    public Board m_board;

    public MessageWindow messageWindow;
    public Sprite goalIcon;
    public Sprite winIcon;
    public Sprite loseIcon;

    // Start is called before the first frame update
    void Start()
    {
        m_board = FindObjectOfType<Board>().GetComponent<Board>();

        if (levelNameText != null)
        {
            Scene scene = SceneManager.GetActiveScene();
            levelNameText.text = scene.name;
        }

        UpdateMoves();

        StartCoroutine("ExecuteGameLoop");
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
        if (messageWindow != null)
        {
            messageWindow.ShowMessage(goalIcon, "Your goal \n" + scoreGoal.ToString(), "start");
            messageWindow.GetComponent<RectXformMove>().MoveOn();
        }

        while (!m_isReadyToBegin)
        {
            yield return null;
        }

        if (screenFader != null)
        {
            screenFader.FadeOff();
        }

        yield return new WaitForSeconds(0.5f);

        if (m_board != null)
        {
            m_board.SetupBoard();
        }
    }

    IEnumerator PlayGameRoutine()
    {
        while (!m_isGameOver)
        {
            if (ScoreManager.Instance != null)
            {
                if (ScoreManager.Instance.CurrentScore >= scoreGoal)
                {
                    m_isGameOver = true;
                    m_isWinner = true;
                }
            }

            if (movesLeft == 0)
            {
                m_isGameOver = true;
                m_isWinner = false;
            }
            yield return null;
        }
    }

    IEnumerator EndGameRoutine()
    {
        m_isReadyToReplay = false;

        if (m_isWinner)
        {
            if (messageWindow != null)
            {
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayWinSound();
                }
                messageWindow.ShowMessage(winIcon, "You win", "OK");
                messageWindow.GetComponent<RectXformMove>().MoveOn();
            }
        } else
        {
            if (messageWindow != null)
            {
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayLoseSound();
                }
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
}
