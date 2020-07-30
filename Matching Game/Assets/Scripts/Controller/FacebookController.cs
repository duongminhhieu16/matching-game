using Facebook.Unity;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class FacebookController : MonoBehaviour
{
    public GameObject DialogLoggedIn;
    public GameObject DialogLoggedOut;
    public GameObject DialogUserName;
    // Start is called before the first frame update
    void Awake()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(() =>
            {
                if (FB.IsInitialized)
                    FB.ActivateApp();
                else
                    Debug.Log("FB initialize error");
            },
                isGameShown => { Time.timeScale = !isGameShown ? 0 : 1; });
        }
        else FB.ActivateApp();
    }
    void SetInit()
    {
        if (FB.IsLoggedIn)
        {
            Debug.Log("FB is logged in!");
        }
        else
        {
            Debug.Log("FB is not logged in!!@!@!@!@!@!@");
        }
        HandleFBMenus(FB.IsLoggedIn);
    }
    void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
    public void FBLogIn()
    {
        List<string> permissions = new List<string>();
        permissions.Add("public_profile");
        FB.LogInWithReadPermissions(permissions, AuthCallBack);
    }
    void AuthCallBack(IResult result)
    {
        if(result.Error != null)
        {
            Debug.Log(result.Error);
        }
        else
        {
            if (FB.IsLoggedIn)
            {
                Debug.Log("FB is logged in");
            }
            else
            {
                Debug.Log("FB is not logged in!@!");
            }
            HandleFBMenus(FB.IsLoggedIn);
        }
    }
    void HandleFBMenus(bool isLoggedIn)
    {
        if (isLoggedIn)
        {
            DialogLoggedIn.SetActive(true);
            DialogLoggedOut.SetActive(false);
            FB.API("/me?fields=name", HttpMethod.GET, DisplayUserName);
        }
        else
        {
            DialogLoggedIn.SetActive(false);
            DialogLoggedOut.SetActive(true);
        }
    }
    void DisplayUserName(IGraphResult result)
    {
        TextMeshProUGUI userName = DialogUserName.GetComponent<TextMeshProUGUI>();
        if(result.Error == null)
        {
            userName.text = "Hi there, \n" + result.ResultDictionary["name"];
        }
        else
        {
            Debug.LogError(result.Error);
        }
    }
}
