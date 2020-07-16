using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI highScoreText;
    private void OnEnable()
    {
        int score = PlayerPrefs.GetInt("score");
        int highScore;
        
        
        if (PlayerPrefs.HasKey("highScore"))
        {
            highScore = PlayerPrefs.GetInt("highScore");
        }
        else 
        {
            highScore = 0; 
        }
        if (score > highScore)
        {
            PlayerPrefs.SetInt("highScore", score);
            highScoreText.text = score.ToString();
        }
        else
        {
            highScoreText.text = highScore.ToString();
        }
        //if (nextLevel) PlayerPrefs.SetInt("score", score);
        //else PlayerPrefs.SetInt("score", 0);
    }


    static void ResetHighScore()
    {
        PlayerPrefs.SetInt("highScore", 0);
        PlayerPrefs.SetInt("score", 0);
    }

}
