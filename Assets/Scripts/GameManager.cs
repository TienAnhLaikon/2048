﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Board board;
    public CanvasGroup gameOver;

    // gameManager sẽ quản lý UI nữa
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestScoreText;
    private int score = 0;
    void Start()
    {
        NewGame();
    }
    public void NewGame()
    {
        SetScore(100);
        bestScoreText.text = LoadBestScore().ToString();
        gameOver.alpha = 0f;
        gameOver.interactable = false;
        board.ClearBoard();
        board.CreateTile();
        board.CreateTile();
        board.enabled = true;
    }
    public void GameOver()
    {
        board.enabled = false;
        gameOver.interactable = true;
        StartCoroutine(Fade(gameOver, 1f, 1f));
    }
    public IEnumerator Fade(CanvasGroup canvasGroup, float to, float delay)
    {
        yield return new WaitForSeconds(delay);
        float elapsed = 0f;
        float duration = 0.5f;
        float from = canvasGroup.alpha;

        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = to;
    }
    public void IncreaseScore(int points)
    {
        SetScore(score + points);
    }
    public void SetScore(int newScore)
    {
        score = newScore;
        scoreText.text = score.ToString();
        SaveBestScore();
    }
    private void SaveBestScore()
    {
        int bestScore = LoadBestScore();
        if (score > bestScore)
        {
            PlayerPrefs.SetInt("bestscore", score);
        }
    }
    private int LoadBestScore()
    {
        return PlayerPrefs.GetInt("bestscore", 0);
    }
}
