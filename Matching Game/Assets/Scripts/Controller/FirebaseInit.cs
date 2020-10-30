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
    public static User playerInfo;
    public static bool loaded = false;
    public static List<User> users = new List<User>();
    

    // Start is called before the first frame update
   
    private async void Awake()
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
        await LoadDataOfCurrentPlayer();
    }
    public static FirebaseInit firebaseInit { get; private set; }
    
    public async void CreatePlayer()
    {
        if (FB.IsLoggedIn) await CreateFacebookPlayer();
        else if (PlayerPrefs.GetInt("Google") == 1) await CreateGooglePlayer();
        else await CreateGuestPlayer();
    }
    private async Task CreateGuestPlayer()
    {
        
        await FirebaseDatabase.DefaultInstance.GetReference("users").Child(guestID)
            .GetValueAsync().ContinueWith(task =>
            {
                DataSnapshot d = task.Result;
                System.Random rand = new System.Random();
                int num = rand.Next(100000);
                User u = JsonUtility.FromJson<User>(d.GetRawJsonValue());
                if (u == null)
                    CreatePlayerInfo(guestID, "Guest" + num.ToString(), "", "");
                Debug.Log("create guest");
            });
        await LoadDataOfCurrentPlayer();
    }
    private async Task CreateFacebookPlayer()
    {
        
        await FirebaseDatabase.DefaultInstance.GetReference("users").Child(FacebookController.facebookID)
             .GetValueAsync().ContinueWith(task =>
             {
                 DataSnapshot d = task.Result;
                 User u = JsonUtility.FromJson<User>(d.GetRawJsonValue());
                 if (u == null)
                     CreatePlayerInfo(FacebookController.facebookID, FacebookController.facebookPlayerName, "facebook", "http://graph.facebook.com/" + FacebookController.facebookID + "/picture");
             });
        await Task.Delay(100);
        await LoadDataOfCurrentPlayer();
    }
    private async Task CreateGooglePlayer()
    {
        var googlePlayer = GoogleController.auth.CurrentUser;
        
        await FirebaseDatabase.DefaultInstance.GetReference("users").Child(googlePlayer.UserId)
             .GetValueAsync().ContinueWith(task =>
             {
                 DataSnapshot d = task.Result;
                 User u = JsonUtility.FromJson<User>(d.GetRawJsonValue());
                 if (u == null)
                     CreatePlayerInfo(googlePlayer.UserId, googlePlayer.DisplayName, "google", googlePlayer.PhotoUrl.ToString());
             });

        await Task.Delay(100);
        await LoadDataOfCurrentPlayer();
    }
    public static async Task LoadDataOfCurrentPlayer()
    {
        loaded = false;
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
                    loaded = true;
                    foreach (var user in snapshot.Children)
                    {
                        User us = JsonUtility.FromJson<User>(user.GetRawJsonValue());
                        if (FB.IsLoggedIn)
                        {
                            if (FacebookController.facebookID == us.id)
                            {
                                playerInfo = us;
                            }
                        }
                        else if(status == 1)
                        {
                            if (GoogleController.auth.CurrentUser.UserId == us.id)
                            {
                                playerInfo = us;
                            }
                        }
                        else
                        {
                            if (guestID == us.id)
                            {
                                playerInfo = us;
                            }
                        }
                    }
                }
            });
        PlayerPrefs.SetInt("highScore", playerInfo.userScore);
        PlayerPrefs.SetInt("score", playerInfo.currentScore);
        PlayerPrefs.SetInt("numMoves", playerInfo.currentNumMoves);
        ScoreData.currentNumMoves = playerInfo.currentNumMoves;
        ScoreData.level = FirebaseInit.playerInfo.currentLevel;
        Debug.Log("score: " + PlayerPrefs.GetInt("score"));
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
    public static void UpdateCurrentStatus(int currentScore, int currentLevel, int currentNumMoves)
    {
        if (FB.IsLoggedIn)
        {
            reference.Child("users").Child(FacebookController.facebookID).Child("currentScore").SetValueAsync(currentScore);
            reference.Child("users").Child(FacebookController.facebookID).Child("currentLevel").SetValueAsync(currentLevel);
            reference.Child("users").Child(FacebookController.facebookID).Child("currentNumMoves").SetValueAsync(currentNumMoves);
        }
        else if (PlayerPrefs.GetInt("Google") == 1)
        {
            reference.Child("users").Child(GoogleController.auth.CurrentUser.UserId).Child("currentScore").SetValueAsync(currentScore);
            reference.Child("users").Child(GoogleController.auth.CurrentUser.UserId).Child("currentLevel").SetValueAsync(currentLevel);
            reference.Child("users").Child(GoogleController.auth.CurrentUser.UserId).Child("currentNumMoves").SetValueAsync(currentNumMoves);
        }
        else
        {
            reference.Child("users").Child(guestID).Child("currentScore").SetValueAsync(currentScore);
            reference.Child("users").Child(guestID).Child("currentLevel").SetValueAsync(currentLevel);
            reference.Child("users").Child(guestID).Child("currentNumMoves").SetValueAsync(currentNumMoves);
        }
    }
    public static void UpdateLevel(int level)
    {
        if (FB.IsLoggedIn) reference.Child("users").Child(FacebookController.facebookID).Child("userHighestLevel").SetValueAsync(level);
        else if (PlayerPrefs.GetInt("Google") == 1) reference.Child("users").Child(GoogleController.auth.CurrentUser.UserId).Child("userHighestLevel").SetValueAsync(level);
        else reference.Child("users").Child(guestID).Child("userHighestLevel").SetValueAsync(level);
    }
    public static void UpdateScore(int score)
    {
        if (FB.IsLoggedIn) reference.Child("users").Child(FacebookController.facebookID).Child("userScore").SetValueAsync(score);
        else if(PlayerPrefs.GetInt("Google") == 1) reference.Child("users").Child(GoogleController.auth.CurrentUser.UserId).Child("userScore").SetValueAsync(score);
        else reference.Child("users").Child(guestID).Child("userScore").SetValueAsync(score);
    }
    public static void CreatePlayerInfo(string id, string name, string email, string profileURL)
    {
        playerInfo = new User(id, name, email, 0,  profileURL, 1, 1, 0, ScoreData.startingMoves);
        string json = JsonUtility.ToJson(playerInfo);
        reference.Child("users").Child(id).SetRawJsonValueAsync(json);
    }

}
