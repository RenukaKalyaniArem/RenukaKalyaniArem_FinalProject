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
        score = 0;
        currentLives = startingLives;
        UpdateHUD();              
        UpdateLevelHUD(); 
        Resume();
        SceneManager.LoadScene(firstLevelSceneName);
    }

    public void OnPlayerDestroyed()
    {
        StopSpawning();
        Pause();
        if (ui) ui.ShowDestroyed("Spaceship Destroyed");
    }
    public void OnShipLanded()
    {
        StopSpawning();

        int current = SceneManager.GetActiveScene().buildIndex;
        int total = SceneManager.sceneCountInBuildSettings;
        bool lastLevel = (current + 1 >= total);

        // Pause to display appropriate panel
        Pause();
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
    public void ContinueNextLevel()
    {
        Resume();
        int current = SceneManager.GetActiveScene().buildIndex;
        int total = SceneManager.sceneCountInBuildSettings;
        if (current + 1 < total)
        {
            SceneManager.LoadScene(current + 1);
        }
        else
        {
            OnShipLanded();
        }
    }
    public void RetryLevel()
    {
        Resume();
        currentLives = startingLives; // Reset lives for a clean game retry
        UpdateHUD();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void StartFromLevel1()
    {
        Resume();
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
    }

    void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
    }
}

    

