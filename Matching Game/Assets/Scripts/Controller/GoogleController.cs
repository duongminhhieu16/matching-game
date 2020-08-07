using UnityEngine;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;
using Facebook.Unity;
using UnityEngine.UI;
using System.Collections;
using Google;
using TMPro;
using UnityEngine.Networking;
using Firebase.Extensions;
using System.Xml.XPath;

public class GoogleController : MonoBehaviour
{
    public GameObject DialogSignedIn;
    public GameObject DialogSignedOut;
    public GameObject DialogUserName;
    public GameObject DialogProfilePic;
    private Texture2D profilePic;
    public static FirebaseAuth auth;
    public static int cnt = 0;
    public static GoogleController google { get; private set; }
    private void Awake()
    {
        google = this;
        GetStatus();
    }
    public void GetStatus()
    {
        
        if (!FB.IsLoggedIn)
        {
            if (PlayerPrefs.GetInt("Google") == 1)
            {
                Debug.Log("in");
                DialogSignedIn.SetActive(true);
                DialogSignedOut.SetActive(false);
            }
            else
            {
                DialogSignedIn.SetActive(false);
                DialogSignedOut.SetActive(true);
            }
        }
        else
        {
            DialogSignedIn.SetActive(false);
            DialogSignedOut.SetActive(false);
            Debug.Log("hehehehehe");
        }
    }
    public void GoogleLogIn()
    {
        auth = FirebaseAuth.DefaultInstance;
        GoogleSignIn.Configuration = new GoogleSignInConfiguration
        {
            RequestIdToken = true,
            // Copy this value from the google-service.json file.
            // oauth_client with type == 3
            WebClientId = "471308679030-umh5t7e244umvfkp29i05f24av0ojvjf.apps.googleusercontent.com"
        };
        Task<GoogleSignInUser> signIn = GoogleSignIn.DefaultInstance.SignIn();
        
        TaskCompletionSource<FirebaseUser> signInCompleted = new TaskCompletionSource<FirebaseUser>();
        signIn.ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
                signInCompleted.SetCanceled();
            }
            else if (task.IsFaulted)
            {
                signInCompleted.SetException(task.Exception);
            }
            else
            {
                Credential credential = GoogleAuthProvider.GetCredential(((Task<GoogleSignInUser>)task).Result.IdToken, null);
                auth.SignInWithCredentialAsync(credential).ContinueWith(authTask => {
                    if (authTask.IsCanceled)
                    {
                        signInCompleted.SetCanceled();
                    }
                    else if (authTask.IsFaulted)
                    {
                        signInCompleted.SetException(authTask.Exception);
                    }
                    else
                    {
                        signInCompleted.SetResult(((Task<FirebaseUser>)authTask).Result);
                    }
                });
                
            }
            DisplayUserName(true, signIn.Result);
            DisplayUserProfilePic(true, signIn.Result);
        });
        
        PlayerPrefs.SetInt("Google", 1);
    }

    public void GoogleSignOut()
    {
        var user = auth.CurrentUser;
        if (user == null) return;
        auth.SignOut();
        GoogleSignIn.DefaultInstance.SignOut();
        PlayerPrefs.SetInt("Google", 0);
    }
    public void DisplayUserName(bool completed, GoogleSignInUser result)
    {

        var user = result.DisplayName;
        if (completed)
        {
            TextMeshProUGUI userName = DialogUserName.GetComponent<TextMeshProUGUI>();
            userName.text = "Hi there \n" + user.ToString();
            Debug.Log(userName.text.ToString());
            
        }
    }
    public IEnumerator DisplayUserProfilePic(bool completed, GoogleSignInUser result)
    {
        var url = result.ImageUrl;
        
        if (completed)
        {
            yield return DownloadImage(url.ToString());
            Image pic = DialogProfilePic.GetComponent<Image>();
            pic.sprite = Sprite.Create(profilePic, new Rect(0, 0, 100, 100), new Vector2());
        }
    }
    IEnumerator DownloadImage(string MediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
        {
            profilePic = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }

    }
}
