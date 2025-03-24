using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public bool mute;
    public Button volume;
    public Sprite muted;
    public Sprite unmuted;

    public GameObject OptionsPanel;
    public GameObject LoginPanel;
    public GameObject MainMenuPanel;
    public GameObject ProfilePanel;

    public TMP_Text totalCoins;

    public FirebaseAuth auth;

    public void Awake()
    {
        mute = PlayerPrefs.GetInt("Mute") == 1 ? true : false;
    }

    private void Start()
    {
        mute = PlayerPrefs.GetInt("Mute") == 1 ? true : false;

        Debug.Log("mute : " + mute);

        if (mute)
        {
            volume.image.sprite = muted;
        }
        else
        {
            volume.image.sprite = unmuted;
        }

        totalCoins.text = PlayerPrefs.GetInt("TotalCoins").ToString();
    }

    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void MuteUnmute()
    {
        if (mute)
        {
            volume.image.sprite = unmuted;
        }
        else
        {
            volume.image.sprite = muted;
        }

        AudioListener.pause = !mute;

        mute = !mute;

        PlayerPrefs.SetInt("Mute", mute ? 1 : 0);

        Debug.Log("mute : " + mute);
    }

    public void OptionsOpen()
    {
        MainMenuPanel.SetActive(false);
        OptionsPanel.SetActive(true);
    }

    public void OptionsClose()
    {
        OptionsPanel.SetActive(false);
        MainMenuPanel.SetActive(true);
    }

    public void LoginOrProfile()
    {
        auth = FirebaseAuth.DefaultInstance;
        if (auth.CurrentUser != null)
        {
            ProfileOpen();
        } else
        {
            LoginOpen();
        }
    }

    public void ProfileOpen()
    {
        MainMenuPanel.SetActive(false);
        ProfilePanel.SetActive(true);
    }

    public void LoginOpen()
    {
        MainMenuPanel.SetActive(false);
        LoginPanel.SetActive(true);
    }

    public void LoginClose()
    {
        LoginPanel.SetActive(false);
        MainMenuPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
