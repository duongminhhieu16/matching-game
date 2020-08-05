using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity.Editor;
using Boo.Lang;
using System.Threading.Tasks;
using Facebook.Unity;
using UnityEngine.UI;
using System.Collections;
using Google;
using TMPro;
using UnityEngine.Networking;
using Facebook.MiniJSON;

public class FirebaseInit : MonoBehaviour
{
    public static DatabaseReference reference;
    public static string guestID;
    public static int highscoreOfUser;
    public static User playerInfo;
    public static List<User> users = new List<User>();
    public GameObject DialogUserName;
    public GameObject DialogProfilePic;
    private Texture2D profilePic;
    
    // Start is called before the first frame update
    private void Awake()
    {
        if(reference != null)
        {          
            return;
        }
        else
        {
            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://match-3-game-81f2c.firebaseio.com/");
            reference = FirebaseDatabase.DefaultInstance.RootReference;
            guestID = SystemInfo.deviceUniqueIdentifier;
        }
        
        // check if this user exists
    }
    public void CreatePlayer()
    {
        if (FB.IsLoggedIn) CreateFacebookPlayer();
        else CreateGuestPlayer();
    }
    private async void CreateGuestPlayer()
    {
        await FirebaseDatabase.DefaultInstance.GetReference("users").Child(guestID)
            .GetValueAsync().ContinueWith(task =>
            {
                DataSnapshot d = task.Result;
                System.Random rand = new System.Random();
                int num = rand.Next(100000);
                User u = JsonUtility.FromJson<User>(d.GetRawJsonValue());
                if (u == null)
                    CreatePlayerInfo(guestID, "Guest" + num.ToString());
                Debug.Log("create guest");
            });
        await LoadHighScoreOfCurrentPlayer();
    }
    private async void CreateFacebookPlayer()
    {
        await FirebaseDatabase.DefaultInstance.GetReference("users").Child(FacebookController.facebookID)
             .GetValueAsync().ContinueWith(task =>
             {
                 DataSnapshot d = task.Result;
                 User u = JsonUtility.FromJson<User>(d.GetRawJsonValue());
                 if (u == null)
                     CreatePlayerInfo(FacebookController.facebookID, FacebookController.facebookPlayerName);
             });
        await LoadHighScoreOfCurrentPlayer();
    }
    public void GoogleLogIn()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        
        GoogleSignIn.Configuration = new GoogleSignInConfiguration
        {
            RequestIdToken = true,
            // Copy this value from the google-service.json file.
            // oauth_client with type == 3
            WebClientId = "471308679030-umh5t7e244umvfkp29i05f24av0ojvjf.apps.googleusercontent.com"
        };

        Task<GoogleSignInUser> signIn = GoogleSignIn.DefaultInstance.SignIn();

        TaskCompletionSource<FirebaseUser> signInCompleted = new TaskCompletionSource<FirebaseUser>();
        signIn.ContinueWith(task => {
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
            DisplayUserName(task.IsCompleted);
            DisplayUserProfilePic(task.IsCompleted);
        });
    }
    public void GoogleSignOut()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        auth.SignOut();
    }
    public void DisplayUserName(bool completed)
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        FirebaseUser user = auth.CurrentUser;
        if (completed)
        {
            TextMeshProUGUI userName = DialogUserName.GetComponent<TextMeshProUGUI>();
            userName.text = "Hi there, \n" + user.DisplayName;
        }
    }
    public void DisplayUserProfilePic(bool completed)
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        FirebaseUser user = auth.CurrentUser;
        if (completed)
        {
            DownloadImage(user.PhotoUrl.ToString());
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
    public static async Task LoadHighScoreOfCurrentPlayer()
    {
        await FirebaseDatabase.DefaultInstance.GetReference("users")
            .GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log("Error loaded users info!!!");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;

                    foreach (var user in snapshot.Children)
                    {
                        User us = JsonUtility.FromJson<User>(user.GetRawJsonValue());
                        if (FB.IsLoggedIn)
                        {
                            if (FacebookController.facebookID == us.id)
                            {
                                playerInfo = us;
                                highscoreOfUser = us.userScore;
                            }
                        }
                        else
                        {
                            if (guestID == us.id)
                            {
                                playerInfo = us;
                                highscoreOfUser = us.userScore;
                            }
                        }
                    }
                }
            });
        PlayerPrefs.SetInt("highScore", highscoreOfUser);
    }
    public static async Task LoadHighestScoreUsersInfo(int user_num)
    {
        users.Clear();
        await FirebaseDatabase.DefaultInstance.GetReference("users").OrderByChild("userScore").LimitToLast(user_num)
            .GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log("Error loaded users info!!!");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    int rank = 0;
                    foreach (DataSnapshot user in snapshot.Children)
                    {
                        User us = JsonUtility.FromJson<User>(user.GetRawJsonValue());
                        users.Insert(rank, us);
                        rank++;
                    }
                }
            });
        FirebaseDatabase.DefaultInstance
            .GetReference("users")
            .ValueChanged += HandleValueChanged;
    }
    static void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        // Do something with the data in args.Snapshot
    }

    public static void UpdateScore(int score)
    {
        if (FB.IsLoggedIn) reference.Child("users").Child(FacebookController.facebookID).Child("userScore").SetValueAsync(score);
        else reference.Child("users").Child(guestID).Child("userScore").SetValueAsync(score);
    }
    public static void CreatePlayerInfo(string id, string name)
    {
        playerInfo = new User(name, "email@gmail.com", 0, id);
        string json = JsonUtility.ToJson(playerInfo);
        reference.Child("users").Child(id).SetRawJsonValueAsync(json);
    }

}
