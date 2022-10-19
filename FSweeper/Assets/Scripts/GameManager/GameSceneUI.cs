using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameSceneUI : MonoBehaviour
{
    private static bool isPaused;
    [SerializeField] private GameObject RetryMenu;
    [SerializeField] private GameObject WinMenu;
    [SerializeField] private GameObject timerUI;
    private TextMeshProUGUI timerText;
    public static float timer;
    private bool isUpdated;

    private const float TIMER_COUNT_DELAY = .2f;
    private void Awake()
    {
        timerText = timerUI.GetComponent<TextMeshProUGUI>();
        isUpdated = true;
        isPaused = false;
        GameManager.OnBombNodeClicked += UI_OpenLoseMenu;
        GameManager.OnWinSweeper += UI_OpenWinMenu;
        RetryMenu.SetActive(false);
        WinMenu.SetActive(false);
    }

    private void Update()
    {
        if (!isPaused)
        {
            timer += Time.deltaTime;
            if (isUpdated)
            {
                StartCoroutine(UpdateTimerUICoroutine());
            }
        }
    }

    private IEnumerator UpdateTimerUICoroutine()
    {
        isUpdated = false;
        timerText.text = (Mathf.Round(timer * 100f) / 100f).ToString();
        yield return new WaitForSeconds(TIMER_COUNT_DELAY);
        isUpdated = true;

    }
    public static bool IsPaused()
    {
        return isPaused;
    }
    private void UI_OpenWinMenu(object sender, EventArgs e)
    {
        if (WinMenu != null)
        {
            OpenMenu(WinMenu);
        }else
        {
            Debug.LogWarning("No Win Scene Object");
        }
    }

    private void UI_OpenLoseMenu(object sender, GameManager.OnSweeperNodeChangedEventArgs e)
    {
        if (RetryMenu != null)
        {
            OpenMenu(RetryMenu);
        }
        else
        {
            Debug.LogWarning("No Retry Scene Object");
        }
    }

    public void TogglePauseMenu(GameObject pauseMenuGameObject)
    {
        if (isPaused)
        {
            isPaused = true;
            pauseMenuGameObject.SetActive(false);
        }
        else
        {
            isPaused = false;
            pauseMenuGameObject.SetActive(true);
        }
    }

    public void OpenMenu(GameObject menuGameObject)
    {
        isPaused = true;
        menuGameObject.SetActive(true);
    }

    public void CloseMenu(GameObject menuGameObject)
    {
        isPaused = false;
        menuGameObject.SetActive(false);
    }
    public void Retry()
    {
        timer = 0;
        CloseMenu(RetryMenu);
        GameManager.ReloadSweeper();
    }

    public void ReplayGame()
    {
        timer = 0;
        CloseMenu(WinMenu);
        GameManager.ReloadSweeper();
    }

    public void LoadGameScene()
    {
        SceneManagerUtils.LoadGameScene();
    }

    public void LoadHomeScene()
    {
        SceneManagerUtils.LoadHomeScene();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
