using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    public Text textDisplay;
    public string[] sentences;
    public float typingSpeed;

    public GameObject continueButton;
    public GameObject exitButton;
    public GameObject noButton;
    public GameObject getDressButton;

    private int index = 0;

    IEnumerator TypeSentence(string sentence)
    {
        ClearText();
        HideContinueButton();
        foreach (char letter in sentence.ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        ShowContinueButton();
    }

    public void Reset() {
        StopTyping();
        ClearText();
        HideOptionButtons();
        PointToFirstSentence();
    }

    public void NextSentences()
    {
        if (NoMoreSentences())
            return;

        if (PointingToLastSentence()) {
            ShowOptionButtons();
            HideContinueButton();
        }
        
        StartTypingPointedSentence();
        PointToNextSentence();
    }

    public void CloseThroughGameMaster() {
        PlaygroundMaster.Instance.HideNpcDialogue();
    }

    private void PointToFirstSentence() {
        index = 0;
    }
    private void PointToNextSentence() {
        index++;
    }

    private void ClearText() {
        textDisplay.text = "";
    }

    private void StopTyping() {
        StopAllCoroutines();
    }

    private void StartTypingPointedSentence() {
        StopTyping();
        ClearText();
        StartCoroutine(TypeSentence(sentences[index]));
    }

    private bool PointingToLastSentence() {
        return index == sentences.Length - 1; // intentionally minus 1
    }

    private bool NoMoreSentences() {
        return index >= sentences.Length;
    }

    private void HideContinueButton() {
        continueButton.SetActive(false);
    }

    private void ShowContinueButton() {
        continueButton.SetActive(true);
    }

    private void ShowOptionButtons() {
        exitButton.SetActive(true);
        noButton.SetActive(true);
        getDressButton.SetActive(true);
    }

    private void HideOptionButtons() {
        exitButton.SetActive(false);
        noButton.SetActive(false);
        getDressButton.SetActive(false);

    }
}
