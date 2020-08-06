using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Boo.Lang;
using System.Threading.Tasks;
using Facebook.Unity;
using Firebase.Auth;

public class FirebaseInit : MonoBehaviour
{
    public static DatabaseReference reference;
    public static string guestID;
    public static int highscoreOfUser;
    public static User playerInfo;
    public static List<User> users = new List<User>();
    

    // Start is called before the first frame update
   
    private void Awake()
    {
        firebaseInit = this;
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
        GoogleController.auth = FirebaseAuth.DefaultInstance;
        LoadHighScoreOfCurrentPlayer();
    }
    public static FirebaseInit firebaseInit { get; private set; }
    
    public void CreatePlayer()
    {
        if (FB.IsLoggedIn) CreateFacebookPlayer();
        else if (PlayerPrefs.GetInt("Google") == 1) CreateGooglePlayer();
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
        await Task.Delay(100);
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
        await Task.Delay(100);
        await LoadHighScoreOfCurrentPlayer();
    }
    private async void CreateGooglePlayer()
    {
        var googlePlayer = GoogleController.auth.CurrentUser;
        
        await FirebaseDatabase.DefaultInstance.GetReference("users").Child(googlePlayer.UserId)
             .GetValueAsync().ContinueWith(task =>
             {
                 DataSnapshot d = task.Result;
                 User u = JsonUtility.FromJson<User>(d.GetRawJsonValue());
                 if (u == null)
                     CreatePlayerInfo(googlePlayer.UserId, googlePlayer.DisplayName);
             });

        await Task.Delay(100);
        await LoadHighScoreOfCurrentPlayer();
    }
    public static async Task LoadHighScoreOfCurrentPlayer()
    {
        int status = PlayerPrefs.GetInt("Google");
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
                        else if(status == 1)
                        {
                            if (GoogleController.auth.CurrentUser.UserId == us.id)
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
        else if(PlayerPrefs.GetInt("Google") == 1) reference.Child("users").Child(GoogleController.auth.CurrentUser.UserId).Child("userScore").SetValueAsync(score);
        else reference.Child("users").Child(guestID).Child("userScore").SetValueAsync(score);
    }
    public static void CreatePlayerInfo(string id, string name)
    {
        playerInfo = new User(name, "email@gmail.com", 0, id);
        string json = JsonUtility.ToJson(playerInfo);
        reference.Child("users").Child(id).SetRawJsonValueAsync(json);
    }

}
