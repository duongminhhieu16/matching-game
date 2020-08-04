using Facebook.Unity;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FacebookController : MonoBehaviour
{
    public GameObject DialogLoggedIn;
    public GameObject DialogLoggedOut;
    public GameObject DialogUserName;
    public GameObject DialogProfilePic;

    public static string facebookID = "a";
    public static string facebookPlayerName = "a";
    public static int cnt = 0;
    // Start is called before the first frame update
    void Awake()
    {
        facebookController = this;
        if (!FB.IsLoggedIn && cnt == 0)
        {
            FB.Init(SetInit, OnHideUnity);
            cnt++;
        }
        else if(FB.IsLoggedIn && cnt > 0)
        {
            HandleFBMenus(FB.IsLoggedIn);
        }
       
        GetStatus();
    }
    public static GameObject instance;
    public static FacebookController facebookController { get; private set; }
    
    public void GetStatus()
    {
        if (FB.IsLoggedIn)
        {
            if (!DialogLoggedIn.activeSelf)
            {
                DialogLoggedIn.SetActive(true);
                DialogLoggedOut.SetActive(false);
            }
        }
        else
        {
            if (!DialogLoggedOut.activeSelf)
            {
                DialogLoggedIn.SetActive(false);
                DialogLoggedOut.SetActive(true);
            }
        }
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
        System.Collections.Generic.List<string> permissions = new System.Collections.Generic.List<string>();
        permissions.Add("public_profile");
        permissions.Add("email");
        FB.LogInWithReadPermissions(permissions, AuthCallBack);
    }
    public void FBLogOut()
    {
        FB.LogOut();
        PlayerPrefs.SetInt("FBLogOut", 1);
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
            FB.API("/me/picture?type=square&height=100&width=100", HttpMethod.GET, DisplayProfilePicture);
            facebookID = AccessToken.CurrentAccessToken.UserId;
        }
        else
        {
            DialogLoggedIn.SetActive(false);
            DialogLoggedOut.SetActive(true);
        }
    }
    void DisplayUserName(IResult result)
    {
        TextMeshProUGUI userName = DialogUserName.GetComponent<TextMeshProUGUI>();
        if(result.Error == null)
        {
            userName.text = "Hi there, \n" + result.ResultDictionary["name"];
            facebookPlayerName = "" + result.ResultDictionary["name"];
        }
        else
        {
            Debug.LogError(result.Error);
        }
    }
    void DisplayProfilePicture(IGraphResult result)
    {
        
        if(result.Texture != null)
        {
            Image profilePic = DialogProfilePic.GetComponent<Image>();
            profilePic.sprite = Sprite.Create(result.Texture, new Rect(0, 0, 100, 100), new Vector2());
        }
        else
        {
            Debug.Log("Loading profile picture error!");
        }
    }
    
}
