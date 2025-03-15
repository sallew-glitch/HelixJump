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

    //public Image heart;
    public Sprite heartFill;
    public Sprite heartEmpty;

    public static int currentLives;
    public static int maxLives = 5;

    public static int currentLevelIndex;
    public static int noOfPassingRings;

    public TextMeshProUGUI currentLevelText;
    public TextMeshProUGUI nextLevelText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI mistakeText;

    public Slider progressBar;

    private void Awake()
    {
        currentLevelIndex = PlayerPrefs.GetInt("CurrentLevelIndex", 1);
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void Start()
    {
        Time.timeScale = 1f;
        noOfPassingRings = 0;
        gameOver = false;
        levelWin = false;
        currentLives = 3;
        healthText.text = currentLives.ToString();
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

        //CoinsManager.instance.SpinCoins();
    }

    public void ringPassed()
    {
        noOfPassingRings++;
        int progress = noOfPassingRings * 100 / FindObjectOfType<HelixManager>().noOfRings;
        progressBar.value = progress;
    }

    public void Hit()
    {
        currentLives--;
        healthText.text = currentLives.ToString();

        if (currentLives != 0)
        {
            //wait for 3 seconds, text on screen 3 2 1
            Time.timeScale = 0;
            mistakePanel.SetActive(true);
            StartCoroutine(Waiter());

            audioManager.Play("Mistake");
        }

        updateHearts();
    }

    IEnumerator Waiter()
    {
        mistakeText.text = "3";
        Debug.Log("3");
        yield return new WaitForSecondsRealtime(1);
        mistakeText.text = "2";
        Debug.Log("2");
        yield return new WaitForSecondsRealtime(1);
        mistakeText.text = "1";
        Debug.Log("1");
        yield return new WaitForSecondsRealtime(1);
        Time.timeScale = 1;
        mistakePanel.SetActive(false);
        yield return new WaitForSecondsRealtime(1);
    }

    public void Win() 
    {
        levelWinPanel.SetActive(true);
        levelWin = true;
        audioManager.Play("LevelWin");
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
    }

    private void updateHearts()
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            hearts[i].sprite = i < currentLives ? heartFill : heartEmpty;
        }
    }
}
