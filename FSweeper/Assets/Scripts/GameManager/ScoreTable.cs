using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using MongoDB.Bson;

public class ScoreTable : MonoBehaviour
{
    private Transform entryContainer;
    private Transform entryTemplate;
    private List<Transform> highScoreEntryTransformList = new List<Transform>();

    private bool isOpened;
    private void Awake()
    {
        entryContainer = transform.Find("ScoreEntryContainer");
        entryTemplate = entryContainer.Find("ScoreEntryTemplate");
        isOpened = false;

        entryTemplate.gameObject.SetActive(false);

        DatabaseAccess.GetAllPlayers((string error) =>
        {
            Debug.Log("Error: " + error);
        }, (List<Player> playerList) => {
            int lastShowedRank = 8;
            for (int i = 0; i < playerList.Count; i++) {
                CreateHighScoreEntryTransform(playerList[i], entryContainer, highScoreEntryTransformList);
                if (i + 1 >= lastShowedRank) break;
            }
        });
    }

    public void Toggle()
    {
        if (!isOpened) Show();
        else
        {
            Hide();
        }
    }
    public void Show()
    {
        gameObject.SetActive(true);
        UpdateScoreboard();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void UpdateScoreboard()
    {
        DatabaseAccess.GetAllPlayers((string error) =>
        {
            Debug.Log("Error: " + error);
        }, (List<Player> playerList) => {
            for (int i = 0; i < highScoreEntryTransformList.Count; i++) {
                UpdateEntryScore(highScoreEntryTransformList[i], playerList[i]);
            }
        });
    }

    private void UpdateEntryScore(Transform entryTransform, Player player)
    {
        Color textColor = Color.white;
        float score = player.score;
        entryTransform.Find("scoreText").GetComponent<TextMeshProUGUI>().text = score.ToString();
        entryTransform.Find("scoreText").GetComponent<TextMeshProUGUI>().color = textColor;

        string name = player.name;
        entryTransform.Find("nameText").GetComponent<TextMeshProUGUI>().text = name;
        entryTransform.Find("nameText").GetComponent<TextMeshProUGUI>().color = textColor;
    }

    private void CreateHighScoreEntryTransform(Player player, Transform container, List<Transform> transformList)
    {
        float templateHeight = 25f;
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        int rank = transformList.Count + 1;
        Color textColor = Color.white;
        string rankString;
        switch (rank)
        {
            default:
                rankString = rank + "TH";
                break;
            case 1:
                {
                    rankString = "1ST";
                    textColor = Color.green;
                };
                break;
            case 2:
                rankString = "2ND";
                break;
            case 3:
                rankString = "3RD";
                break;
        }

        entryTransform.Find("rankText").GetComponent<TextMeshProUGUI>().text = rankString;
        entryTransform.Find("rankText").GetComponent<TextMeshProUGUI>().color = textColor;

        float score = player.score;
        entryTransform.Find("scoreText").GetComponent<TextMeshProUGUI>().text = score.ToString();
        entryTransform.Find("scoreText").GetComponent<TextMeshProUGUI>().color = textColor;

        string name = player.name;
        entryTransform.Find("nameText").GetComponent<TextMeshProUGUI>().text = name;
        entryTransform.Find("nameText").GetComponent<TextMeshProUGUI>().color = textColor;

        transformList.Add(entryRectTransform);
    }

}

