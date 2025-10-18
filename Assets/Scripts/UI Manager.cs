using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public TMP_Text titleText;
    public Button playLevel1Button;
    public TMP_Text instructionsText;

    public GameObject hudPanel;
    public TMP_Text livesText;
    public TMP_Text scoreText;
    public TMP_Text levelText;

    public GameObject successPanel;
    public TMP_Text successMessageText;
    public Button nextLevelButton;

    public GameObject destroyedPanel;
    public TMP_Text destroyedMessageText;
    public Button retryButton;

    public GameObject completePanel;
    public TMP_Text completeMessageText;
    public Button playFromLevel1Button;
    public TMP_Text totalScoreText; 

    void Start()
    {
        HideAllOverlays();

        if (mainMenuPanel != null && titleText != null)
        {
            mainMenuPanel.SetActive(true);
            if (hudPanel) hudPanel.SetActive(false);
        }
        else
        {
            if (hudPanel) hudPanel.SetActive(true);
        }

    }

    public void HideAllOverlays()
    {
        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        if (hudPanel) hudPanel.SetActive(false);
        if (successPanel) successPanel.SetActive(false);
        if (destroyedPanel) destroyedPanel.SetActive(false);
        if (completePanel) completePanel.SetActive(false);
    }

    public void SetLives(int lives)
    {
        if (livesText) livesText.text = "Lives: " + lives;
    }

    public void SetScore(int score)
    {
        if (scoreText) scoreText.text = "Score: " + score;
    }
    public void SetLevelLabel(string label)
    {
        if (levelText) levelText.text = label;
    }

    public void ShowSuccessfulLanding(string msg)
    {
        if (successMessageText) successMessageText.text = msg;
        if (successPanel) successPanel.SetActive(true);
        if (hudPanel) hudPanel.SetActive(false);
    }

    public void ShowDestroyed(string msg)
    {
        if (destroyedMessageText) destroyedMessageText.text = msg;
        if (destroyedPanel) destroyedPanel.SetActive(true);
        if (hudPanel) hudPanel.SetActive(false);
    }

    public void ShowAllComplete(string msg)
    {
        if (completeMessageText) completeMessageText.text = msg;
        if (totalScoreText) totalScoreText.text = "Total Score: " + GameManager.Instance.score;
        if (completePanel) completePanel.SetActive(true);
        if (hudPanel) hudPanel.SetActive(false);
    }

    public void OnClickPlayLevel1()
    {
        if (GameManager.Instance) GameManager.Instance.StartGame();
    }
    public void OnClickContinueNextLevel()
    {
        if (GameManager.Instance) GameManager.Instance.ContinueNextLevel(); 
    }
    public void OnClickRetry()             { if (GameManager.Instance) GameManager.Instance.RetryLevel(); }
    public void OnClickStartFromLevel1()   { if (GameManager.Instance) GameManager.Instance.StartFromLevel1(); }
}


    
