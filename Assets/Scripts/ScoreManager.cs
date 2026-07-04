using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : Singleton<ScoreManager>
{
    protected override bool ShouldPersistAcrossScenes
    {
        get
        {
            return false;
        }
    }

    int m_currentScore = 0;

    public int CurrentScore
    {
        get
        {
            return m_currentScore;
        }
    }

    int m_counterValue = 0;
    int m_increment = 5;
    Coroutine m_countScoreRoutine;

    public Text scoreText;

    // Start is called before the first frame update
    void Start()
    {
        UpdateScoreText(m_currentScore);
    }

    public void UpdateScoreText(int scoreValue)
    {
        RuntimeUiShell shell = RuntimeUiShell.Active;
        if (shell == null && scoreText != null)
        {
            scoreText.text = scoreValue.ToString();
        }
        else if (shell != null)
        {
            if (scoreText != null)
            {
                GameObject legacyScoreObject = scoreText.gameObject;
                Transform parent = scoreText.transform.parent;
                if (parent != null && parent.GetComponent<Canvas>() == null)
                {
                    legacyScoreObject = parent.gameObject;
                }

                legacyScoreObject.SetActive(false);
            }

            shell.UpdateGameplayScore(scoreValue);
        }
    }

    public void AddScore(int scoreValue)
    {
        m_currentScore += scoreValue;
        if (m_countScoreRoutine == null)
        {
            m_countScoreRoutine = StartCoroutine(CountScoreRoutine());
        }
    }

    IEnumerator CountScoreRoutine()
    {
        int iterations = 0;

        while (iterations < 100000 && m_counterValue < m_currentScore)
        {
            m_counterValue += m_increment;
            if (m_counterValue > m_currentScore)
            {
                m_counterValue = m_currentScore;
            }

            UpdateScoreText(m_counterValue);
            iterations += 1;
            yield return null;
        }

        m_counterValue = m_currentScore;
        UpdateScoreText(m_counterValue);
        m_countScoreRoutine = null;
    }
}
