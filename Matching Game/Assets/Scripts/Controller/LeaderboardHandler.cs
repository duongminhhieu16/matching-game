using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;
using Facebook.Unity;
using UnityEngine.Networking;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;

public class LeaderboardHandler : MonoBehaviour
{
    //firebase database initialization
    public List<TextMeshProUGUI> nameList = new List<TextMeshProUGUI>();
    public List<TextMeshProUGUI> scoreList = new List<TextMeshProUGUI>();
    public List<GameObject> profilePicList = new List<GameObject>();
    public GameObject previousPageButton;
    public GameObject nextPageButton;
    public GameObject backToMenuButton;

    public TextMeshProUGUI namePrefab;
    public TextMeshProUGUI scorePrefab;
    public GameObject profilePicPrefab;
    public Sprite defaultProfilePic;
    private Texture2D texture;
    private static int page = 0;
    private static int userNumEachPage = 5;
    private int rank_num = 0;
    public Transform firstUserPosition;
    public static FirebaseAuth auth;
    public const int MAX_USERS_LEADERBOARD = 100;


    private async void Awake()
    {
        texture = defaultProfilePic.texture;
        page = 0;
        auth = FirebaseAuth.DefaultInstance;
        await LoadDataFromServer();
        float yPos = firstUserPosition.transform.localPosition.y;
        for (int i = 0; i < 5; i++)
        {
           
            TextMeshProUGUI nameText = Instantiate(namePrefab);
            nameText.transform.SetParent(transform);
            nameText.GetComponent<RectTransform>().localScale = new Vector2(1.0f, 1.0f);
            nameText.transform.localPosition = new Vector2(namePrefab.transform.localPosition.x, yPos);

            TextMeshProUGUI scoreText = Instantiate(scorePrefab);
            scoreText.transform.SetParent(transform);
            scoreText.GetComponent<RectTransform>().localScale = new Vector2(1.0f, 1.0f);
            scoreText.transform.localPosition = new Vector2(scorePrefab.transform.localPosition.x, yPos);

            GameObject image = Instantiate(profilePicPrefab);
            image.transform.SetParent(transform);
            image.GetComponent<RectTransform>().localScale = new Vector2(1.0f, 1.0f);
            image.transform.localPosition = new Vector2(profilePicPrefab.transform.localPosition.x, yPos);

            yPos -= 100;
            scoreList.Add(scoreText);
            nameList.Add(nameText);
            profilePicList.Add(image);
        }
        UpdateDataToLeaderBoard();
    }

    

    private async Task LoadDataFromServer()
    {
        await FirebaseInit.LoadHighestScoreUsersInfo(MAX_USERS_LEADERBOARD);
        rank_num = FirebaseInit.users.Count;
    }
    private void UpdateDataToLeaderBoard()
    {
        int cnt = 0;
        //each page display 5 users
        for (int rank = userNumEachPage*page; rank < rank_num; rank++)
        {
            
            profilePicList[cnt].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            if (FB.IsLoggedIn)
            {
                if (FacebookController.facebookID == FirebaseInit.users[rank_num - rank - 1].id) nameList[cnt].text = rank + 1 + ". YOU";
                else nameList[cnt].text = rank + 1 + ". " + FirebaseInit.users[rank_num - rank - 1].userName;
                
            }
            else if (PlayerPrefs.GetInt("Google") == 1)
            {
                if (GoogleController.auth.CurrentUser.UserId == FirebaseInit.users[rank_num - rank - 1].id) nameList[cnt].text = rank + 1 + ". YOU";
                else nameList[cnt].text = rank + 1 + ". " + FirebaseInit.users[rank_num - rank - 1].userName;
                
            }
            else
            {
                if (FirebaseInit.guestID == FirebaseInit.users[rank_num - rank - 1].id) nameList[cnt].text = rank + 1 + ". YOU";
                else nameList[cnt].text = rank + 1 + ". " + FirebaseInit.users[rank_num - rank - 1].userName;
            }
            StartCoroutine(DisplayPicture(FirebaseInit.users[rank_num - rank - 1].profileURL, profilePicList[cnt]));
            scoreList[cnt].text = FirebaseInit.users[rank_num - rank - 1].userScore.ToString();
            Debug.Log(texture);
            cnt++;
            if (cnt == 5) break;
        }
        for(int i = cnt; i < userNumEachPage; i++)
        {
            nameList[i].text = "";
            scoreList[i].text = "";
            profilePicList[i].GetComponent<Image>().sprite = defaultProfilePic;
            profilePicList[i].GetComponent<Image>().color = new Color32(255, 255, 255, 0);
        }
    }
    IEnumerator DisplayPicture(string url, GameObject image)
    {
        if (url == "")
        {
            Image pic = image.GetComponent<Image>();
            pic.sprite = defaultProfilePic;
            Debug.Log(url);
        }
        else
        {
            yield return DownloadImage(url);
            Image pic = image.GetComponent<Image>();
            pic.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2());
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
            texture = DownloadHandlerTexture.GetContent(request);
        }
    }
    public void NextPage()
    {
        if (rank_num - (page + 1) * userNumEachPage > 0)
        {
            page++;
            previousPageButton.SetActive(true);
            backToMenuButton.SetActive(false);
        }
        UpdateDataToLeaderBoard();
    }
    public void PreviousPage()
    {
        if (page > 0)
        {
            page--;
            UpdateDataToLeaderBoard();
        }
        if (page == 0)
        {
            previousPageButton.SetActive(false);
            backToMenuButton.SetActive(true);
        }
    }
}