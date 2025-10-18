using UnityEngine;
using UnityEngine.SceneManagement;
public class LandingPadGate : MonoBehaviour
{
 

    public GameObject landingPadRoot;

    public bool autoByBuildIndex = true;

    public int requiredScore = 0; // used when autoByBuildIndex is false

    [Header("Per-Level Scores")]
    public int level1Score = 100;
    public int level2Score = 300;
    public int level3Score = 500;

    private GameManager gm;
    private bool unlocked = false;

    private void Start()
    {
        if (landingPadRoot == null) landingPadRoot = gameObject;
        gm = GameManager.Instance != null ? GameManager.Instance : FindAnyObjectByType<GameManager>();

        if (autoByBuildIndex)
        {
            int idx = SceneManager.GetActiveScene().buildIndex; // Main Menu typically 0
            if (idx == 1) requiredScore = level1Score;
            else if (idx == 2) requiredScore = level2Score;
            else if (idx == 3) requiredScore = level3Score;
        }

        unlocked = requiredScore <= 0;
        if (landingPadRoot) landingPadRoot.SetActive(unlocked);
    }

    private void Update()
    {
        if (unlocked || gm == null) return;
        if (gm.score >= requiredScore)
        {
            unlocked = true;
            if (landingPadRoot) landingPadRoot.SetActive(true);
        }
    }
}


