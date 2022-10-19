using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_InputWindow : MonoBehaviour
{

    private bool isOpened;
    private TextMeshProUGUI titleText;
    private TMP_InputField inputField;
    private TextMeshProUGUI popUpText;

    private void Awake()
    {
        isOpened = false;
        titleText = transform.Find("titleText").GetComponent<TextMeshProUGUI>();
        popUpText = transform.Find("popUpText").GetComponent<TextMeshProUGUI>();
        inputField = transform.Find("inputField").GetComponent<TMP_InputField>();
        Hide();
    }

    public void Toggle()
    {
        if (isOpened) Hide();
        else Show();
    }
    public void Show()
    {
        isOpened = true;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        isOpened = false;
        gameObject.SetActive(false);
    }

    public void SubmitName()
    {
        string inputName = inputField.text;
        if (inputName != "")
        {
            DatabaseAccess.AddPlayer(new Player { name = inputName, score = 999f }, (string error) => {
                Debug.Log(error);
                popUpText.text = error;
            }, (string success) => {
                Debug.Log(success);
                popUpText.text = success;
                PlayerPrefs.SetString("playerName", inputName);
                Hide();
            });
        }
    }
}
