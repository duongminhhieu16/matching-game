﻿using System;
using UnityEngine;

[Serializable]
public class User
{
    public string userName;
    public int userScore;
    public string email;
    public string id;

    public User()
    {
        userScore = 0;
    }

    public User(string userName, string email, int score, string id)
    {
        this.userName = userName;
        this.email = email;
        this.userScore = score;
        this.id = id;
    }

    public void LogPrint()
    {
        Debug.Log("XX" + userName + " " + userScore);
    }

    
}