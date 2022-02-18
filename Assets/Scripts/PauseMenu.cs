using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Image pauseBackground;
    [SerializeField] private Text gamePaused;
    [SerializeField] private Button resumeGame;
    [SerializeField] private Button returnToMainMenu;
    //[SerializeField] private GameObject levelManager;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject player;
    //[SerializeField] private GameObject playerHealth;
    private bool isPaused = false;

    //activate the pause menu and enable assets.
    public void ActivateMenu()
    {
        Time.timeScale = 0;
        isPaused = true;
        pauseBackground.gameObject.SetActive(true);
        gamePaused.gameObject.SetActive(true);
        resumeGame.gameObject.SetActive(true);
        returnToMainMenu.gameObject.SetActive(true);
        player.GetComponent<PlayerMovement>().SetUI(false, false);
        //playerHealth.SetActive(false);
    }

    // Start the game
    public void ResumeGame()
    {
        Time.timeScale = 1;
        isPaused = false;
        pauseBackground.gameObject.SetActive(false);
        gamePaused.gameObject.SetActive(false);
        resumeGame.gameObject.SetActive(false);
        returnToMainMenu.gameObject.SetActive(false);
        //playerHealth.SetActive(true);
        player.GetComponent<PlayerMovement>().SetUI(true, false);
        if ((LevelManager.Instance.GetCurrentSpawn() < 9) && (LevelManager.Instance.GetCurrentSpawn() > 5))
            player.GetComponent<PlayerMovement>().SetUI(true, true);
    }

    // Return to the main menu
    public void ReturnToLMainMenu()
    {
        Time.timeScale = 1;
        isPaused = false;
        if (CutsceneHandler.Instance && CutsceneHandler.Instance.isDoneTransitioning)
        {
            StartCoroutine(TransitionToMainMenuFromPauseMenu());
        }
        
        /*levelManager.GetComponent<LevelManager>().ResetPlayer();
        pauseBackground.gameObject.SetActive(false);
        gamePaused.gameObject.SetActive(false);
        resumeGame.gameObject.SetActive(false);
        returnToMainMenu.gameObject.SetActive(false);
        mainMenu.GetComponent<MainMenu>().ActivateMenu();
        player.GetComponent<PlayerMovement>().enabled = false;*/
    }

    public IEnumerator TransitionToMainMenuFromPauseMenu()
    {
        StartCoroutine(CutsceneHandler.Instance.FadeInAndOutBlackOutSquare());
        yield return new WaitForSeconds(1);
        pauseBackground.gameObject.SetActive(false);
        gamePaused.gameObject.SetActive(false);
        resumeGame.gameObject.SetActive(false);
        returnToMainMenu.gameObject.SetActive(false);
        mainMenu.GetComponent<MainMenu>().ActivateMenu();
        player.GetComponent<PlayerMovement>().enabled = false;
        StoryProgressionHandler.Instance.ResetAllScenes();
        //StartCoroutine(CutsceneHandler.Instance.FadeBlackOutSquare(false));
        yield return null;
    }

    //returns boolean indicating whether or not the game is paused.
    public bool Paused()
    {
        return isPaused;
    }
}
