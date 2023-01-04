using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MatchValue
{
    First,
    Second,
    Third,
    Fourth,
    Fifth,
    Sixth,
    Seventh,
    Eighth,
    Ninth,
    None
};

public class GamePiece : MonoBehaviour
{
    public int xIndex;
    public int yIndex;

    Board m_board;
    bool isMoving = false;

    public InterpolationType interpolation = InterpolationType.Linear;

    public enum InterpolationType
    {
        Linear,
        EasyOut,
        EasyIn,
        SmoothStep,
        SmootherStep
    };

    public int scoreValue = 20;
    public MatchValue matchValue;
    public AudioClip clearSound;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init(Board board)
    {
        m_board = board;
    }

    private void Update()
    {

    }

    public void SetCoord(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }

    public void Move(int destX, int destY, float timeToMove)
    {
        if (!isMoving)
        {
            StartCoroutine(MoveRoutine(new Vector3((float)destX, (float)destY, 0), timeToMove));
        }
    }

    IEnumerator MoveRoutine(Vector3 destination, float timeToMove)
    {
        Vector3 startPostion = transform.position;
        bool reachedDestination = false;
        float eslapedTime = 0f;

        isMoving = true;

        while (!reachedDestination)
        {
            if (Vector3.Distance(transform.position, destination) <= 0.01f)
            {
                reachedDestination = true;
                m_board.PlaceGamePiece(this, (int)destination.x, (int)destination.y);
                break;
            }

            eslapedTime += Time.deltaTime;

            float t = Mathf.Clamp(eslapedTime / timeToMove, 0f, 1f);

            switch (interpolation) {
                case InterpolationType.Linear:
                    break;
                case InterpolationType.EasyOut:
                    t = Mathf.Sin(t * Mathf.PI * 0.5f);
                    break;
                case InterpolationType.EasyIn:
                    t = 1 - Mathf.Cos(t * Mathf.PI * 0.5f);
                    break;
                case InterpolationType.SmoothStep:
                    t = t * t * (3 - 2 * t);
                    break;
                case InterpolationType.SmootherStep:
                    t = t * t * t * (t * (t * 6 - 15) + 10);
                    break;
            };

            transform.position = Vector3.Lerp(startPostion, destination, t);

            yield return null;
        }

        isMoving = false;
    }

    public void ChangeColor(GamePiece pieceMatched)
    {
        SpriteRenderer rendererToChange = GetComponent<SpriteRenderer>();

        if (pieceMatched != null)
        {
            SpriteRenderer rendererMatched = pieceMatched.GetComponent<SpriteRenderer>();

            if (rendererToChange != null && rendererMatched != null)
            {
                rendererToChange.color = rendererMatched.color;
            }

            matchValue = pieceMatched.matchValue;
        }
    }

    public void AddScore(int multiplier = 0, int bonus = 0)
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(scoreValue * multiplier + bonus);
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayClipAtPoint(clearSound, Vector3.zero);
        }
    }
}
