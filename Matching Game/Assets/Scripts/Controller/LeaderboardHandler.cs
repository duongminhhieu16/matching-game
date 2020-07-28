
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using UnityEngine.UI;


public class LeaderboardHandler : MonoBehaviour
{
    //firebase database initialization
    public List<TextMeshProUGUI> usersBarList = new List<TextMeshProUGUI>();
    public TextMeshProUGUI userBarPrefab;

    private Transform usersPlacePosition;
    public Transform firstUserPosition;

    public const int MAX_USERS_LEADERBOARD = 5;

    public GameObject leaderBoardScreen;


    private void Awake()
    {
        float yPos = firstUserPosition.transform.localPosition.y;
        for (int i = 0; i < MAX_USERS_LEADERBOARD; i++)
        {
            TextMeshProUGUI userBar = Instantiate(userBarPrefab);
            userBar.transform.SetParent(transform);
            userBar.GetComponent<RectTransform>().localScale = new Vector2(1.0f, 1.0f);
            userBar.transform.localPosition = new Vector2(0, yPos);
            yPos -= 120;
            usersBarList.Add(userBar);
        }
    }

    private void Start()
    {

    }

    public void OpenLeaderBoard()
    {
        StartCoroutine(LoadDataFromServer());
    }

    private IEnumerator LoadDataFromServer()
    {
        FirebaseInit.loaded = false;
        FirebaseInit.LoadHighestScoreUsersInfo(MAX_USERS_LEADERBOARD);
        while (!FirebaseInit.loaded)
        {
            yield return new WaitForEndOfFrame();
        }
        UpdateDataToLeaderBoard();
        Invoke("ShowLeaderBoard", 1.0f);
        yield return null;
    }

    private void ShowLeaderBoard()
    {
        leaderBoardScreen.SetActive(true);
        float waitTime = 0;
        for (int i = 0; i < MAX_USERS_LEADERBOARD; i++)
        {
            StartCoroutine(usersBarList[i].GetComponent<UserBar>().StartAppear(waitTime));
            waitTime += 0.1f;
        }
    }

    public void CloseLeaderBoard()
    {
        leaderBoardScreen.SetActive(false);
    }

    private void UpdateDataToLeaderBoard()
    {
        long scoreFront = 0;
        int rankCurrent = 0;
        int i = 0;
        foreach (User userInfo in FirebaseInit.users)
        {
            if (i != 0)
            {
                if (userInfo.userScore == scoreFront)
                {
                    userInfo.SetRank(rankCurrent);
                }
                else
                {
                    rankCurrent = i + 1;
                    userInfo.SetRank(rankCurrent);
                    scoreFront = userInfo.userScore;
                }
            }
            else
            {
                scoreFront = userInfo.userScore;
                rankCurrent = 1;
                userInfo.SetRank(rankCurrent);
            }
            usersBarList[i].GetComponent<UserBar>().SetUser(userInfo);
            i++;
        }
    }
}