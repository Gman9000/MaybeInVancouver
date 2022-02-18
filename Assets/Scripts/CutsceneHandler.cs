using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

public class CutsceneHandler : MonoBehaviour
{
    public static CutsceneHandler Instance { get; protected set; } //instance of CutsceneHandler that can be accessed by other scripts easily
    public SpriteRenderer blackOutSquare;
    public bool isDoneTransitioning;
    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeBlackOutSquare(false));
        
    }

    public IEnumerator FadeBlackOutSquare(bool fadeToBlack = true, int fadeSpeed = 1)
    {
        Color objectColor = blackOutSquare.color;
        float fadeAmount;
        isDoneTransitioning = false;
        if (fadeToBlack)
        {
            
            while (blackOutSquare.color.a < 1)
            {
                fadeAmount = objectColor.a + (fadeSpeed * Time.deltaTime);

                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                blackOutSquare.color = objectColor;
                yield return null;
            }
            isDoneTransitioning = true;
        }
        else
        {
            while (blackOutSquare.color.a > 0)
            {
                fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);

                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                blackOutSquare.color = objectColor;
                yield return null;
            }
            isDoneTransitioning = true;
        }
    }

    //fadeSpeed is 1 so i can remove the parameter for fungus
    public IEnumerator FadeInAndOutBlackOutSquare()
    {
        //we will always fade to black first and then fade back out later
        Color objectColor = blackOutSquare.color;
        float fadeAmount;
        isDoneTransitioning = false;
        while (blackOutSquare.color.a < 1)
        {
            fadeAmount = objectColor.a + (Time.deltaTime);
            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            blackOutSquare.color = objectColor;
            yield return null;
        }
        while (blackOutSquare.color.a > 0)
        {
            fadeAmount = objectColor.a - (Time.deltaTime);
            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            blackOutSquare.color = objectColor;
            yield return null;
        }
        isDoneTransitioning = true;
        yield return null;

    }
}
