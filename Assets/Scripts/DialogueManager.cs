using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
	public static DialogueManager Instance { get; protected set; } //instance of PlayerMovement that can be accessed by other scripts easily
	//public Text nameText;
	public Text dialogueText;
	private int index;
	private int chapterNumber;
	public Dialogue[] chapterOneDialogues;
	public Dialogue[] chapterTwoDialogues;
	public Dialogue[] chapterThreeDialogues;
	public Dialogue[] endingDialogues;
	//public Animator animator;
	public GameObject nextButton;
	private string currentSentence;
	private Queue<string> sentences;
	public bool inCutscene;

    private void Awake()
    {
		Instance = this;
		inCutscene = false;

	}

	public void ResetCutSceneIndex()
    {
		index = 0;
    }
    void Update()
    {
        if(dialogueText.text == currentSentence && inCutscene)
        {
			nextButton.SetActive(true);
        }
        else
        {
			nextButton.SetActive(false);
		}
	}
    // Use this for initialization
    void Start()
	{
		sentences = new Queue<string>();
	}
	public void SetChapterNumber(int newChapterNumber)
	{
		chapterNumber = newChapterNumber;
	}

	public void StartDialogue(int startingIndex)
	{
		//animator.SetBool("IsOpen", true);

		//nameText.text = dialogue.name;
		inCutscene = true;
		sentences.Clear();

		switch (chapterNumber)
		{
			case 1:
					foreach (string sentence in chapterOneDialogues[startingIndex].sentences)
					{
						sentences.Enqueue(sentence);
					}
				break;
			case 2:
				foreach (string sentence in chapterTwoDialogues[startingIndex].sentences)
				{
					sentences.Enqueue(sentence);
				}
				break;
			case 3:
				foreach (string sentence in chapterThreeDialogues[startingIndex].sentences)
				{
					sentences.Enqueue(sentence);
				}
				break;
			case 4:
				foreach (string sentence in endingDialogues[startingIndex].sentences)
				{
					sentences.Enqueue(sentence);
				}
				break;
		}

		DisplayNextSentence();
	}

	public void DisplayNextSentence()
	{
		if (sentences.Count == 0)
		{
			EndDialogue();
			return;
		}

		currentSentence = sentences.Dequeue();
		StopAllCoroutines();
		StartCoroutine(TypeSentence(currentSentence));
	}

	IEnumerator TypeSentence(string sentence)
	{
		dialogueText.text = "";
		foreach (char letter in sentence.ToCharArray())
		{
			dialogueText.text += letter;
			yield return null;
		}
	}

	void EndDialogue()
	{
		switch (chapterNumber)
		{
			case 1:
				if (index < chapterOneDialogues.Length - 1) //minus one because we want that last scene to stay while we fade the screen to black and back again
				{
					index++;
					StartDialogue(index);
                }
				break;
			case 2:
				if (index < chapterTwoDialogues.Length - 1) //minus one because we want that last scene to stay while we fade the screen to black and back again
				{
					index++;
					StartDialogue(index);
				}
				break;
			case 3:
				if (index < chapterThreeDialogues.Length - 1) //minus one because we want that last scene to stay while we fade the screen to black and back again
				{
					index++;
					StartDialogue(index);
				}
				break;
			case 4:
				if (index < endingDialogues.Length - 1) //minus one because we want that last scene to stay while we fade the screen to black and back again
				{
					index++;
					StartDialogue(index);
				}
				break;
		}

		
		StoryProgressionHandler.Instance.TransitionToNextImage();
		//animator.SetBool("IsOpen", false);
	}

}