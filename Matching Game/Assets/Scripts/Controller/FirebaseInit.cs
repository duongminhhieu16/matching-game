using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Boo.Lang;
using System.Threading.Tasks;

public class FirebaseInit : MonoBehaviour
{
    public static DatabaseReference reference;
    public static string playerID;
    public static int highscoreOfUser;
    public static User playerInfo;
    public static List<User> users = new List<User>();
    
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
            playerID = SystemInfo.deviceUniqueIdentifier;
        }
        // check if this user exists
        FirebaseDatabase.DefaultInstance.GetReference("users").Child(playerID)
            .GetValueAsync().ContinueWith(task =>
            {
                DataSnapshot d = task.Result;
                User u = JsonUtility.FromJson<User>(d.GetRawJsonValue());
                if (u == null)
                    CreatePlayerInfo();
            });
    }
    
    public static async Task LoadScoreOfSpecificUser()
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
                        if (playerID == us.id)
                        {
                            playerInfo = us;
                            highscoreOfUser = us.userScore;
                        }
                    }
                }
            });
        
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
        if (highscoreOfUser <= score)
        {
            highscoreOfUser = score;
        }
        reference.Child("users").Child(playerID).Child("userScore").SetValueAsync(highscoreOfUser);
    }
    public static void CreatePlayerInfo()
    {
        playerInfo = new User("name", "email@gmail.com", 0, playerID);
        string json = JsonUtility.ToJson(playerInfo);
        reference.Child("users").Child(playerID).SetRawJsonValueAsync(json);
    }
    
}
