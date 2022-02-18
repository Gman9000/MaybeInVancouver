using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryProgressionHandler : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject thirdBoss;
    [SerializeField] private GameObject nextButton;
    //[SerializeField] private GameObject playerHealth;

    public static StoryProgressionHandler Instance { get; protected set; } //instance of StoryProgressionHandler that can be accessed by other scripts easily

    public Image[] chapterOneScenes;
    public Image[] chapterTwoScenes;
    public Image[] chapterThreeScenes;
    public Image[] endingScenes;

    //public Text[] chapterOneText;
    //public Text[] chapterTwoText;
    //public Text[] chapterThreeText;
    //public Text[] endingText;
    //make sure the length of both the text and image containers are the same thanks!

    public GameObject chapterOneContainer;
    public GameObject chapterTwoContainer;
    public GameObject chapterThreeContainer;
    public GameObject chapterEndContainer;
    public GameObject dialogueBoxContainer;
    private int index;
    private int chapterNumber;

    public bool isDoneTransitioning;


    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        index = 0;
        isDoneTransitioning = true;
    }

    public void TransitionToNextImage()
    {
        if (isDoneTransitioning)
        {
            switch (chapterNumber)
            {
                case 1:
                    if (index < chapterOneScenes.Length-1) //minus one because we want that last scene to stay while we fade the screen to black and back again
                    {
                        StartCoroutine(FadeOutScene(chapterOneScenes[index]));
                        //StartCoroutine(FadeOutText(chapterOneText[index]));
                        index++;
                       
                        //Debug.Log("Chapter scenes.length" + chapterOneScenes.Length);
                    }
                    else
                    {
                        index = 0;
                        DialogueManager.Instance.inCutscene = false;
                        DialogueManager.Instance.ResetCutSceneIndex();
                        nextButton.SetActive(false);
                        StartCoroutine(TransitionToMainGameFromCutscene());
                    }
                    break;
                case 2:
                    if (index < chapterTwoScenes.Length - 1) //minus one because we want that last scene to stay while we fade the screen to black and back again
                    {
                        StartCoroutine(FadeOutScene(chapterTwoScenes[index]));
                        //StartCoroutine(FadeOutText(chapterOneText[index]));
                        index++;

                        //Debug.Log("Chapter scenes.length" + chapterOneScenes.Length);
                    }
                    else
                    {
                        index = 0;
                        DialogueManager.Instance.inCutscene = false;
                        DialogueManager.Instance.ResetCutSceneIndex();
                        nextButton.SetActive(false);
                        StartCoroutine(TransitionToMainGameFromCutscene());
                    }
                    //StartCoroutine(FadeOutScene(chapterTwoScenes[index]));
                    break;
                case 3:
                    if (index < chapterThreeScenes.Length - 1) //minus one because we want that last scene to stay while we fade the screen to black and back again
                    {
                        StartCoroutine(FadeOutScene(chapterThreeScenes[index]));
                        //StartCoroutine(FadeOutText(chapterOneText[index]));
                        index++;

                        //Debug.Log("Chapter scenes.length" + chapterOneScenes.Length);
                    }
                    else
                    {
                        index = 0;
                        DialogueManager.Instance.inCutscene = false;
                        DialogueManager.Instance.ResetCutSceneIndex();
                        nextButton.SetActive(false);
                        StartCoroutine(TransitionToMainGameFromCutscene());
                    }
                    //StartCoroutine(FadeOutScene(chapterThreeScenes[index]));
                    break;
                case 4:
                    if (index < endingScenes.Length - 1) //minus one because we want that last scene to stay while we fade the screen to black and back again
                    {
                        StartCoroutine(FadeOutScene(endingScenes[index]));
                        //StartCoroutine(FadeOutText(chapterOneText[index]));
                        index++;

                        //Debug.Log("Chapter scenes.length" + chapterOneScenes.Length);
                    }
                    else
                    {
                        index = 0;
                        DialogueManager.Instance.inCutscene = false;
                        DialogueManager.Instance.ResetCutSceneIndex();
                        nextButton.SetActive(false);
                        StartCoroutine(TransitionToMainGameFromCutscene());
                    }
                    StartCoroutine(FadeOutScene(endingScenes[index]));
                    break;
            }
        }
    }

    public IEnumerator TransitionToMainGameFromCutscene()
    {
        StartCoroutine(CutsceneHandler.Instance.FadeInAndOutBlackOutSquare());
        yield return new WaitForSeconds(1);
        chapterOneContainer.SetActive(false);
        chapterTwoContainer.SetActive(false);
        chapterThreeContainer.SetActive(false);
        chapterEndContainer.SetActive(false);
        dialogueBoxContainer.SetActive(false);
        player.GetComponent<PlayerMovement>().enabled = true;
        player.GetComponent<PlayerMovement>().SetUI(true, false);
        //playerHealth.SetActive(true);

        if (LevelManager.Instance.GetCurrentSpawn() == 6) player.GetComponent<PlayerMovement>().SetUI(true, true);
        if (LevelManager.Instance.GetCurrentSpawn() == 9) thirdBoss.GetComponent<ThirdBoss>().StartChase();
    }


    public void EnableCutsceneChapter()
    {
        nextButton.SetActive(true);
        dialogueBoxContainer.SetActive(true);
        DialogueManager.Instance.SetChapterNumber(chapterNumber);
        DialogueManager.Instance.StartDialogue(0);
        switch (chapterNumber)
        {
            case 1:
                chapterOneContainer.SetActive(true);
                chapterTwoContainer.SetActive(false);
                chapterThreeContainer.SetActive(false);
                chapterEndContainer.SetActive(false);
                break;
            case 2:
                chapterOneContainer.SetActive(false);
                chapterTwoContainer.SetActive(true);
                chapterThreeContainer.SetActive(false);
                chapterEndContainer.SetActive(false); 
                break;
            case 3:
                chapterOneContainer.SetActive(false);
                chapterTwoContainer.SetActive(false);
                chapterThreeContainer.SetActive(true);
                chapterEndContainer.SetActive(false); 
                break;
            case 4:
                chapterOneContainer.SetActive(false);
                chapterTwoContainer.SetActive(false);
                chapterThreeContainer.SetActive(false);
                chapterEndContainer.SetActive(true); 
                break;
        }
    }

    public void SetChapterNumber(int newChapterNumber)
    {
        chapterNumber = newChapterNumber;
    }

    public void ResetAllScenes()
    {
        foreach (Image img in chapterOneScenes)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, 1);
        }
        foreach (Image img in chapterTwoScenes)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, 1);
        }
        foreach (Image img in chapterThreeScenes)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, 1);
        }
        foreach (Image img in endingScenes)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, 1);
        }

        /*foreach (Text img in chapterOneText)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, 1);
        }
        foreach (Text img in chapterTwoText)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, 1);
        }
        foreach (Text img in chapterThreeText)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, 1);
        }
        foreach (Text img in endingText)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, 1);
        }*/
    }

    public IEnumerator FadeOutScene(Image sceneToFadeOut, int fadeSpeed = 1)
    {
        isDoneTransitioning = false;
        Color objectColor = sceneToFadeOut.color;
        float fadeAmount;
        while (sceneToFadeOut.color.a > 0)
        {
            fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            sceneToFadeOut.color = objectColor;
            yield return null;
        }
        isDoneTransitioning = true;

    }

    /*public IEnumerator FadeOutText(Text textToFadeOut, int fadeSpeed = 1)
    {
        isDoneTransitioning = false;
        Color objectColor = textToFadeOut.color;
        float fadeAmount;
        while (textToFadeOut.color.a > 0)
        {
            fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            textToFadeOut.color = objectColor;
            yield return null;
        }
        isDoneTransitioning = true;
    }*/
}
