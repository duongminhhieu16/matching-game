using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
public class ScorePresenter : MonoBehaviour
{
    public TextMeshProUGUI movesText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI goalText;
    public TextMeshProUGUI highscoreText;
    public ScoreController scoreController;
    private void Awake()
    {
        int level = PlayerPrefs.GetInt("win");
        if (level != 1) scoreController.scoreData.Score = PlayerPrefs.GetInt("score");
        else scoreController.scoreData.Score = 0;

        scoreController.scoreData.NumMoves = scoreController.scoreData.startingMoves;
        PlayerPrefs.SetInt("numMoves", scoreController.scoreData.startingMoves);
        scoreController.scoreData.Goal = level*100;
        PlayerPrefs.SetInt("goal", scoreController.scoreData.Goal);
        scoreController.scoreData.HighScore = PlayerPrefs.GetInt("highScore");
    }
    private void OnEnable()
    {
        
        PlayerPrefs.SetInt("score", scoreController.scoreData.Score);
        PlayerPrefs.SetInt("win", PlayerPrefs.GetInt("win"));
        
        //if (nextLevel) PlayerPrefs.SetInt("score", score);
        //else PlayerPrefs.SetInt("score", 0);
    }
    private void Update()
    {
        if (scoreController.scoreData.Score > scoreController.scoreData.HighScore)
        {
            PlayerPrefs.SetInt("highScore", scoreController.scoreData.Score);
            scoreController.scoreData.HighScore = scoreController.scoreData.Score;
        }
        if (scoreController.scoreData.Score >= scoreController.scoreData.Goal)
        {
            PlayerPrefs.SetInt("win", PlayerPrefs.GetInt("win") + 1);
            scoreController.CheckIfGameEnd();
        }
        else
        {
            if (scoreController.scoreData.NumMoves <= 0)
            {
                scoreController.scoreData.NumMoves = 0;
                PlayerPrefs.SetInt("win", 1);
                scoreController.CheckIfGameEnd();
            }
        }
        ReadScore();
        DisplayScore();
        scoreController.UpdateScoreFromDatabase();
    }
    
    
    public void ReadScore()
    {
        scoreController.scoreData.Score = PlayerPrefs.GetInt("score");
        scoreController.scoreData.Goal = PlayerPrefs.GetInt("goal");
        scoreController.scoreData.NumMoves = PlayerPrefs.GetInt("numMoves");
        scoreController.scoreData.HighScore = PlayerPrefs.GetInt("highScore");
    }
    public void DisplayScore()
    {
        scoreText.text = scoreController.scoreData.Score.ToString();
        movesText.text = scoreController.scoreData.NumMoves.ToString();
        goalText.text = scoreController.scoreData.Goal.ToString();
        highscoreText.text = scoreController.scoreData.HighScore.ToString();
    }
}
