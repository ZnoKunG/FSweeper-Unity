using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneManagerUtils
{
    private static string HOME_SCENE_NAME = "HomeScene";
    private static string GAME_SCENE_NAME = "GameScene";
    public static void LoadHomeScene()
    {
        SceneManager.LoadScene(HOME_SCENE_NAME);
    }

    public static void LoadGameScene()
    {
        SceneManager.LoadScene(GAME_SCENE_NAME);
    }

    public static void ReloadGameScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public static bool isGameScene()
    {
        return SceneManager.GetActiveScene().name == GAME_SCENE_NAME;
    }
}
