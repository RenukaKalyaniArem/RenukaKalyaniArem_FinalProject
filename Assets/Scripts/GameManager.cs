using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance; 
    public int startingLives = 5;
    public int currentLives;
    public int score;
    public string firstLevelSceneName = "Level1";
    public int firstLevelBuildIndex = 1;
    public UIManager ui;
    public bool isPaused = false;
    public bool isGameEnded = false;

    void Awake() 
    {
        if (Instance != null && Instance != this) 
         {
            Destroy(gameObject);
            return;
         }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        currentLives = startingLives;
        score = 0;

        if (ui == null) ui = FindAnyObjectByType<UIManager>();
        UpdateHUD();
        UpdateLevelHUD(); 

        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (ui == null) ui = FindAnyObjectByType<UIManager>();
        UpdateHUD();
        UpdateLevelHUD(); 
        Resume();
    }
    void UpdateHUD()
    {
        if (ui != null)
        {
            ui.SetLives(currentLives);
            ui.SetScore(score);
        }
    }
    void UpdateLevelHUD()
    {
        if (ui == null) return;
        int idx = SceneManager.GetActiveScene().buildIndex;
        if (idx < firstLevelBuildIndex)
        {
            ui.SetLevelLabel(""); // hides level text on menus
            return;
        }
        int levelNumber = (idx - firstLevelBuildIndex) + 1;
        ui.SetLevelLabel("Level: " + levelNumber);
    }

    public void ChangeLives(int delta)
    {
        currentLives = Mathf.Max(0, currentLives + delta);
        UpdateHUD();
        if (currentLives <= 0)
        {
            OnPlayerDestroyed();
        }
    }
    public void AddScore(int points)
    {
        score = Mathf.Max(0, score + points);
        if (ui) ui.SetScore(score);
      
    }
    public void StartGame()
    {
        isGameEnded = false;
        score = 0;
        currentLives = startingLives;
        UpdateHUD();              
        UpdateLevelHUD(); 
        Resume();
        SceneManager.LoadScene(firstLevelSceneName);
    }

    public void OnPlayerDestroyed()
    {
        isGameEnded = true;
        StopSpawning();
        Resume();
        if (ui) {
        ui.HidePausePanel(); 
        ui.ShowDestroyed("Spaceship Destroyed");
        }
    }
    public void OnShipLanded()
    {
        isGameEnded = true;
        StopSpawning();
        var ship = FindAnyObjectByType<PlayerController>();
        if (ship != null)
        {
            ship.FreezeMovement(2f); // Freeze controls for 2 seconds
            StartCoroutine(FreezeCompletelyAfterDelay(ship, 2f)); // Keep frozen after 2s and until next level
        }
        int current = SceneManager.GetActiveScene().buildIndex;
        int total = SceneManager.sceneCountInBuildSettings;
        bool lastLevel = (current + 1 >= total);

        Resume();
        StartCoroutine(ShowPanelAfterDelay(lastLevel));
    }
    IEnumerator ShowPanelAfterDelay(bool lastLevel)
    {
        yield return new WaitForSecondsRealtime(2f);

        if (ui)
        {
            if (lastLevel)
            {
                ui.ShowAllComplete("Congratulations for successfully completing all levels");
            }
            else
            {
                ui.ShowSuccessfulLanding("Successful Landing");
            }
        }
    }

    private IEnumerator FreezeCompletelyAfterDelay(PlayerController ship, float delay) // NEW
    {
        yield return new WaitForSecondsRealtime(delay);
        if (ship != null)
            ship.SetControlsEnabled(false); // Remain frozen until next scene/retry
    }

    public void ContinueNextLevel() {
        isGameEnded = false;
        Resume();
        var ship = FindAnyObjectByType<PlayerController>();
        if (ship != null)
            ship.SetControlsEnabled(true); // UNFREEZE controls for next game
        int current = SceneManager.GetActiveScene().buildIndex;
        int total = SceneManager.sceneCountInBuildSettings;
        if (current + 1 < total) {
            SceneManager.LoadScene(current + 1);
        } else {
            OnShipLanded();
        }
    }
    public void RetryLevel() 
    {
        isGameEnded = false;
        Resume();
        var ship = FindAnyObjectByType<PlayerController>();
        if (ship != null)
            ship.SetControlsEnabled(true); // UNFREEZE controls for retry
        currentLives = startingLives;
        UpdateHUD();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void StartFromLevel1() 
    {
        isGameEnded = false;
        Resume();
        var ship = FindAnyObjectByType<PlayerController>();
        if (ship != null)
            ship.SetControlsEnabled(true); // UNFREEZE controls for restart
        score = 0;
        currentLives = startingLives;
        UpdateHUD();
        SceneManager.LoadScene(firstLevelSceneName);
    }
     void StopSpawning()
    {
        var sp = FindAnyObjectByType<SpawnManager>();
        if (sp) sp.StopSpawning();
    }

    void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (ui) ui.ShowPausePanel();
    }

    void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
         if (ui) ui.HidePausePanel();
    }
    void Update()
    {
        if (!isGameEnded && currentLives > 0) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                if (isPaused) {
                    Resume();
                } else {
                    Pause();
                }
            }
        }
    }

}

    

