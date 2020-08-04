
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Facebook.Unity;

public class LeaderboardHandler : MonoBehaviour
{
    //firebase database initialization
    public List<TextMeshProUGUI> nameList = new List<TextMeshProUGUI>();
    public List<TextMeshProUGUI> scoreList = new List<TextMeshProUGUI>();
    public GameObject previousPageButton;
    public GameObject nextPageButton;
    public GameObject backToMenuButton;

    public TextMeshProUGUI namePrefab;
    public TextMeshProUGUI scorePrefab;
    private static int page = 0;
    private static int userNumEachPage = 5;
    private int rank_num = 0;
    public Transform firstUserPosition;

    public const int MAX_USERS_LEADERBOARD = 100;


    private async void Awake()
    {
        page = 0;
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
            yPos -= 100;
            scoreList.Add(scoreText);
            nameList.Add(nameText);
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
            if(FB.IsLoggedIn) 
                if (FacebookController.facebookID == FirebaseInit.users[rank_num - rank - 1].id) nameList[cnt].text = rank + 1 + ". YOU";
                else nameList[cnt].text = rank + 1 + ". " + FirebaseInit.users[rank_num - rank - 1].userName;
            else
                if (FirebaseInit.guestID == FirebaseInit.users[rank_num - rank - 1].id) nameList[cnt].text = rank + 1 + ". YOU";
                else nameList[cnt].text = rank + 1 + ". " + FirebaseInit.users[rank_num - rank - 1].userName;
            scoreList[cnt].text = FirebaseInit.users[rank_num - rank - 1].userScore.ToString();
            cnt++;
            if (cnt == 5) break;
        }
        for(int i = cnt; i < userNumEachPage; i++)
        {
            nameList[i].text = "";
            scoreList[i].text = "";
        }
    }
    public void NextPage()
    {
        if(rank_num - (page+1) * userNumEachPage > 0) page++;
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