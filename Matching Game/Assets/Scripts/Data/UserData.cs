using System;
using UnityEngine;

[Serializable]
public class User
{
    public string userName;
    public int userScore;
    public string email;
    public string id;
    public string profileURL;
    public int userHighestLevel;
    public int currentLevel;
    public int currentScore;
    public int currentNumMoves;
    public User()
    {
        userScore = 0;
    }

    public User(string id, string userName, string email, int score, string profileURL, int level, int currentLevel, int currentScore, int currentNumMoves)
    {
        this.userName = userName;
        this.email = email;
        this.userScore = score;
        this.id = id;
        this.profileURL = profileURL;
        userHighestLevel = level;
        this.currentLevel = currentLevel;
        this.currentScore = currentScore;
        this.currentNumMoves = currentNumMoves;
    }

    public void LogPrint()
    {
        Debug.Log("XX" + userName + " " + userScore);
    }

    
}
