using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameOverHandler gameOverHandler; 
    private bool isGameOver = false;
    private float playtime = 0f;

    private void Awake()
    {
        gameOverHandler ??= GetComponentInChildren<GameOverHandler>();
    }

    private void Update()
    {
        if (!isGameOver)
        {
            playtime += Time.deltaTime;
        }

        //For Debug
        if (Input.GetKeyDown(KeyCode.R) && !isGameOver)
        {
            EndGame();
        }
    }

    public void EndGame()
    {
        isGameOver = true;
        gameOverHandler.OnGameOver(playtime);
    }
}