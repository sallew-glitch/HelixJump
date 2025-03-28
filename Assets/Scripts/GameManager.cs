using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    AudioManager audioManager;

    public static bool gameOver;
    public static bool levelWin;

    public GameObject gameOverPanel;
    public GameObject levelWinPanel;
    public GameObject mistakePanel;
    public GameObject pausePanel;
    public GameObject healthPanel;

    private bool mute;
    public Image volume;
    public Sprite muted;
    public Sprite unmuted;

    public List<Image> hearts;

    public Sprite heartFill;
    public Sprite heartEmpty;

    public static int currentLives;
    //public static int maxLives = 5;

    public static int currentLevelIndex;
    public static int noOfPassingRings;

    public TextMeshProUGUI currentLevelText;
    public TextMeshProUGUI nextLevelText;
    //public TextMeshProUGUI healthText;
    public TextMeshProUGUI mistakeText;

    public Slider progressBar;

    public List<TextMeshProUGUI> names;
    public List<TextMeshProUGUI> numCoins;

    private void Awake()
    {
        currentLevelIndex = PlayerPrefs.GetInt("CurrentLevelIndex", 1);
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void Start()
    {
        Time.timeScale = 1f;
        noOfPassingRings = 0;
        mute = PlayerPrefs.GetInt("Mute") == 1 ? true : false;

        Debug.Log("mute : " + mute);

        if (mute)
        {
            volume.sprite = muted;
        }
        else
        {
            volume.sprite = unmuted;
        }

        gameOver = false;
        levelWin = false;
        currentLives = 3;
        //healthText.text = currentLives.ToString();
        Debug.Log("Total Coins : " + PlayerPrefs.GetInt("TotalCoins"));
    }

    private void Update()
    {

        if (currentLives == 0)
        {
            gameOver = true;
            GameOver();
        }

        currentLevelText.text = currentLevelIndex.ToString();
        nextLevelText.text = (currentLevelIndex + 1).ToString();
    }

    public void ringPassed()
    {
        noOfPassingRings++;
        int progress = noOfPassingRings * 100 / FindObjectOfType<HelixManager>().noOfRings;
        progressBar.value = progress;
    }

    public void Hit()
    {
        addOrReduceLives(-1);
        //healthText.text = currentLives.ToString();

        if (currentLives != 0)
        {
            //wait for 3 seconds, text on screen 3 2 1
            Time.timeScale = 0;
            mistakePanel.SetActive(true);
            StartCoroutine(Waiter());

            audioManager.Play("Mistake");
        }
    }

    IEnumerator Waiter()
    {
        mistakeText.text = "3";
        yield return new WaitForSecondsRealtime(1);
        mistakeText.text = "2";
        yield return new WaitForSecondsRealtime(1);
        mistakeText.text = "1";
        yield return new WaitForSecondsRealtime(1);
        Time.timeScale = 1;
        mistakePanel.SetActive(false);
        yield return new WaitForSecondsRealtime(1);
    }

    public void Win() 
    {
        int totalCoins = PlayerPrefs.GetInt("TotalCoins") + CoinsManager.instance.score;
        PlayerPrefs.SetInt("TotalCoins", totalCoins);
        FirebaseManager.instance.UpdateCoins(totalCoins);
        levelWinPanel.SetActive(true);
        levelWin = true;
        audioManager.Play("LevelWin");
        FirebaseManager.instance.GetLeaderboardData();
    }

    public void NextLevel()
    {
        PlayerPrefs.SetInt("CurrentLevelIndex", currentLevelIndex + 1);
        SceneManager.LoadScene(1);
    }

    private void GameOver()
    {
        Time.timeScale = 0;
        gameOverPanel.SetActive(true);
        audioManager.Play("GameOver");
        if (Input.GetMouseButton(1))
        {
            SceneManager.LoadScene(1);
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        mistakePanel.SetActive(true);
        StartCoroutine(Waiter());
    }

    public void GotoMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
        FirebaseManager.instance.AssignUIElements();
    }

    public void MuteUnmute()
    {
        if (mute)
        {
            volume.sprite = unmuted;
        }
        else
        {
            volume.sprite = muted;
        }

        AudioListener.pause = !mute;

        mute = !mute;

        PlayerPrefs.SetInt("Mute", mute ? 1 : 0);

        Debug.Log("mute : " + mute);
    }

    public void addOrReduceLives(int n)
    {
        currentLives = Mathf.Min(3, currentLives + n);

        updateHearts();
    }

    public void updateHearts()
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            hearts[i].sprite = i < currentLives ? heartFill : heartEmpty;
        }
    }
}
