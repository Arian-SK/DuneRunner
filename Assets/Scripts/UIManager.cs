using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Handles all UI: main menu, score, hearts, pause menu and game over panel
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Main Menu")]
    public GameObject mainMenuPanel;
    public TMP_Text menuHighScoreText;

    [Header("HUD (hidden while in the main menu)")]
    public GameObject hudPanel;       // parent object of the score texts and hearts
    public TMP_Text scoreText;
    public TMP_Text highScoreText;

    [Header("Hearts")]
    public Image[] hearts;       // three Image objects
    public Sprite fullHeart;     // full heart sprite
    public Sprite emptyHeart;    // empty heart sprite (optional)

    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public TMP_Text finalScoreText;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    void Update()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null) return;

        // Score and high score are shown live
        scoreText.text = "Score: " + gm.Score;
        highScoreText.text = "High Score: " + gm.HighScore;
    }

    // Show the main menu and hide the HUD
    public void ShowMainMenu(int highScore)
    {
        mainMenuPanel.SetActive(true);
        hudPanel.SetActive(false);
        menuHighScoreText.text = "High Score: " + highScore;
    }

    // Hide the main menu and show the HUD
    public void HideMainMenu()
    {
        mainMenuPanel.SetActive(false);
        hudPanel.SetActive(true);
    }

    // One lit heart for each remaining life, the rest are turned off
    public void UpdateHealth(int lives, int maxLives)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            bool alive = i < lives;

            if (fullHeart != null && emptyHeart != null)
            {
                // If both sprites are assigned, swap between them
                hearts[i].sprite = alive ? fullHeart : emptyHeart;
                hearts[i].color = Color.white;
            }
            else
            {
                // Otherwise, a lost heart becomes dark and transparent
                hearts[i].color = alive ? Color.white : new Color(0.2f, 0.2f, 0.2f, 0.4f);
            }
        }
    }

    public void ShowPause(bool show)
    {
        pausePanel.SetActive(show);
    }

    public void ShowGameOver(int score, int highScore)
    {
        gameOverPanel.SetActive(true);
        finalScoreText.text = "Score: " + score + "\nHigh Score: " + highScore;
    }

    // These are connected to the buttons' OnClick events
    // Plays the button click sound
    void PlayClick()
    {
        AudioManager.Instance.PlayButton();
    }

    // These are connected to the buttons' OnClick events
    public void OnPlayClicked() { PlayClick(); GameManager.Instance.StartGame(); }
    public void OnQuitClicked() { PlayClick(); GameManager.Instance.QuitGame(); }
    public void OnResumeClicked() { PlayClick(); GameManager.Instance.TogglePause(); }
    public void OnRestartClicked() { PlayClick(); GameManager.Instance.Restart(); }
    public void OnMenuClicked() { PlayClick(); GameManager.Instance.LoadMenu(); }
}