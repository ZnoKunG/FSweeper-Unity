using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static int dimension;
    private string currentUserName;
    private UI_InputWindow inputWindow;

    private void Awake()
    {
        currentUserName = PlayerPrefs.GetString("playerName");
        DatabaseAccess.SetUrl("https://fsweeperapi-znokung.azurewebsites.net/api/leaderboard");
        inputWindow = transform.Find("Input").GetComponent<UI_InputWindow>();
        inputWindow.Hide();
        if (currentUserName == "") {
            Debug.Log(PlayerPrefs.GetString("playerName"));
            inputWindow.Show();
        }
    }

    public void SetSweeperDimensionAndStartGame(int _dimension)
    {
        dimension = _dimension;
        LoadGameScene();
    }

    public void LoadGameScene()
    {
        SceneManagerUtils.LoadGameScene();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
