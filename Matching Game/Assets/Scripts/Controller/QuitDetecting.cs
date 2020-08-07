using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitDetecting : MonoBehaviour
{
    private FirebaseInit firebase;
    public static GameObject instance;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null)
            instance = gameObject;
        else
            Destroy(gameObject);
    }
    private void OnApplicationQuit()
    {
        GoogleController.google.GoogleSignOut();
        FacebookController.facebookController.FBLogOut();
    }
}
