using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
   public Text livesText;
    public GameObject gameOverPanel;

void Start()
{
    if (gameOverPanel) gameOverPanel.SetActive(false);
}

    public void SetLives(int lives)
    {
        if (livesText) livesText.text = "Lives: " + lives;
    }

    public void ShowGameOver()
    {
        if (gameOverPanel) gameOverPanel.SetActive(true);
    }
}
