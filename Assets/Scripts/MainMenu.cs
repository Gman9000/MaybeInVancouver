using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Image menuBackground;
    [SerializeField] private Text title;
    [SerializeField] private Button startGame;
    [SerializeField] private Button quitGame;
    [SerializeField] private GameObject player;

    //Activate the main menu and enable assets.
    public void ActivateMenu()
    {
        menuBackground.gameObject.SetActive(true);
        title.gameObject.SetActive(true);
        startGame.gameObject.SetActive(true);
        quitGame.gameObject.SetActive(true);
    }

    // Start the game
    public void StartGame()
    {
        /*if (StoryProgressionHandler.Instance && CutsceneHandler.Instance && CutsceneHandler.Instance.isDoneTransitioning)
        {
            StartCoroutine(TransitionToMainGame());
        }*/
        if (CutsceneHandler.Instance && CutsceneHandler.Instance.isDoneTransitioning)
        {
            StartCoroutine(TransitionToMainGame());
        }
        /*player.GetComponent<PlayerMovement>().enabled = true;
        menuBackground.gameObject.SetActive(false);
        title.gameObject.SetActive(false);
        startGame.gameObject.SetActive(false);
        quitGame.gameObject.SetActive(false);*/
    }

    public IEnumerator TransitionToMainGame()
    {
        StartCoroutine(CutsceneHandler.Instance.FadeInAndOutBlackOutSquare());
        yield return new WaitForSeconds(1);
        menuBackground.gameObject.SetActive(false);
        title.gameObject.SetActive(false);
        startGame.gameObject.SetActive(false);
        quitGame.gameObject.SetActive(false);
        LevelManager.Instance.Disableplayer();
        LevelManager.Instance.storyFlowChart.ExecuteBlock("Chapter1");
        //StoryProgressionHandler.Instance.SetChapterNumber(1); //this will always be set to 1 from the main menu
        //StoryProgressionHandler.Instance.EnableCutsceneChapter();
        //StartCoroutine(CutsceneHandler.Instance.FadeBlackOutSquare(false));
        //player.GetComponent<PlayerMovement>().enabled = true;
        yield return null;

    }


    //quit the game
    public void QuitGame()
    {
        Application.Quit();
    }
}
