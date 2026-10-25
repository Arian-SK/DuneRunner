using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Main game manager: speed, score, menu, pause, game over and high score saving
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Speed")]
    public float startSpeed = 6f;
    public float maxSpeed = 12f;
    public float speedIncreasePerSecond = 0.1f;

    [Header("Score")]
    public float scorePerUnit = 2f;
    public float gameOverPanelDelay = 1f;   // wait so the death animation can be seen

    public float BaseSpeed { get; private set; }
    public float SpeedMultiplier { get; set; } = 1f;   // dash sets this above 1
    public bool IsPlaying { get; private set; }
    public bool IsPaused { get; private set; }
    public int Score => Mathf.FloorToInt(score);
    public int HighScore { get; private set; }

    // The speed that all other scripts use to move the world (0 in menu / game over)
    public float CurrentSpeed => IsPlaying ? BaseSpeed * SpeedMultiplier : 0f;

    const string HighScoreKey = "HighScore";

    // Static, so it survives a scene reload.
    // True after "Restart": the game starts right away without showing the main menu.
    static bool skipMenu;

    float score;
    InputAction pauseAction;

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
        BaseSpeed = startSpeed;
        HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);

        // Pause input using the new Input System
        pauseAction = new InputAction("Pause", InputActionType.Button, "<Keyboard>/escape");
        pauseAction.performed += ctx => TogglePause();
    }

    void OnEnable() { pauseAction.Enable(); }
    void OnDisable() { pauseAction.Disable(); }



    void Start()
    {
        if (skipMenu)
        {
            // Came from Restart: go straight into the game
            StartGame();
        }
        else
        {
            // Fresh launch (or back from menu): show the main menu, the game waits
            UIManager.Instance.ShowMainMenu(HighScore);
            AudioManager.Instance.PlayMenuMusic();   // NEW
        }
        skipMenu = false;
    }

    void Update()
    {
        if (!IsPlaying) return;

        // Game gets faster over time, up to maxSpeed
        BaseSpeed = Mathf.Min(BaseSpeed + speedIncreasePerSecond * Time.deltaTime, maxSpeed);
        score += CurrentSpeed * scorePerUnit * Time.deltaTime;

        // High score updates live while playing
        if (Score > HighScore) HighScore = Score;
    }


    // Called by the Play button in the main menu
    public void StartGame()
    {
        IsPlaying = true;
        IsPaused = false;
        Time.timeScale = 1f;
        UIManager.Instance.HideMainMenu();
        AudioManager.Instance.PlayGameMusic();   // NEW
    }

    public void AddScore(int points)
    {
        score += points;
    }

    public void TogglePause()
    {
        if (!IsPlaying) return;   // cannot pause in the menu or after game over
        IsPaused = !IsPaused;
        Time.timeScale = IsPaused ? 0f : 1f;
        UIManager.Instance.ShowPause(IsPaused);
        AudioManager.Instance.PlayPause();                  // NEW
        AudioManager.Instance.SetMusicPaused(IsPaused);     // NEW
    }

    public void GameOver()
    {
        if (!IsPlaying) return;
        IsPlaying = false;
        SaveHighScore();
        AudioManager.Instance.StopMusic();   // NEW
        Invoke(nameof(ShowGameOverUI), gameOverPanelDelay);
    }

    void ShowGameOverUI()
    {
        UIManager.Instance.ShowGameOver(Score, HighScore);
        AudioManager.Instance.PlayGameOver();   // NEW
    }

    // Reload the scene and start playing immediately
    public void Restart()
    {
        skipMenu = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Reload the scene and show the main menu
    public void LoadMenu()
    {
        skipMenu = false;
        Time.timeScale = 1f;
        SaveHighScore();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        SaveHighScore();
        Application.Quit();   // only works in a build, not inside the Editor
    }

    // Save the high score using PlayerPrefs
    void SaveHighScore()
    {
        PlayerPrefs.SetInt(HighScoreKey, HighScore);
        PlayerPrefs.Save();
    }

    void OnApplicationQuit()
    {
        SaveHighScore();
    }
}