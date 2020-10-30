using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreController : MonoBehaviour
{
    public ScoreData scoreData = new ScoreData();
    private void Awake()
    {
        scoreData.NumMoves = ScoreData.currentNumMoves;
        
        scoreData.Goal += 100 + (ScoreData.level-1)*10;
        PlayerPrefs.SetInt("goal", scoreData.Goal);
        scoreData.HighScore = PlayerPrefs.GetInt("highScore");
    }
    public void UpdateScoreToDatabase()
    {
        if (scoreData.Score > FirebaseInit.playerInfo.userScore)
        {
            FirebaseInit.playerInfo.userScore = scoreData.Score;
            FirebaseInit.UpdateScore(scoreData.Score);
        }
        
        PlayerPrefs.SetInt("highScore", FirebaseInit.playerInfo.userScore);
    }
    public void CheckIfGameEnd()
    {
        bool win = PlayerPrefs.GetInt("win") > 1;
        if (win)
        {
            PlayerPrefs.SetInt("score", scoreData.Score);
            SceneManager.LoadScene(2);
        }
        else
        {
            PlayerPrefs.SetInt("score", 0);
            SceneManager.LoadScene(3);
        }
    }
    
}
