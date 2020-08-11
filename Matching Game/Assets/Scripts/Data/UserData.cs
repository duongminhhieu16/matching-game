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
    public User()
    {
        userScore = 0;
    }

    public User(string id, string userName, string email, int score, string profileURL)
    {
        this.userName = userName;
        this.email = email;
        this.userScore = score;
        this.id = id;
        this.profileURL = profileURL;
    }

    public void LogPrint()
    {
        Debug.Log("XX" + userName + " " + userScore);
    }

    
}
