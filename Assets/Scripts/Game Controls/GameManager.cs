using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverCanvas;
    public GameObject gameInterface;
    public GameObject Nimbus;

    public static bool isGamePaused = false;

    // public Tutorial tutorialObject;
    public HashSet<string> tutorialLevels = new HashSet<string>{"Stage 1-1"};
    public static bool isTutorial = false;

    // Status constants
    public const String STATUS_JUMP = "Jumping";
    public const String STATUS_REST = "Resting";
    public const String STATUS_GAMECLEAR = "GameClear";
    public const String STATUS_GAMEOVER = "GameOver";
    public const string STATUS_TUTORIAL = "Tutorial";

    private void Start()
    {
        InitializeGame();
    }

    /*
        Game initializes 
    */
    public void InitializeGame()
    {
        // Set Canvas
        gameInterface.SetActive(true);
        gameOverCanvas.SetActive(false);

        // Reset time scale
        Time.timeScale = 1;

        // Reset jump count
        NimbusJump.jumpCount = 0;

        // Set Game Status
        if(IsTutorialStage(DetectCurrentLevel()))
        {
            SetTutorialGameStatus();
            // tutorialObject.ChooseTutorial(DetectCurrentLevel());
        }
        else
        {
            SetRestGameStatus();
        }
        NimbusEvents.TriggerOnGameStart();
        PlayBGM();
    }

    // Setters functions
    public void SetRestGameStatus()
    {
        PlayerPrefs.SetString("Status", STATUS_REST);
    }

    public void SetTutorialGameStatus()
    {
        PlayerPrefs.SetString("Status", STATUS_TUTORIAL);
    }

    public string DetectCurrentLevel()
    {
        return SceneManager.GetActiveScene().name;
    }

    public bool IsTutorialStage(string level)
    {
        if(tutorialLevels.Contains(level))
        {
            isTutorial = true;
            return true;
        }
        else
        {
            return false;
        }
    }

    /*
        Game Restarts from game over state which calls initialize game.
        This function is often times part of OnClick on PlayAgain Button in different canvas.
    */
    public void RestartGame()
    {
        Debug.Log("Restart Game");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        var status = PlayerPrefs.GetString("Status");
        if(status == STATUS_GAMEOVER)
        {
            InitializeGame();
        }
        else
        {
            Debug.LogError($"The current game status doesn't allow RestartGame(). Current status is {status}");
        }

        NimbusEvents.TriggerOnGameStart();
    }

    /*
        Successfully finishes the stage before countdown hits 0
    */
    public void GameCleared()
    {
        Debug.Log("Game Cleared!");

        Time.timeScale = 0f;
        gameInterface.SetActive(false);
        // Score.CalculateFinalScore();
        UnlockNewLevel();
    }

    /*
        Unsuccessfully finishing the stage either because the countdown hit 0 before getting to top,
        or picked wrong direction to jump.
    */
    public void GameOver()
    {
        Debug.Log("Game Over!");
        PlayerPrefs.SetString("Status", "GameOver");
        Time.timeScale = 0f;
        gameInterface.SetActive(false);
        gameOverCanvas.SetActive(true);
        NimbusEvents.TriggerOnGameEnd();
    }

    private void UnlockNewLevel()
    {
        if(SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachedIndex"))
        {
            PlayerPrefs.SetInt("ReachedIndex", SceneManager.GetActiveScene().buildIndex + 1);
            PlayerPrefs.SetInt("UnlockedLevel", PlayerPrefs.GetInt("UnlockedLevel", 1) + 1);
            PlayerPrefs.Save();
        }
    }

    public void PlayBGM(){
        AudioManager.Instance.PlayMusic("BGM");
    }
}