using TMPro;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SocialPlatforms.Impl;

public class ScorePresenter : MonoBehaviour
{
    public TextMeshProUGUI movesText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI goalText;
    public TextMeshProUGUI highscoreText;
    public TextMeshProUGUI levelText;
    public ScoreController scoreController;
    private int currentLevel;
    private int _win;
    private void Awake()
    {
        _win = PlayerPrefs.GetInt("win");
        currentLevel = FirebaseInit.playerInfo.currentLevel;
        levelText.text = "Level " + ScoreData.level;
        scoreController.scoreData.Score = PlayerPrefs.GetInt("score");

        /*scoreController.scoreData.Score = PlayerPrefs.GetInt("score");
        scoreController.scoreData.NumMoves = scoreController.scoreData.startingMoves;
        PlayerPrefs.SetInt("numMoves", scoreController.scoreData.startingMoves);
        scoreController.scoreData.Goal = ScoreData.level * 30;
        PlayerPrefs.SetInt("goal", scoreController.scoreData.Goal);
        scoreController.scoreData.HighScore = PlayerPrefs.GetInt("highScore");*/

    }

    private void OnEnable()
    {
        
        PlayerPrefs.SetInt("score", scoreController.scoreData.Score);
        PlayerPrefs.SetInt("win", ScoreData.level);
        
        //if (nextLevel) PlayerPrefs.SetInt("score", score);
        //else PlayerPrefs.SetInt("score", 0);
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
    private async void Update()
    {
        if (scoreController.scoreData.Score > scoreController.scoreData.HighScore)
        {
            PlayerPrefs.SetInt("highScore", scoreController.scoreData.Score);
            scoreController.scoreData.HighScore = scoreController.scoreData.Score;
        }
        if (scoreController.scoreData.Score >= scoreController.scoreData.Goal)
        {
            currentLevel = ScoreData.level+1;
            if(ScoreData.level+1 > FirebaseInit.playerInfo.userHighestLevel) FirebaseInit.UpdateLevel(ScoreData.level+1);
            PlayerPrefs.SetInt("win", _win + 1);
            await Task.Delay(300);
            scoreController.CheckIfGameEnd();
        }
        else
        {
            if (scoreController.scoreData.NumMoves <= 0)
            {
                scoreController.scoreData.NumMoves = 0;
                ScoreData.level = 1;
                PlayerPrefs.SetInt("win", 1);
                _win = 1;
                scoreController.CheckIfGameEnd();
            }
        }
        ReadScore();
        DisplayScore();
        scoreController.UpdateScoreToDatabase();
        FirebaseInit.UpdateCurrentStatus(scoreController.scoreData.Score, currentLevel, ScoreData.currentNumMoves);
    }
 
}
