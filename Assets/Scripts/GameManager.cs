using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class GameManager : MonoBehaviour
{
    public int startingLives = 5;
    [HideInInspector] public int currentLives;

    public UIManager ui; 

    void Awake()
    {
        currentLives = startingLives;
        if (ui == null) ui = FindAnyObjectByType<UIManager>();
        UpdateUI();
    }

    public void ChangeLives(int delta)
    {
        currentLives += delta;
        currentLives = Mathf.Max(0, currentLives);
        UpdateUI();
    }

    void UpdateUI()
    {
        if (ui) ui.SetLives(currentLives);
    }

    public void GameOver(string reason)
    {
        Debug.Log("GameOver: " + reason);
        if (ui) ui.ShowGameOver();

        SpawnManager sp = FindAnyObjectByType<SpawnManager>();
        if (sp != null)
            sp.StopSpawning();
            StartCoroutine(RestartAfterDelay(2f));
    }

    IEnumerator RestartAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LevelComplete()
    {
        StartCoroutine(LevelCompleteRoutine());
    }
    IEnumerator LevelCompleteRoutine()
    {

        yield return new WaitForSeconds(1.0f);
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        int totalScenes = SceneManager.sceneCountInBuildSettings;


        if (currentScene + 1 < totalScenes)
        {

            SceneManager.LoadScene(currentScene + 1);
        }
        else
        {

            SceneManager.LoadScene(0);
        }
        yield break; 
    
    }

}
