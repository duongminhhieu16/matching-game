using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class ScorePresenter : MonoBehaviour
{
    public TextMeshProUGUI movesText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI goalText;
    public TextMeshProUGUI highscoreText;

    private bool win = false;
    public ScoreData scoreData = new ScoreData();
    private void Awake()
    {
        int level = PlayerPrefs.GetInt("win");
        if (level != 1) scoreData.Score = PlayerPrefs.GetInt("score");
        else scoreData.Score = 0;

        scoreData.NumMoves = scoreData.startingMoves;
        PlayerPrefs.SetInt("numMoves", scoreData.startingMoves);
        scoreData.Goal = level*100;
        PlayerPrefs.SetInt("goal", scoreData.Goal);
        scoreData.HighScore = PlayerPrefs.GetInt("highScore");
    }
    private void OnEnable()
    {
        
        PlayerPrefs.SetInt("score", scoreData.Score);
        PlayerPrefs.SetInt("win", PlayerPrefs.GetInt("win"));
        
        //if (nextLevel) PlayerPrefs.SetInt("score", score);
        //else PlayerPrefs.SetInt("score", 0);
    }
    private void Update()
    {
        if (scoreData.Score > scoreData.HighScore)
        {
            PlayerPrefs.SetInt("highScore", scoreData.Score);
            scoreData.HighScore = scoreData.Score;
        }
        if (scoreData.Score >= scoreData.Goal)
        {
            PlayerPrefs.SetInt("win", PlayerPrefs.GetInt("win") + 1);
            win = true;
            CheckIfGameEnd();
        }
        else
        {
            if (scoreData.NumMoves <= 0)
            {
                scoreData.NumMoves = 0;
                PlayerPrefs.SetInt("win", 1);
                win = false;
                CheckIfGameEnd();
            }
        }
        scoreData.Score = PlayerPrefs.GetInt("score");
        scoreData.Goal = PlayerPrefs.GetInt("goal");
        scoreData.NumMoves = PlayerPrefs.GetInt("numMoves");
        scoreData.HighScore = PlayerPrefs.GetInt("highScore");
        scoreText.text = scoreData.Score.ToString();
        movesText.text = scoreData.NumMoves.ToString();
        goalText.text = scoreData.Goal.ToString();
        highscoreText.text = scoreData.HighScore.ToString();
    }
    private void CheckIfGameEnd()
    {
        if (win)
        {
            Debug.Log("YOU WIN!!!");
            PlayerPrefs.SetInt("score", scoreData.Score);
            win = false;
            SceneManager.LoadScene(2);
        }
        else
        {
            Debug.Log("YOU LOSE!!!!!!!!!!!!!!!!!!!!!!");
            PlayerPrefs.SetInt("score", 0);
            SceneManager.LoadScene(3);
        }
    }
}
