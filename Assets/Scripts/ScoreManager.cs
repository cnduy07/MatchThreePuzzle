using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : Singleton<ScoreManager>
{
    int m_currentScore = 0;
    int m_counterValue = 0;
    int m_increment = 5;

    public TextMeshProUGUI scoreText;

    // Start is called before the first frame update
    void Start()
    {
        UpdateScoreText(m_currentScore);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateScoreText(int scoreValue)
    {
        if (scoreText != null)
        {
            scoreText.text = scoreValue.ToString();
        }
    }

    public void AddScore(int scoreValue)
    {
        m_currentScore += scoreValue;
        StartCoroutine(CountScoreRoutine());
    }

    IEnumerator CountScoreRoutine()
    {
        int interations = 0;

        while (interations < 100000 && m_counterValue < m_currentScore)
        {
            m_counterValue += m_increment;
            UpdateScoreText(m_counterValue);
            interations += 1;
            yield return null;
        }

        m_counterValue = m_currentScore;
    }
}
