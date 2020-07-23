using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ScorePresenter : MonoBehaviour
{
    public TextMeshProUGUI movesText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI goalText;
    public TextMeshProUGUI highscoreText;
    private int _score;
    private int _goal;
    private int _highscore;
    private int startingMoves = 30;
    private int remainingMoves;
    private bool win = false;
    public int NumMoves
    {
        get
        {
            return remainingMoves;
        }
        set
        {
            remainingMoves = value;
            movesText.text = remainingMoves.ToString();
        }
    }

    public int Score
    {
        get
        {
            return _score;
        }
        set
        {
            _score = value;
            scoreText.text = _score.ToString();
        }
    }

    public int Goal
    {
        get
        {
            return _goal;
        }
        set
        {
            _goal = value;
            goalText.text = _goal.ToString();
        }
    }

    public int HighScore
    {
        get
        {
            return _highscore;
        }
        set
        {
            _highscore = value;
            highscoreText.text = _highscore.ToString();
        }
    }
    private void Awake()
    {
        int level = PlayerPrefs.GetInt("win");
        if (level != 1) Score = PlayerPrefs.GetInt("score");
        else Score = 0;

        NumMoves = startingMoves;
        PlayerPrefs.SetInt("numMoves", startingMoves);
        Goal = level*100;
        PlayerPrefs.SetInt("goal", Goal);
        HighScore = PlayerPrefs.GetInt("highScore");
    }
    private void OnEnable()
    {
        
        PlayerPrefs.SetInt("score", Score);
        PlayerPrefs.SetInt("win", PlayerPrefs.GetInt("win"));
        scoreText.text = Score.ToString();
        movesText.text = NumMoves.ToString();
        goalText.text = Goal.ToString();
        highscoreText.text = HighScore.ToString();
        //if (nextLevel) PlayerPrefs.SetInt("score", score);
        //else PlayerPrefs.SetInt("score", 0);
    }
    private void Update()
    {
        if (Score > HighScore)
        {
            PlayerPrefs.SetInt("highScore", Score);
            HighScore = Score;
        }
        if (Score >= Goal)
        {
            PlayerPrefs.SetInt("win", PlayerPrefs.GetInt("win") + 1);
            win = true;
            CheckIfGameEnd();
        }
        else
        {
            if (NumMoves <= 0)
            {
                NumMoves = 0;
                PlayerPrefs.SetInt("win", 1);
                win = false;
                CheckIfGameEnd();
            }
        }
        Score = PlayerPrefs.GetInt("score");
        Goal = PlayerPrefs.GetInt("goal");
        NumMoves = PlayerPrefs.GetInt("numMoves");
        HighScore = PlayerPrefs.GetInt("highScore");
    }
    private void CheckIfGameEnd()
    {
        if (win)
        {
            Debug.Log("YOU WIN!!!");
            PlayerPrefs.SetInt("score", Score);
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
